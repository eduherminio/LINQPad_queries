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
	HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(new());
	
	HostApplicationBuilder result = builder.AddR9Goodies_Generic();
	HostApplicationBuilder result2 = builder.AddR9Goodies_Interface();	// Build error
	
	builder
		.AddR9Goodies_Generic()
		.ExistingR9UserCode();

	builder
		.AddR9Goodies_Interface()		// Build error
		.ExistingR9UserCode();
}

public static class Extensions
{
	public static TBuilder AddR9Goodies_Generic<TBuilder>(this TBuilder builder)
			where TBuilder : IHostApplicationBuilder
		=> builder;

	public static IHostApplicationBuilder AddR9Goodies_Interface(this IHostApplicationBuilder builder) => builder;
	
	public static HostApplicationBuilder ExistingR9UserCode(this HostApplicationBuilder builder) => builder;
}
