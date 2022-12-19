namespace Chacode2022;

using System.Diagnostics;
using System.Text.RegularExpressions;

internal class Day19 : DayX
{
	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput();
		bool isPart1 = part == 1;
		List<Blueprint> blueprints = new();
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			var r = new Regex(
				"Blueprint ([0-9]*): Each ore robot costs ([0-9]*) ore. Each clay robot costs ([0-9]*) ore. Each obsidian robot costs ([0-9]*) ore and ([0-9]*) clay. Each geode robot costs ([0-9]*) ore and ([0-9]*) obsidian.");
			Match match = r.Match(line);
			int i = 1;
			Blueprint blueprint = new(
				int.Parse(match.Groups[i++].Value),
				int.Parse(match.Groups[i++].Value),
				int.Parse(match.Groups[i++].Value),
				int.Parse(match.Groups[i++].Value),
				int.Parse(match.Groups[i++].Value),
				int.Parse(match.Groups[i++].Value),
				int.Parse(match.Groups[i++].Value)
			);
			blueprints.Add(blueprint);
		}

		long result = 0;
		if (isPart1)
		{
			foreach (Blueprint b in blueprints)
			{
				result += this.TestBlueprint(b) * b.Id;
				Console.Write("+");
			}

			this.ReportResult(result.ToString());
		}
		else
		{
			result = 1;
			Day19.maxMinutes = 32;
			foreach (Blueprint b in blueprints.Take(3))
			{
				result *= this.TestBlueprint(b);
				Console.Write("+");
			}

			ReportResult(result.ToString());
		}
	}

	private static int maxMinutes = 24;

	private int GetGeodeCount(Blueprint blueprint, IEnumerable<Robot> buildOrder)
	{
		var oreRobotCount = 1;
		var clayRobotCount = 0;
		var obsidianRobotCount = 0;
		var geodeRobotCount = 0;
		var ore = 0;
		var clay = 0;
		var obsidian = 0;
		var geode = 0;
		foreach (Robot build in buildOrder)
		{
			switch (build)
			{
				case Robot.None:
					break;
				case Robot.Ore:
					ore -= blueprint.OreRobotCost;
					break;
				case Robot.Clay:
					ore -= blueprint.ClayRobotCost;
					break;
				case Robot.Obsidian:
					ore -= blueprint.ObsidianRobotCostOre;
					clay -= blueprint.ObsidianRobotCostClay;
					break;
				case Robot.Geode:
					ore -= blueprint.GeodeRobotCostOre;
					obsidian -= blueprint.GeodeRobotCostObsidian;
					break;
			}

			if (ore < 0 || clay < 0 || obsidian < 0)
			{
				return 0;
			}

			ore += oreRobotCount;
			clay += clayRobotCount;
			obsidian += obsidianRobotCount;
			geode += geodeRobotCount;

			switch (build)
			{
				case Robot.None:
					break;
				case Robot.Ore:
					oreRobotCount++;
					break;
				case Robot.Clay:
					clayRobotCount++;
					break;
				case Robot.Obsidian:
					obsidianRobotCount++;
					break;
				case Robot.Geode:
					geodeRobotCount++;
					break;
			}
		}

		return geode;
	}

	private class State
	{
		public int OreRobotCount { get; set; }
		public int ClayRobotCount { get; set; }
		public int ObsidianRobotCount { get; set; }
		public int GeodeRobotCount { get; set; }
		public int Ore { get; set; }
		public int Clay { get; set; }
		public int Obsidian { get; set; }
		public int Geode { get; set; }

		public void Mine()
		{
			this.Ore += this.OreRobotCount;
			this.Clay += this.ClayRobotCount;
			this.Obsidian += this.ObsidianRobotCount;
			this.Geode += this.GeodeRobotCount;
		}

		public void UnMine()
		{
			this.Ore -= this.OreRobotCount;
			this.Clay -= this.ClayRobotCount;
			this.Obsidian -= this.ObsidianRobotCount;
			this.Geode -= this.GeodeRobotCount;
		}
	}

	private int TestBlueprint(Blueprint blueprint)
	{
		Day19.maxGeodes = 0;
		Stack<Robot> buildOrder = new();
		State state = new() {OreRobotCount = 1};
		return this.TestBlueprint(blueprint, buildOrder, state);
	}

	private static int maxGeodes;

	private static readonly int[] possibleGeodes =
		Enumerable.Range(0, 34).Select(m => Enumerable.Range(0, m).Sum()).ToArray();

	private int TestBlueprint(Blueprint blueprint, Stack<Robot> buildOrder, State state)
	{
		int geodes = state.Geode;

		if (geodes > Day19.maxGeodes)
		{
			Day19.maxGeodes = geodes;
		}

		if (buildOrder.Count == Day19.maxMinutes)
		{
			Debug.Assert(geodes == this.GetGeodeCount(blueprint, buildOrder.Reverse()));

			return state.Geode;
		}

		int remaining = Day19.maxMinutes - buildOrder.Count;

		if (state.Geode + state.GeodeRobotCount * remaining + Day19.possibleGeodes[remaining] <= Day19.maxGeodes)
		{
			return state.Geode;
		}

		if (buildOrder.Count != Day19.maxMinutes - 1)
		{
			// No robots on the last minute
			if (state.Ore >= blueprint.GeodeRobotCostOre && state.Obsidian >= blueprint.GeodeRobotCostObsidian)
			{
				state.Ore -= blueprint.GeodeRobotCostOre;
				state.Obsidian -= blueprint.GeodeRobotCostObsidian;
				state.Mine();
				state.GeodeRobotCount++;
				buildOrder.Push(Robot.Geode);
				geodes = Math.Max(geodes, this.TestBlueprint(blueprint, buildOrder, state));
				buildOrder.Pop();
				state.GeodeRobotCount--;
				state.UnMine();
				state.Obsidian += blueprint.GeodeRobotCostObsidian;
				state.Ore += blueprint.GeodeRobotCostOre;
			}

			if (state.Ore >= blueprint.ObsidianRobotCostOre && state.Clay >= blueprint.ObsidianRobotCostClay &&
			    state.ObsidianRobotCount < blueprint.GeodeRobotCostObsidian)
			{
				state.Ore -= blueprint.ObsidianRobotCostOre;
				state.Clay -= blueprint.ObsidianRobotCostClay;
				state.Mine();
				state.ObsidianRobotCount++;
				buildOrder.Push(Robot.Obsidian);
				geodes = Math.Max(geodes, this.TestBlueprint(blueprint, buildOrder, state));
				buildOrder.Pop();
				state.ObsidianRobotCount--;
				state.UnMine();
				state.Clay += blueprint.ObsidianRobotCostClay;
				state.Ore += blueprint.ObsidianRobotCostOre;
			}

			if (state.Ore >= blueprint.ClayRobotCost &&
			    state.ClayRobotCount < blueprint.ObsidianRobotCostClay)
			{
				state.Ore -= blueprint.ClayRobotCost;
				state.Mine();
				state.ClayRobotCount++;
				buildOrder.Push(Robot.Clay);
				geodes = Math.Max(geodes, this.TestBlueprint(blueprint, buildOrder, state));
				buildOrder.Pop();
				state.ClayRobotCount--;
				state.UnMine();
				state.Ore += blueprint.ClayRobotCost;
			}

			if (state.Ore >= blueprint.OreRobotCost &&
			    state.OreRobotCount < blueprint.MaxOreCost)
			{
				state.Ore -= blueprint.OreRobotCost;
				state.Mine();
				state.OreRobotCount++;
				buildOrder.Push(Robot.Ore);
				geodes = Math.Max(geodes, this.TestBlueprint(blueprint, buildOrder, state));
				buildOrder.Pop();
				state.OreRobotCount--;
				state.UnMine();
				state.Ore += blueprint.OreRobotCost;
			}
		}

		{
			state.Mine();
			buildOrder.Push(Robot.None);
			geodes = Math.Max(geodes, this.TestBlueprint(blueprint, buildOrder, state));
			buildOrder.Pop();
			state.UnMine();
		}

		return geodes;
	}

	private record struct Blueprint
	{
		public Blueprint(int id, int oreRobotCost, int clayRobotCost, int obsidianRobotCostOre,
			int obsidianRobotCostClay, int geodeRobotCostOre, int geodeRobotCostObsidian)
		{
			this.Id = id;
			this.OreRobotCost = oreRobotCost;
			this.ClayRobotCost = clayRobotCost;
			this.ObsidianRobotCostOre = obsidianRobotCostOre;
			this.ObsidianRobotCostClay = obsidianRobotCostClay;
			this.GeodeRobotCostOre = geodeRobotCostOre;
			this.GeodeRobotCostObsidian = geodeRobotCostObsidian;
			this.MaxOreCost = new[]
				{this.OreRobotCost, this.ClayRobotCost, this.ObsidianRobotCostOre, this.GeodeRobotCostOre}.Max();
		}

		public int MaxOreCost { get; }
		public int Id { get; }
		public int OreRobotCost { get; }
		public int ClayRobotCost { get; }
		public int ObsidianRobotCostOre { get; }
		public int ObsidianRobotCostClay { get; }
		public int GeodeRobotCostOre { get; }
		public int GeodeRobotCostObsidian { get; }

	}

	private enum Robot
	{
		None,
		Ore,
		Clay,
		Obsidian,
		Geode
	}
}