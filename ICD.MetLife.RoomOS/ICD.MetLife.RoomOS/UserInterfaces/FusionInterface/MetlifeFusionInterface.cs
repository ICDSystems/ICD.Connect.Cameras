using System;
using ICD.Connect.Settings.Core;
using ICD.Connect.Analytics.FusionPro;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface
{
	/// <summary>
	/// Fusion interface communicates between the room and the fusion device to
	/// report device statuses.
	/// </summary>
	public sealed class MetlifeFusionInterface : IDisposable
	{
		private readonly IFusionPresenterFactory m_PresenterFactory;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="fusionRoom"></param>
		/// <param name="core"></param>
		public MetlifeFusionInterface(MetlifeRoom room, IFusionRoom fusionRoom, ICore core)
		{
			IFusionViewFactory viewFactory = new MetlifeFusionViewFactory(fusionRoom);
			m_PresenterFactory = new MetlifeFusionPresenterFactory(room, viewFactory, core);

			// Generate all of the presenters and update the sigs
			foreach (IFusionPresenter presenter in m_PresenterFactory.GetPresenters())
				presenter.RefreshAsync();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_PresenterFactory.Dispose();
		}
	}
}
