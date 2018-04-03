using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
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
		public PanTiltControl(T parent, int id)
			: base(parent, id)
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
