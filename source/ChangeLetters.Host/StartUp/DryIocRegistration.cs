using DryIoc;
using System.Reflection;
using DryIoc.MefAttributedModel;
using DryIoc.Microsoft.DependencyInjection;

namespace ChangeLetters.StartUp;

/// <summary> 
/// Class DryIocRegistration. 
/// </summary>
internal class DryIocRegistration
{
    internal static void Initialize(HostBuilderContext hostContext, IContainer container)
    {
        var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "ChangeLetters*.dll")
            .Select(Assembly.LoadFrom)
            .Concat([Assembly.GetExecutingAssembly()])
            .ToList();
        container.RegisterExports(assemblies);
    }

    internal static DryIocServiceProviderFactory GetDryIocFactory()
        => new(GetContainer());

    internal static IContainer GetContainer()
        => new Container(Rules.Default.WithDefaultReuse(Reuse.Transient)
            .With(FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic))
            .WithDependencyInjectionAdapter()
            ;
}
