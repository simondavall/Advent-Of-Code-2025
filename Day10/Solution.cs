using AocHelper;

namespace Day10;

internal static partial class Program
{
  private const string Title = "\n## Day 10: Factory ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/10";
  private const long ExpectedPartOne = 457;
  private const long ExpectedPartTwo = 0;

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

    HashSet<long> seen;

    foreach (var (_, buttonSchematics, required) in schematics) {
      Console.WriteLine($"Trying for required: {required.Values.Print()}");
      seen = [];
      var startingJoltage = Joltage.CreateInitial(required.Length);

      var q = new PriorityQueue<(Joltage, long), int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
      q.Enqueue((startingJoltage, 0), 0);
      seen.Add(startingJoltage.CacheKey);

      var foundMatch = false;
      while (q.Count > 0) {
        if (!q.TryDequeue(out var item, out int priority))
          continue;
        var (current, depth) = item;
        foreach (var buttons in buttonSchematics) {
          var newJoltage = current.Clone();
          foreach (var button in buttons) {
            newJoltage.Increment(button);
          }
          if (seen.Contains(newJoltage.CacheKey)){
            //Console.WriteLine($"Cache hit: {newJoltage.CacheKey}, current:{newJoltage.Values.Print()}");
            continue;
          }
          seen.Add(newJoltage.CacheKey);
          if (newJoltage.CacheKey == required.CacheKey) {
            foundMatch = true;
            break;
          }
          if (!JoltageExceeded(newJoltage.Values, required.Values, out int proximity))
            q.Enqueue((newJoltage, depth + 1), proximity);
        }
        if (foundMatch) {
          tally += depth + 1;
          break;
        }
      }
    }

    return tally;
  }

  private static bool JoltageExceeded(int[] current, int[] required, out int proximity)
  {
    proximity = 0;
    for (var i = 0; i < required.Length; i++) {
      proximity += current[i];
      if (current[i] > required[i])
        return true;
    }
    return false;
  }

  private static (IndicatorLights indicatorLights, int[][] buttons, Joltage joltage)[] ProcessData(string data)
  {
    var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

    var output = new List<(IndicatorLights, int[][], Joltage)>();
    foreach (var line in lines) {
      var schematic = line.Split(' ');

      var strIndicator = schematic[0];
      var indicatorLights = new IndicatorLights(strIndicator[1..^1].ToCharArray());

      var strJoltage = schematic[^1];
      var jolts = strJoltage[1..^1].Split(',').ToIntArray();
      var joltage = new Joltage(jolts); 

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

  private class Joltage
  {
    private int[] _joltage = [];
    private long _cacheKey;

    private Joltage() { }
    public Joltage(int[] joltage)
    {
      _joltage = joltage;
      _cacheKey = CreateCacheKey();
    }

    public void Increment(int idx){
      _joltage[idx] += 1; 
      _cacheKey += (long)Math.Pow(100, _joltage.Length - idx - 1);
    }

    public int[] Values => _joltage;
    public long CacheKey => _cacheKey;

    public Joltage Clone()
    {
      return new Joltage() {
        _joltage = _joltage.ToArray(),
        _cacheKey = _cacheKey
      };
    }

    public int Length => _joltage.Length;

    public static Joltage CreateInitial(int length)
    {
      var jolts = Helper.CreateArray(length, 0);
      return new Joltage(jolts);
    }

    private long CreateCacheKey()
    {
      long key = 0;
      for (var i = 0; i < _joltage.Length; i++) {
        key *= 100;
        key += _joltage[i];
      }
      return key;
    }
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
