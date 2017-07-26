using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;
using ICD.Common.Services.Logging;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Settings;

namespace ICD.Connect.Devices
{
	/// <summary>
	/// Base class for devices.
	/// </summary>
	public abstract class AbstractDeviceBase<T> : AbstractOriginator<T>, IDeviceBase, IConsoleNode
		where T : ISettings, new()
	{
		/// <summary>
		/// Raised when the device becomes online/offline.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnIsOnlineStateChanged;

		private bool m_IsOnline;

		#region Properties

		/// <summary>
		/// Returns true if the device hardware is detected by the system.
		/// </summary>
		public bool IsOnline
		{
			get { return m_IsOnline; }
			private set
			{
				if (value == m_IsOnline)
					return;

				m_IsOnline = value;

				Logger.AddEntry(eSeverity.Informational, "{0} - Online status changed to {1}", this, IsOnline);
				OnIsOnlineStateChanged.Raise(this, new BoolEventArgs(IsOnline));
			}
		}

		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		public virtual string ConsoleName { get { return string.IsNullOrEmpty(Name) ? GetType().Name : Name; } }

		/// <summary>
		/// Gets the help information for the node.
		/// </summary>
		public virtual string ConsoleHelp { get { return string.Empty; } }

		#endregion

		private readonly DeviceControlsCollection m_Controls;

		#region Properties

		/// <summary>
		/// Gets the controls for this device.
		/// </summary>
		public DeviceControlsCollection Controls { get { return m_Controls; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractDeviceBase()
		{
			m_Controls = new DeviceControlsCollection();

			Name = GetType().Name;
			UpdateCachedOnlineStatus();
		}

		#region Private Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnIsOnlineStateChanged = null;

			Controls.Dispose();

			base.DisposeFinal(disposing);
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected abstract bool GetIsOnlineStatus();

		/// <summary>
		/// Updates the cached online status and raises the OnIsOnlineStateChanged event if the cache changes.
		/// </summary>
		protected void UpdateCachedOnlineStatus()
		{
			IsOnline = GetIsOnlineStatus();
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			yield return ConsoleNodeGroup.IndexNodeMap("Controls", Controls);
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public virtual void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			addRow("ID", Id);
			addRow("Name", Name);
			addRow("Online", IsOnline);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			yield break;
		}

		#endregion
	}
}
