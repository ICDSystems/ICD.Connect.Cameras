﻿using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Devices;

namespace ICD.Connect.Cameras.Devices
{
	public abstract class AbstractCameraDevice<TSettings> : AbstractDevice<TSettings>, ICameraDevice
		where TSettings : ICameraDeviceSettings, new()
	{
		private eCameraFeatures m_SupportedCameraFeatures;
		public abstract event EventHandler<GenericEventArgs<IEnumerable<CameraPreset>>> OnPresetsChanged;
		public event EventHandler<GenericEventArgs<eCameraFeatures>> OnSupportedCameraFeaturesChanged;
		public abstract event EventHandler<BoolEventArgs> OnCameraMuteStateChanged;

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
		public abstract bool IsCameraMuted { get; }

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
		public abstract void SendCameraHome();

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			if(SupportedCameraFeatures.HasFlag(eCameraFeatures.Pan))
				CameraWithPanConsole.BuildConsoleStatus(this, addRow);
			if(SupportedCameraFeatures.HasFlag(eCameraFeatures.Tilt))
				CameraWithTiltConsole.BuildConsoleStatus(this, addRow);
			if(SupportedCameraFeatures.HasFlag(eCameraFeatures.Zoom))
				CameraWithZoomConsole.BuildConsoleStatus(this, addRow);
			if(SupportedCameraFeatures.HasFlag(eCameraFeatures.Presets))
				CameraWithPresetsConsole.BuildConsoleStatus(this, addRow);
			if(SupportedCameraFeatures.HasFlag(eCameraFeatures.Mute))
				CameraWithMuteConsole.BuildConsoleStatus(this, addRow);
			if(SupportedCameraFeatures.HasFlag(eCameraFeatures.Home))
				CameraWithHomeConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			if(SupportedCameraFeatures.HasFlag(eCameraFeatures.Pan))
				foreach (IConsoleCommand command in CameraWithPanConsole.GetConsoleCommands(this))
					yield return command;

			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Tilt))
				foreach (IConsoleCommand command in CameraWithTiltConsole.GetConsoleCommands(this))
					yield return command;

			if(SupportedCameraFeatures.HasFlag(eCameraFeatures.Zoom))
				foreach (IConsoleCommand command in CameraWithZoomConsole.GetConsoleCommands(this))
					yield return command;

			if(SupportedCameraFeatures.HasFlag(eCameraFeatures.Presets))
				foreach (IConsoleCommand command in CameraWithPresetsConsole.GetConsoleCommands(this))
					yield return command;

			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Mute))
				foreach (IConsoleCommand command in CameraWithMuteConsole.GetConsoleCommands(this))
					yield return command;

			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Home))
				foreach (IConsoleCommand command in CameraWithHomeConsole.GetConsoleCommands(this))
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

			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Pan))
				foreach (IConsoleNodeBase node in CameraWithPanConsole.GetConsoleNodes(this))
					yield return node;

			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Tilt))
				foreach (IConsoleNodeBase node in CameraWithTiltConsole.GetConsoleNodes(this))
					yield return node;

			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Zoom))
				foreach (IConsoleNodeBase node in CameraWithZoomConsole.GetConsoleNodes(this))
					yield return node;

			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Presets))
				foreach (IConsoleNodeBase node in CameraWithPresetsConsole.GetConsoleNodes(this))
					yield return node;

			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Mute))
				foreach (IConsoleNodeBase node in CameraWithMuteConsole.GetConsoleNodes(this))
					yield return node;

			if (SupportedCameraFeatures.HasFlag(eCameraFeatures.Home))
				foreach (IConsoleNodeBase node in CameraWithHomeConsole.GetConsoleNodes(this))
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
