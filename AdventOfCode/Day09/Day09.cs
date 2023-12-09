internal static class Day09
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day09/Input.txt");
        var sequences = lines.Select(x => x.Split(" ").Select(int.Parse).ToArray()).ToArray();
        var variations = new List<List<List<int>>>();

        foreach (var sequence in sequences)
        {
            var newDifferences = new List<int>();
            var previousDifferences = sequence.Select(x => x).ToList();
            variations.Add(new List<List<int>>() { previousDifferences });

            do
            {
                for (int j = 1; j < previousDifferences.Count(); j++)
                {
                    newDifferences.Add(previousDifferences[j] - previousDifferences[j - 1]);
                }

                variations.Last().Add(newDifferences);
                previousDifferences = newDifferences;
                newDifferences = new List<int>();
            }
            while (previousDifferences.Any(x => x != 0));
        }

        foreach (var variation in variations)
        {
            for (int i = variation.Count() - 2; i >= 0; i--)
            {
                variation[i].Add(variation[i].Last() + variation[i + 1].Last());
                variation[i].Insert(0, variation[i].First() - variation[i + 1].First());
            }
        }

        Console.WriteLine($"Day 9, Part 1: {variations.Sum(x => x.First().Last())}");
        Console.WriteLine($"Day 9, Part 2: {variations.Sum(x => x.First().First())}");
    }
}