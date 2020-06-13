<Query Kind="Program" />

void Main()
{
	const string basePath = @"C:\dev\RepoFolder";

	foreach (var dir in Directory.GetDirectories(basePath))
	{
		Console.WriteLine($"Analysing Directory: ${dir}");

		var bin = $"{dir}\\bin";
		var obj = $"{dir}\\obj";
		if (Directory.Exists(bin))
		{
			Console.WriteLine($"\t\tDeleting {bin}");
			Directory.Delete(bin, true);
		}
		if (Directory.Exists(obj))
		{
			Console.WriteLine($"\t\tDeleting {obj}");
			Directory.Delete(obj, true);
		}

		Console.WriteLine();
	}
}