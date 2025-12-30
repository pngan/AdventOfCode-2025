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

                        // Scored heuristic: accumulate signals for multiple categories and pick the strongest.
                        var scores = new System.Collections.Generic.Dictionary<string, int>(System.StringComparer.OrdinalIgnoreCase);
                        void AddScore(string name, int v)
                        {
                            if (v <= 0) return;
                            if (!scores.ContainsKey(name)) scores[name] = 0;
                            scores[name] += v;
                        }

                        int ContainsCount(params string[] toks)
                        {
                            var c = 0;
                            foreach (var tok in toks)
                                if (!string.IsNullOrEmpty(tok) && s.Contains(tok))
                                    c++;
                            return c;
                        }

                        AddScore("Graph", ContainsCount("dijkstra", "bfs", "dfs", "adjacent", "neighbors", "graph", "edge", "vertex", "shortest path", "astar", "priorityqueue", "priority queue"));
                        AddScore("Dynamic Programming", ContainsCount("dp[", "dynamic programming", "memo", "memoize", "cache", "topdown", "bottomup"));
                        AddScore("Regex", ContainsCount("regex", "match(", "matches(", "regexoptions"));
                        AddScore("Grid/Map", ContainsCount("grid", "rows", "columns", "point", "image", "pixel", "manhattan", "neighbors", "adjacent", "heightmap", "floodfill"));
                        AddScore("Simulation", ContainsCount("simulate", "simulation", "step", "ticks", "turns", "evolve", "state", "nextstate", "applyrules", "runsteps"));

                        var bitCount = 0;
                        if (s.Contains("<<")) bitCount++;
                        if (s.Contains(">>")) bitCount++;
                        if (s.Contains(" & ") || s.Contains("&(")) bitCount++;
                        if (s.Contains(" ^ ")) bitCount++;
                        if (s.Contains("popcount") || s.Contains("countbits") || s.Contains("bitcount")) bitCount++;
                        if (s.Contains("mask") || s.Contains("bits")) bitCount++;
                        AddScore("Bitwise", bitCount);

                        AddScore("Math", ContainsCount("bigint", "biginteger", "gcd", "lcm", "sqrt(", "pow(", "mod", "prime", "primes", "log("));
                        AddScore("Combinatorics", ContainsCount("permut", "combin", "factorial", "nchoosek", "choose(", "binomial"));
                        AddScore("Search/Backtracking", ContainsCount("backtrack", "backtracking", "bruteforce", "dfs(", "search("));
                        AddScore("Sorting/Search", ContainsCount("sort(", "orderby", "binarysearch"));
                        AddScore("Queue", ContainsCount("queue", "enqueue", "dequeue"));
                        AddScore("Stack", ContainsCount("stack", "push", "pop"));
                        AddScore("Union-Find", ContainsCount("unionfind", "disjoint set", "dsu", "union find"));

                        // Parsing should be a fallback — only chosen when stronger signals are absent.
                        var parseSignals = 0;
                        if (s.Contains("parse(") || s.Contains("tryparse") || s.Contains("string.split") || s.Contains("split(") || s.Contains("split('") || s.Contains("split(\"") )
                            parseSignals = 1;
                        AddScore("Parsing", parseSignals);

                        // Choose best-scoring category. If there's a tie, prefer more specific categories by ordering.
                        var bestKey = (string?)null;
                        var bestVal = 0;
                        foreach (var kv in scores)
                        {
                            if (kv.Value > bestVal)
                            {
                                bestVal = kv.Value;
                                bestKey = kv.Key;
                            }
                        }

                        if (!string.IsNullOrEmpty(bestKey) && bestVal > 0)
                        {
                            // If the only signal is "Parsing" but other categories had 0, prefer Unknown unless parseSignals present.
                            if (bestKey == "Parsing" && bestVal == parseSignals && bestVal == 1)
                                t = "Parsing";
                            else
                                t = bestKey;
                        }
                        else if (parseSignals > 0)
                        {
                            t = "Parsing";
                        }
                        else
                        {
                            t = "Unknown";
                        }

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
