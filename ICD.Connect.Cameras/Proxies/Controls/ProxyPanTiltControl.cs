using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Devices.Proxies.Devices;

namespace ICD.Connect.Cameras.Proxies.Controls
{
	public sealed class ProxyPanTiltControl : AbstractProxyCameraDeviceControl, IPanTiltControl
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public ProxyPanTiltControl(IProxyDeviceBase parent, int id)
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
		/// Begin panning the camera to the left.
		/// </summary>
		public void PanLeft()
		{
			CallMethod(CameraControlApi.METHOD_PAN_LEFT);
		}

		/// <summary>
		/// Begin panning the camera to the right.
		/// </summary>
		public void PanRight()
		{
			CallMethod(CameraControlApi.METHOD_PAN_RIGHT);
		}

		/// <summary>
		/// Begin tilting the camera up.
		/// </summary>
		public void TiltUp()
		{
			CallMethod(CameraControlApi.METHOD_TILT_UP);
		}

		/// <summary>
		/// Begin tilting the camera down.
		/// </summary>
		public void TiltDown()
		{
			CallMethod(CameraControlApi.METHOD_TILT_DOWN);
		}

		/// <summary>
		/// Perfoms the given pan/tilt action.
		/// </summary>
		/// <param name="action"></param>
		public void PanTilt(eCameraPanTiltAction action)
		{
			CallMethod(CameraControlApi.METHOD_PAN_TILT, action);
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

			foreach (IConsoleNodeBase node in PanTiltControlConsole.GetConsoleNodes(this))
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

			PanTiltControlConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in PanTiltControlConsole.GetConsoleCommands(this))
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
