using System.Collections.Immutable;

using AdventOfCode_2025.utilities.image;

using AdventureOfCode.Utilities.Image;

public class Day03
{
    public const string Day = "03";
    public static ImmutableArray<ImmutableArray<ulong>> Input() =>
        File.ReadLines($"input/2025_{Day}_input.txt")
            .Where(l => !string.IsNullOrEmpty(l))
            .Select(l => l.Select(c => ulong.Parse(c.ToString())).ToImmutableArray())
            .ToImmutableArray();

    public static object Solve1() => Day03.Joltage(2);
    public static object Solve2() => Day03.Joltage(12);
    public static object Joltage(int digits)
    {
        ulong joltage = 0;
        foreach (var bank in Input())
        {
            int maxPos = -1;
            ulong j = 0;

            for (int n = digits; n > 0; n--)
            {
                ulong max = 0;
                for (int i = maxPos + 1; i < bank.Length - n + 1; i++)
                {
                    if (bank[i] > max)
                    {
                        maxPos = i;
                        max = bank[i];
                    }
                }
                j *= 10;
                j += max;
            }
            joltage += j;
        }
        return joltage;
    }


    public static void Test()
    {
        ImageWriter iw = new();
        var im = CharImage2.Parse(File.ReadLines($"input/2025_04_input.txt")
            .Where(l => !string.IsNullOrEmpty(l)));
        for (int f = 0; f < 200; f++)
        {
            for (int r = 0; r < im.ROWS; r++)
            {
                for (int c = 0; c < im.COLS; c++)
                {
                    im[(r, c)] = (char)f;
                    //im[(r, c)] = (char)(((r + c) % 255));
                }
            }
            iw.WriteImage(im, "temp", f);
        }

    }
}