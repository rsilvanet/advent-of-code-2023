internal static class Day10
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day10/Input.txt").ToArray();
        var tiles = new List<Position>();

        for (int line = 0; line < lines.Count(); line++)
        {
            for (int column = 0; column < lines[line].Count(); column++)
            {
                tiles.Add(new Position(line, column, lines[line][column]));
            }
        }

        var startingPosition = tiles.Single(x => x.Value == 'S');
        var possibleConnections = GetPossibleConnections(lines, startingPosition).ToArray();
        var validConnection = possibleConnections.First(x => lines[x.line][x.column] == x.value);

        var queue = new Queue<Position>();
        var currentPosition = default(Position);

        queue.Enqueue(new Position(startingPosition, validConnection.line, validConnection.column, validConnection.value));

        while (queue.Any())
        {
            currentPosition = queue.Dequeue();

            if (currentPosition.Value == 'S')
            {
                break;
            }

            foreach (var connection in GetPossibleConnections(lines, currentPosition))
            {
                if (connection.line == currentPosition.Previous!.Line && connection.column == currentPosition.Previous!.Column)
                {
                    continue;
                }

                if (lines[connection.line][connection.column] == connection.value)
                {
                    queue.Enqueue(new Position(currentPosition, connection.line, connection.column, connection.value));
                }
            }
        }

        var loopTiles = 0;
        var boundaries = currentPosition!.ReadLoop().ToArray();

        tiles.Single(x => x.Value == 'S').Value = 'J';

        foreach (var tile in tiles)
        {
            if (boundaries.Any(x => x.Line == tile.Line && x.Column == tile.Column))
            {
                continue;
            }

            var bondaryTilesToTheLeft = boundaries.Where(x => x.Line == tile.Line && x.Column < tile.Column);
            var verticalBoundaryTilesToTheLeft = bondaryTilesToTheLeft.Count(x => x.Value == '|');

            var jointBoundaryTilesToTheLeft = bondaryTilesToTheLeft
                .Where(x => "L7FJ".Contains(x.Value))
                .OrderBy(x => x.Column)
                .Select(x => x.Value)
                .ToArray();

            for (int i = 1; i < jointBoundaryTilesToTheLeft.Count(); i++)
            {
                var verticalJoint = $"{jointBoundaryTilesToTheLeft[i - 1]}{jointBoundaryTilesToTheLeft[i]}";

                if (verticalJoint == "L7" || verticalJoint == "FJ")
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

        IEnumerable<(int line, int column, char value)> GetPossibleConnections(string[] lines, Position position)
        {
            var value = lines[position.Line][position.Column];

            if (value == 'S')
            {
                yield return (position.Line, position.Column + 1, '-');
                yield return (position.Line, position.Column + 1, 'J');
                yield return (position.Line, position.Column + 1, '7');

                yield return (position.Line, position.Column - 1, '-');
                yield return (position.Line, position.Column - 1, 'F');
                yield return (position.Line, position.Column - 1, 'L');

                yield return (position.Line + 1, position.Column, '|');
                yield return (position.Line + 1, position.Column, 'L');
                yield return (position.Line + 1, position.Column, 'J');

                yield return (position.Line - 1, position.Column, '|');
                yield return (position.Line - 1, position.Column, 'F');
                yield return (position.Line - 1, position.Column, '7');
            }
            else if (value == '-')
            {
                yield return (position.Line, position.Column + 1, '-');
                yield return (position.Line, position.Column + 1, 'J');
                yield return (position.Line, position.Column + 1, '7');
                yield return (position.Line, position.Column + 1, 'S');

                yield return (position.Line, position.Column - 1, '-');
                yield return (position.Line, position.Column - 1, 'F');
                yield return (position.Line, position.Column - 1, 'L');
                yield return (position.Line, position.Column - 1, 'S');
            }
            else if (value == '|')
            {
                yield return (position.Line + 1, position.Column, '|');
                yield return (position.Line + 1, position.Column, 'L');
                yield return (position.Line + 1, position.Column, 'J');
                yield return (position.Line + 1, position.Column, 'S');

                yield return (position.Line - 1, position.Column, '|');
                yield return (position.Line - 1, position.Column, 'F');
                yield return (position.Line - 1, position.Column, '7');
                yield return (position.Line - 1, position.Column, 'S');
            }
            else if (value == 'F')
            {
                yield return (position.Line, position.Column + 1, '-');
                yield return (position.Line, position.Column + 1, 'J');
                yield return (position.Line, position.Column + 1, '7');
                yield return (position.Line, position.Column + 1, 'S');

                yield return (position.Line + 1, position.Column, '|');
                yield return (position.Line + 1, position.Column, 'L');
                yield return (position.Line + 1, position.Column, 'J');
                yield return (position.Line + 1, position.Column, 'S');
            }
            else if (value == '7')
            {
                yield return (position.Line, position.Column - 1, '-');
                yield return (position.Line, position.Column - 1, 'F');
                yield return (position.Line, position.Column - 1, 'L');
                yield return (position.Line, position.Column - 1, 'S');

                yield return (position.Line + 1, position.Column, '|');
                yield return (position.Line + 1, position.Column, 'L');
                yield return (position.Line + 1, position.Column, 'J');
                yield return (position.Line + 1, position.Column, 'S');
            }
            else if (value == 'L')
            {
                yield return (position.Line, position.Column + 1, '-');
                yield return (position.Line, position.Column + 1, 'J');
                yield return (position.Line, position.Column + 1, '7');
                yield return (position.Line, position.Column + 1, 'S');

                yield return (position.Line - 1, position.Column, '|');
                yield return (position.Line - 1, position.Column, 'F');
                yield return (position.Line - 1, position.Column, '7');
                yield return (position.Line - 1, position.Column, 'S');
            }
            else if (value == 'J')
            {
                yield return (position.Line, position.Column - 1, '-');
                yield return (position.Line, position.Column - 1, 'F');
                yield return (position.Line, position.Column - 1, 'L');
                yield return (position.Line, position.Column - 1, 'S');

                yield return (position.Line - 1, position.Column, '|');
                yield return (position.Line - 1, position.Column, 'F');
                yield return (position.Line - 1, position.Column, '7');
                yield return (position.Line - 1, position.Column, 'S');
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    class Position
    {
        public Position(int line, int column, char value)
        {
            Line = line;
            Column = column;
            Value = value;
            Steps = 0;
        }

        public Position(Position previous, int line, int column, char value)
        {
            Previous = previous;
            Line = line;
            Column = column;
            Value = value;
            Steps = previous.Steps + 1;
        }

        public Position? Previous { get; set; }
        public int Line { get; }
        public int Column { get; }
        public char Value { get; set; }
        public int Steps { get; }

        public IEnumerable<Position> ReadLoop()
        {
            var curent = this;

            while(curent.Previous != null)
            {
                yield return curent.Previous;
                curent = curent.Previous;
            }
        }
    }
}