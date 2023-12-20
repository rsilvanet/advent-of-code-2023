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

        var vertices = ParseVertices(commands).ToArray();
        var verticesFromHexa = ParseVerticesFromHexa(commands).ToArray();

        Console.WriteLine($"Day 18, Part 1: {CalculateArea(vertices)}");
        Console.WriteLine($"Day 18, Part 2: {CalculateArea(verticesFromHexa)}");

        long CalculateArea((int row, int column)[] vertices)
        {
            long area = 0;
            long contour = 0;
            
            for (int i = 0; i < vertices.Length; i++)
            {
                int next = (i + 1) % vertices.Length;
                area += (long)vertices[i].row * vertices[next].column;
                area -= (long)vertices[i].column * vertices[next].row;
                contour += CalculateDistance(vertices[i], vertices[next]);
            }

            area = Math.Abs(area) / 2;
            area += contour / 2 + 1;
            return area;
        }

        long CalculateDistance((int row, int column) point1, (int row, int column) point2)
        {
            return (long)Math.Sqrt(Math.Pow(point2.row - point1.row, 2) + Math.Pow(point2.column - point1.column, 2));
        }

        IEnumerable<(int row, int column)> ParseVertices((char direction, int steps, string hexa)[] commands)
        {
            var current = (row: 0, column: 0);

            for (int i = 0; i < commands.Length; i++)
            {
                var command = commands[i];

                current = command.direction switch
                {
                    'D' => (current.row + command.steps, current.column),
                    'U' => (current.row - command.steps, current.column),
                    'R' => (current.row, current.column + command.steps),
                    'L' => (current.row, current.column - command.steps),
                    _ => throw new NotImplementedException()
                };

                yield return current;
            }
        }

        IEnumerable<(int row, int column)> ParseVerticesFromHexa((char direction, int steps, string hexa)[] commands)
        {
            var current = (row: 0, column: 0);

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

                current = hexaDirection switch
                {
                    'D' => (current.row + hexaSteps, current.column),
                    'U' => (current.row - hexaSteps, current.column),
                    'R' => (current.row, current.column + hexaSteps),
                    'L' => (current.row, current.column - hexaSteps),
                    _ => throw new NotImplementedException()
                };

                yield return current;
            }
        }
    }
}