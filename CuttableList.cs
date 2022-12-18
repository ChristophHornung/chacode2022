namespace Chacode2022;

using System;
using System.Buffers;

internal sealed class CuttableList
{
	private byte[] buffer;
	private Memory<byte> bufferMemory;
	private MemoryHandle bufferMemoryHandle;
	private long cuttable;
	private long offset;
	private long maxSetIndex = -1;
	private const int bufferLength = 100_000;

	public CuttableList()
	{
		this.buffer = new byte[CuttableList.bufferLength];
		this.bufferMemory = new Memory<byte>(this.buffer);
		this.bufferMemoryHandle = this.bufferMemory.Pin();
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
	}

	public unsafe void SetRocksDirect(long index, byte[] shapes)
	{
		long realIndex = index - this.offset;
		if (realIndex + shapes.Length > buffer.Length - 20)
		{
			byte[] newBuffer = new byte[buffer.Length];
			Array.Copy(this.buffer, this.cuttable, newBuffer, 0, this.maxSetIndex + 1 - this.cuttable);
			this.buffer = newBuffer;
			this.bufferMemory = new Memory<byte>(this.buffer);
			this.bufferMemoryHandle.Dispose();
			this.bufferMemoryHandle = this.bufferMemory.Pin();
			this.offset += this.cuttable;
			this.cuttable = 0;
			realIndex = index - this.offset;
		}

		byte* pb = (byte*)this.bufferMemoryHandle.Pointer;
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

	public unsafe void SetRocksDirect(long index, int shape, int shapeLines)
	{
		long realIndex = index - this.offset;
		if (realIndex + shapeLines > buffer.Length - 20)
		{
			byte[] newBuffer = new byte[buffer.Length];
			Array.Copy(this.buffer, this.cuttable, newBuffer, 0, this.maxSetIndex + 1 - this.cuttable);
			this.buffer = newBuffer;
			this.bufferMemory = new Memory<byte>(this.buffer);
			this.bufferMemoryHandle.Dispose();
			this.bufferMemoryHandle = this.bufferMemory.Pin();
			this.offset += this.cuttable;
			this.cuttable = 0;
			realIndex = index - this.offset;
		}

		byte* pb = (byte*)this.bufferMemoryHandle.Pointer;
		int* pbI = (int*)(pb+realIndex);
		{
			pbI[0] = shape;
		}

		if (this.maxSetIndex < realIndex + shapeLines - 1)
		{
			this.maxSetIndex = realIndex + shapeLines - 1;
		}
	}

	public unsafe bool CheckCollision(long index, int shape, int shapeLines)
	{
		long realIndex = index - this.offset;
		byte* pb = (byte*)this.bufferMemoryHandle.Pointer;
		int* pbI = (int*)(pb+realIndex);
		return (pbI[0] & shape) > 0;
	}

	public unsafe bool CheckCollision(long index, byte[] shapes)
	{
		long realIndex = index - this.offset;
		byte* pb = (byte*)this.bufferMemoryHandle.Pointer;
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
		int realIndex = (int)(index - this.offset);
		if (realIndex + shapes.Length > buffer.Length - 20)
		{
			byte[] newBuffer = new byte[buffer.Length];
			Array.Copy(this.buffer, this.cuttable, newBuffer, 0, this.maxSetIndex + 1 - this.cuttable);
			this.buffer = newBuffer;
			this.bufferMemory = new Memory<byte>(this.buffer);
			this.bufferMemoryHandle.Dispose();
			this.bufferMemoryHandle = this.bufferMemory.Pin();
			this.offset += this.cuttable;
			this.cuttable = 0;
			realIndex = (int)(index - this.offset);
		}

		byte* sb = (byte*)this.bufferMemoryHandle.Pointer;
		for (int i = 0; i < shapes.Length; i++)
		{
			sb[realIndex + i] |= shapes[i];
		}

		if (this.maxSetIndex < realIndex + shapes.Length - 1)
		{
			this.maxSetIndex = realIndex + shapes.Length - 1;
		}
	}

	public unsafe void SetRocks(long index, int shape, int shapeLines)
	{
		long realIndex = index - this.offset;
		if (realIndex + shapeLines > buffer.Length - 20)
		{
			byte[] newBuffer = new byte[buffer.Length];
			Array.Copy(this.buffer, this.cuttable, newBuffer, 0, this.maxSetIndex + 1 - this.cuttable);
			this.buffer = newBuffer;
			this.bufferMemory = new Memory<byte>(this.buffer);
			this.bufferMemoryHandle.Dispose();
			this.bufferMemoryHandle = this.bufferMemory.Pin();
			this.offset += this.cuttable;
			this.cuttable = 0;
			realIndex = index - this.offset;
		}

		byte* pb = (byte*)this.bufferMemoryHandle.Pointer;
		int* pbI = (int*)(pb+realIndex);
		pbI[0] |= shape;

		if (this.maxSetIndex < realIndex + shapeLines - 1)
		{
			this.maxSetIndex = realIndex + shapeLines - 1;
		}
	}
}