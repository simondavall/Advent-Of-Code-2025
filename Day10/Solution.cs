using System.ComponentModel.DataAnnotations;
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

    for (var s = 0; s < schematics.Length; s++) {
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

      var h = 0;
      var k = 0;
      var m = matrix.Length;
      var n = matrix[0].Length;

      while (h < m && k < n) {
        var i_max = 0;
        // Helper.VarDump(matrix);
        for (var row = h; row < m; row++) {
          i_max = Math.Abs(matrix[row][k]) > Math.Abs(matrix[i_max][k]) ? row : i_max;
        }
        Console.WriteLine($"h(row):{h}, k(col):{k}, imax: {i_max}");
        if (i_max == 0) {
          Console.WriteLine($"Imax zero so skipping to next column");
          k++;
        } else {
          // Console.WriteLine($"Swapping rows {h} and {i_max}");
          (matrix[h], matrix[i_max]) = (matrix[i_max], matrix[h]);
          // Helper.VarDump(matrix);
          for (var i = 0; i < m; i++) {
            if (i == h)
              continue;
            Console.WriteLine($"Working on row(i):{i}");
            // todo: check if this loses precision (i.e. not an exact integer division)
            // and divide by zero
            var nom = matrix[i][k];
            var denom = matrix[h][k];
            Console.WriteLine($"Calc nom/denom, matrix[i][k]:{nom} / matrix[h][k]:{denom}");
            if (nom % denom != 0){
              Console.WriteLine($"Fraction");
            }
            var f = nom / denom;
            // Console.WriteLine($"Setting matrix[i][k]:{matrix[i][k]} to 0, i:{i}, k:{k}");
            matrix[i][k] = 0;
            for (var j = k + 1; j < n; j++) {
              // Console.WriteLine($"For remaining columns matrix[i][j]:{matrix[i][j]} - matrix[h][j]:{matrix[h][j]} * f:{f}");
              matrix[i][j] = matrix[i][j] - matrix[h][j] * f;
            }
            // Console.WriteLine($"h:{h}");
            // Helper.VarDump(matrix);
          }
          h++;
          k++;
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
