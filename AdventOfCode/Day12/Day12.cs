using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

internal static class Day12
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day12/Input.txt").Select(x => Regex.Replace(x, @"\.{2,}", ".")).ToArray();
        var springRowsPart1 = new Dictionary<int, string>();
        var damagedMapsPart1 = new Dictionary<int, int[]>();

        for (int i = 0; i < lines.Count(); i++)
        {
            var line = lines[i];
            var lineSplit = line.Split(' ');

            springRowsPart1.Add(i, lineSplit[0]);
            damagedMapsPart1.Add(i, lineSplit[1].Split(',').Select(int.Parse).ToArray());
        }

        Console.WriteLine($"Day 12, Part 1: {CountPossibleArrangements(springRowsPart1, damagedMapsPart1)}");

        var springRowsPart2 = new Dictionary<int, string>();
        var damagedMapsPart2 = new Dictionary<int, int[]>();

        for (int i = 0; i < lines.Count(); i++)
        {
            var line = lines[i];
            var lineSplit = line.Split(' ');

            springRowsPart2.Add(i, lineSplit[0] + string.Concat(Enumerable.Repeat($"?{lineSplit[0]}", 4)));
            damagedMapsPart2.Add(i, Enumerable.Repeat(lineSplit[1].Split(',').Select(int.Parse), 5).SelectMany(x => x).ToArray());
        }

        Console.WriteLine($"Day 12, Part 2: {CountPossibleArrangements(springRowsPart2, damagedMapsPart2)}");


        long CountPossibleArrangements(Dictionary<int, string> springRows, Dictionary<int, int[]> damagedMap)
        {
            var sum = 0L;
            var stopWatch = Stopwatch.StartNew();

            Parallel.For(0, springRows.Count, new ParallelOptions() { MaxDegreeOfParallelism = -1 }, (i) =>
            {
                var rowDamagedMap = damagedMap[i];
                var memo = new Dictionary<string, long>();

                Interlocked.Add(ref sum, CountPossibleArrangementsRecursive(springRows[i], rowDamagedMap, 0, memo));
            });

            return sum;
        }

        long CountPossibleArrangementsRecursive(string springs, int[] damagedMap, long sum, Dictionary<string, long> memo)
        {
            var cacheKey = $"{new string(springs)}|{string.Join(',', damagedMap)}";

            if (memo.TryGetValue(cacheKey, out var value))
            {
                return sum + value;
            }

            var questionMarkIndex = springs.IndexOf('?');

            if (questionMarkIndex > -1)
            {
                if (!IsMatchUntilQuestionMark(springs, damagedMap, questionMarkIndex, out int newDamagedIndex, out int newLastSafeIndex))
                {
                    memo.Add(cacheKey, 0);
                    return sum;
                }

                if (!CanFormEnoughGroups(springs, damagedMap))
                {
                    memo.Add(cacheKey, 0);
                    return sum;
                }

                springs = springs.Substring(newLastSafeIndex);
                damagedMap = damagedMap.Skip(newDamagedIndex).ToArray();
                questionMarkIndex = springs.IndexOf('?');

                var sumBefore = sum;
                var newSprings = springs.Remove(questionMarkIndex, 1);

                sum = CountPossibleArrangementsRecursive(newSprings.Insert(questionMarkIndex, "."), damagedMap, sum, memo);
                sum = CountPossibleArrangementsRecursive(newSprings.Insert(questionMarkIndex, "#"), damagedMap, sum, memo);

                memo.Add(cacheKey, sum - sumBefore);
            }
            else if (IsPossible(springs, damagedMap))
            {
                memo.Add(cacheKey, 1);
                return sum + 1;
            }

            return sum;
        }

        bool IsMatchUntilQuestionMark(string springs, int[] damagedMap, int questionMarkIndex, out int newDamagedIndex, out int newLastSafeIndex)
        {
            newDamagedIndex = 0;
            newLastSafeIndex = 0;

            if (questionMarkIndex >= 0)
            {
                int length = 0;

                for (int i = 0; i < questionMarkIndex; i++)
                {
                    if (springs[i] == '.')
                    {
                        if (length > 0)
                        {
                            if (newDamagedIndex >= damagedMap.Length || length != damagedMap[newDamagedIndex])
                            {
                                return false;
                            }

                            newLastSafeIndex = i;
                            newDamagedIndex++;
                            length = 0;
                        }
                    }
                    else
                    {
                        length++;
                    }
                }

                if (newDamagedIndex > damagedMap.Length)
                {
                    return false;
                }
            }

            return true;
        }

        bool IsPossible(string springs, int[] damagedMap)
        {
            var length = 0;
            var damagedMapIndex = 0;

            foreach (char spring in springs)
            {
                if (spring == '.')
                {
                    if (length > 0)
                    {
                        if (damagedMapIndex >= damagedMap.Length || damagedMap[damagedMapIndex] != length)
                        {
                            return false;
                        }

                        damagedMapIndex++;
                        length = 0;
                    }
                }
                else
                {
                    length++;
                }
            }

            if (length > 0)
            {
                if (damagedMapIndex >= damagedMap.Length || damagedMap[damagedMapIndex] != length)
                {
                    return false;
                }

                damagedMapIndex++;
            }

            return damagedMapIndex == damagedMap.Length;
        }

        bool CanFormEnoughGroups(string springs, int[] damagedMap)
        {
            if (damagedMap.Length == 0)
            {
                return true;
            }

            var currentDamageIndex = 0;
            var currentSectionCounter = 0;

            for (int i = 0; i < springs.Length; i++)
            {
                if (springs[i] == '?' || springs[i] == '#')
                {
                    currentSectionCounter++;
                }

                if (springs[i] == '.' || i == springs.Length - 1)
                {
                    while (currentSectionCounter >= damagedMap[currentDamageIndex])
                    {
                        currentSectionCounter -= damagedMap[currentDamageIndex] + 1;
                        currentDamageIndex++;

                        if (currentDamageIndex >= damagedMap.Length)
                        {
                            return true;
                        }
                    }

                    currentSectionCounter = 0;
                }
            }

            return false;
        }
    }
}