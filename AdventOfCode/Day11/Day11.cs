using System.Collections.Concurrent;

internal static class Day11
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day11/Input.txt").ToList();
        var emptyLines = lines.Select((line, index) => (line, index)).Where(x => x.line.All(c => c == '.')).Select(x => x.index).ToHashSet();
        var emptyColumns = Enumerable.Range(0, lines[0].Length).Where(c => lines.All(l => l[c] == '.')).ToHashSet();
        var galaxies = new Dictionary<(int line, int column), int>();

        for (int line = 0; line < lines.Count; line++)
        {
            for (int column = 0; column < lines[line].Length; column++)
            {
                if (lines[line][column] == '#')
                {
                    galaxies.Add((line, column), galaxies.Count + 1);
                }
            }
        }

        Console.WriteLine($"Day 11, Part 1: {CalculateDistances(2)}");
        Console.WriteLine($"Day 11, Part 2: {CalculateDistances(1_000_000)}");

        long CalculateDistances(int size)
        {
            var galaxyDistances = new ConcurrentDictionary<(int galaxy1, int galaxy2), long>();

            Parallel.ForEach(galaxies, (galaxy) =>
            {
                var galaxyId = galaxy.Value;
                var positionsQueue = new Queue<(int line, int column, long distance)>();
                var positionsVisited = new HashSet<(int line, int column)>();
                var otherGalaxiesCount = 0;

                positionsQueue.Enqueue((galaxy.Key.line + 1, galaxy.Key.column, emptyLines.Contains(galaxy.Key.line) ? size : 1));
                positionsQueue.Enqueue((galaxy.Key.line - 1, galaxy.Key.column, emptyLines.Contains(galaxy.Key.line) ? size : 1));
                positionsQueue.Enqueue((galaxy.Key.line, galaxy.Key.column + 1, emptyColumns.Contains(galaxy.Key.column) ? size : 1));
                positionsQueue.Enqueue((galaxy.Key.line, galaxy.Key.column - 1, emptyColumns.Contains(galaxy.Key.column) ? size : 1));

                while (positionsQueue.Any())
                {
                    var position = positionsQueue.Dequeue();

                    if (otherGalaxiesCount == galaxies.Count - 1)
                    {
                        continue;
                    }

                    if (positionsVisited.Contains((position.line, position.column)))
                    {
                        continue;
                    }

                    if (position.line < 0 || position.line > lines.Count || position.column < 0 || position.column > lines[0].Length)
                    {
                        continue;
                    }

                    positionsVisited.Add((position.line, position.column));

                    if (galaxies.TryGetValue((position.line, position.column), out int otherGalaxyId) && otherGalaxyId != galaxyId)
                    {
                        galaxyDistances.TryAdd((galaxyId, otherGalaxyId), position.distance);
                        otherGalaxiesCount++;
                    }

                    positionsQueue.Enqueue((position.line + 1, position.column, position.distance + (emptyLines.Contains(position.line) ? size : 1)));
                    positionsQueue.Enqueue((position.line - 1, position.column, position.distance + (emptyLines.Contains(position.line) ? size : 1)));
                    positionsQueue.Enqueue((position.line, position.column + 1, position.distance + (emptyColumns.Contains(position.column) ? size : 1)));
                    positionsQueue.Enqueue((position.line, position.column - 1, position.distance + (emptyColumns.Contains(position.column) ? size : 1)));
                }
            });
            
            return galaxyDistances.Sum(x => x.Value) / 2;
        }
    }
}