<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	const string filePath = "C:/dev/";
	const string fileName = "E12.46FRC-1250k-D12-1s-Resolved.book";

	const string inputPath = filePath + fileName;
	string outputPath = filePath + Path.GetFileNameWithoutExtension(inputPath) + "-no-castle" + Path.GetExtension(inputPath);

	int lineCounter = 0, noCastleLineCounter = 0;

	using var sw = new StreamWriter(outputPath);
	await foreach (var line in File.ReadLinesAsync(inputPath))
	{
		if (line.Contains("- -"))
		{
			sw.WriteLine(line);
			++noCastleLineCounter;
		}

		if (++lineCounter % 1_000_000 == 0)
		{
			$"{noCastleLineCounter} / {lineCounter} ({(100 * noCastleLineCounter / (double)lineCounter).ToString("N2")} %)".Dump();
		}
	}
}

// You can define other methods, fields, classes and namespaces here
