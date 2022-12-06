namespace Chacode2022;

internal class Day6 : DayX
{
	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput(6);
		string line = reader.ReadLine()!;
		int result = 0;
		int markerLength = part == 1 ? 4 : 14;
		for (int i = 0; i < line.Length; i++)
		{
			if (line.Skip(i).Take(markerLength).Distinct().Count() == markerLength)
			{
				result = i + markerLength;
				break;
			}
		}

		Console.WriteLine($"Day 5-{part}: {result}");
	}
}