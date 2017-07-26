using System.Collections.Generic;
using ICD.Connect.Panels.SigIo;
using ICD.Connect.Analytics.FusionPro;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class SettingsFusionView
	{
		private SerialSigInputOutput m_RoomNumberInputOutput;
		private SerialSigInputOutput m_RoomNameInputOutput;
		private SerialSigInputOutput m_RoomTypeInputOutput;
		private SerialSigInputOutput m_RoomOwnerInputOutput;
		private SerialSigInputOutput m_RoomPhoneNumberInputOutput;
		private SerialSigInputOutput m_RoomBuildingInputOutput;
		private DigitalSigInputOutput m_ApplySettingsOutput;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected override void InstantiateControls(IFusionRoom fusionRoom)
		{
			m_RoomNumberInputOutput = new SerialSigInputOutput(fusionRoom, 51);
			m_RoomNameInputOutput = new SerialSigInputOutput(fusionRoom, 52);
			m_RoomTypeInputOutput = new SerialSigInputOutput(fusionRoom, 53);
			m_RoomOwnerInputOutput = new SerialSigInputOutput(fusionRoom, 54);
			m_RoomPhoneNumberInputOutput = new SerialSigInputOutput(fusionRoom, 80);
			m_RoomBuildingInputOutput = new SerialSigInputOutput(fusionRoom, 50);
			m_ApplySettingsOutput = new DigitalSigInputOutput(fusionRoom, 71);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<AbstractSigOutput> GetChildren()
		{
			yield return m_RoomNumberInputOutput;
			yield return m_RoomNameInputOutput;
			yield return m_RoomTypeInputOutput;
			yield return m_RoomOwnerInputOutput;
			yield return m_RoomPhoneNumberInputOutput;
			yield return m_RoomBuildingInputOutput;
			yield return m_ApplySettingsOutput;
		}
	}
}
