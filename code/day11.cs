using MoreLinq;

// State is node and whether fft and dac have been seen
using State = (string node, bool seenfft, bool seendac);
public class Day11
{
    public const string Day = "11";
    public static Dictionary<string, string[]> Input()
    {
        Dictionary<string, string[]> input = File.ReadLines($"input/2025_{Day}_input.txt")
            .Where(l => !string.IsNullOrEmpty(l))
            .Select(line => line.Split(':', StringSplitOptions.TrimEntries))
            .Where(parts => parts.Length == 2)
            .ToDictionary(
                parts => parts[0],
                parts => parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
        );

        return input;
    }

    public static object Solve1()
    {
        Dictionary<string, string[]> input = Input();
        Queue<string> queue = new();
        input["you"].ForEach(x => queue.Enqueue(x));
        int count = 0;
        while (queue.Count > 0)
        {
            var curr = queue.Dequeue();
            if (curr == "out")
            {
                count++;
                continue;
            }
            input[curr].ForEach(x => queue.Enqueue(x));
        }
        return count;
    }

    public static object Solve2()
    {
        Dictionary<State, ulong> _memo = new();
        return Walk(Input(), ("svr", false, false), "out");

        ulong Walk(Dictionary<string, string[]> next, State curr, string end)
        {
            ulong count = 0UL;
            // Have visited before?
            if (_memo.TryGetValue(curr, out count))
                return count;

            if (curr.node == end) // Met stop condition?
            {
                if (curr.seenfft && curr.seendac) // Count if achieved target condition
                    count++;
                return count;
            }

            // Assess target condition
            var seenfft = curr.seenfft || curr.node == "fft";
            var seendac = curr.seendac || curr.node == "dac";

            // Visit next state, including target condition
            foreach (var n in next[curr.node])
            {
                count += Walk(next, (n, seenfft, seendac) , end);
            }

            _memo[curr] = count; // Update memo with the count at current state
            return count;
        }
    }
}