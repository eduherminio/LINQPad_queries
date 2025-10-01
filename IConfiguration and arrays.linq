<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.Configuration.Json</NuGetReference>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
</Query>

void Main()
{
	var builder = new ConfigurationBuilder()
		.AddJsonFile(@"C:\Users\<eric>\Downloads\a.json")
		.AddJsonFile(@"C:\Users\<eric>\Downloads\a - Copy.json");
	
	IConfiguration config = builder.Build();
	
	var section1 = config.GetSection("Section1").GetChildren();

	foreach (var item in section1)
		foreach (var child in item.GetChildren())
			Console.WriteLine($"{child.Path}: {child.Value}");
}