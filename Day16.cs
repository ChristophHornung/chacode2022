using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Chacode2022;

internal class Day16 : DayX
{
	private static bool part1;

	public void Solve(int part)
	{
		Day16.part1 = part == 1;
		using StreamReader reader = this.GetInput();
		Dictionary<string, List<string>> connections = new();
		Dictionary<string, Valve> valves = new();
		Dictionary<(Valve, Valve), int> distances = new();
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			var reg = new Regex("Valve ([A-Z]*) has flow rate=([0-9]*); tunnels? leads? to valves? ");
			Match match = reg.Match(line);
			var v = new Valve(match.Groups[1].Value, int.Parse(match.Groups[2].Value));
			valves.Add(v.Code, v);
			string[] connection = line[match.Length..].Split(',', StringSplitOptions.TrimEntries);
			connections[v.Code] = new List<string>();
			foreach (string con in connection)
			{
				connections[v.Code].Add(con);
			}
		}


		foreach (KeyValuePair<string, List<string>> connection in connections)
		foreach (string dest in connection.Value)
		{
			valves[connection.Key].Connections.Add(valves[dest]);
			distances.Add((valves[connection.Key], valves[dest]), 1);
		}

		foreach (Valve a in valves.Values)
		foreach (Valve b in valves.Values)
		{
			distances[(a, b)] = GetDistance(a, b);

			int GetDistance(Valve start, Valve end, int currentDist = 0, HashSet<Valve>? visited = null)
			{
				visited ??= new HashSet<Valve>();
				visited = new HashSet<Valve>(visited);
				if (start == end)
				{
					return currentDist;
				}

				if (distances.TryGetValue((start, end), out int d))
				{
					return d + currentDist;
				}

				visited.Add(start);

				var dist = int.MaxValue;

				currentDist++;
				foreach (Valve aConnection in start.Connections.Where(v => !visited.Contains(v)))
				{
					dist = Math.Min(GetDistance(aConnection, end, currentDist, visited), dist);
				}

				return dist;
			}
		}

		this.ReportResult(this.CheckAllPaths(valves["AA"], valves["AA"], new HashSet<Valve>(),
			valves.Values.Where(f => f.FlowRate > 0).ToHashSet(), distances, 30).ToString());
		Day16.currentMaxFlow = 0;
		Day16.part1 = false;

		this.ReportResult(this.CheckAllPaths(valves["AA"], valves["AA"], new HashSet<Valve>(),
			valves.Values.Where(f => f.FlowRate > 0).ToHashSet(), distances, 26).ToString());

		this.ReportResult(this.CheckAllPathsWithElephantBuddy(
			valves["AA"],
			new List<Valve>(),
			new List<Valve>(),
			valves.Values.Where(f => f.FlowRate > 0).ToHashSet(),
			distances).ToString());
	}

	private static int currentMaxFlow;

	private int CheckAllPaths(Valve start, Valve position, HashSet<Valve> openValves, HashSet<Valve> closedValves,
		Dictionary<(Valve, Valve), int> distances, int minutesRemaining)
	{
		int longest = Day16.GetFlow(distances, openValves, start).flow;
		if (closedValves.Select(c => c.FlowRate * minutesRemaining).Sum() + longest <= Day16.currentMaxFlow ||
		    minutesRemaining <= 0)
		{
			return longest;
		}

		if (Day16.currentMaxFlow < longest)
		{
			Day16.currentMaxFlow = longest;
		}

		foreach (Valve valve in closedValves.ToList())
		{
			openValves.Add(valve);
			closedValves.Remove(valve);
			int candidate = this.CheckAllPaths(start, valve, openValves, closedValves, distances,
				minutesRemaining - distances[(position, valve)] - 1);
			openValves.Remove(valve);
			closedValves.Add(valve);
			if (candidate > longest)
			{
				longest = candidate;
			}
		}

		return longest;
	}

	private static bool flip;

	private int CheckAllPathsWithElephantBuddy(Valve start,
		List<Valve> openValvesHero, List<Valve> openValvesElephant, HashSet<Valve> closedValves,
		Dictionary<(Valve, Valve), int> distances)
	{
		(int minutesRemainingAfterLastOpen, int flow) flowHero = Day16.GetFlow(distances, openValvesHero, start);
		(int minutesRemainingAfterLastOpen, int flow)
			flowElephant = Day16.GetFlow(distances, openValvesElephant, start);

		int largest = flowElephant.flow + flowHero.flow;

		if (Day16.currentMaxFlow < largest)
		{
			Day16.currentMaxFlow = largest;
		}

		int minutesRemaining =
			Math.Max(flowElephant.minutesRemainingAfterLastOpen, flowHero.minutesRemainingAfterLastOpen);

		Valve positionHero = openValvesHero.Count > 0 ? openValvesHero[^1] : start;
		Valve positionElephant = openValvesElephant.Count > 0 ? openValvesElephant[^1] : start;
		if (flowElephant.minutesRemainingAfterLastOpen - (2 * distances[(positionHero, positionElephant)] + 1) >
		    flowHero.minutesRemainingAfterLastOpen)
		{
			// Invalid path, the elephant could have gotten to and from the valve the hero just opened in less time remaining
			return largest;
		}

		if (flowHero.minutesRemainingAfterLastOpen - (2 * distances[(positionHero, positionElephant)] + 1) >
		    flowElephant.minutesRemainingAfterLastOpen)
		{
			// Reverse check
			return largest;
		}

		if (minutesRemaining <= 0)
		{
			return largest;
		}

		if (openValvesHero.Count + closedValves.Count < openValvesElephant.Count)
		{
			// The hero will always open more or equal the amount of valves than the elephant.
			return largest;
		}

		if (closedValves.Select(c => c.FlowRate * minutesRemaining).Sum() + largest <= Day16.currentMaxFlow)
		{
			return largest;
		}

		foreach (Valve valve in closedValves.ToList())
		{
			bool firstValve = openValvesElephant.Count + openValvesHero.Count == 0;
			bool secondValve = openValvesElephant.Count + openValvesHero.Count == 1;
			if (firstValve)
			{
				Console.WriteLine("X");
			}

			bool hero = Day16.flip;
			Day16.flip = !Day16.flip;
			if (hero || firstValve)
			{
				// Hero starts
				openValvesHero.Add(valve);
			}
			else
			{
				openValvesElephant.Add(valve);
			}

			closedValves.Remove(valve);

			int candidate =
				this.CheckAllPathsWithElephantBuddy(start, openValvesHero, openValvesElephant, closedValves, distances);

			if (candidate > largest)
			{
				largest = candidate;
			}

			if (firstValve)
			{
				// We do not need to check the elephant as well since it doesn't matter who started
				openValvesHero.RemoveAt(openValvesHero.Count - 1);
				closedValves.Add(valve);
			}
			else
			{
				if (hero)
				{
					openValvesHero.RemoveAt(openValvesHero.Count - 1);
					openValvesElephant.Add(valve);
				}
				else
				{
					openValvesElephant.RemoveAt(openValvesElephant.Count - 1);
					openValvesHero.Add(valve);
				}

				candidate = this.CheckAllPathsWithElephantBuddy(start, openValvesHero, openValvesElephant, closedValves,
					distances);

				if (secondValve)
				{
					Console.Write("+");
				}

				if (candidate > largest)
				{
					largest = candidate;
				}

				if (hero)
				{
					openValvesElephant.RemoveAt(openValvesElephant.Count - 1);
				}
				else
				{
					openValvesHero.RemoveAt(openValvesHero.Count - 1);
				}

				closedValves.Add(valve);
			}
		}

		return largest;
	}

	private static (int minutesRemainingAfterLastOpen, int flow) GetFlow(Dictionary<(Valve, Valve), int> distances,
		IEnumerable<Valve> path, Valve start)
	{
		var flow = 0;
		var flowRate = 0;
		var minute = 0;
		Valve position = start;
		int maxMinutes = Day16.part1 ? 30 : 26;
		int minutesRemaining = maxMinutes;
		List<Valve> comb = path.ToList();
		foreach (Valve valve in comb)
		{
			// Go to valve
			int distance = distances[(position, valve)];
			for (var i = 0; i < distance; i++)
			{
				minute++;
				minutesRemaining--;
				flow += flowRate;
				if (minute == maxMinutes)
				{
					break;
				}
			}

			position = valve;
			if (minute == maxMinutes)
			{
				break;
			}

			// Open the valve
			minute++;
			minutesRemaining--;
			flow += flowRate;
			flowRate += valve.FlowRate;
			if (minute == maxMinutes)
			{
				break;
			}
		}

		while (minute < maxMinutes)
		{
			minute++;
			flow += flowRate;
		}

		// Debug.WriteLine(string.Join('-', comb.Select(v => v.Code)) + " " + flow);
		return (minutesRemaining, flow);
	}

	public record Valve(string Code, int FlowRate)
	{
		public List<Valve> Connections { get; } = new();

		public override string ToString()
		{
			return $"{this.Code}:{this.FlowRate}";
		}
	}
}