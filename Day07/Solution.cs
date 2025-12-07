namespace Day07;

internal static partial class Program {
  private const string Title = "\n## Day 7: Laboratories ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/7";
  private const long ExpectedPartOne = 1490;
  private const long ExpectedPartTwo = 3806264447357;

  private static long PartOne(string data) {
    var (manifold, start) = GetManifold(data);
    var height = manifold.Length;
    var width = manifold[0].Length;

    long tally = 0;

    var beams = new HashSet<int>();
    beams.Add(start.x);

    for (var y = 0; y < height; y++) {
      var new_beams = new HashSet<int>();
      foreach (var x in beams) {
        if (manifold[y][x] == '^') {
          tally++;
          if (x + 1 < width)
            new_beams.Add(x + 1);
          if (x - 1 >= 0)
            new_beams.Add(x - 1);
        } else {
          new_beams.Add(x);
        }
      }
      beams = new_beams;
    }

    return tally;
  }

  private static long PartTwo(string data) {
    var (manifold, start) = GetManifold(data);
    return FindBeamPathCount(start.x, start.y, manifold);
  }

  private static Dictionary<(int, int), long> cache = [];

  private static long FindBeamPathCount(int x, int y, char[][] manifold){
    var height = manifold.Length;
    var width = manifold[0].Length;

    if (y == height){
      return 1;
    }
    
    if (cache.TryGetValue((x, y), out long value)){
      return value;
    }

    long beamPathCount = 0;
    if (manifold[y][x] == '^') {
      if (x + 1 < width)
        beamPathCount += FindBeamPathCount(x + 1, y + 1, manifold);
      if (x - 1 >= 0)
        beamPathCount += FindBeamPathCount(x - 1, y + 1, manifold);
    } else {
      beamPathCount += FindBeamPathCount(x, y + 1, manifold);
    }

    cache[(x,y)] = beamPathCount;
    return beamPathCount;
  }

  private static (char[][] diagnoser, (int x, int y) start) GetManifold(string data) {
    List<char[]> diagnoser = [];
    (int, int) start = (-1, -1);
    var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
    for (var y = 0; y < lines.Length; y++) {
      var x = lines[y].IndexOf('S');
      if (x > -1)
        start = (x, y);
      diagnoser.Add(lines[y].ToCharArray());
    }
    return (diagnoser.ToArray(), start);
  }
}
