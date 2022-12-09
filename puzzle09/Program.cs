//Day 09: Rope Bridge

// Rules:
// Head and Tail must always be touching
// If Head is 2 steps away cardinally, move Tail cardinally
// If Head and Tail NOT TOUCHING and not in same X or Y, Tail moves diagonally

// Grid orientation
//   3
// Y 2
//   1   
//   0 1 2 3
//      X

string[] lines = File.ReadAllLines("input");

const int chainLength = 10;
List<Knot> chain = new();

for (int i = 0; i < chainLength; i++) chain.Add(Knot.Empty);

List<Knot> tailHistory = new() { chain[^1] };

foreach (string line in lines)
{
    string[] segments = line.Split(" ");
    (string direction, int steps) = (segments[0], Convert.ToInt32(segments[1]));

    for (int step = 1; step <= steps; step++)
    {
        chain[0] += direction switch
        {
            "U" => Knot.Up,
            "D" => Knot.Down,
            "L" => Knot.Left,
            "R" => Knot.Right,
            _ => throw new ArgumentOutOfRangeException()
        };

        for (int i = 0; i < chain.Count - 1; i++)
        {
            Knot leader = chain[i];
            Knot follower = chain[i + 1];

            chain[i + 1] = follower.FollowKnot(leader);
        }

        tailHistory.Add(chain[^1]);
    }
}

Console.WriteLine($"Distinct Tiles visited by Tail: {tailHistory.Distinct().Count()}");

internal readonly struct Knot
{
    public int X { get; }
    public int Y { get; }

    public static Knot Empty => new(0, 0);

    private Knot(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    public Knot FollowKnot(Knot leader)
    {
        int distanceFrom = DistanceFrom(leader);
        bool areTouching = distanceFrom <= 1;

        return !areTouching ? MoveToward(leader) : this;
    }

    private int DistanceFrom(Knot other)
    {
        (int otherX, int otherY) = other;
        int deltaX = Math.Abs(otherX - X);
        int deltaY = Math.Abs(otherY - Y);

        int diagonalSteps = Math.Min(deltaX, deltaY);
        int straightSteps = Math.Max(deltaX, deltaY) - diagonalSteps;

        return Convert.ToInt32(Math.Sqrt(2) * diagonalSteps + straightSteps);
    }

    private Knot MoveToward(Knot other)
    {
        Knot temp = this;

        if (temp.IsAbove(other)) temp += Down;
        if (temp.IsUnder(other)) temp += Up;
        if (temp.IsLeftOf(other)) temp += Right;
        if (temp.IsRightOf(other)) temp += Left;

        return temp;
    }

    private bool IsAbove(Knot other) => Y > other.Y;
    private bool IsUnder(Knot other) => Y < other.Y;
    private bool IsLeftOf(Knot other) => X < other.X;
    private bool IsRightOf(Knot other) => X > other.X;

    public static Knot Up => new (0, 1);
    public static Knot Down => new (0, -1);
    public static Knot Left => new (-1, 0);
    public static Knot Right => new (1, 0);

    public static Knot operator +(Knot a, Knot b) => new(a.X + b.X, a.Y + b.Y);
    public static Knot operator -(Knot a, Knot b) => new(a.X - b.X, a.Y - b.Y);

    public override string ToString() => $"(X:{X}, Y:{Y})";
}