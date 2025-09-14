namespace ChangeLetters.Configurations;

/// <summary> Class AppConfigAttribute.Inherits from <see cref="Attribute" />. </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AppConfigAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppConfigAttribute"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public AppConfigAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppConfigAttribute"/> class.
    /// </summary>
    public AppConfigAttribute(Type type)
    {
        Name = type.Name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppConfigAttribute"/> class.
    /// </summary>
    public AppConfigAttribute()
    {
        Name = string.Empty;
    }

    /// <summary>Gets the name of the configuration topic.</summary>
    public string Name { get; }
}
