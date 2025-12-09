using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;

using MoreLinq;

using Point = (long r, long c);
public class Day09
{
    public const string Day = "09";

    public static IEnumerable<Point> Input() =>
         File.ReadLines($"input/2025_{Day}_input.txt")
            .Where(l => !string.IsNullOrEmpty(l))
            .Select(p => p.Split(',')
                          .Fold((r, c) => (long.Parse(c), long.Parse(r))));

    public static object Solve1()
    {
        var input = Input().ToArray();
        long maxArea = long.MinValue;
        long numTiles = input.Length;
        for(int i = 0; i < numTiles; i++)
        {
            var tile1 = input[i];
            for (int j = i+1; j < numTiles; j++ )
            {
                var tile2 = input[j];
                var area = (Math.Abs(tile1.r - tile2.r) + 1) * (Math.Abs(tile1.c - tile2.c)+1);
                if (area > maxArea)
                {
                    maxArea = area;
                }
            }
        }


        return maxArea;
    }

    // 1. Eliminate impossible pairs, ie. those that topologically spans a hole
    // 2. Largest Rects ordered largest to smallest
    // 3. Rectangle intrusion detection - does something intrude the rectangle
    public static object Solve2()
    {
        (long r, long c)[] input = Input().ToArray();

        List<(Point a, Point b, long area)> candidateRects
            = FindTopologicallyPlausibleRects(input);
            //.Where(v=>v.i.c==94916 || v.j.c == 94916).ToList();

        var orderedRects = candidateRects.OrderByDescending(r => r.area);
        int count = 0;
        foreach (var r in orderedRects)
        {
            if (count++%1000==0)
            Console.WriteLine($"{count}");
            bool hasIntrusion = false;
            var bp = BoundaryPoints(input.ToList());
            foreach (var p in bp)
            {
                // Skip the corner points themselves
                if (p == r.a || p == r.b)
                    continue;

                if (HasIntrusion(r.a, r.b, p))
                {
                    hasIntrusion = true;
                    break;
                }
            }

            if (!hasIntrusion)
            {
                // Found a valid rectangle with no intrusions
                return r.area;
            }
        }

        return -1;
    }

    // Between three points that always form a right angle, return 
    // +1 for clockwise rotation
    // -1 for counter clockwise rotation
    // 0 No rotation
    static int Rotation(Point p1, Point p2, Point p3)
    {
        // Validate that p1 and p2 are connected (share row or column)
        if (!(p1.r == p2.r || p1.c == p2.c))
            throw new InvalidDataException("Points p1 and p2 not connected");

        // Validate that p2 and p3 are connected (share row or column)
        if (!(p2.r == p3.r || p2.c == p3.c))
            throw new InvalidDataException("Points p2 and p3 not connected");

        // Calculate direction vectors
        long dir1_r = p2.r - p1.r;
        long dir1_c = p2.c - p1.c;
        long dir2_r = p3.r - p2.r;
        long dir2_c = p3.c - p2.c;

        // Calculate cross product (z-component of 2D cross product)
        // Positive = counter-clockwise (left turn)
        // Negative = clockwise (right turn)
        // Zero = straight line (no turn)
        long crossProduct = (dir1_r * dir2_c) - (dir1_c * dir2_r);

        if (crossProduct > 0)
            return 1;  // Counter-clockwise rotation (left turn)
        else if (crossProduct < 0)
            return -1; // Clockwise rotation (right turn)
        else
            return 0;  // No rotation (straight line or backtrack)
    }
private static bool HasIntrusion((long r, long c) corner1, (long r, long c) corner2, (long r, long c) point)
    {
        // Find the bounding box of the rectangle
        long minR = Math.Min(corner1.r, corner2.r);
        long maxR = Math.Max(corner1.r, corner2.r);
        long minC = Math.Min(corner1.c, corner2.c);
        long maxC = Math.Max(corner1.c, corner2.c);

        // Check if point is strictly inside (excluding boundaries, since corners are already in the input)
        return point.r > minR && point.r < maxR && point.c > minC && point.c < maxC;
    }

    // This function finds whether the rectangle form by
    // by two points in a closed polygon falls outside the polygon or not.
    // 
    // Found that walking the circular list from point a to point b and aggregating the 
    // unit rotations, and then walking the same direction from point b to point a
    // and again aggregating the rotation, that if the aggregates differ in Sign, the
    // two corners of the rectangle falls outside the body of the closed shape. 
    //
    // I don't know how this works theoretically. Found this out by trial and error using
    // pen and paper.
    private static List<(Point i, Point j, long area)> FindTopologicallyPlausibleRects((long r, long c)[] input)
    {
        List<(Point i, Point j, long area)> result = new();
        int np = input.Length;
        for (int i = 0; i < np; i++)
        {
            var p1 = input[i];
            for (int j = i + 1; j < np; j++)
            {
                var p2 = input[j];

                // [1] Walk from p1 to p2, calc rotation
                // [2] Walk from p2 to p1, calc rotation
                // [3] Those pairs whose rotations are different sign are a "hole" so cannot be a rectangle
                int rotationPitoPj = 0;
                for (int pi = i; pi <= j - 2; pi++) //[1]
                {
                    rotationPitoPj += Rotation(input[pi], input[(pi + 1) % np], input[(pi + 2) % np]);
                }

                int rotationPjtoPi = 0;
                int end = (i - 1 + np) % np;
                
                for(int pj = j;(pj % np) == end; pj++)
                { 
                    rotationPitoPj += Rotation(input[pj], input[(pj + 1) % np], input[(pj + 2) % np]);
                }
                if ((Math.Sign(rotationPitoPj)>0 && Math.Sign(rotationPitoPj) <0)
                    || (Math.Sign(rotationPitoPj) > 0 && Math.Sign(rotationPitoPj) < 0))
                {
                    continue; // This pair of points have a hold in between them
                }

                var area = (Math.Abs(p1.r - p2.r) + 1) * (Math.Abs(p1.c - p2.c) + 1);
                result.Add((input[i], input[j], area)); 
            }
        }
        return result;
    }

    static List<Point> BoundaryPoints(List<Point> corners)
    {
        List<Point> points = new();

        for (int i = 0; i < corners.Count()-1; i++ )
        {
            Point a = corners[i];
            Point b = corners[i + 1];
            if (a.r == b.r)
            {
                if (a.c < b.c)
                    for (long c = a.c; c < b.c; c++)
                        points.Add((a.r, c));
                else
                    for (long c = a.c; c > b.c; c--)
                        points.Add((a.r, c));
            }
            else
            {
                if (a.r < b.r)
                    for (long r = a.r; r < b.r; r++)
                        points.Add((r, a.c));
                else
                    for(long r = a.r; r>b.r; r--)
                        points.Add((r, a.c));
            }
        }

        // Do this for one more pair of points: first and last in the list.
        {
            Point a = corners[^1];
            Point b = corners[0];
            if (a.r == b.r)
            {
                if (a.c < b.c)
                    for (long c = a.c; c < b.c; c++)
                        points.Add((a.r, c));
                else
                    for (long c = a.c; c > b.c; c--)
                        points.Add((a.r, c));
            }
            else
            {
                if (a.r < b.r)
                    for (long r = a.r; r < b.r; r++)
                        points.Add((r, a.c));
                else
                    for (long r = a.r; r > b.r; r--)
                        points.Add((r, a.c));
            }
        }
        return points;
    }

}