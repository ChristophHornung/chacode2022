using System.Text.RegularExpressions;

namespace Chacode2022;

internal class Day5 : DayX
{
	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput();
		string? line = reader.ReadLine();

		List<List<char>> horizontals = new();
		while (line != null)
		{
			List<char> horizontal = new();
			for (int i = 0; i < line.Length; i += 4)
			{
				horizontal.Add(line[i..(i + 3)][1]);
			}

			horizontals.Add(horizontal);
			line = reader.ReadLine();
			if (line!.Trim().StartsWith("1"))
			{
				break;
			}
		}

		List<Stack<char>> crateStacks = new();
		for (int i = 0; i < horizontals[0].Count; i++)
		{
			crateStacks.Add(new Stack<char>());
		}

		horizontals.Reverse();
		foreach (List<char> horizontal in horizontals)
		{
			int i = 0;
			foreach (char c in horizontal)
			{
				if (c != ' ')
				{
					crateStacks[i].Push(c);
				}

				i++;
			}
		}

		line = reader.ReadLine();
		line = reader.ReadLine();

		while (line != null)
		{
			Regex reg = new Regex("move ([0-9]*) from ([0-9]*) to ([0-9]*)");
			Match match = reg.Match(line);
			if (part == 1)
			{
				this.ExecuteMove9000(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
					int.Parse(match.Groups[3].Value), crateStacks);
			}
			else
			{
				this.ExecuteMove9001(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
					int.Parse(match.Groups[3].Value), crateStacks);
			}

			line = reader.ReadLine();
		}

		List<char> tops = crateStacks.Select(s => s.Pop()).ToList();
		Console.WriteLine($"Day 5-{part}: {new string(tops.ToArray())}");
	}

	private void ExecuteMove9000(int count, int from, int to, List<Stack<char>> crateStacks)
	{
		for (int i = 0; i < count; i++)
		{
			char crate = crateStacks[from - 1].Pop();
			crateStacks[to - 1].Push(crate);
		}
	}

	private void ExecuteMove9001(int count, int from, int to, List<Stack<char>> crateStacks)
	{
		List<char> crates = new();
		for (int i = 0; i < count; i++)
		{
			crates.Add(crateStacks[from - 1].Pop());
		}

		crates.Reverse();
		foreach (char crate in crates)
		{
			crateStacks[to - 1].Push(crate);
		}
	}
}