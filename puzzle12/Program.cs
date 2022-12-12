//Day 12: Hill Climbing Algorithm

//Current Position = S
//Best signal = E
// Lowest = a -> Highest -> z

//Can move to a destination at most one higher eg. g->h or b->c etc.
//Can jump down eg. l->c or e->d or z->a

/*
Algorithm:

Each step, log the number of legal moves

If a step one higher than this one is available, take it
If not, take the highest legal letter available

Do not repeat position (leave breadcrumb)

If hitting a dead-end, reverse until you reach a fork in the road and block off the path you took
*/

string[] lines = File.ReadAllLines("test_input_2");

List<List<char>> grid = lines.Select(line => line.Select(letter => letter).ToList()).ToList();

Queue<Position> visited = new();

Position currentPosition = GetPositionsOf('S').First();
Position summit = GetPositionsOf('E').First();

Console.WriteLine($"Start: {currentPosition} | Summit: {summit}");
PrintGrid(grid, currentPosition, summit);

while (!currentPosition.Equals(summit))
{
    int currentElevation = ElevationOf(currentPosition);

    List<(Position pos, int elevation)> options = new List<Position>
        {
            currentPosition + Position.Down,
            currentPosition + Position.Right,
            currentPosition + Position.Up,
            currentPosition + Position.Left,
        }
        .Where(pos => pos.X >= 0 && pos.Y >= 0 && pos.X < grid[0].Count && pos.Y < grid.Count)
        .Select(position => (position, elevation: ElevationOf(position)))
        .Where(tup => tup.elevation <= currentElevation + 1 && !visited.Contains(tup.position))
        .OrderByDescending(tup => tup.elevation)
        .ThenBy(tup => tup.position.DistanceFrom(summit))
        .ToList();


    if (options.All(tup => tup.elevation < currentElevation) || options.Count == 0)
    {
        grid[currentPosition.Y][currentPosition.X] = '.';

        Console.WriteLine($"REVERSING TO LAST POSITION | Current: {currentPosition} -> Previous: {visited.Last()}");
        currentElevation = ElevationOf(currentPosition);
        currentPosition = visited.Dequeue();
    }
    else
    {
        (currentPosition, currentElevation) = options.First();
        visited.Enqueue(currentPosition);
    }


    PrintGrid(grid, currentPosition, summit);
    Console.WriteLine($"[{currentPosition} ({currentElevation})] Options: {string.Join(", ", options)}");
    Console.WriteLine();
    Thread.Sleep(300);
}


Console.WriteLine($"Steps taken: {visited.Count}");


void PrintGrid(List<List<char>> grid, Position you, Position top)
{
    Write("\t");

    for (int column = 0; column < grid[0].Count; column++)
    {
        Write($"{column:00}");
    }

    Console.WriteLine();

    for (int row = 0; row < grid.Count; row++)
    {
        List<char> line = grid[row];
        Write($"[{row}]\t ");

        for (int column = 0; column < line.Count; column++)
        {
            char letter = line[column];

            var position = new Position { X = column, Y = row };

            if (position.Equals(you))
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

char GetCharAt(Position position) => grid[position.Y][position.X];

List<Position> GetPositionsOf(char letter) => (from line in grid
    from item in line
    where item == letter
    select new Position { X = line.IndexOf(item), Y = grid.IndexOf(line) }).ToList();

int ElevationOf(Position position) => HeightOf(GetCharAt(position));

int HeightOf(char letter) => letter switch
{
    'S' => 1,
    'E' => 26,
    '.' => 99, //Visited
    _ => letter - 'a' + 1
};

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

    private Position MoveToward(Position other)
    {
        Position temp = this;

        if (temp.IsAbove(other)) temp += Down;
        if (temp.IsUnder(other)) temp += Up;
        if (temp.IsLeftOf(other)) temp += Right;
        if (temp.IsRightOf(other)) temp += Left;

        return temp;
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