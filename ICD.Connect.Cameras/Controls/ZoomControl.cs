using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class ZoomControl<T> : AbstractDeviceControl<T>, IZoomControl
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
			Parent.Zoom(eCameraZoomAction.Stop);
		}

		public void ZoomIn()
		{
			Parent.Zoom(eCameraZoomAction.ZoomIn);
		}

		public void ZoomOut()
		{
			Parent.Zoom(eCameraZoomAction.ZoomOut);
		}
		#endregion

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("Stop", "Sends the stop signal to the camera.", () => Stop());
			yield return new ConsoleCommand("ZoomIn", "Sends the zoom in signal to the camera.", () => ZoomIn());
			yield return new ConsoleCommand("ZoomOut", "Sends the zoom out signal to the camera.", () => ZoomOut());
		}

		/// <summary>
		/// Workaround for the "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}