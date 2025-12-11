using System.Diagnostics;
using AocHelper;

namespace Day11;

internal static partial class Program
{
  private const string Title = "\n## Day 11: Reactor ##";
  private const string AdventOfCode = "https://adventofcode.com/2025/day/11";
  private const long ExpectedPartOne = 448;
  private const long ExpectedPartTwo = 553204221431080;

  private static long PartOne(string data)
  {
    var (devices, start, _) = ProcessData(data);
    if (start is null)
      throw new ApplicationException("Could not find start");

    long tally = 0;
    var q = new Queue<Device>([start]);
    
    while(q.Count > 0){
      var current = q.Dequeue();
      if (current.Name == "out")
        tally++;
      else
        Array.ForEach(current.Outputs, x => q.Enqueue(devices.Single(d => d.Name == x)));
    }
    return tally;
  }

  private static long PartTwo(string data)
  {
    cache = []; 
    var (devices, _, server) = ProcessData(data);
    if (server is null)
      throw new ApplicationException("Could not find server");

    long tally = GetPathCount(server, (false, false), devices);

    return tally;
  }

  private static Dictionary<(string, (bool, bool)) , long> cache = [];
  private static long GetPathCount(Device current, (bool dac, bool fft) targets, Device[] devices){
    if (current.Name == "out"){
      return targets == (true, true) ? 1 : 0;
    }

    if (cache.TryGetValue((current.Name, targets), out var value)){
      return value;
    }

    if (current.Name == "dac")
      targets.dac = true;
    else if (current.Name == "fft")
      targets.fft = true;

    long tally = 0;
    foreach(var o in current.Outputs){
      tally += GetPathCount(devices.Single(d => d.Name == o), targets, devices);
    }

    cache[(current.Name, targets)] = tally;
    return tally;
  }

  private static (Device[] devices, Device? start, Device? server) ProcessData(string data)
  {
    var input = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
    var devices = new List<Device>();
    Device? start = null, server = null;
    foreach (var line in input) {
      var pairs = line.Split(": ");
      Debug.Assert(pairs.Length == 2, $"Expected 2 pairs, found {pairs.Length}");
      var device =new Device(pairs[0], pairs[1]);
      if (device.Name == "you")
        start = device;
      if (device.Name == "svr")
        server = device;
      devices.Add(device);
    }
    
    devices.Add(new Device("out", string.Empty));
    return (devices.ToArray(), start, server);
  }

  private class Device(string name, string outputDevices)
  {
    public string Name { get; set; } = name;
    public string[] Outputs { get; set; } = outputDevices.Split(' ');

    public override string ToString()
    {
      return $"{Name}-{Outputs.Print(20)}";
    }
  }
}
