internal static class Day10
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day10/Input.txt").ToArray();
        var tiles = new List<Tile>();

        for (int line = 0; line < lines.Count(); line++)
        {
            for (int column = 0; column < lines[line].Count(); column++)
            {
                tiles.Add(new Tile(line, column, lines[line][column]));
            }
        }

        var startingTile = tiles.Single(x => x.Value == 'S');
        var possibleConnections = GetPossibleConnections(lines, startingTile).ToArray();
        var validConnection = possibleConnections.First(x => lines[x.line][x.column] == x.value);
        var startingPosition = new Position(startingTile.Line, startingTile.Column, startingTile.Value);
        var currentPosition = new Position(startingPosition, validConnection.line, validConnection.column, validConnection.value);

        while (currentPosition.Value != 'S')
        {
            var previousPosition = currentPosition.Previous!;

            currentPosition = GetPossibleConnections(lines, currentPosition)
                .Where(x => x.line != previousPosition.Line || x.column != previousPosition.Column)
                .Where(x => lines[x.line][x.column] == x.value)
                .Select(x => new Position(currentPosition, x.line, x.column, x.value))
                .Single();
        }

        var loopTiles = 0;
        var boundaries = currentPosition!.ReadLoop().ToArray();
        var validVerticalJoints = new HashSet<string>() { "L7", "FJ", "LS", "S7", "FS", "SJ" };

        foreach (var tile in tiles)
        {
            if (boundaries.Any(x => x.Line == tile.Line && x.Column == tile.Column))
            {
                continue;
            }

            var bondaryTilesToTheLeft = boundaries.Where(x => x.Line == tile.Line && x.Column < tile.Column);
            var verticalBoundaryTilesToTheLeft = bondaryTilesToTheLeft.Count(x => x.Value == '|');

            var jointBoundaryTilesToTheLeft = bondaryTilesToTheLeft
                .Where(x => "L7FJS".Contains(x.Value))
                .OrderBy(x => x.Column)
                .Select(x => x.Value)
                .ToArray();

            for (int i = 1; i < jointBoundaryTilesToTheLeft.Count(); i++)
            {
                var verticalJoint = $"{jointBoundaryTilesToTheLeft[i - 1]}{jointBoundaryTilesToTheLeft[i]}";

                if (validVerticalJoints.Contains(verticalJoint))
                {
                    verticalBoundaryTilesToTheLeft++;
                }
            }

            if (verticalBoundaryTilesToTheLeft % 2 == 1)
            {
                loopTiles++;
            }
        }

        Console.WriteLine($"Day 10, Part 1: {currentPosition!.Steps / 2}");
        Console.WriteLine($"Day 10, Part 2: {loopTiles}");

        IEnumerable<(int line, int column, char value)> GetPossibleConnections(string[] lines, Tile tile)
        {
            var left = new[]
            {
                (tile.Line, tile.Column - 1, '-'),
                (tile.Line, tile.Column - 1, 'F'),
                (tile.Line, tile.Column - 1, 'L'),
                (tile.Line, tile.Column - 1, 'S')
            };

            var right = new[]
            {
                (tile.Line, tile.Column + 1, '-'),
                (tile.Line, tile.Column + 1, 'J'),
                (tile.Line, tile.Column + 1, '7'),
                (tile.Line, tile.Column + 1, 'S')
            };

            var bottom = new[]
            {
                (tile.Line + 1, tile.Column, '|'),
                (tile.Line + 1, tile.Column, 'L'),
                (tile.Line + 1, tile.Column, 'J'),
                (tile.Line + 1, tile.Column, 'S')
            };

            var up = new[]
            {
                (tile.Line - 1, tile.Column, '|'),
                (tile.Line - 1, tile.Column, 'F'),
                (tile.Line - 1, tile.Column, '7'),
                (tile.Line - 1, tile.Column, 'S')
            };

            return lines[tile.Line][tile.Column] switch
            {
                'S' => right.Concat(left).Concat(bottom).Concat(up),
                '-' => right.Concat(left),
                '|' => bottom.Concat(up),
                'F' => right.Concat(bottom),
                '7' => left.Concat(bottom),
                'L' => right.Concat(up),
                'J' => left.Concat(up),
                _ => throw new NotImplementedException()
            };
        }
    }

    class Tile
    {
        public Tile(int line, int column, char value)
        {
            Line = line;
            Column = column;
            Value = value;
        }

        public int Line { get; }
        public int Column { get; }
        public char Value { get; set; }
    }

    class Position : Tile
    {
        public Position(int line, int column, char value) : base(line, column, value)
        {
            Steps = 0;
        }

        public Position(Position previous, int line, int column, char value) : base(line, column, value)
        {
            Previous = previous;
            Steps = previous.Steps + 1;
        }

        public Position? Previous { get; }
        public int Steps { get; }

        public IEnumerable<Position> ReadLoop()
        {
            var current = this;

            while (current.Previous != null)
            {
                current = current.Previous;
                yield return current;
            }
        }
    }
}