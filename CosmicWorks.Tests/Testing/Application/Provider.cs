namespace CosmicWorks.Tests.Testing.Application;

internal sealed class Provider : IServiceProvider
{
    private readonly object _value;
    public Provider(object value) => _value = value;
    public object? GetService(Type serviceType) => _value;
}