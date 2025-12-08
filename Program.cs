Console.WriteLine();
Console.Write($"Day ");
Console.WriteLine($"{Day08.Day}");
Console.WriteLine($"{Day08.Solve1(),20}");
Console.WriteLine($"{Day08.Solve2(),20}");
Console.WriteLine();
Console.WriteLine();

//Day03.Test();
//Timing.Run(Day01.Day, Day01.Input, Day01.Solve1, Day01.Solve2);
//Timing.Run(Day02.Day, Day02.Input, Day02.Solve1, Day02.Solve2);
//Timing.Run(Day03.Day, Day03.Input, Day03.Solve1, Day03.Solve2);
//Timing.Run(Day04.Day, Day04.Input, Day04.Solve1, Day04.Solve2);
//Timing.Run(Day05.Day, Day05.Input, Day05.Solve1, Day05.Solve2);
//Timing.Run(Day06.Day, Day06.Input, Day06.Solve1, Day06.Solve2);
//Timing.Run(Day06.Day, Day07.Input, Day06.Solve1, Day06.Solve2);

static public class Timing
{
    static public void Run( string day, Delegate input, Func<object> solve1, Func<object> solve2)
    {
        input.DynamicInvoke();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        solve1();
        sw.Stop();
        Console.WriteLine($"Day {day} - Part 1: {sw.ElapsedMilliseconds, 6} ms");
        sw.Restart();
        solve2();
        sw.Stop();
        Console.WriteLine($"Day {day} - Part 2: {sw.ElapsedMilliseconds, 6} ms");
    }
}