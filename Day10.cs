using System.Text;
using System.Text.RegularExpressions;

namespace Chacode2022;

internal class Day10 : DayX
{
	public void Solve()
	{
		using StreamReader reader = this.GetInput(10);
		List<Cycle> cycles = new();
		int cycle = 1;
		int register = 1;
		Regex addX = new("addx (\\-?[0-9]*)");

		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			Match match = addX.Match(line);
			if (match.Success)
			{
				cycles.Add(new Cycle(cycle++, register));
				cycles.Add(new Cycle(cycle++, register));
				register += int.Parse(match.Groups[1].Value);
			}
			else
			{
				cycles.Add(new Cycle(cycle++, register));
			}
		}

		StringBuilder sb = new();
		foreach (Cycle fullCycle in cycles)
		{
			int crtRowPosition = (fullCycle.Number - 1) % 40;
			if (crtRowPosition == 0)
			{
				sb.AppendLine();
			}

			if (Math.Abs(crtRowPosition - fullCycle.RegisterValue) <= 1)
			{
				sb.Append('#');
			}
			else
			{
				sb.Append('.');
			}
		}

		int[] cyclesToCount = { 20, 60, 100, 140, 180, 220 };
		Console.WriteLine($"Day 10-1: {cyclesToCount.Sum(i => cycles[i - 1].Strength)}");
		Console.WriteLine($"Day 10-2: {sb}");
	}

	private record Cycle(int Number, int RegisterValue)
	{
		public int Strength => this.Number * this.RegisterValue;
	}
}