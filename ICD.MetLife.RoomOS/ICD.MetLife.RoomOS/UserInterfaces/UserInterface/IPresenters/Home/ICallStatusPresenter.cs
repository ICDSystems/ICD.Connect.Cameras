using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home;
using ICD.Connect.Conferencing.ConferenceSources;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Home
{
	public interface ICallStatusPresenter : IPresenter<ICallStatusView>
	{
		/// <summary>
		/// Gets/sets the conference source for this presenter.
		/// </summary>
		/// <value></value>
		IConferenceSource ConferenceSource { get; set; }
	}
}
