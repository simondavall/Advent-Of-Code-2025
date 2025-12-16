using AocHelper;

namespace Day10;

internal static partial class Program
{
  private const string Title = "\n## Day 10: Factory ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/10";
  private const long ExpectedPartOne = 457;
  private const long ExpectedPartTwo = 17576;

  private static long PartOne(string data)
  {
    var schematics = ProcessData(data);
    long tally = 0;

    HashSet<long> seen;

    foreach (var (required, buttonSchematics, _) in schematics) {
      seen = [];
      var startingLights = IndicatorLights.CreateInitial(required.Length);
      var q = new PriorityQueue<IndicatorLights, long>();
      q.Enqueue(startingLights, 0);

      var foundMatch = false;
      while (q.Count > 0) {
        if (!q.TryDequeue(out var current, out long depth))
          continue;
        if (seen.Contains(current.CacheKey))
          continue;
        seen.Add(current.CacheKey);
        foreach (var buttons in buttonSchematics) {
          var newLights = current.Clone();
          foreach (var button in buttons) {
            newLights.SwitchLight(button);
          }
          if (newLights.CacheKey == required.CacheKey) {
            foundMatch = true;
            break;
          }
          q.Enqueue(newLights, depth + 1);
        }
        if (foundMatch) {
          tally += depth + 1;
          break;
        }
      }
    }

    return tally;
  }

  private static long PartTwo(string data)
  {
    var schematics = ProcessData(data);
    long tally = 0;

    for (var s = 0; s < schematics.Length; s++) {
      double[][] buttons = schematics[s].buttons.ToDoubleArray();
      double[] joltage = schematics[s].joltage.ToDoubleArray();
      int buttonCount = buttons.Length;
      int joltageCount = joltage.Length;

      double[][] orig_matrix = new double[joltageCount][];
      for (int i = 0; i < joltageCount; i++) {
        orig_matrix[i] = new double[buttonCount + 1];
        orig_matrix[i][^1] = joltage[i];
        for (int j = 0; j < buttons.Length; j++)
          orig_matrix[i][j] = buttons[j].Contains(i) ? 1 : 0;
      }

      var matrix = ReducedRowEchelonForm(orig_matrix, s);
      var m = matrix.Length;
      var n = matrix[0].Length - 1;

      int tallyForMatrix = 0;
      if (n > m) {
        var max = 0;
        Array.ForEach(joltage, x => max = (int)Math.Max(max, x));
        var freevariables = n - m;
        if (freevariables > 3)
          throw new ApplicationException($"Expected max free variables to be 3. Found:{freevariables}, Matrix Id:{s}");
        var max_b = freevariables > 1 ? max : 0;
        var max_c = freevariables > 2 ? max : 0;

        var possibles = new int[freevariables];
        var minSolution = int.MaxValue;
        for (var a = 0; a <= max; a++) {
          possibles[0] = a;
          for (var b = 0; b <= max_b; b++) {
            if (freevariables > 1)
              possibles[1] = b;
            for (var c = 0; c <= max_c; c++) {
              if (freevariables > 2)
                possibles[2] = c;
              var solution = 0;
              var solutionFound = true;
              var presses = a + b + c;

              for (var i = 0; i < m; i++) {
                var fv_sum = 0;
                for (var j = 0; j < freevariables; j++) {
                  fv_sum += matrix[i][^(j + 2)] * possibles[j];
                }

                var num = matrix[i][^1] - fv_sum;
                if (num < 0 || num % matrix[i][i] != 0) {
                  // failed, move to next attempt.
                  solutionFound = false;
                  break;
                }

                solution += num / matrix[i][i];
              }
              if (solutionFound) {
                minSolution = Math.Min(minSolution, solution + presses);
              }
            }
          }
        }
        tallyForMatrix = minSolution;

      } else {
        for (var i = 0; i < matrix.Length; i++)
          tallyForMatrix += matrix[i][^1] / matrix[i][i];
      }
      tally += tallyForMatrix;
    }

    return tally;
  }

  private static int[][] ReducedRowEchelonForm(double[][] orig_matrix, int idx)
  {
    var matrix = new double[orig_matrix.Length][];
    foreach (var (i, row) in orig_matrix.Index())
      matrix[i] = row.ToArray();
    int pivot = 0, rowCount = matrix.Length, columnCount = matrix[0].Length;
    for (int r = 0; r < rowCount; r++) {
      if (columnCount <= pivot) break;
      int i = r;
      while (matrix[i][pivot] == 0) {
        i++;
        if (i == rowCount) {
          i = r;
          pivot++;
          if (columnCount == pivot) {
            pivot--;
            break;
          }
        }
      }
      for (int j = 0; j < columnCount; j++) {
        (matrix[i][j], matrix[r][j]) = (matrix[r][j], matrix[i][j]);
      }
      double div = matrix[r][pivot];
      if (div != 0) {
        for (int j = 0; j < columnCount; j++) {
          matrix[r][j] /= div;
        }
        for (int j = 0; j < rowCount; j++) {
          if (j != r) {
            double sub = matrix[j][pivot];
            for (int k = 0; k < columnCount; k++) {
              matrix[j][k] -= sub * matrix[r][k];
              if (Math.Abs(matrix[j][k] % 1) < 0.0001)
                matrix[j][k] = Math.Truncate(matrix[j][k]);
            }
          }
        }
      }
      pivot++;
    }

    // normalize values to integer format
    for (var i = 0; i < matrix.Length; i++) {
      for (var j = 0; j < matrix[0].Length; j++) {
        if (matrix[i][j] % 1 != 0) {
          double mod = Math.Abs(matrix[i][j] % 1);
          if (mod > 0.5) {
            mod /= 2;
          }
          if (mod < 0.00001) {
            matrix[i][j] = Math.Truncate(matrix[i][j]);
            continue;
          }

          for (var k = 0; k < matrix[0].Length; k++) {
            matrix[i][k] /= mod;

            // round values up or down if they are very near integers.
            var rem = matrix[i][k] % 1;
            if (rem > 0.999)
              matrix[i][k] = Math.Ceiling(matrix[i][k]);
            else if (rem < -0.999)
              matrix[i][k] = Math.Floor(matrix[i][k]);
            else if (rem > 0 && rem < 0.00001)
              matrix[i][k] = Math.Floor(matrix[i][k]);
            else if (rem < 0 && rem > -0.00001)
              matrix[i][k] = Math.Ceiling(matrix[i][k]);
            else if (rem != 0 & k < j)
              j = k - 1;
          }
        }
      }
    }

    // remove all zero rows
    var newMatrix = new List<int[]>();
    for (var i = 0; i < matrix.Length; i++) {
      var hasNumbers = false;
      var newRow = new int[matrix[0].Length];
      for (var j = 0; j < matrix[0].Length; j++) {
        if (matrix[i][j] != 0) {
          newRow[j] = (int)matrix[i][j];
          hasNumbers = true;
        } else {
          newRow[j] = 0;
        }
      }
      if (hasNumbers) {
        newMatrix.Add(newRow);
      }
    }

    // push free variable column to the right
    var txMatrix = TransposeMatrix(newMatrix.ToArray());
    for (var i = 0; i < txMatrix[0].Length; i++) {
      var push = 0;
      while (txMatrix[i + push][i] == 0) {
        push++;
      }
      if (push > 0) {
        (txMatrix[i + push], txMatrix[i]) = (txMatrix[i], txMatrix[i + push]);
      }
    }
    txMatrix = TransposeMatrix(txMatrix);

    return txMatrix.ToArray();
  }

  private static int[][] TransposeMatrix(int[][] arr)
  {
    var newArray = new int[arr[0].Length][];
    for (var i = 0; i < arr[0].Length; i++) {
      newArray[i] = new int[arr.Length];
    }
    for (var i = 0; i < arr.Length; i++) {
      for (var j = 0; j < arr[0].Length; j++) {
        newArray[j][i] = arr[i][j];
      }
    }
    return newArray;
  }

  private static double[][] ToDoubleArray(this int[][] intArray)
  {
    var array = new List<double[]>();
    foreach (var intArr in intArray)
      array.Add(intArr.ToDoubleArray());

    return array.ToArray();
  }

  private static double[] ToDoubleArray(this int[] intArray)
  {
    var array = new double[intArray.Length];
    foreach (var (i, val) in intArray.Index())
      array[i] = val;
    return array;
  }

  private static (IndicatorLights indicatorLights, int[][] buttons, int[] joltage)[] ProcessData(string data)
  {
    var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

    var output = new List<(IndicatorLights, int[][], int[])>();
    foreach (var line in lines) {
      var schematic = line.Split(' ');

      var strIndicator = schematic[0];
      var indicatorLights = new IndicatorLights(strIndicator[1..^1].ToCharArray());

      var strJoltage = schematic[^1];
      var joltage = strJoltage[1..^1].Split(',').ToIntArray();

      var buttonSchematics = schematic[1..^1];
      var buttonsList = new List<int[]>();
      foreach (var buttonSchematic in buttonSchematics) {
        var buttons = buttonSchematic[1..^1].Split(',').ToIntArray();
        buttonsList.Add(buttons);
      }
      output.Add((indicatorLights, buttonsList.ToArray(), joltage));
    }
    return output.ToArray();
  }

  private class IndicatorLights
  {
    private char[] _lights = [];
    private long _cacheKey;

    private IndicatorLights() { }
    public IndicatorLights(char[] lights)
    {
      _lights = lights;
      _cacheKey = CreateCacheKey();
    }

    public void SwitchLight(int n)
    {
      _lights[n] = _lights[n] == '#' ? '.' : '#';
      UpdateCacheKey(n);
    }

    public char[] Lights => _lights;
    public long CacheKey => _cacheKey;

    public IndicatorLights Clone()
    {
      return new IndicatorLights() {
        _lights = _lights.ToArray(),
        _cacheKey = _cacheKey
      };
    }

    public int Length => _lights.Length;

    public static IndicatorLights CreateInitial(int length)
    {
      var lights = Helper.CreateArray(length, '.');
      return new IndicatorLights(lights);
    }

    private long CreateCacheKey()
    {
      long key = 0;
      for (var i = 0; i < _lights.Length; i++) {
        key *= 10;
        key += _lights[i] == '.' ? 0 : 1;
      }
      return key;
    }

    private void UpdateCacheKey(int idx)
    {
      var n = (long)Math.Pow(10, _lights.Length - idx - 1);
      if (_lights[idx] == '#')
        _cacheKey += n;
      else
        _cacheKey -= n;
    }
  }
}
