using System.Numerics;

internal static class Day24
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day24/Input.txt");
        var hailstones = lines.Select(x => new Hailstone(x)).ToArray();

        Console.WriteLine($"Day 24, Part 1: {CountFutureIntersections(hailstones)}");
        Console.WriteLine($"Day 24, Part 2: {InterceptAllHailstones(hailstones)}");

        int CountFutureIntersections(Hailstone[] hailstones)
        {
            long testAreaMin = hailstones.Length > 5 ? 200000000000000 : 7;
            long testAreaMax = hailstones.Length > 5 ? 400000000000000 : 27;

            var intersections = new HashSet<(Hailstone, Hailstone)>();

            foreach (var stone1 in hailstones)
            {
                foreach (var stone2 in hailstones)
                {
                    if (stone1 == stone2)
                    {
                        continue;
                    }

                    if (intersections.Contains((stone1, stone2)) || intersections.Contains((stone2, stone1)))
                    {
                        continue;
                    }

                    var intersection = FindIntersection(stone1, stone2, true);

                    if (!intersection.HasValue)
                    {
                        continue;
                    }

                    if (intersection.Value.x >= testAreaMin &&
                        intersection.Value.y >= testAreaMin &&
                        intersection.Value.x <= testAreaMax &&
                        intersection.Value.y <= testAreaMax)
                    {
                        intersections.Add((stone1, stone2));
                    }
                }
            }

            return intersections.Count;
        }

        BigInteger InterceptAllHailstones(Hailstone[] hailstones)
        {
            for (int x = -500; x <= 500; x++)
            {
                for (int y = -500; y <= 500; y++)
                {
                    var intersection1 = FindIntersection(hailstones[1].WithModifiedVelocity(x, y), hailstones[0].WithModifiedVelocity(x, y));
                    var intersection2 = FindIntersection(hailstones[2].WithModifiedVelocity(x, y), hailstones[0].WithModifiedVelocity(x, y));
                    var intersection3 = FindIntersection(hailstones[3].WithModifiedVelocity(x, y), hailstones[0].WithModifiedVelocity(x, y));

                    if (!intersection1.HasValue || !intersection2.HasValue || !intersection3.HasValue)
                    {
                        continue;
                    }

                    if (intersection1.Value.x != intersection2.Value.x || intersection1.Value.y != intersection2.Value.y)
                    {
                        continue;
                    }

                    if (intersection1.Value.x != intersection3.Value.x || intersection1.Value.y != intersection3.Value.y)
                    {
                        continue;
                    }

                    for (int z = -500; z <= 500; z++)
                    {
                        var intersectionZ1 = hailstones[1].Position.z + intersection1.Value.time * (hailstones[1].Velocity.z + z);
                        var intersectionZ2 = hailstones[2].Position.z + intersection2!.Value.time * (hailstones[2].Velocity.z + z);
                        var intersectionZ3 = hailstones[3].Position.z + intersection3!.Value.time * (hailstones[3].Velocity.z + z);

                        if (intersectionZ1 != intersectionZ2 || intersectionZ1 != intersectionZ3)
                        {
                            continue;
                        }

                        return intersection1.Value.x + intersection1.Value.y + intersectionZ1;
                    }
                }
            }

            return -1;
        }

        (BigInteger x, BigInteger y, BigInteger time)? FindIntersection(Hailstone stone1, Hailstone stone2, bool futureOnly = false)
        {
            var x1 = stone1.Position.x;
            var y1 = stone1.Position.y;
            var x2 = stone1.NextPosition.x;
            var y2 = stone1.NextPosition.y;
            var x3 = stone2.Position.x;
            var y3 = stone2.Position.y;
            var x4 = stone2.NextPosition.x;
            var y4 = stone2.NextPosition.y;

            var denominator = ((x1 - x2) * (y3 - y4)) - ((y1 - y2) * (x3 - x4));

            if (denominator == 0)
            {
                return null;
            }

            var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denominator;
            var u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / denominator;

            if (futureOnly && (t < 0 || u < 0))
            {
                return null;
            }

            return (x: x1 + t * (x2 - x1), y: y1 + t * (y2 - y1), time: t);
        }
    }

    class Hailstone
    {
        private Hailstone() { }

        public Hailstone(string line)
        {
            var lineSplit = line.Split('@');
            var positionSplit = lineSplit[0].Split(',', StringSplitOptions.TrimEntries).Select(BigInteger.Parse).ToArray();
            var velocitySplit = lineSplit[1].Split(',', StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();

            Position = (positionSplit[0], positionSplit[1], positionSplit[2]);
            Velocity = (velocitySplit[0], velocitySplit[1], velocitySplit[2]);
        }

        public (BigInteger x, BigInteger y, BigInteger z) Position { get; private set; }
        public (int x, int y, int z) Velocity { get; private set; }
        public (BigInteger x, BigInteger y, BigInteger z) NextPosition => (Position.x + Velocity.x, Position.y + Velocity.y, Position.z + Velocity.z);

        public Hailstone WithModifiedVelocity(int x, int y)
        {
            return new Hailstone()
            {
                Position = Position,
                Velocity = new(Velocity.x + x, Velocity.y + y, Velocity.z)
            };
        }

        public override string ToString() => Position.ToString();
    }
}