namespace Chacode2022;

internal class Day18 : DayX
{
	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput();
		bool isPart1 = part == 1;
		HashSet<Position> positions = new();
		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine()!.Split(',');
			positions.Add(new Position(int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2])));
		}

		int surfaceFaces = 0;
		foreach (Position position in positions)
		{
			if (!positions.Contains(new Position(position.X - 1, position.Y, position.Z)))
			{
				surfaceFaces++;
			}

			if (!positions.Contains(new Position(position.X + 1, position.Y, position.Z)))
			{
				surfaceFaces++;
			}

			if (!positions.Contains(new Position(position.X, position.Y - 1, position.Z)))
			{
				surfaceFaces++;
			}

			if (!positions.Contains(new Position(position.X, position.Y + 1, position.Z)))
			{
				surfaceFaces++;
			}

			if (!positions.Contains(new Position(position.X, position.Y, position.Z - 1)))
			{
				surfaceFaces++;
			}

			if (!positions.Contains(new Position(position.X, position.Y, position.Z + 1)))
			{
				surfaceFaces++;
			}
		}

		ReportResult(surfaceFaces.ToString());
	}

	private record struct Position(int X, int Y, int Z);
}