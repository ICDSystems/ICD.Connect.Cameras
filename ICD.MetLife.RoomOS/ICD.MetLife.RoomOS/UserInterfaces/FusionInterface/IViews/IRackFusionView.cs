namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface IRackFusionView : IFusionView
	{
		void SetRacklinkIpAddress(string address);

		void SetRackTemperature(string temperature);

		void SetRackPeakVolts(string volts);

		void SetRackRmsVolts(string volts);

		void SetRackPeakAmps(string amps);

		void SetRackRmsAmps(string amps);
	}
}
