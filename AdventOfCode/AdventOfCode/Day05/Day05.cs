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

        var lowestLocationPart2 = long.MaxValue;

        var seedRanges = new List<(long start, long end)>();

        for (int i = 0; i < seeds.Count(); i = i + 2)
        {
            seedRanges.Add((start: seeds[i], end: seeds[i] + seeds[i + 1]));
        }

        Console.WriteLine($"Day 5, Part 2: Starting search {DateTime.Now}...");

        Parallel.For(0, 100_000_000, (location, state) =>
        {
            if (location >= lowestLocationPart2)
            {
                state.Break();
            }

            var seed = FindSeed(maps, location);

            if (seedRanges.Any(x => seed >= x.start && seed < x.end))
            {
                lowestLocationPart2 = Math.Min(lowestLocationPart2, location);
                Console.WriteLine($"Day 5, Part 2: Found location {lowestLocationPart2} for seed {seed} at {DateTime.Now}.");
                state.Break();
            }
        });

        Console.WriteLine($"Day 5, Part 2: Finished search {DateTime.Now}.");
        Console.WriteLine($"Day 5, Part 2: {lowestLocationPart2}");


        long[] ParseSeeds(IEnumerable<string> lines)
        {
            return lines.First().Replace("seeds: ", "").Trim().Split(" ").Select(long.Parse).ToArray();
        }

        Dictionary<(string sourceType, string destinationType), List<Range>> ParseMaps(IEnumerable<string> lines)
        {
            var maps = new Dictionary<(string sourceType, string destinationType), List<Range>>();

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                else if (line.Contains("map"))
                {
                    var split = line.Replace("map:", "").Trim().Split("-to-");
                    maps.Add((sourceType: split[0], destinationType: split[1]), new List<Range>());
                }
                else
                {
                    var split = line.Split(" ").Select(long.Parse).ToArray();
                    maps.Last().Value.Add(new Range(destinationStart: split[0], sourceStart: split[1], length: split[2]));
                }
            }

            return maps;
        }

        long FindDestinationNumber(Dictionary<(string source, string destination), List<Range>> maps, string source, string destination, long sourceNumber)
        {
            var range = maps[(source, destination)]
                .Where(x => sourceNumber >= x.SourceStart && sourceNumber < x.SourceEnd)
                .FirstOrDefault();

            return range?.CalculateDestinationNumber(sourceNumber) ?? sourceNumber;
        }

        long FindSourceNumber(Dictionary<(string source, string destination), List<Range>> maps, string source, string destination, long destinationNumber)
        {
            var range = maps[(source, destination)]
                .Where(x => destinationNumber >= x.DestinationStart && destinationNumber < x.DestinationEnd)
                .FirstOrDefault();

            return range?.CalculateSourceNumber(destinationNumber) ?? destinationNumber;
        }

        long FindLocation(Dictionary<(string sourceType, string destinationType), List<Range>> maps, long seed)
        {
            var sourceType = "seed";
            var destinationType = maps.First(x => x.Key.sourceType == sourceType).Key.destinationType;
            var destinationNumber = FindDestinationNumber(maps, sourceType, destinationType, seed);

            while (destinationType != "location")
            {
                sourceType = destinationType;
                destinationType = maps.First(x => x.Key.sourceType == sourceType).Key.destinationType;
                destinationNumber = FindDestinationNumber(maps, sourceType, destinationType, destinationNumber);
            }

            return destinationNumber;
        }

        long FindSeed(Dictionary<(string sourceType, string destinationType), List<Range>> maps, long location)
        {
            var destinationType = "location";
            var sourceType = maps.First(x => x.Key.destinationType == destinationType).Key.sourceType;
            var sourceNumber = FindSourceNumber(maps, sourceType, destinationType, location);

            while (sourceType != "seed")
            {
                destinationType = sourceType;
                sourceType = maps.First(x => x.Key.destinationType == destinationType).Key.sourceType;
                sourceNumber = FindSourceNumber(maps, sourceType, destinationType, sourceNumber);
            }

            return sourceNumber;
        }
    }

    class Range
    {
        public Range(long destinationStart, long sourceStart, long length)
        {
            DestinationStart = destinationStart;
            SourceStart = sourceStart;
            Length = length;
            SourceEnd = SourceStart + length;
            DestinationEnd = DestinationStart + length;
        }

        public long DestinationStart { get; }
        public long SourceStart { get; }
        public long Length { get; }
        public long SourceEnd { get; }
        public long DestinationEnd { get; }

        public long CalculateSourceNumber(long destinationNumber) => SourceStart + (destinationNumber - DestinationStart);
        public long CalculateDestinationNumber(long sourceNumber) => DestinationStart + (sourceNumber - SourceStart);
    }
}