using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Share
{
	public interface IDisplaySelectView : IView
	{
		event EventHandler OnListenToSourceButtonPressed;
		event EventHandler OnShowVideoCallButtonPressed;
		event EventHandler<UShortEventArgs> OnDestinationButtonPressed;
		event EventHandler<BoolEventArgs> OnDestinationsMovingChanged; 

		void SetSourceName(string name);

		void SetListenToSourceButtonEnabled(bool enable);

		void SetShowVideoCallButtonEnabled(bool enable);

		void SetShowVideoCallButtonVisible(bool visible);

		void SetDestinationLabels(IEnumerable<string> labels);

		void SetDestinationSelected(ushort index, bool selected);
		void SetDestinationVisible(ushort index, bool visible);
	}
}
