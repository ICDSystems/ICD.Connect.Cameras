using System;
using ICD.Common.EventArguments;
using ICD.Connect.Devices;

namespace ICD.Connect.Partitions.Devices
{
	/// <summary>
	/// IPartitionDevice simply notifies if a partition has been opened.
	/// </summary>
	public interface IPartitionDevice : IDevice
	{
		/// <summary>
		/// Raised when the partition is detected as open or closed.
		/// </summary>
		event EventHandler<BoolEventArgs> OnOpenStatusChanged;

		/// <summary>
		/// Returns the current open state of the partition.
		/// </summary>
		bool IsOpen { get; }
	}
}
