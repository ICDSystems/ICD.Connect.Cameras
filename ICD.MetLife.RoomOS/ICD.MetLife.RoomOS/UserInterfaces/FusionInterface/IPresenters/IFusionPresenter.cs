using System;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters
{
	public interface IFusionPresenter : IDisposable
	{
		/// <summary>
		/// Gets the view.
		/// </summary>
		/// <returns></returns>
		IFusionView GetView();

		/// <summary>
		/// Refreshes the state of the view.
		/// </summary>
		void Refresh();

		/// <summary>
		/// Asynchronously refreshes the state of the view.
		/// </summary>
		void RefreshAsync();
	}

	public interface IFusionPresenter<TView> : IFusionPresenter
		where TView : IFusionView
	{
		/// <summary>
		/// Gets the view.
		/// </summary>
		/// <returns></returns>
		new TView GetView();

		/// <summary>
		/// Sets the view.
		/// </summary>
		/// <param name="view"></param>
		void SetView(TView view);
	}
}
