using System;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Contacts;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public abstract class AbstractDialPresenter<T> : AbstractMainPresenter<T>
		where T : class, IView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractDialPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		#region Methods

		/// <summary>
		/// Attempts to dial the given number.
		/// </summary>
		/// <param name="number"></param>
		protected void Dial(string number)
		{
			if (Room == null)
				throw new InvalidOperationException("No Room");

			if (ValidateCanDial())
				Room.ConferenceManager.Dial(number);
		}

		/// <summary>
		/// Attempts to dial the given contact.
		/// </summary>
		/// <param name="contact"></param>
		protected void Dial(IContact contact)
		{
			if (Room == null)
				throw new InvalidOperationException("No Room");

			if (ValidateCanDial())
				Room.ConferenceManager.Dial(contact);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Shows an alert and returns false if we are currently unable to dial.
		/// </summary>
		/// <returns></returns>
		private bool ValidateCanDial()
		{
			string reason;
			if (CanDial(out reason))
				return true;

			ShowAlert(reason);

			return false;
		}

		/// <summary>
		/// Returns false if we are currently unable to dial.
		/// </summary>
		/// <param name="reason"></param>
		/// <returns></returns>
		private bool CanDial(out string reason)
		{
			reason = string.Empty;

			if (Room == null)
				return false;

			// Todo - how to handle multiple displays in one room?
			if (Room.GetDisplays().Any(d => d.IsOnline))
				return true;

			reason = "Display is offline";

			return false;
		}

		private void ShowAlert(string reason)
		{
			AlertOption[] options =
			{
				new AlertOption("OK")
			};
			Alert alert = new Alert("Unable to Place Call", reason, options);
			Navigation.NavigateTo<IAlertBoxPresenter>().Enqueue(alert);
		}

		#endregion
	}
}
