using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Mock;
using ICD.Connect.Settings;

namespace ICD.Connect.Cameras.Mock
{
	public sealed class MockCameraDevice : AbstractCameraDevice<MockCameraDeviceSettings>, IDeviceWithPower, IMockDevice
	{
		private readonly Dictionary<int, CameraPosition> m_PresetPositions;
		private readonly Dictionary<int, CameraPreset> m_Presets;

		private bool m_IsOnline;
		private bool m_Powered;

		private int m_VPosition;
		private int m_HPosition;
		private int m_ZPosition;

		private int m_HomeHPosition;
		private int m_HomeVPosition;
		private int m_HomeZPosition;

		private int? m_PanTiltSpeed;
		private int? m_ZoomSpeed;

		#region Properties

		/// <summary>
		/// Gets the maximum number of presets this camera can support
		/// </summary>
		public override int MaxPresets { get { return 4; } }

		public bool DefaultOffline { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public MockCameraDevice()
		{
			m_Presets = new Dictionary<int, CameraPreset>();
			m_PresetPositions = new Dictionary<int, CameraPosition>();

			SupportedCameraFeatures =
				eCameraFeatures.PanTiltZoom |
				eCameraFeatures.Presets |
				eCameraFeatures.Mute |
				eCameraFeatures.Home;

			Controls.Add(new GenericCameraRouteSourceControl<MockCameraDevice>(this, 0));
			Controls.Add(new CameraDeviceControl(this, 1));
			Controls.Add(new PowerDeviceControl<MockCameraDevice>(this, 3));
		}

		public void SetIsOnlineState(bool isOnline)
		{
			m_IsOnline = isOnline;
			UpdateCachedOnlineStatus();
		}

		#region PTZ

		/// <summary>
		/// Begins panning the camera
		/// </summary>
		/// <param name="action"></param>
		public override void Pan(eCameraPanAction action)
		{
			int speed = m_PanTiltSpeed == null ? 1 : m_PanTiltSpeed.Value;
			switch (action)
			{
				case eCameraPanAction.Left:
					m_HPosition = MathUtils.Clamp(m_HPosition - speed, int.MinValue, int.MaxValue);
					break;
				case eCameraPanAction.Right:
					m_HPosition = MathUtils.Clamp(m_HPosition + speed, int.MinValue, int.MaxValue);
					break;
				case eCameraPanAction.Stop:
					break;
				default:
					throw new ArgumentOutOfRangeException("action");
			}
			LogCameraMovement(action.ToString());
		}

		/// <summary>
		/// Begin tilting the camera.
		/// </summary>
		public override void Tilt(eCameraTiltAction action)
		{
			int speed = m_PanTiltSpeed == null ? 1 : m_PanTiltSpeed.Value;
			switch (action)
			{
				case eCameraTiltAction.Up:
					m_VPosition = MathUtils.Clamp(m_VPosition + speed, int.MinValue, int.MaxValue);
					break;
				case eCameraTiltAction.Down:
					m_VPosition = MathUtils.Clamp(m_VPosition - speed, int.MinValue, int.MaxValue);
					break;
				case eCameraTiltAction.Stop:
					break;
				default:
					throw new ArgumentOutOfRangeException("action");
			}
			LogCameraMovement(action.ToString());
		}

		public override void Zoom(eCameraZoomAction action)
		{
			int speed = m_ZoomSpeed == null ? 1 : m_ZoomSpeed.Value;
			switch (action)
			{
				case eCameraZoomAction.ZoomIn:
					m_ZPosition = MathUtils.Clamp(m_ZPosition + speed, int.MinValue, int.MaxValue);
					break;
				case eCameraZoomAction.ZoomOut:
					m_ZPosition = MathUtils.Clamp(m_ZPosition - speed, int.MinValue, int.MaxValue);
					break;
				case eCameraZoomAction.Stop:
					break;
				default:
					throw new ArgumentOutOfRangeException("action");
			}
			LogCameraMovement(action.ToString());
		}

		#endregion

		#region Presets

		public override IEnumerable<CameraPreset> GetPresets()
		{
			return m_Presets.Values;
		}

		public override void ActivatePreset(int presetId)
		{
			if (presetId < 1 || presetId > MaxPresets)
			{
				Log(eSeverity.Warning, "Mock camera preset must be between 1 and {0}, preset was not loaded.", MaxPresets);
				return;
			}
			CameraPosition position = m_PresetPositions[presetId];
			m_HPosition = position.HPosition;
			m_VPosition = position.VPosition;
			m_ZPosition = position.ZPosition;
			LogCameraMovement(string.Format("Loaded Preset {0}", presetId));
		}

		public override void StorePreset(int presetId)
		{
			if (presetId < 1 || presetId > MaxPresets)
			{
				Log(eSeverity.Warning, "Mock camera preset must be between 1 and {0}, preset was not stored.", MaxPresets);
				return;
			}

			m_Presets.Add(presetId, new CameraPreset(presetId, string.Format("Preset {0}", presetId)));
			m_PresetPositions.Add(presetId,
			                      new CameraPosition {HPosition = m_HPosition, VPosition = m_VPosition, ZPosition = m_ZPosition});

			RaisePresetsChanged();
		}

		/// <summary>
		/// Sets if the camera mute state should be active
		/// </summary>
		/// <param name="enable"></param>
		public override void MuteCamera(bool enable)
		{
			IsCameraMuted = enable;
		}

		/// <summary>
		/// Resets camera to its predefined home position
		/// </summary>
		public override void ActivateHome()
		{
			m_HPosition = m_HomeHPosition;
			m_VPosition = m_HomeVPosition;
			m_ZPosition = m_HomeZPosition;
		}

		/// <summary>
		/// Stores the current position as the home position.
		/// </summary>
		public override void StoreHome()
		{
			m_HomeHPosition = m_HPosition;
			m_HomeVPosition = m_VPosition;
			m_HomeZPosition = m_ZPosition;
		}

		#endregion

		#region IDeviceWithPower

		public void PowerOn()
		{
			m_Powered = true;
		}

		public void PowerOff()
		{
			m_Powered = false;
		}

		#endregion

		#region Private Methods

		private void LogCameraMovement(string action)
		{
			Log(eSeverity.Informational,
			    "MockCamera {0}: Instruction {1}: New Position - h{2},v{3},z{4}",
			    Name, action, m_HPosition, m_VPosition, m_ZPosition);
		}

		#endregion

		#region QueryCommands

		private void QueryPowerState()
		{
			Log(eSeverity.Informational, "Power State: {0}", m_Powered);
		}

		private void QueryCoordinates()
		{
			Log(eSeverity.Informational, "Coordinates: h{0},v{1},z{2}", m_HPosition, m_VPosition, m_ZPosition);
		}

		#endregion

		#region Settings

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_IsOnline;
		}

		protected override void ApplySettingsFinal(MockCameraDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			MockDeviceHelper.ApplySettings(this, settings);

			m_PanTiltSpeed = settings.PanTiltSpeed;
			m_ZoomSpeed = settings.ZoomSpeed;
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(MockCameraDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			MockDeviceHelper.CopySettings(this, settings);

			settings.PanTiltSpeed = m_PanTiltSpeed;
			settings.ZoomSpeed = m_ZoomSpeed;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			MockDeviceHelper.ClearSettings(this);

			m_PanTiltSpeed = null;
			m_ZoomSpeed = null;
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

			foreach (IConsoleCommand command in MockDeviceHelper.GetConsoleCommands(this))
				yield return command;

			yield return new ConsoleCommand("PowerOn", "Powers the camera device", () => PowerOn());
			yield return new ConsoleCommand("PowerOff", "Places the camera device on standby", () => PowerOff());
			yield return new ConsoleCommand("PowerQuery", "Returns the Powered State of the device", () => QueryPowerState());
			yield return new ConsoleCommand("PositionQuery", "Queries the current position of the camera", () => QueryCoordinates());
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
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			MockDeviceHelper.BuildConsoleStatus(this, addRow);

			addRow("Pan/Tilt Speed", m_PanTiltSpeed);
			addRow("Zoom Speed", m_ZoomSpeed);
		}

		#endregion
	}

	internal sealed class CameraPosition
	{
		public int HPosition { get; set; }
		public int VPosition { get; set; }
		public int ZPosition { get; set; }
	}
}
