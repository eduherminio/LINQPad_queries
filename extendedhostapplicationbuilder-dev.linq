<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.Configuration</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Diagnostics.Metrics</Namespace>
  <RuntimeVersion>9.0</RuntimeVersion>
</Query>

void Main()
{
	// Builder similar to existing IHostBuilder, but simplified
	{
		var builder = CosmicHost
			.R9Builder()
			.CustomerCodeOverridingOurR9GoodiesConfig();

		builder.Configuration.AddKeyValue(("key", "custom config 2"));

		using (var host = builder.Build())
		{
			RunApp(host);
		}
	}

	// Builder without public members
	{
		var builder = CosmicHost
			.R9Builder2()
			.CustomerCodeOverridingOurR9GoodiesConfig();

		//builder2.Configuration.AddKeyValue(("key", "custom config 2"));

		using (var host = builder.Build())
		{
			RunApp(host);
		}
	}

	// 100% simplified builder, no wrapper class
	{
		var builder = CosmicHost
			.R9Builder3()
			.CustomerCodeOverridingOurR9GoodiesConfig();

		builder.Configuration.AddKeyValue(("key", "custom config 2"));

		using (var host = builder.Build())
		{
			RunApp(host);
		}
	}

	// Example of what not to do
	{
		var builder = CosmicHost
			.R9Builder4();

		builder.Configuration.AddKeyValue(("key", "custom config 2"));

		using (var host = builder.Build())
		{
			RunApp(host);
		}
	}

	void RunApp(IHost host)
	{
		var config = host.Services.GetRequiredService<IConfiguration>();
		config["key"].Dump();

		var service = host.Services.GetRequiredService<TelemetrySender>();
		service.Run();

		Console.WriteLine("--");
	}
}

public static class Extensions
{
	public static TBuilder AddR9Goodies_Generic<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		_ = builder.Configuration.AddKeyValue(("key", "default config"));
		_ = builder.Services.AddSingleton<TelemetrySender>();

		return builder;
	}

	public static TBuilder CustomerCodeOverridingOurR9GoodiesConfig<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		_ = builder.Configuration.AddKeyValue(("key", "custom config"));

		return builder;
	}

	public static TBuilder ExampleOfWhatNOTToDo<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		_ = builder.Configuration.AddKeyValue(("key", "default config"));

		if (builder.Configuration["key"] == "default config")
			_ = builder.Services.AddSingleton<TelemetrySender, TelemetrySender_Default>();
		else
			_ = builder.Services.AddSingleton<TelemetrySender, TelemetrySender_Custom>();

		return builder;
	}

	public static T AddKeyValue<T>(this T configuration, params (string key, string value)[] tuple)
		where T : IConfigurationBuilder
	{
		_ = configuration
			.AddInMemoryCollection(tuple.Select(i =>
				new KeyValuePair<string, string>(i.key, i.value)));

		return configuration;
	}
}

public class TelemetrySender(IConfiguration Configuration)
{
	public virtual void Run() => Console.WriteLine($"key = {Configuration["key"]}");
}


public class TelemetrySender_Default(IConfiguration Configuration) : TelemetrySender(Configuration)
{
	public override void Run() => Console.WriteLine(nameof(TelemetrySender_Default));
}


public class TelemetrySender_Custom(IConfiguration Configuration) : TelemetrySender(Configuration)
{
	public override void Run() => Console.WriteLine(nameof(TelemetrySender_Custom));
}


public static class CosmicHost
{
	public static ExtendedIHostApplicationBuilder R9Builder()
		=> new ExtendedIHostApplicationBuilder();

	public static ExtendedIHostApplicationBuilder2 R9Builder2()
		=> new ExtendedIHostApplicationBuilder2();

	public static HostApplicationBuilder R9Builder3() => Host
		.CreateApplicationBuilder()
		.AddR9Goodies_Generic();


	public static HostApplicationBuilder R9Builder4() => Host
		.CreateApplicationBuilder()
		.ExampleOfWhatNOTToDo();
}

public class ExtendedIHostApplicationBuilder : IHostApplicationBuilder
{
	private readonly HostApplicationBuilder _innerBuilder;

	public ExtendedIHostApplicationBuilder()
	{
		_innerBuilder = Host.CreateApplicationBuilder();

		_innerBuilder.AddR9Goodies_Generic();
	}

	#region Interface implementation

	// Properties, getter only: you give control to the user to add/configure stuff,
	// which will happen immediately, and more importantly after our default goodies are executed

	public IDictionary<object, object> Properties => (_innerBuilder as IHostApplicationBuilder).Properties;

	public IConfigurationManager Configuration => _innerBuilder.Configuration;

	public IHostEnvironment Environment => _innerBuilder.Environment;

	public ILoggingBuilder Logging => _innerBuilder.Logging;

	public IMetricsBuilder Metrics => _innerBuilder.Metrics;

	public IServiceCollection Services => _innerBuilder.Services;

	public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder> configure)
	{
		_innerBuilder.ConfigureContainer(factory, configure);
	}

	#endregion

	/// <summary>
	/// NOT part of the interface
	/// </summary>
	public IHost Build()
	{
		// Lots of stuff happening

		return _innerBuilder.Build();
	}
}

public class ExtendedIHostApplicationBuilder2 : IHostApplicationBuilder
{
	private readonly HostApplicationBuilder _innerBuilder;

	public ExtendedIHostApplicationBuilder2()
	{
		_innerBuilder = Host.CreateApplicationBuilder();

		_innerBuilder.AddR9Goodies_Generic();
	}

	#region Interface implementation

	IDictionary<object, object> IHostApplicationBuilder.Properties => (_innerBuilder as IHostApplicationBuilder)!.Properties;

	IConfigurationManager IHostApplicationBuilder.Configuration => _innerBuilder.Configuration;

	IHostEnvironment IHostApplicationBuilder.Environment => _innerBuilder.Environment;

	ILoggingBuilder IHostApplicationBuilder.Logging => _innerBuilder.Logging;

	IMetricsBuilder IHostApplicationBuilder.Metrics => _innerBuilder.Metrics;

	IServiceCollection IHostApplicationBuilder.Services => _innerBuilder.Services;

	void IHostApplicationBuilder.ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder> configure)
	{
		_innerBuilder.ConfigureContainer(factory, configure);
	}

	#endregion

	/// <summary>
	/// NOT part of the interface
	/// </summary>
	public IHost Build()
	{
		// Lots of stuff happening

		return _innerBuilder.Build();
	}
}

// You can define other methods, fields, classes and namespaces here
