using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class PanTiltControl<T> : AbstractDeviceControl<T>, IPanTiltControl
		where T : ICameraWithPanTilt
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public PanTiltControl(T parent, int id) : base(parent, id)
		{
		}

		#region IPanTiltControl
		public void Stop()
		{
			Parent.PanTilt(eCameraPanTiltAction.Stop);
		}

		public void PanLeft()
		{
			Parent.PanTilt(eCameraPanTiltAction.Left);
		}

		public void PanRight()
		{
			Parent.PanTilt(eCameraPanTiltAction.Right);
		}

		public void TiltUp()
		{
			Parent.PanTilt(eCameraPanTiltAction.Up);
		}

		public void TiltDown()
		{
			Parent.PanTilt(eCameraPanTiltAction.Down);
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
			yield return new ConsoleCommand("Up", "Sends the tilt up signal to the camera.", () => TiltUp());
			yield return new ConsoleCommand("Down", "Sends the tilt down signal to the camera.", () => TiltDown());
			yield return new ConsoleCommand("Left", "Sends the pan left signal to the camera.", () => PanLeft());
			yield return new ConsoleCommand("Right", "Sends the pan right signal to the camera.", () => PanRight());
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