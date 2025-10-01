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
	#region Intro

	var builder0 = CosmicHost.CreateBuilder_2();
	var host0 = builder0.Build();

	var builder = CosmicHost.CreateBuilder();
	var host = builder.Build();

	#endregion

	#region Real use case

	var builder5 = CosmicHost.CreateBuilder_5();	// Invokes .AddR9Goodies_Generic
	
	// This will be executed after default stuff happening in CreateBuilder(), so it can't affect it
	// i.e. we can't add configuration/service overrides like this
	builder5.CustomerCodeOverridingOurR9GoodiesConfig();
	
	builder5.Build();

	#endregion
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

	public static TBuilder CustomerCodeOverridingOurR9GoodiesConfig<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		//_ = builder.Services.AddSingleton(...);
		//_ = builder.Configuration.AddInMemoryCollection(...);

		return builder;
	}
}

public static class CosmicHost
{
	#region Intro

	public static TBuilder CreateBuilder_0<TBuilder>()
			where TBuilder : IHostApplicationBuilder
		=> Host.CreateApplicationBuilder(new HostApplicationBuilderSettings());

	public static HostApplicationBuilder CreateBuilder()
		=> Host.CreateApplicationBuilder(new HostApplicationBuilderSettings());

	public static IHostApplicationBuilder CreateBuilder_2()
		=> Host.CreateApplicationBuilder(new HostApplicationBuilderSettings());

	#endregion

	#region Real use case

	// If we extend IHostApplicationBuilder, consumers won't be able to invoke .Build()
	public static IHostApplicationBuilder CreateBuilder_3()
		=> new ExtendedIHostApplicationBuilder();

	// We can't make ExtendedHostApplicationBuilder inherit from HostApplicationBuilder because the latter is sealed
	public static HostApplicationBuilder CreateBuilder_4()
		=> new ExtendedIHostApplicationBuilder();

	// We return a type that:
	// - Implements IHostApplicationBuilder
	// - Has a Build() method that returns an IHost
	public static ExtendedIHostApplicationBuilder CreateBuilder_5()
		=> new ExtendedIHostApplicationBuilder();

	#endregion
}


public class ExtendedHostBuilder : IHostBuilder
{
	private readonly IHostBuilder _wrappedHostBuilder;

	public IDictionary<object, object> Properties => throw new NotImplementedException();

	#region Rest of interface implementation

	// Methods, which we need to define: we save our own + user configuration as 'callbacks'/'commands'/'delegates'
	// and run them altogether at the end of the pipeline, on .Build()

	private readonly List<Action<IConfigurationBuilder>> _configureHostConfigurationActions = new();
	private readonly List<Action<ConfigurationManager, IServiceProvider>> _configureConfigurationActions = new();
	private readonly List<Action<IHostBuilder, IServiceProvider>> _configureHostBuilderActions = new();

	public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate) => throw new NotImplementedException();
	public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate) => throw new NotImplementedException();
	public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate) => throw new NotImplementedException();
	public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate) => throw new NotImplementedException();
	public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull => throw new NotImplementedException();
	public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull => throw new NotImplementedException();

	#endregion

	private readonly ConfigurationManager _bootConfig;
	private readonly IServiceProvider _bootServices;

	/// <summary>
	/// Part of interface
	/// </summary>
	public IHost Build()
	{
		// Lots of stuff happening

		foreach (var action in _configureHostConfigurationActions)
			action(_bootConfig);

		foreach (var action in _configureConfigurationActions)
			action(_bootConfig, _bootServices);

		foreach (var action in _configureHostBuilderActions)
			action(this, _bootServices);
		
		// ...

		return _wrappedHostBuilder.Build();
	}

}

public class ExtendedHostApplicationBuilder : HostApplicationBuilder
{

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
	// which will happen immediately, and more importantly after our default R9 goodies are executed
	// That means, our extension methods can't rely on user's configuration code
	// Rings a bell? See config override example of what we can't support
	
	IDictionary<object, object> IHostApplicationBuilder.Properties => (_innerBuilder as IHostApplicationBuilder).Properties;

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