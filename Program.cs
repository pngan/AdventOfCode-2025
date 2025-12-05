Day05.Input();

Console.WriteLine();
var sw = System.Diagnostics.Stopwatch.StartNew();
var result1 = Day05.Solve1();
sw.Stop();
Console.WriteLine($"{result1, 20} : {sw.ElapsedMilliseconds} ms");

sw.Restart();
var result2 = Day05.Solve2();
sw.Stop();
Console.WriteLine($"{result2, 20} : {sw.ElapsedMilliseconds} ms");

Console.WriteLine();
Console.WriteLine();