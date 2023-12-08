internal static class Day03
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day03/Input.txt");
        var partNumbers = new List<int>();
        var possibleGears = new Dictionary<(int line, int column), List<int>>();

        for (int lineIndex = 1; lineIndex < lines.Count(); lineIndex++)
        {
            var line = lines.ElementAt(lineIndex);

            for (int columnIndex = 0; columnIndex < line.Length; columnIndex++)
            {
                var numericSequence = string.Empty;

                while (char.IsNumber(line[columnIndex]))
                {
                    numericSequence += line[columnIndex];
                    columnIndex++;
                }

                if (!string.IsNullOrWhiteSpace(numericSequence))
                {
                    var number = int.Parse(numericSequence);
                    var columnIndexBeforeNumber = columnIndex - numericSequence.Length - 1;

                    if (line[columnIndex] != '.')
                    {
                        AddOrAppend(possibleGears, line, lineIndex, columnIndex, number);
                        partNumbers.Add(number);
                        continue;
                    }

                    if (line[columnIndexBeforeNumber] != '.')
                    {
                        AddOrAppend(possibleGears, line, lineIndex, columnIndexBeforeNumber, number);
                        partNumbers.Add(number);
                        continue;
                    }

                    for (int otherColumnIndex = columnIndexBeforeNumber; otherColumnIndex <= columnIndex; otherColumnIndex++)
                    {
                        if (lines.ElementAt(lineIndex - 1)[otherColumnIndex] != '.' || lines.ElementAt(lineIndex + 1)[otherColumnIndex] != '.')
                        {
                            AddOrAppend(possibleGears, line, lineIndex - 1, otherColumnIndex, number);
                            AddOrAppend(possibleGears, line, lineIndex + 1, otherColumnIndex, number);
                            partNumbers.Add(number);
                            break;
                        }
                    }
                }
            }
        }

        Console.WriteLine($"Day 3, Part 1: {partNumbers.Sum()}");
        Console.WriteLine($"Day 3, Part 2: {possibleGears.Where(x => x.Value.Count > 1).Select(x => x.Value.Aggregate((x, y) => x * y)).Sum()}");

        void AddOrAppend(Dictionary<(int line, int column), List<int>> possibleGears, string line, int lineIndex, int columnIndex, int number)
        {
            if (line[columnIndex] == '*')
            {
                if (possibleGears.ContainsKey((lineIndex, columnIndex)))
                {
                    possibleGears[(lineIndex, columnIndex)].Add(number);
                }
                else
                {
                    possibleGears.Add((lineIndex, columnIndex), new List<int>() { number });
                }
            }
        }
    }
}