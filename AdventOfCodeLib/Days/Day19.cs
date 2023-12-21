using System.Numerics;

namespace AdventOfCodeLib.Days
{
    public class Day19 : Day
    {
        public override string? Description => "Aplenty";
        public override string TestInput1 => @"px{a<2006:qkq,m>2090:A,rfg}
pv{a>1716:R,A}
lnx{m>1548:A,A}
rfg{s<537:gd,x>2440:R,A}
qs{s>3448:A,lnx}
qkq{x<1416:A,crn}
crn{x>2662:A,R}
in{s<1351:px,qqz}
qqz{s>2770:qs,m<1801:hdj,R}
gd{a>3333:R,R}
hdj{m>838:A,pv}

{x=787,m=2655,a=1222,s=2876}
{x=1679,m=44,a=2067,s=496}
{x=2036,m=264,a=79,s=2244}
{x=2461,m=1339,a=466,s=291}
{x=2127,m=1623,a=2188,s=1013}";
        public override string TestOutput1 => "19114";

        public override string TestOutput2 => "167409079868000";
        public override string RunPart1()
        {
            var (workflows, position) = ParseRules();
            var parts = ParseParts(position + 1);

            long sum = 0;

            for ( var i = 0; i < parts.Count; i++)
            {
                var currentWorkflow = NameToInt("in");
                Part part = parts[i];

                while (true)
                {
                    if (currentWorkflow == 'A')
                    {
                        sum += part.coolLooking + part.musical + part.aerodynamic + part.shiny;
                        break;
                    }
                    else if (currentWorkflow == 'R')
                    {
                        break;
                    }
                    else
                    {
                        var workflow = workflows[currentWorkflow];
                        currentWorkflow = workflow.Apply(part);
                    }
                }
            }

            return sum.ToString();
        }

        public override string RunPart2()
        {
            var (workflows, position) = ParseRules();

            HashSet<int> finalWorkflows = ['A', 'R'];
            List<int> nonFinalWorkflows = [];
            Dictionary<int, List<WorkflowRange>> acceptRanges = new();
            acceptRanges['A'] = [(new Range(1, 4001), new Range(1, 4001), new Range(1, 4001), new Range(1, 4001))];
            acceptRanges['R'] = [];

            foreach (int name in workflows.Keys)
            {
                nonFinalWorkflows.Add(name);
            }


            while (nonFinalWorkflows.Count > 0)
            {
                for (int i = 0; i < nonFinalWorkflows.Count; i++)
                {
                    var workflowId = nonFinalWorkflows[i];

                    Workflow workflow = workflows[workflowId];
                    if (workflow.IsFinal(finalWorkflows))
                    {
                        finalWorkflows.Add(workflowId);
                        nonFinalWorkflows.RemoveAt(i);
                        List<WorkflowRange> workflowAcceptRanges = new();
                        List<WorkflowRule> additionalRules = new();

                        for (int j = 0; j <= workflow.rules.Count; j++)
                        {
                            WorkflowRule rule;
                            if (j < workflow.rules.Count)
                            {
                                rule = workflow.rules[j];
                            }
                            else
                            {
                                rule = new WorkflowRule('x', ComparisonType.Larger, -1000, workflow.defaultJump);
                            }
                            var pointedWorkflowAcceptRanges = acceptRanges[rule.jumpTo]!;

                            var newAcceptRanges = GetNewAcceptRanges(rule, pointedWorkflowAcceptRanges);
                            foreach (var additionalRule in additionalRules)
                            {
                                newAcceptRanges = GetNewAcceptRanges(additionalRule, newAcceptRanges);
                            }

                            workflowAcceptRanges.AddRange(newAcceptRanges);

                            additionalRules.Add(rule.GetOpposite());
                        }



                        acceptRanges.Add(workflowId, workflowAcceptRanges);
                        break;
                    }
                }
            }

            var startAcceptRanges = acceptRanges[NameToInt("in")];

            BigInteger sum = 0;

            for (int i = 0; i < startAcceptRanges.Count; i++)
            {
                var range = startAcceptRanges[i];
                sum += (BigInteger)(range.xRange.End.Value - range.xRange.Start.Value)
                     * (BigInteger)(range.mRange.End.Value - range.mRange.Start.Value)
                     * (BigInteger)(range.aRange.End.Value - range.aRange.Start.Value)
                     * (BigInteger)(range.sRange.End.Value - range.sRange.Start.Value);
            }

            return sum.ToString();
        }

        private List<WorkflowRange> GetNewAcceptRanges(WorkflowRule rule, List<WorkflowRange> pointedWorkflowRanges)
        {
            List<WorkflowRange> newAcceptRanges = new();
            var comparisonType = rule.comparisonType;

            foreach (var (xRange, mRange, aRange, sRange) in pointedWorkflowRanges)
            {
                switch (rule.type)
                {
                    case 'x':
                        var cutRange = CutRange(xRange, comparisonType, rule.threshold);
                        if (cutRange == null)
                        {
                            continue;
                        }
                        newAcceptRanges.Add((cutRange.Value, mRange, aRange, sRange));
                        break;
                    case 'm':
                        cutRange = CutRange(mRange, comparisonType, rule.threshold);
                        if (cutRange == null)
                        {
                            continue;
                        }
                        newAcceptRanges.Add((xRange, cutRange.Value, aRange, sRange));
                        break;
                    case 'a':
                        cutRange = CutRange(aRange, comparisonType, rule.threshold);
                        if (cutRange == null)
                        {
                            continue;
                        }
                        newAcceptRanges.Add((xRange, mRange, cutRange.Value, sRange));
                        break;
                    case 's':
                        cutRange = CutRange(sRange, comparisonType, rule.threshold);
                        if (cutRange == null)
                        {
                            continue;
                        }
                        newAcceptRanges.Add((xRange, mRange, aRange, cutRange.Value));
                        break;
                    default:
                        throw new InvalidOperationException("Found unknown type");
                }
            }

            return newAcceptRanges;
        }

        private Range? CutRange(Range range, ComparisonType comparisonType, int threshold)
        {
            if (comparisonType == ComparisonType.Smaller)
            {
                if (range.Start.Value >= threshold)
                {
                    return null;
                }
                else
                {
                    return new Range(range.Start, Math.Min(range.End.Value, threshold));
                }
            }
            else
            {
                if (range.End.Value <= threshold)
                {
                    return null;
                }
                else
                {
                    return new Range(Math.Max(range.Start.Value, threshold + 1), range.End);
                }
            }
        }

        private List<Part> ParseParts(int startLine)
        {
            var parts = new List<Part>();

            for (int i = startLine; i < Lines.Length; i++)
            {
                var line = Lines[i].AsSpan()[3..];

                int commaPosition = line.IndexOf(',');
                int x = int.Parse(line[0..commaPosition]);
                line = line[(commaPosition + 3)..];
                commaPosition = line.IndexOf(',');
                int m = int.Parse(line[0..commaPosition]);
                line = line[(commaPosition + 3)..];
                commaPosition = line.IndexOf(',');
                int a = int.Parse(line[0..commaPosition]);
                line = line[(commaPosition + 3)..];
                int s = int.Parse(line[..^1]);

                parts.Add(new Part(x, m, a, s));
            }
            return parts;
        }

        private (Dictionary<int, Workflow> workflows, int position) ParseRules()
        {
            Dictionary<int, Workflow> workflows = new Dictionary<int, Workflow>();

            for (int i = 0; i < Lines.Length; i++)
            {
                var line = Lines[i].AsSpan();

                if (line.Length == 0)
                {
                    return (workflows, i);
                }

                var parenthesisOpenPosition = line.IndexOf('{');

                var workflowName = line[0..parenthesisOpenPosition];

                int nextRulePosition = parenthesisOpenPosition + 1;
                List<WorkflowRule> rules = new();
                int defaultJump = 0;

                while (nextRulePosition < line.Length)
                {
                    var remainingText = line[nextRulePosition..];
                    var endPosition = remainingText.IndexOfAny(',', '}');

                    var ruleText = remainingText[0..endPosition];
                    var colonPosition = ruleText.IndexOf(':');
                    if (colonPosition == -1)
                    {
                        defaultJump = NameToInt(ruleText);
                    }
                    else
                    {
                        int jumpTo = NameToInt(ruleText[(colonPosition + 1)..]);
                        int smallerGreaterPosition = ruleText.IndexOfAny('<', '>');
                        var comparisonType = (ComparisonType)ruleText[smallerGreaterPosition];
                        var threshold = int.Parse(ruleText[(smallerGreaterPosition + 1)..colonPosition]);

                        rules.Add(new WorkflowRule(ruleText[0], comparisonType, threshold, jumpTo));
                    }

                    nextRulePosition += endPosition + 1;
                }

                workflows.Add(NameToInt(workflowName), new Workflow(NameToInt(workflowName), rules, defaultJump));
            }

            throw new InvalidOperationException("Could not find empty line");
        }

        public int NameToInt(ReadOnlySpan<char> name)
        {
            int result = 0;

            for (int i = 0; i < name.Length; i++)
            {
                result = result << 8 | name[i];
            }

            return result;
        }
    }

    record struct Workflow (int name, List<WorkflowRule> rules, int defaultJump)
    {
        public int Apply(Part part)
        {
            for (int i = 0; i < rules.Count; i++)
            {
                WorkflowRule rule = rules[i];
                if (rule.Matches(part))
                {
                    return rule.jumpTo;
                }
            }

            return defaultJump;
        }

        public bool IsFinal(HashSet<int> finalWorkflows)
        {
            for (int i = 0; i < rules.Count; i++)
            {
                WorkflowRule rule = rules[i];
                if (!finalWorkflows.Contains(rule.jumpTo))
                {
                    return false;
                }
            }
            return finalWorkflows.Contains(defaultJump);
        }
    }

    record struct WorkflowRule(char type, ComparisonType comparisonType, int threshold, int jumpTo)
    {
        public bool Matches(Part part)
        {
            switch (type)
            {
                case 'x':
                    return comparisonType == ComparisonType.Larger ? part.coolLooking > threshold : part.coolLooking < threshold;
                case 'm':
                    return comparisonType == ComparisonType.Larger ? part.musical > threshold : part.musical < threshold;
                case 'a':
                    return comparisonType == ComparisonType.Larger ? part.aerodynamic > threshold : part.aerodynamic < threshold;
                case 's':
                    return comparisonType == ComparisonType.Larger ? part.shiny > threshold : part.shiny < threshold;
                default:
                    throw new InvalidOperationException("Unknown part property");
            }
        }

        public WorkflowRule GetOpposite()
        {
            return this with
            { 
                comparisonType = comparisonType == ComparisonType.Smaller ? ComparisonType.Larger : ComparisonType.Smaller,
                threshold = comparisonType == ComparisonType.Smaller ? threshold - 1 : threshold + 1
            };
        }
    }


    record struct Part (int coolLooking, int musical, int aerodynamic, int shiny);
    enum ComparisonType : int
    {
        Smaller = '<',
        Larger = '>'
    }

    internal record struct WorkflowRange(Range xRange, Range mRange, Range aRange, Range sRange)
    {
        public static implicit operator (Range xRange, Range mRange, Range aRange, Range sRange)(WorkflowRange value)
        {
            return (value.xRange, value.mRange, value.aRange, value.sRange);
        }

        public static implicit operator WorkflowRange((Range xRange, Range mRange, Range aRange, Range sRange) value)
        {
            return new WorkflowRange(value.xRange, value.mRange, value.aRange, value.sRange);
        }
    }
}