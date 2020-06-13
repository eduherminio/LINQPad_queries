<Query Kind="Program">
  <Namespace>System.IO.Compression</Namespace>
</Query>

void Main()
{
	var unique_id = $"{DateTime.Now.ToLocalTime():yyyy'-'MM'-'dd'__'HH'_'mm'_'ss}";
	var fileName = @$"code_{unique_id}.zip";

	var basePath = @"C:\dev\RepoFolder";
	var zipPath = @$"{basePath}\{fileName}";

	var filesToInclude = Directory.GetFiles(basePath, "*.cs*", SearchOption.AllDirectories) // This includes .csproj
		.Where(f => !f.Contains(@"\obj\") && !f.Contains(@"\bin\"));

	var trimPath = @$"{basePath}\";

	using var fs = new FileStream(zipPath, FileMode.Create);
	using ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Update);
	foreach(var file in filesToInclude)
	{
		zip.CreateEntryFromFile(file, file.Substring(trimPath.Length));
	}

	zipPath.Dump();
}