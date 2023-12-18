
namespace AdventOfCodeLib.Days
{
    public class Day18 : Day
    {
        public override string? Description => "Lavaduct Lagoon";
        public override string TestInput1 => @"R 6 (#70c710)
D 5 (#0dc571)
L 2 (#5713f0)
D 2 (#d2c081)
R 2 (#59c680)
D 2 (#411b91)
L 5 (#8ceee2)
U 2 (#caa173)
L 1 (#1b58a2)
U 2 (#caa171)
R 2 (#7807d2)
U 3 (#a77fa3)
L 2 (#015232)
U 2 (#7a21e3)";
        public override string TestOutput1 => "62";

        public override string TestOutput2 => "952408144115";
        public override string RunPart1()
        {
            int width = 1000;
            int height = 1000;
            (int currentX, int currentY) = (500, 500);

            TileType[][] tiles = new TileType[height][];

            for (int i = 0; i < height; i++)
            {
                tiles[i] = new TileType[width];
            }

            tiles[currentY][currentX] = TileType.Border;

            for (int i = 0; i < Lines.Length; i++)
            {
                Instruction instruction = ParseInstruction(Lines[i]);

                var (moveX, moveY) = instruction.GetOffset();

                for (int step = 1; step <= instruction.NumberOfSteps; step++)
                {
                    (currentX, currentY) = (currentX + moveX, currentY + moveY);

                    tiles[currentY][currentX] = TileType.Border;
                }
            }

            // Do a fill
            Queue<(int x, int y)> toCheck = new();

            for (int i = 0; i < width; i++)
            {
                toCheck.Enqueue((0, i));
                toCheck.Enqueue((width - 1, i));
            }

            for (int i = 1; i < height - 1; i++)
            {
                toCheck.Enqueue((i, 0));
                toCheck.Enqueue((i, height - 1));
            }

            while (toCheck.Count > 0)
            {
                var next = toCheck.Dequeue();

                if (IsValid(width, height, next))
                {
                    if (tiles[next.y][next.x] == TileType.Unknown)
                    {
                        tiles[next.y][next.x] = TileType.Outside;
                        toCheck.Enqueue((next.x + 1, next.y));
                        toCheck.Enqueue((next.x, next.y + 1));
                        toCheck.Enqueue((next.x - 1, next.y));
                        toCheck.Enqueue((next.x, next.y - 1));
                    }
                }
            }

            int insideCounter = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (tiles[j][i] == TileType.Unknown
                        || tiles[j][i] == TileType.Border)
                    {
                        insideCounter++;
                    }
                }
            }

            return insideCounter.ToString();
        }

        private static bool IsValid(int width, int height, (int x, int y) next) => next.x >= 0 && next.y >= 0 && next.x < width && next.y < height;

        private Instruction ParseInstruction(ReadOnlySpan<char> line)
        {
            var direction = Instruction.DirectionFromChar(line[0]);
            int numberEnd = line[3..].IndexOf(' ');
            int steps = int.Parse(line[2..(2 + numberEnd + 1)]);
            line = line[numberEnd..];

            int R = int.Parse(line[6..8], System.Globalization.NumberStyles.HexNumber);
            int G = int.Parse(line[8..10], System.Globalization.NumberStyles.HexNumber);
            int B = int.Parse(line[10..12], System.Globalization.NumberStyles.HexNumber);

            return new Instruction(direction, steps, R, G, B);
        }

        public override string RunPart2()
        {
            return "";
        }

        record struct Instruction (Direction Direction, int NumberOfSteps, int R, int G, int B)
        {
            public (int xOffset, int yOffset) GetOffset()
            {
                switch (Direction)
                {
                    case Direction.Up:
                        return (0, -1);
                    case Direction.Down:
                        return (0, 1);
                    case Direction.Left:
                        return (-1, 0);
                    case Direction.Right:
                        return (1, 0);
                    default:
                        return (0, 0);
                }
            }

            public static Direction DirectionFromChar(char c)
            {
                switch (c)
                {
                    case 'U':
                        return Direction.Up;
                    case 'D':
                        return Direction.Down;
                    case 'L':
                        return Direction.Left;
                    case 'R':
                        return Direction.Right;
                    default:
                        return Direction.None;
                }
            }
        }

        enum Direction
        {
            None,
            Up,
            Down,
            Left,
            Right
        }

        enum TileType
        {
            Unknown,
            Border,
            Outside,
            Inside
        }
    }
}