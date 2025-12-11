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
    var corners = GetCorners(data);
    for (var i = 0; i < corners.Length; i++) {
      var (ax, ay) = corners[i];
      for (var j = i + 1; j < corners.Length; j++) {
        if (i == j)
          continue;
        var (bx, by) = corners[j];
        long area = (Math.Abs(ax - bx) + 1) * (Math.Abs(ay - by) + 1);
        maxAreas.Enqueue(((ax, ay), (bx, by)), area);
      }
    }
    maxAreas.TryPeek(out _, out var maxArea);

    return maxArea;
  }

  private static long PartTwo(string data)
  {
    var corners = GetCorners(data);
    var (horizontalEdges, verticalEdges) = CreateEdges(corners);
    long maxArea;
    while (true) {
      if (!maxAreas.TryDequeue(out var item, out maxArea))
        break;
      var (a, b) = item;

      if (!RectIntersectsEdges(a, b, horizontalEdges, verticalEdges)) {
        break;
      }
    }
    return maxArea;
  }

  private static bool RectIntersectsEdges((long x, long y) a, (long x, long y) b, List<Edge> horizontalEdges, List<Edge> verticalEdges)
  {
    // Normalize rectangle by making corners (top left) and (bottom right)
    if (a.y > b.y) (a.y, b.y) = (b.y, a.y);
    if (a.x > b.x) (a.x, b.x) = (b.x, a.x);

    // shrink rectangle by 1 to take inside edge of rectangle
    a = (a.x + 1, a.y + 1);
    b = (b.x - 1, b.y - 1);

    // if top horizontal line intersects vertical edges return true
    if (HasVerticalIntersect((a.x, a.y), (b.x, a.y), verticalEdges)) return true;
    // if bottom horizontal line intersects vertical edges return true
    if (HasVerticalIntersect((a.x, b.y), (b.x, b.y), verticalEdges)) return true;
    // if left vertical line intersects horizontal edges return true
    if (HasHorizontalIntersect((a.x, a.y), (a.x, b.y), horizontalEdges)) return true;
    // if rigtt vertical line intersects horizontal edges return true
    if (HasHorizontalIntersect((b.x, a.y), (b.x, b.y), horizontalEdges)) return true;

    return false;
  }

  private static bool HasVerticalIntersect((long x, long y) a, (long x, long y) b, List<Edge> edges)
  {
    foreach (var edge in edges) {
      if (a.x < edge.Start.x && b.x > edge.Start.x && a.y > edge.Start.y && a.y < edge.End.y) {
        return true;
      }
    }
    return false;
  }

  private static bool HasHorizontalIntersect((long x, long y) a, (long x, long y) b, List<Edge> edges)
  {
    foreach (var edge in edges) {
      if (a.x > edge.Start.x && a.x < edge.End.x && a.y < edge.Start.y && b.y > edge.Start.y) {
        return true;
      }
    }
    return false;
  }

  private class Edge((long, long) start, (long, long) end)
  {
    public (long x, long y) Start { get; set; } = start;
    public (long x, long y) End { get; set; } = end;

    public override string ToString()
    {
      return $"{Start}->{End}";
    }
  }

  private static (List<Edge> horizontal, List<Edge> vertical) CreateEdges((long x, long y)[] corners)
  {
    List<Edge> verticalEdges = [];
    List<Edge> horizontalEdges = [];
    for (var i = 0; i < corners.Length; i++) {
      var (ax, ay) = corners[i];
      var (bx, by) = corners[(i + 1) % corners.Length];
      // vertical or horizontal
      if (ax == bx) // vertical
      {
        verticalEdges.Add(new Edge((ax, Math.Min(ay, by)), (bx, Math.Max(ay, by))));
      } else {
        horizontalEdges.Add(new Edge((Math.Min(ax, bx), ay), (Math.Max(ax, bx), by)));
      }
    }
    return (horizontalEdges, verticalEdges);
  }

  private static (long x, long y)[] GetCorners(string data)
  {
    var corners = new List<(long, long)>();

    foreach (var line in data.Split('\n', StringSplitOptions.RemoveEmptyEntries)) {
      var (x, y) = line.Split(',').ToLongTuplePair();
      corners.Add((x, y));
    }
    return corners.ToArray();
  }
}
