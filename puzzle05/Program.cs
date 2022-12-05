// Day 05 - Supply Stacks 

var lines = new List<string>(File.ReadAllLines("./input.txt"));

string crateIndexLine = lines.First(line => line.StartsWith(" 1"));

List<Stack<char>> GetCrateStacks(IEnumerable<string> lines)
{
    //K: Stack Number, V: String Index
    var stackIndexes = new Dictionary<int, int>(
        crateIndexLine.Select(c =>
            char.IsNumber(c)
                ? new KeyValuePair<int, int>(Convert.ToInt32(c.ToString()), crateIndexLine.IndexOf(c))
                : new KeyValuePair<int, int>()
        ).Where(kvp => kvp.Key is not 0)
    );

    Console.WriteLine($"Stack Indexes: {string.Join(", ", stackIndexes)}");

    string[] stackStrings = lines.Where(line => line.Trim().StartsWith("[")).Reverse().ToArray();

    List<Stack<char>> crateStacks = stackIndexes.Keys.Select(_ => new Stack<char>()).ToList();

    foreach (string line in stackStrings)
    {
        foreach ((int stackNumber, int stringIndex) in stackIndexes)
        {
            char crate = line[stringIndex];

            if (char.IsWhiteSpace(crate)) continue;

            crateStacks[stackNumber - 1].Push(crate);
        }
    }

    return crateStacks;
}

List<Instruction> ParseInstructions(List<string> lines)
{
    List<Instruction> instructions = new();

    foreach (string line in lines)
    {
        List<string> segments = line.Split(" ").Where(segment => segment.All(char.IsDigit)).ToList();

        var instruction = new Instruction
        {
            CrateCount = Convert.ToInt32(segments[0]),
            IndexOrigin = Convert.ToInt32(segments[1]) - 1,
            IndexDestination = Convert.ToInt32(segments[2]) - 1
        };

        instructions.Add(instruction);
    }

    return instructions;
}

int instructionStartIndex = lines.IndexOf(crateIndexLine) + 2; //Magic number :(
Console.WriteLine($"Instruction start index: {instructionStartIndex}");

List<Stack<char>> crateStacks = GetCrateStacks(lines);
List<Instruction> instructions =
    ParseInstructions(lines.GetRange(instructionStartIndex, lines.Count - instructionStartIndex));

Console.WriteLine("Starting Crate Set:");
PrintCrates();

foreach (Instruction instruction in instructions)
{
    for (int i = 0; i < instruction.CrateCount; i++)
    {
        char movingCrate = crateStacks[instruction.IndexOrigin].Pop();
        crateStacks[instruction.IndexDestination].Push(movingCrate);
    }

    Console.WriteLine(
        $"Moving {instruction.CrateCount} crate(s) from {instruction.IndexOrigin + 1} to {instruction.IndexDestination + 1}");

    PrintCrates();
}

Console.WriteLine("Done!");

void PrintCrates()
{
    Console.WriteLine("------------------------");
    for (int index = 0; index < crateStacks.Count; index++)
    {
        Stack<char>? stack = crateStacks[index];
        Console.Write($"Stack {index + 1}:\t");
        foreach (char crate in stack.Reverse())
        {
            Console.Write($"[{crate.ToString()}] ");
        }

        Console.WriteLine();
    }

    Console.WriteLine("------------------------");
}

internal struct Instruction
{
    public int CrateCount { get; init; }
    public int IndexOrigin { get; init; }
    public int IndexDestination { get; init; }
}