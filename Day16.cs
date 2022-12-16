using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Chacode2022;

internal class Day16 : DayX
{
	private static bool part1;

	public void Solve(int part)
	{
		part1 = part == 1;
		using var reader = GetInput();
		Dictionary<string, List<string>> connections = new();
		Dictionary<string, Valve> valves = new();
		Dictionary<(Valve, Valve), int> distances = new();
		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine()!;
			var reg = new Regex("Valve ([A-Z]*) has flow rate=([0-9]*); tunnels? leads? to valves? ");
			var match = reg.Match(line);
			var v = new Valve(match.Groups[1].Value, int.Parse(match.Groups[2].Value));
			valves.Add(v.Code, v);
			var connection = line[match.Length..].Split(',', StringSplitOptions.TrimEntries);
			connections[v.Code] = new List<string>();
			foreach (var con in connection) connections[v.Code].Add(con);
		}


		foreach (var connection in connections)
		foreach (var dest in connection.Value)
		{
			valves[connection.Key].Connections.Add(valves[dest]);
			distances.Add((valves[connection.Key], valves[dest]), 1);
		}

		foreach (var a in valves.Values)
		foreach (var b in valves.Values)
		{
			distances[(a, b)] = GetDistance(a, b);

			int GetDistance(Valve start, Valve end, int currentDist = 0, HashSet<Valve>? visited = null)
			{
				visited ??= new HashSet<Valve>();
				visited = new HashSet<Valve>(visited);
				if (start == end) return currentDist;
				if (distances.TryGetValue((start, end), out var d)) return d + currentDist;
				visited.Add(start);

				var dist = int.MaxValue;

				currentDist++;
				foreach (var aConnection in start.Connections.Where(v => !visited.Contains(v)))
					dist = Math.Min(GetDistance(aConnection, end, currentDist, visited), dist);

				return dist;
			}
		}

		ReportResult(CheckAllPaths(valves["AA"], valves["AA"], new HashSet<Valve>(),
			valves.Values.Where(f => f.FlowRate > 0).ToHashSet(), distances, 30).ToString());
		currentMaxFlow = 0;
		part1 = false;
		ReportResult(CheckAllPathsWithElephantBuddy(
			valves["AA"],
			new HashSet<Valve>(),
			new HashSet<Valve>(),
			valves.Values.Where(f => f.FlowRate > 0).ToHashSet(),
			distances).ToString());
	}

	private static int currentMaxFlow;

	private int CheckAllPaths(Valve start, Valve position, HashSet<Valve> openValves, HashSet<Valve> closedValves,
		Dictionary<(Valve, Valve), int> distances, int minutesRemaining)
	{
		var longest = GetFlow(distances, openValves, start).flow;
		if (closedValves.Select(c => c.FlowRate * minutesRemaining).Sum() + longest <= currentMaxFlow ||
		    minutesRemaining <= 0)
			return longest;

		if (currentMaxFlow < longest) currentMaxFlow = longest;

		foreach (var valve in closedValves.ToList())
		{
			openValves.Add(valve);
			closedValves.Remove(valve);
			var candidate = CheckAllPaths(start, valve, openValves, closedValves, distances,
				minutesRemaining - distances[(position, valve)] - 1);
			openValves.Remove(valve);
			closedValves.Add(valve);
			if (candidate > longest) longest = candidate;
		}

		return longest;
	}

	private static bool flip;

	private int CheckAllPathsWithElephantBuddy(Valve start,
		HashSet<Valve> openValvesHero, HashSet<Valve> openValvesElephant, HashSet<Valve> closedValves,
		Dictionary<(Valve, Valve), int> distances)
	{
		var flowHero = GetFlow(distances, openValvesHero, start);
		var flowElephant = GetFlow(distances, openValvesElephant, start);

		var largest = flowElephant.flow + flowHero.flow;

		int minutesRemaining =
			Math.Max(flowElephant.minutesRemainingAfterLastOpen, flowHero.minutesRemainingAfterLastOpen);

		if (minutesRemaining <= 0)
			return largest;

		if (closedValves.Select(c => c.FlowRate * minutesRemaining).Sum() + largest <= currentMaxFlow)
			return largest;

		if (currentMaxFlow < largest)
		{
			currentMaxFlow = largest;
		}

		foreach (var valve in closedValves.ToList())
		{
			if (!openValvesElephant.Any() && !openValvesHero.Any())
			{
				Console.WriteLine("X");
			}

			bool hero = flip;
			flip = !flip;
			if (hero)
			{
				openValvesHero.Add(valve);
			}
			else
			{
				openValvesElephant.Add(valve);
			}
			
			closedValves.Remove(valve);

			var candidate =
				CheckAllPathsWithElephantBuddy(start, openValvesHero, openValvesElephant, closedValves, distances);

			if (candidate > largest) largest = candidate;

			if (hero)
			{
				openValvesHero.Remove(valve);
				openValvesElephant.Add(valve);
			}
			else
			{
				openValvesElephant.Remove(valve);
				openValvesHero.Add(valve);
			}
			
			candidate = CheckAllPathsWithElephantBuddy(start, openValvesHero, openValvesElephant, closedValves, distances);
			if (candidate > largest) largest = candidate;

			if (hero)
			{
				openValvesElephant.Remove(valve);
			}
			else
			{
				openValvesHero.Remove(valve);
			}
			
			closedValves.Add(valve);
		}

		return largest;
	}

	private static (int minutesRemainingAfterLastOpen, int flow) GetFlow(Dictionary<(Valve, Valve), int> distances,
		IEnumerable<Valve> path, Valve start)
	{
		var flow = 0;
		var flowRate = 0;
		var minute = 0;
		var position = start;
		int maxMinutes = part1 ? 30 : 26;
		var minutesRemaining = maxMinutes;
		var comb = path.ToList();
		foreach (var valve in comb)
		{
			// Go to valve
			var distance = distances[(position, valve)];
			for (var i = 0; i < distance; i++)
			{
				minute++;
				minutesRemaining--;
				flow += flowRate;
				if (minute == maxMinutes) break;
			}

			position = valve;
			if (minute == maxMinutes) break;

			// Open the valve
			minute++;
			minutesRemaining--;
			flow += flowRate;
			flowRate += valve.FlowRate;
			if (minute == maxMinutes) break;
		}

		while (minute < maxMinutes)
		{
			minute++;
			flow += flowRate;
		}

		// Debug.WriteLine(string.Join('-', comb.Select(v => v.Code)) + " " + flow);
		return (minutesRemaining, flow);
	}

	public static IEnumerable<IEnumerable<T>> DifferentCombinations<T>(IEnumerable<T> elements, int k)
	{
		return k == 0
			? new[] {new T[0]}
			: elements.SelectMany((e, i) =>
				DifferentCombinations<T>(elements.Where(s => !s.Equals(e)), k - 1).Select(c => new[] {e}.Concat(c)));
	}

	public record Valve(string Code, int FlowRate)
	{
		public List<Valve> Connections { get; } = new();

		public override string ToString()
		{
			return $"{Code}:{FlowRate}";
		}
	}
}