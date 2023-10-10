using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.CompilerServices;

namespace Meadow;

/// <summary>
/// Represents a configurable object.
/// </summary>
public abstract class ConfigurableObject
{
    /// <summary>
    /// The default JSON file name for configuration.
    /// </summary>
    public const string DefaultJsonFileName = "app.config.json";

    /// <summary>
    /// The default YAML file name for configuration.
    /// </summary>
    public const string DefaultYamlFileName = "app.config.yaml";

    private string m_parentname = default!;
    private bool m_isArrayElement = false;

    private string PathTypeName { get; }

    /// <summary>
    /// Gets or sets the configuration root.
    /// </summary>
    protected IConfiguration? ConfigurationRoot { get; private set; } = default!;

    /// <summary>
    /// Gets the configuration root path.
    /// </summary>
    protected string ConfigurationRootPath { get; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurableObject"/> class.
    /// </summary>
    protected ConfigurableObject()
    {
        PathTypeName = this.GetType().Name.Replace("Settings", string.Empty);
        SetConfigRoot();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurableObject"/> class with a parent object.
    /// </summary>
    /// <param name="parent">The parent object.</param>
    protected ConfigurableObject(object? parent)
    {
        PathTypeName = this.GetType().Name.Replace("Settings", string.Empty);

        if (parent != null)
        {
            if (parent is ConfigurableObject p)
            {
                if (!string.IsNullOrEmpty(p!.m_parentname))
                {
                    m_parentname = $"{p!.m_parentname}:{parent.GetType().Name}";
                }
                else
                {
                    m_parentname = parent.GetType().Name;
                }
            }
            else
            {
                if (!parent.Equals(string.Empty))
                {
                    m_parentname = parent.GetType().Name;
                }
            }
            if (m_parentname != null && m_parentname.EndsWith("Settings")) m_parentname = m_parentname.Substring(0, m_parentname.Length - 8);
        }

        SetConfigRoot();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurableObject"/> class with a parent object and a configuration root path.
    /// </summary>
    /// <param name="parent">The parent object.</param>
    /// <param name="configRootPath">The configuration root path.</param>
    protected ConfigurableObject(object? parent, string? configRootPath)
        : this(parent)
    {
        ConfigurationRootPath = configRootPath ?? string.Empty;

        if (parent == null && configRootPath != null)
        {
            var idx = configRootPath.LastIndexOf(':');
            if (idx > 0)
            {
                if (int.TryParse(configRootPath.Substring(idx + 1), out int i))
                {
                    m_isArrayElement = true;
                }
            }
        }
    }

    private void SetConfigRoot()
    {
        var b = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
        b.AddYamlFile(DefaultYamlFileName, true);
        b.AddJsonFile(DefaultJsonFileName, true);

        try
        {
            this.ConfigurationRoot = b.Build();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("json"))
            {
                throw new Exception($"Error loading {DefaultJsonFileName}");
            }
            else if (ex.Message.Contains("yaml"))
            {
                throw new Exception($"Error loading {DefaultYamlFileName}");
            }
            else throw;
        }
    }


    /// <summary>
    /// Gets the configured float value for the specified name.
    /// </summary>
    /// <param name="name">The name of the configuration value.</param>
    /// <param name="defaultValue">The default value to return if the configuration value is not found or is invalid.</param>
    /// <returns>The configured float value.</returns>
    public float GetConfiguredFloat([CallerMemberName] string? name = null, float defaultValue = 0f)
    {
        if (string.IsNullOrWhiteSpace(name)) return defaultValue;
        try
        {
            var stringVal = GetConfiguredValue(name);
            if (stringVal == null) return defaultValue;
            return Convert.ToSingle(stringVal);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Gets the configured boolean value for the specified name.
    /// </summary>
    /// <param name="name">The name of the configuration value.</param>
    /// <param name="defaultValue">The default value to return if the configuration value is not found or is invalid.</param>
    /// <returns>The configured boolean value.</returns>
    public bool GetConfiguredBool([CallerMemberName] string? name = null, bool defaultValue = false)
    {
        if (string.IsNullOrWhiteSpace(name)) return defaultValue;
        try
        {
            var stringVal = GetConfiguredValue(name);
            if (stringVal == null) return defaultValue;
            return Convert.ToBoolean(stringVal);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Gets the configured integer value for the specified name.
    /// </summary>
    /// <param name="name">The name of the configuration value.</param>
    /// <param name="defaultValue">The default value to return if the configuration value is not found or is invalid.</param>
    /// <returns>The configured integer value.</returns>
    public int GetConfiguredInt([CallerMemberName] string? name = null, int defaultValue = 0)
    {
        if (string.IsNullOrWhiteSpace(name)) return defaultValue;
        try
        {
            var stringVal = GetConfiguredValue(name);
            if (stringVal == null) return defaultValue;
            return Convert.ToInt32(stringVal);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Gets the configured string value for the specified name.
    /// </summary>
    /// <param name="name">The name of the configuration value.</param>
    /// <param name="defaultValue">The default value to return if the configuration value is not found or is invalid.</param>
    /// <returns>The configured string value.</returns>
    public string GetConfiguredString([CallerMemberName] string? name = null, string defaultValue = "")
    {
        if (string.IsNullOrWhiteSpace(name)) return defaultValue;
        try
        {
            var stringVal = GetConfiguredValue(name);
            if (stringVal == null) return defaultValue;
            return stringVal;
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Gets the configured value for the specified name.
    /// </summary>
    /// <param name="name">The name of the configuration value.</param>
    /// <returns>The configured value as a string, or null if not found.</returns>
    public string? GetConfiguredValue([CallerMemberName] string? name = null)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));

        string key;

        if (ConfigurationRoot == null) return null;

        if (ConfigurationRootPath != null)
        {
            if (m_parentname != null)
            {
                key = $"{ConfigurationRootPath}:{m_parentname}:{PathTypeName}:{name}".Replace('.', ':');
            }
            else
            {
                // if we're in an array, we may have no type name in the key
                if (m_isArrayElement)
                {
                    key = $"{ConfigurationRootPath}:{name}".Replace('.', ':');
                }
                else
                {
                    key = $"{ConfigurationRootPath}:{PathTypeName}:{name}".Replace('.', ':');
                }
            }
        }
        else if (m_parentname != null)
        {
            key = $"{m_parentname}:{PathTypeName}:{name}".Replace('.', ':');
        }
        else
        {
            key = $"{PathTypeName}:{name}".Replace('.', ':');
        }

        // ReSharper disable once PossibleNullReferenceException
        return this.ConfigurationRoot[key] ?? null;
    }
}
