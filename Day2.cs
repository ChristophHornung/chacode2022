namespace Chacode2022;

internal class Day2 : DayX
{
	public void Solve()
	{
		using StreamReader reader = this.GetInput(2);

		List<RoundMisunderstood> roundsMisunderstood = new();
		List<Round> rounds = new();
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!.Trim();
			string[] entries = line.Split(' ');
			Hand opponent = entries[0] switch
			{
				"A" => Hand.Rock, "B" => Hand.Paper, "C" => Hand.Scissors,
				_ => throw new ArgumentOutOfRangeException()
			};

			Hand you = entries[1] switch
			{
				"X" => Hand.Rock, "Y" => Hand.Paper, "Z" => Hand.Scissors,
				_ => throw new ArgumentOutOfRangeException()
			};

			GameResult result = entries[1] switch
			{
				"X" => GameResult.Lost, "Y" => GameResult.Draw, "Z" => GameResult.Won,
				_ => throw new ArgumentOutOfRangeException()
			};

			roundsMisunderstood.Add(new RoundMisunderstood(opponent, you));
			rounds.Add(new Round(opponent, result));
		}

		Console.WriteLine(
			$"Day 2: {roundsMisunderstood.Sum(r => r.Score)}|{rounds.Sum(r => r.Score)}");
	}

	private record Round(Hand Opponent, GameResult Result)
	{
		public int Score => (int)this.You + (int)this.Result;

		private Hand You
		{
			get
			{
				if (this.Result == GameResult.Draw)
				{
					return this.Opponent;
				}

				if (this.Opponent == Hand.Rock)
				{
					return this.Result == GameResult.Won ? Hand.Paper : Hand.Scissors;
				}

				if (this.Opponent == Hand.Paper)
				{
					return this.Result == GameResult.Won ? Hand.Scissors : Hand.Rock;
				}

				if (this.Opponent == Hand.Scissors)
				{
					return this.Result == GameResult.Won ? Hand.Rock : Hand.Paper;
				}

				throw new InvalidOperationException();
			}
		}
	}

	private record RoundMisunderstood(Hand Opponent, Hand You)
	{
		public int Score => (int)this.You + (int)this.Result;

		private GameResult Result
		{
			get
			{
				if (this.Opponent == this.You)
				{
					return GameResult.Draw;
				}

				if (this.Opponent == Hand.Rock)
				{
					return this.You == Hand.Paper ? GameResult.Won : GameResult.Lost;
				}

				if (this.Opponent == Hand.Paper)
				{
					return this.You == Hand.Scissors ? GameResult.Won : GameResult.Lost;
				}

				if (this.Opponent == Hand.Scissors)
				{
					return this.You == Hand.Rock ? GameResult.Won : GameResult.Lost;
				}

				throw new InvalidOperationException();
			}
		}
	}

	private enum GameResult
	{
		Lost = 0,
		Won = 6,
		Draw = 3
	}

	private enum Hand
	{
		Rock = 1,
		Paper = 2,
		Scissors = 3
	}
}