// Day 15: Beacon Exclusion Zone

//Sensor
//Beacon
//Int grid
//Sensors can only lock onto the one beacon closest to the sensor by Manhattan distance (there is never a tie)

// Part 1: How many positions cannot contain a beacon?

//Algo: Determine bounds of grid by detecting left,right,top and bottom borders

// Grid Orientation:
//     -3 
//     -2
//     -1   X
//-2 -1 0 1 2 3
//      1
//    Y 2
//      3

using System.Text.RegularExpressions;

const int frequencyMultiplier = 4_000_000;

// const int checkRow = 10;
// const string fileName = "test_input";
// const int searchSpaceLimit = 20;

const int checkRow = 2_000_000;
const string fileName = "input";
const int searchSpaceLimit = 4_000_000;

string[] lines = File.ReadAllLines(fileName);

List<Instruction> instructions = ParseInstructions(lines);

int impossiblePositions = OccupiedPositionsInRow(instructions, checkRow).Count();
Console.WriteLine($"Part 1: {impossiblePositions}");

DetectTuningFrequency(instructions, searchSpaceLimit, frequencyMultiplier);

return;

//Methods

static void DetectTuningFrequency(List<Instruction> instructions, int searchSpaceLimit, int frequencyMultiplier)
{
    // Simulate(instructions);

    // (X,Y) of missing beacon must be between (0,0) and (searchSpaceLimit,searchSpaceLimit)
    Position missingBeacon = Position.Zero;

    long was = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    for (int row = 0; row <= searchSpaceLimit; row++)
    {
        RangeCollection occupiedRange = instructions
            .Where(instructions => instructions.Sensor.TruePosition.Y - instructions.Sensor.Radius <= row &&
                                   instructions.Sensor.TruePosition.Y + instructions.Sensor.Radius >= row
            )
            .Aggregate(new RangeCollection(),
                (total, instruction) => instruction.Sensor.RangeTakenAtRow(row, searchSpaceLimit) is { } range
                    ? total.AddRange(range)
                    : total);

        int rangeCount = occupiedRange.Ranges.Count;

        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        // Console.WriteLine($"[{now - was}ms | Y: {row}] Occupied Ranges: \n\tX: {string.Join($"\n\tX: ", occupiedRange.Ranges)}");
        was = now;

        if (rangeCount > 1)
        {
            // Console.WriteLine($"[Y: {row}] FOUND SPLIT! ({nextCount} segments) | Ranges: \n\tY: {row}\tX: {string.Join($"\n\tY: {row}\tX: ", occupiedRange.Ranges)}");

            missingBeacon = new Position(occupiedRange.Ranges[0].End + 1, row);
            
            Console.WriteLine($"MISSING BEACON: [{missingBeacon}]");
            break;
        }

        if (row % 100_000 == 0) Console.WriteLine($"No beacon found on row: {row}");
    }


    int tuningFrequency = (missingBeacon.X * frequencyMultiplier) + missingBeacon.Y;
    Console.WriteLine($"Part 2: {missingBeacon.X} x {frequencyMultiplier} + {missingBeacon.Y} = <{tuningFrequency}>");
}

static IEnumerable<Position> OccupiedPositionsInRow(List<Instruction> instructions, int row)
{
    List<Position> knownBeaconsInRow = instructions.Select(i => i.Beacon.TruePosition)
        .Where(p => p.Y == row)
        .ToList();

    return instructions
        .Where(instructions => instructions.Sensor.TruePosition.Y - instructions.Sensor.Radius <= row &&
                               instructions.Sensor.TruePosition.Y + instructions.Sensor.Radius >= row
        )
        .SelectMany(instruction => instruction.Sensor.PositionsInRadiusAtRow(row, knownBeaconsInRow))
        .Except(knownBeaconsInRow)
        .Distinct();
}


static List<Instruction> ParseInstructions(string[] lines) =>
    (from line in lines
        select Regex.Matches(line, @"((-|)\d+)")
            .Select(match => Convert.ToInt32(match.Value))
            .ToList()
        into parsedCoordinates
        let beacon = new Beacon { TruePosition = new Position(parsedCoordinates[2], parsedCoordinates[3]) }
        let sensorPosition = new Position(parsedCoordinates[0], parsedCoordinates[1])
        let sensor = new Sensor
            { TruePosition = sensorPosition, Radius = sensorPosition.DistanceFrom(beacon.TruePosition) }
        select new Instruction { Beacon = beacon, Sensor = sensor }).ToList();

static void Simulate(List<Instruction> instructions)
{
    int top = instructions.Min(instruction => instruction.Top);
    int bottom = instructions.Max(instruction => instruction.Bottom);
    int leftSide = instructions.Min(instruction => instruction.Left);
    int rightSide = instructions.Max(instruction => instruction.Right);

    var offset = new Position(Math.Min(leftSide, 0).Abs(), Math.Min(top, 0).Abs());
    var grid = new GridItem?[rightSide.Abs() + offset.X + 1, bottom.Abs() + offset.Y + 1];

    AddItems(ref grid, offset, instructions);
    PrintGrid(grid, offset);

    Console.WriteLine("\nCheck signals...\n");
    List<Signal> signals = GetSignals(instructions);
    ScanBeacons(ref grid, offset, signals);

    PrintGrid(grid, offset);
}

static void ScanBeacons(ref GridItem?[,] grid, Position offset, List<Signal> signals)
{
    foreach (Signal signal in signals)
    {
        signal.Offset = offset;

        if (!grid.IsWithinBounds(signal.GridPosition)) continue;

        (int sigX, int sigY) = signal.GridPosition;
        grid[sigX, sigY] ??= signal;
    }
}

static void AddItems(ref GridItem?[,] grid, Position offset, List<Instruction> instructions)
{
    foreach (Instruction instruction in instructions)
    {
        instruction.ApplyOffset(offset);
        Beacon beacon = instruction.Beacon;
        Sensor sensor = instruction.Sensor;

        grid[beacon.GridPosition.X, beacon.GridPosition.Y] = beacon;
        grid[sensor.GridPosition.X, sensor.GridPosition.Y] = sensor;
    }
}

static void PrintGrid(GridItem?[,] grid, Position offset)
{
    Console.Write($"{"",3}");
    for (int column = 0; column < grid.GetLength(0); column++)
    {
        Console.Write($"{column - offset.X,3}");
    }

    Console.WriteLine();

    for (int row = 0; row < grid.GetLength(1); row++)
    {
        Console.Write($"{row - offset.Y,3}");

        for (int column = 0; column < grid.GetLength(0); column++)
        {
            string gridIcon = grid[column, row] switch
            {
                Sensor => "S",
                Beacon => "B",
                Signal sig => "#", //sig.GridIcon,
                _ => "."
            };


            Console.Write($"{gridIcon,3}");
        }

        Console.WriteLine();
    }
}


static List<Signal> GetSignals(List<Instruction> instructions) => instructions
    // .Where(instruction => instruction.Sensor.TruePosition == new Position(8, 7))
    .SelectMany(instruction => instruction.Sensor
        .PositionsInRadius()
        .Select(pos => new Signal
            {
                TruePosition = pos,
                GridIcon = pos.DistanceFrom(instruction.Sensor.TruePosition).ToString()
            }
        )
    ).ToList();

//Signal Origin
class Sensor : GridItem
{
    public int Radius { get; init; }

    public List<Position> PositionsInRadiusAtRow(int atRow, List<Position> shouldIgnore)
    {
        List<Position> signals = new();

        int lowerY = TruePosition.Y - Radius;
        int upperY = TruePosition.Y + Radius + 1;

        if (atRow < lowerY || atRow > upperY) return signals;

        for (int row = -Radius; row <= Radius; row++)
        {
            if (row != atRow - TruePosition.Y) continue;

            for (int col = -Radius; col <= Radius; col++)
            {
                if (row.Abs() + col.Abs() > Radius) continue;

                if (shouldIgnore.Any(ignore => ignore.X == TruePosition.X + col)) continue;

                signals.Add(TruePosition + new Position(col, row));
            }
        }

        return signals;
    }

    public Range? RangeTakenAtRow(int row, int rightSideLimit)
    {
        int originDistanceFromRow = (row - TruePosition.Y).Abs();

        if (originDistanceFromRow > Radius) return null;

        int left = TruePosition.X - (Radius - originDistanceFromRow);
        int right = TruePosition.X + (Radius - originDistanceFromRow);

        return new Range(Math.Max(0, left), Math.Min(rightSideLimit, right));
    }

    public List<Position> PositionsInRadius()
    {
        List<Position> signals = new();

        for (int row = -Radius; row <= Radius; row++)
        {
            for (int col = -Radius; col <= Radius; col++)
            {
                if (row.Abs() + col.Abs() > Radius) continue;
                signals.Add(TruePosition + new Position(col, row));
            }
        }

        return signals;
    }
}

internal readonly struct RangeCollection
{
    public List<Range> Ranges { get; }

    public RangeCollection()
    {
        Ranges = new();
    }

    public RangeCollection AddRange(Range newRange)
    {
        // Console.WriteLine($"Adding range: {newRange} to ({string.Join(",", Ranges)})");
        if (Ranges.FindIndex(range => range.CanAdd(newRange)) is var index and >= 0)
        {
            // Console.WriteLine($"\t{newRange} DOES fit into ({string.Join(",", Ranges)})!");
            Ranges[index] += newRange;
        }
        else
        {
            // Console.WriteLine($"\t{newRange} does NOT fit into ({string.Join(",", Ranges)}), so add it.");
            Ranges.Add(newRange);
        }

        CombineRanges();

        // Console.WriteLine($"After adding range: ({string.Join(",", Ranges)})");

        return this;
    }

    private void CombineRanges()
    {
        for (int i = 0; i < Ranges.Count; i++)
        {
            Range current = Ranges[i];
            if (Ranges.FindIndex(r => r != current && r.CanAdd(current)) is var canAddIndex and >= 0)
            {
                Ranges[i] += Ranges[canAddIndex];
                // Console.WriteLine($"Combining ranges: {Ranges[i]} + {Ranges[canAddIndex]} = {Ranges[i] + Ranges[canAddIndex]}");
                Ranges.RemoveAt(canAddIndex);

                CombineRanges();
                return;
            }
        }
    }
}

internal readonly struct Range
{
    public static Range Zero => new(0, 0);

    public int Start { get; }
    public int End { get; }

    public Range(int start, int end)
    {
        if (end < start)
        {
            throw new Exception($"End cannot be lower than start! S: {start}, E: {end}");
        }

        Start = start;
        End = end;
    }

    public int Length => End - Start;

    public static bool operator ==(Range a, Range b) => a.Equals(b);
    public static bool operator !=(Range a, Range b) => !a.Equals(b);

    public static Range operator +(Range a, Range b)
    {
        if (!a.CanAdd(b))
        {
            throw new Exception($"Ranges do not overlap! ({a}) vs ({b})");
        }

        return new Range(Math.Min(a.Start, b.Start), Math.Max(a.End, b.End));
    }

    public bool CanAdd(Range other)
    {
        bool otherIsEntirelyWithinMe = other.Start >= Start && other.End <= End;
        bool iAmEntirelyWithinOther = Start >= other.Start && End <= other.End;
        bool iPartiallOverlapWithOther =
            Start >= other.Start && Start <= other.End || End <= other.End && End >= other.Start;
        bool otherPartiallyOverlapsWithMe =
            other.Start >= Start && other.Start <= End || other.End <= End && other.End >= Start;

        bool areAdjacent = other.End == Start - 1 || other.Start == End + 1 ||
                           End == other.Start - 1 || Start == other.End + 1;

        return otherIsEntirelyWithinMe || iAmEntirelyWithinOther || iPartiallOverlapWithOther ||
               otherPartiallyOverlapsWithMe || areAdjacent;
    }

    public bool Equals(Range other)
    {
        return Start == other.Start && End == other.End;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }

    public override string ToString() => $"<Start: {Start}, End: {End}>";
}

internal readonly struct Instruction
{
    public Sensor Sensor { get; init; }
    public Beacon Beacon { get; init; }

    public void ApplyOffset(Position offset)
    {
        Sensor.Offset = offset;
        Beacon.Offset = offset;
    }

    public int Top => Math.Min(Sensor.TruePosition.Y, Beacon.TruePosition.Y);
    public int Bottom => Math.Max(Sensor.TruePosition.Y, Beacon.TruePosition.Y);
    public int Left => Math.Min(Sensor.TruePosition.X, Beacon.TruePosition.X);
    public int Right => Math.Max(Sensor.TruePosition.X, Beacon.TruePosition.X);
}

abstract class GridItem
{
    public Position TruePosition { get; init; }
    public Position Offset { get; set; } = Position.Zero;

    public Position GridPosition => TruePosition + Offset;
}


//Signal Destination
class Beacon : GridItem
{
}

//Filler between sensor and beacon
class Signal : GridItem
{
    public string GridIcon { get; set; } = "#";
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

    public static Position Zero => new(0, 0);

    public static Position Up => new(0, -1);
    public static Position Down => new(0, 1);
    public static Position Left => new(-1, 0);
    public static Position Right => new(1, 0);

    public Position MoveUp => this + Up;
    public Position MoveDown => this + Down;
    public Position MoveLeft => this + Left;
    public Position MoveRight => this + Right;

    public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);
    public static Position operator -(Position a, Position b) => new(a.X - b.X, a.Y - b.Y);
    public static bool operator ==(Position a, Position b) => a.Equals(b);
    public static bool operator !=(Position a, Position b) => !a.Equals(b);

    public Position Absolute => new(X.Abs(), Y.Abs());

    public int DistanceFrom(Position other)
    {
        int vertDistance = Math.Max(Y, other.Y) - Math.Min(Y, other.Y);
        int horiDistance = Math.Max(X, other.X) - Math.Min(X, other.X);

        return vertDistance + horiDistance;
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

internal static class Extensions
{
    public static int Abs(this int me) => Math.Abs(me);

    public static bool IsWithinBounds(this GridItem?[,] me, Position them) =>
        them.X >= 0 && them.X < me.GetLength(0) && them.Y >= 0 && them.Y < me.GetLength(1);
}