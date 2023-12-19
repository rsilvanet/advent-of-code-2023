internal static class Day19
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day19/Input.txt");
        (var parts, var workflows) = Parse(lines);

        foreach (var part in parts)
        {
            part.Process(workflows);
        }

        Console.WriteLine($"Day 19, Part 1: {parts.Where(x => x.Status == PartStatus.Accepted).Sum(x => x.Attributes.Sum(x => x.Value))}");
        Console.WriteLine($"Day 19, Part 2: {CountRanges(workflows)}");

        (List<Part> parts, Dictionary<string, List<Rule>> workflows) Parse(IEnumerable<string> lines)
        {
            var parts = new List<Part>();
            var workflows = new Dictionary<string, List<Rule>>();

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("{"))
                {
                    var part = new Part();
                    var attributes = line.Trim('{').Trim('}').Split(',');

                    foreach (var attribute in attributes)
                    {
                        var attSplit = attribute.Split('=');
                        var key = attSplit[0][0];
                        var value = int.Parse(attSplit[1]);
                        part.Attributes.Add(key, value);
                    }

                    parts.Add(part);
                    continue;
                }

                var workflowSplit = line.Trim('}').Split('{');
                var workflowName = workflowSplit[0];
                var ruleSplit = workflowSplit[1].Split(',');
                var rules = new List<Rule>();

                for (int i = 0; i < ruleSplit.Length; i++)
                {
                    if (ruleSplit[i].Contains('>'))
                    {
                        var attribute = ruleSplit[i].Split('>')[0][0];
                        var valueToCheck = int.Parse(ruleSplit[i].Split('>')[1].Split(':')[0]);
                        var result = ruleSplit[i].Split('>')[1].Split(':')[1];
                        rules.Add(new Rule(attribute, '>', valueToCheck, result));
                    }
                    else if (ruleSplit[i].Contains('<'))
                    {
                        var attribute = ruleSplit[i].Split('<')[0][0];
                        var valueToCheck = int.Parse(ruleSplit[i].Split('<')[1].Split(':')[0]);
                        var result = ruleSplit[i].Split('<')[1].Split(':')[1];
                        rules.Add(new Rule(attribute, '<', valueToCheck, result));
                    }
                    else
                    {
                        rules.Add(new Rule(ruleSplit[i]));
                    }
                }

                workflows.Add(workflowName, rules);
            }

            return (parts, workflows);
        }

        long CountRanges(Dictionary<string, List<Rule>> workflows)
        {
            var ranges = new Dictionary<char, (int min, int max)>()
            {
                { 'x', (1, 4000) },
                { 'm', (1, 4000) },
                { 'a', (1, 4000) },
                { 's', (1, 4000) }
            };

            var count = 0L;

            foreach (var rule in workflows["in"])
            {
                count = CountRangesRecursive(rule, workflows, ranges, count);
            }

            return count;
        }

        long CountRangesRecursive(Rule rule, Dictionary<string, List<Rule>> workflows, Dictionary<char, (int min, int max)> ranges, long count)
        {
            var rangesCopy = ranges.ToDictionary();

            if (rule.Operation == '>')
            {
                ranges[rule.Attribute] = (rangesCopy[rule.Attribute].min, rule.ValueToCheck);
                rangesCopy[rule.Attribute] = (rule.ValueToCheck + 1, rangesCopy[rule.Attribute].max);
            }
            else if (rule.Operation == '<')
            {
                ranges[rule.Attribute] = (rule.ValueToCheck, rangesCopy[rule.Attribute].max);
                rangesCopy[rule.Attribute] = (rangesCopy[rule.Attribute].min, rule.ValueToCheck - 1);
            }

            if (rule.Result == "A")
            {
                long x = (rangesCopy['x'].max - rangesCopy['x'].min + 1);
                long m = (rangesCopy['m'].max - rangesCopy['m'].min + 1);
                long a = (rangesCopy['a'].max - rangesCopy['a'].min + 1);
                long s = (rangesCopy['s'].max - rangesCopy['s'].min + 1);

                return count + (x * m * a * s);
            }
            else if (rule.Result == "R")
            {
                return count;
            }
            else
            {
                foreach (var nextRule in workflows[rule.Result])
                {
                    count = CountRangesRecursive(nextRule, workflows, rangesCopy, count);
                }
            }

            return count;
        }
    }

    class Rule
    {
        public Rule(string result)
        {
            IsDefault = true;
            Result = result;
        }

        public Rule(char attribute, char operation, int valueToCheck, string result)
        {
            IsDefault = false;
            Attribute = attribute;
            Operation = operation;
            ValueToCheck = valueToCheck;
            Result = result;
        }

        public bool IsDefault { get; }
        public char Attribute { get; }
        public char Operation { get; }
        public int ValueToCheck { get; }
        public string Result { get; }

        public override string ToString() => IsDefault ? Result : $"{Attribute}{Operation}{ValueToCheck}:{Result}";
    }

    class Part
    {
        public Part()
        {
            Attributes = new Dictionary<char, int>();
        }

        public Part(int x, int m, int a, int s) : this()
        {
            Attributes['x'] = x;
            Attributes['m'] = m;
            Attributes['a'] = a;
            Attributes['s'] = s;
        }

        public Dictionary<char, int> Attributes { get; }
        public PartStatus Status { get; private set; }

        public void Accept() => Status = PartStatus.Accepted;
        public void Reject() => Status = PartStatus.Rejected;

        public bool Matches(Rule rule) => rule.Operation switch
        {
            '>' => Attributes[rule.Attribute] > rule.ValueToCheck,
            '<' => Attributes[rule.Attribute] < rule.ValueToCheck,
            _ => throw new NotImplementedException()
        };

        public void Process(Dictionary<string, List<Rule>> workflows)
        {
            var rules = workflows["in"];

            while (Status == PartStatus.Pending)
            {
                foreach (var rule in rules)
                {
                    if (rule.IsDefault || Matches(rule))
                    {
                        if (rule.Result == "A")
                        {
                            Accept();
                            break;
                        }

                        if (rule.Result == "R")
                        {
                            Reject();
                            break;
                        }

                        rules = workflows[rule.Result];
                        break;
                    }
                }
            }
        }
    }

    enum PartStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}