<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.Configuration</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <NuGetReference Version="8.10.0-rtm.24502.7" Prerelease="true">Microsoft.Extensions.Hosting.Testing</NuGetReference>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.Configuration.Memory</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection.Extensions</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.Hosting.Testing</Namespace>
</Query>

#nullable enable

void Main()
{
	using (var host = CreateUsingHostBuilder())
	{
		host.Services.GetRequiredService<IOptions<SectionName>>().Value.location.Dump();
		host.Services.GetRequiredService<IOptions<SectionName>>().Value.name.Dump();
	}

	"----".Dump();

	using (var host = CreateUsingApplicationHostBuilder())
	{
		host.Services.GetRequiredService<IOptions<SectionName>>().Value.location.Dump();
		host.Services.GetRequiredService<IOptions<SectionName>>().Value.name.Dump();
	}

	static IHost CreateUsingHostBuilder()
	{
#pragma warning disable EXTEXP0016
		return FakeHost
			.CreateBuilder()
			.ConfigureHostConfiguration(b => b.AddBuildMetadataConfig())
			.ConfigureServices((ctx, s) => s.AddBuildMetadataServices(ctx.Configuration.GetSection("SectionName")))
			.Build();
#pragma warning restore EXTEXP0016
	}

	static IHost CreateUsingApplicationHostBuilder()
	{
		var builder = Host.CreateEmptyApplicationBuilder(new());

		builder.Configuration.AddBuildMetadataConfig();

		var section = builder.Configuration.GetSection("SectionName");
		builder.Services.AddBuildMetadataServices(section);

		return builder.Build();
	}


}

public static class Extensions
{
	public static IConfigurationBuilder AddBuildMetadataConfig(this IConfigurationBuilder builder)
	{
		return builder.Add(new MetadataSource());
	}

	public static IServiceCollection AddBuildMetadataServices(this IServiceCollection services, IConfigurationSection section)
	{
		_ = services
			.AddOptions<SectionName>()
			.Bind(section);

		return services;
	}
}

public class MetadataSource : IConfigurationSource
{
	public IConfigurationProvider Build(IConfigurationBuilder builder) =>
	   new MemoryConfigurationProvider(new MemoryConfigurationSource())
		{
			{ "SectionName:location", string.Empty },
			{ "SectionName:name", "name" }
		};
}

public class SectionName
{
	public string? location { get; set; }

	public string? name { get; set; }
}
