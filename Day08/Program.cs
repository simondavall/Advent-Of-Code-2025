using System.Diagnostics;

namespace Day08;

internal static partial class Program
{
  public static int Main(string[] args)
  {
    if (args.Length != 2)
      throw new ArgumentException($"Expected 2 args found {args.Length}");
    Console.WriteLine(Title);
    Console.WriteLine(AdventOfCode);

    long resultPartOne = -1;
    long resultPartTwo = -1;

    for (var i = 0; i < args.Length; i += 2) {
      var filePath = args[i];
      var count = int.Parse(args[i + 1]);

      Console.WriteLine($"\nFile: {filePath}, Count:{count}\n");
      string input = GetData(filePath);
      var stopwatch = Stopwatch.StartNew();

      resultPartOne = PartOne(input, count);
      PrintResult("1", resultPartOne.ToString(), stopwatch);

      resultPartTwo = PartTwo(input);
      PrintResult("2", resultPartTwo.ToString(), stopwatch);
    }

    return resultPartOne == ExpectedPartOne && resultPartTwo == ExpectedPartTwo ? 0 : 1;
  }

  private static string GetData(string filePath)
  {
    using var streamReader = new StreamReader(filePath);
    return streamReader.ReadToEnd();
  }

  private static void PrintResult(string partNo, string result, Stopwatch sw)
  {
    sw.Stop();
    Console.WriteLine($"Part {partNo} Result: {result} in {sw.Elapsed.TotalMilliseconds}ms");
    sw.Restart();
  }
}
