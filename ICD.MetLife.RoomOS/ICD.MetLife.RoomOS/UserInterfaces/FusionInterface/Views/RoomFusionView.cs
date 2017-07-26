using System;
using ICD.Connect.Analytics.FusionPro;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class RoomFusionView : AbstractFusionView, IRoomFusionView
	{
		public event EventHandler OnUpdateContacts;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public RoomFusionView(IFusionRoom fusionRoom)
			: base(fusionRoom)
		{
		}

		public void SetRoomOccupied(bool occupied)
		{
			m_RoomOccupiedInput.SendValue(occupied);
		}

		public void SetVoiceConferencingDialPlan(string dialPlan)
		{
			m_VoiceConferencingDialPlanInput.SendValue(dialPlan);
		}

		#region Private Methods

		/// <summary>
		/// Subscribe to the control events.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_UpdateContactsOutput.OnOutput += UpdateContactsOutputOnOutput;
		}

		/// <summary>
		/// Unsubscribe from the control events.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_UpdateContactsOutput.OnOutput -= UpdateContactsOutputOnOutput;
		}

		private void UpdateContactsOutputOnOutput(object sender, BoolEventArgs args)
		{
			if (args.Data)
				OnUpdateContacts.Raise(this);
		}

		#endregion
	}
}
