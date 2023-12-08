internal static class Day07
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day07/Input.txt");
        var hands = new List<Hand>();

        foreach (var line in lines)
        {
            var lineSplit = line.Split(" ");

            hands.Add(new Hand(
                cards: lineSplit[0],
                bid: int.Parse(lineSplit[1])
            ));
        }

        var resultPart1 = hands
            .OrderBy(x => x.CalculateStrength())
            .ThenBy(x => x.GetCardValue(x.Cards[0]))
            .ThenBy(x => x.GetCardValue(x.Cards[1]))
            .ThenBy(x => x.GetCardValue(x.Cards[2]))
            .ThenBy(x => x.GetCardValue(x.Cards[3]))
            .ThenBy(x => x.GetCardValue(x.Cards[4]))
            .Select((hand, index) => hand.Bid * (index + 1))
            .Sum();

        var resultPart2 = hands
            .OrderBy(x => x.CalculateStrengthWithJoker())
            .ThenBy(x => x.GetCardValueWithJoker(x.Cards[0]))
            .ThenBy(x => x.GetCardValueWithJoker(x.Cards[1]))
            .ThenBy(x => x.GetCardValueWithJoker(x.Cards[2]))
            .ThenBy(x => x.GetCardValueWithJoker(x.Cards[3]))
            .ThenBy(x => x.GetCardValueWithJoker(x.Cards[4]))
            .Select((hand, index) => hand.Bid * (index + 1))
            .Sum();
        
        Console.WriteLine($"Day 7, Part 1: {resultPart1}");
        Console.WriteLine($"Day 7, Part 2: {resultPart2}");
    }

    class Hand
    {
        private readonly Dictionary<char, int> _cardCounts = new Dictionary<char, int>()
        {
            { 'A' , 0 }, { 'K' , 0 }, { 'Q' , 0 }, { 'J' , 0 }, { 'T' , 0 },
            { '9' , 0 }, { '8' , 0 }, { '7' , 0 }, { '6' , 0 },
            { '5' , 0 }, { '4' , 0 }, { '3' , 0 }, { '2' , 0 }
        };

        public Hand(string cards, int bid)
        {
            Cards = cards.Select(x => x).ToArray();
            Bid = bid;

            foreach (var card in cards)
            {
                _cardCounts[card] += 1;
            }
        }

        public char[] Cards { get; }
        public int Bid { get; }

        public HandType CalculateStrength()
        {
            if (_cardCounts.Any(x => x.Value == 5))
                return HandType.Five;
            if (_cardCounts.Any(x => x.Value == 4))
                return HandType.Four;
            if (_cardCounts.Any(x => x.Value == 3) && _cardCounts.Any(x => x.Value == 2))
                return HandType.FullHouse;
            if (_cardCounts.Any(x => x.Value == 3))
                return HandType.Three;
            if (_cardCounts.Count(x => x.Value == 2) == 2)
                return HandType.TwoPair;
            if (_cardCounts.Any(x => x.Value == 2))
                return HandType.OnePair;
            return HandType.High;
        }

        public HandType CalculateStrengthWithJoker()
        {
            var amountOfJokers = _cardCounts['J'];
            var cardCountsWithoutJoker = _cardCounts.Where(x => x.Key != 'J').ToDictionary(x => x.Key, y => y.Value);

            if (cardCountsWithoutJoker.Any(x => x.Value + amountOfJokers == 5))
                return HandType.Five;
            if (cardCountsWithoutJoker.Any(x => x.Value + amountOfJokers == 4))
                return HandType.Four;

            var bestCardCombination = LookForBestCardCombination(cardCountsWithoutJoker, amountOfJokers);

            if (bestCardCombination == HandType.FullHouse)
                return HandType.FullHouse;
            if (cardCountsWithoutJoker.Any(x => x.Value + amountOfJokers == 3))
                return HandType.Three;
            if (bestCardCombination == HandType.TwoPair)
                return HandType.TwoPair;
            if (cardCountsWithoutJoker.Any(x => x.Value + amountOfJokers == 2))
                return HandType.OnePair;
            return HandType.High;
        }

        private HandType LookForBestCardCombination(Dictionary<char, int> cardCountsWithoutJoker, int amountOfJokers)
        {
            var bestCardCombination = HandType.High;

            for (int setOfJokers1 = 0; setOfJokers1 <= amountOfJokers; setOfJokers1++)
            {
                var firstPair = cardCountsWithoutJoker.FirstOrDefault(x => x.Value + setOfJokers1 == 2);

                if (firstPair.Value > 0)
                {
                    var setOfJokers2 = amountOfJokers - setOfJokers1;

                    if (cardCountsWithoutJoker.Any(x => x.Key != firstPair.Key && x.Value + setOfJokers2 == 3))
                        return HandType.FullHouse;
                    else if (cardCountsWithoutJoker.Any(x => x.Key != firstPair.Key && x.Value + setOfJokers2 == 2))
                        bestCardCombination = HandType.TwoPair;
                }
            }

            return bestCardCombination;
        }

        public int GetCardValue(char card) => card switch
        {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 11,
            'T' => 10,
            _ => (int)char.GetNumericValue(card)
        };

        public int GetCardValueWithJoker(char card) => card switch
        {
            'J' => 1,
            _ => GetCardValue(card)
        };

        public override string ToString() => new string(Cards);
    }

    enum HandType
    {
        High,
        OnePair,
        TwoPair,
        Three,
        FullHouse,
        Four,
        Five
    }
}