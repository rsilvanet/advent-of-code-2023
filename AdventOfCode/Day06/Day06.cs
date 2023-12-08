internal static class Day06
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day06/Input.txt");
        var raceBestTimes = lines.ElementAt(0).Replace("Time:", "").Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(int.Parse).ToArray();
        var raceBestDistances = lines.ElementAt(1).Replace("Distance:", "").Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(int.Parse).ToArray();
        var resultsPart1 = new Dictionary<(int race, int speed), long>();

        for (int race = 0; race < raceBestTimes.Length; race++)
        {
            for (int speed = 1; speed < raceBestTimes[race]; speed++)
            {
                if (IsNewRecord(speed, raceBestTimes[race], raceBestDistances[race], out long newDistance))
                {
                    resultsPart1.Add((race, speed), newDistance);
                }
            }
        }

        var singleRaceBestTime = long.Parse(lines.ElementAt(0).Replace("Time:", "").Replace(" ", ""));
        var singleRaceBestDistance = long.Parse(lines.ElementAt(1).Replace("Distance:", "").Replace(" ", ""));
        var resultsPart2 = new Dictionary<long, long>();

        for (int speed = 1; speed < singleRaceBestTime; speed++)
        {
            if (IsNewRecord(speed, singleRaceBestTime, singleRaceBestDistance, out long newDistance))
            {
                resultsPart2.Add(speed, newDistance);
            }
            else if (resultsPart2.Any())
            {
                break;
            }
        }

        Console.WriteLine($"Day 5, Part 1: {resultsPart1.GroupBy(x => x.Key.race).Select(x => x.Count()).Aggregate((x, y) => x * y)}");
        Console.WriteLine($"Day 5, Part 2: {resultsPart2.Count}");

        bool IsNewRecord(long speed, long bestTime, long bestDistance, out long newDistance) => (newDistance = speed * (bestTime - speed)) > bestDistance;
    }
}