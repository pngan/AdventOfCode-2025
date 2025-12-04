using System.Collections.Immutable;

public class Day03
{
    const string day = "03";
    public static ImmutableArray<ImmutableArray<ulong>> Input() =>
        File.ReadLines($"input/2025_{day}_input.txt")
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
}