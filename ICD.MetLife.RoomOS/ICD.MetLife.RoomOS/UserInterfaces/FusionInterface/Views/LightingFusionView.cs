using ICD.Connect.Analytics.FusionPro;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class LightingFusionView : AbstractFusionView, ILightingFusionView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public LightingFusionView(IFusionRoom fusionRoom)
			: base(fusionRoom)
		{
		}

		public void SetLightingProcessorOnline(bool online)
		{
			m_LightingProcessorOnlineInput.SendValue(online);
		}

		public void SetRoomHasShades(bool shades)
		{
			m_RoomHasShadesInput.SendValue(shades);
		}

		public void SetShadesProcessorOnline(bool online)
		{
			m_ShadesProcessorOnlineInput.SendValue(online);
		}
	}
}
