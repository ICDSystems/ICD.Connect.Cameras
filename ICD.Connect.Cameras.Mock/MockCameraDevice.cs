using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Settings;

namespace ICD.Connect.Cameras.Mock
{
	public sealed class MockCameraDevice : AbstractCameraDevice<MockCameraDeviceSettings>,
	                                       ICameraWithPanTilt, ICameraWithZoom, ICameraWithPresets, IDeviceWithPower
	{
		#region Properties

		private bool m_Powered;
		private int m_VPosition;
		private int m_HPosition;
		private int m_ZPosition;
		private readonly Dictionary<int, CameraPosition> m_PresetPositions;
		private int? m_PanTiltSpeed;
		private int? m_ZoomSpeed;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public MockCameraDevice()
		{
			m_Presets = new Dictionary<int, CameraPreset>();
			m_PresetPositions = new Dictionary<int, CameraPosition>();

			Controls.Add(new GenericCameraRouteSourceControl<MockCameraDevice>(this, 0));
			Controls.Add(new PanTiltControl<MockCameraDevice>(this, 1));
			Controls.Add(new ZoomControl<MockCameraDevice>(this, 2));
			Controls.Add(new PowerDeviceControl<MockCameraDevice>(this, 3));
			Controls.Add(new PresetControl<MockCameraDevice>(this, 4));
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnPresetsChanged = null;

			base.DisposeFinal(disposing);
		}

		#region ICameraWithPanTilt

		public void PanTilt(eCameraPanTiltAction action)
		{
			int speed = m_PanTiltSpeed == null ? 1 : m_PanTiltSpeed.Value;
			switch (action)
			{
				case eCameraPanTiltAction.Left:
					m_HPosition = MathUtils.Clamp(m_HPosition - speed, int.MinValue, int.MaxValue);
					break;
				case eCameraPanTiltAction.Right:
					m_HPosition = MathUtils.Clamp(m_HPosition + speed, int.MinValue, int.MaxValue);
					break;
				case eCameraPanTiltAction.Up:
					m_VPosition = MathUtils.Clamp(m_VPosition + speed, int.MinValue, int.MaxValue);
					break;
				case eCameraPanTiltAction.Down:
					m_VPosition = MathUtils.Clamp(m_VPosition - speed, int.MinValue, int.MaxValue);
					break;
				case eCameraPanTiltAction.Stop:
					break;
				default:
					throw new ArgumentOutOfRangeException("action");
			}
			LogCameraMovement(action.ToString());
		}

		#endregion

		#region ICameraWithZoom

		public void Zoom(eCameraZoomAction action)
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

		#region ICameraWithPresets

		public event EventHandler OnPresetsChanged;

		public int MaxPresets { get { return 4; } }
		private readonly Dictionary<int, CameraPreset> m_Presets;

		public IEnumerable<CameraPreset> GetPresets()
		{
			return m_Presets.Values;
		}

		public void ActivatePreset(int presetId)
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

		public void StorePreset(int presetId)
		{
			if (presetId < 1 || presetId > MaxPresets)
			{
				Log(eSeverity.Warning, "Mock camera preset must be between 1 and {0}, preset was not stored.", MaxPresets);
				return;
			}

			m_Presets.Add(presetId, new CameraPreset(presetId, string.Format("Preset{0}", presetId)));
			m_PresetPositions.Add(presetId,
			                      new CameraPosition {HPosition = m_HPosition, VPosition = m_VPosition, ZPosition = m_ZPosition});

			OnPresetsChanged.Raise(this);
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

		#region DeviceBase

		protected override bool GetIsOnlineStatus()
		{
			return true;
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

		protected override void ApplySettingsFinal(MockCameraDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);
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
			settings.PanTiltSpeed = m_PanTiltSpeed;
			settings.ZoomSpeed = m_ZoomSpeed;
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

			yield return new ConsoleCommand("PowerOn", "Powers the camera device", () => PowerOn());
			yield return new ConsoleCommand("PowerOff", "Places the camera device on standby", () => PowerOff());
			yield return new ConsoleCommand("PowerQuery", "Returns the Powered State of the device", () => QueryPowerState());
			yield return
				new ConsoleCommand("PositionQuery", "Querries the current position of the camera", () => QueryCoordinates());
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

	internal sealed class CameraPosition
	{
		public int HPosition { get; set; }
		public int VPosition { get; set; }
		public int ZPosition { get; set; }
	}
}
