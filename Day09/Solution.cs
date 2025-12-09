using AocHelper;

namespace Day09;

internal static partial class Program
{
  private const string Title = "\n## Day 9: Movie Theater ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/9";
  private const long ExpectedPartOne = 4760959496;
  private const long ExpectedPartTwo = 1343576598;

  private static PriorityQueue<((long x, long y) a, (long x, long y) b), long> maxAreas = new(Comparer<long>.Create((a, b) => b.CompareTo(a)));
  private static long PartOne(string data)
  {
    var tiles = GetTiles(data);
    for (var i = 0; i < tiles.Length; i++) {
      var (ax, ay) = tiles[i];
      for (var j = i + 1; j < tiles.Length; j++) {
        if (i == j)
          continue;
        var (bx, by) = tiles[j];
        long area = (Math.Abs(ax - bx) + 1) * (Math.Abs(ay - by) + 1);
        maxAreas.Enqueue(((ax, ay), (bx, by)), area);
      }
    }
    maxAreas.TryPeek(out _, out var maxArea);

    return maxArea;
  }

  private static long PartTwo(string data)
  {
    var tiles = GetTiles(data);
    var edges = CreateEdges(tiles);

    long maxArea;
    while (true) {
      if (!maxAreas.TryDequeue(out var item, out maxArea))
        break;
      var (a, b) = item;

      if (IsContainedByPolygon(a, b, edges)) {
        break;
      }
    }

    return maxArea;
  }

  private static bool IsContainedByPolygon((long x, long y) a, (long x, long y) b, HashSet<(long x, long y)> edges)
  {
    if (a.y > b.y) (a.y, b.y) = (b.y, a.y);
    if (a.x > b.x) (a.x, b.x) = (b.x, a.x);
    // search along top row for tiles and edges
    var y = a.y + 1;
    for (var x = a.x + 1; x < b.x; x++) {
      if (edges.Contains((x, y))) {
        return false;
      }
    }
    // search along left column for tiles and edges
    for (y = a.y + 2; y < b.y; y++) {
      var x = a.x + 1;
      if (edges.Contains((x, y))) {
        return false;
      }
    }
    // search along right column for tiles and edges
    for (y = a.y + 2; y < b.y; y++) {
      var x = b.x - 1;
      if (edges.Contains((x, y))) {
        return false;
      }
    }
    // search along bottom row for tiles and edges
    y = b.y - 1;
    for (var x = a.x + 1; x < b.x; x++) {
      if (edges.Contains((x, y))) {
        return false;
      }
    }

    return true;
  }

  private static HashSet<(long x, long y)> CreateEdges((long x, long y)[] tiles)
  {
    var edges = new HashSet<(long, long)>();
    for (var i = 0; i < tiles.Length - 1; i++) {
      CreateEdge(tiles[i], tiles[i + 1], edges);
    }
    CreateEdge(tiles[0], tiles[^1], edges);
    return edges;
  }

  private static void CreateEdge((long x, long y) a, (long x, long y) b, HashSet<(long x, long y)> edges)
  {
    if (a.x == b.x) {
      if (a.y > b.y) (a.y, b.y) = (b.y, a.y);
      for (var y = a.y; y <= b.y; y++) {
        edges.Add((a.x, y));
      }
    } else if (a.y == b.y) {
      if (a.x > b.x) (a.x, b.x) = (b.x, a.x);
      for (var x = a.x; x <= b.x; x++) {
        edges.Add((x, a.y));
      }
    }
  }

  private static (long x, long y)[] GetTiles(string data)
  {
    var tiles = new List<(long, long)>();

    foreach (var line in data.Split('\n', StringSplitOptions.RemoveEmptyEntries)) {
      var (x, y) = line.Split(',').ToLongTuplePair();
      tiles.Add((x, y));
    }
    return tiles.ToArray();
  }
}
