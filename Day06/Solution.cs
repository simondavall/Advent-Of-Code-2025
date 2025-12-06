using System.Diagnostics;
using AocHelper;

namespace Day06;

internal static partial class Program {
  private const string Title = "\n## Day 6: Trash Compactor ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/6";
  private const long ExpectedPartOne = 4719804927602;
  private const long ExpectedPartTwo = 9608327000261;

  private static long PartOne(string data) {
    var (numbers, operators) = ProcessData(data);
    long tally = 0;

    for (var i = 0; i < operators.Length; i++) {
      var currentNumbers = new long[numbers.Length];
      for (var j = 0; j < numbers.Length; j++) {
        currentNumbers[j] = numbers[j][i];
      }
      tally += PerformCalc(currentNumbers, operators[i]);
    }

    return tally;
  }

  private static long PartTwo(string data) {
    var (numbers, operators) = ProcessData2(data);
    var multiplier = 10;

    long tally = 0;
    for (var i = 0; i < operators.Length; i++) {
      List<long> currentNumbers = [];
      var currentNumber = 0;
      for (var j = 0; j < numbers[i][0].Length; j++) {
        foreach(var number in numbers[i]){
          var n = number[j] - 48;
          if (n > 0) {
            currentNumber *= multiplier;
            currentNumber += n;
          }
        }
        currentNumbers.Add(currentNumber);
        currentNumber = 0;
      }
      tally += PerformCalc(currentNumbers.ToArray(), operators[i]);
    }

    return tally;
  }

  private static long PerformCalc(long[] numbers, char op) {
    long total = 0;
    foreach (var n in numbers) {
      switch (op) {
        case '*':
          if (total == 0)
            total = 1;
          total *= n;
          break;
        case '+':
          total += n;
          break;
        default:
          throw new ApplicationException($"Unhandled operator: '{op}'");
      }
    }
    return total;
  }

  private static (long[][] numbers, char[] operators) ProcessData(string data) {
    var rows = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
    List<long[]> numbers = [];
    foreach (var row in rows[..^1]) {
      numbers.Add(row.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToLongArray());
    }
    var operators = rows[^1].ToCharArray().Where(ch => ch != ' ').ToArray();
    foreach (var number in numbers) {
      Debug.Assert(number.Length == operators.Length, $"Expected {operators.Length} numbers. Found:{number.Length}");
    }
    return (numbers.ToArray(), operators);
  }

  private static (string[][] numbers, char[] operators) ProcessData2(string data) {
    var rows = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
    int space = 0;
    List<char> operators = [];
    List<int> spaces = [];
    for (var i = 0; i < rows[^1].Length; i++) {
      char ch = rows[^1][i];
      if (ch == ' ') {
        space++;
      } else {
        operators.Add(ch);
        if (space > 0)
          spaces.Add(space);
        space = 0;
      }
    }
    spaces.Add(space + 1);

    // get numbers with fixed spaces.
    List<string[]> numbers = [];

    int idx = 0;
    foreach (var sp in spaces) {
      List<string> rowNumbers = [];
      foreach (var row in rows[..^1]) {
        var n = row[idx..(idx + sp)].Replace(' ', '0');
        rowNumbers.Add(n);
      }
      idx += sp + 1;
      numbers.Add(rowNumbers.ToArray());
    }

    return (numbers.ToArray(), operators.ToArray());
  }
}
