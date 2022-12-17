namespace Chacode2022;

using System.Reflection;

internal class DayX
{
	protected StreamReader GetInput()
	{
		var assembly = Assembly.GetExecutingAssembly();

		int day = this.GetDay();

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
		Console.WriteLine($"Day {this.GetDay()} {result}");
	}
}