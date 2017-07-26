using System.Linq;
using ICD.Connect.Panels;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Home
{
	public sealed partial class RoomNumbersView : AbstractView, IRoomNumbersView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public RoomNumbersView(ISigInputOutput panel)
			: base(panel)
		{
		}

		/// <summary>
		/// Sets the audio number text.
		/// </summary>
		/// <param name="number"></param>
		public void SetAudioNumber(string number)
		{
			m_AudioText.SetLabelTextAtJoin(m_AudioText.SerialLabelJoins.First(), number);
		}

		/// <summary>
		/// Sets the visibility of the audio label.
		/// </summary>
		/// <param name="show"></param>
		public void ShowAudioLabel(bool show)
		{
			m_AudioText.Show(show);
		}

		/// <summary>
		/// Sets the video number text.
		/// </summary>
		/// <param name="number"></param>
		public void SetVideoNumber(string number)
		{
			m_VideoText.SetLabelTextAtJoin(m_VideoText.SerialLabelJoins.First(), number);
		}

		/// <summary>
		/// Sets the visibility of the video label.
		/// </summary>
		/// <param name="show"></param>
		public void ShowVideoLabel(bool show)
		{
			m_VideoText.Show(show);
		}

		/// <summary>
		/// Sets the sharing status text (e.g. "Watching TV").
		/// </summary>
		/// <param name="sharingStatus"></param>
		public void SetSharingStatusText(string sharingStatus)
		{
			m_ShareStatusText.SetLabelTextAtJoin(m_ShareStatusText.SerialLabelJoins.First(), sharingStatus);
		}
	}
}
