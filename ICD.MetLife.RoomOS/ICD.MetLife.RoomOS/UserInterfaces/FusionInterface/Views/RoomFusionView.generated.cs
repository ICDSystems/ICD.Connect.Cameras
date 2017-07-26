using System.Collections.Generic;
using ICD.Connect.Analytics.FusionPro;
using ICD.Connect.Panels.SigIo;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class RoomFusionView
	{
		private DigitalSigInputOutput m_UpdateContactsOutput;
		private DigitalSigInputOutput m_RoomOccupiedInput;
		private SerialSigInputOutput m_VoiceConferencingDialPlanInput;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected override void InstantiateControls(IFusionRoom fusionRoom)
		{
			m_UpdateContactsOutput = new DigitalSigInputOutput(fusionRoom, 70);
			m_RoomOccupiedInput = new DigitalSigInputOutput(fusionRoom, 60);
			m_VoiceConferencingDialPlanInput = new SerialSigInputOutput(fusionRoom, 106);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<AbstractSigOutput> GetChildren()
		{
			yield return m_UpdateContactsOutput;
			yield return m_RoomOccupiedInput;
			yield return m_VoiceConferencingDialPlanInput;
		}
	}
}
