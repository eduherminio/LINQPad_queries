<Query Kind="Program">
  <NuGetReference>ObjectLayoutInspector</NuGetReference>
  <Namespace>ObjectLayoutInspector</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
</Query>

void Main()
{
	//ParseFENResult.Size.Dump();
	//TypeLayout.PrintLayout(typeof(ParseFENResult));

	TTResult.Size.Dump();
	TypeLayout.PrintLayout(typeof(TTResult));
}

public readonly struct ParseFENResult
{
#pragma warning disable S3887 // Mutable, non-private fields should not be "readonly"
	public readonly ulong[] PieceBitBoards;
	public readonly ulong[] OccupancyBitBoards;
	public readonly int[] Board;
#pragma warning restore S3887 // Mutable, non-private fields should not be "readonly"

	public readonly int Side;
	public readonly byte Castle;
	public readonly int EnPassant;
	public readonly int HalfMoveClock;

	public ParseFENResult(
		ulong[] pieceBitBoards,
		ulong[] occupancyBitBoards,
		int[] board,
		int side,
		byte castle,
		int enPassant,
		int halfMoveClock)
	{
		PieceBitBoards = pieceBitBoards;
		OccupancyBitBoards = occupancyBitBoards;
		Board = board;
		Side = side;
		Castle = castle;
		EnPassant = enPassant;
		HalfMoveClock = halfMoveClock;
	}

	public static ulong Size => (ulong)Marshal.SizeOf(typeof(ParseFENResult));
}

public readonly struct TTResult
{
	public readonly int Score;// = EvaluationConstants.NoScore;
	public readonly int StaticEval;// = EvaluationConstants.NoScore;
		public readonly int Depth;
	
	public readonly short BestMove;

	public readonly NodeType NodeType;

	public readonly bool WasPv;

	public TTResult(int score, short bestMove, NodeType nodeType, int staticEval, int depth, bool wasPv)
	{
		Score = score;
		BestMove = bestMove;
		NodeType = nodeType;
		StaticEval = staticEval;
		Depth = depth;
		WasPv = wasPv;
	}
	
	public static ulong Size => (ulong)Marshal.SizeOf(typeof(ParseFENResult));
}


public enum NodeType : byte
{
	Unknown,    // Making it 0 instead of -1 because of default struct initialization
	Exact,
	Alpha,
	Beta,
	None
}


// You can define other methods, fields, classes and namespaces here
