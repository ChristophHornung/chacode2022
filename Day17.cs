namespace Chacode2022;

using System.Diagnostics;

internal class Day17 : DayX
{
	private static bool part1;

	public void Solve(int part)
	{
		Day17.part1 = part == 1;
		using StreamReader reader = this.GetInput();
		string line = reader.ReadLine()!;
		List<int> winds = line.Select(l => l == '<' ? -1 : 1).ToList();

		List<Shape> shapes = new()
		{
			new Minus(),
			new Plus(),
			new MirrorL(),
			new Rod(),
			new Block()
		};

		Chamber chamber = new();
		long shapesCount = Day17.part1 ? 2022 : 1_000_000_000_000;
		var windGenerator = new WindGenerator(winds);
		var sw = Stopwatch.StartNew();
		for (long i = 0; i < shapesCount; i++)
		{
			if (i % 10_000_000 == 0 && i != 0)
			{
				Console.WriteLine(sw.Elapsed * (1_000_000_000_000 / (double)i));
			}

			this.PlaceShape(shapes[(int)(i % shapes.Count)], chamber, windGenerator);
		}

		this.ReportResult($"{chamber.HighestRock + 1}");
	}

	private void PlaceShape(Shape shape, Chamber chamber, WindGenerator windGenerator)
	{
		var positionX = 2;
		long positionY = chamber.HighestRock + 4;
		var stopped = false;
		while (!stopped)
		{
			positionX = this.Blow(windGenerator, shape, chamber, positionX, positionY);
			stopped = this.IsStopped(shape, chamber, positionX, positionY);

			if (!stopped)
			{
				positionY--;
			}
		}

		this.AddRocks(shape, chamber, positionX, positionY);
	}

	private void AddRocks(Shape shape, Chamber chamber, int positionX, long positionY)
	{
		foreach((int x, int y) in shape.Rocks)
		{
			chamber.SetRock(positionX + x, positionY + y);
		}
	}

	private int Blow(WindGenerator windGenerator, Shape shape, Chamber chamber, int positionX, long positionY)
	{
		int wind = windGenerator.GetWind();

		if (wind == -1)
		{
			if (positionX == 0)
			{
				return positionX;
			}

			foreach ((int x, int y) in shape.LeftBlowChecks)
			{
				if (chamber.HasRock(positionX + x + wind, positionY + y))
				{
					return positionX;
				}
			}
		}
		else
		{
			if (positionX + shape.Width >= 7)
			{
				return positionX;
			}

			foreach ((int x, int y) in shape.RightBlowChecks)
			{
				if (chamber.HasRock(positionX + x + wind, positionY + y))
				{
					return positionX;
				}
			}
		}

		return positionX + wind;
	}

	private bool IsStopped(Shape shape, Chamber chamber, int positionX, long positionY)
	{
		if (positionY == 0)
		{
			return true;
		}

		if (positionY > chamber.HighestRock + 1)
		{
			return false;
		}

		foreach ((int x, int y) in shape.DownBlowChecks)
		{
			if (chamber.HasRock(positionX + x, positionY + y - 1))
			{
				return true;
			}
		}

		return false;
	}

	private class WindGenerator
	{
		private List<int> Winds { get; }
		private int index;

		public WindGenerator(List<int> winds)
		{
			this.Winds = winds;
		}

		public int GetWind()
		{
			int wind = this.Winds[this.index];
			this.index++;
			if (this.index == this.Winds.Count)
			{
				this.index = 0;
			}

			return wind;
		}
	}

	private abstract class Shape
	{
		public int Height { get; set; }
		public int Width { get; set; }
		public abstract bool HasRockAt(int x, int y);
		public (int x, int y)[] LeftBlowChecks { get; protected set; }
		public (int x, int y)[] RightBlowChecks { get; protected set; }
		public (int x, int y)[] DownBlowChecks { get; protected set; }
		public (int x, int y)[] Rocks { get; protected set; }
	}

	private class Chamber
	{
		private long lowestRelevantRock = -1;

		private readonly Dictionary<long, bool[]> rocks = new();

		public long HighestRock { get; private set; } = -1;

		public void SetRock(int x, long y)
		{
			if (!this.rocks.TryGetValue(y, out bool[]? line))
			{
				line = new bool[7];
				this.rocks[y] = line;
			}

			line[x] = true;
			if (y > this.HighestRock)
			{
				this.HighestRock = y;
				if (this.HighestRock % 1_000 == 0)
				{
					this.Cleanup();
				}
			}
		}

		public bool HasRock(int x, long y)
		{
			if (this.rocks.TryGetValue(y, out bool[]? line))
			{
				return line[x];
			}

			return false;
		}

		private void Cleanup()
		{
			// Only cleanup if too high
			if (this.HighestRock - this.lowestRelevantRock > 1000)
			{
				long fullLine = this.HighestRock;

				// Search for complete line
				for (long y = this.HighestRock - 1000; y > this.lowestRelevantRock; y--)
				{
					if (this.rocks.TryGetValue(y, out bool[]? line) && line[0] && line[1] && line[2] && line[3] &&
					    line[4] && line[5] && line[6])
					{
						fullLine = y;
						break;
					}
				}

				if (fullLine != this.HighestRock)
				{
					for (long y = this.lowestRelevantRock; y < fullLine; y++)
					{
						this.rocks.Remove(y);
					}
				}

				this.lowestRelevantRock = fullLine;
			}
		}
	}

	private class Minus : Shape
	{
		public Minus()
		{
			this.Width = 4;
			this.Height = 1;
			this.LeftBlowChecks = new (int x, int y)[] {(0, 0)};
			this.RightBlowChecks = new (int x, int y)[] {(3, 0)};
			this.DownBlowChecks = new (int x, int y)[] {(0, 0), (1, 0), (2, 0), (3, 0)};
			this.Rocks = this.DownBlowChecks;
		}

		public override bool HasRockAt(int x, int y)
		{
			return true;
		}
	}

	private class Plus : Shape
	{
		public Plus()
		{
			this.Width = 3;
			this.Height = 3;
			this.LeftBlowChecks = new (int x, int y)[] {(1, 0), (0, 1), (1, 2)};
			this.RightBlowChecks = new (int x, int y)[] {(1, 0), (2, 1), (1, 2)};
			this.DownBlowChecks = new (int x, int y)[] {(0, 1), (1, 0), (2, 1)};
			this.Rocks = new (int x, int y)[] {(0, 1), (1, 0), (1, 1), (1, 2), (2, 1)};
		}

		public override bool HasRockAt(int x, int y)
		{
			return (x != 0 || y != 0) && (x != 0 || y != 2) && (x != 2 || y != 0) && (x != 2 || y != 2);
		}
	}

	private class MirrorL : Shape
	{
		public MirrorL()
		{
			this.Width = 3;
			this.Height = 3;
			this.LeftBlowChecks = new (int x, int y)[] {(0, 0), (2, 1), (2, 2)};
			this.RightBlowChecks = new (int x, int y)[] {(2, 0), (2, 1), (2, 2)};
			this.DownBlowChecks = new (int x, int y)[] {(0, 0), (1, 0), (2, 0)};
			this.Rocks = new (int x, int y)[] {(0, 0), (1, 0), (2, 0), (2, 1), (2, 2)};
		}

		public override bool HasRockAt(int x, int y)
		{
			return y == 0 || y == 1 && x == 2 || y == 2 && x == 2;
		}
	}

	private class Rod : Shape
	{
		public Rod()
		{
			this.Width = 1;
			this.Height = 4;
			this.LeftBlowChecks = new (int x, int y)[] {(0, 0), (0, 1), (0, 2), (0,3)};
			this.RightBlowChecks = this.LeftBlowChecks;
			this.DownBlowChecks = new (int x, int y)[] {(0, 0)};
			this.Rocks = this.RightBlowChecks;
		}

		public override bool HasRockAt(int x, int y)
		{
			return true;
		}
	}

	private class Block : Shape
	{
		public Block()
		{
			this.Width = 2;
			this.Height = 2;
			this.LeftBlowChecks = new (int x, int y)[] {(0, 0), (0, 1)};
			this.RightBlowChecks = new (int x, int y)[] {(1, 0), (1, 1)};
			this.DownBlowChecks = new (int x, int y)[] {(0, 0), (1, 0)};
			this.Rocks = new (int x, int y)[] {(0, 0), (1, 0), (0, 1), (1, 1)};
		}

		public override bool HasRockAt(int x, int y)
		{
			return true;
		}
	}
}