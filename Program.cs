Console.WriteLine();
Console.Write($"Day ");
Console.WriteLine($"{Day12.Day}");
Console.WriteLine($"{Day12.Solve1(),20}");
Console.WriteLine($"{Day12.Solve2(),20}");
Console.WriteLine();
Console.WriteLine();

Timing.Run(Day01.Day, Day01.Input, Day01.Solve1, Day01.Solve2);
Timing.Run(Day02.Day, Day02.Input, Day02.Solve1, Day02.Solve2);
Timing.Run(Day03.Day, Day03.Input, Day03.Solve1, Day03.Solve2);
Timing.Run(Day04.Day, Day04.Input, Day04.Solve1, Day04.Solve2);
Timing.Run(Day05.Day, Day05.Input, Day05.Solve1, Day05.Solve2);
Timing.Run(Day06.Day, Day06.Input, Day06.Solve1, Day06.Solve2);
Timing.Run(Day07.Day, Day07.Input, Day07.Solve1, Day07.Solve2);
Timing.Run(Day08.Day, Day08.Input, Day08.Solve1, Day08.Solve2);
Timing.Run(Day09.Day, Day09.Input, Day09.Solve1, Day09.Solve2);
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
            table.AppendLine("| Day | Name | Type | Part 1 (ms) | Part 2 (ms) |");
            table.AppendLine("|---:|---|---|---:|---:|");

            // Attempt to fetch puzzle names from adventofcode.com (works in CI with network access).
            System.Collections.Generic.Dictionary<string, string> names = new();
            foreach (var e in _entries)
            {
                // ensure we have an entry for this day even if fetch fails
                names[e.Day] = "";
            }

            try
            {
                using var http = new System.Net.Http.HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd("github-actions-aoc-timings/1.0");
                var keys = new System.Collections.Generic.List<string>(names.Keys);
                foreach (var key in keys)
                {
                    try
                    {
                        // Parse day number (allow leading zeros)
                        if (!int.TryParse(key, out int dayNum))
                            continue;
                        var url = $"https://adventofcode.com/2025/day/{dayNum}";
                        var html = http.GetStringAsync(url).GetAwaiter().GetResult();
                        // Try to extract <title>...</title>
                        var m = System.Text.RegularExpressions.Regex.Match(html, "<title>(.*?)</title>", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                        if (m.Success)
                        {
                            var title = System.Net.WebUtility.HtmlDecode(m.Groups[1].Value).Trim();
                            string extracted = null;
                            // Title often contains the year and day, e.g. "Advent of Code 2025 - Day 1: Report Repair"
                            var idx = title.IndexOf(':');
                            if (idx >= 0 && idx + 1 < title.Length)
                            {
                                extracted = title.Substring(idx + 1).Trim();
                            }

                            // If title didn't contain the puzzle name, try to find it in the page body.
                            if (string.IsNullOrEmpty(extracted))
                            {
                                // Look for an <h2> that often contains the day and puzzle name.
                                var h2 = System.Text.RegularExpressions.Regex.Match(html, "<h2[^>]*>(.*?)</h2>", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                                if (h2.Success)
                                {
                                    var h2text = System.Net.WebUtility.HtmlDecode(h2.Groups[1].Value).Trim();
                                    var m2 = System.Text.RegularExpressions.Regex.Match(h2text, @":\s*(.*)$");
                                    if (m2.Success)
                                        extracted = m2.Groups[1].Value.Trim();
                                }
                            }

                            // As a last resort, search for "Day <n>[:\-] <name>" in the HTML
                            if (string.IsNullOrEmpty(extracted))
                            {
                                var re = System.Text.RegularExpressions.Regex.Match(html, $"Day\\s*{dayNum}\\s*[:\\-]\\s*(.*?)<", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                                if (re.Success)
                                    extracted = System.Net.WebUtility.HtmlDecode(re.Groups[1].Value).Trim();
                            }

                            // Remove trailing space + three hyphens if present (e.g. "Secret Entrance ---")
                            if (!string.IsNullOrEmpty(extracted))
                            {
                                extracted = extracted.Trim();
                                if (extracted.EndsWith(" ---", System.StringComparison.Ordinal))
                                    extracted = extracted.Substring(0, extracted.Length - 4).TrimEnd();
                                names[key] = extracted;
                            }
                            else
                            {
                                var fallback = title?.Trim() ?? "";
                                if (fallback.EndsWith(" ---", System.StringComparison.Ordinal))
                                    fallback = fallback.Substring(0, fallback.Length - 4).TrimEnd();
                                names[key] = fallback; // fallback to raw title
                            }
                        }
                    }
                    catch
                    {
                        // ignore per-day fetch failures and leave name blank
                    }
                }
            }
            catch
            {
                // If HTTP client cannot be created or network is blocked, skip name fetching.
            }

            // Detect problem type heuristically by scanning the day's source file.
            System.Collections.Generic.Dictionary<string, string> types = new();
            foreach (var e in _entries)
                types[e.Day] = "";

            try
            {
                foreach (var key in new System.Collections.Generic.List<string>(types.Keys))
                {
                    try
                    {
                        var dayStr = key;
                        var path = $"code/day{dayStr}.cs";
                        string src = System.IO.File.Exists(path) ? System.IO.File.ReadAllText(path) : string.Empty;
                        var s = src.ToLowerInvariant();
                        string t = "Unknown";

                        if (string.IsNullOrWhiteSpace(s))
                        {
                            types[key] = t;
                            continue;
                        }

                        if (s.Contains("dijkstra") || s.Contains("bfs") || s.Contains("dfs") || s.Contains("adjacent") || s.Contains("neighbors") || s.Contains("graph"))
                            t = "Graph";
                        else if (s.Contains("dp[") || s.Contains("dynamic programming") || s.Contains("memo") || s.Contains("memoize") || s.Contains("dynamic "))
                            t = "Dynamic Programming";
                        else if (s.Contains("regex") || s.Contains("match(") && s.Contains("regex"))
                            t = "Regex";
                        else if (s.Contains("grid") || s.Contains("rows") || s.Contains("columns") || s.Contains("point") || s.Contains("image") || s.Contains("pixel") || s.Contains("x,") || s.Contains("y,"))
                            t = "Grid/Map";
                        else if (s.Contains("permut") || s.Contains("combin") || s.Contains("factorial"))
                            t = "Combinatorics";
                        else if (s.Contains("stack") && s.Contains("push"))
                            t = "Stack";
                        else if (s.Contains("queue") || s.Contains("enqueue") || s.Contains("dequeue"))
                            t = "Queue";
                        else if (s.Contains("sort(") || s.Contains("orderby") || s.Contains("binarysearch") )
                            t = "Sorting/Search";
                        else if (s.Contains("bigint") || s.Contains("biginteger") || s.Contains("math") || s.Contains("sqrt(") || s.Contains("pow("))
                            t = "Math";
                        else if (s.Contains("parse(") || s.Contains("split('") || s.Contains("split(\",") )
                            t = "Parsing";

                        types[key] = t;
                    }
                    catch
                    {
                        // ignore per-day classification failures
                    }
                }
            }
            catch
            {
                // ignore overall failures
            }
            foreach (var e in _entries)
            {
                var name = "";
                if (names.TryGetValue(e.Day, out var n))
                    name = n ?? "";
                string display = name;
                if (!string.IsNullOrEmpty(display))
                {
                    // Escape pipe and bracket characters for Markdown table/link safety
                    display = display.Replace("|", "\\|").Replace("]", "\\]");
                    if (int.TryParse(e.Day, out int dayNum))
                    {
                        var url = $"https://adventofcode.com/2025/day/{dayNum}";
                        display = $"[{display}]({url})";
                    }
                }

                var type = "";
                if (types.TryGetValue(e.Day, out var tt))
                    type = tt ?? "";
                if (!string.IsNullOrEmpty(type))
                    type = type.Replace("|", "\\|");

                // Make the Day entry a link to the local source file in the repo (preserve leading zeros)
                var dayLink = e.Day;
                try
                {
                    var dayPath = $"code/day{e.Day}.cs";
                    if (System.IO.File.Exists(dayPath))
                        dayLink = $"[{e.Day}]({dayPath})";
                }
                catch
                {
                    // ignore filesystem issues and leave day as plain text
                }

                table.AppendLine($"| {dayLink} | {display} | {type} | {e.Part1Ms} | {e.Part2Ms} |");
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