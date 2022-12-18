using System.Diagnostics;

namespace Chacode2022;

internal class Day17 : DayX
{
	private static bool part1;

	public void Solve(int part)
	{
		Day17.part1 = part == 1;
		using StreamReader reader = this.GetInput();
		string line = reader.ReadLine()!;
		int[] winds = line.Select(l => l == '<' ? -1 : 1).ToArray();

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
		WindGenerator windGenerator = new WindGenerator(winds);
		Stopwatch sw = Stopwatch.StartNew();
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
		int positionX = 2;
		long positionY = chamber.HighestRock + 4;
		bool stopped = false;
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
		for (int y = 0; y < shape.ShapeData.Length; y++)
		{
			byte shapeLine = shape.ShapeData[y];
			int shift = positionX - 2;
			if (shift >= 0)
			{
				chamber.SetRocks(positionY + y, (byte)(shapeLine >> shift));
			}
			else
			{
				shift = -shift;
				chamber.SetRocks(positionY + y, (byte)(shapeLine << shift));
			}
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

			for (int y = 0; y < shape.ShapeData.Length; y++)
			{
				byte shapeLine = shape.ShapeData[y];
				int shift = positionX - 2 + wind;
				if (shift >= 0)
				{
					if (chamber.CheckCollision(positionY + y, (byte)(shapeLine >> shift)))
					{
						return positionX;
					}
				}
				else
				{
					shift = -shift;
					if (chamber.CheckCollision(positionY + y, (byte)(shapeLine << shift)))
					{
						return positionX;
					}
				}
			}
		}
		else
		{
			if (positionX + shape.Width >= 7)
			{
				return positionX;
			}

			for (int y = 0; y < shape.ShapeData.Length; y++)
			{
				byte shapeLine = shape.ShapeData[y];
				int shift = positionX - 2 + wind;
				if (shift >= 0)
				{
					if (chamber.CheckCollision(positionY + y, (byte)(shapeLine >> shift)))
					{
						return positionX;
					}
				}
				else
				{
					shift = -shift;
					if (chamber.CheckCollision(positionY + y, (byte)(shapeLine << shift)))
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
		if (positionY == 0)
		{
			return true;
		}

		if (positionY > chamber.HighestRock + 1)
		{
			return false;
		}

		for (int y = 0; y < shape.DownCheckHeight; y++)
		{
			byte shapeLine = shape.ShapeData[y];
			int shift = positionX - 2;
			if (shift >= 0)
			{
				if (chamber.CheckCollision(positionY + y - 1, (byte)(shapeLine >> shift)))
				{
					return true;
				}
			}
			else
			{
				shift = -shift;
				if (chamber.CheckCollision(positionY + y - 1, (byte)(shapeLine << shift)))
				{
					return true;
				}
			}
		}

		return false;
	}

	private class WindGenerator
	{
		private int[] Winds { get; }
		private int index;

		public WindGenerator(int[] winds)
		{
			this.Winds = winds;
		}

		public int GetWind()
		{
			int wind = this.Winds[this.index];
			this.index++;
			if (this.index == this.Winds.Length)
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
			this.rocks[y] |= shapeLine;
			if (y > this.HighestRock)
			{
				this.HighestRock = y;
			}

			if (this.rocks[y] == 0b01111111)
			{
				this.Cleanup(y);
			}
		}

		public bool CheckCollision(long y, byte shapeLine)
		{
			return (this.rocks[y] & shapeLine) > 0;
		}

		private void Cleanup(long y)
		{
			this.rocks.CutBelow(y);
			this.lowestRelevantRock = y;
		}
	}

	private class Minus : Shape
	{
		public Minus()
		{
			this.Width = 4;
			this.Height = 1;
			this.DownCheckHeight = 1;
			this.ShapeData = new byte[]
			{
				0b00011110
			};
		}
	}

	private class Plus : Shape
	{
		public Plus()
		{
			this.Width = 3;
			this.Height = 3;
			this.DownCheckHeight = 2;
			this.ShapeData = new byte[]
			{
				0b00001000,
				0b00011100,
				0b00001000
			};
		}
	}

	private class MirrorL : Shape
	{
		public MirrorL()
		{
			this.Width = 3;
			this.Height = 3;
			this.DownCheckHeight = 1;
			this.ShapeData = new byte[]
			{
				0b00011100,
				0b00000100,
				0b00000100
			};
		}
	}

	private class Rod : Shape
	{
		public Rod()
		{
			this.Width = 1;
			this.Height = 4;
			this.DownCheckHeight = 1;
			this.ShapeData = new byte[]
			{
				0b00010000,
				0b00010000,
				0b00010000,
				0b00010000
			};
		}
	}

	private class Block : Shape
	{
		public Block()
		{
			this.Width = 2;
			this.Height = 2;
			this.DownCheckHeight = 1;
			this.ShapeData = new byte[]
			{
				0b00011000,
				0b00011000
			};
		}
	}
}