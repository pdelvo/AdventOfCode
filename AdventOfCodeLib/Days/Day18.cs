
using System;
using System.Globalization;
using System.Numerics;

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
            return Compute(false).ToString();
        }

        public override string RunPart2()
        {
            return "";
            return Compute(true).ToString();
        }

        private string Compute(bool isPart2)
        {
            checked
            {
                List<(int x, int y, Turn turn)> points = [(0, 0, Turn.None)];
                LinkedList<Instruction> instructions = new();
                BigInteger insideCounter = 0;

                for (int i = 0; i < Lines.Length; i++)
                {
                    Instruction instruction = ParseInstruction(Lines[i], isPart2);
                    instructions.AddLast(instruction);
                }

                var instruction0Node = instructions.First!;

                for (int i = 0; ; i++)
                {
                    if (instructions.Count == 4)
                    {
                        insideCounter += ((BigInteger)instructions.First!.Value.NumberOfSteps + 1) * ((BigInteger)instructions.First.Next!.Value.NumberOfSteps + 1);
                        break;
                    }

                    var instruction1Node = GetNextCycle(instruction0Node);
                    var instruction2Node = GetNextCycle(instruction1Node);
                    var instruction3Node = GetNextCycle(instruction2Node);
                    var instruction4Node = GetNextCycle(instruction3Node);
                    Instruction instruction0 = instruction0Node.Value;
                    Instruction instruction1 = instruction1Node.Value;
                    Instruction instruction2 = instruction2Node.Value;
                    Instruction instruction3 = instruction3Node.Value;
                    Instruction instruction4 = instruction4Node.Value;

                    var turn0 = GetTurnType(instruction0, instruction1);
                    var turn1 = GetTurnType(instruction1, instruction2);
                    var turn2 = GetTurnType(instruction2, instruction3);
                    var turn3 = GetTurnType(instruction3, instruction4);

                    if (turn0 == turn1 && turn1 == turn2 && turn2 == turn3)
                    {
                        // Scary case. Don't want to. Find something else
                        continue;
                    }

                    if (turn1 == turn2)
                    {
                        long shorterSide;
                        if (instruction1.NumberOfSteps == instruction3.NumberOfSteps)
                        {
                            shorterSide = instruction1.NumberOfSteps;
                            instruction0Node.ValueRef = instruction0 with { NumberOfSteps = instruction0.NumberOfSteps + instruction2.NumberOfSteps + instruction4.NumberOfSteps };

                            instructions.Remove(instruction1Node);
                            instructions.Remove(instruction2Node);
                            instructions.Remove(instruction3Node);
                            instructions.Remove(instruction4Node);
                            if (instruction2.Direction != instruction0.Direction)
                            {
                                insideCounter -= instruction0.NumberOfSteps;
                            }
                            if (instruction2.Direction != instruction4.Direction)
                            {
                                insideCounter -= instruction4.NumberOfSteps;
                            }
                        }
                        else if (instruction1.NumberOfSteps < instruction3.NumberOfSteps)
                        {
                            shorterSide = instruction1.NumberOfSteps;

                            instruction0Node.ValueRef = instruction0 with
                            {
                                Direction = instruction2.Direction,
                                NumberOfSteps = instruction2.NumberOfSteps + (instruction2.Direction == instruction0.Direction ? instruction0.NumberOfSteps : -instruction0.NumberOfSteps)
                            };

                            instructions.Remove(instruction1Node);
                            instructions.Remove(instruction2Node);

                            instruction3Node.ValueRef = instruction3 with { NumberOfSteps = instruction3.NumberOfSteps - instruction1.NumberOfSteps };


                            if (instruction2.Direction != instruction0.Direction)
                            {
                                insideCounter += instruction0.NumberOfSteps;
                            }
                        }
                        else
                        {
                            shorterSide = instruction3.NumberOfSteps;
                            instruction4Node.ValueRef = instruction4 with
                            {
                                Direction = instruction2.Direction,
                                NumberOfSteps = instruction2.NumberOfSteps + (instruction2.Direction == instruction4.Direction ? instruction4.NumberOfSteps : -instruction4.NumberOfSteps)
                            };

                            instructions.Remove(instruction3Node);
                            instructions.Remove(instruction2Node);

                            instruction1Node.ValueRef = instruction1 with { NumberOfSteps = instruction1.NumberOfSteps - instruction3.NumberOfSteps };


                            if (instruction2.Direction != instruction4.Direction)
                            {
                                insideCounter += instruction4.NumberOfSteps;
                            }
                        }

                        if (turn1 == Turn.Left)
                        {
                            insideCounter -= (BigInteger)shorterSide * ((BigInteger)instruction2.NumberOfSteps - 1);
                        }
                        else
                        {
                            insideCounter += (BigInteger)shorterSide * ((BigInteger)instruction2.NumberOfSteps + 1);
                        }
                    }
                    else
                    {
                        instruction0Node = instruction1Node;
                    }

                    // Debug:
                    var fillSolution = FillAlgorithm(instructions) + insideCounter;


                }

                return insideCounter.ToString();
            }
        }

        private int FillAlgorithm(LinkedList<Instruction> instructions)
        {
            int width = 1000;
            int height = 1000;
            (int x, int y) start = (500, 500);
            (int currentX, int currentY) = start;

            TileType[][] tiles = new TileType[height][];

            for (int i = 0; i < height; i++)
            {
                tiles[i] = new TileType[width];
            }

            tiles[currentY][currentX] = TileType.Border;

            foreach (Instruction instruction in instructions)
            {

                var (moveX, moveY) = instruction.GetOffset();

                for (int step = 1; step <= instruction.NumberOfSteps; step++)
                {
                    (currentX, currentY) = (currentX + moveX, currentY + moveY);

                    tiles[currentY][currentX] = TileType.Border;
                }
            }

            if (currentX != start.x || currentY != start.y)
            {
                throw new InvalidOperationException("Something is wrong here, not a cycle");
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

            return insideCounter;
        }

        private static bool IsValid(int width, int height, (int x, int y) next) => next.x >= 0 && next.y >= 0 && next.x < width && next.y < height;


        private static LinkedListNode<T> GetNextCycle<T>(LinkedListNode<T> node)
        {
            var list = node.List!;
            if (list.Last == node)
            {
                return list.First!;
            }

            return node.Next!;
        }

        private Turn GetTurnType(Instruction instruction1, Instruction instruction2)
        {
            if (instruction1.Direction == Direction.None || instruction2.Direction == Direction.None)
            {
                return Turn.None;
            }

            if (((int)instruction1.Direction + 1) % 4 == (int)instruction2.Direction)
            {
                return Turn.Right;
            }

            if ((int)instruction1.Direction == ((int)instruction2.Direction + 1) % 4)
            {
                return Turn.Left;
            }

            return Turn.None;
        }

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
                int steps = int.Parse(line[^7..^2], NumberStyles.HexNumber);
                var direction = Instruction.DirectionFromChar(line[^2]);

                return new Instruction(direction, steps);
            }
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

        enum TileType
        {
            Unknown,
            Border,
            Outside,
            Inside
        }
    }
}