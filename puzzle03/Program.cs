// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

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

int prioritySum = 0;
foreach (string line in lines)
{
    int midpoint = line.Length / 2;
    string leftCompartment = line[..midpoint];
    string rightCompartment = line[midpoint..];

    Console.WriteLine($"({leftCompartment} | {rightCompartment})");

    char duplicate = rightCompartment.First(c => leftCompartment.Contains(c));

    int priorityValue = (int)Enum.Parse<ItemPriority>(duplicate.ToString());
    
    Console.WriteLine($"Found: ({duplicate}), Value: ({priorityValue})");

    prioritySum += priorityValue;
}

Console.WriteLine($"Total Priority Sum: {prioritySum}");



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
