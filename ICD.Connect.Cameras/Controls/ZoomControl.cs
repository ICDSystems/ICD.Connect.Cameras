using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Devices;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class ZoomControl<T> : AbstractCameraDeviceControl<T>, IZoomControl
		where T : ICameraWithZoom
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public ZoomControl(T parent, int id)
			: base(parent, id)
		{
		}

		#region IZoomControl

		public void Stop()
		{
			Zoom(eCameraZoomAction.Stop);
		}

		public void ZoomIn()
		{
			Zoom(eCameraZoomAction.ZoomIn);
		}

		public void ZoomOut()
		{
			Zoom(eCameraZoomAction.ZoomOut);
		}

		public void Zoom(eCameraZoomAction action)
		{
			Parent.Zoom(action);
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			foreach (IConsoleNodeBase node in ZoomControlConsole.GetConsoleNodes(this))
				yield return node;
		}

		/// <summary>
		/// Wrokaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			ZoomControlConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in ZoomControlConsole.GetConsoleCommands(this))
				yield return command;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}