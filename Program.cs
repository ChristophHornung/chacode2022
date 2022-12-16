using System.Reflection;

namespace Chacode2022
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//new Day1().Solve();
			//new Day2().Solve();
			//new Day3().Solve();
			//new Day4().Solve();
			//new Day5().Solve(1);
			//new Day5().Solve(2);
			//new Day6().Solve(1);
			//new Day6().Solve(2);
			//new Day7().Solve();
			//new Day8().Solve();
			//new Day9().Solve(1);
			//new Day9().Solve(2);
			//new Day10().Solve();
			//new Day11().Solve(1);
			//new Day11().Solve(2);
			//new Day12().Solve(1);
			//new Day12().Solve(2);
			//new Day13().Solve(1);
			//new Day14().Solve(1);
			//new Day14().Solve(2);
			//new Day15().Solve(1);
			new Day16().Solve(1);
			Console.ReadKey();
		}
	}

	internal class DayX
	{
		protected StreamReader GetInput()
		{
			var assembly = Assembly.GetExecutingAssembly();

			int day = GetDay();

			Stream stream = assembly.GetManifestResourceStream($"Chacode2022.Day{day}.txt")!;
			StreamReader reader = new StreamReader(stream);
			return reader;
		}

		private int GetDay()
		{
			int day = int.Parse(this.GetType().Name[3..]);
			return day;
		}

		protected void ReportResult(string result)
		{
			Console.WriteLine($"Day {GetDay()} {result}");
		}
	}
}