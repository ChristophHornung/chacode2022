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
			if (i % 1_000_000 == 0 && i != 0)
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
			if (shape.GetPixel(x, y) == Pixel.Rock)
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
					if (shape.GetPixel(x, y) == Pixel.Rock)
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
					if (shape.GetPixel(x, y) == Pixel.Rock)
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
			if (shape.GetPixel(x, y) == Pixel.Rock)
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
		public List<Wind> Winds { get; }
		private int index = 0;

		public WindGenerator(List<Wind> winds)
		{
			this.Winds = winds;
		}

		public Wind GetWind()
		{
			Wind wind = this.Winds[this.index % this.Winds.Count];
			this.index++;
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
		public abstract Pixel GetPixel(int x, int y);
	}

	private class Chamber
	{
		private HashSet<(int x, long y)> rocks = new();

		public long HighestRock { get; private set; } = -1;

		public void SetRock(int x, long y)
		{
			this.rocks.Add((x, y));
			if (y > this.HighestRock)
			{
				this.HighestRock = y;
			}
		}

		public bool HasRock(int x, long y)
		{
			return this.rocks.Contains((x, y));
		}
	}

	private class Minus : Shape
	{
		public Minus()
		{
			this.Width = 4;
			this.Height = 1;
		}

		public override Pixel GetPixel(int x, int y)
		{
			return Pixel.Rock;
		}
	}

	private class Plus : Shape
	{
		public Plus()
		{
			this.Width = 3;
			this.Height = 3;
		}

		public override Pixel GetPixel(int x, int y)
		{
			switch (x)
			{
				case 0 when y == 0:
				case 0 when y == 2:
				case 2 when y == 0:
				case 2 when y == 2:
					return Pixel.Free;
				default:
					return Pixel.Rock;
			}
		}
	}

	private class MirrorL : Shape
	{
		public MirrorL()
		{
			this.Width = 3;
			this.Height = 3;
		}

		public override Pixel GetPixel(int x, int y)
		{
			switch (y)
			{
				case 0:
				case 1 when x == 2:
				case 2 when x == 2:
					return Pixel.Rock;
				default:
					return Pixel.Free;
			}
		}
	}

	private class Rod : Shape
	{
		public Rod()
		{
			this.Width = 1;
			this.Height = 4;
		}

		public override Pixel GetPixel(int x, int y)
		{
			return Pixel.Rock;
		}
	}

	private class Block : Shape
	{
		public Block()
		{
			this.Width = 2;
			this.Height = 2;
		}

		public override Pixel GetPixel(int x, int y)
		{
			return Pixel.Rock;
		}
	}

	internal enum Pixel
	{
		Free,
		Rock
	}
}