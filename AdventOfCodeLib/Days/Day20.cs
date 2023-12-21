namespace AdventOfCodeLib.Days
{
    public class Day20 : Day
    {
        public override string? Description => "Pulse Propagation";
        public override string TestInput1 => @"broadcaster -> a
%a -> inv, con
&inv -> b
%b -> con
&con -> rx";
        public override string TestOutput1 => "11687500";

        public override string TestOutput2 => "0";
        public override string RunPart1()
        {
            Dictionary<int, Module> input = ParseInput();

            Queue<(int from, int to, bool value)> nextPulses = new();

            long sumLow = 0;
            long sumHigh = 0;

            for (int i = 0; i < 1000; i++)
            {
                nextPulses.Enqueue((-2, -1, false));
                sumLow++;

                while (nextPulses.Count > 0)
                {
                    var (from, to, state) = nextPulses.Dequeue();

                    if (input.TryGetValue(to, out var toModule)) {

                        var nextStateOrNull = toModule.GetPulse(from, state);

                        if (nextStateOrNull is bool nextState)
                        {
                            for (int j = 0; j < toModule.Destinations.Count; j++)
                            {
                                var destination = toModule.Destinations[j];
                                if (nextState)
                                {
                                    sumHigh++;
                                }
                                else
                                {
                                    sumLow++;
                                }

                                nextPulses.Enqueue((to, destination, nextState));
                            }
                        } 
                    }
                }
            }

            return (sumLow * sumHigh).ToString();
        }

        public override string RunPart2()
        {

            Dictionary<int, Module> input = ParseInput();

            // Skip test
            if (input.Count < 10)
            {
                return "";
            }

            Queue<(int from, int to, bool value)> nextPulses = new();

            (int start, int end)[] pairs =
                [
                    (NameToInt("xg"), NameToInt("sk")),
                    (NameToInt("fb"), NameToInt("kk")),
                    (NameToInt("dl"), NameToInt("vt")),
                    (NameToInt("gh"), NameToInt("xc")),
                ];


            long scm = 1;
            for (int i = 0; i < pairs.Length; i++)
            {
                input = ParseInput();
                var (start, end) = pairs[i];
                var result = CountUntilFirstNegative(input, start, end) + 1;
                scm = AOCMath.LeastCommonMultiple(scm, result);
            }

            return scm.ToString();
        }

        public long CountUntilFirstNegative(Dictionary<int, Module> input, int startVertex, int endVertex)
        {
            Queue<(int from, int to, bool value)> nextPulses = new();

            for (int i = 0; ; i++)
            {
                nextPulses.Enqueue((-2, startVertex, false));
                while (nextPulses.Count > 0)
                {
                    var (from, to, state) = nextPulses.Dequeue();

                    if (input.TryGetValue(to, out var toModule))
                    {
                        if (toModule.GetPulse(from, state) is bool nextState)
                        {
                            for (int j = 0; j < toModule.Destinations.Count; j++)
                            {
                                var destination = toModule.Destinations[j];
                                if (!nextState)
                                {
                                    if (destination == endVertex)
                                    {
                                        return i;
                                    }
                                }

                                nextPulses.Enqueue((to, destination, nextState));
                            }
                        }
                    }
                }
            }
        }

        public Dictionary<int, Module> ParseInput()
        {
            Dictionary<int, Module> modules = [];

            for (int i = 0; i < Lines.Length; i++)
            {
                var line = Lines[i].AsSpan();

                ModuleType type;
                switch (line[0])
                {
                    case '%':
                        line = line[1..];
                        type = ModuleType.FlipFlop;
                        break;
                    case '&':
                        line = line[1..];
                        type = ModuleType.Conjunction;
                        break;
                    default:
                        type = ModuleType.Broadcaster;
                        break;
                }

                var firstWhitespace = line.IndexOf(' ');
                var name = NameToInt(line[0..firstWhitespace]);

                line = line[(firstWhitespace + 4)..];
                List<int> destinations = [];

                while (line.Length > 0)
                {
                    int commaIndex = line.IndexOf(',');
                    if (commaIndex == -1)
                    {
                        destinations.Add(NameToInt(line));
                        break;
                    }

                    destinations.Add(NameToInt(line[0..commaIndex]));
                    line = line[(commaIndex + 2)..];
                }

                modules.Add(name, new Module(name, type, destinations));
            }

            foreach (var module in modules)
            {
                foreach (var destination in module.Value.Destinations)
                {
                    if (modules.TryGetValue(destination, out var destinationModule))
                    {
                        destinationModule.SetInput(module.Key);
                    }
                }
            }

            return modules;
        }
        public static int NameToInt(ReadOnlySpan<char> name)
        {
            if (name.Length == 11)
            {
                return -1;
            }

            int result = 0;

            for (int i = 0; i < name.Length; i++)
            {
                result = result << 8 | name[i];
            }

            return result;
        }

        public record Module(int Name, ModuleType Type, List<int> Destinations)
        {
            readonly Dictionary<int, bool> inputStates = [];
            long state = 0;
            long flipFlopFactor = 2;

            public void SetInput(int inputName)
            {
                inputStates[inputName] = false;
            }
            public void ChangeInput(int previousInputName, int newInputName)
            {
                inputStates.Remove(previousInputName);
                inputStates[newInputName] = false;
            }

            public void Combine(Module module, Dictionary<int, Module> input)
            {
                Destinations.Clear();
                Destinations.AddRange(module.Destinations);
                flipFlopFactor = flipFlopFactor * module.flipFlopFactor;
                foreach (var (_, value) in input)
                {
                    value.ChangeInput(module.Name, Name);
                }

                input.Remove(module.Name);
            }

            public bool? GetPulse(int inputName, bool inputValue)
            {
                switch (Type)
                {
                    case ModuleType.FlipFlop:
                        if (inputStates.Count > 1 && Destinations.Count > 1)
                        {

                        }

                        if (inputValue)
                        {
                            // Ignore
                            return null;
                        }
                        else
                        {
                            state = (state + 1) % flipFlopFactor;
                            return state == flipFlopFactor - 1;
                        }
                    case ModuleType.Conjunction:
                        inputStates[inputName] = inputValue;
                        foreach (var item in inputStates.Values)
                        {
                            if (!item)
                            {
                                return true;
                            }
                        }

                        return false;
                    default:
                        return inputValue;
                }
            }
        }

        public enum ModuleType
        {
            FlipFlop,
            Conjunction,
            Broadcaster
        }
    }
}