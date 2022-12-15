using System.Diagnostics;

namespace Chacode2022;

internal class Day12 : DayX
{
	private static bool part1;

	public void Solve(int part)
	{
		part1 = part == 1;
		using StreamReader reader = this.GetInput();
		Dictionary<Point, int> mapValues = new();
		List<Point> starts = new();
		Point? end = null;
		int x = 0;
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			int y = 0;
			foreach (char c in line)
			{
				int elevation = c - 97;
				if (c == 'S')
				{
					starts.Add(new Point(x, y));
					elevation = 'a' - 97;
				}
				else if (c == 'E')
				{
					end = new Point(x, y);
					elevation = 'z' - 97;
				}
				else if (c == 'a' && part == 2)
				{
					starts.Add(new Point(x, y));
				}

				mapValues.Add(new Point(x, y++), elevation);
			}

			x++;
		}

		Debug.Assert(end != null, nameof(end) + " != null");
		List<int> shortestFromStart = new();
		foreach (Point start in starts)
		{
			Path startPath = new Path(start, new Map(mapValues));
			List<Path> paths = this.FindAllPaths(startPath, end).ToList();
			if (paths.Any())
			{
				shortestFromStart.Add(paths.Select(p => p.Visited.Count).Min() - 1);
			}
		}


		Console.WriteLine($"Day 12-{part} {shortestFromStart.Min()}");
	}

	private IEnumerable<Path> FindAllPaths(Path startPath, Point end)
	{
		if (startPath.Position == end)
		{
			yield return startPath;
		}
		foreach (Path subPath in startPath.GetSubPaths())
		{
			foreach (Path path in this.FindAllPaths(subPath, end))
			{
				yield return path;
			}
		}
	}

	private class Path
	{
		private readonly Map map;

		public Path(Point position, Map map) : this(position, new HashSet<Point>(), map)
		{
		}

		public Path(Point position, HashSet<Point> visited, Map map)
		{
			this.map = map;
			this.Position = position;
			this.Visited = visited;
			this.Visited.Add(position);
		}

		public HashSet<Point> Visited { get; }

		public Point Position { get; }

		public IEnumerable<Path> GetSubPaths()
		{
			int elevation = this.map.GetElevation(this.Position)!.Value;
			Point next = this.Position with { X = this.Position.X + 1 };
			int? currentShortest = this.map.GetShortestKnown(next);
			if (this.map.GetElevation(next).HasValue && this.map.GetElevation(next) <= elevation + 1 &&
			    (part1 || this.map.GetElevation(next) > 0) &&
			    !this.Visited.Contains(next) && (currentShortest == null || currentShortest > this.Visited.Count + 1))
			{
				this.map.SetShortestKnown(next, this.Visited.Count + 1);
				yield return new Path(next, new HashSet<Point>(this.Visited), this.map);
			}

			next = this.Position with { X = this.Position.X - 1 };
			currentShortest = this.map.GetShortestKnown(next);
			if (this.map.GetElevation(next).HasValue && this.map.GetElevation(next) <= elevation + 1 &&
			    (part1 || this.map.GetElevation(next) > 0) &&
			    !this.Visited.Contains(next) && (currentShortest == null || currentShortest > this.Visited.Count + 1))
			{
				this.map.SetShortestKnown(next, this.Visited.Count + 1);
				yield return new Path(next, new HashSet<Point>(this.Visited), this.map);
			}

			next = this.Position with { Y = this.Position.Y + 1 };
			currentShortest = this.map.GetShortestKnown(next);
			if (this.map.GetElevation(next).HasValue && this.map.GetElevation(next) <= elevation + 1 &&
			    (part1 || this.map.GetElevation(next) > 0) &&
			    !this.Visited.Contains(next) && (currentShortest == null || currentShortest > this.Visited.Count + 1))
			{
				this.map.SetShortestKnown(next, this.Visited.Count + 1);
				yield return new Path(next, new HashSet<Point>(this.Visited), this.map);
			}

			next = this.Position with { Y = this.Position.Y - 1 };
			currentShortest = this.map.GetShortestKnown(next);
			if (this.map.GetElevation(next).HasValue && this.map.GetElevation(next) <= elevation + 1 &&
			    (part1 || this.map.GetElevation(next) > 0) &&
			    !this.Visited.Contains(next) && (currentShortest == null || currentShortest > this.Visited.Count + 1))
			{
				this.map.SetShortestKnown(next, this.Visited.Count + 1);
				yield return new Path(next, new HashSet<Point>(this.Visited), this.map);
			}
		}
	}

	private class Map
	{
		private readonly Dictionary<Point, int> elevation;
		private Dictionary<Point, int> shortest = new();

		public Map(Dictionary<Point, int> elevation)
		{
			this.elevation = elevation;
		}

		public int? GetElevation(Point p)
		{
			if (this.elevation.TryGetValue(p, out int elevationAtPoint))
			{
				return elevationAtPoint;
			}

			return null;
		}

		public int? GetShortestKnown(Point p)
		{
			if (this.shortest.TryGetValue(p, out int shortestKnown))
			{
				return shortestKnown;
			}

			return null;
		}

		public void SetShortestKnown(Point p, int length)
		{
			this.shortest[p] = length;
		}
	}

	private record Point(int X, int Y);
}