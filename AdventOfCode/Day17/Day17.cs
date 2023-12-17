internal static class Day17
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day17/Input.txt").ToArray();
        var map = new int[lines.Length, lines[0].Length];

        for (int row = 0; row < map.GetLength(0); row++)
        {
            for (int column = 0; column < map.GetLength(1); column++)
            {
                map[row, column] = (int)char.GetNumericValue(lines[row][column]);
            }
        }

        Console.WriteLine($"Day 17, Part 1: {GetHeatLoss(isUltra: false)}");
        Console.WriteLine($"Day 17, Part 2: {GetHeatLoss(isUltra: true)}");

        int GetHeatLoss(bool isUltra)
        {
            var results = new List<int>();
            var visited = new HashSet<string>();
            var priorityQueue = new PriorityQueue<Position, int>();

            priorityQueue.Enqueue(new Position(0, 0, 'R', 0, isUltra), 0);
            priorityQueue.Enqueue(new Position(0, 0, 'D', 0, isUltra), 0);

            while (priorityQueue.Count > 0)
            {
                priorityQueue.TryDequeue(out var current, out var loss);

                foreach (var neighbor in GetNeighbors(current!))
                {
                    if (neighbor.Row < 0 || neighbor.Column < 0 || neighbor.Row >= map.GetLength(0) || neighbor.Column >= map.GetLength(1))
                    {
                        continue;
                    }

                    if (current!.Row == map.GetLength(0) - 1 && current.Column == map.GetLength(0) - 1)
                    {
                        results.Add(loss);
                        continue;
                    }

                    if (!visited.Contains(neighbor.Key))
                    {
                        visited.Add(neighbor.Key);
                        priorityQueue.Enqueue(neighbor, loss + map[neighbor.Row, neighbor.Column]);
                    }
                }
            }

            return results.Min();
        }

        IEnumerable<Position> GetNeighbors(Position position)
        {
            if (position.IsUltra)
            {
                if (position.Steps < 10)
                {
                    yield return position.GoForward();
                }
                
                if (position.Steps >= 4)
                {
                    yield return position.TurnLeft();
                    yield return position.TurnRight();
                }
            }
            else
            {
                if (position.Steps < 3)
                {
                    yield return position.GoForward();
                }

                yield return position.TurnLeft();
                yield return position.TurnRight();
            }
        }
    }

    class Position
    {
        public Position(int row, int column, char direction, int steps, bool isUltra)
        {
            Row = row;
            Column = column;
            Direction = direction;
            Steps = steps;
            IsUltra = isUltra;
        }

        public int Row { get; private set; }
        public int Column { get; private set; }
        public char Direction { get; private set; }
        public int Steps { get; private set; }
        public bool IsUltra { get; private set; }
        public string Key => $"{Row},{Column},{Direction},{Steps}";

        public Position GoForward() => Direction switch
        {
            'D' => new Position(Row + 1, Column, 'D', Steps + 1, IsUltra),
            'U' => new Position(Row - 1, Column, 'U', Steps + 1, IsUltra),
            'R' => new Position(Row, Column + 1, 'R', Steps + 1, IsUltra),
            'L' => new Position(Row, Column - 1, 'L', Steps + 1, IsUltra),
            _ => throw new NotImplementedException()
        };

        public Position TurnLeft() => Turn(GetLeft(Direction));

        public Position TurnRight() => Turn(GetRight(Direction));

        private Position Turn(char direction) => direction switch
        {
            'D' => new Position(Row + 1, Column, 'D', 1, IsUltra),
            'U' => new Position(Row - 1, Column, 'U', 1, IsUltra),
            'R' => new Position(Row, Column + 1, 'R', 1, IsUltra),
            'L' => new Position(Row, Column - 1, 'L', 1, IsUltra),
            _ => throw new NotImplementedException()
        };

        private char GetLeft(char current) => current switch
        {
            'D' => 'R',
            'U' => 'L',
            'R' => 'U',
            'L' => 'D',
            _ => throw new NotImplementedException()
        };

        private char GetRight(char current) => current switch
        {
            'D' => 'L',
            'U' => 'R',
            'R' => 'D',
            'L' => 'U',
            _ => throw new NotImplementedException()
        };

        public override string ToString() => Key;
    }
}