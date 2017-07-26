using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views
{
	/// <summary>
	/// Simple binding class to ensure component views always have a parent and an index.
	/// </summary>
	public abstract class AbstractComponentView : AbstractView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractComponentView(ISigInputOutput panel, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
		}
	}
}
