namespace Day04;

internal static partial class Program {
  private const string Title = "\n## Day 4: Printing Department ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/4";
  private const long ExpectedPartOne = 1604;
  private const long ExpectedPartTwo = 9397;

  private static readonly (int dx, int dy)[] _directions = [(0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1)];

  private static long PartOne(string[] data) {
    var grid = GetGrid(data);
    var height = data.Length;
    var width = data[0].Length;

    long tally = 0;

    for (var y = 0; y < height; y++) {
      for (var x = 0; x < width; x++) {
        if (grid[y][x] != '@')
          continue;
        var counter = GetCount(x, y, height, width, grid);
        if (counter < 4)
          tally++;
      }
    }

    return tally;
  }

  private static long PartTwo(string[] data) {
    var grid = GetGrid(data);
    var height = data.Length;
    var width = data[0].Length;
    List<(int x, int y)> toBeRemoved = [];

    long tally = 0;

    while (true) {
      toBeRemoved.Clear();
      for (var y = 0; y < height; y++) {
        for (var x = 0; x < width; x++) {
          if (grid[y][x] != '@')
            continue;
          var counter = GetCount(x, y, height, width, grid);
          if (counter < 4)
            toBeRemoved.Add((x, y));
        }
      }
      if (toBeRemoved.Count == 0)
        break;

      tally += toBeRemoved.Count;
      foreach (var (x, y) in toBeRemoved)
        grid[y][x] = '.';
    }

    return tally;
  }

  private static int GetCount(int x, int y, int height, int width, char[][] grid) {
    var counter = 0;
    foreach (var (dx, dy) in _directions) {
      var (nx, ny) = (x + dx, y + dy);
      if (!IsInBounds(nx, ny, height, width))
        continue;
      if (grid[ny][nx] == '@')
        counter++;
      if (counter >= 4)
        break;
    }
    return counter;
  }

  private static bool IsInBounds(int x, int y, int height, int width) {
    return x >= 0 && x < width && y >= 0 && y < height;
  }

  private static char[][] GetGrid(string[] data) {
    var grid = new List<char[]>();
    foreach (var row in data)
      grid.Add(row.ToCharArray());
    return grid.ToArray();
  }
}
