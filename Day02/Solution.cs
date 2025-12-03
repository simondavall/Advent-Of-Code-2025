using System.Collections.Concurrent;
using AocHelper;

namespace Day02;

internal static partial class Program {
  private const string Title = "\n## Day 2: Gift Shop ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/2";
  private const long ExpectedPartOne = 12850231731;
  private const long ExpectedPartTwo = 24774350322;

  private static long PartOne(string[] ranges) {
    long tally = 0;

    foreach (var str in ranges) {
      (long low, long high) = str.Split('-').ToLongTuplePair();
      (bool isValid, long max, long divisor) = (false, -1, -1);
      for (long current = low; current <= high; current++) {
        if (current > max) { // will be the same result up to max value, then re calculate
          (isValid, max, divisor) = IsValid(current);
        }
        if (isValid) { // has even number of digits
          if (current % divisor == current / divisor) { // divisor calculated to split number in two with mod and div
            tally += current;
          }
        }
      }
    }

    return tally;
  }

  private static long PartTwo(string[] ranges) {
    ConcurrentBag<long> inValidIds = [];
    
    foreach (var str in ranges) {
      (long low, long high) = str.Split('-').ToLongTuplePair();
      var numbers = Helper.Range(low, high - low + 1);

      Parallel.ForEach(numbers, number => {
        var curStr = number.ToString();

        var n = 1;
        while (n <= curStr.Length / 2) {
          if (HasRepeatedNumbers(curStr, n)) {
            inValidIds.Add(number);
            break;
          }
          n++;
        }
      });
    }

    return inValidIds.Sum();
  }

  private static bool HasRepeatedNumbers(ReadOnlySpan<char> curStr, int chunkSize) {
    if (curStr.Length % chunkSize != 0)
      return false;

    var match = true;
    for (var i = 0; i < chunkSize; i++) {
      for (var j = i + chunkSize; j < curStr.Length; j += chunkSize) {
        if (curStr[i] != curStr[j]) {
          match = false;
          break;
        }
      }
      if (!match)
        break;
    }

    return match;
  }

  private static (bool isValid, long max, long divisor) IsValid(long n) {
    bool isValid = true;
    long max = 0;
    long len = 1;
    long multiplier = 1;
    while (n > 0) {
      n /= 10;
      max += 9 * multiplier;
      multiplier *= 10;
      isValid = !isValid;
      if (isValid) {
        len *= 10;
      }
    }
    return (isValid, max, len);
  }
}
