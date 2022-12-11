﻿using System.Linq.Expressions;

namespace Chacode2022;

internal class Day11 : DayX
{
	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput(11);
		List<Monkey> monkeys = new();
		bool isPart1 = part == 1;
		while (!reader.EndOfStream)
		{
			monkeys.Add(Monkey.Parse(reader));
		}

		List<int> modulos = monkeys.Select(m => m.DivisibleTest).ToList();
		if (!isPart1)
		{
			foreach (Item item in monkeys.SelectMany(i => i.Items))
			{
				item.SwitchToModuloMode(modulos);
			}
		}

		for (int i = 0; i < (isPart1 ? 20 : 10_000); i++)
		{
			this.StartRound(monkeys, worryReductionActive: isPart1);
		}

		List<Monkey> activity = monkeys.OrderByDescending(m => m.InspectionCount).ToList();
		Console.WriteLine($"Day 11-{part} {activity[0].InspectionCount * activity[1].InspectionCount}");
	}

	private void StartRound(List<Monkey> monkeys, bool worryReductionActive)
	{
		foreach (Monkey monkey in monkeys)
		{
			monkey.StartTurn(monkeys, worryReductionActive);
		}
	}

	public class Monkey
	{
		public int Id { get; init; }
		public List<Item> Items { get; } = new();
		public int DivisibleTest { get; set; }
		public int TrueThrow { get; set; }
		public int FalseThrow { get; set; }
		public ExpressionType Operator { get; set; }
		public Expression RightSide { get; set; } = null!;
		public long InspectionCount { get; private set; }

		public static Monkey Parse(StreamReader sr)
		{
			string line = sr.ReadLine()!;
			int id = int.Parse(line[^2].ToString());
			Monkey monkey = new()
			{
				Id = id
			};

			line = sr.ReadLine()!;
			foreach (string item in line.Split(':')[1].Split(',', StringSplitOptions.TrimEntries))
			{
				monkey.Items.Add(new Item() { WorryLevel = int.Parse(item) });
			}

			line = sr.ReadLine()!;

			string operation = line.Split('=')[1].Trim();

			string[] parts = operation.Split(' ');

			ParameterExpression parameterExpression = Expression.Parameter(typeof(int), "old");

			monkey.Operator = GetOpType(parts[1]);
			monkey.RightSide = GetInput(parts[2], parameterExpression);

			// This was so nice for part 1... :_( Alas had to change it for part 2
			//Expression<Func<int, int>> operationEx = Expression.Lambda<Func<int, int>>(Expression.MakeBinary(
			//	GetOpType(parts[1]),
			//	GetInput(parts[0], parameterExpression),
			//	GetInput(parts[2], parameterExpression)), parameterExpression);

			line = sr.ReadLine()!;
			monkey.DivisibleTest = int.Parse(line.Split(' ').Last());
			line = sr.ReadLine()!;
			monkey.TrueThrow = int.Parse(line.Split(' ').Last());
			line = sr.ReadLine()!;
			monkey.FalseThrow = int.Parse(line.Split(' ').Last());

			// Last empty line;
			sr.ReadLine();
			return monkey;
		}

		private static Expression GetInput(string part, ParameterExpression parameterExpression)
		{
			return part switch
			{
				"old" => parameterExpression,
				_ => Expression.Constant(int.Parse(part))
			};
		}

		private static ExpressionType GetOpType(string part)
		{
			return part switch
			{
				"+" => ExpressionType.Add,
				"*" => ExpressionType.Multiply,
				_ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
			};
		}

		public void StartTurn(List<Monkey> monkeys, bool worryReductionActive)
		{
			foreach (Item item in this.Items.ToList())
			{
				// Lets inspect the item.
				this.InspectionCount++;

				// We modify the level.
				item.ModifyLevel(this.Operator, this.RightSide);
				if (worryReductionActive)
				{
					item.WorryLevel /= 3;
				}

				// Check divisibility and move the item to a new monkey accordingly.
				bool test = item.TestDivisible(this.DivisibleTest);
				monkeys[test ? this.TrueThrow : this.FalseThrow].Items.Add(item);
				this.Items.Remove(item);
			}
		}

		public override string ToString()
		{
			return $"{this.Id}: {this.InspectionCount}";
		}
	}

	internal class Item
	{
		private bool moduloMode;

		private readonly Dictionary<int, int> moduloValues = new();

		public int WorryLevel { get; set; }

		public void ModifyLevel(ExpressionType op, Expression rightSide)
		{
			if (this.moduloMode)
			{
				foreach (int modulo in this.moduloValues.Keys)
				{
					this.ModifyLevelModulo(modulo, op, rightSide);
				}
			}

			switch (op)
			{
				case ExpressionType.Add when rightSide is ConstantExpression cex:
					this.WorryLevel += (int)cex.Value!;
					break;
				case ExpressionType.Multiply when rightSide is ConstantExpression cexM:
					this.WorryLevel *= (int)cexM.Value!;
					break;
				case ExpressionType.Multiply when rightSide is ParameterExpression:
					this.WorryLevel *= this.WorryLevel;
					break;
				default:
					throw new InvalidOperationException();
			}
		}

		private void ModifyLevelModulo(int modulo, ExpressionType op, Expression rightSide)
		{
			switch (op)
			{
				case ExpressionType.Add when rightSide is ConstantExpression cex:
					// Add in modulo space just adds
					this.moduloValues[modulo] += (int)cex.Value!;
					break;
				case ExpressionType.Multiply when rightSide is ConstantExpression cexM:
					// (x * a) mod m = (x mod m) * (a mod m)
					this.moduloValues[modulo] *= ((int)cexM.Value! % modulo);
					break;
				case ExpressionType.Multiply when rightSide is ParameterExpression pex:
					this.moduloValues[modulo] *= (this.moduloValues[modulo] % modulo);
					break;
				default:
					throw new InvalidOperationException();
			}

			this.moduloValues[modulo] %= modulo;
		}

		public void SwitchToModuloMode(List<int> modulos)
		{
			this.moduloMode = true;
			foreach (int modulo in modulos)
			{
				if (!this.moduloValues.ContainsKey(modulo))
				{
					this.moduloValues[modulo] = this.WorryLevel % modulo;
				}
			}
		}

		public bool TestDivisible(int divisibleTest)
		{
			if (!this.moduloMode)
			{
				return this.WorryLevel % divisibleTest == 0;
			}
			else
			{
				return this.moduloValues[divisibleTest] == 0;
			}
		}
	}
}

