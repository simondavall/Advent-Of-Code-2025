namespace Day03;

internal static partial class Program {
  private const string Title = "\n## Day 3: Lobby ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/3";
  private const long ExpectedPartOne = 17092;
  private const long ExpectedPartTwo = 170147128753455;

  private static long PartOne(string[] banks) {
    long tally = 0;

    foreach (var row in banks) {
      int first = 0;
      int second = 0;
      for (var i = 0; i < row.Length - 1; i++) {
        if (row[i] > first) {
          first = row[i];
          second = row[i + 1];
        } else {
          second = Math.Max(second, row[i + 1]);
        }
      }
      first -= 48;
      second -= 48;
      tally += first * 10 + second;
    }

    return tally;
  }

  private const int DIGIT_COUNT = 12;

  private static long PartTwo(string[] banks) {
    long tally = 0;

    foreach (var row in banks) {
      int[] digits = new int[DIGIT_COUNT];
      int counter = 0;
      int discarded = 0;

      while (counter < DIGIT_COUNT) {
        var idx = counter + discarded;
        int max = row[idx];
        for (var i = idx; i <= row.Length - (DIGIT_COUNT - counter); i++) {
          if (row[i] > max){
            max = row[i];
            idx = i;
          }
        }
        digits[DIGIT_COUNT - (counter + 1)] = max - 48;
        discarded += idx - (counter + discarded);
        counter++;
      }

      long multiplier = 1;
      foreach(var digit in digits){
        tally += digit * multiplier;
        multiplier *= 10;
      }
    }

    return tally;
  }
}
