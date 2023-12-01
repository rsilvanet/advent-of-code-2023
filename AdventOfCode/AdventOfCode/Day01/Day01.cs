internal static class Day01
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day01/Input.txt");

        Console.WriteLine($"Day 1, Part 1: {Sum(lines)}");
        Console.WriteLine($"Day 1, Part 2: {Sum(lines.Select(ReplaceDigits))}");

        int Sum(IEnumerable<string> lines) => lines
            .Select(line => line.Where(c => char.IsNumber(c)))
            .Where(numbers => numbers.Any())
            .Sum(numbers => int.Parse($"{numbers.First()}{numbers.Last()}"));

        string ReplaceDigits(string line) => line
            .Replace("one", "one1one")
            .Replace("two", "two2two")
            .Replace("three", "three3three")
            .Replace("four", "four4four")
            .Replace("five", "five5five")
            .Replace("six", "six6six")
            .Replace("seven", "seven7seven")
            .Replace("eight", "eight8eight")
            .Replace("nine", "nine9nine");
    }
}