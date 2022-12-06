using System.Text;

const int messageMarker = 14; //4

using FileStream stream = File.OpenRead("input.txt");

byte[] initialSegment = new byte[messageMarker];
var encoding = new UTF8Encoding(true);

Queue<char> buffer = new();

if (stream.Read(initialSegment, 0, initialSegment.Length) > 0)
{
    encoding.GetChars(initialSegment).ToList().ForEach(c => buffer.Enqueue(c));
}

int processed = messageMarker;
byte[] segment = new byte[1];

while (stream.Read(segment, 0, segment.Length) > 0)
{
    if (buffer.Distinct().ToArray().Length == messageMarker)
    {
        Console.WriteLine(
            $"Found packet start marker:\t({string.Join("", buffer)}) after ({processed}) characters processed.");
        return;
    }

    buffer.Dequeue();
    buffer.Enqueue(encoding.GetChars(segment)[0]);
    processed++;
}