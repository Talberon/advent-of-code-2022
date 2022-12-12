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


Position startingPoint = GetPositionsOf('S').First();
Position summit = GetPositionsOf('E').First();

int DistanceToSummit(Position start, Position destination)
{
    //Reset the mountain
    grid.ForEach(row => row.ForEach(col => col.DistanceFromOrigin = null));

    Console.WriteLine($"Start: {start} | Summit: {destination}");

    //From starting point, create a DistanceFromOrigin grid tracking the cost
    grid[start.Y][start.X].DistanceFromOrigin = 0;

    int distanceToSummit = 0;

    //Break and return -1 if we are stuck in a hole
    int distanceCellsThisRound;
    int distanceCellsLastRound = 0;

    while (grid[destination.Y][destination.X].DistanceFromOrigin is null)
    {
        //Beginning with start point, update DistanceFromOrigins for each legal move around current distance

        List<Position> maxDistanceCells = new();
        for (int row = 0; row < grid.Count; row++)
        {
            for (int col = 0; col < grid[0].Count; col++)
            {
                if (grid[row][col].DistanceFromOrigin == distanceToSummit)
                {
                    maxDistanceCells.Add(new Position { X = col, Y = row });
                }
            }
        }

        if (maxDistanceCells.Count == 0)
        {
            Console.WriteLine("STUCK! EXIT EARLY");
            return -1;
        }

        foreach (Position position in maxDistanceCells)
        {
            start = position;

            List<GridCell> legalCells = new List<Position>
                {
                    start + Position.Down,
                    start + Position.Right,
                    start + Position.Up,
                    start + Position.Left,
                }
                .Where(pos => pos.X >= 0 && pos.Y >= 0 && pos.X < grid[0].Count && pos.Y < grid.Count)
                .Select(pos => grid[pos.Y][pos.X])
                .Where(cell =>
                    cell.Elevation - 1 <= CellAtPosition(start).Elevation && cell.DistanceFromOrigin is null)
                .ToList();

            // Console.WriteLine($"Legal cells: {string.Join(",", legalCells)}");

            foreach (GridCell legalCell in legalCells)
            {
                legalCell.DistanceFromOrigin = distanceToSummit + 1;
            }

        }

        // PrintGrid(mountain, currentPosition, summit);
        // Console.WriteLine();
        // Thread.Sleep(500);

        distanceToSummit++;
    }

    // PrintGrid(grid, start, destination);
    return distanceToSummit;
}

Console.WriteLine($"Part 1");
int distanceFromSToE = DistanceToSummit(startingPoint, summit);
Console.WriteLine($"Part 1: {distanceFromSToE}");

//Part 2

//1. Get all positions of 'a' character in grid
//2. Feed them into a loop calculating distance
//3. Select the lowest number

Console.WriteLine($"Part 2");
List<Position> positions = GetPositionsOf('a');
List<int> distancesToSummit = positions.Select(position => DistanceToSummit(position, summit)).ToList();

Console.WriteLine($"Part 2: {distancesToSummit.Where(distance => distance > 0).Min()}");

void PrintGrid(List<List<GridCell>> grid, Position you, Position top)
{
    Write("\t");

    for (int column = 0; column < grid[0].Count; column++)
    {
        Write($"{column:00}");
    }

    Console.WriteLine();

    for (int row = 0; row < grid.Count; row++)
    {
        List<GridCell> line = grid[row];
        Write($"[{row}]\t ");

        for (int column = 0; column < line.Count; column++)
        {
            char letter = line[column].Letter;
            int? distanceFromOrigin = line[column].DistanceFromOrigin;

            var position = new Position { X = column, Y = row };

            if (distanceFromOrigin.HasValue)
            {
                Write(@"{=Blue}" + distanceFromOrigin + "{/} "); //({HeightOf(letter)})}
            }
            else if (position.Equals(you))
            {
                Write(@"{=Red}X{/} "); //({HeightOf(letter)})
            }
            else if (position.Equals(top))
            {
                Write(@"{=Magenta}${/} "); //({HeightOf(letter)})
            }
            else
            {
                if (letter == '.')
                {
                    Write(@"{=Blue}" + letter + "{/} "); //({HeightOf(letter)})
                }
                else
                {
                    Write(@"{/}" + letter + "{/} "); //({HeightOf(letter)})}
                }
            }
        }

        Console.WriteLine();
    }
}


GridCell CellAtPosition(Position position)
{
    return grid[position.Y][position.X];
}

List<Position> GetPositionsOf(char letter) => (from line in grid
    from item in line
    where item.Letter == letter
    select new Position { X = line.IndexOf(item), Y = grid.IndexOf(line) }).ToList();

void Write(string msg)
{
    string[] ss = msg.Split('{', '}');
    ConsoleColor c;
    foreach (var s in ss)
        if (s.StartsWith("/"))
            Console.ResetColor();
        else if (s.StartsWith("=") && Enum.TryParse(s.Substring(1), out c))
            Console.ForegroundColor = c;
        else
            Console.Write(s);
}

class GridCell
{
    public int? DistanceFromOrigin { get; set; }

    public char Letter { get; init; }

    public int Elevation => Letter switch
    {
        'S' => 1,
        'E' => 26,
        '.' => 99, //Visited
        _ => Letter - 'a' + 1
    };

    public override string ToString()
    {
        return $"[GridCell: (DistanceFromOrigin: {DistanceFromOrigin}, Letter: {Letter}, Elevation: {Elevation})]";
    }
}

internal readonly struct Position
{
    public int X { get; init; }
    public int Y { get; init; }

    public static Position Empty => new(0, 0);

    private Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    public int DistanceFrom(Position other)
    {
        (int otherX, int otherY) = other;
        int deltaX = Math.Abs(otherX - X);
        int deltaY = Math.Abs(otherY - Y);

        int diagonalSteps = Math.Min(deltaX, deltaY);
        int straightSteps = Math.Max(deltaX, deltaY) - diagonalSteps;

        return Convert.ToInt32(Math.Sqrt(2) * diagonalSteps + straightSteps);
    }

    private bool IsAbove(Position other) => Y > other.Y;
    private bool IsUnder(Position other) => Y < other.Y;
    private bool IsLeftOf(Position other) => X < other.X;
    private bool IsRightOf(Position other) => X > other.X;

    public static Position Up => new(0, 1);
    public static Position Down => new(0, -1);
    public static Position Left => new(-1, 0);
    public static Position Right => new(1, 0);

    public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);
    public static Position operator -(Position a, Position b) => new(a.X - b.X, a.Y - b.Y);

    public override string ToString() => $"(X:{X}, Y:{Y})";
}