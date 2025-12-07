using System.Text;

using AdventureOfCode.Utilities.Image;

namespace AdventOfCode_2025.utilities.image;

internal class ImageWriter
{
    internal void WriteImage(CharImage2 image, string filename, int? sequence)
    {
        List<string> data = new();
        data.Add("P2");
        data.Add($"{image.COLS} {image.ROWS}");
        data.Add($"255");
        for (int r = 0; r < image.ROWS; r++)
        {
            StringBuilder row = new();
            for (int c = 0; c < image.COLS; c++)
            {
                //row.Append($" {(byte)(255-image[(r, c)])} {(byte)image[(r, c)]} {(byte)image[(r, c)]}");
                row.Append($" {(int)image[(r, c)]}");
            }
            data.Add(row.ToString());
        }

        filename = sequence.HasValue ? filename + $"{sequence:0000}.pgm" : filename + ".pgm"; 
        File.WriteAllLines(filename, data);
    }
}
