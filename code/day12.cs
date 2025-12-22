using row = (int width, int height, int[] shapes);
public class Day12
{
    public const string Day = "12";
    public static row[] Input() => 
        File.ReadLines($"input/2025_{Day}_input.txt").Skip(30)
            .Where(l=>!string.IsNullOrEmpty(l))
            .Select(x=> ( int.Parse(x[..2]), int.Parse(x[3..5]), new []{ int.Parse(x[7..9]), int.Parse(x[10..12]), int.Parse(x[13..15]), int.Parse(x[16..18]), int.Parse(x[19..21]), int.Parse(x[22..24])}) )
            .ToArray();

    public static object Solve1()
    {
        int canFit = 0;
        foreach(var inp in Input())
        {
            var shapeCount = inp.shapes.Sum();
            var edgeToEdgePackCount = (inp.width/3)*(inp.height/3);
            if (shapeCount <= edgeToEdgePackCount)
            {
                canFit++;
            }
        }
        return canFit;
    }

    public static object Solve2()
    {
        // No part two for day 12
        return 0;
    }
}