using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Devices.Controls
{
	/// <summary>
	/// Base class for device controls that wrap a specific device type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AbstractDeviceControl<T> : IDeviceControl
		where T : IDevice
	{
		private readonly T m_Parent;
		private readonly int m_Id;

		#region Properties

		/// <summary>
		/// Gets the parent device for this control.
		/// </summary>
		IDevice IDeviceControl.Parent { get { return Parent; } }

		/// <summary>
		/// Gets the id for this control.
		/// </summary>
		public int Id { get { return m_Id; } }

		/// <summary>
		/// Gets the parent device for this control.
		/// </summary>
		[PublicAPI]
		public T Parent { get { return m_Parent; } }

		/// <summary>
		/// Gets the human readable name for this control.
		/// </summary>
		public virtual string Name { get { return null; } }

		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		public virtual string ConsoleName { get { return Name; } }

		/// <summary>
		/// Gets the help information for the node.
		/// </summary>
		public virtual string ConsoleHelp { get { return string.Empty; } }

		/// <summary>
		/// Returns true if this instance has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		protected AbstractDeviceControl(T parent, int id)
		{
			m_Id = id;
			m_Parent = parent;
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~AbstractDeviceControl()
		{
			Dispose(false);
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Gets the string representation for this instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}(Id={1}, Parent={2})", GetType().Name, Id, Parent.Id);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Releases resources but also allows for finalizing without touching managed resources.
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			if (!IsDisposed)
				DisposeFinal(disposing);
			IsDisposed = IsDisposed || disposing;
		}

		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void DisposeFinal(bool disposing)
		{
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			yield break;
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public virtual void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			addRow("Name", Name);
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
