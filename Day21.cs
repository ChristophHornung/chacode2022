namespace Chacode2022;

internal class Day21 : DayX
{
	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput();
		bool isPart1 = part == 1;
		Dictionary<string, Monkey> monkeys = new();
		Dictionary<string, string> leftMapping = new();
		Dictionary<string, string> rightMapping = new();
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			string[] lineSplit = line.Split(':', StringSplitOptions.TrimEntries);
			Monkey monkey = new Monkey(lineSplit[0]);
			monkeys.Add(monkey.Name, monkey);
			if (monkey.Name == "humn")
			{
				monkey.IsHuman = true;
			}

			if (long.TryParse(lineSplit[1], out long number))
			{
				monkey.Number = number;
			}
			else
			{
				var opDef = lineSplit[1].Split(' ');
				leftMapping.Add(monkey.Name, opDef[0]);
				rightMapping.Add(monkey.Name, opDef[2]);
				monkey.Operation = opDef[1] switch
				{
					"+" => Operation.Add,
					"-" => Operation.Subtract,
					"/" => Operation.Divide,
					"*" => Operation.Multiply,
					_ => throw new ArgumentOutOfRangeException()
				};
			}
		}

		foreach ((string source, string target) in leftMapping)
		{
			monkeys[source].LeftInput = monkeys[target];
		}

		foreach ((string source, string target) in rightMapping)
		{
			monkeys[source].RightInput = monkeys[target];
		}

		if (isPart1)
		{
			this.ReportResult(monkeys["root"].GetNumber().ToString());
		}
		else
		{
			Monkey root = monkeys["root"];
			NumberWithX left = root.LeftInput.GetNumberX();
			NumberWithX right = root.RightInput.GetNumberX();

			this.ReportResult($"{left}|{right}");
			if (left.TimesX != 0)
			{
				this.ReportResult("X = " +
				                  ((right - new NumberWithX(left.Constant, 0)) / new NumberWithX(left.TimesX, 0))
				                  .Constant);
			}

			if (root.RightInput.GetNumberX().TimesX != 0)
			{
				this.ReportResult("X= " +
				                  ((left - new NumberWithX(right.Constant, 0)) / new NumberWithX(right.TimesX, 0))
				                  .Constant);
			}
		}
	}

	private class Monkey
	{
		public Monkey(string name)
		{
			this.Name = name;
		}

		public string Name { get; }

		public long? Number { get; set; }
		public NumberWithX? NumberX { get; set; }
		public Operation Operation { get; set; }
		public Monkey LeftInput { get; set; } = null!;
		public Monkey RightInput { get; set; } = null!;
		public bool IsHuman { get; set; }

		public long GetNumber()
		{
			if (this.Number == null)
			{
				this.Number = this.Operation switch
				{
					Operation.Add => this.LeftInput.GetNumber() + this.RightInput.GetNumber(),
					Operation.Subtract => this.LeftInput.GetNumber() - this.RightInput.GetNumber(),
					Operation.Divide => this.LeftInput.GetNumber() / this.RightInput.GetNumber(),
					Operation.Multiply => this.LeftInput.GetNumber() * this.RightInput.GetNumber(),
					_ => throw new ArgumentOutOfRangeException()
				};
			}

			return this.Number.Value;
		}

		public NumberWithX GetNumberX()
		{
			if (this.IsHuman)
			{
				return new NumberWithX(0, 1);
			}

			if (this.NumberX == null)
			{
				if (this.Number != null)
				{
					this.NumberX = new NumberWithX(this.Number.Value, 0);
				}
				else
				{
					this.NumberX = this.Operation switch
					{
						Operation.Add => this.LeftInput.GetNumberX() + this.RightInput.GetNumberX(),
						Operation.Subtract => this.LeftInput.GetNumberX() - this.RightInput.GetNumberX(),
						Operation.Divide => this.LeftInput.GetNumberX() / this.RightInput.GetNumberX(),
						Operation.Multiply => this.LeftInput.GetNumberX() * this.RightInput.GetNumberX(),
						_ => throw new ArgumentOutOfRangeException()
					};
				}
			}

			return this.NumberX.Value;
		}
	}

	internal enum Operation
	{
		Add,
		Subtract,
		Divide,
		Multiply
	}

	private readonly record struct NumberWithX(double Constant, double TimesX)
	{
		public static NumberWithX operator +(NumberWithX a, NumberWithX b) =>
			new NumberWithX(a.Constant + b.Constant, a.TimesX + b.TimesX);

		public static NumberWithX operator -(NumberWithX a, NumberWithX b) =>
			new NumberWithX(a.Constant - b.Constant, a.TimesX - b.TimesX);

		public static NumberWithX operator /(NumberWithX a, NumberWithX b)
		{
			if (b.TimesX != 0 && a.TimesX != 0)
			{
				throw new InvalidOperationException();
			}

			if (b.TimesX == 0)
			{
				return new NumberWithX(a.Constant / b.Constant, a.TimesX / b.Constant);
			}
			else
			{
				return new NumberWithX(a.Constant / b.Constant, a.Constant / b.TimesX);
			}
		}

		public static NumberWithX operator *(NumberWithX a, NumberWithX b)
		{
			if (b.TimesX != 0 && a.TimesX != 0)
			{
				throw new InvalidOperationException();
			}

			if (b.TimesX == 0)
			{
				return new NumberWithX(a.Constant * b.Constant, a.TimesX * b.Constant);
			}
			else
			{
				return new NumberWithX(a.Constant * b.Constant, a.Constant * b.TimesX);
			}
		}

		public override string ToString()
		{
			return $"{this.Constant} + {this.TimesX}*x";
		}
	}
}