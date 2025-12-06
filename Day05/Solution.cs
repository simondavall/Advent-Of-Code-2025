using System.Diagnostics;
using AocHelper;

namespace Day05;

internal static partial class Program {
  private const string Title = "\n## Day 5: Cafeteria ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/5";
  private const long ExpectedPartOne = 623;
  private const long ExpectedPartTwo = 353507173555373;

  private static long PartOne(string data) {
    var (ranges, ingredients) = ProcessData(data);
    var idx = 0;
    long tally = 0;
    foreach (var ingredient in ingredients) {

      for (var i = idx; i < ranges.Length; i++) {
        if (ingredient < ranges[i].start) {
          break;
        }
        if (ingredient <= ranges[i].end) {
          tally++;
          break;
        }
        idx = i;
      }
    }

    return tally;
  }

  private static long PartTwo(string data) {
    var (ranges, _) = ProcessData(data);
    long idx = 0;
    long tally = 0;
    foreach (var (start, end) in ranges) {
      if (idx < start)
        idx = start;
      if (idx > end){
        continue;
      }
      tally += end - idx + 1;
      idx = end + 1;
    }
    return tally;
  }

  private static ((long start, long end)[] ranges, long[] ingredients) ProcessData(string data) {
    var items = data.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    Debug.Assert(items.Length == 2, $"Expected 2 items. Found:{items.Length}");
    List<(long start, long end)> ranges = [];
    Array.ForEach(items[0]
          .Split('\n', StringSplitOptions.RemoveEmptyEntries), item => ranges
            .Add(item
              .Split('-')
              .ToLongTuplePair()));
    var ingredients = items[1].Split('\n', StringSplitOptions.RemoveEmptyEntries).ToLongArray();

    Array.Sort(ingredients);
    ranges.Sort();

    return (ranges.ToArray(), ingredients);
  }
}
