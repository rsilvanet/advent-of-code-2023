internal static class Day21
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day21/Input.txt").ToArray();
        var board = new char[lines.Length, lines[0].Length];
        var start = (row: 0, column: 0, realRow: 0, realColumn: 0);

        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int column = 0; column < board.GetLength(1); column++)
            {
                board[row, column] = lines[row][column];

                if (lines[row][column] == 'S')
                {
                    start = (row, column, row, column);
                    board[row, column] = '.';
                }
            }
        }

        var memo = new Dictionary<string, int>();
        var countByStep = Enumerable.Range(0, 1000).ToDictionary(x => x, x => 0);

        Console.WriteLine($"Day 21, Part 1: {Walk(board, start, 0, 0, 64, memo, countByStep)}");
        Console.WriteLine($"Day 21, Part 2: {CalculateQuadratic(26501365)}");

        int Walk(char[,] board, (int row, int column, int realRow, int realColumn) current, int steps, int count, int goal, Dictionary<string, int> memo, Dictionary<int, int> countByStep)
        {
            var memoKey = $"{current.realRow},{current.realColumn},{steps}";

            if (memo.TryGetValue(memoKey, out int cached))
            {
                return Math.Max(cached, count);
            }

            countByStep[steps]++;

            if (steps == goal)
            {
                count++;
                memo.Add(memoKey, count);
                return count;
            }

            foreach (var neighbor in GetNeighbors(board, current.row, current.column, current.realRow, current.realColumn))
            {
                count = Walk(board, neighbor, steps + 1, count, goal, memo, countByStep);
            }

            memo.Add(memoKey, count);

            return count;
        }

        IEnumerable<(int row, int column, int realRow, int realColumn)> GetNeighbors(char[,] board, int row, int column, int realRow, int realColumn)
        {
            const char plot = '.';
            var maxRow = board.GetLength(0);
            var maxColumn = board.GetLength(0);
            var rowPlus = row + 1;
            var rowMinus = row - 1;
            var columnPlus = column + 1;
            var columnMinus = column - 1;

            if (rowPlus >= maxRow)
            {
                yield return (0, column, realRow + 1, realColumn);
            }
            else if (board[rowPlus, column] == plot)
            {
                yield return (rowPlus, column, realRow + 1, realColumn);
            }

            if (rowMinus < 0)
            {
                yield return (maxRow - 1, column, realRow - 1, realColumn);
            }
            else if (board[rowMinus, column] == plot)
            {
                yield return (rowMinus, column, realRow - 1, realColumn);
            }

            if (columnPlus >= maxColumn)
            {
                yield return (row, 0, realRow, realColumn + 1);
            }
            else if (board[row, columnPlus] == plot)
            {
                yield return (row, columnPlus, realRow, realColumn + 1);
            }

            if (columnMinus < 0)
            {
                yield return (row, maxColumn - 1, realRow, realColumn - 1);
            }
            else if (board[row, columnMinus] == plot)
            {
                yield return (row, columnMinus, realRow, realColumn - 1);
            }
        }
    
        long CalculateQuadratic(long input)
        {
            double a = 15181.0 / 17161.0;
            double b = 30901.0 / 17161.0;
            double c = -95601.0 / 17161.0;

            // Quadratic coefficients calculated based on steps 65, 196, 327 (full cycles of the input)

            return (long)Math.Round(a * Math.Pow(input, 2) + b * input + c);
        }
    }
}