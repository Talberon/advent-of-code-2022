// Day 14: Regolith Reservoir

// Grid Orientation:
//      X
//   0 1 2 3
// Y 1
//   2
//   3

string[] lines = File.ReadAllLines("input");

List<List<Position>> wallDirections = lines.Select(line => line.Split(" -> ")
        .Select(Position.FromString)
        .ToList())
    .ToList();

int rightSide = wallDirections.Max(positions => positions.Select(pos => pos.X).Max(x => x));
int bottom = wallDirections.Max(positions => positions.Select(pos => pos.Y).Max(y => y)) + 1;
int leftSide = wallDirections.Min(positions => positions.Select(pos => pos.X).Min(x => x));

var sandOrigin = new Position(500, 0);

GridItem?[,] grid1 = BuildGrid(bottom, rightSide, wallDirections);
int abyssGrains = CountSand(grid1, sandOrigin, true);
PrintGrid(grid1, leftSide, bottom);
Console.WriteLine($"Part 1: {abyssGrains}");

GridItem?[,] grid2 = BuildGrid(bottom, rightSide, wallDirections);
int clogGrains = CountSand(grid2, sandOrigin, false);
PrintGrid(grid2, leftSide, bottom);
Console.WriteLine($"Part 2: {clogGrains}");

static int CountSand(GridItem?[,] grid, Position sandSpawn, bool stopCountingIfSpill)
{
    Sand? fallingSand = null;
    foreach (GridItem? item in grid)
    {
        if (item is Sand gridSand) fallingSand = gridSand;
    }

    if (fallingSand is null)
    {
        fallingSand = new Sand { Position = sandSpawn };
        grid[sandSpawn.X, sandSpawn.Y] = fallingSand;
    }

    Sand currentSand = fallingSand;
    int sandGrainCount = 0;

    while (true)
    {
        if (NextLegalMove(currentSand, grid) is not null)
        {
            grid[currentSand.Position.X, currentSand.Position.Y] = null;
            currentSand.Position = NextLegalMove(currentSand, grid)!.Value;
            grid[currentSand.Position.X, currentSand.Position.Y] = currentSand;
            continue;
        }

        if (stopCountingIfSpill)
        {
            bool grainReachesBottomOfGrid = currentSand.Position.Y == grid.GetLength(1) - 1;
            if (grainReachesBottomOfGrid)
            {
                return sandGrainCount;
            }
        }
        else
        {
            bool sandPileReachedSpout = currentSand.Position == sandSpawn;
            if (sandPileReachedSpout)
            {
                return sandGrainCount + 1;
            }
        }

        currentSand = new Sand { Position = sandSpawn };
        sandGrainCount++;
    }
}

static Position? NextLegalMove(Sand sand, GridItem?[,] grid)
{
    var below = sand.Position + Position.Down;
    var leftBelow = sand.Position + Position.Down + Position.Left;
    var rightBelow = sand.Position + Position.Down + Position.Right;

    if (below.Y >= grid.GetLength(1)) return null;

    if (grid[below.X, below.Y] is null) return below;
    if (grid[leftBelow.X, leftBelow.Y] is null) return leftBelow;
    if (grid[rightBelow.X, rightBelow.Y] is null) return rightBelow;

    return null;
}

static GridItem?[,] BuildGrid(int bottom, int rightSide, List<List<Position>> instructions)
{
    const int spillBuffer = 1;
    //Make grid taller for floor/wider for spillover
    var gridItems = new GridItem?[rightSide + bottom, bottom + spillBuffer];

    foreach (List<Position> wallList in instructions)
    {
        for (int index = 1; index < wallList.Count; index++)
        {
            (int prevX, int prevY) = wallList[index - 1];
            (int currX, int currY) = wallList[index];

            gridItems[prevX, prevY] = new Wall();
            gridItems[currX, currY] = new Wall();

            if (prevY == currY)
            {
                int lower = Math.Min(currX, prevX);
                int higher = Math.Max(currX, prevX);

                for (int col = lower; col < higher; col++) gridItems[col, currY] = new Wall();
            }
            else if (prevX == currX)
            {
                int lower = Math.Min(currY, prevY);
                int higher = Math.Max(currY, prevY);

                for (int row = lower; row < higher; row++) gridItems[currX, row] = new Wall();
            }
            else
            {
                throw new Exception("Esta no bueno, hombre.");
            }
        }
    }

    return gridItems;
}

static void PrintGrid(GridItem?[,] grid, int leftSide, int bottom)
{
    for (int row = 0; row < grid.GetLength(1); row++)
    {
        for (int column = leftSide - bottom; column < grid.GetLength(0); column++)
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


internal abstract class GridItem
{
    public Position Position { get; set; }
}

internal class Sand : GridItem
{
}

internal class Wall : GridItem
{
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