using ICD.Connect.Panels;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views
{
	public sealed partial class SoundsView : AbstractView, ISoundsView
	{
		private const long RINGTONE_INTERVAL = 7 * 1000;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public SoundsView(ISigInputOutput panel)
			: base(panel)
		{
		}

		/// <summary>
		/// Sets the visibility of the view.
		/// </summary>
		/// <param name="visible"></param>
		public override void Show(bool visible)
		{
			// Always visible
		}

		/// <summary>
		/// Starts/stops the ringtone sound.
		/// </summary>
		/// <param name="playing"></param>
		public void PlayRingtone(bool playing)
		{
			if (playing)
				m_Ringtone.Play(RINGTONE_INTERVAL);
			else
				m_Ringtone.Stop();
		}
	}
}
