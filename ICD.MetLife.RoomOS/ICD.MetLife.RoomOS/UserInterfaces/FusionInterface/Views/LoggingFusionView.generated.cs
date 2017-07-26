using System.Collections.Generic;
using ICD.Connect.Panels.SigIo;
using ICD.Connect.Analytics.FusionPro;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class LoggingFusionView
	{
		private AnalogSigInputOutput m_LoggingSeverityInputOutput;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected override void InstantiateControls(IFusionRoom fusionRoom)
		{
			m_LoggingSeverityInputOutput = new AnalogSigInputOutput(fusionRoom)
			{
				Number = 50
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<AbstractSigOutput> GetChildren()
		{
			yield return m_LoggingSeverityInputOutput;
		}
	}
}
