//Day 12: Hill Climbing Algorithm

//Current Position = S
//Best signal = E
// Lowest = a -> Highest -> z

//Can move to a destination at most one higher eg. g->h or b->c etc.
//Can jump down eg. l->c or e->d or z->a


string[] lines = File.ReadAllLines("input");

List<List<GridCell>> grid =
    lines.Select(
        line => line.Select(letter => new GridCell
        {
            Letter = letter,
            DistanceFromOrigin = null
        }).ToList()
    ).ToList();


Stack<Position> visited = new();

Position currentPosition = GetPositionsOf('S').First();
Position summit = GetPositionsOf('E').First();

Console.WriteLine($"Start: {currentPosition} | Summit: {summit}");
PrintGrid(grid, currentPosition, summit);

//From starting point, create a DistanceFromOrigin grid tracking the cost
SetDistanceFromOriginAtPosition(currentPosition, 0);

int currentDistance = 0;

while (grid[summit.Y][summit.X].DistanceFromOrigin is null)
{
    //New strategy

    //Beginning with start point, update DistanceFromOrigins for each legal move around current distance

    List<Position> maxDistanceCells = new();
    for (int row = 0; row < grid.Count; row++)
    {
        for (int col = 0; col < grid[0].Count; col++)
        {
            if (grid[row][col].DistanceFromOrigin == currentDistance)
            {
                maxDistanceCells.Add(new Position { X = col, Y = row });
            }
        }
    }

    foreach (Position position in maxDistanceCells)
    {
        currentPosition = position;

        List<GridCell> legalCells = new List<Position>
            {
                currentPosition + Position.Down,
                currentPosition + Position.Right,
                currentPosition + Position.Up,
                currentPosition + Position.Left,
            }
            .Where(pos => pos.X >= 0 && pos.Y >= 0 && pos.X < grid[0].Count && pos.Y < grid.Count)
            .Select(pos => grid[pos.Y][pos.X])
            .Where(cell =>
                cell.Elevation - 1 <= CellAtPosition(currentPosition).Elevation && cell.DistanceFromOrigin is null)
            .ToList();
        
        Console.WriteLine($"Legal cells: {string.Join(",", legalCells)}");

        foreach (GridCell legalCell in legalCells)
        {
            legalCell.DistanceFromOrigin = currentDistance + 1;
        }
    }

    // PrintGrid(grid, currentPosition, summit);
    // Console.WriteLine();
    // Thread.Sleep(500);
    currentDistance++;
}

PrintGrid(grid, currentPosition, summit);
Console.WriteLine($"Steps taken: {grid[summit.Y][summit.X].DistanceFromOrigin}");

void SetDistanceFromOriginAtPosition(Position position, int distanceFromOrigin)
{
    grid[position.Y][position.X].DistanceFromOrigin = distanceFromOrigin;
}


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
            else if (visited.Contains(position))
            {
                Write(@"{=Green}" + letter + "{/} "); //({HeightOf(letter)})}
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