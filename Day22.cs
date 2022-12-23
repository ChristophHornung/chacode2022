namespace Chacode2022;

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
				Tile t = new Tile(new Position(index + 1, y));
				t.Left = left!;
				left = t;
				if (t.Left != null)
				{
					t.Left.Right = t;
				}

				switch (c)
				{
					case '.':
						tiles.Add(t.Position, t);
						break;
					case '#':
						tiles.Add(t.Position, t);
						t.HasWall = true;
						break;
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

		Position pos = start.Position;

		Dictionary<Position, Face> faces = new();

		Face? leftFace = null;
		Face? topFace = null;

		while (faces.Count < 6)
		{
			Face current = new Face(pos, Day22.faceSize);
			if (leftFace != null)
			{
				current.Left = leftFace;
				current.Left.Right = current;
			}

			if (topFace != null)
			{
				current.Top = topFace;
				current.Top.Bottom = current;
			}

			faces.Add(current.Position, current);
			if (faces.Count < 6)
			{
				if (tiles.ContainsKey(pos with { X = pos.X + Day22.faceSize }))
				{
					pos = pos with { X = pos.X + Day22.faceSize };
					leftFace = current;
					topFace = null;
				}
				else
				{
					pos = tiles.Keys.Where(k => k.Y == pos.Y + Day22.faceSize).MinBy(p => p.X)!;
					topFace = current;
					leftFace = null;
				}
			}
		}

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
			foreach (Face face in faces.Values)
			{
				for (int i = 0; i < Day22.faceSize; i++)
				{
					tiles[face.GetEdgePosition(Direction.Left, i)].Left =
						tiles[face.GetNext(Direction.Left).GetEdgePosition(Direction.Right, i)];
					tiles[face.GetEdgePosition(Direction.Right, i)].Right =
						tiles[face.GetNext(Direction.Right).GetEdgePosition(Direction.Left, i)];
					tiles[face.GetEdgePosition(Direction.Top, i)].Top =
						tiles[face.GetNext(Direction.Top).GetEdgePosition(Direction.Bottom, i)];
					tiles[face.GetEdgePosition(Direction.Bottom, i)].Bottom =
						tiles[face.GetNext(Direction.Bottom).GetEdgePosition(Direction.Top, i)];
				}
			}
		}


		string path = reader.ReadLine()!;

		this.Walk(path, start);
	}

	private void Walk(string path, Tile position)
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
				int steps = int.Parse(num);
				for (int i = 0; i < steps; i++)
				{
					if (!position.GetNext(facing).HasWall)
					{
						position = position.GetNext(facing);
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
			if (!position.GetNext(facing).HasWall)
			{
				position = position.GetNext(facing);
			}
		}

		this.ReportResult($"{position} {facing}");
		this.ReportResult(position.Position.Y * 1000 + position.Position.X * 4 + (int)facing);
	}

	private void WalkV2(string path, Tile position, Dictionary<Position, Face> faces)
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
				int steps = int.Parse(num);
				for (int i = 0; i < steps; i++)
				{
					//if (!position.GetNextV2(facing, faces).HasWall)
					{
						position = position.GetNext(facing);
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
			if (!position.GetNext(facing).HasWall)
			{
				position = position.GetNext(facing);
			}
		}

		this.ReportResult($"{position} {facing}");
		this.ReportResult(position.Position.Y * 1000 + position.Position.X * 4 + (int)facing);
	}

	private class Face
	{
		private readonly int faceSize;

		public Face(Position position, int faceSize)
		{
			this.faceSize = faceSize;
			this.Position = position;
		}

		public Position Position { get; }

		public Face? Left { get; set; }
		public Face? Right { get; set; }
		public Face? Top { get; set; }
		public Face? Bottom { get; set; }

		public Position GetEdgePosition(Direction direction, int offset)
		{
			return direction switch
			{
				Direction.Left => this.Position with
				{
					Y = this.Position.Y + offset
				},
				Direction.Right => new Position(
					Y: this.Position.Y + offset,
					X: this.Position.X + this.faceSize - 1),
				Direction.Top => this.Position with
				{
					X = this.Position.X + offset
				},
				Direction.Bottom => new Position(
					X: this.Position.X + offset,
					Y: this.Position.Y + this.faceSize - 1),
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}

		public Face GetNext(Direction direction)
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
		public Tile Right { get; set; }
		public Tile Top { get; set; }
		public Tile Bottom { get; set; }

		public Tile GetNext(Direction direction)
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

		public override string ToString()
		{
			return this.Position.ToString();
		}

		//public (Position position, Direction facing) GetNextV2(Direction facing, Dictionary<Position, Face> faces)
		//{
		//	Position next = this.GetNext(facing).Position;
		//	if (next == null)
		//	{
		//		// we have to move to a different face
		//		Face current =
		//			faces[
		//				new Position(this.Position.X - (this.Position.X % Day22.faceSize),
		//					this.Position.Y - (this.Position.Y % Day22.faceSize))];
		//		switch (facing)
		//		{
		//			case Direction.Right:
		//				return 
		//					(
		//						this.WalkFaces(current,this.Position,Direction.Left,Direction.Left,Direction.Left,Direction.Right,false)??
		//						this.WalkFaces(current,this.Position,Direction.Top,Direction.Right,Direction.Bottom,Direction.Top,false)??
		//						this.WalkFaces(current,this.Position,Direction.Left,Direction.Left,Direction.Left,Direction.Right,false)??
		//						this.WalkFaces(current,this.Position,Direction.Left,Direction.Left,Direction.Left,Direction.Right,false)
		//					 )!.Value;
		//				break;
		//			case Direction.Bottom:
		//				break;
		//			case Direction.Left:
		//				if (current.Right?.Right != null)
		//				{
		//					next = current.Right.Right.GetEdgePosition(Direction.Right, this.Position.Y % Day22.faceSize);
		//				}
		//				else if (current.Bottom?.Left != null)
		//				{
		//					next = current.Bottom.Left.GetEdgePosition(Direction.Top, this.Position.Y % Day22.faceSize);
		//					facing = Direction.Bottom;
		//				}
		//				else if (current.Top?.Left != null)
		//				{
		//					next = current.Top.Left.GetEdgePosition(Direction.Bottom,
		//						Day22.faceSize - (this.Position.Y % Day22.faceSize));
		//					facing = Direction.Top;
		//				}
		//				break;
		//			case Direction.Top:
		//				break;
		//			default:
		//				throw new ArgumentOutOfRangeException(nameof(facing), facing, null);
		//		}
		//	}

		//	return (facing, next);
		//}

		private (Position next, Direction facing)? WalkFaces(Face face, Position p, Direction a, Direction b, Direction enteringFaceAt, Direction facingTo, bool reverseOrder)
		{
			int index;
			if (a is Direction.Left or Direction.Right)
			{
				index = p.Y % Day22.faceSize;
			}
			else
			{
				index = p.X % Day22.faceSize;
			}

			if (reverseOrder)
			{
				index = 50 - index;
			}

			Face? targetFace = face.GetNext(a)?.GetNext(b);
			if (targetFace != null)
			{
				return (targetFace.GetEdgePosition(enteringFaceAt, index), facingTo);
			}

			return null;
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
	public static Day22.Direction GetOpposite(this Day22.Direction direction)
	{
		return (Day22.Direction)(direction + 2 % 4);
	}

	public static Day22.Direction Clockwise(this Day22.Direction direction)
	{
		return (Day22.Direction)(direction + 1 % 4);
	}

	public static Day22.Direction AntiClockwise(this Day22.Direction direction)
	{
		return (Day22.Direction)(direction - 1 % 4);
	}
}