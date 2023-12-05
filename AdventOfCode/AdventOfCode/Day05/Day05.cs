internal static class Day05
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day05/Input.txt");
        var seeds = ParseSeeds(lines);
        var maps = ParseMaps(lines);

        var lowestLocationPart1 = long.MaxValue;

        foreach (var seed in seeds)
        {
            lowestLocationPart1 = Math.Min(lowestLocationPart1, FindLocation(maps, seed));
        }

        Console.WriteLine($"Day 5, Part 1: {lowestLocationPart1}");

        var seedRanges = new List<(long start, long end)>();

        for (int i = 0; i < seeds.Count(); i = i + 2)
        {
            seedRanges.Add((start: seeds[i], end: seeds[i] + seeds[i + 1]));
        }

        Console.WriteLine($"Day 5, Part 2: Starting search at {DateTime.Now}...");

        var lowestLocationPart2 = long.MaxValue;

        for (int location = 0; location < int.MaxValue; location++)
        {
            var seed = FindSeed(maps, location);

            if (seedRanges.Any(x => x.start <= seed && x.end > seed))
            {
                lowestLocationPart2 = location;
                break;
            }

            if (location % 5_000_000 == 0)
            {
                Console.WriteLine($"Day 5, Part 2: Checking locations {location / 1_000_000}M+ at {DateTime.Now}.");
            }
        }
        
        Console.WriteLine($"Day 5, Part 2: Finished search at {DateTime.Now}.");
        Console.WriteLine($"Day 5, Part 2: {lowestLocationPart2}");

        long[] ParseSeeds(IEnumerable<string> lines)
        {
            return lines.First().Replace("seeds: ", "").Trim().Split(" ").Select(long.Parse).ToArray();
        }

        Dictionary<(string source, string destination), Range[]> ParseMaps(IEnumerable<string> lines)
        {
            var maps = new Dictionary<(string source, string destination), List<Range>>();

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                else if (line.Contains("map"))
                {
                    var split = line.Replace("map:", "").Trim().Split("-to-");
                    maps.Add((source: split[0], destination: split[1]), new List<Range>() { Range.Empty });
                }
                else
                {
                    var split = line.Split(" ").Select(long.Parse).ToArray();
                    maps.Last().Value.Add(new Range(destinationStart: split[0], sourceStart: split[1], length: split[2]));
                }
            }

            return maps.ToDictionary(x => x.Key, y => y.Value.OrderBy(z => z.IsEmpty).ToArray());
        }

        long FindDestinationNumber(Dictionary<(string source, string destination), Range[]> maps, string source, string destination, long sourceNumber)
        {
            var range = maps[(source, destination)].First(x => sourceNumber >= x.SourceStart && sourceNumber < x.SourceEnd);
            return range.CalculateDestinationNumber(sourceNumber);
        }

        long FindSourceNumber(Dictionary<(string source, string destination), Range[]> maps, string source, string destination, long destinationNumber)
        {
            var range = maps[(source, destination)].First(x => destinationNumber >= x.DestinationStart && destinationNumber < x.DestinationEnd);
            return range.CalculateSourceNumber(destinationNumber);
        }

        long FindLocation(Dictionary<(string source, string destination), Range[]> maps, long seed)
        {
            var soil = FindDestinationNumber(maps, "seed", "soil", seed);
            var fertilizer = FindDestinationNumber(maps, "soil", "fertilizer", soil);
            var water = FindDestinationNumber(maps, "fertilizer", "water", fertilizer);
            var light = FindDestinationNumber(maps, "water", "light", water);
            var temperature = FindDestinationNumber(maps, "light", "temperature", light);
            var humidity = FindDestinationNumber(maps, "temperature", "humidity", temperature);
            var location = FindDestinationNumber(maps, "humidity", "location", humidity);

            return location;
        }

        long FindSeed(Dictionary<(string source, string destination), Range[]> maps, long location)
        {
            var humidity = FindSourceNumber(maps, "humidity", "location", location);
            var temperature = FindSourceNumber(maps, "temperature", "humidity", humidity);
            var light = FindSourceNumber(maps, "light", "temperature", temperature);
            var water = FindSourceNumber(maps, "water", "light", light);
            var fertilizer = FindSourceNumber(maps, "fertilizer", "water", water);
            var soil = FindSourceNumber(maps, "soil", "fertilizer", fertilizer);
            var seed = FindSourceNumber(maps, "seed", "soil", soil);

            return seed;
        }
    }

    struct Range
    {
        public Range(long destinationStart, long sourceStart, long length)
        {
            DestinationStart = destinationStart;
            SourceStart = sourceStart;
            Length = length;
            SourceEnd = SourceStart + length;
            DestinationEnd = DestinationStart + length;
            IsEmpty = length == long.MaxValue;
        }

        public long DestinationStart { get; }
        public long SourceStart { get; }
        public long Length { get; }
        public long SourceEnd { get; }
        public long DestinationEnd { get; }
        public bool IsEmpty { get; }

        public static Range Empty => new(0, 0, long.MaxValue);

        public long CalculateSourceNumber(long destinationNumber) => SourceStart + (destinationNumber - DestinationStart);
        public long CalculateDestinationNumber(long sourceNumber) => DestinationStart + (sourceNumber - SourceStart);
    }
}