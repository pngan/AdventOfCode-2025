using System.Collections.Immutable;
using MoreLinq;

public class Day02
{
    const string day = "02";

    public static IImmutableList<(long a, long b)> Input() =>
        File.ReadLines($"input/2025_{day}_input.txt").First()
            .Split(',')
            .Select(p => p.Split('-')
                          .Fold((a, b) => (long.Parse(a), long.Parse(b))))
            .ToImmutableArray();

    public static object Solve1()
    {
        List<long> invalidIds = new();
        var input = Input();
        foreach ( var v in input )
        {
            for (long i = v.a;  i <= v.b; i++)
            {
                string s = i.ToString();
                int h = (s.Length + 1) / 2;
                if (IsInvalidId(s, h))
                {
                    invalidIds.Add(i);
                }
            }
        }
        return invalidIds.Sum();
    }

    public static object Solve2()
    {
        List<long> invalidIds = new();
        var input = Input();
        foreach (var v in input)
        {
            for (long i = v.a; i <= v.b; i++)
            {
                string s = i.ToString();
                int h = (s.Length + 1) / 2;
                for (int l = h; l > 0; l--)
                {
                    if (IsInvalidId(s, l))
                    {
                        invalidIds.Add(i);
                        break;
                    }
                }
            }
        }
        return invalidIds.Sum();
    }

    // Pattern occurs as first l chars, and then repeated for the rest of the string
    private static bool IsInvalidId(string s, int l)
    {
        if (s.Length == 1) // Single char strings cannot be a repeated pattern
            return false;
        string pattern = s[..l]; 
        for (int j = l; j < s.Length; j += l)
        {
            if (j + l > s.Length || s.IndexOf(pattern, j, l) == -1) // Look for pattern
            {
                return false;
            }
        }

        return true;
    }
}