string[] lines = File.ReadAllLines("./input.txt");

int totalOverlapCounter = 0;
int partialOverlapCounter = 0;

foreach (string line in lines)
{
    string[] segments = line.Split(',');

    int[] left = segments[0].Split('-').Select(val => Convert.ToInt32(val)).ToArray();
    int[] right = segments[1].Split('-').Select(val => Convert.ToInt32(val)).ToArray();

    (int leftStart, int leftEnd) = (left[0], left[1]);
    (int rightStart, int rightEnd) = (right[0], right[1]);

    bool leftIsInsideRight = leftStart.Between(rightStart, rightEnd) && leftEnd.Between(rightStart, rightEnd);
    bool rightIsInsideLeft = rightStart.Between(leftStart, leftEnd) && rightEnd.Between(leftStart, leftEnd);

    bool leftOverlapsRight = leftStart.Between(rightStart, rightEnd) || leftEnd.Between(rightStart, rightEnd);
    bool rightOverlapsLeft = rightStart.Between(leftStart, leftEnd) || rightEnd.Between(leftStart, leftEnd);

    Console.WriteLine(line);

    if (leftIsInsideRight || rightIsInsideLeft)
    {
        Console.WriteLine("Yes (Total Overlap)");
        totalOverlapCounter++;
    }

    if (leftOverlapsRight || rightOverlapsLeft)
    {
        Console.WriteLine("Yes (Partial Overlap)");
        partialOverlapCounter++;
    }
}

Console.WriteLine($"# of total overlap: {totalOverlapCounter}");
Console.WriteLine($"# of partial overlap: {partialOverlapCounter}");

internal static class IntExtensions
{
    public static bool Between(this int subject, int start, int end) => (subject <= end && subject >= start);
}