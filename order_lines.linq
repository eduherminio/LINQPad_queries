<Query Kind="Program" />

void Main()
{
	const string src = @"C:\dev\File.txt";
	const string dst = @"C:\dev\File_ordered.txt";

	using (StreamReader sr = new StreamReader(new FileStream(src, FileMode.Open)))
	using (StreamWriter sw = new StreamWriter(new FileStream(dst, FileMode.Create)))
	{
		var lines = new List<string>();

		while (!sr.EndOfStream)
		{
			lines.Add(sr.ReadLine());
		}

		var sorted = lines
			.OrderBy(_ => _)
			.ToList();

		// sorted.Dump();

		sorted.ForEach(sw.WriteLine);
	}
}