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

	protected void ReportResult(string result)
	{
		Console.WriteLine($"Day {this.GetDay()} {result}");
	}

	protected void ReportResult(int result)
	{
		ReportResult(result.ToString());
	}

	private int GetDay()
	{
		string typeName = this.GetType().Name;
		int day = int.Parse(typeName[(typeName.IndexOf('y') + 1)..]);
		return day;
	}
}