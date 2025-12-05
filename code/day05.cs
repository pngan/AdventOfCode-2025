using System.Collections.Immutable;
using MoreLinq;
using Range = (ulong a, ulong b);
public class Day05
{
    public const string Day = "05";
    public static (Range[] ranges, ulong[] ids) Input() =>
        File.ReadLines($"input/2025_{Day}_input.txt").Split("").Fold((range, available) => (
            range.Select(pair => pair.Split('-').Fold((start, end) => (ulong.Parse(start), ulong.Parse(end)))).ToArray(),
            available.Select(id => Convert.ToUInt64(id)).ToArray())); 

    public static object Solve1()
    {
        var input = Input();
        int count = 0;
        foreach (var id in input.ids)
        {
            foreach (var range in input.ranges)
            {
                if (id >= range.a && id <= range.b)
                {
                    count++;
                    break;
                }
            }
        }
        return count;
    }

    public static object Solve2()
    {
        List<Range> freshRanges = new();
        List<int> rangeParts = new();
        var input = Input();
        foreach (var r in input.ranges)
        {
            rangeParts.Clear();
            for( int i = 0; i < freshRanges.Count; i++)
            {
                if (Overlaps(r, freshRanges[i]))
                {
                    rangeParts.Add(i);
                }
            }

            Range unionRange = r;
            foreach( int rangePart in rangeParts)
            {
                unionRange = RangeUnion(unionRange, freshRanges[rangePart]);
            }

            // Remove the parts that have been combined by union
            rangeParts.Reverse();
            foreach ( var i in rangeParts)
            {
                freshRanges.RemoveAt(i);
            }

            freshRanges.Add(unionRange);
        }

        return freshRanges.Aggregate(0UL, (acc, r) => acc + RangeLength(r));
    }

    public static bool Overlaps(Range x, Range y) 
        => y.b >= x.a && y.a <= x.b 
        || x.b >= y.a && x.a <= y.b;

    public static Range RangeUnion(Range x, Range y) => (Math.Min(x.a, y.a), Math.Max(x.b, y.b));

    public static ulong RangeLength(Range r) => r.b - r.a + 1;
}