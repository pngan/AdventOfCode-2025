using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AdventOfCode_2025.utilities.numeric;

public static class Point3Extensions
{
    public static T Dist2<T>(this (T x, T y, T z) a, (T x, T y, T z) b) where T : INumber<T>
    {
        T dx = a.x - b.x;
        T dy = a.y - b.y;
        T dz = a.z - b.z;
        return (dx * dx) + (dy * dy) + (dz * dz);
    }
}
