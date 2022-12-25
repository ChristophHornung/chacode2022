namespace Chacode2022;

using System.Text;

internal class Day25 : DayX
{
	public void Solve(int part)
	{
		using StreamReader reader = this.GetInput();
		bool isPart1 = part == 1;
		int y = 0;
		List<long> inputs = new();

		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine()!;
			inputs.Add(this.SnafuToDecimal(line.Select(this.SnafuCharToSByte).ToList()));
		}

		long sumFuel = inputs.Sum();
		ReportResult(PrintSnafu(DecimalToSnafu(sumFuel)));

		ReportResult(string.Join(',',
			"MERRY CHRISTMAS".Select(c => (int)c).Select(i => DecimalToSnafu(i)).Select(s => PrintSnafu(s))));
	}

	private string PrintSnafu(IList<sbyte> snafu)
	{
		StringBuilder sb = new();
		for (var index = snafu.Count - 1; index >= 0; index--)
		{	
			sbyte s = snafu[index];
			sb.Append(
				s switch
				{
					0 => '0',
					1 => '1',
					2 => '2',
					-1 => '-',
					-2 => '=',
					_ => throw new ArgumentOutOfRangeException()
				}
			);
		}

		return sb.ToString();
	}

	private sbyte SnafuCharToSByte(char c)
	{
		return c switch
		{
			'0' => 0,
			'1' => 1,
			'2' => 2,
			'-' => -1,
			'=' => -2,
			_ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
		};
	}

	private long SnafuToDecimal(IList<sbyte> snafuNumber)
	{
		long decimalNo = 0;
		for (int i = 0; i < snafuNumber.Count; i++)
		{
			decimalNo += (long)Math.Pow(5, i) * snafuNumber[^(i + 1)];
		}

		return decimalNo;
	}

	private IList<sbyte> DecimalToSnafu(long number)
	{
		if (number == 0)
		{
			return new List<sbyte>() { 0 };
		}

		List<sbyte> snafuNumber = new();
		int i = 0;
		sbyte carry = 0;
		while (number > 0 || carry != 0)
		{
			sbyte r = (sbyte)(number % 5);
			r += carry;
			carry = 0;
			if (r <= 2)
			{
				snafuNumber.Insert(i, r);
			}
			else
			{
				carry++;
				snafuNumber.Insert(i, (sbyte)(r - 5));
			}

			number /= 5;

			i++;
		}

		return snafuNumber;
	}
}