﻿using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.Cameras.Devices;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class PanTiltControl<T> : AbstractCameraDeviceControl<T>, IPanTiltControl
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
			PanTilt(eCameraPanTiltAction.Stop);
		}

		public void PanLeft()
		{
			PanTilt(eCameraPanTiltAction.Left);
		}

		public void PanRight()
		{
			PanTilt(eCameraPanTiltAction.Right);
		}

		public void TiltUp()
		{
			PanTilt(eCameraPanTiltAction.Up);
		}

		public void TiltDown()
		{
			PanTilt(eCameraPanTiltAction.Down);
		}

		public void PanTilt(eCameraPanTiltAction action)
		{
			Parent.PanTilt(action);
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