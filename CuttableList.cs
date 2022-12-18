namespace Chacode2022;

internal sealed class CuttableList
{
	private  byte[] buffer;
	private long cuttable = 0;
	private long offset = 0;
	private long maxSetIndex = -1;
	private const int bufferLength = 100_000;

	public CuttableList()
	{
		this.buffer = new byte[CuttableList.bufferLength];

	}

	public void CutBelow(long index)
	{
		this.cuttable = index - this.offset;
	}

	public byte this[long index]
	{
		get
		{
			long realIndex = index - this.offset;
			byte[] b = this.buffer;
			if (realIndex > b.Length - 1)
			{
				return 0b0;
			}

			return b[realIndex];
		}

		set
		{
			byte[] b = this.buffer;
			long realIndex = index - this.offset;
			if (realIndex <= b.Length - 1)
			{
				b[realIndex] = value;
				this.maxSetIndex = realIndex;
			}
			else
			{
				byte[] newBuffer = new byte[b.Length];
				Array.Copy(this.buffer, this.cuttable, newBuffer, 0, this.maxSetIndex + 1 - this.cuttable);
				this.buffer = newBuffer;
				b = newBuffer;
				this.offset += this.cuttable;
				this.cuttable = 0;
				realIndex = index - this.offset;
				b[realIndex] = value;
				this.maxSetIndex = realIndex;
			}
		}
	}
}