internal static class Day04
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day04/Input.txt");
        var amountsPerCard = new Dictionary<int, int>();
        var matchesPerCard = new Dictionary<int, int>();
        var processingQueue = new Queue<int>();

        foreach (var line in lines)
        {
            var lineSplit = line.Split(":");
            var cardId = int.Parse(lineSplit[0].Replace("Card", "").Trim());
            var allNumbers = lineSplit[1].Trim();
            var allNumbersSplit = allNumbers.Split("|");
            var winningNumbers = allNumbersSplit[0].Trim().Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(int.Parse);
            var numbersYouHave = allNumbersSplit[1].Trim().Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(int.Parse);
            var matchingNumbers = numbersYouHave.Where(x => winningNumbers.Contains(x));

            amountsPerCard.Add(cardId, 1);
            matchesPerCard.Add(cardId, matchingNumbers.Count());

            if (matchingNumbers.Count() > 0)
            {
                processingQueue.Enqueue(cardId);
            }
        }

        while (processingQueue.Any())
        {
            var cardId = processingQueue.Dequeue();

            if (matchesPerCard[cardId] > 0)
            {
                for (int i = cardId + 1; i <= Math.Min(cardId + matchesPerCard[cardId], matchesPerCard.Count); i++)
                {
                    processingQueue.Enqueue(i);
                    amountsPerCard[i] += 1;
                }
            }
        }

        Console.WriteLine($"Day 4, Part 1: {matchesPerCard.Where(x => x.Value > 0).Sum(x => Math.Pow(2, x.Value - 1))}");
        Console.WriteLine($"Day 4, Part 2: {amountsPerCard.Select(x => x.Value).Sum()}");
    }
}