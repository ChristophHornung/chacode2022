namespace Chacode2022;

internal class Day23 : DayX
{
	private static void WriteBoard(Dictionary<Position, Elf> elves)
	{
		Console.Clear();
		for (long yPos = 0;
		     yPos <= elves.Select(e => e.Value.Position.Y).Max();
		     yPos++)
		{
			for (long xPos = 0;
			     xPos <= elves.Select(e => e.Value.Position.X).Max();
			     xPos++)
			{
				if (elves.ContainsKey(new Position(xPos, yPos)))
				{
					Console.Write('#');
				}
				else
				{
					Console.Write('.');
				}
			}

			Console.WriteLine();
		}

		Console.ReadKey();
	}

	public void Solve(int part)
	{
		Direction direction = Direction.N;
		using StreamReader reader = this.GetInput();
		bool isPart1 = part == 1;
		Dictionary<Position, Elf> elves = new();
		int y = 0;
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			int x = 0;
			foreach (char c in line)
			{
				if (c == '#')
				{
					elves.Add(new Position(x, y), new Elf(new Position(x, y)));
				}

				x++;
			}

			y++;
		}

		bool moved = true;
		int roundCount = 0;
		while (isPart1 ? roundCount < 10 : moved)
		{
			Dictionary<Position, List<Elf>> targets = new();
			moved = false;
			foreach (Elf elf in elves.Values)
			{
				Position[] check = new[]
				{
					elf.Position.Get(Direction.N),
					elf.Position.Get(Direction.N).Get(Direction.E),
					elf.Position.Get(Direction.E),
					elf.Position.Get(Direction.E).Get(Direction.S),
					elf.Position.Get(Direction.S),
					elf.Position.Get(Direction.S).Get(Direction.W),
					elf.Position.Get(Direction.W),
					elf.Position.Get(Direction.W).Get(Direction.N),
				};

				if (check.All(s => !elves.ContainsKey(s)))
				{
					continue;
				}

				bool foundTarget = false;
				for (int i = 0; i < 4; i++)
				{
					if (foundTarget)
					{
						break;
					}

					switch ((Direction)(((int)direction + i) % 4))
					{
						case Direction.N:
							Position target = elf.Position.Get(Direction.N);
							if (!elves.ContainsKey(target) &&
							    !elves.ContainsKey(target.Get(Direction.E)) &&
							    !elves.ContainsKey(target.Get(Direction.W)))
							{
								if (!targets.TryGetValue(target, out var list))
								{
									list = new List<Elf>();
									targets[target] = list;
								}

								targets[target].Add(elf);
								foundTarget = true;
							}

							break;
						case Direction.S:
							target = elf.Position.Get(Direction.S);
							if (!elves.ContainsKey(target) &&
							    !elves.ContainsKey(target.Get(Direction.E)) &&
							    !elves.ContainsKey(target.Get(Direction.W)))
							{
								if (!targets.TryGetValue(target, out var list))
								{
									list = new List<Elf>();
									targets[target] = list;
								}

								targets[target].Add(elf);
								foundTarget = true;
							}


							break;
						case Direction.W:
							target = elf.Position.Get(Direction.W);
							if (!elves.ContainsKey(target) &&
							    !elves.ContainsKey(target.Get(Direction.N)) &&
							    !elves.ContainsKey(target.Get(Direction.S)))
							{
								if (!targets.TryGetValue(target, out var list))
								{
									list = new List<Elf>();
									targets[target] = list;
								}

								targets[target].Add(elf);
								foundTarget = true;
							}

							break;
						case Direction.E:
							target = elf.Position.Get(Direction.E);
							if (!elves.ContainsKey(target) &&
							    !elves.ContainsKey(target.Get(Direction.N)) &&
							    !elves.ContainsKey(target.Get(Direction.S)))
							{
								if (!targets.TryGetValue(target, out var list))
								{
									list = new List<Elf>();
									targets[target] = list;
								}

								targets[target].Add(elf);
								foundTarget = true;
							}

							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}

			foreach ((Position target, List<Elf> movingElf) in targets.Where(v => v.Value.Count == 1))
			{
				elves.Remove(movingElf[0].Position);
				movingElf[0].Position = target;
				elves[target] = movingElf[0];
				moved = true;
			}

			roundCount++;
			direction = (Direction)(((int)direction + 1) % 4);

			//Day23.WriteBoard(elves);
		}

		long width =
			Math.Abs(elves.Select(e => e.Value.Position.X).Min() - elves.Select(e => e.Value.Position.X).Max()) + 1;
		long height =
			Math.Abs(elves.Select(e => e.Value.Position.Y).Min() - elves.Select(e => e.Value.Position.Y).Max()) + 1;
		long result = width * height;
		result -= elves.Count;
		if (isPart1)
		{
			this.ReportResult(result);
		}
		else
		{
			this.ReportResult(roundCount);
		}
	}

	private class Elf
	{
		public Elf(Position position)
		{
			this.Position = position;
		}

		public Position Position { get; set; }
	}

	private record struct Position(long X, long Y)
	{
		public Position Get(Direction direction)
		{
			return direction switch
			{
				Direction.N => this with { Y = this.Y - 1 },
				Direction.S => this with { Y = this.Y + 1 },
				Direction.W => this with { X = this.X - 1 },
				Direction.E => this with { X = this.X + 1 },
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}
	};

	private enum Direction
	{
		N,
		S,
		W,
		E
	}
}