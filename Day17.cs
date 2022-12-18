using System.Diagnostics;

namespace Chacode2022;

internal class Day17 : DayX
{
	private static bool part1;
	private static int windIndex;
	private static int[] winds = null!;

	public void Solve(int part)
	{
		Day17.windIndex = 0;
		Day17.part1 = part == 1;
		using StreamReader reader = this.GetInput();
		string line = reader.ReadLine()!;
		Day17.winds = line.Select(l => l == '<' ? -1 : 1).ToArray();

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
		Stopwatch sw = Stopwatch.StartNew();
		for (long i = 0; i < shapesCount; i++)
		{
			if (i % 50_000_000 == 0 && i != 0)
			{
				TimeSpan ts = sw.Elapsed * (1_000_000_000_000 / (double)i);
				Console.WriteLine($"{ts}:{DateTime.Now.Add(ts)}");
			}

			this.PlaceShape(shapes[(int)(i % shapes.Count)], chamber);
		}

		this.ReportResult($"{chamber.HighestRock + 1}");
	}

	private void PlaceShape(Shape shape, Chamber chamber)
	{
		int positionX = 2;
		long positionY = chamber.HighestRock + 4;
		bool stopped = false;
		while (!stopped)
		{
			positionX = this.Blow(shape, chamber, positionX, positionY);
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
		chamber.SetRocks(positionY,shape.ShapeDatasI[positionX], shape.Height);
		//chamber.SetRocks(positionY,shape.ShapeDatas[positionX]);
	}

	private int Blow(Shape shape, Chamber chamber, int positionX, long positionY)
	{
		int wind = Day17.winds[Day17.windIndex++];
		if (Day17.windIndex == Day17.winds.Length)
		{
			Day17.windIndex = 0;
		}

		if (wind == -1)
		{
			if (positionX == 0)
			{
				return positionX;
			}

			if (chamber.HighestRock < positionY)
			{
				return positionX + wind;
			}

			if (chamber.CheckCollision(positionY, shape.ShapeDatasI[positionX + wind]))
			{
				return positionX;
			}
		}
		else
		{
			if (positionX + shape.Width >= 7)
			{
				return positionX;
			}

			if (chamber.HighestRock < positionY)
			{
				return positionX + wind;
			}

			if (chamber.CheckCollision(positionY, shape.ShapeDatasI[positionX + wind]))
			{
				return positionX;
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

		return chamber.CheckCollision(positionY - 1, shape.ShapeDatasI[positionX]);
	}

	private abstract class Shape
	{
		public int Height { get; set; }
		public int Width { get; init; }

		public int DownCheckHeight { get; set; }
		public byte[] ShapeData { get; protected init; }
		public byte[][] ShapeDatas { get; protected init; }
		public int[] ShapeDatasI { get; protected init; }

		protected byte[] ShiftShapeData(int shift)
		{
			if (shift >= 0)
			{
				return this.ShapeData.Select(s => (byte)(s >> shift)).ToArray();
			}
			else
			{
				shift = -shift;
				return this.ShapeData.Select(s => (byte)(s << shift)).ToArray();
			}
		}
	}

	private sealed class Chamber
	{
		private readonly CuttableList rocks = new();

		public long HighestRock { get; private set; } = -1;
		public void SetRocks(long y, int shape, int shapeLines)
		{
			if (y > this.HighestRock)
			{
				this.HighestRock = y + shapeLines - 1;
				this.rocks.SetRocksDirect(y, shape, shapeLines);
				return;
			}

			if (y + shapeLines - 1 > this.HighestRock)
			{
				this.HighestRock = y + shapeLines - 1;
			}

			this.rocks.SetRocks(y, shape, shapeLines);

			if (this.rocks[y] == 0b01111111)
			{
				this.Cleanup(y);
			}
		}
		public void SetRocks(long y, byte[] shapeLines)
		{
			if (y > this.HighestRock)
			{
				this.HighestRock = y + shapeLines.Length - 1;
				this.rocks.SetRocksDirect(y, shapeLines);
				return;
			}

			if (y + shapeLines.Length - 1 > this.HighestRock)
			{
				this.HighestRock = y + shapeLines.Length - 1;
			}

			this.rocks.SetRocks(y, shapeLines);

			if (this.rocks[y] == 0b01111111)
			{
				this.Cleanup(y);
			}
		}
		public bool CheckCollision(long y, int shape)
		{
			return this.rocks.CheckCollision(y, shape);
		}

		public bool CheckCollision(long y, byte[] shapeLines)
		{
			return this.rocks.CheckCollision(y, shapeLines);
		}

		private void Cleanup(long y)
		{
			this.rocks.CutBelow(y);
		}
	}

	private sealed class Minus : Shape
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
			this.ShapeDatas = new[]
			{
				this.ShiftShapeData(-2),
				this.ShiftShapeData(-1),
				this.ShiftShapeData(0),
				this.ShiftShapeData(1)
			};
			this.ShapeDatasI = this.ShapeDatas.Select(s => (int)s[0]).ToArray();
		}
	}

	private sealed class Plus : Shape
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
			this.ShapeDatas = new[]
			{
				this.ShiftShapeData(-2), this.ShiftShapeData(-1), this.ShiftShapeData(0), this.ShiftShapeData(1),
				this.ShiftShapeData(2)
			};
			this.ShapeDatasI = this.ShapeDatas.Select(s =>  s[0] << 16 | s[1] << 8 | s[2]).ToArray();
		}
	}

	private sealed class MirrorL : Shape
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
			this.ShapeDatas = new[]
			{
				this.ShiftShapeData(-2), this.ShiftShapeData(-1), this.ShiftShapeData(0), this.ShiftShapeData(1),
				this.ShiftShapeData(2)
			};

			this.ShapeDatasI = this.ShapeDatas.Select(s =>  s[2] << 16 | s[1] << 8 | s[0]).ToArray();
		}
	}

	private sealed class Rod : Shape
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
			this.ShapeDatas = new[]
			{
				this.ShiftShapeData(-2), this.ShiftShapeData(-1), this.ShiftShapeData(0), this.ShiftShapeData(1),
				this.ShiftShapeData(2), this.ShiftShapeData(3), this.ShiftShapeData(4)
			};
			this.ShapeDatasI = this.ShapeDatas.Select(s => s[0] << 24 | s[1] << 16 | s[2] << 8 | s[3]).ToArray();
		}
	}

	private sealed class Block : Shape
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
			this.ShapeDatas = new[]
			{
				this.ShiftShapeData(-2), this.ShiftShapeData(-1), this.ShiftShapeData(0), this.ShiftShapeData(1),
				this.ShiftShapeData(2), this.ShiftShapeData(3)
			};
			this.ShapeDatasI = this.ShapeDatas.Select(s => s[0] << 8 | s[1]).ToArray();
		}
	}
}