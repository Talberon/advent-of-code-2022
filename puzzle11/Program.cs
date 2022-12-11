// See https://aka.ms/new-console-template for more information

string[] lines = File.ReadAllLines("input");


//Focus on two MOST_ACTIVE monkeys
// Count total number of times the two MOST_ACTIVE monkey inspects items over 20 Rounds (INSPECTION_COUNT)
// MONKEY_BUSINESS = each monkey's INSPECTION_COUNT multiplied together
// RETURN MONKEY_BUSINESS

const int rounds = 20;

List<Monkey> monkeys = ParseMonkeys(lines);

//On a single monkey's turn it inspects and throws all items it is holding one-at-a-time from first-to-last (left to right)

for (int round = 1; round <= rounds; round++)
{
    foreach (Monkey currentMonkey in monkeys)
    {
        while (currentMonkey.Items.Count > 0)
        {
            int item = currentMonkey.Items.First();
            int newWorryLevel = currentMonkey.Operation(item);

            int monkeyToThrowTo = currentMonkey.Inspect(newWorryLevel);

            currentMonkey.Items.RemoveAt(0);
            monkeys[monkeyToThrowTo].Items.Add(newWorryLevel);
        }
    }

    Console.WriteLine($"[Round {round}] Monkey inventories:");
    for (int index = 0; index < monkeys.Count; index++)
    {
        Monkey monkey = monkeys[index];
        Console.WriteLine($"Monkey {index} (inspected [{monkey.InspectionCount}] items):\t{string.Join(", ", monkey.Items)}");
    }
}

int[] topMonkeyCounts = monkeys.Select(m => m.InspectionCount).OrderByDescending(count => count).ToArray();
int monkeyBusiness = topMonkeyCounts[0] * topMonkeyCounts[1];

Console.WriteLine($"Monkey Business: {topMonkeyCounts[0]} * {topMonkeyCounts[1]} = [{monkeyBusiness}]");


List<Monkey> ParseMonkeys(IReadOnlyList<string> instructions)
{
    var parsedMonkeys = new List<Monkey>();

    //Assume reach record is 6 lines long + whitespace line
    for (int i = 0; i < instructions.Count; i += 7)
    {
        string startingItemsLine = instructions[i + 1];
        string operationLine = instructions[i + 2];
        string testLine = instructions[i + 3];
        string ifTrueLine = instructions[i + 4];
        string ifFalseLine = instructions[i + 5];

        int[] startingItems = startingItemsLine.Replace("  Starting items: ", "")
            .Split(", ")
            .Select(s => Convert.ToInt32(s))
            .ToArray();

        bool willMultiply = operationLine.Contains('*');
        string lastSegment = operationLine.Split(" ").Last();

        int? operationNumber = lastSegment == "old" ? null : Convert.ToInt32(lastSegment);

        int divisibleBy = Convert.ToInt32(testLine.Split(" ").Last());

        int trueIndex = Convert.ToInt32(ifTrueLine.Split(" ").Last());
        int falseIndex = Convert.ToInt32(ifFalseLine.Split(" ").Last());

        var parsedMonkey = new Monkey(startingItems)
        {
            WillMultiplyInsteadOfAdd = willMultiply,
            OperationNumber = operationNumber,
            DivisibleBy = divisibleBy,
            PassMonkeyIndex = trueIndex,
            FailMonkeyIndex = falseIndex,
        };

        parsedMonkeys.Add(parsedMonkey);
    }

    return parsedMonkeys;
}

internal class Monkey
{
    public int InspectionCount { get; private set; }
    
    public List<int> Items { get; }

    public bool WillMultiplyInsteadOfAdd { private get; init; }
    public int? OperationNumber { private get; init; }

    public int DivisibleBy { private get; init; }

    public int PassMonkeyIndex { private get; init; }
    public int FailMonkeyIndex { private get; init; }

    public Monkey(IEnumerable<int> startingItems)
    {
        Items = startingItems.ToList();
        InspectionCount = 0;
    }

    public int Operation(int item) => (WillMultiplyInsteadOfAdd
        ? item * (OperationNumber ?? item)
        : item + (OperationNumber ?? item)) / 3;

    /*
     * After each monkey inspects an item but before it tests your worry level,
     * your relief that the monkey's inspection didn't damage the item causes
     * your worry level to be divided by three and rounded down to the nearest
     * integer.
     */
    public int Inspect(int item)
    {
        InspectionCount++;
        return item % DivisibleBy == 0 ? PassMonkeyIndex : FailMonkeyIndex;
    }
}