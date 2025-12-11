using System.Collections.Immutable;

using MoreLinq;

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

    public static object Solve1a()
    {
        Dictionary<string, string[]> input = Input();
        Queue<string> queue = new();
        input["fft"].ForEach(x => queue.Enqueue(x));
        int count = 0;
        HashSet<string> visited = new();

        while (queue.Count > 0)
        {
            var curr = queue.Dequeue();



            if (curr == "out")
            {
                count++;
                continue;
            }

            if (!visited.Add(curr))
                Console.WriteLine($"[{curr}]");
            input[curr].ForEach(x => queue.Enqueue(x));
        }

        return count;
    }

    public static object Solve2()
    {
        Dictionary<string, string[]> input = Input();
        Stack<(string dev, int dac, int fft, int depth)> stack = new();
        HashSet<(string dev, int dac, int fft, int depth)> visited = new();
        stack.Push(("svr", 0, 0, 0));
        int count = 0;
        while (stack.Count > 0)
        {
            var curr = stack.Pop();
            if (!visited.Add(curr))
            {
                continue;
            }
                Console.Write($"{curr.depth} ");

            int dac = 0;
            int fft = 0;
            if (curr.dev == "dac") dac++;
            if (curr.dev == "fft") fft++;   

            if (curr.dev == "out")
            {
                if (curr.dac>0 && curr.fft>0)
                    count++;
                Console.WriteLine();
                continue;
            }


            Console.WriteLine($"{curr}");
            string[] next = input[curr.dev];
            next.Reverse();
            foreach (var s in next)
            {

                    stack.Push((s, curr.dac + dac, curr.fft + fft, curr.depth + 1));
                
            }

        }

        return count;
    }
}