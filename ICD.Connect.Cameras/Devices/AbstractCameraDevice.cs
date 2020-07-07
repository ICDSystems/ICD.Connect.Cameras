using System;
using System.Collections.Generic;
using ICD.Common.Logging.Activities;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Devices;
using ICD.Common.Logging.LoggingContexts;

namespace ICD.Connect.Cameras.Devices
{
	public abstract class AbstractCameraDevice<TSettings> : AbstractDevice<TSettings>, ICameraDevice
		where TSettings : ICameraDeviceSettings, new()
	{
		public event EventHandler<GenericEventArgs<IEnumerable<CameraPreset>>> OnPresetsChanged;
		public event EventHandler<GenericEventArgs<eCameraFeatures>> OnSupportedCameraFeaturesChanged;
		public event EventHandler<BoolEventArgs> OnCameraMuteStateChanged;

		private eCameraFeatures m_SupportedCameraFeatures;
		private bool m_IsCameraMuted;

		#region Properties

		/// <summary>
		/// Flags which indicate which features this camera can support
		/// </summary>
		public eCameraFeatures SupportedCameraFeatures
		{
			get { return m_SupportedCameraFeatures; }
			protected set
			{
				if(m_SupportedCameraFeatures == value)
					return;

				m_SupportedCameraFeatures = value;

				Logger.LogSetTo(eSeverity.Informational, "SupportedCameraFeatures", m_SupportedCameraFeatures);

				OnSupportedCameraFeaturesChanged.Raise(this, new GenericEventArgs<eCameraFeatures>(m_SupportedCameraFeatures));
			}
		}

		/// <summary>
		/// Gets the maximum number of presets this camera can support
		/// </summary>
		public abstract int MaxPresets { get; }

		/// <summary>
		/// Gets whether the camera is currently muted
		/// </summary>
		public bool IsCameraMuted
		{
			get { return m_IsCameraMuted; }
			protected set
			{
				if (value == m_IsCameraMuted)
					return;

				m_IsCameraMuted = value;

				Logger.LogSetTo(eSeverity.Informational, "IsCameraMuted", m_IsCameraMuted);
				Activities.LogActivity(m_IsCameraMuted
					                   ? new Activity(Activity.ePriority.Medium, "Camera Muted", "Camera Muted",
					                                  eSeverity.Informational)
					                   : new Activity(Activity.ePriority.Low, "Camera Muted", "Camera Unmuted",
					                                  eSeverity.Informational));

				OnCameraMuteStateChanged.Raise(this, new BoolEventArgs(m_IsCameraMuted));
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnPresetsChanged = null;
			OnSupportedCameraFeaturesChanged = null;
			OnCameraMuteStateChanged = null;

			base.DisposeFinal(disposing);
		}

		/// <summary>
		/// Begins panning the camera
		/// </summary>
		/// <param name="action"></param>
		public abstract void Pan(eCameraPanAction action);

		/// <summary>
		/// Begin tilting the camera.
		/// </summary>
		public abstract void Tilt(eCameraTiltAction action);

		/// <summary>
		/// Zooms the camera.
		/// </summary>
		public abstract void Zoom(eCameraZoomAction action);

		/// <summary>
		/// Gets the stored camera presets.
		/// </summary>
		public abstract IEnumerable<CameraPreset> GetPresets();

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		public abstract void ActivatePreset(int presetId);

		/// <summary>
		/// Stores the cameras current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		public abstract void StorePreset(int presetId);

		/// <summary>
		/// Sets if the camera mute state should be active
		/// </summary>
		/// <param name="enable"></param>
		public abstract void MuteCamera(bool enable);

		/// <summary>
		/// Resets camera to its predefined home position
		/// </summary>
		public abstract void ActivateHome();

		/// <summary>
		/// Stores the current position as the home position.
		/// </summary>
		public abstract void StoreHome();

		#endregion

		#region Private Methods

		/// <summary>
		/// Raised the presets changed event.
		/// </summary>
		protected void RaisePresetsChanged()
		{
			IEnumerable<CameraPreset> data = GetPresets();
			OnPresetsChanged.Raise(this, new GenericEventArgs<IEnumerable<CameraPreset>>(data));
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			CameraDeviceConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in CameraDeviceConsole.GetConsoleCommands(this))
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

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			foreach (IConsoleNodeBase node in CameraDeviceConsole.GetConsoleNodes(this))
				yield return node;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		#endregion
	}
}
