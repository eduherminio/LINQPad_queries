<Query Kind="Program">
  <NuGetReference>ObjectLayoutInspector</NuGetReference>
  <Namespace>ObjectLayoutInspector</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
</Query>

void Main()
{
	ChessBoard.Size.Dump();
	TypeLayout.PrintLayout<ChessBoard>();

	ChessBoard2.Size.Dump();
	TypeLayout.PrintLayout<ChessBoard2>();
}

public struct ChessBoard
{
	public ulong Occupancy { get; set; }
	public byte[] Pieces { get; set; } = new byte[16];
	public short Score { get; set; }
	public byte Result { get; set; }
	public byte KingSquare { get; set; }
	public byte OppositeKingSquare { get; set; }
	public byte[] Extra { get; set; } = new byte[3];

	public static ulong Size => (ulong)Marshal.SizeOf<ChessBoard>();

	public ChessBoard(ulong occupancy, byte[] pieces, short score, byte result, byte kingSquare, byte oppositeKingSquare, byte[] extra)
	{
		Occupancy = occupancy;
		Pieces = pieces;
		Score = score;
		Result = result;
		KingSquare = kingSquare;
		OppositeKingSquare = oppositeKingSquare;
		Extra = extra;
	}
}

public struct ChessBoard2
{
	ulong _occupancy;
	readonly byte[] _pieces = new byte[16];
	readonly short _score;
	readonly byte _result;
	readonly byte _kingSquare;
	readonly byte _oppositeKingSquare;
	readonly byte[] _extra = new byte[3];

	public int ResultIdx => _result;
	public double Result => _result / 2.0;
	public short Score => _score;

	public static ulong Size => (ulong)Marshal.SizeOf<ChessBoard2>();

	public ChessBoard2(ulong occupancy, byte[] pieces, short score, byte result, byte kingSquare, byte oppositeKingSquare, byte[] extra)
	{
		_occupancy = occupancy;
		_pieces = pieces;
		_score = score;
		_result = result;
		_kingSquare = kingSquare;
		_oppositeKingSquare = oppositeKingSquare;
		_extra = extra;
	}
}
