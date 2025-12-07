using System.Numerics;

namespace AdventureOfCode.Utilities.Image;

public class Image2<T> where T : INumber<T>
{
    private readonly Dictionary<(int r, int c), T> _image = [];
    public int ROWS { get; init; } 
    public int COLS { get; init; }

    public Image2(int rows, int cols)
    {
        ROWS = rows; 
        COLS = cols;
        for (int r = 0; r < ROWS; r++)
        {
            for (int c = 0; c < COLS; c++)
            {
                this[(r, c)] = default(T);
            }
        }
    }

    public T this[(int r, int c ) p]
    {
        get {
            if (!InBounds(p))
                throw new IndexOutOfRangeException();
            return _image[p]; }
        set {
            if (!InBounds(p))
                throw new IndexOutOfRangeException();
            
            _image[p] = value;
        }
    }

    public bool InBounds((int r, int c ) p) => p.r >= 0 && p.c >= 0 && p.r < ROWS && p.c < COLS;


    // Returns only neighbours in bounds
    public IEnumerable<(int r, int c )> Neighbours8((int r, int c ) p) => p.Neighbours8().Where(InBounds);
    public IEnumerable<(int r, int c )> Neighbours8((int r, int c ) p, Func<T, bool> predicate) => p.Neighbours8().Where(InBounds).Where(p=>predicate(_image[p]));
    public IEnumerable<(int r, int c )> Neighbours4((int r, int c ) p) => p.Neighbours4().Where(InBounds);
    public IEnumerable<(int r, int c)> Neighbours4((int r, int c) p, Func<T, bool> predicate) => p.Neighbours4().Where(InBounds).Where(p => predicate(_image[p]));

    public IEnumerable<(int r, int c )> EveryPoint() => _image.Keys;

    public bool Exists((int r, int c ) p) => _image.ContainsKey(p);

    public bool TryGetValue((int r, int c ) p, out T value)
    {
        value = default;
        if (!Exists(p)) return false;
        value = _image[p];
        return true;
    }

    public IEnumerable<(int r, int c)> Find(T value) => _image.Where(kvp => kvp.Value.Equals(value)).Select(kvp => kvp.Key);
}
