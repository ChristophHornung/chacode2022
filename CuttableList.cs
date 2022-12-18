namespace Chacode2022;

using System;

internal sealed class CuttableList
{
	private  byte[] buffer;
	private long cuttable;
	private long offset;
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
				if (this.maxSetIndex < realIndex)
				{
					this.maxSetIndex = realIndex;
				}
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
				if (this.maxSetIndex < realIndex)
				{
					this.maxSetIndex = realIndex;
				}
			}
		}
	}

	public unsafe void SetRocksDirect(long index, byte[] shapes)
	{
		byte[] b = this.buffer;
		long realIndex = index - this.offset;
		if (realIndex + shapes.Length > b.Length)
		{
			byte[] newBuffer = new byte[b.Length];
			Array.Copy(this.buffer, this.cuttable, newBuffer, 0, this.maxSetIndex + 1 - this.cuttable);
			this.buffer = newBuffer;
			b = newBuffer;
			this.offset += this.cuttable;
			this.cuttable = 0;
			realIndex = index - this.offset;
		}
		
		fixed (byte* pb = b)
		{
			for (int i = 0; i < shapes.Length; i++)
			{
				pb[realIndex + i] = shapes[i];
			}
		}

		if (this.maxSetIndex < realIndex + shapes.Length - 1)
		{
			this.maxSetIndex = realIndex + shapes.Length - 1;
		}
	}

	public unsafe bool CheckCollision(long index, byte[] shapes)
	{
		long realIndex = index - this.offset;
		fixed (byte* pb = this.buffer)
		{
			for (int i = 0; i < shapes.Length; i++)
			{
				if (realIndex + i > this.maxSetIndex)
				{
					return false;
				}

				if ((pb[realIndex + i] & shapes[i]) > 0)
				{
					return true;
				}
			}
		}

		return false;
	}

	public unsafe void SetRocks(long index, byte[] shapes)
	{
		byte[] b = this.buffer;
		long realIndex = index - this.offset;
		if (realIndex + shapes.Length > b.Length)
		{
			byte[] newBuffer = new byte[b.Length];
			Array.Copy(this.buffer, this.cuttable, newBuffer, 0, this.maxSetIndex + 1 - this.cuttable);
			this.buffer = newBuffer;
			b = newBuffer;
			this.offset += this.cuttable;
			this.cuttable = 0;
			realIndex = index - this.offset;
		}
		
		fixed (byte* pb = b)
		{
			for (int i = 0; i < shapes.Length; i++)
			{
				pb[realIndex + i] |= shapes[i];
			}
		}

		if (this.maxSetIndex < realIndex + shapes.Length - 1)
		{
			this.maxSetIndex = realIndex + shapes.Length - 1;
		}
	}
}