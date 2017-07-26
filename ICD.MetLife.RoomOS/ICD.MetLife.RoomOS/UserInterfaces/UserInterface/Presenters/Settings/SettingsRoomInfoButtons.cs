using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Connect.Settings.Core;
using ICD.Connect.UI.Utils;
using ICD.MetLife.RoomOS.Rooms;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	/// <summary>
	/// Base class for room info buttons.
	/// </summary>
	public abstract class AbstractSettingsRoomInfoButton
	{
		private readonly int m_RoomId;
		private readonly ICoreSettings m_CoreSettings;

		/// <summary>
		/// Gets the room settings.
		/// </summary>
		protected MetlifeRoomSettings RoomSettings
		{
			get { return m_CoreSettings.OriginatorSettings.GetById(m_RoomId) as MetlifeRoomSettings; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="settings"></param>
		protected AbstractSettingsRoomInfoButton(int roomId, ICoreSettings settings)
		{
			m_RoomId = roomId;
			m_CoreSettings = settings;
		}

		/// <summary>
		/// Gets the room property value.
		/// </summary>
		/// <returns></returns>
		public abstract string GetValue();

		/// <summary>
		/// Sets the room property value.
		/// </summary>
		/// <param name="value"></param>
		public abstract void SetValue(string value);

		/// <summary>
		/// Gets the room property title.
		/// </summary>
		/// <returns></returns>
		protected abstract string GetTitle();

		/// <summary>
		/// Gets the label for the button.
		/// </summary>
		/// <returns></returns>
		public string GetLabel()
		{
			string value = GetValue();
			string title = GetTitle();
			return string.Format("{0}{1}({2})", title, HtmlUtils.NEWLINE, value);
		}
	}

	/// <summary>
	/// Button for room prefix.
	/// </summary>
	public sealed class SettingsRoomPrefixButton : AbstractSettingsRoomInfoButton
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="settings"></param>
		public SettingsRoomPrefixButton(int roomId, ICoreSettings settings)
			: base(roomId, settings)
		{
		}

		/// <summary>
		/// Gets the room property title.
		/// </summary>
		/// <returns></returns>
		protected override string GetTitle()
		{
			return "Room Prefix";
		}

		/// <summary>
		/// Gets the room property value.
		/// </summary>
		/// <returns></returns>
		public override string GetValue()
		{
			return RoomSettings == null ? null : RoomSettings.Prefix;
		}

		/// <summary>
		/// Sets the room property value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override void SetValue(string value)
		{
			if (RoomSettings == null)
				return;

			RoomSettings.Prefix = value;
		}
	}

	/// <summary>
	/// Button for room number.
	/// </summary>
	public sealed class SettingsRoomNumberButton : AbstractSettingsRoomInfoButton
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="settings"></param>
		public SettingsRoomNumberButton(int roomId, ICoreSettings settings)
			: base(roomId, settings)
		{
		}

		/// <summary>
		/// Gets the room property title.
		/// </summary>
		/// <returns></returns>
		protected override string GetTitle()
		{
			return "Room Number";
		}

		/// <summary>
		/// Gets the room property value.
		/// </summary>
		/// <returns></returns>
		public override string GetValue()
		{
			return RoomSettings == null ? null : RoomSettings.Number;
		}

		/// <summary>
		/// Sets the room property value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override void SetValue(string value)
		{
			if (RoomSettings == null)
				return;

			RoomSettings.Number = value;
		}
	}

	/// <summary>
	/// Button for room phone number.
	/// </summary>
	public sealed class SettingsRoomPhoneNumberButton : AbstractSettingsRoomInfoButton
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="settings"></param>
		public SettingsRoomPhoneNumberButton(int roomId, ICoreSettings settings)
			: base(roomId, settings)
		{
		}

		/// <summary>
		/// Gets the room property title.
		/// </summary>
		/// <returns></returns>
		protected override string GetTitle()
		{
			return "Room Phone Number";
		}

		/// <summary>
		/// Gets the room property value.
		/// </summary>
		/// <returns></returns>
		public override string GetValue()
		{
			return RoomSettings == null ? null : RoomSettings.PhoneNumber;
		}

		/// <summary>
		/// Sets the room property value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override void SetValue(string value)
		{
			if (RoomSettings == null)
				return;

			RoomSettings.PhoneNumber = value;
		}
	}

	/// <summary>
	/// Button for room config name.
	/// </summary>
	public sealed class SettingsRoomConfigNameButton : AbstractSettingsRoomInfoButton
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="settings"></param>
		public SettingsRoomConfigNameButton(int roomId, ICoreSettings settings)
			: base(roomId, settings)
		{
		}

		/// <summary>
		/// Gets the room property title.
		/// </summary>
		/// <returns></returns>
		protected override string GetTitle()
		{
			return "Room Config";
		}

		/// <summary>
		/// Gets the room property value.
		/// </summary>
		/// <returns></returns>
		public override string GetValue()
		{
			return RoomSettings == null ? null : RoomSettings.Name;
		}

		/// <summary>
		/// Sets the room property value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override void SetValue(string value)
		{
			if (RoomSettings == null)
				return;

			RoomSettings.Name = value;
		}
	}

	/// <summary>
	/// Button for room owner name.
	/// </summary>
	public sealed class SettingsRoomOwnerNameButton : AbstractSettingsRoomInfoButton
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="settings"></param>
		public SettingsRoomOwnerNameButton(int roomId, ICoreSettings settings)
			: base(roomId, settings)
		{
		}

		/// <summary>
		/// Gets the room property title.
		/// </summary>
		/// <returns></returns>
		protected override string GetTitle()
		{
			return "Room Owner Name";
		}

		/// <summary>
		/// Gets the room property value.
		/// </summary>
		/// <returns></returns>
		public override string GetValue()
		{
			return RoomSettings == null ? null : RoomSettings.OwnerName;
		}

		/// <summary>
		/// Sets the room property value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override void SetValue(string value)
		{
			if (RoomSettings == null)
				return;

			RoomSettings.OwnerName = value;
		}
	}

	/// <summary>
	/// Button for room owner phone number.
	/// </summary>
	public sealed class SettingsRoomOwnerPhoneButton : AbstractSettingsRoomInfoButton
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="settings"></param>
		public SettingsRoomOwnerPhoneButton(int roomId, ICoreSettings settings)
			: base(roomId, settings)
		{
		}

		/// <summary>
		/// Gets the room property title.
		/// </summary>
		/// <returns></returns>
		protected override string GetTitle()
		{
			return "Room Owner Phone";
		}

		/// <summary>
		/// Gets the room property value.
		/// </summary>
		/// <returns></returns>
		public override string GetValue()
		{
			return RoomSettings == null ? null : RoomSettings.OwnerPhone;
		}

		/// <summary>
		/// Sets the room property value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override void SetValue(string value)
		{
			if (RoomSettings == null)
				return;

			RoomSettings.OwnerPhone = value;
		}
	}

	/// <summary>
	/// Button for room owner email.
	/// </summary>
	public sealed class SettingsRoomOwnerEmailButton : AbstractSettingsRoomInfoButton
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="settings"></param>
		public SettingsRoomOwnerEmailButton(int roomId, ICoreSettings settings)
			: base(roomId, settings)
		{
		}

		/// <summary>
		/// Gets the room property title.
		/// </summary>
		/// <returns></returns>
		protected override string GetTitle()
		{
			return "Room Owner Email";
		}

		/// <summary>
		/// Gets the room property value.
		/// </summary>
		/// <returns></returns>
		public override string GetValue()
		{
			return RoomSettings == null ? null : RoomSettings.OwnerEmail;
		}

		/// <summary>
		/// Sets the room property value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override void SetValue(string value)
		{
			if (RoomSettings == null)
				return;

			RoomSettings.OwnerEmail = value;
		}
	}

	/// <summary>
	/// Button for room id.
	/// </summary>
	public sealed class SettingsRoomIdButton : AbstractSettingsRoomInfoButton
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="settings"></param>
		public SettingsRoomIdButton(int roomId, ICoreSettings settings)
			: base(roomId, settings)
		{
		}

		/// <summary>
		/// Gets the room property title.
		/// </summary>
		/// <returns></returns>
		protected override string GetTitle()
		{
			return "Room ID";
		}

		/// <summary>
		/// Gets the room property value.
		/// </summary>
		/// <returns></returns>
		public override string GetValue()
		{
			return RoomSettings == null ? null : RoomSettings.Id.ToString();
		}

		/// <summary>
		/// Sets the room property value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override void SetValue(string value)
		{
			if (RoomSettings == null)
				return;

			int id;
			if (StringUtils.TryParse(value, out id))
				RoomSettings.Id = id;
		}
	}
}
