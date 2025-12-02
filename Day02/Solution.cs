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
        if (current > max) {
          (isValid, max, divisor) = IsValid(current);
        }
        if (isValid) {
          if (current % divisor == current / divisor) {
            tally += current;
          }
        }
      }
    }

    return tally;
  }

  private static long PartTwo(string[] ranges) {
    long tally = 0;

    foreach (var str in ranges) {
      (long low, long high) = str.Split('-').ToLongTuplePair();
      for (long current = low; current <= high; current++) {
        var curStr = current.ToString();
        var n = 1;
        while (n <= curStr.Length / 2) {
          if (HasRepeatedStrings(curStr, n)){
            tally += current;
            break;
          }
          n++;
        }
      }
    }

    return tally;
  }

  private static bool HasRepeatedStrings(ReadOnlySpan<char> curStr, int chunkSize){
    if (curStr.Length % chunkSize != 0)
      return false;

    var match = true;
    for (var i = 0; i < chunkSize; i++){
      for (var j = i + chunkSize; j < curStr.Length; j += chunkSize){
        if (curStr[i] != curStr[j]){
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
