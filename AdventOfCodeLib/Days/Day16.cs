namespace AdventOfCodeLib.Days
{
    public class Day16 : Day
    {
        public override string? Description => "The Floor Will Be Lava";
        public override string TestInput1 => @".|...\....
|.-.\.....
.....|-...
........|.
..........
.........\
..../.\\..
.-.-/..|..
.|....-|.\
..//.|....";
        public override string TestOutput1 => "46";

        public override string TestOutput2 => "51";
        public override string RunPart1()
        {
            var startConfiguration = (0, 0, Direction.Right);

            int result = CastRay(startConfiguration);

            return result.ToString();
        }

        public override string RunPart2()
        {
            int max = 0;
            for (int i = 0; i < Lines[0].Length; i++)
            {
                var startConfiguration = (i, 0, Direction.Down);

                int result = CastRay(startConfiguration);

                if (result > max)
                {
                    max = result;
                }

                startConfiguration = (i, Lines.Length - 1, Direction.Up);

                result = CastRay(startConfiguration);

                if (result > max)
                {
                    max = result;
                }
            }

            for (int i = 0; i < Lines.Length; i++)
            {
                var startConfiguration = (0, i, Direction.Right);

                int result = CastRay(startConfiguration);

                if (result > max)
                {
                    max = result;
                }

                startConfiguration = (Lines.Length - 1, i, Direction.Left);

                result = CastRay(startConfiguration);

                if (result > max)
                {
                    max = result;
                }
            }

            return max.ToString();
        }

        private int CastRay((int, int, Direction Right) startConfiguration)
        {
            bool[][] visited = new bool[Lines.Length][];

            for (int i = 0; i < visited.Length; i++)
            {
                visited[i] = new bool[Lines[0].Length];
            }

            Queue<(int x, int y, Direction direction)> toVisit = new Queue<(int x, int y, Direction direction)>();
            toVisit.Enqueue(startConfiguration);

            while (toVisit.TryDequeue(out var next))
            {
                var (x, y, direction) = next;
                bool skipPath = false;

                while (true)
                {
                    if (!IsInside(x, y))
                    {
                        break;
                    }
                    bool wasVisitedBefore = visited[y][x];

                    visited[y][x] = true;

                    char c = Lines[y][x];
                    switch (c)
                    {
                        case '.':
                            var (nextX, nextY) = GetNext(x, y, direction);
                            (x, y) = (nextX, nextY);
                            break;
                        case '/':
                        case '\\':
                            (x, y, direction) = GetMirroredPosition(x, y, direction, c);
                            break;
                        case '|':
                        case '-':
                            if (wasVisitedBefore)
                            {
                                skipPath = true;
                                break;
                            }

                            if (IsSameDirection(direction, c))
                            {
                                (x, y) = GetNext(x, y, direction);
                            }
                            else
                            {
                                var leftTurn = GetLeftTurn(x, y, direction);
                                var rightTurn = GetRightTurn(x, y, direction);

                                if (IsInside(leftTurn.x, leftTurn.y))
                                {
                                    toVisit.Enqueue(leftTurn);
                                }

                                (x, y, direction) = rightTurn;
                            }
                            break;
                        default:
                            break;
                    }

                    if (skipPath)
                    {
                        break;
                    }
                }
            }

            int result = 0;
            //string visualization = "";

            for (int i = 0; i < visited.Length; i++)
            {
                for (int j = 0; j < visited[i].Length; j++)
                {
                    if (visited[i][j])
                    {
                        //visualization += '#';
                        result++;
                    }
                    else
                    {
                        //visualization += '.';
                    }
                }
                //visualization += "\r\n";
            }

            return result;
        }

        private (int x, int y, Direction direction) GetMirroredPosition(int nextX, int nextY, Direction direction, char mirror)
        {
            switch (direction)
            {
                case Direction.Up:
                    if (mirror == '/')
                    {
                        return (nextX + 1, nextY, Direction.Right);
                    }
                    else
                    {
                        return (nextX - 1, nextY, Direction.Left);
                    }
                case Direction.Down:
                    if (mirror == '/')
                    {
                        return (nextX - 1, nextY, Direction.Left);
                    }
                    else
                    {
                        return (nextX + 1, nextY, Direction.Right);
                    }
                case Direction.Left:
                    if (mirror == '/')
                    {
                        return (nextX, nextY + 1, Direction.Down);
                    }
                    else
                    {
                        return (nextX, nextY - 1, Direction.Up);
                    }
                case Direction.Right:
                    if (mirror == '/')
                    {
                        return (nextX, nextY - 1, Direction.Up);
                    }
                    else
                    {
                        return (nextX, nextY + 1, Direction.Down);
                    }
                default:
                    return (-1, -1, default(Direction));
            }
        }

        private (int x, int y, Direction direction) GetLeftTurn(int nextX, int nextY, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return (nextX - 1, nextY, Direction.Left);
                case Direction.Down:
                    return (nextX + 1, nextY, Direction.Right);
                case Direction.Left:
                    return (nextX, nextY + 1, Direction.Down);
                case Direction.Right:
                    return (nextX, nextY - 1, Direction.Up);
                default:
                    return (-1, -1, default(Direction));
            }
        }

        private (int x, int y, Direction direction) GetRightTurn(int nextX, int nextY, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return (nextX + 1, nextY, Direction.Right);
                case Direction.Down:
                    return (nextX - 1, nextY, Direction.Left);
                case Direction.Left:
                    return (nextX, nextY - 1, Direction.Up);
                case Direction.Right:
                    return (nextX, nextY + 1, Direction.Down);
                default:
                    return (-1, -1, default(Direction));
            }
        }

        private bool IsSameDirection(Direction direction, char c)
        {
            if (direction == Direction.Up || direction == Direction.Down)
            {
                return c == '|';
            }
            else
            {
                return c == '-';
            }
        }

        private bool IsInside(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Lines[0].Length && y < Lines.Length;
        }

        private (int nextX, int nextY) GetNext(int x, int y, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return (x, y - 1);
                case Direction.Down:
                    return (x, y + 1);
                case Direction.Left:
                    return (x - 1, y);
                case Direction.Right:
                    return (x + 1, y);
            }
            return (-1, -1);
        }

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}