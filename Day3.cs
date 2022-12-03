namespace Chacode2022;

internal class Day3 : DayX
{
	private static char GetBadge(Rucksack[] rucksacks)
	{
		return rucksacks.First().Whole.Distinct().Single(c => rucksacks.Skip(1).All(m => m.Whole.Contains(c)));
	}

	private static int GetScore(char c)
	{
		if (char.IsUpper(c))
		{
			return c - 64 + 26;
		}
		else
		{
			return c - 96;
		}
	}

	public void Solve()
	{
		using StreamReader reader = this.GetInput(3);
		List<Rucksack> rucksacks = new();
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!.Trim();
			rucksacks.Add(Rucksack.Parse(line, null));
		}

		Console.WriteLine(
			$"Day 2: {rucksacks.Select(s => s.Compartment1.Distinct().Single(t => s.Compartment2.Contains(t))).Sum(GetScore)}|" +
			$"{rucksacks.Chunk(3).Select(GetBadge).Sum(GetScore)}");
	}

	private record Rucksack(List<char> Compartment1, List<char> Compartment2) : IParsable<Rucksack>
	{
		public IEnumerable<char> Whole => this.Compartment1.Concat(this.Compartment2);

		public static Rucksack Parse(string s, IFormatProvider? provider)
		{
			return new Rucksack(s[..(s.Length / 2)].ToList(), s[(s.Length / 2)..].ToList());
		}

		public static bool TryParse(string? s, IFormatProvider? provider, out Rucksack result)
		{
			throw new NotImplementedException();
		}
	}
}