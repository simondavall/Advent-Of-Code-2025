using AocHelper;
using Microsoft.Z3;

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

    var min = 0;
    var max = 2;

    for (var s = 0; s < schematics.Length; s++) {
      if (s < min || s > max || max == -1)
        continue;
  
      Console.WriteLine($"\nMatrix index:{s}");
      int[][] buttons = schematics[s].buttons;
      int[] joltage = schematics[s].joltage;
      int buttonCount = buttons.Length;
      int joltageCount = joltage.Length;

      int[][] matrix = new int[joltageCount][];
      for (int l = 0; l < joltageCount; l++) {
        matrix[l] = new int[buttonCount + 1];
        matrix[l][^1] = joltage[l];
        for (int j = 0; j < buttons.Length; j++)
          matrix[l][j] = buttons[j].Contains(l) ? 1 : 0;
      }

      Helper.VarDump(matrix);

      var currentRow = 0;
      var currentCol = 0;
      var rowCount = matrix.Length;
      var colCount = matrix[0].Length;

      while (currentRow < rowCount && currentCol < colCount) {
        Console.WriteLine($"\nMoving to row:{currentRow} and col:{currentCol}");
        var i_max = currentRow;
        // Loop through rows from current row to the end looking for next pivot
        for (var row = currentRow; row < rowCount; row++) {
          // loop through rows to find the max value for the current column
          i_max = Math.Abs(matrix[row][currentCol]) > Math.Abs(matrix[i_max][currentCol]) ? row : i_max;
        }
        // Console.WriteLine($"h(row):{currentRow}, k(col):{currentCol}, imax: {i_max}");
        // if there is no max then move to the next column 
        if (matrix[i_max][currentCol] == 0) {
          // Console.WriteLine($"i_max zero for row:{currentRow}, col:{currentCol} so skipping to next column:{currentCol + 1}");
          currentCol++;
        } else {
          // Helper.VarDump(matrix);
          // Console.WriteLine($"Swapping rows {currentRow} and {i_max}");
          // Swap currentRow with i_max row
          (matrix[currentRow], matrix[i_max]) = (matrix[i_max], matrix[currentRow]);
          // Helper.VarDump(matrix[currentRow]);
          // Helper.VarDump(matrix[i_max]);
          // Helper.VarDump(matrix);
          // set the denominator for the row calculations
          var row_h = matrix[currentRow];
          var denom = row_h[currentCol];
          // Console.WriteLine($"Denom:{denom}");

          // loop through the rows following currentRow (trying from 0)
          for (var i = 0; i < rowCount; i++) {
            if (i == currentRow || matrix[i][currentCol] == 0)
              continue;
            var row_i = matrix[i];
            // Console.WriteLine($"Working on row(i):{i}, currentCol:{currentCol} nom:{row_i[currentCol]}");
            var nom = row_i[currentCol];
            // Console.WriteLine($"Nom:{nom}");
            // todo: check if this loses precision (i.e. not an exact integer division)
            // and divide by zero
            if (nom % denom != 0) {
              Console.WriteLine($"Fraction: {nom} / {denom}");
            }
            // set the multiplier 'f' to the i row values / the currentRow value (this can be set out side the loop)
            var f = nom / denom;
            // Console.WriteLine($"Set matrix[{i}][{currentCol}] from {matrix[i][currentCol]} to 0");
            // set the i row value to zero
            row_i[currentCol] = 0;
            // loop through the remaining columns in the ro
            // // loop through the remaining columns in the row
            for (var j = currentCol + 1; j < colCount; j++) {
              // Console.WriteLine($"For remaining columns matrix[i][j]:{matrix[i][j]} - matrix[h][j]:{matrix[h][j]} * f:{f}");
              // subtract the currentRow value * (i row value / currentRow value).
              // (So if the currentRow val is 2 and the iRow value is 4)
              // then the iRow value = 4 - (4 / 2) = 2
              Console.WriteLine($"Set row_i[{j}] = row_i[{j}]:{row_i[j]} - row_h[{j}]:{row_h[j]} * f:{f}");
              row_i[j] = row_i[j] - row_h[j] * f;
            }
            // Console.WriteLine($"h:{h}");
            // Helper.VarDump(matrix);
          }
          // increment diagonal (i.e. row and column at the same time, 0,0, 1,1, 2,2 ... etc.)
          currentRow++;
          currentCol++;
          // Helper.VarDump(matrix);
        }
      }

      Helper.VarDump(matrix);
      // using var ctx = new Context();
      // Optimize o = ctx.MkOptimize();
      //
      // IntExpr[] expr = new IntExpr[buttonCount];
      //
      // for (int j = 0; j < buttonCount; j++) {
      //   expr[j] = ctx.MkIntConst($"expr_{s}_{j}");
      //   o.Add(ctx.MkGe(expr[j], ctx.MkInt(0)));
      // }
      //
      // for (int j = 0; j < joltageCount; j++) {
      //   var terms = new List<ArithExpr>();
      //
      //   for (int b = 0; b < buttonCount; b++) {
      //     int[] button = buttons[b];
      //
      //     bool hasMatch = false;
      //     for (int l = 0; l < button.Length; l++) {
      //       if (button[l] == j) {
      //         hasMatch = true;
      //         break;
      //       }
      //     }
      //
      //     if (hasMatch)
      //       terms.Add(expr[b]);
      //   }
      //
      //   ArithExpr lhsExpr;
      //   if (terms.Count == 0) {
      //     lhsExpr = ctx.MkInt(0);
      //   } else if (terms.Count == 1) {
      //     lhsExpr = terms[0];
      //   } else {
      //     lhsExpr = ctx.MkAdd(terms.ToArray());
      //   }
      //
      //   o.Add(ctx.MkEq(lhsExpr, ctx.MkInt(joltage[j])));
      // }
      //
      // ArithExpr totalExpr;
      // if (buttonCount == 1)
      //   totalExpr = expr[0];
      // else
      //   totalExpr = ctx.MkAdd(expr);
      //
      // o.MkMinimize(totalExpr);
      //
      // if (o.Check() != Status.SATISFIABLE) {
      //   return -1;
      // }
      //
      // Model model = o.Model;
      //
      // int best = 0;
      // for (int j = 0; j < buttonCount; j++) {
      //   IntNum val = (IntNum)model.Evaluate(expr[j], true);
      //   best += val.Int;
      // }
      //
      // tally += best;
    }
    return tally;
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
