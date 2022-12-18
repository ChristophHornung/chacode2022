namespace Chacode2022;

using System.Security;

internal class Day18 : DayX
{
	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput();
		bool isPart1 = part == 1;
		HashSet<Position> positions = new();
		Dictionary<Position, Voxel> voxels = new();
		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine()!.Split(',');
			var pos = new Position(int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2]));
			positions.Add(pos);
			voxels.Add(pos, new Voxel(pos));
		}

		int surfaceFaces = 0;
		List<HashSet<Voxel>> clusters = new();
		foreach (Position position in positions)
		{
			List<Position> connections = new();
			foreach (FaceSide side in Enum.GetValues<FaceSide>())
			{
				if (this.Connects(positions, position, side))
				{
					connections.Add(position.GetConnectingVoxelPosition(side));
				}
				else
				{
					surfaceFaces++;
				}
			}

			connections.Add(position);
			HashSet<Voxel>? existingCluster =
				clusters.FirstOrDefault(c => connections.Any(con => c.Contains(voxels[con])));

			if (existingCluster == null)
			{
				existingCluster = new HashSet<Voxel>();
				clusters.Add(existingCluster);
			}

			foreach (Position connection in connections)
			{
				existingCluster.Add(voxels[connection]);
			}
		}

		while (clusters.Any(c => clusters.Any(dupe => dupe != c && dupe.Any(c.Contains))))
		{
			var merge = clusters.First(c => clusters.Any(dupe => dupe != c && dupe.Any(c.Contains)));
			var with = clusters.First(c => c != merge && c.Any(merge.Contains));
			foreach (Voxel voxel in with)
			{
				merge.Add(voxel);
			}

			clusters.Remove(with);
		}

		foreach (HashSet<Voxel> cluster in clusters)
		{
			Voxel rightMost = cluster.MaxBy(p => p.Position.X)!;
			// We start on the rightmost right face. This is always outside.
			{
				this.Walk(rightMost, FaceSide.Right, positions, voxels);
			}
		}

		this.ReportResult(surfaceFaces.ToString());
		this.ReportResult(clusters.Sum(s => s.Sum(v => v.VisitedFaces.Count(f => f))).ToString());
	}

	private void Walk(Voxel position, FaceSide onFace, HashSet<Position> positions, Dictionary<Position, Voxel> voxels)
	{
		Queue<(Voxel, FaceSide)> next = new();

		next.Enqueue((position, onFace));

		while (next.Count > 0)
		{
			(position, onFace) = next.Dequeue();

			if (position.VisitedFaces[(int)onFace])
			{
				// Already visited
				continue;
			}

			position.VisitedFaces[(int)onFace] = true;
			position.Visited = true;

			HashSet<FaceSide> reachable =
				Enum.GetValues<FaceSide>().Where(s => s != onFace && s != this.GetOpposite(onFace)).ToHashSet();
			
			foreach (FaceSide faceSide in reachable)
			{
				if (this.IsWall(positions, position.Position, onFace, faceSide))
				{
					// We walk on the wall.
					next.Enqueue((voxels[position.Position.GetBlockingVoxelPosition(onFace, faceSide)], this.GetOpposite(faceSide)));
				}
				else if (this.Connects(positions, position.Position, faceSide))
				{
					// We walk to another voxel.
					next.Enqueue((voxels[position.Position.GetConnectingVoxelPosition(faceSide)], faceSide));
				}
				else
				{
					next.Enqueue((position, faceSide));
				}
			}
		}
	}

	private FaceSide GetOpposite(FaceSide side)
	{
		return side switch
		{
			FaceSide.Left => FaceSide.Right,
			FaceSide.Right => FaceSide.Left,
			FaceSide.Back => FaceSide.Front,
			FaceSide.Front => FaceSide.Back,
			FaceSide.Down => FaceSide.Up,
			FaceSide.Up => FaceSide.Down,
			_ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
		};
	}

	private bool Connects(HashSet<Position> positions, Position position, FaceSide side)
	{
		return positions.Contains(position.GetConnectingVoxelPosition(side));
	}

	private bool IsWall(HashSet<Position> positions, Position position, FaceSide onSide, FaceSide movingTo)
	{
		return positions.Contains(position.GetBlockingVoxelPosition(onSide, movingTo));
	}

	private readonly record struct Position(int X, int Y, int Z)
	{
		public Position GetConnectingVoxelPosition(FaceSide sideOfVoxel)
		{
			return sideOfVoxel switch
			{
				FaceSide.Left => this with {X = this.X - 1},
				FaceSide.Right => this with {X = this.X + 1},
				FaceSide.Back => this with {Y = this.Y + 1},
				FaceSide.Front => this with {Y = this.Y - 1},
				FaceSide.Down => this with {Z = this.Z - 1},
				FaceSide.Up => this with {Z = this.Z + 1},
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public Position GetBlockingVoxelPosition(FaceSide onSide, FaceSide movingTo)
		{
			return this.GetConnectingVoxelPosition(onSide).GetConnectingVoxelPosition(movingTo);
		}
	}

	private record Voxel(Position Position)
	{
		// Left, Right, Back, Front, Down, Up
		public bool[] VisitedFaces { get; } = new bool[6];
		public bool Visited { get; set; }
	}

	private enum FaceSide
	{
		Left = 0,
		Right = 1,
		Back = 2,
		Front = 3,
		Down = 4,
		Up = 5
	}
}