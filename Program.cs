using System.Reflection;

namespace Chacode2022
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			new Day1().Solve();
		}
	}

	internal class Day1
	{
		public void Solve()
		{
			var assembly = Assembly.GetExecutingAssembly();

			using Stream stream = assembly.GetManifestResourceStream(EmbeddedResources.Day1_txt)!;
			using StreamReader reader = new StreamReader(stream);

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

		private record Elf(int Calories)
		{
		}
	}
}