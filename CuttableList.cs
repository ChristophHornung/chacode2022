namespace Chacode2022;

internal class CuttableList
{
	private byte[] buffer;
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
			if (realIndex > this.buffer.Length - 1)
			{
				return 0b0;
			}

			return this.buffer[realIndex];
		}

		set
		{
			long realIndex = index - this.offset;
			if (realIndex > this.buffer.Length - 1)
			{
				byte[] newBuffer = new byte[CuttableList.bufferLength];
				Array.Copy(this.buffer, this.cuttable, newBuffer, 0, this.maxSetIndex + 1 - this.cuttable);
				this.buffer = newBuffer;
				this.offset += this.cuttable;
				this.cuttable = 0;
				realIndex = index - this.offset;
			}

			this.buffer[realIndex] = value;
			this.maxSetIndex = realIndex;
		}
	}
}