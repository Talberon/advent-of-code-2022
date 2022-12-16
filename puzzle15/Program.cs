// Day 15: Beacon Exclusion Zone

using System.Text.RegularExpressions;

const int frequencyMultiplier = 4_000_000;
const int checkRow = 2_000_000;
const string fileName = "input";
const int searchSpaceLimit = 4_000_000;

string[] lines = File.ReadAllLines(fileName);

List<Instruction> instructions = ParseInstructions(lines);

int impossiblePositions = OccupiedPositionsInRow(instructions, checkRow).Count();
Console.WriteLine($"Part 1: {impossiblePositions}");

long frequency = DetectTuningFrequency(instructions, searchSpaceLimit, frequencyMultiplier);
Console.WriteLine($"Part 2: {frequency}");

return;

static long DetectTuningFrequency(List<Instruction> instructions, int searchSpaceLimit, int frequencyMultiplier)
{
    for (int row = 0; row <= searchSpaceLimit; row++)
    {
        RangeCollection occupiedRange = instructions
            .Where(instructions => instructions.Sensor.Position.Y - instructions.Sensor.Radius <= row &&
                                   instructions.Sensor.Position.Y + instructions.Sensor.Radius >= row
            )
            .Aggregate(new RangeCollection(),
                (total, instruction) => instruction.Sensor.RangeTakenAtRow(row, searchSpaceLimit) is { } range
                    ? total.AddRange(range)
                    : total);

        if (occupiedRange.Ranges.Count <= 1) continue;

        var missingBeacon = new Position(occupiedRange.Ranges.MinBy(range => range.End).End + 1, row);
        long tuningFrequency = ((long)missingBeacon.X * (long)frequencyMultiplier) + (long)missingBeacon.Y;
        return tuningFrequency;
    }

    throw new Exception("No beacon found!");
}

static IEnumerable<Position> OccupiedPositionsInRow(List<Instruction> instructions, int row)
{
    List<Position> knownBeaconsInRow = instructions.Select(i => i.Beacon.Position)
        .Where(p => p.Y == row)
        .ToList();

    return instructions
        .Where(i => i.Sensor.Position.Y - i.Sensor.Radius <= row && i.Sensor.Position.Y + i.Sensor.Radius >= row)
        .SelectMany(i => i.Sensor.PositionsInRadiusAtRow(row, knownBeaconsInRow))
        .Except(knownBeaconsInRow)
        .Distinct();
}


static List<Instruction> ParseInstructions(string[] lines) =>
    (from line in lines
        select Regex.Matches(line, @"((-|)\d+)")
            .Select(match => Convert.ToInt32(match.Value))
            .ToList()
        into parsedCoordinates
        let beacon = new Beacon { Position = new Position(parsedCoordinates[2], parsedCoordinates[3]) }
        let sensorPosition = new Position(parsedCoordinates[0], parsedCoordinates[1])
        let sensor = new Sensor
            { Position = sensorPosition, Radius = sensorPosition.DistanceFrom(beacon.Position) }
        select new Instruction { Beacon = beacon, Sensor = sensor }).ToList();

class Sensor : GridItem
{
    public int Radius { get; init; }

    public List<Position> PositionsInRadiusAtRow(int atRow, List<Position> shouldIgnore)
    {
        List<Position> signals = new();

        int lowerY = Position.Y - Radius;
        int upperY = Position.Y + Radius + 1;

        if (atRow < lowerY || atRow > upperY) return signals;

        for (int row = -Radius; row <= Radius; row++)
        {
            if (row != atRow - Position.Y) continue;

            for (int col = -Radius; col <= Radius; col++)
            {
                if (row.Abs() + col.Abs() > Radius) continue;

                if (shouldIgnore.Any(ignore => ignore.X == Position.X + col)) continue;

                signals.Add(Position + new Position(col, row));
            }
        }

        return signals;
    }

    public Range? RangeTakenAtRow(int row, int rightSideLimit)
    {
        int originDistanceFromRow = (row - Position.Y).Abs();
        
        if (originDistanceFromRow > Radius) return null;

        int left = Position.X - (Radius - originDistanceFromRow);
        int right = Position.X + (Radius - originDistanceFromRow);

        const int lowerLimit = 0;
        return new Range(Math.Max(lowerLimit, left), Math.Min(rightSideLimit, right));
    }
}

internal readonly struct RangeCollection
{
    public List<Range> Ranges { get; }

    public RangeCollection()
    {
        Ranges = new List<Range>();
    }

    public RangeCollection AddRange(Range newRange)
    {
        if (Ranges.FindIndex(range => range.CanAdd(newRange)) is var index and >= 0)
        {
            Ranges[index] += newRange;
        }
        else
        {
            Ranges.Add(newRange);
        }

        CombineRanges();

        return this;
    }

    private void CombineRanges()
    {
        for (int i = 0; i < Ranges.Count; i++)
        {
            Range current = Ranges[i];
            if (Ranges.FindIndex(r => r != current && r.CanAdd(current)) is not (var canAddIndex and >= 0)) continue;

            Ranges[i] += Ranges[canAddIndex];
            Ranges.RemoveAt(canAddIndex);
            return;
        }
    }
}

internal readonly struct Range
{
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
        bool iPartiallyOverlapWithOther =
            Start >= other.Start && Start <= other.End || End <= other.End && End >= other.Start;
        bool otherPartiallyOverlapsWithMe =
            other.Start >= Start && other.Start <= End || other.End <= End && other.End >= Start;
        bool areAdjacent = other.End == Start - 1 || other.Start == End + 1 || End == other.Start - 1 ||
                           Start == other.End + 1;

        return otherIsEntirelyWithinMe ||
               iAmEntirelyWithinOther ||
               iPartiallyOverlapWithOther ||
               otherPartiallyOverlapsWithMe ||
               areAdjacent;
    }

    public override bool Equals(object? obj) => obj is Range other && Start == other.Start && End == other.End;
    public override int GetHashCode() => HashCode.Combine(Start, End);

    public override string ToString() => $"<Start: {Start}, End: {End}>";
}

internal readonly struct Instruction
{
    public Sensor Sensor { get; init; }
    public Beacon Beacon { get; init; }
}

internal abstract class GridItem
{
    public Position Position { get; init; }
}

internal class Beacon : GridItem
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

    public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);
    public static Position operator -(Position a, Position b) => new(a.X - b.X, a.Y - b.Y);
    public static bool operator ==(Position a, Position b) => a.Equals(b);
    public static bool operator !=(Position a, Position b) => !a.Equals(b);

    public int DistanceFrom(Position other)
    {
        int vertDistance = Math.Max(Y, other.Y) - Math.Min(Y, other.Y);
        int horiDistance = Math.Max(X, other.X) - Math.Min(X, other.X);

        return vertDistance + horiDistance;
    }

    public override bool Equals(object? obj) => obj is Position other && X == other.X && Y == other.Y;
    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => $"(X:{X}, Y:{Y})";
}

internal static class MyExtensions
{
    public static int Abs(this int me) => Math.Abs(me);
}