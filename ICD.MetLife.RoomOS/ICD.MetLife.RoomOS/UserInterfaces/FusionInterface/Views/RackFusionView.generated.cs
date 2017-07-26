using System.Collections.Generic;
using ICD.Connect.Panels.SigIo;
using ICD.Connect.Analytics.FusionPro;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class RackFusionView
	{
		private SerialSigInputOutput m_RacklinkIpAddressInput;
		private SerialSigInputOutput m_RackTemperatureInput;
		private SerialSigInputOutput m_RackPeakVoltsInput;
		private SerialSigInputOutput m_RackRmsVoltsInput;
		private SerialSigInputOutput m_RackPeakAmpsInput;
		private SerialSigInputOutput m_RackRmsAmpsInput;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected override void InstantiateControls(IFusionRoom fusionRoom)
		{
			m_RacklinkIpAddressInput = new SerialSigInputOutput(fusionRoom, 94);
			m_RackTemperatureInput = new SerialSigInputOutput(fusionRoom, 95);
			m_RackPeakVoltsInput = new SerialSigInputOutput(fusionRoom, 96);
			m_RackRmsVoltsInput = new SerialSigInputOutput(fusionRoom, 97);
			m_RackPeakAmpsInput = new SerialSigInputOutput(fusionRoom, 98);
			m_RackRmsAmpsInput = new SerialSigInputOutput(fusionRoom, 99);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<AbstractSigOutput> GetChildren()
		{
			yield return m_RacklinkIpAddressInput;
			yield return m_RackTemperatureInput;
			yield return m_RackPeakVoltsInput;
			yield return m_RackRmsVoltsInput;
			yield return m_RackPeakAmpsInput;
			yield return m_RackRmsAmpsInput;
		}
	}
}
