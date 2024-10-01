<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

string Transform(ReadOnlySpan<char> line)
{
	Span<Range> ranges = stackalloc Range[7];

	_ = line.Split(ranges, ',');

	var fen = line[ranges[3]];
	var wdl = line[ranges[6]];

	var output = $"{fen} [{wdl}]";

	return output;
}

async Task Main()
{
	const string wslPath = @"\\wsl$/Ubuntu";
	const string inputFile = $"{wslPath}/home/ed/datasets/pedantic_master_training_data_ver7.0d.csv";	// https://www.kaggle.com/datasets/joannpeeler/labeled-chess-positions-109m-csv-format
	const string outputFile = $"{wslPath}/home/ed/pedantic_master_training_data_ver7.0d_wdl.csv";

	if (Path.Exists(outputFile))
	{
		File.Delete(outputFile);
	}

	var fs = File.Create(outputFile);
	fs.Close();

	using var sw = new StreamWriter(outputFile);

	await foreach (var line in File.ReadLinesAsync(inputFile))
	{
		var output = Transform(line);
		await sw.WriteLineAsync(output);
	}
}
