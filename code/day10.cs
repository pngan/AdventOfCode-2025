using Microsoft.Z3;

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

        int totalSum = 0;

        foreach (var inp in input)
        {
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

            bool[,] matrix = new bool[equations, unknowns];
            int[] rightHandSide = new int[equations];

            for (int eq = 0; eq < equations; eq++)
            {
                rightHandSide[eq] = inp.joltage[eq];
            }
            for (int b = 0; b < unknowns; b++)
            {
                foreach (var counter in inp.buttons[b])
                {
                    matrix[counter, b] = true;
                }
            }

            // Solve using Z3
            using (Context ctx = new Context())
            {
                // Create variables dynamically
                IntExpr[] vars = new IntExpr[unknowns];
                for (int i = 0; i < unknowns; i++)
                {
                    vars[i] = ctx.MkIntConst($"x{i}");
                }

                // Create optimizer
                Optimize opt = ctx.MkOptimize();

                // Add non-negativity constraints
                foreach (var v in vars)
                {
                    opt.Assert(ctx.MkGe(v, ctx.MkInt(0)));
                }

                // Build equations dynamically from matrix
                for (int eq = 0; eq < equations; eq++)
                {
                    List<IntExpr> termsInEquation = new List<IntExpr>();

                    // Find which variables appear in this equation
                    for (int varIdx = 0; varIdx < unknowns; varIdx++)
                    {
                        if (matrix[eq, varIdx])
                        {
                            termsInEquation.Add(vars[varIdx]);
                        }
                    }

                    // Build sum incrementally
                    if (termsInEquation.Count > 0)
                    {
                        ArithExpr equationSum = termsInEquation[0];
                        for (int i = 1; i < termsInEquation.Count; i++)
                        {
                            equationSum = (ArithExpr)ctx.MkAdd(equationSum, termsInEquation[i]);
                        }

                        opt.Assert(ctx.MkEq(equationSum, ctx.MkInt(rightHandSide[eq])));
                    }
                }

                // Objective: minimize total button presses
                IntExpr total = (IntExpr)ctx.MkAdd(vars);
                opt.MkMinimize(total);

                // Solve
                if (opt.Check() == Status.SATISFIABLE)
                {
                    Model model = opt.Model;

                    // Uncomment to see intermediate results
                    //Console.WriteLine("Solution:");
                    //for (int i = 0; i < vars.Length; i++)
                    //{
                    //    Console.WriteLine($"{vars[i]} = {model.Evaluate(vars[i])}");
                    //}

                    int solutionTotal = int.Parse(model.Evaluate(total).ToString());
                    //Console.WriteLine($"Total = {solutionTotal}");
                    totalSum += solutionTotal;
                }
                else
                {
                    Console.WriteLine("No solution found.");
                }
            }
        }

        return totalSum;
    }
}