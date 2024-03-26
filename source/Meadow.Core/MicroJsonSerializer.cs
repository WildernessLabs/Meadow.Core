using Meadow.Foundation.Serialization;
using System;

namespace Meadow;

/// <summary>
/// An implementation of IJsonSerializer that uses MicroJson 
/// </summary>
public class MicroJsonSerializer : IJsonSerializer
{
    /// <inheritdoc/>
    public T Deserialize<T>(string json)
    {
        return MicroJson.Deserialize<T>(json);
    }

    /// <inheritdoc/>
    public object Deserialize(string json, Type type)
    {
        return MicroJson.Deserialize(json, type);
    }

    /// <inheritdoc/>
    public string? Serialize(object source)
    {
        return MicroJson.Serialize(source);
    }
}
