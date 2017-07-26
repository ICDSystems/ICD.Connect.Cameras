using System;
using ICD.Common.Properties;

namespace ICD.Connect.UI.Controls
{
	public interface IVtProControl : IIndexed, IDisposable
	{
		#region Properties

		/// <summary>
		/// The parent responsible for join offsets.
		/// </summary>
		[PublicAPI]
		IJoinOffsets Parent { get; }

		/// <summary>
		/// Gets the visibility state of the control.
		/// </summary>
		bool IsVisible { get; }

		/// <summary>
		/// Returns true if this control, and all parent controls are visible.
		/// </summary>
		bool IsVisibleRecursive { get; }

		/// <summary>
		/// Gets the enabled state of the control.
		/// </summary>
		bool IsEnabled { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Enables/disables the button. Throws InvalidOperationException if there is no enable join.
		/// </summary>
		void Enable(bool state);

		/// <summary>
		/// Shows/hides the button. Throws InvalidOperationException if there is no visibility join.
		/// </summary>
		/// <param name="state"></param>
		void Show(bool state);

		#endregion
	}
}
