string[] lines = System.IO.File.ReadAllLines("./input");

Folder fileSystem = ParseFileSystem(lines);

const int MaxSize = 100_000;
int part01 = TotalOfLimitedFolderSize(MaxSize, fileSystem);
Console.WriteLine($"Total Of Folders with size under {MaxSize}: {part01}");

const int TotalDiskSpace = 70_000_000;
const int RequiredUnusedSpace = 30_000_000;
int usedSpace = fileSystem.GetSize();
int currentUnusedSpace = TotalDiskSpace - usedSpace;
int spaceNeeded = RequiredUnusedSpace - currentUnusedSpace;
int part02 = SizeOfSmallestDirectoryToClearUpSpace(spaceNeeded, fileSystem);
Console.WriteLine($"Size of folder that can clear up {spaceNeeded} space: {part02}");

int SizeOfSmallestDirectoryToClearUpSpace(int spaceNeeded, Folder system) => GetAllFolders(system)
    .Select(folder => folder.GetSize()).Where(i => i > spaceNeeded).MinBy(i => i);

int TotalOfLimitedFolderSize(int maxSize, Folder fileSystem)
{
    List<Folder> fittedFolders = new();

    if (fileSystem.GetSize() <= maxSize)
    {
        fittedFolders.Add(fileSystem);
    }

    fittedFolders.AddRange(GetFoldersOfMaximumSize(maxSize, fileSystem));

    int totalOfLimitedFolderSize = fittedFolders.Sum(f => f.GetSize());

    return totalOfLimitedFolderSize;
}

List<Folder> GetAllFolders(Folder root)
{
    var result = new List<Folder>();

    foreach (Folder folder in root.Folders)
    {
        result.Add(folder);
        result.AddRange(GetAllFolders(folder));
    }

    return result;
}

List<Folder> GetFoldersOfMaximumSize(int maxSize, Folder root)
{
    var result = new List<Folder>();

    foreach (Folder folder in root.Folders)
    {
        if (folder.GetSize() <= maxSize)
        {
            result.Add(folder);
        }

        result.AddRange(GetFoldersOfMaximumSize(maxSize, folder));
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

        string firstSegment = segments[0];

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