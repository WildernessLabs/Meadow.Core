namespace Meadow.Cloud;

/// <summary>
/// Wrapper to hold raw JSON that can be re-serialized correctly
/// Prevents double-deserialization/serialization which loses type info
/// </summary>
internal class JsonWrapper
{
    private readonly string _rawJson;

    public JsonWrapper(string rawJson)
    {
        _rawJson = rawJson;
    }

    /// <summary>
    /// Returns the raw JSON when accessed - allows MicroJson.Serialize to pass through
    /// </summary>
    public string ToJson()
    {
        return _rawJson;
    }
}
