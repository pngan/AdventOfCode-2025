
using System.Text.RegularExpressions;

public class Day01
{
    const string day = "01";
    static List<int> Input()
    {
        var input = File.ReadLines($"input/2025_{day}_input.txt");
        List<int> inputList = new ();
        foreach (var line in input)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            int i = int.Parse(line.Substring(1));
            inputList.Add(i * (line[0]=='L' ? -1 : 1));
        }

        return inputList;
    }
    static List<(char dir, int clicks)> Input2()
    {
        var input = File.ReadLines($"input/2025_{day}_input.txt");
        List<(char dir, int clicks)>  inputList = new();
        foreach (var line in input)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            int i = int.Parse(line.Substring(1));
            inputList.Add((line[0], i));
        }

        return inputList;
    }

    public static object Solve1()
    {
        int code = 50;
        var input = Input();
        int result = 0;

        foreach (var i in input)
        {
            code += ((i % 100) + 100);
            code %= 100;
            if (code== 0)
                result++;
        }


        return result;
    }

    public static object Solve2()
    {
        int code = 50;
        List<(char dir, int clicks)> input = Input2();
        int zeroes = 0;

        foreach ((char dir, int c)  in input)
        {

            int clicks = c;
            while(clicks-- > 0)
            {
                if (dir == 'L')
                {
                    if (code == 0)
                    {
                        code = 100;
                    }
                    code--;
                }
                else
                {
                    if (code == 100)
                    {
                        code = 0;
                    }
                    code++;
                }
                if (code == 0 || code ==100)
                    zeroes++;
            }
        }

        return zeroes;
    }
}