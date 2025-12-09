using AocHelper;

namespace Day09;

internal static partial class Program
{
  private const string Title = "\n## Day 9: Movie Theater ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/9";
  private const long ExpectedPartOne = 4760959496;
  private const long ExpectedPartTwo = 0;

  private static long minX = 0;
  private static long minY = 0;
  private static long maxX = long.MaxValue;
  private static long maxY = long.MaxValue;

  private static PriorityQueue<((long x, long y) a, (long x, long y) b), long> maxAreas = new(Comparer<long>.Create((a, b) => b.CompareTo(a)));
  private static long PartOne(string data)
  {
    var (tiles, _, _) = GetTiles(data);
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
    var (tiles, min, max) = GetTiles(data);
    (minX, minY) = min;
    (maxX, maxY) = max;
    Console.WriteLine($"Create edges");
    var edges = CreateEdges(tiles);

    Console.WriteLine($"Loop areas");
    long maxArea;
    while (true) {
      if (!maxAreas.TryDequeue(out var item, out maxArea))
        break;
      var (a, b) = item;
      Console.WriteLine($"Trying max Area: {maxArea}, a:{a}, b:{b}");

      if (IsCompletelyContained(a, b, tiles, edges)) {
        break;
      }
      Console.WriteLine($"Not contained");
    }

    return maxArea;
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
      for (var y = a.y + 1; y < b.y; y++) {
        edges.Add((a.x, y));
      }
    } else if (a.y == b.y) {
      if (a.x > b.x) (a.x, b.x) = (b.x, a.x);
      for (var x = a.x + 1; x < b.x; x++) {

        edges.Add((x, a.y));
      }
    }
  }

  private static Dictionary<(long, long), bool> cache = [];
  private static bool IsCompletelyContained((long x, long y) a, (long x, long y) b, (long x, long y)[] tiles, HashSet<(long x, long y)> edges)
  {
    if (a.y > b.y) (a.y, b.y) = (b.y, a.y);
    if (a.x > b.x) (a.x, b.x) = (b.x, a.x);
    // Console.WriteLine($"Co-ordinated rearranged, a:{a}, b:{b}");
    var foundContainedInX = false;
    var y = a.y + 1;
    for (var x = a.x + 1; x < b.x; x++) {
      if (tiles.Contains((x, y))) {
        Console.WriteLine($"Is tile:{(x, y)}");
        foundContainedInX = false;
        continue;
      }
      if (edges.Contains((x, y))) {
        Console.WriteLine($"Is edge:{(x, y)}");
        foundContainedInX = false;
        continue;
      }
      if (foundContainedInX) {
        //Console.WriteLine($"Short cutting");
        cache[(x, y)] = true;
        continue;
      }
      // bool isContained;
      if (!cache.TryGetValue((x, y), out bool isContained)) {
        isContained = IsCellContained(x, y, tiles, edges);
        cache[(x, y)] = isContained;
      }
      if (!isContained)
        return false;
      foundContainedInX = true;
    }

    for (y = a.y + 2; y < b.y; y++) {
      var x = a.x + 1;
      if (tiles.Contains((x, y))) {
        Console.WriteLine($"Is tile:{(x, y)}");
        foundContainedInX = false;
        continue;
      }
      if (edges.Contains((x, y))) {
        Console.WriteLine($"Is edge:{(x, y)}");
        foundContainedInX = false;
        continue;
      }
      if (foundContainedInX) {
        //Console.WriteLine($"Short cutting");
        cache[(x, y)] = true;
        continue;
      }
      // bool isContained;
      if (!cache.TryGetValue((x, y), out bool isContained)) {
        isContained = IsCellContained(x, y, tiles, edges);
        cache[(x, y)] = isContained;
      }
      if (!isContained)
        return false;
      foundContainedInX = true;
    }

    for (y = a.y + 2; y < b.y; y++) {
      var x = b.x - 1;
      if (tiles.Contains((x, y))) {
        Console.WriteLine($"Is tile:{(x, y)}");
        foundContainedInX = false;
        continue;
      }
      if (edges.Contains((x, y))) {
        Console.WriteLine($"Is edge:{(x, y)}");
        foundContainedInX = false;
        continue;
      }
      if (foundContainedInX) {
        //Console.WriteLine($"Short cutting");
        cache[(x, y)] = true;
        continue;
      }
      // bool isContained;
      if (!cache.TryGetValue((x, y), out bool isContained)) {
        isContained = IsCellContained(x, y, tiles, edges);
        cache[(x, y)] = isContained;
      }
      if (!isContained)
        return false;
      foundContainedInX = true;
    }

    y = b.y - 1;
    for (var x = a.x + 1; x < b.x; x++) {
      if (tiles.Contains((x, y))) {
        Console.WriteLine($"Is tile:{(x, y)}");
        foundContainedInX = false;
        continue;
      }
      if (edges.Contains((x, y))) {
        Console.WriteLine($"Is edge:{(x, y)}");
        foundContainedInX = false;
        continue;
      }
      if (foundContainedInX) {
        //Console.WriteLine($"Short cutting");
        cache[(x, y)] = true;
        continue;
      }
      // bool isContained;
      if (!cache.TryGetValue((x, y), out bool isContained)) {
        isContained = IsCellContained(x, y, tiles, edges);
        cache[(x, y)] = isContained;
      }
      if (!isContained)
        return false;
      foundContainedInX = true;
    }

    return true;
  }

  // private static (int dx, int dy)[] _directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];
  private static bool IsCellContained(long x, long y, (long x, long y)[] tiles, HashSet<(long x, long y)> edges)
  {
    Console.WriteLine($"See if {(x, y)} has edge access");
    var directions = new PriorityQueue<(int dx, int dy), long>();
    directions.Enqueue((0, -1), y - minY);
    directions.Enqueue((-1, 0), x - minX);
    directions.Enqueue((0, 1), maxY - y);
    directions.Enqueue((1, 0), maxX - x);

    while (directions.Count > 0) {
      var (dx, dy) = directions.Dequeue();
      // Console.WriteLine($"Direction: {(dx, dy)}");
      var (nx, ny) = (x + dx, y + dy);
      // Console.WriteLine($"Next location: {(nx, ny)}");
      var intersections = 0;
      var encounteredTile = false;
      while (IsInBounds(nx, ny)) {
        if (tiles.Contains((nx, ny))) {
          // Console.WriteLine($"Encountered tile at: {(nx, ny)}");
          encounteredTile = true;
          break;
        }
        if (edges.Contains((nx, ny))) {
          // Console.WriteLine($"Encountered edge at: {(nx, ny)}");
          intersections++;
        }
        nx += dx;
        ny += dy;
        // Console.WriteLine($"Next location: {(nx, ny)}");
      }
      if (!encounteredTile) {
        // Console.WriteLine($"Intersections for {(x, y)}: {intersections}");
        return intersections % 2 == 1;
      }
    }
    return true;
  }

  private static bool IsInBounds(long x, long y)
  {
    return x >= minX && x <= maxX && y >= minY && y <= maxY;
  }

  private static ((long x, long y)[] tiles, (long x, long y) min, (long x, long y) max) GetTiles(string data)
  {
    var tiles = new List<(long, long)>();
    (long x, long y) min = (long.MaxValue, long.MaxValue);
    (long x, long y) max = (0, 0);

    foreach (var line in data.Split('\n', StringSplitOptions.RemoveEmptyEntries)) {
      var (x, y) = line.Split(',').ToLongTuplePair();
      min.x = Math.Min(min.x, x);
      min.y = Math.Min(min.y, y);
      max.x = Math.Max(max.x, x);
      max.y = Math.Max(max.y, y);
      tiles.Add((x, y));
    }
    return (tiles.ToArray(), min, max);
  }
}
