using System.Diagnostics;

namespace Chacode2022;

internal class Day17 : DayX
{
	private static bool part1;

	public void Solve(int part)
	{
		part1 = part == 1;
		using var reader = GetInput();
		var line = reader.ReadLine()!;
		var winds = line.Select(l => l == '<' ? -1 : 1).ToList();

		List<Shape> shapes = new()
		{
			new Minus(),
			new Plus(),
			new MirrorL(),
			new Rod(),
			new Block()
		};

		Chamber chamber = new();
		var shapesCount = part1 ? 2022 : 1_000_000_000_000;
		var windGenerator = new WindGenerator(winds);
		var sw = Stopwatch.StartNew();
		for (long i = 0; i < shapesCount; i++)
		{
			if (i % 10_000_000 == 0 && i != 0) Console.WriteLine(sw.Elapsed * (1_000_000_000_000 / (double) i));

			PlaceShape(shapes[(int) (i % shapes.Count)], chamber, windGenerator);
		}

		ReportResult($"{chamber.HighestRock + 1}");
	}

	private void PlaceShape(Shape shape, Chamber chamber, WindGenerator windGenerator)
	{
		var positionX = 2;
		var positionY = chamber.HighestRock + 4;
		var stopped = false;
		while (!stopped)
		{
			positionX = Blow(windGenerator, shape, chamber, positionX, positionY);
			stopped = IsStopped(shape, chamber, positionX, positionY);

			if (!stopped) positionY--;
		}

		AddRocks(shape, chamber, positionX, positionY);
	}

	private void AddRocks(Shape shape, Chamber chamber, int positionX, long positionY)
	{
		for (var y = 0; y < shape.ShapeData.Length; y++)
		{
			var shapeLine = shape.ShapeData[y];
			int shift = positionX - 2;
			if (shift >= 0)
			{
				chamber.SetRocks(positionY +y, (byte) (shapeLine >> shift));
			}
			else
			{
				shift = -shift;
				chamber.SetRocks(positionY +y, (byte) (shapeLine << shift));
			}
			
		}
	}

	private int Blow(WindGenerator windGenerator, Shape shape, Chamber chamber, int positionX, long positionY)
	{
		var wind = windGenerator.GetWind();

		if (wind == -1)
		{
			if (positionX == 0) return positionX;
			for (var y = 0; y < shape.ShapeData.Length; y++)
			{
				var shapeLine = shape.ShapeData[y];
				int shift = positionX - 2 + wind;
				if (shift >= 0)
				{
					if (chamber.CheckCollision(positionY + y, (byte) (shapeLine >> shift)))
					{
						return positionX;
					}
				}
				else
				{
					shift = -shift;
					if (chamber.CheckCollision(positionY + y, (byte) (shapeLine << shift)))
					{
						return positionX;
					}
				}
			}
		}
		else
		{
			if (positionX + shape.Width >= 7) return positionX;

			for (var y = 0; y < shape.ShapeData.Length; y++)
			{
				var shapeLine = shape.ShapeData[y];
				int shift = positionX - 2 + wind;
				if (shift >= 0)
				{
					if (chamber.CheckCollision(positionY + y, (byte) (shapeLine >> shift)))
					{
						return positionX;
					}
				}
				else
				{
					shift = -shift;
					if (chamber.CheckCollision(positionY + y, (byte) (shapeLine << shift)))
					{
						return positionX;
					}
				}
			}
		}

		return positionX + wind;
	}

	private bool IsStopped(Shape shape, Chamber chamber, int positionX, long positionY)
	{
		if (positionY == 0) return true;

		if (positionY > chamber.HighestRock + 1) return false;

		for (var y = 0; y < shape.DownCheckHeight; y++)
		{
			var shapeLine = shape.ShapeData[y];
			int shift = positionX - 2;
			if (shift >= 0)
			{
				if (chamber.CheckCollision(positionY + y - 1, (byte) (shapeLine >> shift)))
				{
					return true;
				}
			}
			else
			{
				shift = -shift;
				if (chamber.CheckCollision(positionY + y - 1, (byte) (shapeLine << shift)))
				{
					return true;
				}
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
			Winds = winds;
		}

		public int GetWind()
		{
			var wind = Winds[index];
			index++;
			if (index == Winds.Count) index = 0;

			return wind;
		}
	}

	private abstract class Shape
	{
		public int Height { get; set; }
		public int Width { get; set; }

		public int DownCheckHeight { get; set; }
		public byte[] ShapeData { get; protected set; }
	}

	private class Chamber
	{
		private long lowestRelevantRock = -1;

		private readonly CuttableList rocks = new();

		public long HighestRock { get; private set; } = -1;

		public void SetRocks(long y, byte shapeLine)
		{
			rocks[y] |= shapeLine;
			if (y > HighestRock)
			{
				HighestRock = y;
			}

			if (rocks[y] == 0b01111111)
			{
				Cleanup(y);
			}
		}

		public bool CheckCollision(long y, byte shapeLine)
		{
			return (rocks[y] & shapeLine) > 0;
		}

		private void Cleanup(long y)
		{
			rocks.CutBelow(y);
			lowestRelevantRock = y;
		}
	}

	private class Minus : Shape
	{
		public Minus()
		{
			Width = 4;
			Height = 1;
			DownCheckHeight = 1;
			ShapeData = new byte[]
			{
				0b00011110
			};
		}
	}

	private class Plus : Shape
	{
		public Plus()
		{
			Width = 3;
			Height = 3;
			DownCheckHeight = 2;
			ShapeData = new byte[]
			{
				0b00001000,
				0b00011100,
				0b00001000,
			};
		}
	}

	private class MirrorL : Shape
	{
		public MirrorL()
		{
			Width = 3;
			Height = 3;
			DownCheckHeight = 1;
			ShapeData = new byte[]
			{
				0b00011100,
				0b00000100,
				0b00000100,
			};
		}
	}

	private class Rod : Shape
	{
		public Rod()
		{
			Width = 1;
			Height = 4;
			DownCheckHeight = 1;
			ShapeData = new byte[]
			{
				0b00010000,
				0b00010000,
				0b00010000,
				0b00010000,
			};
		}
	}

	private class Block : Shape
	{
		public Block()
		{
			Width = 2;
			Height = 2;
			DownCheckHeight = 1;
			ShapeData = new byte[]
			{
				0b00011000,
				0b00011000,
			};
		}
	}
}