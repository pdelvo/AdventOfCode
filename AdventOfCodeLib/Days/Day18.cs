
using System.Globalization;

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
            List<(int x, int y, Turn turn)> points = [(0, 0, Turn.None)];
            List<Instruction> instructions = new();
            long insideCounter = 0;

            for (int i = 0; i < Lines.Length; i++)
            {
                Instruction instruction = ParseInstruction(Lines[i]);

                var (moveX, moveY) = instruction.GetOffset();

                var currentPoint = points[i];

                (int x, int y) nextPoint = (currentPoint.x + moveX * instruction.NumberOfSteps, currentPoint.y + moveY * instruction.NumberOfSteps);

                if (instructions.Count > 1)
                {
                    var turn1 = GetTurnType(instructions[i - 2], instructions[i - 1]);
                    var turn2 = GetTurnType(instructions[i - 1], instruction);

                    if (turn1 == turn2)
                    {

                    }
                }
                instructions.Add(instruction);
                points.Add((nextPoint.x, nextPoint.y, );
            }

            return insideCounter.ToString();
        }

        private Turn GetTurnType(Instruction instruction1, Instruction instruction2)
        {
            if (instruction1.Direction == Direction.None || instruction2.Direction == Direction.None)
            {
                return Turn.None;
            }

            if (instruction1.Direction + 1 % 4 == instruction2.Direction)
            {
                return Turn.Right;
            }

            if (instruction1.Direction == instruction2.Direction + 1 % 4)
            {
                return Turn.Right;
            }

            return Turn.None;
        }

        private static bool IsValid(int width, int height, (int x, int y) next) => next.x >= 0 && next.y >= 0 && next.x < width && next.y < height;

        private Instruction ParseInstruction(ReadOnlySpan<char> line, bool part2 = false)
        {
            if (!part2)
            {
                var direction = Instruction.DirectionFromChar(line[0]);
                int numberEnd = line[3..].IndexOf(' ');
                int steps = int.Parse(line[2..(2 + numberEnd + 1)]);

                return new Instruction(direction, steps);
            }
            else
            {
                int steps = int.Parse(line[^6..^1], NumberStyles.HexNumber);
                var direction = Instruction.DirectionFromChar(line[^0]);

                return new Instruction(direction, steps);
            }
        }

        public override string RunPart2()
        {
            return "";
        }

        record struct Instruction (Direction Direction, int NumberOfSteps)
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
                    case '0':
                    case 'R':
                        return Direction.Right;
                    case '1':
                    case 'D':
                        return Direction.Down;
                    case '2':
                    case 'L':
                        return Direction.Left;
                    case 'U':
                    case '3':
                        return Direction.Up;
                    default:
                        return Direction.None;
                }
            }
        }

        enum Direction : int
        {
            None = 5,
            Right = 0,
            Down = 1,
            Left = 2,
            Up = 3,
        }

        enum Turn
        {
            Left,
            Right,
            None
        }
    }
}