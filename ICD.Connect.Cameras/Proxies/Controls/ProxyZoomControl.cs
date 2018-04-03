using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Devices.Proxies.Devices;

namespace ICD.Connect.Cameras.Proxies.Controls
{
	public sealed class ProxyZoomControl : AbstractProxyCameraDeviceControl, IZoomControl
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public ProxyZoomControl(IProxyDeviceBase parent, int id)
			: base(parent, id)
		{
		}

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		public void Stop()
		{
			CallMethod(CameraControlApi.METHOD_STOP);
		}

		/// <summary>
		/// Begin zooming the camera in.
		/// </summary>
		public void ZoomIn()
		{
			CallMethod(CameraControlApi.METHOD_ZOOM_IN);
		}

		/// <summary>
		/// Begin zooming the camera out.
		/// </summary>
		public void ZoomOut()
		{
			CallMethod(CameraControlApi.METHOD_ZOOM_OUT);
		}

		/// <summary>
		/// Performs the given zoom action.
		/// </summary>
		public void Zoom(eCameraZoomAction action)
		{
			CallMethod(CameraControlApi.METHOD_ZOOM, action);
		}

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
