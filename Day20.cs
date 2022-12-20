namespace Chacode2022;

internal class Day20 : DayX
{
	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput();
		bool isPart1 = part == 1;
		List<Entry> files = new();
		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			files.Add(new Entry(long.Parse(line) * (isPart1 ? 1 : 811589153)));
		}

		LinkedList<Entry> list = new LinkedList<Entry>(files);

		int mod = (list.Count - 1);
		for (int c = 0; c < (isPart1 ? 1 : 10); c++)
		{
			foreach (Entry entry in files)
			{
				LinkedListNode<Entry> node = list.Find(entry)!;
				LinkedListNode<Entry> target = node.Next ?? list.First!;
				long steps = (entry.Value % mod + mod) % mod;
				for (int i = 0; i < steps; i++)
				{
					target = target.Next ?? list.First!;
				}

				if (target != node)
				{
					list.Remove(node);
					list.AddBefore(target, node.Value);
				}
			}
		}

		var start = list.Find(files.First(f => f.Value == 0))!;
		long result = 0;
		for (int i = 0; i < 3001; i++)
		{
			if (i % 1000 == 0)
			{
				result += start.Value.Value;
			}

			start = start.Next ?? list.First!;
		}

		this.ReportResult(result.ToString());
	}

	private class Entry
	{
		public Entry(long value)
		{
			this.Value = value;
		}

		public long Value { get; }
	}
}