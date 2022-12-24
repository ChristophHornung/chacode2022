namespace Chacode2022;

using System.Numerics;

internal class Day24 : DayX
{
	private static int fullPeriod;
	private static Dictionary<(int x, int y, int p), long> shortestPath = new();
	private static Dictionary<(int x, int y, int p), long> shortestPathFromFinish = new();

	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput();
		bool isPart1 = part == 1;
		int y = 0;
		string line = reader.ReadLine()!;
		HashSet<Blizzard> blizzards = new();

		int width = line.Length - 2;

		while (!reader.EndOfStream)
		{
			line = reader.ReadLine()!;
			if (line[2] == '#')
			{
				break;
			}

			int x = 0;
			foreach (char c in line.Skip(1).Where(c => c != '#'))
			{
				Blizzard? blizzard = null;
				switch (c)
				{
					case '<':
						blizzard = new Blizzard(Direction.Left, x, y);
						break;
					case '>':
						blizzard = new Blizzard(Direction.Right, x, y);
						break;
					case 'v':
						blizzard = new Blizzard(Direction.Down, y, x);
						break;
					case '^':
						blizzard = new Blizzard(Direction.Up, y, x);
						break;
					case '.':
						break;
					default: throw new InvalidOperationException();
				}

				if (blizzard != null)
				{
					blizzards.Add(blizzard.Value);
				}

				x++;
			}

			y++;
		}

		int height = y;
		ILookup<int, Blizzard> upDownBlizzardsByLine =
			blizzards.Where(b => b.Direction is Direction.Up or Direction.Down).ToLookup(l => l.Line);
		ILookup<int, Blizzard> leftRightBlizzardsByLine =
			blizzards.Where(b => b.Direction is Direction.Left or Direction.Right).ToLookup(l => l.Line);

		HashSet<(int x, int y, int p)> freePeriods =
			this.GetFreePeriods(upDownBlizzardsByLine, leftRightBlizzardsByLine, width, height);


		this.FillDistances(freePeriods, width, height, true);
		this.FillDistances(freePeriods, width, height, false);

		long shortest = Day24.shortestPath[(0, -1, 0)];
		int shortestPeriodFinish = (int)(shortest % Day24.fullPeriod);

		this.ReportResult(shortest);

		long backToStartTime = Day24.shortestPathFromFinish[(width - 1, height, shortestPeriodFinish)];

		int backToStartPeriodFinish = (int)((shortestPeriodFinish + backToStartTime) % Day24.fullPeriod);

		long backToFinishTime = Day24.shortestPath[(0, -1, backToStartPeriodFinish)];

		this.ReportResult(backToStartTime);
		this.ReportResult(backToFinishTime);
		this.ReportResult(shortest + backToStartTime + backToFinishTime);
	}

	private void FillDistances(HashSet<(int x, int y, int p)> freePeriods, int width, int height, bool fromStart)
	{
		Queue<(int x, int y, int p, int pathLength)> calculatedPositions = new();
		if (fromStart)
		{
			for (int i = 0; i < Day24.fullPeriod; i++)
			{
				calculatedPositions.Enqueue((width - 1, height, i, 0));
			}
		}
		else
		{
			for (int i = 0; i < Day24.fullPeriod; i++)
			{
				calculatedPositions.Enqueue((0, -1, i, 0));
			}
		}

		while (calculatedPositions.Count > 0)
		{
			//Calculate neighbors
			(int x, int y, int p, int pathLength) = calculatedPositions.Dequeue();

			int previousPeriod = (p - 1).RealMod(Day24.fullPeriod);

			foreach (var previous in new (int x, int y, int p)[]
			         {
				         (x, y, previousPeriod),
				         (x + 1, y, previousPeriod),
				         (x - 1, y, previousPeriod),
				         (x, y + 1, previousPeriod),
				         (x, y - 1, previousPeriod),
			         })
			{
				bool shorter = false;
				if (freePeriods.Contains(previous))
				{
					Dictionary<(int x, int y, int p), long> shortestLookup =
						fromStart ? Day24.shortestPath : Day24.shortestPathFromFinish;
					if (shortestLookup.TryGetValue(previous, out var distance))
					{
						if (distance > pathLength + 1)
						{
							shortestLookup[previous] = pathLength + 1;
							shorter = true;
						}
					}
					else
					{
						shortestLookup[previous] = pathLength + 1;
						shorter = true;
					}
				}

				if (shorter)
				{
					// We are shorter than any previous path
					// Queue neighbor calculation
					calculatedPositions.Enqueue((previous.x, previous.y, previous.p, pathLength + 1));
				}
			}
		}
	}

	private HashSet<(int x, int y, int p)> GetFreePeriods(ILookup<int, Blizzard> upDownBlizzardsByLine,
		ILookup<int, Blizzard> leftRightBlizzardsByLine, int width, int height)
	{
		HashSet<(int x, int y, int p)> freePeriods = new();
		Day24.fullPeriod = MathHelper.Lcm(width, height);

		for (int p = 0; p < Day24.fullPeriod; p++)
		{
			freePeriods.Add(new(0, -1, p));
			freePeriods.Add(new(width - 1, height, p));
		}

		for (int p = 0; p < Day24.fullPeriod; p++)
		for (int x = 0; x < width; x++)
		for (int y = 0; y < height; y++)
		{
			bool blocked = false;
			foreach (Blizzard blizzard in upDownBlizzardsByLine[x].Concat(leftRightBlizzardsByLine[y]))
			{
				switch (blizzard.Direction)
				{
					case Direction.Up:
						if ((blizzard.Period - p).RealMod(height) == y)
						{
							blocked = true;
						}

						break;
					case Direction.Down:
						if ((blizzard.Period + p).RealMod(height) == y)
						{
							blocked = true;
						}

						break;
					case Direction.Left:
						if ((blizzard.Period - p).RealMod(width) == x)
						{
							blocked = true;
						}

						break;
					case Direction.Right:
						if ((blizzard.Period + p).RealMod(width) == x)
						{
							blocked = true;
						}

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				if (blocked)
				{
					break;
				}
			}

			if (!blocked)
			{
				freePeriods.Add((x, y, p));
			}
		}

		return freePeriods;
	}

	private record struct Blizzard(Direction Direction, int Period, int Line);

	private enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}
}

internal static class MathHelper
{
	internal static T1 RealMod<T1>(this T1 value, T1 mod) where T1 : INumber<T1>
	{
		T1 result = ((value % mod) + mod) % mod;
		return result;
	}

	internal static int Gfc(int a, int b)
	{
		while (b != 0)
		{
			int temp = b;
			b = a % b;
			a = temp;
		}

		return a;
	}

	internal static int Lcm(int a, int b)
	{
		return (a / MathHelper.Gfc(a, b)) * b;
	}
}