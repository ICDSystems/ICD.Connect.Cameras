using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Connect.UI.Controls;

namespace ICD.Connect.UI.ClientControls
{
	/// <summary>
	/// Client controls are objects that emulate VtPro controls from the panels perspective.
	/// For example, a ClientControl may respond to a SetLabel event and update the graphics on screen.
	/// </summary>
	public interface IClientControl : IIndexed, IDisposable
	{
		event EventHandler<BoolEventArgs> OnSetEnabled;
		event EventHandler<BoolEventArgs> OnSetVisible; 

		#region Properties

		/// <summary>
		/// The parent responsible for join offsets.
		/// </summary>
		[PublicAPI]
		IJoinOffsets Parent { get; }

		#endregion
	}
}
