using MoreLinq;

public class Day06
{
    public const string Day = "06";
    public static (List<long>[] values, char[] ops) Input()
    {
        List<List<long>> valuesByLine = new();
        List<List<long>> values = new();
        string[] li = File.ReadLines($"input/2025_{Day}_input.txt").Split("").First().ToArray();

        var ops = li[^1].Split([" "], StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();

        for (int l = 0; l < li.Count()-1; l++)
        {
            var lineValues =  li[l].Split([" "], StringSplitOptions.RemoveEmptyEntries).Select(v=>Convert.ToInt64(v));
            valuesByLine.Add(lineValues.ToList());    
        }

        for(int opi=0; opi < ops.Length; opi++)
        {
            List<long> operands = new();
            for (int l = 0; l < li.Count() - 1; l++)
            {
                operands.Add(valuesByLine[l][opi]);
            }
            values.Add(operands);
        }

        return (values.ToArray(), ops);
    }

    public static object Solve1()
    {
        var input = Input();
        if (input.values.Length != input.ops.Length) throw new Exception("Bad input");
        long sum = 0L;
        for (int i = 0; i < input.ops.Length; i++)
        {
            var operands = input.values[i];
            var operation = input.ops[i];
            sum += ApplyOperation(operation, operands);
        }
        return sum;
    }

    // [1] Read the input
    // [2] Treat input as 2D array and swap rows and cols (transpose)
    // [3] Get the list of operations
    // [4] Iterate over every column in transposed
    // [5] Convert every column to number and store them in list of operands
    // [6] A blank column denotes end of operands, and therefore apply the operator to the operands
    // [7] Aggregate the calculation
    public static object Solve2()
    {
        string[] lines = File.ReadLines($"input/2025_{Day}_input.txt").Split("").First().ToArray(); // [1]
        int rows = lines.Length - 1; // -1 ignore operator
        int cols = lines.First().Length;

        List<List<char>> transpose = new();
        for (int c = 0; c < cols; c++)
        {
            transpose.Add(Enumerable.Repeat(' ', rows).ToList());
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                transpose[c][r] = lines[r][c]; // [2]
            }
        }

        var ops = lines[^1].Split([" "], StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray(); // [3]

        long sum = 0L;
        List<long> operands = new();
        int opi = 0;
        for (int i = 0; i < transpose.Count; i++) // [4]
        {
            var digits = transpose[i];
            if (digits.Any(c => c != ' ')) // [6]
            {
                string digitString = new string(digits.Where(c => c != ' ').ToArray());
                if (!string.IsNullOrEmpty(digitString)) // [5]
                {
                    long operand = long.Parse(digitString);
                    operands.Add(operand);
                }
            }
            else
            {
                long result = ApplyOperation(ops[opi], operands);
                opi++;
                operands.Clear();
                sum += result; // [7]
            }
        }

        sum += ApplyOperation(ops[opi], operands);//[7] Last set of operands special because it is not denoted by a blank column, being at the end of the set

        return sum;
    }

    private static long ApplyOperation(char operation, IEnumerable<long> operands)
    {
        return operation switch
        {
            '+' => operands.Sum(),
            '*' => operands.Aggregate(1L, (acc, val) => acc * val),
            _ => throw new Exception($"Unknown operation: {operation}")
        };
    }
}