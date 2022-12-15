namespace Chacode2022;

internal class Day14 : DayX
{
	private static bool part1;

	public void Solve(int part)
	{
		part1 = part == 1;
		using var reader = GetInput();
		NoitaMap map = new();
		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine()!;
			Point? current = null;
			foreach (var end in line.Replace("->", null)
				         .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
			{
				var split = end.Split(',');
				var endPoint = new Point(int.Parse(split[0]), int.Parse(split[1]));
				if (current == null)
				{
					current = endPoint;
					map.SetElement(current, Element.Rock);
				}
				else
				{
					while (current != endPoint)
					{
						current = current with
						{
							X = current.X + Math.Sign(endPoint.X - current.X),
							Y = current.Y + Math.Sign(endPoint.Y - current.Y)
						};
						map.SetElement(current, Element.Rock);
					}
				}
			}
		}

		if (!part1)
		{
			var maxY = map.MaxY;
			for (var xf = 0; xf < 10_000; xf++) map.SetElement(new Point(xf, maxY + 2), Element.Rock);
		}

		var x = 0;
		while (map.AddSand(new Point(500, 0))) x++;
		if (!part1) x++;
		Console.WriteLine($"Day 14-{part} {x}");
	}

	private class NoitaMap
	{
		private readonly Dictionary<Point, Element> map = new();

		public int MaxY { get; private set; }

		public void SetElement(Point p, Element e)
		{
			map[p] = e;
			if (p.Y > MaxY) MaxY = p.Y;
		}

		public Element GetElement(Point p)
		{
			if (map.TryGetValue(p, out var element)) return element;

			return Element.None;
		}

		public bool AddSand(Point p)
		{
			var start = p;
			var restOrFreeFall = false;
			while (!restOrFreeFall)
			{
				var dropped = DropOne(p);
				if (dropped.Y == p.Y || dropped.Y >= MaxY) restOrFreeFall = true;

				p = dropped;
			}

			if (p.Y >= MaxY || p == start) return false;

			SetElement(p, Element.Sand);
			return true;
		}

		private Point DropOne(Point p)
		{
			foreach (var checkPoint in new[]
			         {
				         p with {Y = p.Y + 1},
				         new Point(p.X - 1, p.Y + 1),
				         new Point(p.X + 1, p.Y + 1)
			         })
				if (GetElement(checkPoint) == Element.None)
					return checkPoint;

			return p;
		}
	}


	private record Point(int X, int Y);

	internal enum Element
	{
		None,
		Rock,
		Sand
	}
}