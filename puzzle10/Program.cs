// Day 10: Cathode-Ray Tube

using System.Text.RegularExpressions;

var lines = new Queue<string>(File.ReadAllLines("input"));

List<int> signalIntervals = new() { 20, 60, 100, 140, 180, 220 };

int x = 1;

//Part 1
const int cycleLag = 2;
(int addition, int cyclesRemaining) pending = new(0, -1);
int signalStrengthTotal = 0;

//Part 2
const int crtWidth = 40;
const int crtHeight = 6;
string crtScreen = "";

for (int cycle = 1; cycle <= crtWidth * crtHeight; cycle++)
{
    if (pending.cyclesRemaining > 0)
    {
        pending.cyclesRemaining--;
        UpdateSignalStrength(cycle);
        UpdateCrtScreen();
        continue;
    }

    if (pending.cyclesRemaining == 0)
    {
        x += pending.addition;
    }

    UpdateSignalStrength(cycle);
    UpdateCrtScreen();

    string line = lines.Count > 0 ? lines.Dequeue() : "";

    if (line.StartsWith("addx"))
    {
        int addition = Convert.ToInt32(line.Split(" ")[1]);
        pending = (addition, cycleLag);
    }

    pending.cyclesRemaining--;
}

Console.WriteLine($"[Part 1] Total Signal Strength: {signalStrengthTotal}");
Console.WriteLine($"[Part 2] Screen:\n{ScreenOutput(crtScreen)}");

return;

//Methods

void UpdateSignalStrength(int cycle)
{
    if (!signalIntervals.Contains(cycle)) return;
    signalStrengthTotal += x * cycle;
}

void UpdateCrtScreen()
{
    int column = crtScreen.Length % crtWidth;
    if (x - 1 == column || x == column || x + 1 == column)
    {
        crtScreen += "#";
    }
    else
    {
        crtScreen += ".";
    }
}

string ScreenOutput(string input) => Regex.Replace(input, "(.{" + crtWidth + "})", "$1" + Environment.NewLine);