using System.Runtime.InteropServices;
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

    var lower = 0;
    var upper = -1;

    var solved = 0;
    var matrices = schematics.Length;

    for (var s = 0; s < matrices; s++) {
      if (s < lower || (s > upper && upper != -1))
        continue;

      if (!new float[]{18,31, 80,109,163}.Contains(s)){
        continue;
      }

      Console.WriteLine($"\nMatrix index:{s}");
      float[][] buttons = schematics[s].buttons.ToFloatArray();
      float[] joltage = schematics[s].joltage.ToFloatArray();
      int buttonCount = buttons.Length;
      int joltageCount = joltage.Length;

      float[][] orig_matrix = new float[joltageCount][];
      for (int i = 0; i < joltageCount; i++) {
        orig_matrix[i] = new float[buttonCount + 1];
        orig_matrix[i][^1] = joltage[i];
        for (int j = 0; j < buttons.Length; j++)
          orig_matrix[i][j] = buttons[j].Contains(i) ? 1 : 0;
      }
      // Console.WriteLine($"Original matrix");
      // Helper.VarDump(matrix);

      var matrix = ReducedRowEchelonForm(orig_matrix, s);
      // Helper.VarDump(matrix1);

      var m = matrix.Length;
      var n = matrix[0].Length - 1;

      int tallyForMatrix = 0;
      if (n > m) {
        Console.WriteLine($"\nMatrix index:{s}");
        Helper.VarDump(joltage);
        // we need to deal with free variables
        var max = 0;
        Array.ForEach(joltage, x => max = (int)Math.Max(max, x));
        Console.WriteLine($"Max joltage value: {max}");
        Console.WriteLine($"Free = {n - m}");
        Helper.VarDump(matrix);
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
              //Helper.VarDump(possibles);
              for (var i = 0; i < m; i++) {
                var fv_sum = 0;
                for(var j = 0; j < freevariables; j++){
                  fv_sum += matrix[i][^(j+2)] * possibles[j];
                }

                //Console.WriteLine($"fv_sum:{fv_sum}, num:{matrix[i][^1] - fv_sum}, matrix[i][i]:{matrix[i][i]}");
                var num = matrix[i][^1] - fv_sum;
                if (num < 0 || num % matrix[i][i] != 0) {

                  // failed, move to next attempt.
                  solutionFound = false;
                  break;
                }

                solution += num / matrix[i][i];
                //Console.WriteLine($"num:{num}, presses:{presses}, solution so far:{solution}");
              }
              if (solutionFound){
                //Console.WriteLine($"Found a solution: {solution} with possibles:");
                //Helper.VarDump(possibles);

                minSolution = Math.Min(minSolution, solution + presses);           
              }
            }
          }
        }
        solved++;
        tallyForMatrix = minSolution;

      } else {
        // Console.WriteLine($"\nMatrix index:{s}");
        // Helper.VarDump(matrix);
        for (var i = 0; i < matrix.Length; i++) {
          tallyForMatrix += matrix[i][^1] / matrix[i][i];
        }
        solved++;
      }
      Console.WriteLine($"Changed matrices:{changed.ToArray().Print()}");
      tally += tallyForMatrix;
      Console.WriteLine($"Total for matrix:{tallyForMatrix}");
    }

    Console.WriteLine($"Solved: {solved} / {matrices}");
    return tally;
  }

  private static HashSet<int> changed = new HashSet<int>();
  private static int[][] ReducedRowEchelonForm(float[][] orig_matrix, int idx)
  {
    var matrix = new float[orig_matrix.Length][];
    foreach (var (i, row) in orig_matrix.Index())
      matrix[i] = row.ToArray();
    int lead = 0, rowCount = matrix.Length, columnCount = matrix[0].Length;
    for (int r = 0; r < rowCount; r++) {
      if (columnCount <= lead) break;
      int i = r;
      while (matrix[i][lead] == 0) {
        i++;
        if (i == rowCount) {
          i = r;
          lead++;
          if (columnCount == lead) {
            lead--;
            break;
          }
        }
      }
      for (int j = 0; j < columnCount; j++) {
        (matrix[i][j], matrix[r][j]) = (matrix[r][j], matrix[i][j]);
      }
      float div = matrix[r][lead];
      if (div != 0) {
        for (int j = 0; j < columnCount; j++) {
          matrix[r][j] /= div;
        }
        for (int j = 0; j < rowCount; j++) {
          if (j != r) {
            float sub = matrix[j][lead];
            for (int k = 0; k < columnCount; k++) {
              matrix[j][k] -= sub * matrix[r][k];
              if (Math.Abs(matrix[j][k] % 1) < 0.0001)
                matrix[j][k] = (float)Math.Truncate(matrix[j][k]);
            }
          }
        }
      }
      lead++;
    }

    //Helper.VarDump(matrix);

    // normalize floating point values
    for (var i = 0; i < matrix.Length; i++) {
      for (var j = 0; j < matrix[0].Length; j++) {
        if (matrix[i][j] % 1 != 0) {
          float mod = Math.Abs(matrix[i][j] % 1);
          if (mod > 0.5) {
            mod /= 2;
          }
          if (mod < 0.00001) {
              Console.WriteLine($"Before change 1: {matrix[i][j]}");
            matrix[i][j] = (float)Math.Truncate(matrix[i][j]);
            changed.Add(idx);
              Console.WriteLine($"After change: {matrix[i][j]}");
            continue;
          }

          for (var k = 0; k < matrix[0].Length; k++) {
            matrix[i][k] /= mod;
            var rem = matrix[i][k] % 1;
            Console.WriteLine($"Rem:{rem}");
            if (rem > 0.999){
              Console.WriteLine($"Before change 2: {matrix[i][k]}");
              changed.Add(idx);
              matrix[i][k] = (float)Math.Ceiling(matrix[i][k]);
              Console.WriteLine($"After change: {matrix[i][k]}");
            }
            else if (rem < -0.999){
              Console.WriteLine($"Before change 3: {matrix[i][k]}");
              changed.Add(idx);
              matrix[i][k] = (float)Math.Floor(matrix[i][k]);
              Console.WriteLine($"After change: {matrix[i][k]}");
            }
            else if (rem > 0 && rem < 0.00001){
              Console.WriteLine($"Before change 4: {matrix[i][k]}");
              changed.Add(idx);
              matrix[i][k] = (float)Math.Floor(matrix[i][k]);
              Console.WriteLine($"After change: {matrix[i][k]}");
            }
            else if (rem < 0 && rem > -0.00001){
              Console.WriteLine($"Before change 5: {matrix[i][k]}");
              changed.Add(idx);
              matrix[i][k] = (float)Math.Ceiling(matrix[i][k]);
              Console.WriteLine($"After change: {matrix[i][k]}");
            }
          }
        }
      }
    }

    
    var newMatrix = new List<int[]>();
    // remove all zero rows
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

    var txMatrix = Transpose(newMatrix.ToArray());
    // push free variable column to the right
    for (var i = 0; i < txMatrix[0].Length; i++) {
      var push = 0;
      while (txMatrix[i + push][i] == 0) {
        push++;
      }
      if (push > 0) {
        (txMatrix[i + push], txMatrix[i]) = (txMatrix[i], txMatrix[i + push]);
      }
    }
    txMatrix = Transpose(txMatrix);

    return txMatrix.ToArray();
  }

  private static int[][] Transpose(int[][] arr)
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

  private static void OldVersion(float[][] matrix)
  {
    var h = 0;
    var k = 0;
    var m = matrix.Length;
    var n = matrix[0].Length;

    while (h < m && k < n) {
      // Console.WriteLine($"\nMoving to row:{h} and col:{k}");
      var i_max = h;
      // Loop through rows from current row to the end looking for next pivot
      for (var i = h; i < m; i++) {
        // loop through rows to find the max value for the current column
        i_max = Math.Abs(matrix[i][k]) > Math.Abs(matrix[i_max][k]) ? i : i_max;
      }
      // Console.WriteLine($"h(row):{currentRow}, k(col):{currentCol}, imax: {i_max}");
      // if there is no max then move to the next column 
      if (matrix[i_max][k] == 0) {
        k++;
      } else {
        (matrix[h], matrix[i_max]) = (matrix[i_max], matrix[h]);

        var fractional = false;
        // for (var i = 0; i < m; i++) {
        //   if (i == h)
        //     continue;
        //   if (matrix[i][k] % matrix[h][k] != 0) {
        //     fractional = true;
        //     break;
        //   }
        // }

        for (var i = 0; i < m; i++) {
          if (i == h)
            continue;
          var f = fractional ? matrix[i][k] : matrix[i][k] / matrix[h][k];
          matrix[i][k] = 0;
          for (var j = k + 1; j < n; j++) {
            matrix[i][j] = matrix[i][j] - matrix[h][j] * f;
          }
        }
        h++;
        k++;
      }
    }
    Helper.VarDump(matrix);

  }


  private static float[][] ToFloatArray(this int[][] intArray)
  {
    var array = new List<float[]>();
    foreach (var intArr in intArray)
      array.Add(intArr.ToFloatArray());

    return array.ToArray();
  }
  private static float[] ToFloatArray(this int[] intArray)
  {
    var array = new float[intArray.Length];
    foreach (var (i, val) in intArray.Index())
      array[i] = val;
    return array;
  }
  private static long PartTwo_Old(string data)
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

          // check the denominator for non-exact fraction
          var row_h = matrix[currentRow];
          var denom = row_h[currentCol];
          // Console.WriteLine($"Denom:{denom}");
          var fractional = false;
          for (var i = 0; i < rowCount; i++) {
            if (i == currentRow || matrix[i][currentCol] == 0)
              continue;
            var nom = matrix[i][currentCol];
            if (nom % denom != 0) {
              Console.WriteLine($"Fraction: {nom} / {denom}");
              fractional = true;
            }
          }

          // loop through the rows following currentRow (trying from 0)
          for (var i = 0; i < rowCount; i++) {
            if (i == currentRow || matrix[i][currentCol] == 0)
              continue;
            var row_i = matrix[i];
            // Console.WriteLine($"Working on row(i):{i}, currentCol:{currentCol} nom:{row_i[currentCol]}");
            var nom = fractional ? denom * row_i[currentCol] : row_i[currentCol];
            // Console.WriteLine($"Nom:{nom}");
            // Console.WriteLine($"Set matrix[{i}][{currentCol}] from {matrix[i][currentCol]} to 0");
            // set the i row value to zero
            row_i[currentCol] = 0;

            // todo: check if this loses precision (i.e. not an exact integer division)
            // and divide by zero
            if (nom % denom != 0) {
              Console.WriteLine($"Fraction: {nom} / {denom}");

            }
            // set the multiplier 'f' to the i row values / the currentRow value (this can be set out side the loop)
            var f = nom / denom;
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
