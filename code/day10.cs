

using System.Threading.Tasks.Dataflow;

using MoreLinq;

public class Day10
{
    public const string Day = "10";

    public static List<(string, int[][], int[])> Input()
    {
        return File.ReadLines($"input/2025_{Day}_input.txt")
            .Where(l => !string.IsNullOrEmpty(l))
            .Select(line =>
                line.Split([']', '{'], StringSplitOptions.TrimEntries)
                    .Fold((a, b, c) => (
                        a[1..],
                        b.Split(' ', StringSplitOptions.TrimEntries)
                            .Select(x => x[1..^1])
                            .Select(_ => _.Split(',')
                                .Select(x => int.Parse(x))
                                .ToArray())
                            .ToArray(),
                        c[..^1].Split(',', StringSplitOptions.TrimEntries)
                            .Select(x => int.Parse(x))
                            .ToArray()
                        )))
            .ToList();
    }

    public static object Solve1()
    {
        List<(string lights, int[][] buttons, int[] joltage)> input = Input();
        int maxlen = -1;

        List<uint> allLights = new();
        List<List<uint>> allButtons = new();

        foreach (var inp in input)
        {
            // lights
            var pos = 0;
            uint light = 0;
            foreach (var l in inp.lights)
            {
                if (l == '#')
                    light |= (1u << pos);
                pos++;
            }
            allLights.Add(light);

            // buttons
            List<uint> buttons = new();
            foreach (var b in inp.buttons)
            {
                uint button = 0u;
                foreach (var l in b)
                {
                    button |= (1u << l);
                }
                buttons.Add(button);
            }
            allButtons.Add(buttons);
        }


        if (allLights.Count() != allButtons.Count())
            throw new Exception();

        List<int> presses = new();

        for (int i = 0; i < allLights.Count(); i++)
        {
            var lights = allLights[i];
            IEnumerable<IList<uint>> buttonSet = allButtons[i].Subsets();

            foreach (IList<uint>? buttonSubset in buttonSet.Skip(1))
            {
                uint value = buttonSubset.First();
                foreach (var button in buttonSubset.Skip(1))
                {
                    value ^= button;
                }

                if (value == lights)
                {
                    presses.Add(buttonSubset.Count);
                    goto next;
                }

            }

        next:;
        }

        return presses.Sum();
    }

    public static object Solve2()
    {
        List<(string lights, int[][] buttons, int[] joltage)> input = Input();

        long totalMinPresses = 0;

        foreach (var inp in input)
        {
            Console.WriteLine(inp);
            int maxb = -1;

            foreach (var button in inp.buttons)
            {
                foreach (var b in button)
                {
                    if (b > maxb)
                        maxb = b;
                }
            }

            var unknowns = inp.buttons.Length;
            var equations = inp.joltage.Length;

            double[,] matrix = new double[equations, unknowns];
            double[] rightHandSide = new double[equations];

            for (int eq = 0; eq < equations; eq++)
            {
                rightHandSide[eq] = inp.joltage[eq];
            }
            for (int b = 0; b < unknowns; b++)
            {
                foreach (var counter in inp.buttons[b])
                {
                    matrix[counter, b] = 1d;
                }
            }

            double[,] augmented = new double[equations, unknowns + 1];
            for (int i = 0; i < equations; i++)
            {
                for (int j = 0; j < unknowns; j++)
                    augmented[i, j] = matrix[i, j];
                augmented[i, unknowns] = rightHandSide[i];
            }
            var rref = new RREF(augmented);
            var pivots = rref.PivotColumns;
            var freevariables = rref.FreeVariableIndices;

            // Use it to calculate solutions:


            // Heuristic, search over all free variables to find the lowest aggregate key presses
            long maxPresses = inp.joltage.Max();
            long minPresses = int.MaxValue;
            if (freevariables.Count == 0)
            {
                int[] freeValues = { };
                int[] solution = rref.CalculateFromTransformationMatrix(freeValues);
                if (rref.ValidateSolution(freeValues, solution))
                {
                    var presses = solution.Sum();
                    if (presses < minPresses)
                        minPresses = presses;
                }
            }
            else
            if (freevariables.Count == 1)
            {
                for (int i = 0; i < maxPresses; i++)
                {
                    int[] freeValues = { i };
                    int[] solution = rref.CalculateFromTransformationMatrix(freeValues);
                    if (rref.ValidateSolution(freeValues, solution))
                    {
                        var presses = solution.Sum();
                        if (presses < minPresses)
                            minPresses = presses;
                    }
                }
            }

            else if (freevariables.Count == 2)
            {
                for (int i = 0; i < maxPresses; i++)
                    for (int j = 0; j < maxPresses; j++)
                        {
                            int[] freeValues = { i, j };
                            int[] solution = rref.CalculateFromTransformationMatrix(freeValues);
                            if (rref.ValidateSolution(freeValues, solution))
                            {
                                var presses = solution.Sum();
                                if (presses < minPresses)
                                    minPresses = presses;
                            }
                        }
            }

            else if (freevariables.Count == 3)
            {
                for (int i = 0; i < maxPresses; i++)
                    for (int j = 0; j < maxPresses; j++)
                        for (int k = 0; k < maxPresses; k++)
                        {
                            int[] freeValues = { i, j, k };
                            int[] solution = rref.CalculateFromTransformationMatrix(freeValues);
                            if (rref.ValidateSolution(freeValues, solution))
                            {
                                var presses = solution.Sum();
                                if (presses < minPresses)
                                    minPresses = presses;
                            }
                        }
            }
            else
            {
                throw new Exception($"Free variables = {freevariables.Count}");
            }
            totalMinPresses += minPresses;
        }

        return totalMinPresses;
    }
}

// Based on https://github.com/accord-net/framework/blob/development/Sources/Accord.Math/Matrix/ReducedRowEchelonForm.cs
public class RREF
{

    private double[,] rref;
    private int rows;
    private int cols;

    private int[] pivot;
    private int? freeCount;
    private int[]? _pivotColumns;
    private List<int>? freeVariableIndices;


    /// <summary>
    ///   Reduces a matrix to reduced row Echelon form.
    /// </summary>
    /// 
    /// <param name="value">The matrix to be reduced.</param>
    /// <param name="inPlace">
    ///   Pass <see langword="true"/> to perform the reduction in place. The matrix
    ///   <paramref name="value"/> will be destroyed in the process, resulting in less
    ///   memory consumption.</param>
    ///   
    public RREF(double[,] value, bool inPlace = false)
    {
        if (value == null)
            throw new ArgumentNullException("value");

        rref = inPlace ? value : (double[,])value.Clone();

        int lead = 0;
        rows = rref.GetLength(0);
        cols = rref.GetLength(1);

        pivot = new int[rows];
        for (int i = 0; i < pivot.Length; i++)
            pivot[i] = i;


        for (int r = 0; r < rows; r++)
        {
            if (cols <= lead)
                break;

            int i = r;

            while (rref[i, lead] == 0)
            {
                i++;

                if (i >= rows)
                {
                    i = r;

                    if (lead < cols - 1)
                        lead++;
                    else break;
                }
            }

            if (i != r)
            {
                // Swap rows i and r
                for (int j = 0; j < cols; j++)
                {
                    var temp = rref[r, j];
                    rref[r, j] = rref[i, j];
                    rref[i, j] = temp;
                }

                // Update indices
                {
                    var temp = pivot[r];
                    pivot[r] = pivot[i];
                    pivot[i] = temp;
                }
            }

            // Set to reduced row echelon form
            var div = rref[r, lead];
            if (div != 0)
            {
                for (int j = 0; j < cols; j++)
                    rref[r, j] /= div;
            }

            for (int j = 0; j < rows; j++)
            {
                if (j != r)
                {
                    var sub = rref[j, lead];
                    for (int k = 0; k < cols; k++)
                        rref[j, k] -= (sub * rref[r, k]);
                }
            }

            lead++;
        }
    }

    /// <summary>
    ///   Gets the pivot column for each row (-1 if no pivot in that row).
    /// </summary>
    public int[] PivotColumns
    {
        get
        {
            if (_pivotColumns == null)
            {
                _pivotColumns = new int[rows];
                Array.Fill(_pivotColumns, -1);

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols - 1; col++) // Exclude augmented column
                    {
                        if (Math.Abs(rref[row, col] - 1.0) < 1e-9)
                        {
                            // Check if this is a leading 1
                            bool isLeading = true;
                            for (int c = 0; c < col; c++)
                            {
                                if (Math.Abs(rref[row, c]) > 1e-9)
                                {
                                    isLeading = false;
                                    break;
                                }
                            }
                            if (isLeading)
                            {
                                _pivotColumns[row] = col;
                                break;
                            }
                        }
                    }
                }
            }
            return _pivotColumns;
        }
    }

    /// <summary>
    ///   Gets the indices of free variables (columns without pivots).
    /// </summary>
    public List<int> FreeVariableIndices
    {
        get
        {
            if (freeVariableIndices == null)
            {
                freeVariableIndices = new List<int>();
                bool[] hasPivot = new bool[cols - 1]; // Exclude augmented column

                // Mark columns that have pivots
                int[] pivotCols = PivotColumns;
                for (int row = 0; row < rows; row++)
                {
                    if (pivotCols[row] != -1)
                    {
                        hasPivot[pivotCols[row]] = true;
                    }
                }

                // Find free variables
                for (int col = 0; col < cols - 1; col++)
                {
                    if (!hasPivot[col])
                    {
                        freeVariableIndices.Add(col);
                    }
                }
            }
            return freeVariableIndices;
        }
    }

    /// <summary>
    ///   Gets the transformation matrix that calculates all variables from free variables.
    ///   Returns a tuple of (coefficientMatrix, constantVector) where:
    ///   allVariables = coefficientMatrix * freeVariables + constantVector
    /// </summary>
    /// <returns>
    ///   Tuple containing:
    ///   - coefficientMatrix: [unknowns x numFreeVars] matrix mapping free variables to all variables
    ///   - constantVector: [unknowns] vector of constant terms
    /// </returns>
    public (double[,] coefficientMatrix, double[] constantVector) GetTransformationMatrix()
    {
        int unknowns = cols - 1; // Exclude augmented column
        List<int> freeVars = FreeVariableIndices;
        int[] pivotCols = PivotColumns;

        double[,] coeffMatrix = new double[unknowns, freeVars.Count];
        double[] constVector = new double[unknowns];

        // For free variables: identity mapping with zero constant
        for (int i = 0; i < freeVars.Count; i++)
        {
            int freeVarIdx = freeVars[i];
            coeffMatrix[freeVarIdx, i] = 1.0;
            constVector[freeVarIdx] = 0.0;
        }

        // For basic variables: extract coefficients from RREF
        for (int row = 0; row < rows; row++)
        {
            int pivotCol = pivotCols[row];
            if (pivotCol != -1)
            {
                // This row defines a basic variable
                // Basic var = constant - sum(coeff * free var)
                constVector[pivotCol] = rref[row, unknowns]; // RHS constant

                for (int i = 0; i < freeVars.Count; i++)
                {
                    int freeVarCol = freeVars[i];
                    // Negate because equation is: basicVar + coeff*freeVar = constant
                    coeffMatrix[pivotCol, i] = -rref[row, freeVarCol];
                }
            }
        }

        return (coeffMatrix, constVector);
    }

    /// <summary>
    ///   Calculates all variable values using the transformation matrix.
    /// </summary>
    /// <param name="freeValues">Array of values for the free variables.</param>
    /// <returns>Array containing all variable values.</returns>
    public int[] CalculateFromTransformationMatrix(int[] freeValues)
    {
        if (freeValues.Length != FreeVariableIndices.Count)
            throw new ArgumentException($"Expected {FreeVariableIndices.Count} free variable values, got {freeValues.Length}");

        (double[,] coeffMatrix, double[] constVector) = GetTransformationMatrix();
        int unknowns = cols - 1;
        int[] solution = new int[unknowns];

        // solution = coeffMatrix * freeValues + constVector
        for (int i = 0; i < unknowns; i++)
        {
            double value = constVector[i];
            for (int j = 0; j < freeValues.Length; j++)
            {
                value += coeffMatrix[i, j] * freeValues[j];
            }
            solution[i] = (int)Math.Round(value);
        }

        return solution;
    }

    /// <summary>
    ///   Validates if the calculated solution satisfies non-negativity and integrality constraints.
    /// </summary>
    /// <param name="freeValues">Array of values for the free variables.</param>
    /// <param name="solution">The calculated solution (output from CalculateBasicVariables).</param>
    /// <returns>True if all variables are non-negative integers, false otherwise.</returns>
    public bool ValidateSolution(int[] freeValues, int[] solution)
    {
        int unknowns = cols - 1;
        int[] pivotCols = PivotColumns;

        // Check if all basic variables are non-negative
        for (int col = 0; col < unknowns; col++)
        {
            if (solution[col] < 0)
                return false;
        }

        // Verify the solution satisfies the equations
        for (int row = 0; row < rows; row++)
        {
            double sum = 0;
            for (int col = 0; col < unknowns; col++)
            {
                sum += rref[row, col] * solution[col];
            }

            if (Math.Abs(sum - rref[row, unknowns]) > 1e-6)
                return false;
        }

        return true;
    }
    /// <summary>
    ///   Gets the pivot indicating the position
    ///   of the original rows before the swap.
    /// </summary>
    /// 
    public int[] Pivot { get { return pivot; } }

    /// <summary>
    ///   Gets the matrix in row reduced Echelon form.
    /// </summary>
    public double[,] Result { get { return rref; } }
}