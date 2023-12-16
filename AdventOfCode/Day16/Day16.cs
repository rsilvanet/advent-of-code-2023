internal static class Day16
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day16/Input.txt").ToArray();
        var board = new char[lines.Length, lines[0].Length];
        var boarders = new List<Beam>();

        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int column = 0; column < board.GetLength(1); column++)
            {
                board[row, column] = lines[row][column];

                if (row == 0)
                {
                    boarders.Add(new Beam(row, column, BeamDirection.Down));
                }
                else if (row == board.GetLength(0) - 1)
                {
                    boarders.Add(new Beam(row, column, BeamDirection.Up));
                }
                else if (column == 0)
                {
                    boarders.Add(new Beam(row, column, BeamDirection.Right));
                }
                else if (row == board.GetLength(1) - 1)
                {
                    boarders.Add(new Beam(row, column, BeamDirection.Left));
                }
            }
        }

        Console.WriteLine($"Day 16, Part 1: {CalculateEnergy(board, new Beam(0, 0, BeamDirection.Right))}");
        Console.WriteLine($"Day 16, Part 2: {boarders.AsParallel().Max(beam => CalculateEnergy(board, beam))}");

        int CalculateEnergy(char[,] board, Beam initial)
        {
            var beams = new List<Beam>();
            var visited = new HashSet<(int row, int column, BeamDirection direction)>();

            beams.Add(initial);

            while (beams.Any(x => !x.Finished))
            {
                for (int i = 0; i < beams.Count; i++)
                {
                    var beam = beams[i];

                    if (beam.Row < 0 || beam.Row >= board.GetLength(0) || beam.Column < 0 || beam.Column >= board.GetLength(1))
                    {
                        beam.Finish();
                        continue;
                    }

                    if (visited.Contains((beam.Row, beam.Column, beam.Direction)))
                    {
                        beam.Finish();
                        continue;
                    }

                    visited.Add((beam.Row, beam.Column, beam.Direction));

                    Action move;

                    if (beam.Direction == BeamDirection.Right)
                    {
                        move = board[beam.Row, beam.Column] switch
                        {
                            '/' => () => beam.Up(),
                            '\\' => () => beam.Down(),
                            '|' => () => beams.Add(beam.SplitUpDown()),
                            _ => () => beam.Walk()
                        };
                    }
                    else if (beam.Direction == BeamDirection.Left)
                    {
                        move = board[beam.Row, beam.Column] switch
                        {
                            '/' => () => beam.Down(),
                            '\\' => () => beam.Up(),
                            '|' => () => beams.Add(beam.SplitUpDown()),
                            _ => () => beam.Walk()
                        };
                    }
                    else if (beam.Direction == BeamDirection.Up)
                    {
                        move = board[beam.Row, beam.Column] switch
                        {
                            '/' => () => beam.Right(),
                            '\\' => () => beam.Left(),
                            '-' => () => beams.Add(beam.SplitLeftRight()),
                            _ => () => beam.Walk()
                        };
                    }
                    else if (beam.Direction == BeamDirection.Down)
                    {
                        move = board[beam.Row, beam.Column] switch
                        {
                            '/' => () => beam.Left(),
                            '\\' => () => beam.Right(),
                            '-' => () => beams.Add(beam.SplitLeftRight()),
                            _ => () => beam.Walk()
                        };
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    
                    move();

                }
            }

            return visited.DistinctBy(x => new { x.row, x.column }).Count();
        }
    }

    class Beam
    {
        public Beam(int row, int column, BeamDirection direction)
        {
            Row = row;
            Column = column;
            Direction = direction;
            Finished = false;
        }

        public Beam(Beam beam) : this(beam.Row, beam.Column, beam.Direction) { }

        public int Row { get; private set; }
        public int Column { get; private set; }
        public BeamDirection Direction { get; private set; }
        public bool Finished { get; private set; }

        public void Walk()
        {
            switch (Direction)
            {
                case BeamDirection.Up: 
                    Row--;
                    break;
                case BeamDirection.Down:
                    Row++;
                    break;
                case BeamDirection.Left:
                    Column--;
                    break;
                case BeamDirection.Right:
                    Column++;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public Beam Up()
        {
            Row--;
            Direction = BeamDirection.Up;
            return this;
        }

        public Beam Down()
        {
            Row++;
            Direction = BeamDirection.Down;
            return this;
        }

        public Beam Left()
        {
            Column--;
            Direction = BeamDirection.Left;
            return this;
        }

        public Beam Right()
        {
            Column++;
            Direction = BeamDirection.Right;
            return this;
        }

        public Beam Copy()
        {
            return new Beam(this);
        }

        public Beam SplitLeftRight()
        {
            var newBeam = Copy();
            newBeam.Right();
            Left();
            return newBeam;
        }

        public Beam SplitUpDown()
        {
            var newBeam = Copy();
            newBeam.Up();
            Down();
            return newBeam;
        }

        public void Finish()
        {
            Finished = true;
        }
    }

    enum BeamDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
