using System.Diagnostics;
using System.Globalization;

internal static class Day18
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day18/Input.txt");

        var commands = lines.Select(line =>
        {
            var lineSplit = line.Split(' ');
            var direction = lineSplit[0][0];
            var steps = int.Parse(lineSplit[1]);
            var hexa = lineSplit[2].Substring(lineSplit[2].IndexOf("#") + 1, 6);
            return (direction, steps, hexa);
        }).ToArray();

        var stopwatch = Stopwatch.StartNew();

        var loop = ParseLoop(commands).OrderBy(x => x.Start.column).ToArray();
        Console.WriteLine($"Day 18, Part 1: parsed in {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Day 18, Part 1: {Count(loop)} in {stopwatch.ElapsedMilliseconds}ms");

        stopwatch.Restart();

        var loopFromHexa = ParseLoopFromHexa(commands).OrderBy(x => x.Start.column).ToArray();
        Console.WriteLine($"Day 18, Part 2: parsed in {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Day 18, Part 2: {Count(loopFromHexa)} in {stopwatch.ElapsedMilliseconds}ms");

        long Count(Line[] loop)
        {
            var rowMin = loop.Min(x => x.RowMin);
            var rowMax = loop.Max(x => x.RowMax);
            var counter = loop.Sum(x => x.CountBlocks());
            var verticalLines = loop.Where(x => x.IsVertical).ToArray();
            var boundaries = loop.SelectMany(x => x.Points).ToHashSet();

            for (int row = rowMin; row <= rowMax; row++)
            {
                var columnMin = loop.Where(x => x.Rows.Contains(row)).Min(x => x.ColumnMin);
                var columnMax = loop.Where(x => x.Rows.Contains(row)).Max(x => x.ColumnMax);

                for (int column = columnMin; column <= columnMax; column++)
                {
                    if (boundaries.Contains((row, column)))
                    {
                        continue;
                    }

                    var nextLineToTheRight = verticalLines.Where(x => x.ColumnMin > column && x.RowIsInRange(row)).FirstOrDefault();

                    if (nextLineToTheRight == null)
                    {
                        continue;
                    }

                    var linesToTheLeft = 0;
                    var lastLineDirection = 'X';
                    var possibleLinesToTheLeft = verticalLines.Where(x => x.ColumnMin < column && x.RowIsInRange(row)).ToArray();

                    foreach (var possibleLine in possibleLinesToTheLeft)
                    {
                        if (possibleLine.Direction != lastLineDirection)
                        {
                            linesToTheLeft++;
                        }

                        lastLineDirection = possibleLine.Direction;
                    }

                    if (linesToTheLeft % 2 == 1)
                    {
                        counter += nextLineToTheRight.Start.column - column;
                    }

                    column = nextLineToTheRight.Start.column;
                }
            }

            return counter;
        }

        IEnumerable<Line> ParseLoop((char direction, int steps, string hexa)[] commands)
        {
            var start = (row: 10_000_000, column: 10_000_000);

            for (int i = 0; i < commands.Length; i++)
            {
                var command = commands[i];

                var end = command.direction switch
                {
                    'D' => (start.row + command.steps, start.column),
                    'U' => (start.row - command.steps, start.column),
                    'R' => (start.row, start.column + command.steps),
                    'L' => (start.row, start.column - command.steps),
                    _ => throw new NotImplementedException()
                };

                yield return new Line(start, end, command.direction);

                start = end;
            }
        }

        IEnumerable<Line> ParseLoopFromHexa((char direction, int steps, string hexa)[] commands)
        {
            var start = (row: 10_000_000, column: 10_000_000);

            for (int i = 0; i < commands.Length; i++)
            {
                var command = commands[i];
                var hexaSteps = int.Parse(command.hexa.Substring(0, 5), NumberStyles.HexNumber);

                var hexaDirection = command.hexa.Substring(5, 1) switch
                {
                    "0" => 'R',
                    "1" => 'D',
                    "2" => 'L',
                    "3" => 'U',
                    _ => throw new NotImplementedException(),
                };

                var end = hexaDirection switch
                {
                    'D' => (start.row + hexaSteps, start.column),
                    'U' => (start.row - hexaSteps, start.column),
                    'R' => (start.row, start.column + hexaSteps),
                    'L' => (start.row, start.column - hexaSteps),
                    _ => throw new NotImplementedException()
                };

                yield return new Line(start, end, hexaDirection);

                start = end;
            }
        }
    }

    class Line
    {
        public Line((int row, int column) start, (int row, int column) end, char direction)
        {
            Start = start;
            End = end;
            Direction = direction;
            IsVertical = direction == 'U' || direction == 'D';
            RowMin = Math.Min(Start.row, End.row);
            RowMax = Math.Max(Start.row, End.row);
            ColumnMin = Math.Min(Start.column, End.column);
            ColumnMax = Math.Max(Start.column, End.column);
            Rows = Enumerable.Range(RowMin, RowMax - RowMin + 1).ToHashSet();

            if (RowMin == RowMax)
            {
                Points = Enumerable.Range(ColumnMin, ColumnMax - ColumnMin + 1).Select(x => (RowMin, x)).ToHashSet();
            }
            else
            {
                Points = Rows.Select(x => (x, ColumnMin)).ToHashSet();
            }
        }

        public (int row, int column) Start { get; }
        public (int row, int column) End { get; }
        public char Direction { get; }
        public bool IsVertical { get; }
        public int RowMin { get; }
        public int RowMax { get; }
        public int ColumnMin { get; }
        public int ColumnMax { get; }
        public HashSet<int> Rows { get; }
        public HashSet<(int row, int column)> Points { get; }

        public long CountBlocks() => Points.Count - 1;

        public bool ContainsBlock(int row, int column) => Points.Contains((row, column));

        public bool RowIsInRange(int row) => Rows.Contains(row);

        public override string ToString() => $"From {Start.row},{Start.column} to {End.row},{End.column} going {Direction}";
    }
}