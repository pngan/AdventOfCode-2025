
using System.Text.RegularExpressions;

public class Day01
{
    const string day = "01";
    static (List<int> l, List<int> r) Input()
    {
        var input = File.ReadLines($"input/2024_{day}_input.txt");

        List<int> left = new();
        List<int> right = new();

        foreach (var item in input)
        {
            var row = Regex.Split(item, @"\s+");
            left.Add(int.Parse(row[0]));
            right.Add(int.Parse(row[1]));
        }

        return (left.Order().ToList(), right.Order().ToList());
    }

    public static object Solve1()
    {
        var input = Input();
        int diff = 0;
        for (int i = 0; i < input.l.Count(); i++)
        {
            diff += (int)(Math.Abs(input.l[i] - input.r[i]));
        }
        return diff;
    }

    public static object Solve2()
    {
        var input = Input();
        // Histogram
        Dictionary<int, int> histr = input.r
            .GroupBy(i => i)
            .Select(i => new { i.Key, Count = i.Count() })
            .ToDictionary(i => i.Key, i => i.Count);

        long similarity = 0;
        foreach (int key in input.l)
        {
            if (histr.TryGetValue(key, out int tally))
                similarity += key * tally;
        }

        return similarity;
    }
}