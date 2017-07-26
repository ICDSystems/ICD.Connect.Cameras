using ICD.Connect.Analytics.FusionPro;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class PanelFusionView : AbstractFusionView, IPanelFusionView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public PanelFusionView(IFusionRoom fusionRoom)
			: base(fusionRoom)
		{
		}

		public void SetPanelOnline(bool online)
		{
			m_PanelOnlineInput.SendValue(online);
		}

		public void SetPanelType(string type)
		{
			m_PanelTypeInput.SendValue(type);
		}

		public void SetPanelFirmwareVersion(string version)
		{
			m_PanelFirmwareVersionInput.SendValue(version);
		}

		public void SetPanelHeaderImagePath(string path)
		{
			m_PanelHeaderImagePathInput.SendValue(path);
		}

		public void SetPanelBackgroundImagePath(string path)
		{
			m_PanelBackgroundImagePathInput.SendValue(path);
		}
	}
}
