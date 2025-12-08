using AocHelper;

namespace Day08;

internal static partial class Program
{
  private const string Title = "\n## Day 8: Playground ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/8";
  private const long ExpectedPartOne = 66640;
  private const long ExpectedPartTwo = 78894156;

  private static long PartOne(string data, int pairsCount)
  {
    var boxes = ProcessData(data);
    var boxProximities = new PriorityQueue<(JunctionBox, JunctionBox), long>();
    for (var i = 0; i < boxes.Length - 1; i++) {
      for (var j = i + 1; j < boxes.Length; j++) {
        boxProximities.Enqueue((boxes[i], boxes[j]), boxes[i].SquaredDistanceTo(boxes[j]));
      }
    }
    var circuits = new List<HashSet<JunctionBox>>();
    var counter = 0;
    while (counter < pairsCount) {
      var (a, b) = boxProximities.Dequeue();
      var existingCircuit = new List<HashSet<JunctionBox>>();
      foreach (var circuit in circuits) {
        if (circuit.Contains(a) || circuit.Contains(b)) {
          existingCircuit.Add(circuit);
        }
      }
      switch (existingCircuit.Count) {
        case 2:
          foreach (var box in existingCircuit[1])
            existingCircuit[0].Add(box);
          circuits.Remove(existingCircuit[1]);
          break;
        case 1:
          existingCircuit[0].Add(a);
          existingCircuit[0].Add(b);
          break;
        case 0:
          circuits.Add(new HashSet<JunctionBox>([a, b]));
          break;
        default:
          throw new ApplicationException($"Expected max 2 existing circuits. Found:{existingCircuit.Count}");
      }
      counter++;
    }

    var (first, second, third) = (0, 0, 0);
    foreach (var circuit in circuits) {
      var current = circuit.Count;
      if (current > first) {
        (first, second, third) = (current, first, second);
      } else if (current > second) {
        (second, third) = (current, second);
      } else if (current > third) {
        third = current;
      }
    }

    return first * second * third;
  }

  private static long PartTwo(string data)
  {
    var boxes = ProcessData(data);

    var circuits = new List<HashSet<JunctionBox>>();
    var boxProximities = new PriorityQueue<(JunctionBox, JunctionBox), long>();
    for (var i = 0; i < boxes.Length - 1; i++) {
      circuits.Add(new HashSet<JunctionBox>([boxes[i]]));
      for (var j = i + 1; j < boxes.Length; j++) {
        boxProximities.Enqueue((boxes[i], boxes[j]), boxes[i].SquaredDistanceTo(boxes[j]));
      }
    }
    circuits.Add(new HashSet<JunctionBox>([boxes[^1]]));

    long tally;
    while (true) {
      var (a, b) = boxProximities.Dequeue();
      var existingCircuit = new List<HashSet<JunctionBox>>();
      foreach (var circuit in circuits) {
        if (circuit.Contains(a) || circuit.Contains(b)) {
          existingCircuit.Add(circuit);
          if (existingCircuit.Count == 2)
            break;
        }
      }
      if (existingCircuit.Count == 2){
        foreach (var junctionBox in existingCircuit[1])
          existingCircuit[0].Add(junctionBox);
        circuits.Remove(existingCircuit[1]);
      }
      if (circuits.Count == 1) {
        tally = a.X * b.X;
        break;
      }
    }

    return tally;
  }

  private static JunctionBox[] ProcessData(string data)
  {
    var boxes = new List<JunctionBox>();
    var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
    foreach (var line in lines) {
      var (x, y, z) = line.Split(',', StringSplitOptions.RemoveEmptyEntries).ToIntTupleTriple();
      boxes.Add(new JunctionBox(x, y, z));
    }
    return boxes.ToArray();
  }

  private readonly struct JunctionBox(long x, long y, long z)
  {
    private readonly string strPoint = $"{x}-{y}-{z}";
    public long X { get; } = x;
    public long Y { get; } = y;
    public long Z { get; } = z;

    public long SquaredDistanceTo(JunctionBox other)
    {
      return (other.X - X) * (other.X - X)
        + (other.Y - Y) * (other.Y - Y)
         + (other.Z - Z) * (other.Z - Z);
    }
    public override string ToString()
    {
      return strPoint;
    }
  }
}
