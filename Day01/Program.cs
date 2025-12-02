using System.Diagnostics;

namespace Day01;

internal static partial class Program {
  private const string Title = "\n## Day 1: Secret Entrance ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/1";
  private const long ExpectedPartOne = 0;
  private const long ExpectedPartTwo = 0;

  public static int Main(string[] args) {
    Console.WriteLine(Title);
    Console.WriteLine(AdventOfCode);

    long resultPartOne = -1;
    long resultPartTwo = -1;

    foreach (var filePath in args) {
      Console.WriteLine($"\nFile: {filePath}\n");
      string[] input = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(input);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(input);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static string[] GetData(string filePath) {
    using var streamReader = new StreamReader(filePath);
    var data = streamReader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);
    return data;
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw) {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
