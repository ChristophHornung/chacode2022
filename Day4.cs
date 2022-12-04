namespace Chacode2022;

internal class Day4 : DayX
{
	public void Solve()
	{
		using StreamReader reader = this.GetInput(4);

		List<Pair> elves = new();
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			elves.Add(Pair.Parse(line, null));
		}

		int contained = elves.Count(c => this.Covers(c.First, c.Second) || this.Covers(c.Second, c.First));
		int overlaps = elves.Count(c => !this.NoOverlaps(c.First, c.Second));

		Console.WriteLine($"Day 4: {contained}|{overlaps}");
	}

	private bool Covers(Elf first, Elf second)
	{
		return first.SectorStart <= second.SectorStart && first.SectorEnd >= second.SectorEnd;
	}

	private bool NoOverlaps(Elf first, Elf second)
	{
		return first.SectorEnd < second.SectorStart ||
		       first.SectorStart > second.SectorEnd;
	}

	private record Pair(Elf First, Elf Second) : IParsable<Pair>
	{
		public static Pair Parse(string s, IFormatProvider? provider)
		{
			string[] split = s.Split(",");
			return new Pair(Elf.Parse(split[0], null), Elf.Parse(split[1], null));
		}

		public static bool TryParse(string? s, IFormatProvider? provider, out Pair result)
		{
			throw new NotImplementedException();
		}
	}

	private record Elf(int SectorStart, int SectorEnd) : IParsable<Elf>
	{
		public static Elf Parse(string s, IFormatProvider? provider)
		{
			string[] split = s.Split('-');
			return new Elf(int.Parse(split[0]), int.Parse(split[1]));
		}

		public static bool TryParse(string? s, IFormatProvider? provider, out Elf result)
		{
			throw new NotImplementedException();
		}
	}
}