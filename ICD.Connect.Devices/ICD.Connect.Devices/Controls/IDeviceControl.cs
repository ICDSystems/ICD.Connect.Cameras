using ICD.Common.Utils;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Devices.Controls
{
	/// <summary>
	/// IDeviceControl contains the usage features for a parent device.
	/// </summary>
	public interface IDeviceControl : IConsoleNode, IStateDisposable
	{
		/// <summary>
		/// Gets the parent device for this control.
		/// </summary>
		IDevice Parent { get; }

		/// <summary>
		/// Gets the id for this control.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Gets the human readable name for this control.
		/// </summary>
		string Name { get; }
	}
}
