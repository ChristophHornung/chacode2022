namespace Chacode2022;

internal class Day1 : DayX
{
	public void Solve()
	{
		using StreamReader reader = this.GetInput();

		List<Elf> elves = new();
		int total = 0;
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			if (string.IsNullOrEmpty(line))
			{
				elves.Add(new Elf(total));
				total = 0;
			}
			else
			{
				total += int.Parse(line);
			}
		}

		Console.WriteLine(
			$"Day 1: {elves.MaxBy(e => e.Calories)!.Calories}:{elves.OrderByDescending(e => e.Calories).Take(3).Sum(e => e.Calories)}");
	}

	private record Elf(int Calories);
}