using AdventureOfCode.Utilities.Image;

using MoreLinq;
using Point = (int r, int c);

public class Day07
{
    public const string Day = "07";
    public static (CharImage2 data, int start) Input()
    {
        var image = CharImage2.Parse(File.ReadLines($"input/2025_{Day}_input.txt")
                              .Where(l => !string.IsNullOrEmpty(l)));
        return (image, image.Find('S').First().c);
    }


    /*
    [1] init array of tachyon to zero, cols of array
    [2] read array of splitters
    [3] set tachyon for S
    [4] for r = 2, step 2 to end
    [5]   if splitters match tachyon
    [6]      remember splitter in splitters2
    [7]      count++
    [8]      remove tachyon
    [9]   foreach s in splitter2
    [10]      add tachyon to left of s
    [11]      add tachyon to right of s
    */
    public static object Solve1()
    {
        (CharImage2 data, int start) input = Input();
        var splitters = input.data;
        bool[] tachyons = new bool[splitters.COLS];     // [1]
        tachyons[input.start] = true;                   // [2]
        int splitterCount = 0;
        for(int r = 2; r < splitters.ROWS; r++)         // [3]
        {
            HashSet<int> indexOfHitSplitters = new(); 
            for (int c = 0; c < splitters.COLS; c++)    // [4]
            {
                if (tachyons[c] && splitters[(r,c)]=='^')//[5]
                {
                    indexOfHitSplitters.Add(c);         // [6]
                    splitterCount++;                    // [7]
                    tachyons[c] = false;                // [8]
                }
            }
            foreach(var i in indexOfHitSplitters)       // [9]
            {
                tachyons[i - 1] = true;                 //[10]
                tachyons[i + 1] = true;                 //[11]
            }
        }

        return splitterCount;
    }

    // Breadth first traversal to the bottom row
    // The tree traversal is very large.
    // Reduce computation by visiting each node only once and store the number of visits
    // to each node (i.e. memoisation).
    public static object Solve2()
    {
        var input = Input();
        var image = input.data;
        long pathCount = 0L;

        // Traverse using BFS, using a LIFO queue, starting at 'S' position
        var queue = new Queue<Point>();
        queue.Enqueue((0, input.start));

        // Use memoisation to avoid repeated visit to nodes, but maintain a visit count instead
        var memo = new Image2<long>(image.ROWS, image.COLS);
        memo[(0, input.start)] = 1; 

        while (queue.Count > 0)
        {
            var curr = queue.Dequeue();   // Go to next node to process, BFS-wise
            if (curr.r == image.ROWS - 1) // Stop processing when reach the bottom row
            {
                pathCount += memo[curr];
                continue;
            }

            char nextLowerCell = image[curr.StepS()];
            if (nextLowerCell == '.')
            {
                ProcessNextPosition(queue, memo, curr, curr.StepS());
            }
            else
            {
                ProcessNextPosition(queue, memo, curr, curr.StepSW());
                ProcessNextPosition(queue, memo, curr, curr.StepSE());
            }
        }

        return pathCount;
    }

    private static void ProcessNextPosition(Queue<(int r, int c)> queue, Image2<long> memo, (int r, int c) curr, (int r, int c) nextPosition)
    {
        if (memo[nextPosition] == 0)
        {
            queue.Enqueue(nextPosition);
        }
        memo[nextPosition] += memo[curr]; // Carry the memoised visit count to lower node
    }
}