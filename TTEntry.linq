<Query Kind="Program">
  <NuGetReference>ObjectLayoutInspector</NuGetReference>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>ObjectLayoutInspector</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	//	TranspositionTableElement.Size.Dump();
	//	"------------------".Dump();
	//	TypeLayout.PrintLayout<TranspositionTableElement>();
	//
	//"-------------------------------------------------------------------------------------------".Dump();


	//TranspositionTableElement item = new();
	//item.Update(1234, -1, -2, 3, NodeType.None, wasPv : 0, move : 5678, age : 15);
	//
	//item.Type.Dump();
	//item.WasPv.Dump();
	//item.Age.Dump();
	//"-------------------------------------------------------------------------------------------".Dump();

	{
		var bucket = new TranspositionTableBucket();
		ref var entry = ref bucket[0];
		entry.Update(1234, -1, -2, 3, NodeType.None, wasPv: 0, move: 5678, age: 0);

		ref var entry2 = ref bucket[1];
		entry2.Update(1234, -1, -2, 3, NodeType.None, wasPv: 0, move: 5678, age: 2);

		ref var entry3 = ref bucket[2];
		entry3.Update(1234, -1, -2, 3, NodeType.None, wasPv: 0, move: 5678, age: 3);

		bucket[0].Age.Dump();
		bucket[1].Age.Dump();
		bucket[2].Age.Dump();
	}
	"-------------------------------------------------------------------------------------------".Dump();

	{
		var tt = new TranspositionTable();
		ref var bucket = ref tt.Get(0);
		ref var entry = ref bucket[0];
		entry.Update(1234, -1, -2, 3, NodeType.None, wasPv: 0, move: 5678, age: 0);

		ref var entry2 = ref bucket[1];
		entry2.Update(1234, -1, -2, 3, NodeType.None, wasPv: 0, move: 5678, age: 2);

		ref var entry3 = ref bucket[2];
		entry3.Update(1234, -1, -2, 3, NodeType.None, wasPv: 0, move: 5678, age: 3);

		bucket[0].Age.Dump();
		bucket[1].Age.Dump();
		bucket[2].Age.Dump();

		tt._tt[0][0].Age.Dump();
		tt._tt[0][1].Age.Dump();
		tt._tt[0][2].Age.Dump();

		tt.Clear();

		"-----------------------".Dump();

		bucket[0].Age.Dump();
		bucket[1].Age.Dump();
		bucket[2].Age.Dump();

		tt._tt[0][0].Age.Dump();
		tt._tt[0][1].Age.Dump();
		tt._tt[0][2].Age.Dump();
	}
	"-------------------------------------------------------------------------------------------".Dump();

	TypeLayout.PrintLayout<TranspositionTableBucket>();

	"-------------------------------------------------------------------------------------------".Dump();

	TranspositionTableBucket64.Size.Dump();
	TypeLayout.PrintLayout<TranspositionTableBucket64>();

	"-------------------------------------------------------------------------------------------".Dump();
	{
		var tt = new TranspositionTableUnsafe();
		ref var zero = ref tt.Get(0);
		zero._ttEntry0.Update(0, 0, 0, 0, NodeType.Exact, 0, 0, 0);
		zero._ttEntry1.Update(1, 1, 1, 1, NodeType.Exact, 1, 1, 1);
		zero._ttEntry2.Update(2, 2, 2, 2, NodeType.Exact, 2, 2, 2);

		ref var one = ref tt.Get(1);
		one._ttEntry0.Update(10, 10, 10, 10, NodeType.Exact, 10, 10, 10);
		one._ttEntry1.Update(11, 11, 11, 11, NodeType.Exact, 11, 11, 11);
		one._ttEntry2.Update(12, 12, 12, 12, NodeType.Exact, 12, 12, 12);


		ref var two = ref tt.Get(2);
		two._ttEntry0.Update(100, 100, 100, 100, NodeType.Exact, 100, 100, 100);
		two._ttEntry1.Update(101, 101, 101, 101, NodeType.Exact, 101, 101, 101);
		two._ttEntry2.Update(102, 102, 102, 102, NodeType.Exact, 102, 102, 102);

		unsafe
		{
			fixed (TranspositionTableBucketUnsafe* ttPtr = tt._tt)
			{
				var bucket = ttPtr + 1;

					var regularZero = (*bucket)._ttEntry0;
				var entr = (TranspositionTableElement*)&bucket;
				
//				ref var zero = ref entr[0];
//				zero.Update(10, 10, 10, 10, NodeType.Exact, 10, 10, 10);
//
//				ref var one = ref entr[1];
//				zero.Update(11, 11, 11, 11, NodeType.Exact, 11, 11, 11);
//
//				ref var two = ref entr[2];
//				zero.Update(12, 12, 12, 12, NodeType.Exact, 12, 12, 12);
			}
		}
	}
}

//public struct TranspositionTableBucket2
//{
//	//TranspositionTableElement[] Entries;
//	TranspositionTableElement Entry1;
//	TranspositionTableElement Entry2;
//	TranspositionTableElement Entry3;
//	//TranspositionTableElement Entry4;
//	//TranspositionTableElement Entry5;
//	//TranspositionTableElement Entry6;
//
//	public static ulong Size
//	{
//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		get => (ulong)Marshal.SizeOf<TranspositionTableBucket2>();
//	}
//}

public struct TranspositionTable
{
	public readonly TranspositionTableBucket[] _tt;

	private int _age = 0;

	public TranspositionTable()
	{
		_tt = GC.AllocateArray<TranspositionTableBucket>(128, pinned: true);
	}

	internal readonly ref TranspositionTableBucket Get(int index) => ref _tt[index];

	public void Clear()
	{
		var threadCount = 1;

		var sw = Stopwatch.StartNew();

		var tt = _tt;
		var ttLength = tt.Length;
		var sizePerThread = ttLength / threadCount;

		// Instead of just doing Array.Clear(_tt):
		Parallel.For(0, threadCount, i =>
		{
			var start = i * sizePerThread;
			var length = (i == threadCount - 1)
				? ttLength - start
				: sizePerThread;

			Array.Clear(tt, start, length);

			// TODO clusters?
		});

		_age = 0;
	}
}

public struct TranspositionTableUnsafe
{
	public readonly TranspositionTableBucketUnsafe[] _tt;

	private int _age = 0;

	public TranspositionTableUnsafe()
	{
		_tt = GC.AllocateArray<TranspositionTableBucketUnsafe>(128, pinned: true);
	}

	internal readonly ref TranspositionTableBucketUnsafe Get(int index) => ref _tt[index];

	public void Clear()
	{
		var threadCount = 1;

		var sw = Stopwatch.StartNew();

		var tt = _tt;
		var ttLength = tt.Length;
		var sizePerThread = ttLength / threadCount;

		// Instead of just doing Array.Clear(_tt):
		Parallel.For(0, threadCount, i =>
		{
			var start = i * sizePerThread;
			var length = (i == threadCount - 1)
				? ttLength - start
				: sizePerThread;

			Array.Clear(tt, start, length);

			// TODO clusters?
		});

		_age = 0;
	}
}

[StructLayout(LayoutKind.Explicit, Size = 32)]
public unsafe struct TranspositionTableBucketUnsafe
{
#pragma warning disable S1144, RCS1213 // Unused private types or members should be removed
	[FieldOffset(0)]
	public TranspositionTableElement _ttEntry0;

	[FieldOffset(10)]
	public TranspositionTableElement _ttEntry1;

	[FieldOffset(20)]
	public TranspositionTableElement _ttEntry2;
#pragma warning restore S1144 // Unused private types or members should be removed

	// TODO Add byte padding to align with 32 or 64 bytes

	/// <summary>
	/// Struct size in bytes
	/// </summary>
	public static ulong Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ulong)Marshal.SizeOf<TranspositionTableBucket>();
	}
}


[InlineArray(3)]
public struct TranspositionTableBucket
{
	private TranspositionTableElement _ttEntry;

	public static ulong Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ulong)Marshal.SizeOf<TranspositionTableBucket>();
	}
}

public struct TranspositionTableElement
{
	private ushort _key;        // 2 bytes
	private short _move;    // 2 bytes
	private short _score;       // 2 bytes
	private short _staticEval;  // 2 bytes
	private byte _depth;        // 1 byte

	/// <summary>
	/// 1 byte
	/// Binary move bits    Hexadecimal
	/// 0000 0001              0x1           Was PV (0-1)
	/// 0000 1110              0xE           NodeType (0-4)
	/// 1111 0000              0xE           NodeType (0-4)
	/// </summary>
	private byte _age_type_WasPv;

	private const int NodeTypeOffset = 1;
	private const int NodeTypeMask = 0xE;

	private const int AgeOffset = 4;
	private const int AgeExtractionMask = 0xF0;
	public const int AgeCalculationMask = AgeExtractionMask - 1;

	public readonly ushort Key
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _key;
	}

	public readonly short Move
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _move;
	}

	public readonly int Score
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _score;
	}

	public readonly int StaticEval
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _staticEval;
	}

	public readonly int Depth
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _depth;
	}

	// 0-4
	public readonly NodeType Type
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (NodeType)((_age_type_WasPv & NodeTypeMask) >> NodeTypeOffset);
	}

	public readonly bool WasPv
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_age_type_WasPv & 0x1) == 1;
	}

	// 0-16
	public readonly int Age
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_age_type_WasPv) >> AgeOffset;
	}

	public static ulong Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ulong)Marshal.SizeOf<TranspositionTableElement>();
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Update(ulong key, int score, int staticEval, int depth, NodeType nodeType, int wasPv, int? move, int age)
	{
		Debug.Assert(age <= AgeExtractionMask);
		Debug.Assert(nodeType != NodeType.Unknown);

		_key = (ushort)key;
		_score = (short)score;
		_staticEval = (short)staticEval;
		_depth = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref depth, 1))[0];
		_age_type_WasPv = (byte)(wasPv | ((int)nodeType << NodeTypeOffset) | age << AgeOffset);
		_move = move != null ? (short)move : Move;    // Suggested by cj5716 instead of 0. https://github.com/lynx-chess/Lynx/pull/462
	}
}

public enum NodeType : byte
{
	Unknown,    // Making it 0 instead of -1 because of default struct initialization
	Exact,
	Alpha,
	Beta,
	None
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TranspositionTableBucket64
{
#pragma warning disable S1144, RCS1213 // Unused private types or members should be removed
	private TranspositionTableBucketEntries _ttEntry;

	private readonly byte _padding1;

	private readonly byte _padding2;
#pragma warning restore S1144 // Unused private types or members should be removed

	public readonly TranspositionTableElement this[int i]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _ttEntry[i];
	}

	/// <summary>
	/// Struct size in bytes
	/// </summary>
	public static ulong Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ulong)Marshal.SizeOf<TranspositionTableBucket64>();
	}
}

[InlineArray(3)]
public struct TranspositionTableBucketEntries
{
	private TranspositionTableElement _ttEntry;

	public static ulong Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ulong)Marshal.SizeOf<TranspositionTableBucket>();
	}
}
// You can define other methods, fields, classes and namespaces here
