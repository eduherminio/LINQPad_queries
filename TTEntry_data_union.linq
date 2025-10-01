<Query Kind="Program">
  <NuGetReference>ObjectLayoutInspector</NuGetReference>
  <Namespace>Move = int</Namespace>
  <Namespace>ObjectLayoutInspector</Namespace>
  <Namespace>ShortMove = short</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
</Query>

void Main()
{
	//TranspositionTableElement.Size.Dump();
	//TypeLayout.PrintLayout<TranspositionTableElement>();

	TranspositionTableElementExpicit.Size.Dump();
	TypeLayout.PrintLayout<TranspositionTableElementExpicit>();

	//LeorikHashEntry.Size.Dump();
	//TypeLayout.PrintLayout<LeorikHashEntry>();
}

public struct TranspositionTableElement
{
	private ushort _key;        // 2 bytes
	private ShortMove _move;    // 2 bytes
	private short _score;       // 2 bytes
	private short _staticEval;  // 2 bytes
	private byte _depth;        // 1 byte

	/// <summary>
	/// Binary move bits    Hexadecimal
	/// 0000 0001              0x1           Was PV (0-1)
	/// 0000 1110              0xE           NodeType (0-3)
	/// </summary>
	private byte _type_WasPv;	// 1 byte

	private const int NodeTypeOffset = 1;

	public readonly ushort Key => _key;
	public readonly ShortMove Move => _move;
	public readonly int Score => _score;
	public readonly int StaticEval => _staticEval;
	public readonly int Depth => _depth;

	public readonly NodeType Type => (NodeType)((_type_WasPv & 0xE) >> NodeTypeOffset);

	public readonly bool WasPv => (_type_WasPv & 0x1) == 1;

	public static ulong Size => (ulong)Marshal.SizeOf<TranspositionTableElement>();

	public void Update(ulong key, int score, int staticEval, int depth, NodeType nodeType, int wasPv, Move? move)
	{
		_key = (ushort)key;
		_score = (short)score;
		_staticEval = (short)staticEval;
		_depth = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref depth, 1))[0];
		_type_WasPv = (byte)(wasPv | ((int)nodeType << NodeTypeOffset));
		_move = move != null ? (ShortMove)move : Move;    // Suggested by cj5716 instead of 0. https://github.com/lynx-chess/Lynx/pull/462
	}
}

[StructLayout(LayoutKind.Explicit)]
public struct LeorikHashEntry
{
	[FieldOffset(0)]
	public ulong Key; //8 Bytes
	[FieldOffset(8)]
	public short Score; //2 Bytes
	[FieldOffset(10)]
	public byte Depth; //1 Byte
	[FieldOffset(11)]
	public byte Age; //1 Byte
	[FieldOffset(12)]
	public int MoveAndType; //4 Byte
							//=================================
							// 16 Bytes
	[FieldOffset(8)]
	public ulong Data;

	public static ulong Size => (ulong)Marshal.SizeOf<TranspositionTableElement>();
	public ulong GetHash() => Key ^ Data;
	public void SetHash(ulong hash) => Key = hash ^ Data;
}

[StructLayout(LayoutKind.Explicit, Size = 10)]
public struct TranspositionTableElementExpicit
{
	[FieldOffset(0)]
	private ushort _key;        // 2 bytes

	[FieldOffset(2)]
	private ShortMove _move;    // 2 bytes

	[FieldOffset(4)]
	private short _score;       // 2 bytes
	
	[FieldOffset(6)]
	private short _staticEval;  // 2 bytes
	
	[FieldOffset(8)]
	private byte _depth;        // 1 byte

	/// <summary>
	/// Binary move bits    Hexadecimal
	/// 0000 0001              0x1           Was PV (0-1)
	/// 0000 1110              0xE           NodeType (0-3)
	/// </summary>
	[FieldOffset(9)]
	private byte _type_WasPv;   // 1 byte

	[FieldOffset(0)]
	public ulong Data;

	private const int NodeTypeOffset = 1;

	public readonly ushort Key => _key;
	public readonly ShortMove Move => _move;
	public readonly int Score => _score;
	public readonly int StaticEval => _staticEval;
	public readonly int Depth => _depth;

	public readonly NodeType Type => (NodeType)((_type_WasPv & 0xE) >> NodeTypeOffset);

	public readonly bool WasPv => (_type_WasPv & 0x1) == 1;

	public static ulong Size => (ulong)Marshal.SizeOf<LeorikHashEntry>();

	public void Update(ulong key, int score, int staticEval, int depth, NodeType nodeType, int wasPv, Move? move)
	{
		_key = (ushort)key;
		_score = (short)score;
		_staticEval = (short)staticEval;
		_depth = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref depth, 1))[0];
		_type_WasPv = (byte)(wasPv | ((int)nodeType << NodeTypeOffset));
		_move = move != null ? (ShortMove)move : Move;    // Suggested by cj5716 instead of 0. https://github.com/lynx-chess/Lynx/pull/462
	}
}

public enum NodeType : byte
{
	Unknown,    // Making it 0 instead of -1 because of default struct initialization
	Exact,
	Alpha,
	Beta
}

