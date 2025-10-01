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
	var builder0 = CosmicHost.CreateBuilder_2();
	var host0 = builder0.Build();

	var builder = CosmicHost.CreateBuilder();
	var host = builder.Build();
}

public static class Extensions
{
	public static TBuilder AddR9Goodies_Generic<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		//_ = builder.Services.AddSingleton(...);
		//_ = builder.Configuration.AddInMemoryCollection(...);
		
		return builder;
	}
}

public static class CosmicHost
{
	public static TBuilder CreateBuilder_0<TBuilder>()
			where TBuilder : IHostApplicationBuilder
		=> Host.CreateApplicationBuilder(new HostApplicationBuilderSettings());

	public static HostApplicationBuilder CreateBuilder()
		=> Host.CreateApplicationBuilder(new HostApplicationBuilderSettings());
		
	public static IHostApplicationBuilder CreateBuilder_2()
		=> Host.CreateApplicationBuilder(new HostApplicationBuilderSettings());
}


