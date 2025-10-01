<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.Configuration</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
</Query>

void Main()
{
	var builder0 = Host.CreateEmptyApplicationBuilder(new());
	var host0 = builder0.Build();

	HostApplicationBuilder builder1 = Host.CreateEmptyApplicationBuilder(new());
	
	var host1 = builder1
		.AddR9Goodies_Generic()		// Stays as HostApplicationBuilder
		.Build();

	var host2 = builder1
		.AddR9Goodies_Interface()	// Transforms into IHostApplicationBuilder
		.Build();					// Build error

	IHostApplicationBuilder builder2 = Host.CreateEmptyApplicationBuilder(new());

	var host3 = builder2
		.AddR9Goodies_Generic()		// Stays as IHostApplicationBuilder
		.Build();                   // Build error, regardless of using Generic or Interface extension


	var host4 = builder1
		.AddR9Goodies_Generic();     // Stays as HostApplicationBuilder
	
	host4.AddR9Goodies_Interface();	 // Returns IHostApplicationBuilder, but we don't use use it
	host4.Build();
}

public static class Extensions
{
	public static TBuilder AddR9Goodies_Generic<TBuilder>(this TBuilder builder)
			where TBuilder : IHostApplicationBuilder
		=> builder;

	public static IHostApplicationBuilder AddR9Goodies_Interface(this IHostApplicationBuilder builder) => builder;
}
