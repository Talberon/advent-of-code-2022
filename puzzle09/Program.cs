//Day 09: Rope Bridge

// Head and Tail must always be touching
// If Head is 2 steps away cardinally, move Tail cardinally
// If Head and Tail NOT TOUCHING and not in same X or Y, Tail moves diagonally

// Grid orientation
//   3
// Y 2
//   1   
//   0 1 2 3
//      X

const bool devMode = false;
Action<string?> debugConsole = devMode ? Console.WriteLine : (_) => { };

string[] lines = File.ReadAllLines("input");

const int chainLength = 10;
List<Point> chain = new();

for (int i = 0; i < chainLength; i++) chain.Add(new Point());

List<Point> tailHistory = new() { chain[^1] };


debugConsole($"START: Chain | {string.Join(", ", chain)}\n");

foreach (string line in lines)
{
    string[] segments = line.Split(" ");
    (string direction, int steps) = (segments[0], Convert.ToInt32(segments[1]));

    for (int step = 1; step <= steps; step++)
    {
        Point head = chain[0];
        chain[0] = direction switch
        {
            "U" => head.MoveUp,
            "D" => head.MoveDown,
            "L" => head.MoveLeft,
            "R" => head.MoveRight,
            _ => Point.Empty
        };

        debugConsole($"Head moves {step}/{steps} steps to the {direction} | H:{chain[0]}");


        for (int knot = 0; knot < chain.Count - 1; knot++)
        {
            Point leader = chain[knot];
            Point follower = chain[knot + 1];
            debugConsole($"Leader [{knot}]: {leader} | Follower [{knot + 1}]: {follower}");
            chain[knot + 1] = FollowKnot(leader, follower);
        }

        debugConsole($"Chain | {string.Join(", ", chain)}\n");
        tailHistory.Add(chain[^1]);
    }

}

Console.WriteLine($"Tiles visited by Tail: \n{string.Join("\n", tailHistory)}");

Console.WriteLine($"Distinct Tiles visited by Tail: {tailHistory.Distinct().Count()}");

Point FollowKnot(Point leader, Point follower)
{
    bool isDiagonal = (follower.X != leader.X && follower.Y != leader.Y);
    int distanceFrom = follower.DistanceFrom(leader);
    bool areTouching = distanceFrom <= 1;

    // debugConsole($"Distance from Tail and Head: {distanceFrom} | T:{follower}\tH:{leader}");
    // if (isDiagonal) debugConsole($"Tail is diagonal from Head | T:{follower}\tH:{leader}");

    if (!areTouching)
    {
        follower = FollowLeader(follower, leader);

        Point FollowLeader(Point follower, Point leader)
        {
            if (follower.IsAbove(leader))
            {
                follower = follower.MoveDown;
            }

            if (follower.IsUnder(leader))
            {
                follower = follower.MoveUp;
            }

            if (follower.IsLeftOf(leader))
            {
                follower = follower.MoveRight;
            }

            if (follower.IsRightOf(leader))
            {
                follower = follower.MoveLeft;
            }

            return follower;
        }

        debugConsole($"Follower has moved | F:{follower}\tL:{leader}");
    }
    else
    {
        debugConsole($"Follower doesn't have to move | F:{follower}\tL:{leader}");
    }

    return follower;
}

internal struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);

    public static Point Empty => new(0, 0);

    public int DistanceFrom(Point other)
    {
        (int otherX, int otherY) = other;
        int deltaX = Math.Abs(otherX - X);
        int deltaY = Math.Abs(otherY - Y);

        int diagonalSteps = Math.Min(deltaX, deltaY);
        int straightSteps = Math.Max(deltaX, deltaY) - diagonalSteps;

        return Convert.ToInt32(Math.Sqrt(2) * diagonalSteps + straightSteps);
    }

    public bool IsAbove(Point other) => Y > other.Y;
    public bool IsUnder(Point other) => Y < other.Y;
    public bool IsLeftOf(Point other) => X < other.X;
    public bool IsRightOf(Point other) => X > other.X;

    public Point MoveUp => this + new Point(0, 1);
    public Point MoveDown => this + new Point(0, -1);
    public Point MoveLeft => this + new Point(-1, 0);
    public Point MoveRight => this + new Point(1, 0);

    public Point Copy => new(X, Y);

    public bool Equals(Point other)
    {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString() => $"(X:{X}, Y:{Y})";
}