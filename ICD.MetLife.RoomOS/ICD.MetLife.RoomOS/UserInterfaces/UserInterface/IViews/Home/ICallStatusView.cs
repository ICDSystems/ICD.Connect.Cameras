using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home
{
	/// <summary>
	/// Presents information for a single conference source.
	/// </summary>
	public interface ICallStatusView : IView
	{
		/// <summary>
		/// Raised when the user presses the end call button.
		/// </summary>
		event EventHandler OnEndCallButtonPressed;

		/// <summary>
		/// Sets the call type/status text.
		/// </summary>
		/// <param name="status"></param>
		/// <param name="color"></param>
		void SetCallStatusText(string status, eColor color);

		/// <summary>
		/// Sets the call number/name text.
		/// </summary>
		/// <param name="name"></param>
		void SetCallName(string name);

		/// <summary>
		/// Sets the call duration in seconds.
		/// </summary>
		/// <param name="seconds"></param>
		void SetCallDuration(ushort seconds);
	}

	/// <summary>
	/// Extension methods for ICallStatusViews.
	/// </summary>
	public static class CallStatusViewExtensions
	{
		/// <summary>
		/// Sets the type/status text with default color.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="status"></param>
		public static void SetCallStatusText(this ICallStatusView extends, string status)
		{
			extends.SetCallStatusText(status, eColor.Default);
		}
	}
}
