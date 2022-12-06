using System.Text;

const int markerSize = 14; //4

using FileStream stream = File.OpenRead("input.txt");
var encoding = new UTF8Encoding(true);
byte[] segment = new byte[1];

Queue<char> buffer = new();
int processed = 0;

while (stream.Read(segment, 0, segment.Length) > 0)
{
    if (buffer.Distinct().Count() == markerSize)
    {
        Console.WriteLine($"Found packet start marker:\t({string.Join("", buffer)}) after ({processed}) characters processed.");
        return;
    }

    if (buffer.Count == markerSize) buffer.Dequeue();
    
    buffer.Enqueue(encoding.GetChars(segment)[0]);
    processed++;
}