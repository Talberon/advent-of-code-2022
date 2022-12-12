//Day 12: Hill Climbing Algorithm

//Current Position = S
//Best signal = E
// Lowest = a -> Highest -> z

//Can move to a destination at most one higher eg. g->h or b->c etc.
//Can jump down eg. l->c or e->d or z->a

string[] lines = File.ReadAllLines("input");

List<List<GridCell>> grid = lines.Select(line => line.Select(
    letter => new GridCell
    {
        Letter = letter,
        DistanceFromOrigin = null
    }
).ToList()).ToList();

//Part 1
Position startingPoint = GetPositionsOf('S').First();
Position summit = GetPositionsOf('E').First();
int? distanceFromSToE = DistanceToSummit(startingPoint, summit, grid);
Console.WriteLine($"Part 1: {distanceFromSToE}");

//Part 2
List<Position> positions = GetPositionsOf('a');
List<int?> distancesToSummit = positions.Select(position => DistanceToSummit(position, summit, grid)).ToList();
Console.WriteLine($"Part 2: {distancesToSummit.Where(distance => distance is not null).Min()}");

static int? DistanceToSummit(Position start, Position destination, List<List<GridCell>> mountain)
{
    int distanceToSummit = 0;
    Position currentPosition = start;

    mountain.ForEach(row => row.ForEach(col => col.DistanceFromOrigin = null));

    mountain[currentPosition.Y][currentPosition.X].DistanceFromOrigin = 0;

    while (mountain[destination.Y][destination.X].DistanceFromOrigin is null)
    {
        List<Position> edgeCells = EdgeCells(mountain, distanceToSummit);

        if (edgeCells.Count == 0) return null;

        foreach (Position edgePosition in edgeCells)
        {
            currentPosition = edgePosition;

            List<GridCell> legalCells = new List<Position>
                {
                    currentPosition + Position.Down,
                    currentPosition + Position.Right,
                    currentPosition + Position.Up,
                    currentPosition + Position.Left,
                }
                .Where(pos => pos.IsWithinGridBounds(mountain))
                .Select(pos => mountain[pos.Y][pos.X])
                .Where(cell => cell.Elevation - 1 <= mountain[currentPosition.Y][currentPosition.X].Elevation && cell.DistanceFromOrigin is null)
                .ToList();

            foreach (GridCell legalCell in legalCells)
            {
                legalCell.DistanceFromOrigin = distanceToSummit + 1;
            }
        }

        distanceToSummit++;
    }

    return distanceToSummit;
}


static List<Position> EdgeCells(List<List<GridCell>> grid, int edgeDistance)
{
    List<Position> edgeCells = new();
    for (int row = 0; row < grid.Count; row++)
    {
        for (int col = 0; col < grid[0].Count; col++)
        {
            if (grid[row][col].DistanceFromOrigin == edgeDistance)
            {
                edgeCells.Add(new Position(col, row));
            }
        }
    }

    return edgeCells;
}

List<Position> GetPositionsOf(char letter) => (from line in grid
        from item in line
        where item.Letter == letter
        select new Position(line.IndexOf(item), grid.IndexOf(line)))
    .ToList();

internal class GridCell
{
    public int? DistanceFromOrigin { get; set; }

    public char Letter { get; init; }

    public int Elevation => Letter switch
    {
        'S' => 1,
        'E' => 26,
        _ => Letter - 'a' + 1
    };

    public override string ToString()
    {
        return $"[GridCell: (DistanceFromOrigin: {DistanceFromOrigin}, Letter: {Letter}, Elevation: {Elevation})]";
    }
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

    public static Position Up => new(0, 1);
    public static Position Down => new(0, -1);
    public static Position Left => new(-1, 0);
    public static Position Right => new(1, 0);

    public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);
    public static Position operator -(Position a, Position b) => new(a.X - b.X, a.Y - b.Y);

    public bool IsWithinGridBounds(List<List<GridCell>> mountain)
    {
        return X >= 0 && Y >= 0 &&
               X < mountain[0].Count && Y < mountain.Count;
    }

    public override string ToString() => $"(X:{X}, Y:{Y})";
}