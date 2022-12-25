namespace Chacode2022;

using System.Diagnostics;

internal class Day22 : DayX
{
	public enum Direction
	{
		Right = 0,
		Bottom = 1,
		Left = 2,
		Top = 3,
	}

	private const int faceSize = 4;

	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput();
		bool isPart1 = part == 1;
		Dictionary<Position, Tile> tiles = new();
		int y = 1;
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;

			if (line == string.Empty)
			{
				break;
			}

			Tile left = null;
			for (var index = 0; index < line.Length; index++)
			{
				char c = line[index];
				if (c == ' ')
				{
					continue;
				}

				Tile t = new Tile(new Position(index + 1, y));
				t.Left = left!;
				left = t;
				if (t.Left != null)
				{
					t.Left.Right = t;
				}

				tiles.Add(t.Position, t);
				if (c == '#')
				{
					t.HasWall = true;
				}
			}

			y++;
		}

		foreach (var tile in tiles.Values)
		{
			if (tiles.TryGetValue(tile.Position with { Y = tile.Position.Y + 1 }, out var below))
			{
				tile.Bottom = below;
			}

			if (tiles.TryGetValue(tile.Position with { Y = tile.Position.Y - 1 }, out var top))
			{
				tile.Top = top;
			}
		}

		int maxY = tiles.Values.Max(t => t.Position.Y);
		int maxX = tiles.Values.Max(t => t.Position.X);

		Tile start = tiles.Values.MinBy(t => (t.Position.Y * (maxX + 1)) + t.Position.X)!;

		if (isPart1)
		{
			for (int i = 1; i <= maxY; i++)
			{
				int i1 = i;
				Tile left = tiles.Values.Where(t => t.Position.Y == i1).MinBy(t => t.Position.X)!;
				Tile right = tiles.Values.Where(t => t.Position.Y == i1).MaxBy(t => t.Position.X)!;
				left.Left = right;
				right.Right = left;
			}

			for (int i = 1; i <= maxX; i++)
			{
				int i1 = i;
				Tile top = tiles.Values.Where(t => t.Position.X == i1).MinBy(t => t.Position.Y)!;
				Tile bottom = tiles.Values.Where(t => t.Position.X == i1).MaxBy(t => t.Position.Y)!;
				top.Top = bottom;
				bottom.Bottom = top;
			}
		}
		else
		{
			// The cube is
			//     0 0 1
			//     0 5 0
			//     1 1 1
			//01     A B
			//51     C
			//101  E D
			//151  F

			for (int i = 1; i <= 50; i++)
			{
				Tile source;
				Tile target;
				{
					// A top -> F Left
					source = tiles[new Position(50 + i, 1)];
					target = tiles[new Position(1, 150 + i)];
					source.Top = target;
					source.TopResultDirection = Direction.Right;
					target.Left = source;
					target.LeftResultDirection = Direction.Bottom;
				}

				{
					// A left -> E Left Reverse
					source = tiles[new Position(51, i)];
					target = tiles[new Position(1, 151 - i)];
					source.Left = target;
					source.LeftResultDirection = Direction.Right;
					target.Left = source;
					target.LeftResultDirection = Direction.Right;
				}
				{
					// B top -> F Bottom
					source = tiles[new Position(100 + i, 1)];
					target = tiles[new Position(i, 200)];
					source.Top = target;
					source.TopResultDirection = Direction.Top;
					target.Bottom = source;
					target.BottomResultDirection = Direction.Bottom;
				}
				{
					// B right -> D Right Reverse
					source = tiles[new Position(150, i)];
					target = tiles[new Position(100, 151 - i)];
					source.Right = target;
					source.RightResultDirection = Direction.Left;
					target.Right = source;
					target.RightResultDirection = Direction.Left;
				}
				{
					// B bottom -> C Right
					source = tiles[new Position(100 + i, 50)];
					target = tiles[new Position(100, 50 + i)];
					source.Bottom = target;
					source.BottomResultDirection = Direction.Left;
					target.Right = source;
					target.RightResultDirection = Direction.Top;
				}
				{
					// D bottom -> F Right
					source = tiles[new Position(50 + i, 150)];
					target = tiles[new Position(50, 150 + i)];
					source.Bottom = target;
					source.BottomResultDirection = Direction.Left;
					target.Right = source;
					target.RightResultDirection = Direction.Top;
				}
				{
					// E top -> C Left
					source = tiles[new Position(i, 101)];
					target = tiles[new Position(51, 50 + i)];
					source.Top = tiles[new Position(51, 50 + i)];
					source.TopResultDirection = Direction.Right;
					target.Left = source;
					target.LeftResultDirection = Direction.Bottom;
				}
			}
		}

		Debug.Assert(Direction.Bottom.Clockwise() == Direction.Left);
		Debug.Assert(Direction.Left.Clockwise() == Direction.Top);
		Debug.Assert(Direction.Top.Clockwise() == Direction.Right);
		Debug.Assert(Direction.Right.Clockwise() == Direction.Bottom);

		Debug.Assert(Direction.Bottom == Direction.Left.AntiClockwise());
		Debug.Assert(Direction.Left == Direction.Top.AntiClockwise());
		Debug.Assert(Direction.Top == Direction.Right.AntiClockwise());
		Debug.Assert(Direction.Right == Direction.Bottom.AntiClockwise());

		foreach (var tile in tiles.Values)
		{
			Debug.Assert(tile.Right != null);
			Debug.Assert(tile.Top != null);
			Debug.Assert(tile.Left != null);
			Debug.Assert(tile.Bottom != null);
		}

		string path = reader.ReadLine()!;

		(Tile position, Direction facing) = this.Walk(path, start);
		Debug.Assert(this.Walk("L1L1L1", start).position == start);

		this.ReportResult($"{position} {facing}");
		this.ReportResult(position.Position.Y * 1000 + position.Position.X * 4 + (int)facing);
	}

	private (Tile position, Direction facing) Walk(string path, Tile position)
	{
		Direction facing = Direction.Right;
		string num = string.Empty;
		foreach (char c in path)
		{
			if (int.TryParse(c.ToString(), out _))
			{
				num += c;
			}
			else
			{
				if (num != string.Empty)
				{
					int steps = int.Parse(num);
					for (int i = 0; i < steps; i++)
					{
						if (!position.GetNextTile(facing).HasWall)
						{
							(position, facing) = position.GetNext(facing);
						}
					}
				}

				num = string.Empty;
				switch (c)
				{
					case 'L':
						facing = facing.AntiClockwise();
						break;
					case 'R':
						facing = facing.Clockwise();
						break;
				}
			}
		}

		int lastSteps = int.Parse(num);
		for (int i = 0; i < lastSteps; i++)
		{
			if (!position.GetNextTile(facing).HasWall)
			{
				(position, facing) = position.GetNext(facing);
			}
		}

		return (position, facing);
	}

	private class Tile
	{
		public Tile(Position position)
		{
			this.Position = position;
		}

		public Position Position { get; }
		public bool HasWall { get; set; }
		public Tile Left { get; set; }
		public Direction LeftResultDirection { get; set; } = Direction.Left;
		public Tile Right { get; set; }
		public Direction RightResultDirection { get; set; } = Direction.Right;
		public Tile Top { get; set; }
		public Direction TopResultDirection { get; set; } = Direction.Top;
		public Tile Bottom { get; set; }
		public Direction BottomResultDirection { get; set; } = Direction.Bottom;


		public (Tile tile, Direction direction) GetNext(Direction direction)
		{
			return (this.GetNextTile(direction), this.GetNextDirection(direction));
		}

		public Tile GetNextTile(Direction direction)
		{
			return direction switch
			{
				Direction.Left => this.Left,
				Direction.Right => this.Right,
				Direction.Top => this.Top,
				Direction.Bottom => this.Bottom,
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}

		public Direction GetNextDirection(Direction direction)
		{
			return direction switch
			{
				Direction.Left => this.LeftResultDirection,
				Direction.Right => this.RightResultDirection,
				Direction.Top => this.TopResultDirection,
				Direction.Bottom => this.BottomResultDirection,
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}

		public override string ToString()
		{
			return this.Position.ToString();
		}
	}

	private record struct Position(int X, int Y)
	{
		public override string ToString()
		{
			return $"{this.X}|{this.Y}";
		}
	}
}

internal static class DirectionHelper
{
	public static Day22.Direction Clockwise(this Day22.Direction direction)
	{
		return (Day22.Direction)(((int)direction + 1) % 4);
	}

	public static Day22.Direction AntiClockwise(this Day22.Direction direction)
	{
		return (Day22.Direction)(((int)direction + 3) % 4);
	}
}