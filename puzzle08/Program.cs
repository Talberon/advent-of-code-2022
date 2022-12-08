string[] lines = File.ReadAllLines("input");

List<List<int>> grid = lines.Select(line => line.Select(c => Convert.ToInt32(c.ToString())).ToList()).ToList();

int width = grid[0].Count - 1;
int height = grid.Count - 1;

int visibleTrees = VisibleTrees(grid);
Console.WriteLine($"Visible Trees: {visibleTrees}");

int bestScenicScore = GetBestScenicScore(grid);
Console.WriteLine($"Best Score: {bestScenicScore}");

int GetBestScenicScore(List<List<int>> grid)
{
    int bestScore = 0;

    for (int rowIndex = 0; rowIndex < grid.Count; rowIndex++)
    {
        List<int> row = grid[rowIndex];
        for (int columnIndex = 0; columnIndex < row.Count; columnIndex++)
        {
            int currentScore = ScenicScoreForTop(grid, rowIndex, columnIndex) *
                               ScenicScoreForBottom(grid, rowIndex, columnIndex) *
                               ScenicScoreForLeft(grid, rowIndex, columnIndex) *
                               ScenicScoreForRight(grid, rowIndex, columnIndex);

            if (currentScore > bestScore) bestScore = currentScore;
        }
    }

    return bestScore;
}


int VisibleTrees(List<List<int>> grid)
{
    int visibleTrees = 0;

    for (int rowIndex = 0; rowIndex < grid.Count; rowIndex++)
    {
        List<int> row = grid[rowIndex];
        for (int columnIndex = 0; columnIndex < row.Count; columnIndex++)
        {
            if (VisibleFromTop(grid, rowIndex, columnIndex))
            {
                // Console.WriteLine($"Tree ({row[columnIndex]}) at (X{columnIndex + 1},Y{rowIndex + 1}) is visible from TOP!");
                visibleTrees++;
            }
            else if (VisibleFromBottom(grid, rowIndex, columnIndex))
            {
                // Console.WriteLine($"Tree ({row[columnIndex]}) at (X{columnIndex + 1},Y{rowIndex + 1}) is visible from BOTTOM!");
                visibleTrees++;
            }
            else if (VisibleFromLeft(grid, rowIndex, columnIndex))
            {
                // Console.WriteLine($"Tree ({row[columnIndex]}) at (X{columnIndex + 1},Y{rowIndex + 1}) is visible from LEFT!");
                visibleTrees++;
            }
            else if (VisibleFromRight(grid, rowIndex, columnIndex))
            {
                // Console.WriteLine($"Tree ({row[columnIndex]}) at (X{columnIndex + 1},Y{rowIndex + 1}) is visible from RIGHT!");
                visibleTrees++;
            }
            else
            {
                // Console.WriteLine($"Tree ({row[columnIndex]}) at (X{columnIndex + 1},Y{rowIndex + 1}) is NOT VISIBLE!");
            }
        }
    }

    return visibleTrees;
}

//Visible

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
            // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is blocked by ({blocking}) at (X{currentColumn + 1},Y{row + 1}) from the LEFT!");
            return false;
        }
    }

    // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is visible from the LEFT!");
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
            // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is blocked by ({blocking}) at (X{currentColumn + 1},Y{row + 1}) from the RIGHT!");
            return false;
        }
    }

    // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is visible from the RIGHT!");
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
            // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is blocked by ({blocking}) at (X{column + 1},Y{currentRow + 1}) from the TOP!");
            return false;
        }
    }

    // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is visible from the TOP!");
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
            // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is blocked by ({blocking}) at (X{column + 1},Y{currentRow + 1}) from the BOTTOM!");
            return false;
        }
    }

    // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is visible from the BOTTOM!");
    return true;
}

//Scenic
int ScenicScoreForLeft(List<List<int>> grid, int row, int column)
{
    if (column == 0) return 0;

    int originSize = grid[row][column];

    int scenicScore = 0;
    for (int currentColumn = column; currentColumn >= 0; currentColumn--)
    {
        if (currentColumn == column) continue;
        scenicScore++;

        int blocking = grid[row][currentColumn];
        if (blocking >= originSize)
        {
            // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is blocked by ({blocking}) at (X{currentColumn + 1},Y{row + 1}) from the LEFT!\tSCORE: {scenicScore}");
            return scenicScore;
        }
    }

    // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is visible from the LEFT!");
    return scenicScore;
}

int ScenicScoreForRight(List<List<int>> grid, int row, int column)
{
    if (column == width) return 0;

    int originSize = grid[row][column];

    int scenicScore = 0;
    for (int currentColumn = column; currentColumn <= width; currentColumn++)
    {
        if (currentColumn == column) continue;
        scenicScore++;

        int blocking = grid[row][currentColumn];
        if (blocking >= originSize)
        {
            // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is blocked by ({blocking}) at (X{currentColumn + 1},Y{row + 1}) from the RIGHT!\tSCORE: {scenicScore}");
            return scenicScore;
        }
    }

    // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is visible from the RIGHT!");
    return scenicScore;
}

int ScenicScoreForTop(List<List<int>> grid, int row, int column)
{
    if (row == 0) return 0;

    int originSize = grid[row][column];

    int scenicScore = 0;
    for (int currentRow = row; currentRow >= 0; currentRow--)
    {
        if (currentRow == row) continue;
        scenicScore++;

        int blocking = grid[currentRow][column];
        if (blocking >= originSize)
        {
            // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is blocked by ({blocking}) at (X{column + 1},Y{currentRow + 1}) from the TOP!\tSCORE: {scenicScore}");
            return scenicScore;
        }
    }

    // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is visible from the TOP!");
    return scenicScore;
}

int ScenicScoreForBottom(List<List<int>> grid, int row, int column)
{
    if (row == height) return 0;

    int originSize = grid[row][column];

    int scenicScore = 0;
    for (int currentRow = row; currentRow <= height; currentRow++)
    {
        if (currentRow == row) continue;
        scenicScore++;

        int blocking = grid[currentRow][column];
        if (blocking >= originSize)
        {
            // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is blocked by ({blocking}) at (X{column + 1},Y{currentRow + 1}) from the BOTTOM!\tSCORE: {scenicScore}");
            return scenicScore;
        }
    }

    // Console.WriteLine($"Tree ({originSize}) at (X{column + 1},Y{row + 1}) is visible from the BOTTOM!");
    return scenicScore;
}