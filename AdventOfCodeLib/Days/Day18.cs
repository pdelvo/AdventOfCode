
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
            return Compute(true).ToString();
        }

        private string Compute(bool isPart2)
        {
            LinkedList<Instruction> instructions = new();
            BigInteger insideCounter = 0;

            for (int i = 0; i < Lines.Length; i++)
            {
                Instruction instruction = ParseInstruction(Lines[i], isPart2);
                instructions.AddLast(instruction);
            }

            while (instructions.Count > 4)
            {
                Console.WriteLine(instructions.Count);
                var instruction0Node = instructions.First!;

                // Find the smallest corner
                //  ------
                //  |    |
                //  |    |
                //
                int minDistance = int.MaxValue;
                LinkedListNode<Instruction>? minNode = null;
                for (int i = 0; i < instructions.Count; i++)
                {
                    var inst1Node = GetNextCycle(instruction0Node);
                    var inst2Node = GetNextCycle(inst1Node);
                    var inst3Node = GetNextCycle(inst2Node);
                    var inst4Node = GetNextCycle(inst3Node);
                    Instruction inst0 = instruction0Node.Value;
                    Instruction inst1 = inst1Node.Value;
                    Instruction inst2 = inst2Node.Value;
                    Instruction inst3 = inst3Node.Value;
                    Instruction inst4 = inst4Node.Value;

                    var t0 = GetTurnType(inst0, inst1);
                    var t1 = GetTurnType(inst1, inst2);
                    var t2 = GetTurnType(inst2, inst3);
                    var t3 = GetTurnType(inst3, inst4);

                    if (t1 == t2)
                    {
                        if (inst2.NumberOfSteps < minDistance)
                        {
                            minNode = instruction0Node;
                            minDistance = inst2.NumberOfSteps;
                        }
                    }
                    instruction0Node = inst1Node;
                }
                instruction0Node = minNode!;
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
            insideCounter += ((BigInteger)instructions.First!.Value.NumberOfSteps + 1) * ((BigInteger)instructions.First.Next!.Value.NumberOfSteps + 1);

            return insideCounter.ToString();
        }

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
    }
}