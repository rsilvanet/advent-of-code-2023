internal static class Day23
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day23/Input.txt").ToArray();
        var board = new char[lines.Length, lines[0].Length];

        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int column = 0; column < board.GetLength(1); column++)
            {
                board[row, column] = lines[row][column];
            }
        }

        Console.WriteLine($"Day 23, Part 1: {FindLongestPath(board, true)}");
        Console.WriteLine($"Day 23, Part 2: {FindLongestPath(board, false)}");

        int FindLongestPath(char[,] board, bool withSlopes)
        {
            var start = (row: 0, column: 1);
            var end = (row: board.GetLength(0) - 1, column: board.GetLength(1) - 2);
            var visited = new HashSet<(int row, int column)>();

            int DepthFirstSearch((int row, int column) position, int steps)
            {
                if (position == end)
                {
                    return steps;
                }

                visited.Add(position);

                var neighbors = withSlopes ?
                    GetNeighborsWithSlopes(board, position.row, position.column) :
                    GetNeighborsWithoutSlopes(board, position.row, position.column);

                int maxSteps = 0;

                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        maxSteps = Math.Max(maxSteps, DepthFirstSearch(neighbor, steps + 1));
                    }
                }

                visited.Remove(position);

                return maxSteps;
            }

            return DepthFirstSearch(start, 0);
        }

        IEnumerable<(int row, int column)> GetNeighborsWithSlopes(char[,] board, int row, int column)
        {
            (int row, int column) down = (row + 1, column);
            (int row, int column) up = (row - 1, column);
            (int row, int column) right = (row, column + 1);
            (int row, int column) left = (row, column - 1);

            if (board[row, column] == '.')
            {
                if (board[down.row, down.column] is not '#' and not '^')
                {
                    yield return down;
                }

                if (up.row >= 0 && board[up.row, up.column] is not '#' and not 'v')
                {
                    yield return up;
                }

                if (board[right.row, right.column] is not '#' and not '<')
                {
                    yield return right;
                }

                if (board[left.row, left.column] is not '#' and not '>')
                {
                    yield return left;
                }
            }
            else if (board[row, column] == 'v')
            {
                yield return down;
            }
            else if (board[row, column] == '^')
            {
                yield return up;
            }
            else if (board[row, column] == '<')
            {
                yield return left;
            }
            else if (board[row, column] == '>')
            {
                yield return right;
            }
        }

        IEnumerable<(int row, int column)> GetNeighborsWithoutSlopes(char[,] board, int row, int column)
        {
            (int row, int column) down = (row + 1, column);
            (int row, int column) up = (row - 1, column);
            (int row, int column) right = (row, column + 1);
            (int row, int column) left = (row, column - 1);

            if (board[down.row, down.column] is not '#')
            {
                yield return down;
            }

            if (up.row >= 0 && board[up.row, up.column] is not '#')
            {
                yield return up;
            }

            if (board[right.row, right.column] is not '#')
            {
                yield return right;
            }

            if (board[left.row, left.column] is not '#')
            {
                yield return left;
            }
        }
    }
}