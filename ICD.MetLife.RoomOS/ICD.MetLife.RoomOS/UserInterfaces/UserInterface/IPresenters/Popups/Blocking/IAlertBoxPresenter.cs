namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking
{
	public interface IAlertBoxPresenter : IPresenter
	{
		/// <summary>
		/// Enqueues the alert to be displayed.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <param name="options"></param>
		void Enqueue(string title, string message, params AlertOption[] options);

		/// <summary>
		/// Enqueues the alert to be displayed.
		/// </summary>
		/// <param name="alert"></param>
		void Enqueue(Alert alert);
	}
}
