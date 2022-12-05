// Day 05 - Supply Stacks 

var lines = new List<string>(File.ReadAllLines("./input.txt"));

string crateIndexLine = lines.First(line => line.StartsWith(" 1"));

int instructionStartIndex = lines.IndexOf(crateIndexLine) + 2; //Magic number :(
List<Stack<char>> crateStacks = ParseCrateStacks(lines);
List<string> instructionLines = lines.GetRange(instructionStartIndex, lines.Count - instructionStartIndex);
List<Instruction> instructions = ParseInstructions(instructionLines);

Console.WriteLine("Starting Crate Set:");
PrintCrates();

//CrateMover9000(instructions, crateStacks);
CrateMover9001(instructions, crateStacks);
return;

List<Stack<char>> ParseCrateStacks(IEnumerable<string> lines)
{
    //K: Stack Number, V: String Index
    var stackIndexes = new Dictionary<int, int>(
        crateIndexLine.Select(c =>
            char.IsNumber(c)
                ? new KeyValuePair<int, int>(Convert.ToInt32(c.ToString()), crateIndexLine.IndexOf(c))
                : new KeyValuePair<int, int>()
        ).Where(kvp => kvp.Key is not 0)
    );

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
    return lines.Select(line => line.Split(" ")
        .Where(segment => segment.All(char.IsDigit)).ToList()).Select(segments => new Instruction
        {
            CrateCount = Convert.ToInt32(segments[0]),
            IndexOrigin = Convert.ToInt32(segments[1]) - 1,
            IndexDestination = Convert.ToInt32(segments[2]) - 1
        }
    ).ToList();
}

void CrateMover9000(List<Instruction> list, List<Stack<char>> crateStacks)
{
    foreach (Instruction instruction in list)
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
}

void CrateMover9001(List<Instruction> list, List<Stack<char>> crateStacks)
{
    foreach (Instruction instruction in list)
    {
        Stack<char> craneCapacity = new();
        for (int i = 0; i < instruction.CrateCount; i++)
        {
            craneCapacity.Push(crateStacks[instruction.IndexOrigin].Pop());
        }

        foreach (char crate in craneCapacity)
        {
            crateStacks[instruction.IndexDestination].Push(crate);
        }

        Console.WriteLine(
            $"Moving {instruction.CrateCount} crate(s) SIMULTANEOUSLY from {instruction.IndexOrigin + 1} to {instruction.IndexDestination + 1}");

        PrintCrates();
    }
}

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