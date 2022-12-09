using System.Reflection;

namespace Chacode2022
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			new Day1().Solve();
			new Day2().Solve();
			new Day3().Solve();
			new Day4().Solve();
			new Day5().Solve(1);
			new Day5().Solve(2);
			new Day6().Solve(1);
			new Day6().Solve(2);
			new Day7().Solve();
			new Day8().Solve();
			new Day9().Solve(1);
			new Day9().Solve(2);
			Console.ReadKey();
		}
	}

	internal class DayX
	{
		protected StreamReader GetInput(int day)
		{
			var assembly = Assembly.GetExecutingAssembly();

			Stream stream = assembly.GetManifestResourceStream($"Chacode2022.Day{day}.txt")!;
			StreamReader reader = new StreamReader(stream);
			return reader;
		}
	}
}