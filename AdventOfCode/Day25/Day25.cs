internal static class Day25
{
    internal static void Solve()
    {
        var lines = File.ReadLines("Day25/Input.txt");
        var edges = new List<(string from, string to)>();
        var random = new Random(2023);

        foreach (var line in lines)
        {
            var lineSplit = line.Split(':');
            var leftModule = lineSplit[0];
            var rightModules = lineSplit[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var rightModule in rightModules)
            {
                edges.Add((leftModule, rightModule));
            }
        }

        while (true)
        {
            (var verticeGroups, var finalEdges) =  KargersContract(edges, random);

            if (verticeGroups.All(x => x.Value.Count > 1) && finalEdges.Count() == 3)
            {
                Console.WriteLine($"Day 25: {verticeGroups.ElementAt(0).Value.Count * verticeGroups.ElementAt(1).Value.Count}");
                break;
            }
        }

        (Dictionary<string, List<string>> verticeGroups, IEnumerable<(string from, string to)> finalEdges) KargersContract(List<(string from, string to)> edges, Random random)
        {
            edges = edges.ToList();

            var vertices = edges.SelectMany(x => new[] { x.from, x.to }).Distinct();
            var verticeGroups = vertices.ToDictionary(x => x, x => new List<string>() { x });

            while (verticeGroups.Count > 2)
            {
                var randomEdge = edges[random.Next(edges.Count())];
                var verticeToKeep = randomEdge.from;
                var verticeToDelete = randomEdge.to;

                verticeGroups[verticeToKeep].AddRange(verticeGroups[verticeToDelete]);
                verticeGroups.Remove(verticeToDelete);

                edges = edges.FindAll(e => !(e.from == verticeToKeep && e.to == verticeToDelete));
                edges = edges.FindAll(e => !(e.from == verticeToDelete && e.to == verticeToKeep));
                edges = edges.Select(e => (e.from == verticeToDelete ? verticeToKeep : e.from, e.to == verticeToDelete ? verticeToKeep : e.to)).ToList();
            }

            return (verticeGroups, edges);
        }
    }
}