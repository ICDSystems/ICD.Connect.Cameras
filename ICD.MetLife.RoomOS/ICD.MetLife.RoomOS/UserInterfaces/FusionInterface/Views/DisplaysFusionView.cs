using ICD.Connect.Analytics.FusionPro;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class DisplaysFusionView : AbstractFusionView, IDisplaysFusionView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public DisplaysFusionView(IFusionRoom fusionRoom)
			: base(fusionRoom)
		{
		}

		public void SetLeftDisplayOnline(bool online)
		{
			m_LeftDisplayOnlineInput.SendValue(online);
		}

		/// <summary>
		/// Sets the online status of the first source/destination device pathed to the display that isn't a switcher.
		/// </summary>
		/// <param name="online"></param>
		public void SetLeftDisplayReceiverOnline(bool online)
		{
			m_LeftDisplayReceiverOnlineInput.SendValue(online);
		}

		public void SetLeftDisplayReceiverType(string type)
		{
			m_LeftDisplayReceiverTypeInput.SendValue(type);
		}

		public void SetLeftDisplayReceiverFirmwareVersion(string version)
		{
			m_LeftDisplayReceiverFirmwareVersionInput.SendValue(version);
		}

		public void SetRightDisplayReceiverOnline(bool online)
		{
			m_RightDisplayReceiverOnlineInput.SendValue(online);
		}

		public void SetRightDisplayReceiverType(string type)
		{
			m_RightDisplayReceiverTypeInput.SendValue(type);
		}

		public void SetRightDisplayReceiverFirmwareVersion(string version)
		{
			m_RightDisplayReceiverFirmwareVersionInput.SendValue(version);
		}
	}
}
