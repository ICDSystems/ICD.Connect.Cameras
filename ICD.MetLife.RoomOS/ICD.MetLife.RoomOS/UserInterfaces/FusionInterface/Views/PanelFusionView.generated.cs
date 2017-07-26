using System.Collections.Generic;
using ICD.Connect.Panels.SigIo;
using ICD.Connect.Analytics.FusionPro;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class PanelFusionView
	{
		private DigitalSigInputOutput m_PanelOnlineInput;
		private SerialSigInputOutput m_PanelTypeInput;
		private SerialSigInputOutput m_PanelFirmwareVersionInput;
		private SerialSigInputOutput m_PanelHeaderImagePathInput;
		private SerialSigInputOutput m_PanelBackgroundImagePathInput;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected override void InstantiateControls(IFusionRoom fusionRoom)
		{
			m_PanelOnlineInput = new DigitalSigInputOutput(fusionRoom, 73);
			m_PanelTypeInput = new SerialSigInputOutput(fusionRoom, 71);
			m_PanelFirmwareVersionInput = new SerialSigInputOutput(fusionRoom, 72);
			m_PanelHeaderImagePathInput = new SerialSigInputOutput(fusionRoom, 107);
			m_PanelBackgroundImagePathInput = new SerialSigInputOutput(fusionRoom, 108);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<AbstractSigOutput> GetChildren()
		{
			yield return m_PanelOnlineInput;
			yield return m_PanelTypeInput;
			yield return m_PanelFirmwareVersionInput;
			yield return m_PanelHeaderImagePathInput;
			yield return m_PanelBackgroundImagePathInput;
		}
	}
}
