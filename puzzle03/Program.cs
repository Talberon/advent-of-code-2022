string[] lines = File.ReadAllLines("./input.txt");

//Part 01
int SumRucksackDuplicates(string[] rucksacks)
{
    return (from rucksack in rucksacks
        let midpoint = rucksack.Length / 2
        let leftCompartment = rucksack[..midpoint]
        let rightCompartment = rucksack[midpoint..]
        select rightCompartment.First(leftCompartment.Contains)
        into duplicate
        select (int)Enum.Parse<ItemPriority>(duplicate.ToString())).Sum();
}

Console.WriteLine($"Total Priority Sum: {SumRucksackDuplicates(lines)}");

//Part 02
int SumGroupPriorities(Queue<string> rucksacks, int groupSize)
{
    int prioritySum = 0;

    while (rucksacks.Count > 0)
    {
        IEnumerable<char> group = rucksacks.BatchOf(groupSize)
            .Select(rucksack => new string(rucksack.Distinct().ToArray()))
            .SelectMany(distinctItems => distinctItems);

        Dictionary<char, int> occurences = new();

        foreach (char letter in group)
        {
            if (occurences.ContainsKey(letter))
            {
                occurences[letter]++;
            }
            else
            {
                occurences.Add(letter, 1);
            }
        }

        List<char> duplicates = occurences.Where(kvp => kvp.Value == groupSize).Select(kvp => kvp.Key).ToList();

        if (duplicates.Count > 1)
        {
            throw new Exception($"There should not be multiple duplicates! Found: {string.Join(",", duplicates)}");
        }

        char badge = duplicates.First();
        int priorityValue = (int)Enum.Parse<ItemPriority>(badge.ToString());

        prioritySum += priorityValue;
    }

    return prioritySum;
}

int prioritySum = SumGroupPrioritiesLINQ(new Queue<string>(lines), 3);
Console.WriteLine($"LINQ: Total Group Priority Sum: {prioritySum}");

//Part 02 with LINQ!
int SumGroupPrioritiesLINQ(Queue<string> rucksacks, int groupSize)
{
    int prioritySum = 0;
    
    while (rucksacks.Count > 0)
    {
        var sackOne = rucksacks.Dequeue().ToCharArray();
        var sackTwo = rucksacks.Dequeue().ToCharArray();
        var sackThree = rucksacks.Dequeue().ToCharArray();

        var duplicates = (from item1 in sackOne
            join item2 in sackTwo on item1 equals item2
            join item3 in sackThree on item2 equals item3
            select item1).Distinct().ToList();
            
        if (duplicates.Count > 1)
        {
            throw new Exception($"There should not be multiple duplicates! Found: {string.Join(",", duplicates)}");
        }

        char badge = duplicates.First();
        int priorityValue = (int)Enum.Parse<ItemPriority>(badge.ToString());

        prioritySum += priorityValue;
    }

    return prioritySum;
}

int prioritySumLINQ = SumGroupPrioritiesLINQ(new Queue<string>(lines), 3);
Console.WriteLine($"LINQ: Total Group Priority Sum: {prioritySumLINQ}");


//Helpers
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