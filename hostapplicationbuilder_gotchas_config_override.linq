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
#pragma warning disable EXTEXP0016

void Main()
{
	var config = new Dictionary<string, string?>
	{
		//["Cosmic:Cloud"] = "overriden-cloud",
		//["Cosmic:Region"] = "overriden-region",
		//["Cosmic:AzureCloud"] = "overriden-azure-cloud",
		//["Cosmic:AzureRegion"] = "overriden-azure-region",
	};
	
	using (var host = CreateUsingHostBuilder(config))
	{
		host.Services.GetRequiredService<IOptions<Metadata>>().Value.Cloud.Dump();
	}

	"----".Dump();

	using (var host = CreateUsingApplicationHostBuilder(config))
	{		
		host.Services.GetRequiredService<IOptions<Metadata>>().Value.Cloud.Dump();
	}

	static IHost CreateUsingHostBuilder(Dictionary<string, string?> config) => FakeHost
		.CreateBuilder()
		
		//.ConfigureHostConfiguration(i =>
		//	{
		//		i.AddKeyValue((Extensions.CosmicMetadataVersionEnvKey, "v1"));
		//	})
		
		.AddCosmicClusterMetadata()

		//.ConfigureHostConfiguration(i => i.AddInMemoryCollection(config))
				
		.Build();

	static IHost CreateUsingApplicationHostBuilder(Dictionary<string, string?> config)
	{
		var builder = Host.CreateEmptyApplicationBuilder(new());

		//builder.Configuration.AddKeyValue((Extensions.CosmicMetadataVersionEnvKey, "v1"));

		builder.AddCosmicClusterMetadata();

		//builder.Configuration.AddInMemoryCollection(config);

		return builder.Build();
	}
}

public static class Extensions
{
	public const string CosmicMetadataVersionEnvKey = "COSMIC_METADATA_VERSION";

	public static IHostBuilder AddCosmicClusterMetadata(this IHostBuilder builder)
	{
		_ = builder.ConfigureHostConfiguration(ConfigureHost);

		_ = builder.ConfigureAppConfiguration((context, configurationBuilder) =>
			configurationBuilder.ConfigureApplication(context.Configuration, context.HostingEnvironment));

		_ = builder.ConfigureServices((context, services) =>
			services.ConfigureCosmicClusterMetadataServices(context.Configuration.GetSection("Cosmic")));

		return builder;
	}

	public static TBuilder AddCosmicClusterMetadata<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		builder.Configuration.ConfigureHost();
		builder.Configuration.ConfigureApplication(builder.Configuration, builder.Environment);
		builder.Services.ConfigureCosmicClusterMetadataServices(builder.Configuration.GetSection("Cosmic"));

		return builder;
	}
	
	private static void ConfigureCosmicClusterMetadataServices(this IServiceCollection services, IConfigurationSection section)
	{
		_ = services
			.AddOptions<Metadata>()
			.Bind(section);
	}

	private static void ConfigureHost(this IConfigurationBuilder configurationBuilder)
	{
		// Loads "Cosmic:" section.
		_ = configurationBuilder.Add(new MetadataSource());
	}

	private static void ConfigureApplication(this IConfigurationBuilder configurationBuilder, IConfiguration configuration, IHostEnvironment environment)
	{
		var metadataVersionString = configuration[CosmicMetadataVersionEnvKey];
		
		if (!string.IsNullOrWhiteSpace(metadataVersionString))
		{
			if (string.Equals("V1", metadataVersionString, StringComparison.InvariantCultureIgnoreCase))
			{
				configuration["Cosmic:Cloud"] = configuration["Cosmic:AzureCloud"];
				configuration["Cosmic:Region"] = configuration["Cosmic:AzureRegion"];
			}
		}
	}
	
	public static T AddKeyValue<T>(this T configuration, params (string key, string value)[] tuple)
		where T : IConfigurationBuilder
	{
		_ = configuration
			.AddInMemoryCollection(tuple.Select(i =>
				new KeyValuePair<string, string?>(i.key, i.value)));

		return configuration;
	}
}

public class MetadataSource : IConfigurationSource
{
	public IConfigurationProvider Build(IConfigurationBuilder builder) =>
	   new MemoryConfigurationProvider(new MemoryConfigurationSource())
		{
			{ "Cosmic:Cloud", "Default cloud" },
			{ "Cosmic:Region", "Default region" },
			
			{ "Cosmic:AzureCloud", "Default azure cloud" },
			{ "Cosmic:AzureRegion", "Default azure region" }
		};
}

public class Metadata
{
	public string? Cloud { get; set; }

	public string? Region { get; set; }
}

#pragma warning restore EXTEXP0016