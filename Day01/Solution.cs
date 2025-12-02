using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Day01;

internal static partial class Program {
  private const string Title = "\n## Day 1: Secret Entrance ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/1";
  private const long ExpectedPartOne = 997;
  private const long ExpectedPartTwo = 5978;

  private static long PartOne(string[] rotations) {
    long tally = 0;

    var current = 50;
    foreach (var str in rotations) {
      var direction = str[0];
      var distance = int.Parse(str[1..]);
      if (direction == 'L') {
        current = (100 + current - distance) % 100;
      } else {
        current = (current + distance) % 100;
      }
      tally += current == 0 ? 1 : 0;
    }

    return tally;
  }

  private static int Rotations(int distance) => (int)Math.Floor((double)distance / 100);

  private static long PartTwo(string[] rotations) {
    long tally = 0;

    var current = 50;
    foreach (var str in rotations) {
      var direction = str[0];
      var distance = int.Parse(str[1..]);
      var actualDist = distance % 100;

      var fullRotations = Rotations(distance);
      tally += fullRotations;

      if (direction == 'L') {
        if (current != 0 && current <= actualDist)
          tally++;
        current = (100 + current - actualDist) % 100;
      } else {
        if (current != 0 && current + actualDist >= 100)
          tally++;
        current = (current + actualDist) % 100;
      }
    }

    return tally;
  }
}
