using System;
using System.Collections.Generic;
using System.Text;

using AdventureOfCode.Utilities.Image;

namespace AdventOfCode_2025.utilities.image;

internal class ImageWriter
{
    internal void WriteImage(CharImage2 image)
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
                row.Append($" {(r + c) % 255}");
            }
            data.Add(row.ToString());
        }

        File.WriteAllLines("tempimage.pgm", data);
    }
}
