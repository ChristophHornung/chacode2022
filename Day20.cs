namespace Chacode2022;

internal class Day20 : DayX
{
	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput();
		bool isPart1 = part == 1;
		List<Entry> file = new();
		var i = 0;
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			file.Add(new Entry(i++, int.Parse(line)));
		}

		this.Mix(file);
	}

	private void Mix(List<Entry> file)
	{
		foreach (Entry entry in file.ToList())
		{

		}
	}

	private record struct Entry(int Position, int Value);
}