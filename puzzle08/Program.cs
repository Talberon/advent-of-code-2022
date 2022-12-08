string[] lines = File.ReadAllLines("input");

List<List<int>> grid = lines.Select(line => line.Select(c => Convert.ToInt32(c.ToString())).ToList()).ToList();

int width = grid[0].Count - 1;
int height = grid.Count - 1;

int visibleTrees = 0;

for (int rowIndex = 0; rowIndex < grid.Count; rowIndex++)
{
    List<int> row = grid[rowIndex];
    for (int columnIndex = 0; columnIndex < row.Count; columnIndex++)
    {
        if (VisibleFromTop(grid, rowIndex, columnIndex))
        {
            Console.WriteLine($"Tree ({row[columnIndex]}) at (X{columnIndex + 1},Y{rowIndex + 1}) is visible from TOP!");
            visibleTrees++;
        }
        else if (VisibleFromBottom(grid, rowIndex, columnIndex))
        {
            Console.WriteLine($"Tree ({row[columnIndex]}) at (X{columnIndex + 1},Y{rowIndex + 1}) is visible from BOTTOM!");
            visibleTrees++;
        }
        else if (VisibleFromLeft(grid, rowIndex, columnIndex))
        {
            Console.WriteLine($"Tree ({row[columnIndex]}) at (X{columnIndex + 1},Y{rowIndex + 1}) is visible from LEFT!");
            visibleTrees++;
        }
        else if (VisibleFromRight(grid, rowIndex, columnIndex))
        {
            Console.WriteLine($"Tree ({row[columnIndex]}) at (X{columnIndex + 1},Y{rowIndex + 1}) is visible from RIGHT!");
            visibleTrees++;
        }
        else
        {
            Console.WriteLine($"Tree ({row[columnIndex]}) at (X{columnIndex + 1},Y{rowIndex + 1}) is NOT VISIBLE!");
        }
    }
}

Console.WriteLine($"Visible Trees: {visibleTrees}");

bool VisibleFromLeft(List<List<int>> grid, int row, int column)
{
    if (column == 0) return true;

    int originSize = grid[row][column];

    for (int currentColumn = column; currentColumn >= 0; currentColumn--)
    {
        if (currentColumn == column) continue;
        
        int blocking = grid[row][currentColumn];
        if (blocking >= originSize)
        {
            Console.WriteLine($"Tree ({originSize}) at (X{column+1},Y{row+1}) is blocked by ({blocking}) at (X{currentColumn+1},Y{row+1}) from the LEFT!");
            return false;
        }
    }

    Console.WriteLine($"Tree ({originSize}) at (X{column+1},Y{row+1}) is visible from the LEFT!");
    return true;
}

bool VisibleFromRight(List<List<int>> grid, int row, int column)
{
    if (column == width) return true;

    int originSize = grid[row][column];

    for (int currentColumn = column; currentColumn <= width; currentColumn++)
    {
        if (currentColumn == column) continue;
        
        int blocking = grid[row][currentColumn];
        if (blocking >= originSize)
        {
            Console.WriteLine($"Tree ({originSize}) at (X{column+1},Y{row+1}) is blocked by ({blocking}) at (X{currentColumn+1},Y{row+1}) from the RIGHT!");
            return false;
        }
    }

    Console.WriteLine($"Tree ({originSize}) at (X{column+1},Y{row+1}) is visible from the RIGHT!");
    return true;
}

bool VisibleFromTop(List<List<int>> grid, int row, int column)
{
    if (row == 0) return true;

    int originSize = grid[row][column];

    for (int currentRow = row; currentRow >= 0; currentRow--)
    {
        if (currentRow == row) continue;
        
        int blocking = grid[currentRow][column];
        if (blocking >= originSize)
        {
            Console.WriteLine($"Tree ({originSize}) at (X{column+1},Y{row+1}) is blocked by ({blocking}) at (X{column+1},Y{currentRow+1}) from the TOP!");
            return false;
        }
    }

    Console.WriteLine($"Tree ({originSize}) at (X{column+1},Y{row+1}) is visible from the TOP!");
    return true;
}

bool VisibleFromBottom(List<List<int>> grid, int row, int column)
{
    if (row == height) return true;

    int originSize = grid[row][column];

    for (int currentRow = row; currentRow <= height; currentRow++)
    {
        if (currentRow == row) continue;
        
        int blocking = grid[currentRow][column];
        if (blocking >= originSize)
        {
            
            Console.WriteLine($"Tree ({originSize}) at (X{column+1},Y{row+1}) is blocked by ({blocking}) at (X{column+1},Y{currentRow+1}) from the BOTTOM!");
            return false;
        }
    }

    Console.WriteLine($"Tree ({originSize}) at (X{column+1},Y{row+1}) is visible from the BOTTOM!");
    return true;
}