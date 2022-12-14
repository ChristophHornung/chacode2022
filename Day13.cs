using System.Text;

namespace Chacode2022;

internal class Day13 : DayX
{
	private static bool part1;

	public void Solve(int part)
	{
		part1 = part == 1;
		using var reader = GetInput(13);
		List<int> pairsInOrder = new();
		List<PaketData> all = new();
		var pair = 1;
		while (!reader.EndOfStream)
		{
			var first = PaketData.Parse(reader);
			all.Add(first);
			reader.ReadLine();

			var second = PaketData.Parse(reader);
			all.Add(second);
			reader.ReadLine();
			reader.ReadLine();

			if (first < second) pairsInOrder.Add(pair);

			pair++;
		}

		var firstMarker = PaketData.Parse(new StringReader("[[2]]"));
		var secondMarker = PaketData.Parse(new StringReader("[[6]]"));
		all.Add(firstMarker);
		all.Add(secondMarker);
		all.Sort();
		Console.WriteLine(
			$"Day 13 {pairsInOrder.Sum()} | {(all.IndexOf(firstMarker) + 1) * (all.IndexOf(secondMarker) + 1)}");
	}

	private class PaketData : IComparable<PaketData>
	{
		public PaketData(int? entry)
		{
			Entry = entry;
		}

		public PaketData(List<PaketData>? subData)
		{
			SubData = subData;
		}

		private int? Entry { get; }
		private List<PaketData>? SubData { get; }

		public int CompareTo(PaketData? other)
		{
			if (other == null) return -1;

			if (other.Entry.HasValue && Entry.HasValue) return Entry.Value - other.Entry.Value;

			var otherR = other;
			var thisR = this;
			if (SubData == null) thisR = new PaketData(new List<PaketData>(new[] {this}));

			if (other.SubData == null) otherR = new PaketData(new List<PaketData>(new[] {other}));

			for (var i = 0; i < thisR.SubData!.Count; i++)
				if (i < otherR.SubData!.Count)
				{
					var compare = thisR.SubData[i].CompareTo(otherR.SubData[i]);
					if (compare != 0) return compare;
				}
				else
				{
					return 1;
				}

			if (thisR.SubData.Count < otherR.SubData!.Count) return -1;

			return 0;
		}

		public static bool operator <(PaketData? left, PaketData? right)
		{
			return left?.CompareTo(right) < 0;
		}

		public static bool operator >(PaketData? left, PaketData? right)
		{
			return left?.CompareTo(right) > 0;
		}

		public static PaketData Parse(TextReader sr)
		{
			if (sr.Peek() == '[')
			{
				// We are a list
				sr.Read();
				List<PaketData> entries = new();
				while (sr.Peek() != ']')
				{
					// We did not end yet
					entries.Add(Parse(sr));
					if (sr.Peek() == ',') sr.Read();
				}

				// ]
				sr.Read();

				return new PaketData(entries);
			}

			StringBuilder sb = new();
			while (sr.Peek() is >= '0' and <= '9') sb.Append((char) sr.Read());

			return new PaketData(int.Parse(sb.ToString()));
		}

		public override string ToString()
		{
			if (Entry.HasValue) return Entry.Value.ToString();

			var sb = new StringBuilder();
			sb.Append('[');
			sb.Append(string.Join(',', SubData!.Select(s => s.ToString())));
			sb.Append(']');
			return sb.ToString();
		}
	}
}