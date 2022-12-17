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
		List<Wind> winds = line.Select(l => l == '<' ? Wind.Left : Wind.Right).ToList();

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
		for (var x = 0; x < shape.Width; x++)
		for (var y = 0; y < shape.Height; y++)
		{
			if (shape.HasRockAt(x, y))
			{
				chamber.SetRock(positionX + x, positionY + y);
			}
		}
	}

	private int Blow(WindGenerator windGenerator, Shape shape, Chamber chamber, int positionX, long positionY)
	{
		Wind wind = windGenerator.GetWind();
		int addX = wind == Wind.Left ? -1 : +1;

		if (wind == Wind.Left && positionX == 0)
		{
			return positionX;
		}

		if (wind == Wind.Right && positionX + shape.Width >= 7)
		{
			return positionX;
		}

		if (wind == Wind.Left)
		{
			for (var y = 0; y < shape.Height; y++)
			{
				for (var x = 0; x < shape.Width; x++)
				{
					if (shape.HasRockAt(x, y))
					{
						if (chamber.HasRock(positionX + x + addX, positionY + y))
						{
							return positionX;
						}

						break;
					}
				}
			}
		}
		else
		{
			for (var y = 0; y < shape.Height; y++)
			{
				for (int x = shape.Width - 1; x >= 0; x--)
				{
					if (shape.HasRockAt(x, y))
					{
						if (chamber.HasRock(positionX + x + addX, positionY + y))
						{
							return positionX;
						}

						break;
					}
				}
			}
		}

		return positionX + addX;
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

		for (var x = 0; x < shape.Width; x++)
		for (var y = 0; y < shape.Height; y++)
		{
			if (shape.HasRockAt(x, y))
			{
				if (chamber.HasRock(positionX + x, positionY + y - 1))
				{
					return true;
				}

				break;
			}
		}

		return false;
	}

	private class WindGenerator
	{
		private List<Wind> Winds { get; }
		private int index;

		public WindGenerator(List<Wind> winds)
		{
			this.Winds = winds;
		}

		public Wind GetWind()
		{
			Wind wind = this.Winds[this.index];
			this.index++;
			if (this.index == this.Winds.Count)
			{
				this.index = 0;
			}

			return wind;
		}
	}

	private enum Wind
	{
		Left,
		Right
	}

	private abstract class Shape
	{
		public int Height { get; set; }
		public int Width { get; set; }
		public abstract bool HasRockAt(int x, int y);
	}

	private class Chamber
	{
		private long lowestRelevantRock = -1;

		private readonly Dictionary<long, bool[]> rocks = new();

		public long HighestRock { get; private set; } = -1;

		public void SetRock(int x, long y)
		{
			if (!this.rocks.TryGetValue(y,out bool[]? line))
			{
				line = new bool[7];
				this.rocks[y] = line;
			}

			line[x] = true;
			if (y > this.HighestRock)
			{
				this.HighestRock = y;
				if (this.HighestRock % 1000 == 0)
				{
					this.Cleanup();
				}
			}
		}

		public bool HasRock(int x, long y)
		{
			if (this.rocks.TryGetValue(y,out bool[]? line))
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
		}

		public override bool HasRockAt(int x, int y)
		{
			return true;
		}
	}
}