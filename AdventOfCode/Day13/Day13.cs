internal static class Day13
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day13/Input.txt").ToArray();
        var mirrors = new List<char[,]>();
        var mirrorLines = new List<string>();

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i]) || i == lines.Length - 1)
            {
                if (i == lines.Length - 1)
                {
                    mirrorLines.Add(lines[i]);
                }

                var mirror = new char[mirrorLines.Count, mirrorLines[0].Length];

                for (int line = 0; line < mirrorLines.Count; line++)
                {
                    for (int column = 0; column < mirrorLines[0].Length; column++)
                    {
                        mirror[line, column] = mirrorLines[line][column];
                    }
                }

                mirrors.Add(mirror);
                mirrorLines = new List<string>();
                continue;
            }

            mirrorLines.Add(lines[i]);
        }

        var sumPart1 = 0;

        foreach (var mirror in mirrors)
        {
            sumPart1 += GetReflectionPoint(mirror) * 100;
            sumPart1 += GetReflectionPoint(RotateClockwise(mirror));
        }

        var sumPart2 = 0;

        foreach (var mirror in mirrors)
        {
            sumPart2 += GetReflectionPointWithSmudge(mirror) * 100;
            sumPart2 += GetReflectionPointWithSmudge(RotateClockwise(mirror));
        }

        Console.WriteLine($"Day 13, Part 1: {sumPart1}");
        Console.WriteLine($"Day 13, Part 2: {sumPart2}");

        char[,] RotateClockwise(char[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            var rotatedMatrix = new char[columns, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    rotatedMatrix[j, rows - 1 - i] = matrix[i, j];
                }
            }

            return rotatedMatrix;
        }

        char[] ReadRow(char[,] matrix, int rowIndex)
        {
            var rowValue = new char[matrix.GetLength(1)];

            for (int column = 0; column < matrix.GetLength(1); column++)
            {
                rowValue[column] = matrix[rowIndex, column];
            }

            return rowValue;
        }

        int GetReflectionPoint(char[,] mirror)
        {
            var amountOfRows = mirror.GetLength(0);

            for (int rowIndex = 0; rowIndex < amountOfRows - 1; rowIndex++)
            {
                var row = ReadRow(mirror, rowIndex);
                var nextRow = ReadRow(mirror, rowIndex + 1);
                var offset = 0;

                while (row.SequenceEqual(nextRow))
                {
                    offset++;

                    var upperIndex = rowIndex - offset;
                    var bottomIndex = rowIndex + 1 + offset;

                    if (upperIndex < 0 || bottomIndex == amountOfRows)
                    {
                        return rowIndex + 1;
                    }

                    row = ReadRow(mirror, upperIndex);
                    nextRow = ReadRow(mirror, bottomIndex);
                }
            }

            return 0;
        }

        int GetReflectionPointWithSmudge(char[,] mirror)
        {
            var amountOfRows = mirror.GetLength(0);

            for (int rowIndex = 0; rowIndex < amountOfRows - 1; rowIndex++)
            {
                var row = ReadRow(mirror, rowIndex);
                var nextRow = ReadRow(mirror, rowIndex + 1);
                var differences = CountDifferences(row, nextRow);
                var smudgeUsed = false;
                var offset = 0;

                while (differences == 0 || (differences == 1 && !smudgeUsed))
                {
                    if (differences == 1)
                    {
                        smudgeUsed = true;
                    }

                    offset++;

                    var upperIndex = rowIndex - offset;
                    var bottomIndex = rowIndex + 1 + offset;

                    if (upperIndex < 0 || bottomIndex == amountOfRows)
                    {
                        if (!smudgeUsed)
                        {
                            break;
                        }

                        return rowIndex + 1;
                    }

                    row = ReadRow(mirror, upperIndex);
                    nextRow = ReadRow(mirror, bottomIndex);
                    differences = CountDifferences(row, nextRow);
                }
            }

            return 0;
        }

        int CountDifferences(char[] row1, char[] row2)
        {
            var counter = 0;

            for (int i = 0; i < row1.Length; i++)
            {
                if (row1[i] != row2[i])
                {
                    counter++;
                }
            }

            return counter;
        }
    }
}