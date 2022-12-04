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