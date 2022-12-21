namespace Chacode2022;

using System.Diagnostics;

internal class V2Day17 : DayX
{
	private static bool part1;
	private static int windIndex;
	private static int[] winds = null!;
	private static int[][] preCalcWinds = null!;

	private static void PreCalcWinds()
	{
		V2Day17.preCalcWinds = new int[6][];
		for (int w = 1; w < 5; w++)
		{
			V2Day17.preCalcWinds[w] = new int[V2Day17.winds.Length];
			for (int i = 0; i < V2Day17.winds.Length; i++)
			{
				int p = 2;
				for (int add = 0; add < 4; add++)
				{
					int wind = V2Day17.winds[(i + add) % V2Day17.winds.Length];
					p += wind;
					p = Math.Min(6 - w + 1, Math.Max(0, p));
				}

				V2Day17.preCalcWinds[w][i] = p;
			}
		}
	}

	public void Solve(int part)
	{
		V2Day17.windIndex = 0;
		V2Day17.part1 = part == 1;
		using StreamReader reader = this.GetInput();
		string line = reader.ReadLine()!;
		V2Day17.winds = line.Select(l => l == '<' ? -1 : 1).ToArray();
		V2Day17.PreCalcWinds();

		List<Shape> shapes = new()
		{
			new Minus(),
			new Plus(),
			new MirrorL(),
			new Rod(),
			new Block()
		};

		Chamber chamber = new();
		long shapesCount = V2Day17.part1 ? 2022 : 1_000_000_000_000;
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

		this.ReportResult($"{chamber.Top.LineNumber}");
	}

	private void PlaceShape(Shape shape, Chamber chamber)
	{
		int positionX = V2Day17.preCalcWinds[shape.Width][V2Day17.windIndex];
		V2Day17.windIndex += 4;
		V2Day17.windIndex %= V2Day17.winds.Length;

		Chamber.Line? below = chamber.Top;
		bool stopped = this.IsStopped(shape, positionX, chamber.Top);

		if (!stopped)
		{
			below = below!.Next;
		}

		while (!stopped)
		{
			positionX = this.Blow(shape, positionX, below);

			stopped = this.IsStopped(shape, positionX, below);

			if (!stopped)
			{
				below = below!.Next;
			}
		}

		this.AddRocks(shape, chamber, positionX, below);
		//Console.Clear();
		//chamber.ToConsole();
	}

	private bool IsStopped(Shape shape, int positionX, Chamber.Line? line)
	{
		if (line == null)
		{
			return true;
		}

		for (int i = 0; i < shape.Height; i++)
		{
			if (line.CheckCollision(shape.ShapeDatas[positionX][shape.Height - 1 - i]))
			{
				return true;
			}

			line = line.Previous;
			if (line == null)
			{
				break;
			}
		}

		return false;
	}

	private void AddRocks(Shape shape, Chamber chamber, int positionX, Chamber.Line? lineBelow)
	{
		lineBelow = lineBelow?.Previous;
		for (int i = 0; i < shape.Height; i++)
		{
			if (lineBelow == null)
			{
				chamber.AddLine(shape.ShapeDatas[positionX][shape.Height - 1 - i]);
			}
			else
			{
				lineBelow.SetRocks(shape.ShapeDatas[positionX][shape.Height - 1 - i]);
			}

			lineBelow = lineBelow?.Previous;
		}
	}

	private int Blow(Shape shape, int positionX, Chamber.Line? lineBelow)
	{
		int wind = V2Day17.winds[V2Day17.windIndex++];
		if (V2Day17.windIndex == V2Day17.winds.Length)
		{
			V2Day17.windIndex = 0;
		}

		if (wind == -1)
		{
			if (positionX == 0)
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
		}

		if (lineBelow == null)
		{
			return positionX + wind;
		}

		for (int i = 0; i < shape.Height; i++)
		{
			lineBelow = lineBelow?.Previous;
			if (lineBelow != null && lineBelow.CheckCollision(shape.ShapeDatas[positionX + wind][shape.Height - i - 1]))
			{
				return positionX;
			}
		}

		return positionX + wind;
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
		public Line? Top { get; private set; }

		public Line AddLine(byte rocks)
		{
			Line l = new((this.Top?.LineNumber ?? 0) + 1, rocks);

			l.Next = this.Top;
			this.Top = l;
			if (l.Next != null)
			{
				l.Next.Previous = l;
			}

			return l;
		}

		public void ToConsole()
		{
			Line? l = this.Top;
			while (l != null)
			{
				l.ToConsole();
				l = l.Next;
			}

			Console.ReadKey();
		}

		internal class Line
		{
			public Line(long lineNumber, byte rocks)
			{
				this.LineNumber = lineNumber;
				this.Rocks = rocks;
			}

			public long LineNumber { get; }
			public Line? Next { get; set; }
			public Line? Previous { get; set; }
			private byte Rocks { get; set; }

			public void SetRocks(byte rocks)
			{
				this.Rocks |= rocks;
				if (this.Rocks == 0b01111111)
				{
					Line? next = this.Next;
					if (next != null)
					{
						next.Previous = null;
					}

					this.Next = null;
				}
			}

			public bool CheckCollision(byte rocks)
			{
				return (rocks & this.Rocks) > 0;
			}

			public void ToConsole()
			{
				Console.Write('|');
				byte test = 0b01000000;
				for (int i = 0; i < 7; i++)
				{
					if ((this.Rocks & (test >> i)) > 0)
					{
						Console.Write('#');
					}
					else
					{
						Console.Write('.');
					}
				}

				Console.Write('|');
				Console.WriteLine();
			}
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
			this.ShapeDatasI = this.ShapeDatas.Select(s => s[0] << 16 | s[1] << 8 | s[2]).ToArray();
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
				0b00000100,
				0b00000100,
				0b00011100
			};
			this.ShapeDatas = new[]
			{
				this.ShiftShapeData(-2), this.ShiftShapeData(-1), this.ShiftShapeData(0), this.ShiftShapeData(1),
				this.ShiftShapeData(2)
			};

			this.ShapeDatasI = this.ShapeDatas.Select(s => s[2] << 16 | s[1] << 8 | s[0]).ToArray();
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