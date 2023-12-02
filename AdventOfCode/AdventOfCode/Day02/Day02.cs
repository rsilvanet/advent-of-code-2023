internal static class Day02
{
    internal static void Solve()
    {
        const string RED = "red";
        const string GREEN = "green";
        const string BLUE = "blue";

        var lines = File.ReadLines("Day02/Input.txt");
        var games = new List<(int id, int red, int green, int blue, bool possible, int maxRed, int maxGreen, int maxBlue)>();

        foreach (var line in lines)
        {
            var lineSplit = line.Split(":");
            var id = int.Parse(lineSplit[0].Replace("Game ", ""));
            var game = (id, red: 0, green: 0, blue: 0, possible: true, maxRed: 0, maxGreen: 0, maxBlue: 0);

            foreach (var set in lineSplit[1].Split(";"))
            {
                foreach (var cube in set.Split(","))
                {
                    var cubeSplit = cube.Trim().Split(" ");
                    var amount = int.Parse(cubeSplit[0]);
                    var color = cubeSplit[1];

                    game.red += color == RED ? amount : 0;
                    game.green += color == GREEN ? amount : 0;
                    game.blue += color == BLUE ? amount : 0;
                    game.possible = game.possible && IsPossible(color, amount);
                    game.maxRed = color == RED && amount > game.maxRed ? amount : game.maxRed;
                    game.maxGreen = color == GREEN && amount > game.maxGreen ? amount : game.maxGreen;
                    game.maxBlue = color == BLUE && amount > game.maxBlue ? amount : game.maxBlue;
                }
            }

            games.Add(game);
        }

        Console.WriteLine($"Day 2, Part 1: {games.Where(x => x.possible).Sum(x => x.id)}");
        Console.WriteLine($"Day 2, Part 2: {games.Sum(x => x.maxRed * x.maxGreen * x.maxBlue)}");

        bool IsPossible(string color, int amount) => color switch
        {
            RED => amount <= 12,
            GREEN => amount <= 13,
            BLUE => amount <= 14,
            _ => false
        };
    }
}