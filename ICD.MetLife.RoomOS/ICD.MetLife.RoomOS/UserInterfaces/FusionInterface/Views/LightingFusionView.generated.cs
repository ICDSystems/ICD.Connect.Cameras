using System.Collections.Generic;
using ICD.Connect.Panels.SigIo;
using ICD.Connect.Analytics.FusionPro;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class LightingFusionView
	{
		private DigitalSigInputOutput m_LightingProcessorOnlineInput;
		private DigitalSigInputOutput m_RoomHasShadesInput;
		private DigitalSigInputOutput m_ShadesProcessorOnlineInput;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected override void InstantiateControls(IFusionRoom fusionRoom)
		{
			m_LightingProcessorOnlineInput = new DigitalSigInputOutput(fusionRoom, 61);
			m_RoomHasShadesInput = new DigitalSigInputOutput(fusionRoom, 62);
			m_ShadesProcessorOnlineInput = new DigitalSigInputOutput(fusionRoom, 63);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<AbstractSigOutput> GetChildren()
		{
			yield return m_LightingProcessorOnlineInput;
			yield return m_RoomHasShadesInput;
			yield return m_ShadesProcessorOnlineInput;
		}
	}
}
