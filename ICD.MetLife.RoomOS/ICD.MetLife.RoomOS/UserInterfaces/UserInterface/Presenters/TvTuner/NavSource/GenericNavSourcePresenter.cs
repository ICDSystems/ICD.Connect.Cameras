using ICD.Connect.Rooms.Extensions;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner.NavSource;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner.NavSource;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.TvTuner.NavSource
{
	public sealed class GenericNavSourcePresenter : AbstractNavSourcePresenter<IGenericNavSourceView>,
	                                                IGenericNavSourcePresenter
	{
		public GenericNavSourcePresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IGenericNavSourceView view)
		{
			base.Refresh(view);

			string name = Source == null ? "Source" : Source.GetNameOrDeviceName(Room);
			string presentParticiple = Source == null ? "Watching" : Source.SourceType.GetSourcePresentParticiple();

			view.SetSourceName(name);
			view.SetSourcePresentParticiple(presentParticiple);
		}
	}
}
