using System.Collections.Generic;
using ICD.Connect.Analytics.FusionPro;
using ICD.Connect.Panels.SigIo;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	/// <summary>
	/// Base class for all Fusion views.
	/// </summary>
	public abstract class AbstractFusionView : IFusionView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected AbstractFusionView(IFusionRoom fusionRoom)
		{
			InstantiateControls(fusionRoom);
			SubscribeControls();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			UnsubscribeControls();

			foreach (AbstractSigOutput control in GetChildren())
				control.Dispose();
		}

		/// <summary>
		/// Subscribe to the control events.
		/// </summary>
		protected virtual void SubscribeControls()
		{
		}

		/// <summary>
		/// Unsubscribe from the control events.
		/// </summary>
		protected virtual void UnsubscribeControls()
		{
		}

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected abstract void InstantiateControls(IFusionRoom fusionRoom);

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerable<AbstractSigOutput> GetChildren();
	}
}
