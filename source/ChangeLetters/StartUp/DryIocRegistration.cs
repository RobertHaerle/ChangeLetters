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
        var assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
            .Where(a => a.Name?.StartsWith("ChangeLetters") ?? false)
            .Select(Assembly.Load)
            .Concat([Assembly.GetExecutingAssembly()])
            .ToList();
        container.RegisterExports(assemblies);
    }

    internal static DryIocServiceProviderFactory GetDryIocFactory()
        => new(new Container(Rules.Default.WithDefaultReuse(Reuse.Transient))
            .WithDependencyInjectionAdapter());
}
