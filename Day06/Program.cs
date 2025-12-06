using System.Diagnostics;

namespace Day06;

internal static partial class Program {
 public static int Main(string[] args) {
    Console.WriteLine(Title);
    Console.WriteLine(AdventOfCode);

    long resultPartOne = -1;
    long resultPartTwo = -1;

    foreach (var filePath in args) {
      Console.WriteLine($"\nFile: {filePath}\n");
      string input = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(input);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(input);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static string GetData(string filePath) {
    using var streamReader = new StreamReader(filePath);
    return streamReader.ReadToEnd();
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw) {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
