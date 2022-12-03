/*
 * Rucksack
 * 2 large compartments
 * One item type per rucksack (ie: items in one side should not have a duplicate in the other side)
 *
 * lowercase and uppercase represent different items even if they are the same letter
 *
 * each rucksack is a list of items on a single line
 *  the first half of the line represents the first compartment
 *  the second half of the line represents the second compartment
 *
 * Lower a-z has priority 1-26
 * Upper A-Z have prioerities 27-52
 *
 * Step 1: Find the duplicate
 * Step 2: Sum the duplicate's priority to the total
 */


string[] lines = File.ReadAllLines("./input.txt");

//Part 01
int FindDuplicatePriorities(string[] rucksacks)
{
    int prioritySum = 0;
    foreach (string rucksack in rucksacks)
    {
        int midpoint = rucksack.Length / 2;
        string leftCompartment = rucksack[..midpoint];
        string rightCompartment = rucksack[midpoint..];

        // Console.WriteLine($"({leftCompartment} | {rightCompartment})");

        char duplicate = rightCompartment.First(c => leftCompartment.Contains(c));

        int priorityValue = (int)Enum.Parse<ItemPriority>(duplicate.ToString());

        // Console.WriteLine($"Found: ({duplicate}), Value: ({priorityValue})");

        prioritySum += priorityValue;
    }

    return prioritySum;
}

Console.WriteLine($"Total Priority Sum: {FindDuplicatePriorities(lines)}");

//Part 02
int FindGroupPriorities(Queue<string> rucksacks)
{
    int prioritySum = 0;

    while (rucksacks.Count > 0)
    {
        List<string> group = rucksacks.BatchOf(3);
        Dictionary<char, int> occurences = new();

        foreach (string rucksack in group)
        {
            string distinctItems = new(rucksack.ToCharArray().Distinct().ToArray());
            
            Console.WriteLine($"Distinct items: {distinctItems}");
            
            foreach (char letter in distinctItems)
            {
                if (occurences.ContainsKey(letter)) occurences[letter]++;
                else
                {
                    occurences.Add(letter, 1);
                }
            }
        }

        Console.WriteLine($"Occurences: {string.Join(",", occurences)}");

        var duplicates = occurences.Where(kvp => kvp.Value == 3).Select(kvp => kvp.Key).ToList();

        if (duplicates.Count() > 1) throw new Exception($"There should not be multiple duplicates! Found: {string.Join(",", duplicates)}");
        
        char duplicate = duplicates.First();
        int priorityValue = (int)Enum.Parse<ItemPriority>(duplicate.ToString());
        
        Console.WriteLine($"Identified duplicate: {duplicate} which is worth:\t{priorityValue}");

        prioritySum += priorityValue;
    }

    return prioritySum;
}

Console.WriteLine($"Total Group Priority Sum: {FindGroupPriorities(new Queue<string>(lines))}");


static class QueueExtensions
{
    public static List<T> BatchOf<T>(this Queue<T> me, int amountToPop)
    {
        List<T> result = new();
        for (int i = 0; i < amountToPop && me.Count > 0; i++)
        {
            result.Add(me.Dequeue());
        }

        return result;
    }
}

enum ItemPriority
{
    Invalid,
    a,
    b,
    c,
    d,
    e,
    f,
    g,
    h,
    i,
    j,
    k,
    l,
    m,
    n,
    o,
    p,
    q,
    r,
    s,
    t,
    u,
    v,
    w,
    x,
    y,
    z,
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z
}