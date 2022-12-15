namespace Chacode2022;

internal class Day9 : DayX
{
	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput();
		Rope r = new(part == 1 ? 1 : 9);

		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			string[] parts = line.Split(' ');
			int repeat = int.Parse(parts[1]);
			for (int i = 0; i < repeat; i++)
			{
				switch (parts[0])
				{
					case "R":
						r.Move(Direction.Right);
						break;
					case "L":
						r.Move(Direction.Left);
						break;
					case "U":
						r.Move(Direction.Up);
						break;
					case "D":
						r.Move(Direction.Down);
						break;
				}
			}
		}

		Console.WriteLine(
			$"Day 9-{part}: {r.Segments.Last().Visited.Count}");
	}

	private class Rope
	{
		public Rope(int length)
		{
			for (int i = 0; i < length; i++)
			{
				this.Segments.Add(new RopeSegment());
			}
		}

		public List<RopeSegment> Segments { get; } = new();

		public void Move(Direction direction)
		{
			this.Segments[0].Move(direction);
			Point lastTail = this.Segments[0].Tail;
			foreach (RopeSegment ropeSegment in this.Segments.Skip(1))
			{
				ropeSegment.Head = lastTail;
				ropeSegment.DragTail();
				lastTail = ropeSegment.Tail;
			}

			this.AssertCorrect();
			// this.Print();
		}

		private void Print()
		{
			Console.WriteLine(string.Join("|",
				this.Segments.Select(s => s.Head).Append(this.Segments.Last().Tail).Select(s => $"({s.X},{s.Y})")));
		}

		private void AssertCorrect()
		{
			for (int i = 0; i < this.Segments.Count - 1; i++)
			{
				if (this.Segments[i].Tail != this.Segments[i + 1].Head)
				{
					throw new InvalidOperationException();
				}
			}

			if (this.Segments.Any(s => !s.Head.Touches(s.Tail)))
			{
				throw new InvalidOperationException();
			}
		}
	}

	private class RopeSegment
	{
		public HashSet<Point> Visited { get; } = new() { new Point(0, 0) };
		public Point Head { get; set; } = new Point(0, 0);
		public Point Tail { get; private set; } = new Point(0, 0);

		public void Move(Direction direction)
		{
			this.Head = direction switch
			{
				Direction.Up => this.Head with { Y = this.Head.Y + 1 },
				Direction.Down => this.Head with { Y = this.Head.Y - 1 },
				Direction.Left => this.Head with { X = this.Head.X - 1 },
				Direction.Right => this.Head with { X = this.Head.X + 1 },
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};

			this.DragTail();
		}

		public void DragTail()
		{
			if (!this.Tail.Touches(this.Head))
			{
				if (this.Tail.Y < this.Head.Y)
				{
					this.Tail = this.Tail with { Y = this.Tail.Y + 1 };
				}
				else if (this.Tail.Y > this.Head.Y)
				{
					this.Tail = this.Tail with { Y = this.Tail.Y - 1 };
				}

				if (this.Tail.X < this.Head.X)
				{
					this.Tail = this.Tail with { X = this.Tail.X + 1 };
				}
				else if (this.Tail.X > this.Head.X)
				{
					this.Tail = this.Tail with { X = this.Tail.X - 1 };
				}

				this.Visited.Add(this.Tail);
			}
		}

		public override string ToString()
		{
			return $"({this.Head.X},{this.Head.Y})->({this.Tail.X},{this.Tail.Y})";
		}
	}

	private enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}

	internal record Point(int X, int Y);
}

internal static class PointHelper
{
	internal static bool Touches(this Day9.Point a, Day9.Point b)
	{
		int difX = Math.Abs(a.X - b.X);
		int difY = Math.Abs(a.Y - b.Y);
		return !(difX > 1 || difY > 1);
	}
}