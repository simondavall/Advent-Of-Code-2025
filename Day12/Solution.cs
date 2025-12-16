using AocHelper;
namespace Day12;

internal static partial class Program
{
  private const string Title = "\n## Day 12: Christmas Tree Farm ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/12";
  private const long ExpectedPartOne = 521;
  private const long ExpectedPartTwo = 0;

  private static long PartOne(string data)
  {
    var (regions, shapes) = ProcessData(data);

    var notSolveable = 0;
    var definitelySolveable = 0;
    var unknown = 0;

    foreach (var (height, width, required) in regions) {
      var regionArea = height * width;
      var shapeTotalHashes = 0;
      var shapeTotalArea = 0;
      foreach(var (i, n) in required.Index()){
        shapeTotalArea += shapes[i].Area * n;
        shapeTotalHashes += shapes[i].HashCount * n;
      }
      if (shapeTotalHashes > regionArea)
        notSolveable++;
      else if (shapeTotalArea <= regionArea)
        definitelySolveable++;
      else
        unknown++;
    }

    return definitelySolveable;
  }

  private static long PartTwo(string data)
  {
    long tally = 0;

    return tally;


  }

  private static ((int height, int width, int[] requiredShapes)[] regions, Shape[] shapes) ProcessData(string data)
  {
    var input = data.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    var shapesData = input[0..^1];
    var shapes = new List<Shape>();
    foreach (var shapeItem in shapesData) {
      var lines = shapeItem.Split('\n');
      shapes.Add(new Shape(lines[1..]));
    }

    var regionsData = input[^1];
    var regions = new List<(int height, int width, int[] requiredShapes)>();
    foreach (var regionItem in regionsData.Split('\n', StringSplitOptions.RemoveEmptyEntries)) {
      var (region, shapesList) = regionItem.Split(": ").ToTuplePair();
      var (height, width) = region.Split('x').ToIntTuplePair();
      var requiredShapes = shapesList.Split(' ').ToIntArray();
      regions.Add((height, width, requiredShapes));
    }
    return (regions.ToArray(), shapes.ToArray());
  }

  private class Shape
  {
    public char[][] Grid { get; init; } = [];
    public int HashCount {get; init;} = 0;
    public int Area {get; init;} = 0;

    public Shape(string[] rows)
    {
      var grid = new List<char[]>();
      foreach (var row in rows) {
        var chars = row.ToCharArray();
        foreach(var ch in chars){
          HashCount += ch == '#' ? 1 :0;
          Area++;
        }
        grid.Add(chars);
      }
      Grid = grid.ToArray();
    }
  }
}
