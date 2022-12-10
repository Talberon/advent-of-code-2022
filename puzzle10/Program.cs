// Day 10: Cathode-Ray Tube

var lines = new Queue<string>(File.ReadAllLines("test_input"));

const bool debugMode = false;
Action<string?> debugConsole = debugMode ? Console.WriteLine : (_) => { };

// `addx V` takes two cycles to complete. After two cycles, the X register is increased by the value V. (V can be negative.)
// `noop` takes one cycle to complete. It has no other effect.

// Find the signal strength during the 20th, 60th, 100th, 140th, 180th, and 220th cycles. What is the sum of these six signal strengths?

const int start = 20;
const int interval = 40;
const int intervalIterations = 5;
const int intervalLag = 2;

List<int> intervalsToCheck = new() { start };
for (int i = 0; i < intervalIterations; i++)
{
    intervalsToCheck.Add(intervalsToCheck[^1] + interval);
}

Console.WriteLine($"Intervals to check: {string.Join(", ", intervalsToCheck)}");

// How CRT Works:
// CRT is an array of bools (representing pixels) bool[]
// - Start at 0
// - Scan from left to right 3 pixels wide from CURRENT REGISTER (X) as the MIDDLE

// - After drawing everything, print screen, newlining every crtWidth(40) pixels

const int crtWidth = 40;
const int crtHeight = 6;
string crtScreen = "";


int x = 1;
(int addition, int cyclesRemaining) pending = new(0, -1);
int signalStrengthTotal = 0;


for (int cycle = 1; cycle <= intervalsToCheck[^1]; cycle++)
{
    if (pending.cyclesRemaining > 0)
    {
        pending.cyclesRemaining--;
        UpdateSignalStrength();
        continue;
    }

    if (pending.cyclesRemaining == 0)
    {
        x += pending.addition;
    }

    UpdateSignalStrength();

    string line = lines.Count > 0 ? lines.Dequeue() : "";
    debugConsole($"[{cycle:000}] Start cycle \t{cycle}: begin executing {line}");

    if (line.StartsWith("addx"))
    {
        int addition = Convert.ToInt32(line.Split(" ")[1]);
        pending = (addition, intervalLag);
    }

    pending.cyclesRemaining--;

    debugConsole("");

    //Methods

    void UpdateSignalStrength()
    {
        if (!intervalsToCheck.Contains(cycle)) return;
        debugConsole($"[{cycle:000}] X = {x}");
        signalStrengthTotal += x * cycle;
    }
}

//Assumes input ends with several no-ops.
Console.WriteLine($"Done! Total Signal Strength: {signalStrengthTotal}");


// int totalCycles = lines.Select(line => line.StartsWith("addx") ? 2 : 1).Sum();