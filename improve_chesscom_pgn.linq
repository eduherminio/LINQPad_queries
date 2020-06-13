<Query Kind="Program" />

void Main()
{
	const string src = @"C:\dev\ChessComPgnPath.pgn";
	const string dst = @"C:\dev\NewFilePath.pgn";

	var time = new Regex(@"{.*?\n*.*?}");
	var redundantMoveNumber = new Regex(@"\d*\.\.\.\s");
	var newLinesBetweenMoves = new Regex(@"\n(?=[^\[|\n*1])");
	var spaceAfterDots = new Regex(@"\.\s*");

	using (StreamReader sr = new StreamReader(new FileStream(src, FileMode.Open)))
	using (StreamWriter sw = new StreamWriter(new FileStream(dst, FileMode.Create)))
	{
		var rawFile = sr.ReadToEnd();

		var modifiedFile = time.Replace(rawFile, string.Empty);
		modifiedFile = redundantMoveNumber.Replace(modifiedFile, string.Empty);
		modifiedFile = newLinesBetweenMoves.Replace(modifiedFile, string.Empty);
		modifiedFile = spaceAfterDots.Replace(modifiedFile, ".");

		sw.Write(modifiedFile);
	}
}