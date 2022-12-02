string[] lines = File.ReadAllLines("./input.txt");

List<int> ElfCalories(string[] lines)
{
  List<int> elfInventories = new();
  List<int> currentInventory = new();

  foreach (var line in lines)
  {
    if (line.Length == 0)
    {
      elfInventories.Add(currentInventory.Sum());
      currentInventory = new();
      continue;
    }
    currentInventory.Add(Convert.ToInt32(line));
  }

  elfInventories.Add(currentInventory.Sum());

  return elfInventories;
}

var calorieGroups = ElfCalories(lines).OrderByDescending(total => total).ToList();

Console.WriteLine($"Highest total: {calorieGroups.First()}. Top Three: {calorieGroups.Take(3).Sum()}");