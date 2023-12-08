using System.Numerics;

internal static class Day08
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day08/Input.txt").ToArray();
        var directions = lines[0].ToCharArray();
        var instructions = ReadInstructions(lines);

        Console.WriteLine($"Day 8, Part 1: {SolvePart1(directions, instructions)}");
        Console.WriteLine($"Day 8, Part 2: {SolvePart2(directions, instructions)}");

        Dictionary<string, (string left, string right)> ReadInstructions(string[] lines)
        {
            var instructions = new Dictionary<string, (string left, string right)>();

            for (int i = 2; i < lines.Length; i++)
            {
                var lineSplit = lines[i].Replace(" ", "").Split("=");
                var directionSplit = lineSplit[1].Replace("(", "").Replace(")", "").Split(",");

                instructions.Add(lineSplit[0], (directionSplit[0], directionSplit[1]));
            }

            return instructions;
        }

        int SolvePart1(char[] directions, Dictionary<string, (string left, string right)> instructions)
        {
            var steps = 0;
            var current = "AAA";
            var directionIndex = 0;

            while (current != "ZZZ")
            {
                current = directions[directionIndex] switch
                {
                    'L' => instructions[current].left,
                    'R' => instructions[current].right,
                    _ => throw new NotImplementedException(),
                };

                steps++;
                directionIndex = directionIndex + 1 >= directions.Length ? 0 : directionIndex + 1;
            }

            return steps;
        }

        long SolvePart2(char[] directions, Dictionary<string, (string left, string right)> instructions)
        {
            var directionIndex = 0;
            var currentNodes = instructions.Select(x => x.Key).Where(x => x.EndsWith('A')).ToDictionary(x => x, y => (end: y, steps: 0L));

            while (currentNodes.Any(x => !x.Value.end.EndsWith('Z')))
            {
                foreach (var node in currentNodes)
                {
                    if (node.Value.end.EndsWith('Z'))
                    {
                        continue;
                    }

                    currentNodes[node.Key] = directions[directionIndex] switch
                    {
                        'L' => (instructions[node.Value.end].left, node.Value.steps + 1),
                        'R' => (instructions[node.Value.end].right, node.Value.steps + 1),
                        _ => throw new NotImplementedException(),
                    };
                }

                directionIndex = directionIndex + 1 >= directions.Length ? 0 : directionIndex + 1;
            }
            
            return FindLeastCommonMultiple(currentNodes.Select(x => x.Value.steps).ToArray());
        }

        long FindLeastCommonMultiple(long[] numbers) => numbers.Aggregate(FindLeastCommonMultiple2);

        long FindLeastCommonMultiple2(long a, long b) => Math.Abs(a * b) / (long)BigInteger.GreatestCommonDivisor(a, b);
    }
}