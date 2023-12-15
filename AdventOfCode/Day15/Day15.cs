using System.Text;

internal static class Day15
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day15/Input.txt");
        var sequence = lines.First().Split(',');

        Console.WriteLine($"Day 15, Part 1: {sequence.Sum(x => GetHash(x))}");

        var boxes = new Dictionary<int, List<(string label, int lens)>>();

        for (int i = 0; i < 256; i++)
        {
            boxes.Add(i, new List<(string label, int lens)>());
        }

        foreach (var line in sequence)
        {
            if (line.EndsWith('-'))
            {
                var label = line.Substring(0, line.Length - 1);
                var box = GetHash(label);

                boxes[box].RemoveAll(x => x.label == label);

            }
            else
            {
                var split = line.Split('=');
                var label = split[0];
                var box = GetHash(label);
                var lens = int.Parse(split[1]);

                if (boxes[box].Any(x => x.label == label))
                {
                    for (int i = 0; i < boxes[box].Count; i++)
                    {
                        if (boxes[box][i].label == label)
                        {
                            boxes[box][i] = (label, lens);
                            break;
                        }
                    }
                }
                else
                {
                    boxes[box].Add((label, lens));
                }
            }
        }

        Console.WriteLine($"Day 15, Part 2: {boxes.SelectMany(box => box.Value.Select((item, index) => (box.Key + 1) * (index + 1) * item.lens)).Sum()}");

        int GetHash(string word)
        {
            var value = 0;
            var ascBytes = Encoding.ASCII.GetBytes(word);

            foreach (var item in ascBytes)
            {
                value += item;
                value *= 17;
                value %= 256;
            }

            return value;
        }
    }
}