namespace AdventOfCodeLib.Days
{
    public class Day15 : Day
    {
        public override string? Description => "Lens Library";
        public override string TestInput1 => @"rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";
        public override string TestOutput1 => "1320";

        public override string TestOutput2 => "145";
        public override string RunPart1()
        {
            int result = HashList(Lines[0]);
            return result.ToString();
        }

        public override string RunPart2()
        {
            Range[] ranges = new Range[Lines[0].Length];

            var rangeSpan = ranges[0..Lines[0].AsSpan().Split(ranges, ',')];

            LinkedList<(string label, int focalPower)>[] boxes = new LinkedList<(string label, int focalPower)>[256];
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i] = new LinkedList<(string label, int focalPower)>();
            }

            for (int i = 0; i < rangeSpan.Length; i++)
            {
                ReadOnlySpan<char> instruction = Lines[0].AsSpan()[rangeSpan[i]];

                if (instruction[instruction.Length - 1] == '-')
                {
                    var label = instruction[0..^1];
                    var hash = Hash(label);
                    var box = boxes[hash];

                    RemoveLabelIfExists(box, label);
                }
                else
                {
                    var index = instruction.IndexOf('=');

                    var label = instruction[0..index];

                    var focalLength = int.Parse(instruction[(index + 1)..]);
                    var hash = Hash(label);
                    var box = boxes[hash];

                    ReplaceLensIfExists(box, label, focalLength);
                }
            }

            long total = 0;

            for (int i = 0; i < boxes.Length; i++)
            {
                var box = boxes[i];

                long slotNumber = 1;
                foreach (var (label, focalPower) in box)
                {
                    total += (i + 1) * slotNumber * focalPower;

                    slotNumber++;
                }
            }

            return total.ToString();
        }

        private void ReplaceLensIfExists(LinkedList<(string label, int focalPower)> box, ReadOnlySpan<char> label, int focalLength)
        {
            if (box.Count == 0)
            {
                box.AddLast((label.ToString(), focalLength));
                return;
            }

            var current = box.First!;

            while (true)
            {
                if (MemoryExtensions.Equals(current.Value.label, label, StringComparison.Ordinal))
                {
                    current.ValueRef = (current.Value.label, focalLength);

                    return;
                }

                if (current == box.Last)
                {
                    box.AddLast((label.ToString(), focalLength));
                    return;
                }

                current = current.Next!;
            }
        }

        private bool RemoveLabelIfExists(LinkedList<(string label, int focalPower)> box, ReadOnlySpan<char> label)
        {
            if (box.Count == 0)
            {
                return false;
            }

            var current = box.First!;

            while (true)
            {
                if (MemoryExtensions.Equals(current.Value.label, label, StringComparison.Ordinal))
                {
                    box.Remove(current);

                    return true;
                }

                if (current == box.Last)
                {
                    return false;
                }

                current = current.Next!;
            }
        }

        private int HashList(ReadOnlySpan<char> input)
        {
            Range[] ranges = new Range[input.Length];

            var span = ranges[0..input.Split(ranges, ',')];

            int sum = 0;

            for (int i = 0; i < ranges.Length; i++)
            {
                var partialSpan = input[ranges[i]];

                sum += Hash(partialSpan);
            }

            return sum;
        }

        private byte Hash(ReadOnlySpan<char> input)
        {
            byte hash = 0;

            for (int i = 0; i < input.Length; i++)
            {
                byte ascii = (byte)input[i];

                hash += ascii;
                hash *= 17;
            }

            return hash;
        }
    }
}