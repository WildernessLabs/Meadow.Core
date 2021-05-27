namespace Meadow.Hardware
{
    /// <summary>
    /// Interface providing nuttx-specific methods for accessing IO
    /// </summary>
    internal interface INuttxIOController
    {
        void ReassertConfig(IPin pin);
    }
}
