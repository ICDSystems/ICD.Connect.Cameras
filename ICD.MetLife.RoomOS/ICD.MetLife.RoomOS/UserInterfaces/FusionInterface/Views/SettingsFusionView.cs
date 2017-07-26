using System;
using ICD.Connect.Analytics.FusionPro;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class SettingsFusionView : AbstractFusionView, ISettingsFusionView
	{
		public event EventHandler<StringEventArgs> OnRoomNumberChanged;
		public event EventHandler<StringEventArgs> OnRoomNameChanged;
		public event EventHandler<StringEventArgs> OnRoomTypeChanged;
		public event EventHandler<StringEventArgs> OnRoomOwnerChanged;
		public event EventHandler<StringEventArgs> OnRoomPhoneNumberChanged;
		public event EventHandler<StringEventArgs> OnBuildingChanged;
		public event EventHandler OnApplyRoomSettings;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public SettingsFusionView(IFusionRoom fusionRoom)
			: base(fusionRoom)
		{
		}

		#region Methods

		public void SetRoomNumber(string number)
		{
			m_RoomNumberInputOutput.SendValue(number);
		}

		public void SetRoomName(string name)
		{
			m_RoomNameInputOutput.SendValue(name);
		}

		public void SetRoomType(string type)
		{
			m_RoomTypeInputOutput.SendValue(type);
		}

		public void SetRoomOwner(string owner)
		{
			m_RoomOwnerInputOutput.SendValue(owner);
		}

		public void SetRoomPhoneNumber(string number)
		{
			m_RoomPhoneNumberInputOutput.SendValue(number);
		}

		public void SetBuilding(string building)
		{
			m_RoomBuildingInputOutput.SendValue(building);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribe to the control events.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_RoomNumberInputOutput.OnOutput += RoomNumberInputOutputOnOutput;
			m_RoomNameInputOutput.OnOutput += RoomNameInputOutputOnOutput;
			m_RoomTypeInputOutput.OnOutput += RoomTypeInputOutputOnOutput;
			m_RoomOwnerInputOutput.OnOutput += RoomOwnerInputOutputOnOutput;
			m_RoomPhoneNumberInputOutput.OnOutput += RoomPhoneNumberInputOutputOnOutput;
			m_RoomBuildingInputOutput.OnOutput += RoomBuildingInputOutputOnOutput;
			m_ApplySettingsOutput.OnOutput += ApplySettingsOutputOnOutput;
		}

		/// <summary>
		/// Unsubscribe from the control events.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_RoomNumberInputOutput.OnOutput -= RoomNumberInputOutputOnOutput;
			m_RoomNameInputOutput.OnOutput -= RoomNameInputOutputOnOutput;
			m_RoomTypeInputOutput.OnOutput -= RoomTypeInputOutputOnOutput;
			m_RoomOwnerInputOutput.OnOutput -= RoomOwnerInputOutputOnOutput;
			m_RoomPhoneNumberInputOutput.OnOutput -= RoomPhoneNumberInputOutputOnOutput;
			m_RoomBuildingInputOutput.OnOutput -= RoomBuildingInputOutputOnOutput;
			m_ApplySettingsOutput.OnOutput -= ApplySettingsOutputOnOutput;
		}

		private void RoomPhoneNumberInputOutputOnOutput(object sender, StringEventArgs args)
		{
			OnRoomPhoneNumberChanged.Raise(this, new StringEventArgs(args.Data));
		}

		private void ApplySettingsOutputOnOutput(object sender, BoolEventArgs args)
		{
			if (args.Data)
				OnApplyRoomSettings.Raise(this);
		}

		private void RoomBuildingInputOutputOnOutput(object sender, StringEventArgs args)
		{
			OnBuildingChanged.Raise(this, new StringEventArgs(args.Data));
		}

		private void RoomOwnerInputOutputOnOutput(object sender, StringEventArgs args)
		{
			OnRoomOwnerChanged.Raise(this, new StringEventArgs(args.Data));
		}

		private void RoomTypeInputOutputOnOutput(object sender, StringEventArgs args)
		{
			OnRoomTypeChanged.Raise(this, new StringEventArgs(args.Data));
		}

		private void RoomNameInputOutputOnOutput(object sender, StringEventArgs args)
		{
			OnRoomNameChanged.Raise(this, new StringEventArgs(args.Data));
		}

		private void RoomNumberInputOutputOnOutput(object sender, StringEventArgs args)
		{
			OnRoomNumberChanged.Raise(this, new StringEventArgs(args.Data));
		}

		#endregion
	}
}
