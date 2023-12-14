internal static class Day14
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day14/Input.txt").ToArray();
        var boardPart1 = new char[lines.Length, lines[0].Length];
        var boardPart2 = new char[lines.Length, lines[0].Length];

        for (int row = 0; row < boardPart1.GetLength(0); row++)
        {
            for (int column = 0; column < boardPart1.GetLength(1); column++)
            {
                boardPart1[row, column] = lines[row][column];
                boardPart2[row, column] = lines[row][column];
            }
        }

        RollRocksNorth(boardPart1);

        var boardHashes = new List<int>();

        for (int index = 0; true; index++)
        {
            boardPart2 = Cycle(boardPart2);

            var hash = GetBoardHashCode(boardPart2);
            var indexOfHash = boardHashes.IndexOf(hash);

            if (indexOfHash > -1)
            {
                var loopSize = index - indexOfHash;

                for (int i = 0; i < (1_000_000_000 - index - 1) % loopSize; i++)
                {
                    boardPart2 = Cycle(boardPart2);
                }

                break;
            }

            boardHashes.Add(hash);
        }

        Console.WriteLine($"Day 14, Part 1: {CalculateLoad(lines, boardPart1)}");
        Console.WriteLine($"Day 14, Part 2: {CalculateLoad(lines, boardPart2)}");

        void RollRocksNorth(char[,] board)
        {
            var rows = board.GetLength(0);
            var columns = board.GetLength(1);

            for (int row = 1; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (board[row, column] == 'O' && board[row - 1, column] == '.')
                    {
                        for (int topRow = row - 1; topRow >= 0; topRow--)
                        {
                            if (topRow == 0)
                            {
                                if (board[topRow, column] == '.')
                                {
                                    board[topRow, column] = 'O';
                                    break;
                                }

                                board[topRow + 1, column] = 'O';
                                break;
                            }

                            if (board[topRow, column] != '.')
                            {
                                board[topRow + 1, column] = 'O';
                                break;
                            }
                        }

                        board[row, column] = '.';
                    }
                }
            }
        }

        char[,] RotateClockwise(char[,] board)
        {
            var rows = board.GetLength(0);
            var columns = board.GetLength(1);
            var rotatedMatrix = new char[columns, rows];

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    rotatedMatrix[column, rows - row - 1] = board[row, column];
                }
            }

            return rotatedMatrix;
        }

        int CalculateLoad(string[] lines, char[,] board)
        {
            var sum = 0;

            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int column = 0; column < board.GetLength(1); column++)
                {
                    if (board[row, column] == 'O')
                    {
                        sum += lines.Length - row;
                    }
                }
            }

            return sum;
        }

        char[,] Cycle(char[,] board)
        {
            RollRocksNorth(board); // North
            board = RotateClockwise(board); // West
            RollRocksNorth(board);
            board = RotateClockwise(board); // South
            RollRocksNorth(board);
            board = RotateClockwise(board); // East
            RollRocksNorth(board);
            board = RotateClockwise(board); // North

            return board;
        }

        int GetBoardHashCode(char[,] board)
        {
            var rows = board.GetLength(0);
            var columns = board.GetLength(1);
            var hash = 0;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (board[row, column] == 'O')
                    {
                        hash += row.GetHashCode() * 100 + column.GetHashCode();
                    }
                }
            }

            return hash;
        }
    }
}