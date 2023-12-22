using System.Numerics;

internal static class Day22
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day22/Input.txt");
        var bricks = lines.Select(x => new Brick(x)).ToArray();

        while (bricks.Any(x => !x.HasStopped))
        {
            var fallingBrick = bricks.Where(brick => !brick.HasStopped).MinBy(brick => brick.LowestPoint);
            var fallingCubesXY = fallingBrick!.GetCubes().Select(cube => (cube.X, cube.Y)).ToHashSet();

            var bricksUnder = bricks.Where(brick => brick.HighestPoint < fallingBrick!.LowestPoint)
                .Where(brick => brick.GetCubes().Any(cube => fallingCubesXY.Contains((cube.X, cube.Y))))
                .ToArray();

            fallingBrick.Drop(bricksUnder);
        }

        Console.WriteLine($"Day 22, Part 1: {bricks.Count(x => x.BricksRightOnTop.Count == 0 || x.BricksRightOnTop.All(y => y.BricksRightUnder.Count > 1))}");
        Console.WriteLine($"Day 22, Part 2: {bricks.Sum(x => x.CountDependentBricks())}");
    }

    class Brick
    {
        public Brick(string line)
        {
            var label = "X";
            var lineWithLabel = line.Split('|');

            if (lineWithLabel.Length > 1)
            {
                line = lineWithLabel[0];
                label = lineWithLabel[1];
            }

            var lineSplit = line.Replace('~', ',').Split(',').Select(int.Parse).ToArray();

            Label = label;
            Start = new(lineSplit[0], lineSplit[1], lineSplit[2]);
            End = new(lineSplit[3], lineSplit[4], lineSplit[5]);
            HasStopped = Start.Z == 1 || End.Z == 1;
            BricksRightUnder = new List<Brick>();
            BricksRightOnTop = new List<Brick>();
        }

        public string Label { get; private set; }
        public Vector3 Start { get; private set; }
        public Vector3 End { get; private set; }
        public float LowestPoint => Math.Min(Start.Z, End.Z);
        public float HighestPoint => Math.Max(Start.Z, End.Z);
        public bool HasStopped { get; private set; }
        public List<Brick> BricksRightUnder { get; private set; }
        public List<Brick> BricksRightOnTop { get; private set; }
        
        public void Stop()
        {
            HasStopped = true;
        }

        public void Drop(Brick[] bricksUnder)
        {
            var heightToDrop = LowestPoint - 1;
            var highestPointToLandOn = 1f;

            if (bricksUnder.Any())
            {
                highestPointToLandOn = bricksUnder.Max(brick => brick.HighestPoint);
                heightToDrop -= highestPointToLandOn;
            }
            
            Start += new Vector3(0, 0, -heightToDrop);
            End += new Vector3(0, 0, -heightToDrop);
            HasStopped = true;
            BricksRightUnder = bricksUnder.Where(brick => brick.HighestPoint == highestPointToLandOn).ToList();

            foreach (var underBrick in BricksRightUnder)
            {
                underBrick.BricksRightOnTop.Add(this);
            }
        }

        public IEnumerable<Vector3> GetCubes()
        {
            int xStep = Start.X <= End.X ? 1 : -1;
            int yStep = Start.Y <= End.Y ? 1 : -1;
            int zStep = Start.Z <= End.Z ? 1 : -1;

            for (var x = Start.X; xStep > 0 ? x <= End.X : x >= End.X; x += xStep)
            {
                for (var y = Start.Y; yStep > 0 ? y <= End.Y : y >= End.Y; y += yStep)
                {
                    for (var z = Start.Z; zStep > 0 ? z <= End.Z : z >= End.Z; z += zStep)
                    {
                        yield return new(x, y, z);
                    }
                }
            }
        }

        public int CountDependentBricks()
        {
            var destroyed = new HashSet<Brick> { this };
            CountDependentBricks(destroyed);
            return destroyed.Count - 1;
        }

        private void CountDependentBricks(HashSet<Brick> destroyed)
        {
            var dependentBricks = BricksRightOnTop.Where(x => x.BricksRightUnder.All(y => destroyed.Contains(y)));

            foreach (var brick in dependentBricks)
            {
                destroyed.Add(brick);
            }

            foreach (var brick in dependentBricks)
            {
                brick.CountDependentBricks(destroyed);
            }
        }

        public override string ToString() => $"{Label} {Start} {End}";
    }
}