using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Devices;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class CameraDeviceControl : AbstractCameraDeviceControl<ICameraDevice>
	{
		public override event EventHandler<CameraControlPresetsChangedApiEventArgs> OnPresetsChanged;
		public override event EventHandler<CameraControlFeaturesChangedApiEventArgs> OnSupportedCameraFeaturesChanged;
		public override event EventHandler<CameraControlMuteChangedApiEventArgs> OnCameraMuteStateChanged;

		/// <summary>
		/// Gets whether the camera is currently muted
		/// </summary>
		public override bool IsCameraMuted { get { return Parent.IsCameraMuted; } }

		/// <summary>
		/// Flags which indicate which features this camera can support
		/// </summary>
		public override eCameraFeatures SupportedCameraFeatures { get { return Parent.SupportedCameraFeatures; } }

		/// <summary>
		/// Gets the maximum number of presets this camera can support.
		/// </summary>
		public override int MaxPresets { get { return Parent.MaxPresets; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public CameraDeviceControl(ICameraDevice parent, int id) : base(parent, id)
		{
		}

		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			OnPresetsChanged = null;
			OnSupportedCameraFeaturesChanged = null;
			OnCameraMuteStateChanged = null;

			base.DisposeFinal(disposing);
		}

		#region Pan

		public override void PanStop()
		{
			if(SupportedCameraFeatures.HasFlag(eCameraFeatures.Pan))
				Parent.Pan(eCameraPanAction.Stop);
		}

		public override void PanLeft()
		{
			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Pan))
				Parent.Pan(eCameraPanAction.Left);
		}

		public override void PanRight()
		{
			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Pan))
				Parent.Pan(eCameraPanAction.Right);
		}

		#endregion

		#region Tilt

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		public override void TiltStop()
		{
			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Tilt))
				Parent.Tilt(eCameraTiltAction.Stop);
		}

		public override void TiltUp()
		{
			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Tilt))
				Parent.Tilt(eCameraTiltAction.Up);
		}

		public override void TiltDown()
		{
			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Tilt))
				Parent.Tilt(eCameraTiltAction.Down);
		}

		#endregion

		#region Zoom

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		public override void ZoomStop()
		{
			Parent.Zoom(eCameraZoomAction.Stop);
		}

		/// <summary>
		/// Begin zooming the camera in.
		/// </summary>
		public override void ZoomIn()
		{
			Parent.Zoom(eCameraZoomAction.ZoomIn);
		}

		/// <summary>
		/// Begin zooming the camera out.
		/// </summary>
		public override void ZoomOut()
		{
			Parent.Zoom(eCameraZoomAction.ZoomOut);
		}

		#endregion

		#region Presets

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		public override void ActivatePreset(int presetId)
		{
			Parent.ActivatePreset(presetId);
		}

		/// <summary>
		/// Stores the camera's current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		public override void StorePreset(int presetId)
		{
			Parent.StorePreset(presetId);
		}

		/// <summary>
		/// Returns the presets, in order.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<CameraPreset> GetPresets()
		{
			return Parent.GetPresets();
		}

		#endregion

		#region Camera Mute

		/// <summary>
		/// Sets if the camera mute state should be active
		/// </summary>
		/// <param name="enable"></param>
		public override void MuteCamera(bool enable)
		{
			Parent.MuteCamera(enable);
		}

		#endregion

		#region Camera Home

		/// <summary>
		/// Resets camera to its predefined home position
		/// </summary>
		public override void ActivateHome()
		{
			Parent.ActivateHome();
		}

		/// <summary>
		/// Stores the current position as the home position.
		/// </summary>
		public override void StoreHome()
		{
			Parent.StoreHome();
		}

		#endregion

		#region Event Subscriptions

		/// <summary>
		/// Subscribe to the parent events.
		/// </summary>
		/// <param name="parent"></param>
		protected override void Subscribe(ICameraDevice parent)
		{
			base.Subscribe(parent);

			parent.OnPresetsChanged += ParentOnPresetsChanged;
			parent.OnCameraMuteStateChanged += ParentOnCameraMuteStateChanged;
			parent.OnSupportedCameraFeaturesChanged += ParentOnSupportedCameraFeaturesChanged;
		}

		/// <summary>
		/// Unsubscribe from the parent events.
		/// </summary>
		/// <param name="parent"></param>
		protected override void Unsubscribe(ICameraDevice parent)
		{
			base.Unsubscribe(parent);

			parent.OnPresetsChanged -= ParentOnPresetsChanged;
			parent.OnCameraMuteStateChanged -= ParentOnCameraMuteStateChanged;
			parent.OnSupportedCameraFeaturesChanged -= ParentOnSupportedCameraFeaturesChanged;
		}

		/// <summary>
		/// Called when the parent presets change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ParentOnPresetsChanged(object sender, EventArgs eventArgs)
		{
			OnPresetsChanged.Raise(this, new CameraControlPresetsChangedApiEventArgs(GetPresets()));
		}

		/// <summary>
		/// Called when the parent mute state changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ParentOnCameraMuteStateChanged(object sender, BoolEventArgs args)
		{
			OnCameraMuteStateChanged.Raise(this, new CameraControlMuteChangedApiEventArgs(args.Data));
		}

		/// <summary>
		/// Called when the parent supported features change
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ParentOnSupportedCameraFeaturesChanged(object sender, GenericEventArgs<eCameraFeatures> args)
		{
			OnSupportedCameraFeaturesChanged.Raise(this, new CameraControlFeaturesChangedApiEventArgs(args.Data));
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

			foreach (IConsoleNodeBase node in CameraDeviceControlConsole.GetConsoleNodes(this, SupportedCameraFeatures))
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

			CameraDeviceControlConsole.BuildConsoleStatus(this, addRow, SupportedCameraFeatures);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in CameraDeviceControlConsole.GetConsoleCommands(this, SupportedCameraFeatures))
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