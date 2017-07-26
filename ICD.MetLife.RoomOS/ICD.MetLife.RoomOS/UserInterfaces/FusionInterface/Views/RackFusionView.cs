using ICD.Connect.Analytics.FusionPro;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class RackFusionView : AbstractFusionView, IRackFusionView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public RackFusionView(IFusionRoom fusionRoom)
			: base(fusionRoom)
		{
		}

		public void SetRacklinkIpAddress(string address)
		{
			m_RacklinkIpAddressInput.SendValue(address);
		}

		public void SetRackTemperature(string temperature)
		{
			m_RackTemperatureInput.SendValue(temperature);
		}

		public void SetRackPeakVolts(string volts)
		{
			m_RackPeakVoltsInput.SendValue(volts);
		}

		public void SetRackRmsVolts(string volts)
		{
			m_RackRmsVoltsInput.SendValue(volts);
		}

		public void SetRackPeakAmps(string amps)
		{
			m_RackPeakAmpsInput.SendValue(amps);
		}

		public void SetRackRmsAmps(string amps)
		{
			m_RackRmsAmpsInput.SendValue(amps);
		}
	}
}
