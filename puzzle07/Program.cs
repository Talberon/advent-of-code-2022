string[] lines = System.IO.File.ReadAllLines("./input");

// $ cd $LETTER - move in one level
// $ cd .. - move out one level
// $ cd / - moves to root
// $ ls - list files and folder in current level

const int MaxSize = 100_000;

var fileSystem = ParseFileSystem(lines);

List<Folder> fittedFolders = new();

if (fileSystem.GetSize() <= MaxSize)
{
    fittedFolders.Add(fileSystem);
}

fittedFolders.AddRange(GetFoldersOfSize(MaxSize, fileSystem));

int totalOfLimitedFolderSize = fittedFolders.Sum(f => f.GetSize());
Console.WriteLine($"Sum: {totalOfLimitedFolderSize}");

return;

List<Folder> GetFoldersOfSize(int maxSize, Folder root)
{
    var result = new List<Folder>();

    foreach (Folder folder in root.Folders)
    {
        if (folder.GetSize() <= maxSize)
        {
            result.Add(folder);
        }

        result.AddRange(GetFoldersOfSize(maxSize, folder));
    }

    return result;
}

Folder ParseFileSystem(string[] input)
{
    var rootFolder = new Folder
    {
        Name = "/",
        Folders = new List<Folder>(),
        Files = new List<File>()
    };

    Folder currentFolder = rootFolder;

    foreach (string line in input)
    {
        string[] segments = line.Split(" ");

        var firstSegment = segments[0];

        if (firstSegment == "$")
        {
            string command = segments[1];

            if (command == "cd")
            {
                string desiredFolder = segments[2];

                if (desiredFolder == "..")
                {
                    currentFolder = currentFolder.ParentFolder;
                }
                else if (desiredFolder == "/")
                {
                    currentFolder = rootFolder;
                }
                else
                {
                    currentFolder = currentFolder.Folders.First(folder => folder.Name == desiredFolder);
                }
            }
            else if (command == "ls")
            {
                continue;
            }
        }
        else if (firstSegment == "dir")
        {
            currentFolder.Folders.Add(new Folder
            {
                Name = segments[1],
                Folders = new List<Folder>(),
                Files = new List<File>(),
                ParentFolder = currentFolder
            });
        }
        else if (firstSegment.All(char.IsDigit))
        {
            currentFolder.Files.Add(new File
            {
                Size = Convert.ToInt32(segments[0]),
                Name = segments[1]
            });
        }
    }

    return rootFolder;
}

internal class Folder
{
    public Folder? ParentFolder { get; init; }
    public string Name { get; init; }
    public List<Folder> Folders { get; init; }
    public List<File> Files { get; init; }

    public int GetSize(int runningTotal = 0) => Files.Sum(f => f.Size) + Folders.Sum(f => f.GetSize(runningTotal));
}

internal struct File
{
    public string Name { get; init; }
    public int Size { get; init; }
}