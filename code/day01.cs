
public class Day01
{
    public const string Day = "01";
    public static List<(int value, int times)> Input()
    {
        var input = File.ReadLines($"input/2025_{Day}_input.txt");
        List<(int value, int times)> result = new ();
        foreach (var line in input)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            int i = int.Parse(line.Substring(1));
            int times = i / 100;
            int value = i - (times*100);
            result.Add((value * (line[0]=='L' ? -1 : 1), times));
        }

        return result;
    }


    public static object Solve1()
    {
        int code = 50;
        var input = Input();
        int result = 0;

        foreach (var i in input)
        {
            code += i.value;
            if (code%100 == 0)
                result++;
        }

        return result;
    }

    public static object Solve2()
    {
        int code = 50;
        var input = Input();
        int result = 0;

        foreach (var i in input)
        {
            result += i.times; // Complete revolutions
            var next = code + i.value;
            if (next % 100 == 0) 
                result++;// Hits zero
            else if (code %100 != 0 &&  Math.Sign(code) != Math.Sign(next))
                result++; // Changes direction
            else if (next != (next % 100))
                result++; // Out of bounds

            code = next % 100;
        }

        return result;
    }
}