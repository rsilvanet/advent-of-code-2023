using System.Numerics;

internal static class Day20
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day20/Input.txt");
        var modules = lines.Select(x => new Module(x)).ToDictionary(m => m.Name, m => m);

        foreach (var item in modules)
        {
            item.Value.IdentifyInputs(modules.Values);
        }

        var lowPulseCount = 0L;
        var highPulseCount = 0L;
        var pulses = new Queue<(string source, string destination, PulseType type)>();
        var rxInputs = modules.Where(x => x.Value.Directions.Contains("rx")).SelectMany(x => x.Value.RecentByInput.Keys);
        var rxInputCycles = rxInputs.ToDictionary(x => x, x => 0L);
        var counter = 1;

        while (rxInputCycles.Any(x => x.Value == 0))
        {
            pulses.Enqueue(("button", "broadcaster", PulseType.Low));

            while (pulses.Count > 0)
            {
                var pulse = pulses.Dequeue();

                if (counter <= 1000)
                {
                    if (pulse.type == PulseType.Low)
                    {
                        lowPulseCount++;
                    }
                    else
                    {
                        highPulseCount++;
                    }
                }

                if (rxInputCycles.TryGetValue(pulse.destination, out long cycle) && cycle == 0 && pulse.type == PulseType.Low)
                {
                    rxInputCycles[pulse.destination] = counter;
                }

                if (!modules.TryGetValue(pulse.destination, out var module))
                {
                    continue;
                }

                if (!module.ReceivePulse(pulse.source, pulse.type))
                {
                    continue;
                }

                foreach (var newPulse in module.GetPulsesToSend())
                {
                    pulses.Enqueue((module.Name, newPulse.destination, newPulse.type));
                }
            }

            counter++;
        }

        Console.WriteLine($"Day 20, Part 1: {lowPulseCount * highPulseCount}");
        Console.WriteLine($"Day 20, Part 2: {FindLeastCommonMultiple(rxInputCycles.Values.ToArray())}");

        long FindLeastCommonMultiple(long[] numbers) => numbers.Aggregate(FindLeastCommonMultiple2);

        long FindLeastCommonMultiple2(long a, long b) => Math.Abs(a * b) / (long)BigInteger.GreatestCommonDivisor(a, b);
    }

    class Module
    {
        public Module(string line)
        {
            var lineSplit = line.Split(" -> ", StringSplitOptions.TrimEntries);

            if (lineSplit[0] == "broadcaster")
            {
                Name = "broadcaster";
                Type = ModuleType.Broadcaster;
            }
            else if (lineSplit[0][0] == '%')
            {
                Type = ModuleType.FlipFlop;
                Name = lineSplit[0].Substring(1);
            }
            else if (lineSplit[0][0] == '&')
            {
                Type = ModuleType.Conjunction;
                Name = lineSplit[0].Substring(1);
            }
            else
            {
                throw new NotImplementedException(line);
            }

            Directions = lineSplit[1].Split(',', StringSplitOptions.TrimEntries);
            On = false;
            RecentByInput = new Dictionary<string, PulseType>();
        }

        public string Name { get; }
        public ModuleType Type { get; }
        public string[] Directions { get; }
        public bool On { get; private set; }
        public Dictionary<string, PulseType> RecentByInput { get; private set; }
        public int Counter { get; set; }

        public void IdentifyInputs(IEnumerable<Module> modules)
        {
            if (Type == ModuleType.Conjunction)
            {
                foreach (var item in modules.Where(x => x.Directions.Contains(Name)))
                {
                    RecentByInput[item.Name] = PulseType.Low;
                }
            }
        }

        public bool ReceivePulse(string source, PulseType pulseType)
        {
            if (Type == ModuleType.FlipFlop)
            {
                if (pulseType == PulseType.Low)
                {
                    On = !On;
                    return true;
                }

                return false;
            }
            else if (Type == ModuleType.Conjunction)
            {
                RecentByInput[source] = pulseType;
            }
            else if (Type != ModuleType.Broadcaster)
            {
                throw new NotImplementedException($"{pulseType} from {source}");
            }

            return true;
        }

        public IEnumerable<(string destination, PulseType type)> GetPulsesToSend()
        {
            var pulseType = PulseType.Low;

            if (Type == ModuleType.Broadcaster)
            {
                pulseType = PulseType.Low;
            }
            else if (Type == ModuleType.FlipFlop)
            {
                pulseType = On ? PulseType.High : PulseType.Low;
            }
            else if (Type == ModuleType.Conjunction)
            {
                pulseType = RecentByInput.All(x => x.Value == PulseType.High) ? PulseType.Low : PulseType.High;
            }
            else
            {
                throw new NotImplementedException(Type.ToString());
            }

            foreach (var direction in Directions)
            {
                yield return (direction, pulseType);
            }
        }
    }

    enum ModuleType
    {
        Broadcaster,
        FlipFlop,
        Conjunction
    }

    enum PulseType
    {
        Low,
        High
    }
}