namespace Chacode2022;

internal class Day8 : DayX
{
	public void Solve()
	{
		using StreamReader reader = this.GetInput();
		Dictionary<(int X, int Y), Tree> trees = reader.ReadToEnd().Split("\r\n")
			.SelectMany((r, y) => r.Select((c, x) => new Tree(x, y, int.Parse(c.ToString()))))
			.ToDictionary(c => (c.X, c.Y));

		int xMax = trees.Values.Max(c => c.X);
		int yMax = trees.Values.Max(c => c.Y);

		// For every x
		for (int x = 0; x <= xMax; x++)
		{
			// Look up
			int currentHeight = -1;
			for (int y = 0; y <= yMax; y++)
			{
				Tree tree = trees[(x, y)];
				if (tree.Height > currentHeight)
				{
					tree.Visible = true;
					currentHeight = tree.Height;
				}
			}

			// Look down
			currentHeight = -1;
			for (int y = yMax; y >= 0; y--)
			{
				Tree tree = trees[(x, y)];
				if (tree.Height > currentHeight)
				{
					tree.Visible = true;
					currentHeight = tree.Height;
				}
			}
		}

		// For every y
		for (int y = 0; y <= yMax; y++)
		{
			// Look right
			int currentHeight = -1;
			for (int x = 0; x <= xMax; x++)
			{
				Tree tree = trees[(x, y)];
				if (tree.Height > currentHeight)
				{
					tree.Visible = true;
					currentHeight = tree.Height;
				}
			}

			// Look left
			currentHeight = -1;
			for (int x = xMax; x >= 0; x--)
			{
				Tree tree = trees[(x, y)];
				if (tree.Height > currentHeight)
				{
					tree.Visible = true;
					currentHeight = tree.Height;
				}
			}
		}

		Console.WriteLine(
			$"Day 8: {trees.Values.Count(c => c.Visible)} | {trees.Values.Max(t => t.GetScenicScore(trees))}");
	}

	private class Tree
	{
		public Tree(int x, int y, int height)
		{
			this.X = x;
			this.Y = y;
			this.Height = height;
			this.Visible = false;
		}

		public int X { get; }
		public int Y { get; }
		public int Height { get; }
		public bool Visible { get; set; }

		public int GetScenicScore(Dictionary<(int X, int Y), Tree> trees)
		{
			int xMax = trees.Values.Max(c => c.X);
			int yMax = trees.Values.Max(c => c.Y);

			int scenicScore = 0;
			// Look up
			for (int y = this.Y + 1; y <= yMax; y++)
			{
				Tree tree = trees[(this.X, y)];
				scenicScore++;
				if (tree.Height >= this.Height)
				{
					break;
				}
			}

			int fullScenicScore = scenicScore;
			scenicScore = 0;

			// Look down
			for (int y = this.Y - 1; y >= 0; y--)
			{
				Tree tree = trees[(this.X, y)];
				scenicScore++;
				if (tree.Height >= this.Height)
				{
					break;
				}
			}

			fullScenicScore *= scenicScore;
			scenicScore = 0;
			// Look right
			for (int x = this.X + 1; x <= xMax; x++)
			{
				Tree tree = trees[(x, this.Y)];
				scenicScore++;
				if (tree.Height >= this.Height)
				{
					break;
				}
			}

			fullScenicScore *= scenicScore;
			scenicScore = 0;
			// Look left
			for (int x = this.X - 1; x >= 0; x--)
			{
				Tree tree = trees[(x, this.Y)];
				scenicScore++;
				if (tree.Height >= this.Height)
				{
					break;
				}
			}

			return fullScenicScore * scenicScore;
		}
	}
}