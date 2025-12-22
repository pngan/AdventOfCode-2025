using MoreLinq;
using AdventOfCode_2025.utilities.numeric;
using Point = (long x, long y, long z);
using Circuit = System.Collections.Generic.List<(long x, long y, long z)>;
public class Day08
{
    public const string Day = "08";
    public static IEnumerable<Point> Input() =>
         File.ReadLines($"input/2025_{Day}_input.txt")
            .Where(l => !string.IsNullOrEmpty(l))
            .Select(p => p.Split(',')
                          .Fold((x, y, z) => (long.Parse(x), long.Parse(y), long.Parse(z))));

    // Use BFS to do graph spanning - not the best way, but hey it's part 1, part 2 uses set merging instead
    public static object Solve1()
    {
        var junctions = Input();
        var pairsOrderedByDistance = CalculatePairDistances(junctions.ToArray())
                                        .PartialSortBy(1000, p => p.distance)
                                        .ToList();
        HashSet<Point> visited = new();

        List<Circuit> circuits = new();
        for(int i = 0; i < pairsOrderedByDistance.Count; i++)
        {
            Circuit circuit = new();
            Queue<Point> queue = new();
            queue.Enqueue(pairsOrderedByDistance[i].i);
            queue.Enqueue(pairsOrderedByDistance[i].j);
            while(queue.Count > 0)
            {
                Point current = queue.Dequeue();
                if (visited.Contains(current))
                    continue; // "already in the same circuit, nothing happens!"
                else
                    visited.Add(current);
                if (circuit.Contains(current))
                    continue;
                circuit.Add(current);

                for( int j = i+1; j < pairsOrderedByDistance.Count; j++)
                {
                    if (pairsOrderedByDistance[j].i == current || pairsOrderedByDistance[j].j == current)
                    {
                        queue.Enqueue(pairsOrderedByDistance[j].i);
                        queue.Enqueue(pairsOrderedByDistance[j].j);
                    }
                }
            }
            if (circuit.Count > 0)
                circuits.Add(circuit);
        }

        return circuits
            .OrderByDescending(c => c.Count)
            .Select(c => c.Count)
            .Take(3)
            .Aggregate(1L, (acc, val) => acc * val);
    }

    // Represent circuits as sets, and merge sets using Union()
    public static object Solve2()
    {
        var junctions = Input().ToArray();
        var pairsOrderedByDistance = CalculatePairDistances(junctions.ToArray())
                                        .OrderBy(p => p.distance)
                                        .Select(p=>(p.i, p.j))
                                        .ToList();
        List<Circuit> circuits = new();
        foreach(var pair in pairsOrderedByDistance)
        {
            List<int> indexOfMatched = new();
            var mergedCircuit = new Circuit([pair.i, pair.j]);
            for (int j = 0; j < circuits.Count;j++)
            {
                if (circuits[j].Contains(pair.i) || circuits[j].Contains(pair.j))
                    indexOfMatched.Add(j);
            }

            indexOfMatched.Reverse();
            foreach(var indexOfMatch in indexOfMatched)
            {
                mergedCircuit = mergedCircuit.Union(circuits[indexOfMatch]).ToList();
                circuits.RemoveAt(indexOfMatch);
            }
            circuits.Add(mergedCircuit);

            if (circuits.Count == 1 && circuits.First().Count==junctions.Length)
                return pair.i.x * pair.j.x;
        }

        return -1; // Should never be hit
    }

    // Given a list of points, gives you a map of pair-wise distances
    static IEnumerable<(Point i, Point j, long distance)> CalculatePairDistances(Point[] input)
    {
        for (int i = 1; i < input.Length; i++)
            for (int j = 0; j <= i - 1; j++)
                yield return (input[i], input[j], input[i].Dist2(input[j]));
    }
}