using System;
namespace Meadow
{
	/// <summary>
    /// Provides an abstraction for OS services such as configuration so that
    /// Meadow can operate on different OS's and platforms.
    /// </summary>
	public partial interface IPlatformOS
	{
		void Initialize();


	}
}
