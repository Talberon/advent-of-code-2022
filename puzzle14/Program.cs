// Day 14: Regolith Reservoir

// Grid
//      X
//   0 1 2 3
// Y 1
//   2
//   3

// Sand pours in from (X:500, Y:0)

// Sand is produced one unit at a time (until one comes to rest)
// Fall Priority:
// 1) Fall down 1 if possible
// 2) Fall down 1 and to the LEFT (-X) if possible
// 3) Fall down 1 and to the RIGHT (+X) if possible
// 4) Else, COME TO REST, start dropping next grain of sand

// Potential Gotcha: What about a pinched position? Seems like it should still go down/left even if left and down are blocked as long as there's a diagonal available

string[] lines = File.ReadAllLines("input");

List<List<Position>> wallDirections = lines.Select(line => line.Split(" -> ")
        .Select(Position.FromString)
        .ToList())
    .ToList();

int rightSide = wallDirections.Max(positions => positions.Select(pos => pos.X).Max(x => x));
int bottom = wallDirections.Max(positions => positions.Select(pos => pos.Y).Max(y => y));
int leftSide = wallDirections.Min(positions => positions.Select(pos => pos.X).Min(x => x));
const int top = 0;

Console.WriteLine($"Top: {top} | Right Side: {rightSide} | Bottom: {bottom} | Left: {leftSide}");

var sandOrigin = new Position(500, 0);
IGridItem?[,] grid = BuildGrid(bottom, rightSide, wallDirections);
PrintGrid(grid, leftSide);
int sandUnits = CountSand(grid, sandOrigin, leftSide);

PrintGrid(grid, leftSide);
Console.WriteLine($"Part 1: {sandUnits}");

static int CountSand(IGridItem?[,] grid, Position sandSpawn, int leftSide)
{
    //Add sand to grid
    Sand? fallingSand = null;
    foreach (IGridItem? item in grid)
    {
        if (item is Sand gridSand)
        {
            fallingSand = gridSand;
        }
    }

    if (fallingSand is null)
    {
        fallingSand = new Sand { Position = sandSpawn };
        grid[sandSpawn.X, sandSpawn.Y] = fallingSand;
    }

    Sand currentSand = fallingSand.Value;

    int sandGrainCount = 0;

    while (true)
    {
        if (currentSand.NextLegalMove(grid) is not null)
        {
            grid[currentSand.Position.X, currentSand.Position.Y] = null;
            currentSand.Position = currentSand.NextLegalMove(grid)!.Value;
            grid[currentSand.Position.X, currentSand.Position.Y] = currentSand;
        }
        else
        {
            // PrintGrid(grid, leftSide);
            // Console.WriteLine($"Grain Count: {sandGrainCount}\n");
            
            if (currentSand.Position.Y == grid.GetLength(1) - 1 ||
                currentSand.Position.X >= grid.GetLength(0) ||
                currentSand.Position.X < 0)
            {
                return sandGrainCount;
            }

            currentSand = new Sand { Position = sandSpawn };
            sandGrainCount++;
        }
    }
}

static IGridItem?[,] BuildGrid(int bottom, int rightSide, List<List<Position>> instructions)
{
    var gridItems = new IGridItem?[rightSide + 2, bottom + 1];

    foreach (List<Position> wallList in instructions)
    {
        for (int index = 1; index < wallList.Count; index++)
        {
            (int prevX, int prevY) = wallList[index - 1];
            (int currX, int currY) = wallList[index];

            gridItems[prevX, prevY] = new Wall();
            gridItems[currX, currY] = new Wall();

            if (prevY == currY) //Same row
            {
                int lower = Math.Min(currX, prevX);
                int higher = Math.Max(currX, prevX);
                //Run Horizontal
                for (int col = lower; col < higher; col++)
                {
                    gridItems[col, currY] = new Wall();
                }
            }
            else if (prevX == currX) //Same column
            {
                int lower = Math.Min(currY, prevY);
                int higher = Math.Max(currY, prevY);
                //Run Vertical
                for (int row = lower; row < higher; row++)
                {
                    gridItems[currX, row] = new Wall();
                }
            }
            else
            {
                throw new Exception("Previous and current walls should not be the same point");
            }
        }
    }

    return gridItems;
}

static void PrintGrid(IGridItem?[,] grid, int leftSide)
{
    for (int row = 0; row < grid.GetLength(1); row++)
    {
        for (int column = leftSide - 1; column < grid.GetLength(0); column++)
        {
            Console.Write(grid[column, row] switch
            {
                Wall => "#",
                Sand => "O",
                _ => "|"
            });
        }

        Console.WriteLine();
    }
}


internal interface IGridItem
{
    public Position Position { get; set; }
}

internal struct Sand : IGridItem
{
    public Position Position { get; set; }

    public Position? NextLegalMove(IGridItem?[,] grid)
    {
        var below = Position + Position.Down;
        var leftBelow = Position + Position.Down + Position.Left;
        var rightBelow = Position + Position.Down + Position.Right;

        if (below.Y >= grid.GetLength(1)) return null;
        if (leftBelow.X < 0) return null;

        if (grid[below.X, below.Y] is null) return below;
        if (grid[leftBelow.X, leftBelow.Y] is null) return leftBelow;
        if (grid[rightBelow.X, rightBelow.Y] is null) return rightBelow;

        return null;
    }
}

internal struct Wall : IGridItem
{
    public Position Position { get; set; }
}


internal readonly struct Position
{
    public int X { get; }
    public int Y { get; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    public static Position Up => new(0, -1);
    public static Position Down => new(0, 1);
    public static Position Left => new(-1, 0);
    public static Position Right => new(1, 0);

    public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);
    public static Position operator -(Position a, Position b) => new(a.X - b.X, a.Y - b.Y);
    public static bool operator ==(Position a, Position b) => a.Equals(b);
    public static bool operator !=(Position a, Position b) => !a.Equals(b);

    public static Position FromString(string input)
    {
        string[] segments = input.Split(",");

        if (segments.Length != 2) throw new Exception($"Invalid string input! Received: {input}");

        return new Position(Convert.ToInt32(segments[0]), Convert.ToInt32(segments[1]));
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Position other) return false;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString() => $"(X:{X}, Y:{Y})";
}