using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChangeLetters.Swagger;

/// <summary> 
/// Class AddEnumDescriptions.
/// Implements <see cref="IDocumentFilter" />
/// </summary>
public class AddEnumDescriptions : IDocumentFilter
{
    private readonly Type[] _enumTypes;
    private readonly List<string> _allTypeNames;

    /// <summary> Initializes a new instance of the <see cref="AddEnumDescriptions"/> class. </summary>
    public AddEnumDescriptions()
    {
        var allTypes = Assembly.GetExecutingAssembly().GetExportedTypes();
        var enumTypes = allTypes.Where(t => t.BaseType == typeof(Enum)).ToList();
        _allTypeNames = allTypes.Select(t => t.Name).ToList();
        var solutionAssemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(a => a.Name!.StartsWith("ChangeLetters", StringComparison.InvariantCultureIgnoreCase)).ToArray();
        foreach (var assemblyName in solutionAssemblyNames)
        {
            var reference = Assembly.Load(assemblyName);
            var currentTypes = reference.GetExportedTypes();
            enumTypes.AddRange(currentTypes.Where(t => t.BaseType == typeof(Enum)).ToList());
            _allTypeNames.AddRange(currentTypes.Select(t => t.Name));
        }

        _enumTypes = enumTypes.ToArray();
    }

    /// <inheritdoc />
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        RemoveUselessSchemas(swaggerDoc);

        if (swaggerDoc.Paths.Any())
        {
            foreach (var pathItem in swaggerDoc.Paths)
            {
                foreach (var openApiOperation in pathItem.Value.Operations)
                {
                    foreach (var openApiParameter in openApiOperation.Value.Parameters)
                    {
                        if (openApiParameter.Schema.Reference != null)
                        {
                            var type = _enumTypes.FirstOrDefault(t => t.Name == openApiParameter.Schema.Reference?.Id);
                            var myEnum = swaggerDoc.Components.Schemas.FirstOrDefault(kvp => kvp.Key == openApiParameter.Schema.Reference?.Id);
                            if (myEnum.Value != null && type != null)
                            {
                                var descriptions = GetDescriptions(myEnum.Value, type);
                                openApiParameter.Description += $"\n\n Values of {openApiParameter.Schema.Reference?.Id}\n\n"
                                + string.Join(", \n\n", descriptions);
                            }
                        }

                    }

                    foreach (var response in openApiOperation.Value.Responses)
                    {
                        if (response.Value.Description.Contains("$ref:"))
                        {
                            string typeName = GetTypeName(response.Value.Description);
                            if (!string.IsNullOrEmpty(typeName))
                            {
                                var type = _enumTypes.FirstOrDefault(t => t.FullName == typeName);

                                if (type != null)
                                {
                                    response.Value.Description = response.Value.Description.Replace("$ref:", "\n\n");
                                    var values = Enum.GetValues(type);
                                    foreach (object value in values)
                                    {
                                        response.Value.Description += $"\n\n{Convert.ToInt32(value)}: {value}";
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void RemoveUselessSchemas(OpenApiDocument swaggerDoc)
    {
        var uselessSchemas = new List<string>();
        foreach (KeyValuePair<string, OpenApiSchema> kvp in swaggerDoc.Components.Schemas)
        {
            var schema = kvp.Value;
            if (schema.Enum.Any())
            {
                var type = _enumTypes.FirstOrDefault(t => t.Name == kvp.Key);
                if (type != null)
                {
                    var descriptions = GetDescriptions(schema, type);
                    schema.Description += "\n\n" + string.Join(", \n\n", descriptions);
                }
                else
                    uselessSchemas.Add(kvp.Key);
            }
            else if (!_allTypeNames.Contains(kvp.Key))
                uselessSchemas.Add(kvp.Key);

        }

        foreach (var foreignEnum in uselessSchemas)
            swaggerDoc.Components.Schemas.Remove(foreignEnum);
    }

    private string GetTypeName(string valueDescription)
    {
        var typePart = valueDescription.Split(' ').FirstOrDefault(p => p.StartsWith("$ref:"));
        if (!string.IsNullOrEmpty(typePart) && typePart.Length > 5)
            return typePart.Substring(5);
        return string.Empty;
    }

    private static List<string> GetDescriptions(OpenApiSchema schema, Type type)
    {
        var descriptions = new List<string>();
        foreach (var any in schema.Enum)
        {
            if (any.GetType() == typeof(Microsoft.OpenApi.Any.OpenApiInteger))
            {
                var value = ((Microsoft.OpenApi.Any.OpenApiInteger)any).Value;
                descriptions.Add($"{value}: {Enum.GetName(type, value)}");
            }
        }

        return descriptions;
    }
}