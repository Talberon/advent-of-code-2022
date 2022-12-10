// Day 10: Cathode-Ray Tube

var lines = new Queue<string>(File.ReadAllLines("test_input"));

const bool debugMode = false;
Action<string?> debugConsole = debugMode ? Console.WriteLine : (_) => { };

// `addx V` takes two cycles to complete. After two cycles, the X register is increased by the value V. (V can be negative.)
// `noop` takes one cycle to complete. It has no other effect.

// Find the signal strength during the 20th, 60th, 100th, 140th, 180th, and 220th cycles. What is the sum of these six signal strengths?

//PART 01

const int start = 20;
const int interval = 40;
const int intervalIterations = 5;
const int intervalLag = 2;
int signalStrengthTotal = DetermineSignalStrength(start, intervalIterations, interval, lines, intervalLag);

//Assumes input ends with several no-ops.
Console.WriteLine($"Done! Total Signal Strength: {signalStrengthTotal}");

int DetermineSignalStrength(int start1, int intervalIterations1, int interval1, Queue<string> queue, int intervalLag1)
{
    List<int> intervalsToCheck = new() { start1 };
    for (int i = 0; i < intervalIterations1; i++)
    {
        intervalsToCheck.Add(intervalsToCheck[^1] + interval1);
    }

    Console.WriteLine($"Intervals to check: {string.Join(", ", intervalsToCheck)}");

    int x = 1;
    (int addition, int cyclesRemaining) pending = new(0, -1);
    int signalStrengthTotal1 = 0;


    for (int cycle = 1; cycle <= intervalsToCheck[^1]; cycle++)
    {
        if (pending.cyclesRemaining > 0)
        {
            debugConsole($"[Cycle {cycle}] Waiting...");
            pending.cyclesRemaining--;

            UpdateSignalStrength();

            continue;
        }

        if (pending.cyclesRemaining == 0)
        {
            x += pending.addition;
            debugConsole($"[Cycle {cycle}] Adding {pending.addition} to X -> X = {x}");
        }

        UpdateSignalStrength();

        string line = queue.Count > 0 ? queue.Dequeue() : "";
        debugConsole($"[Cycle {cycle}] Current line: {line} | X = {x}");


        if (line.StartsWith("addx"))
        {
            int addition = Convert.ToInt32(line.Split(" ")[1]);

            pending = (addition, intervalLag: intervalLag1);

            debugConsole($"[Cycle {cycle}] {line} | Queueing ({addition}) to be added...");
        }


        debugConsole("");

        pending.cyclesRemaining--;

        void UpdateSignalStrength()
        {
            if (!intervalsToCheck.Contains(cycle)) return;
            Console.WriteLine($"[Cycle {cycle}] X = {x}");
            signalStrengthTotal1 += x * cycle;
        }
    }

    return signalStrengthTotal1;
}
