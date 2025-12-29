Console.WriteLine();
Console.Write($"Day ");
Console.WriteLine($"{Day12.Day}");
Console.WriteLine($"{Day12.Solve1(),20}");
Console.WriteLine($"{Day12.Solve2(),20}");
Console.WriteLine();
Console.WriteLine();

//Day03.Test();
Timing.Run(Day01.Day, Day01.Input, Day01.Solve1, Day01.Solve2);
Timing.Run(Day02.Day, Day02.Input, Day02.Solve1, Day02.Solve2);
Timing.Run(Day03.Day, Day03.Input, Day03.Solve1, Day03.Solve2);
Timing.Run(Day04.Day, Day04.Input, Day04.Solve1, Day04.Solve2);
Timing.Run(Day05.Day, Day05.Input, Day05.Solve1, Day05.Solve2);
Timing.Run(Day06.Day, Day06.Input, Day06.Solve1, Day06.Solve2);
Timing.Run(Day07.Day, Day07.Input, Day07.Solve1, Day07.Solve2);
Timing.Run(Day08.Day, Day08.Input, Day08.Solve1, Day08.Solve2);
//Timing.Run(Day09.Day, Day09.Input, Day09.Solve1, Day09.Solve2); // Takes around 10 minutes
Timing.Run(Day10.Day, Day10.Input, Day10.Solve1, Day10.Solve2);
Timing.Run(Day11.Day, Day11.Input, Day11.Solve1, Day11.Solve2);
Timing.Run(Day12.Day, Day12.Input, Day12.Solve1, Day12.Solve2);
Timing.WriteReadmeTimings("README.md");

static public class Timing
{
    record Entry(string Day, long Part1Ms, long Part2Ms);
    static readonly System.Collections.Generic.List<Entry> _entries = new();

    static public void Run(string day, Delegate input, Func<object> solve1, Func<object> solve2)
    {
        input.DynamicInvoke();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        solve1();
        sw.Stop();
        Console.WriteLine($"Day {day} - Part 1: {sw.ElapsedMilliseconds,6} ms");
        var p1 = sw.ElapsedMilliseconds;
        sw.Restart();
        solve2();
        sw.Stop();
        Console.WriteLine($"Day {day} - Part 2: {sw.ElapsedMilliseconds,6} ms");
        var p2 = sw.ElapsedMilliseconds;
        _entries.Add(new Entry(day, p1, p2));
    }

    static public void WriteReadmeTimings(string readmePath)
    {
        try
        {
            var table = new System.Text.StringBuilder();
            table.AppendLine("<!-- TIMINGS START -->");
            table.AppendLine();
            table.AppendLine("## Timings");
            table.AppendLine();
            table.AppendLine("| Day | Part 1 (ms) | Part 2 (ms) |");
            table.AppendLine("|---:|---:|---:|");
            foreach (var e in _entries)
            {
                table.AppendLine($"| {e.Day} | {e.Part1Ms} | {e.Part2Ms} |");
            }
            table.AppendLine();
            table.AppendLine("<!-- TIMINGS END -->");

            string readme = System.IO.File.Exists(readmePath) ? System.IO.File.ReadAllText(readmePath) : string.Empty;

            var startMarker = "<!-- TIMINGS START -->";
            var endMarker = "<!-- TIMINGS END -->";

            if (readme.Contains(startMarker) && readme.Contains(endMarker))
            {
                int start = readme.IndexOf(startMarker, System.StringComparison.Ordinal);
                int end = readme.IndexOf(endMarker, start, System.StringComparison.Ordinal);
                if (start >= 0 && end >= 0)
                {
                    end += endMarker.Length;
                    readme = readme.Substring(0, start) + table.ToString() + readme.Substring(end);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(readme) && !readme.EndsWith("\n"))
                    readme += "\n";
                readme += table.ToString();
            }

            System.IO.File.WriteAllText(readmePath, readme);
            Console.WriteLine($"Updated {readmePath} with timings.");
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Failed to update README timings: {ex.Message}");
        }
    }
}