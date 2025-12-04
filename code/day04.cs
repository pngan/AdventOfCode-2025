using AdventureOfCode.Utilities.Image;
public class Day04
{
    const string day = "04";

    static CharImage2 Input() =>
        CharImage2.Parse(File.ReadLines($"input/2025_{day}_input.txt")
        .Where(l => !string.IsNullOrEmpty(l)));

    public static object Solve1()
    {
        var map = Input();
        ulong count = 0;
        var rolls = map.Find('@');
        foreach(var roll in rolls)
        {
            if (map.Neighbours8(roll, v => v == '@').Count() < 4)
            {
                count++;
            }
        }
        return count;
    }

    public static object Solve2()
    {
        var map = Input();
        ulong count = 0;
        ulong removed = 1;
        while (removed > 0)
        {
            removed = 0;
            var rolls = map.Find('@');
            foreach (var roll in rolls)
            {
                if (map.Neighbours8(roll, v => v == '@').Count() < 4)
                {
                    map[roll] = 'x';
                    removed++;
                }
            }
            count += removed;
        }
        return count;
    }
}