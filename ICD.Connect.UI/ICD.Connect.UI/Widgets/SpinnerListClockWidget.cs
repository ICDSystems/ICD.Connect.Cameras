using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;

namespace ICD.Connect.UI.Widgets
{
	/// <summary>
	/// Combines an hour, minute and AM/PM spinner for time selection.
	/// TODO - Assumes the spinners have 12 hours and minutes 00, 15, 30 and 45. How can we genericise this?
	/// </summary>
	public sealed class SpinnerListClockWidget : AbstractWidget<IPanelDevice>
	{
		public delegate void TimeChangedCallback(SpinnerListClockWidget sender, DateTime time);

		/// <summary>
		/// Raised when the selected time changes.
		/// </summary>
		public event TimeChangedCallback OnSelectedTimeChanged;

		private readonly VtProSpinnerList m_HourList;
		private readonly VtProSpinnerList m_MinuteList;
		private readonly VtProSpinnerList m_AmPmList;

		private static readonly Dictionary<ushort, int> s_IndexHours = new Dictionary<ushort, int>
		{
			{0, 1},
			{1, 2},
			{2, 3},
			{3, 4},
			{4, 5},
			{5, 6},
			{6, 7},
			{7, 8},
			{8, 9},
			{9, 10},
			{10, 11},
			{11, 12}
		};

		private static readonly Dictionary<ushort, int> s_IndexMinutes = new Dictionary<ushort, int>
		{
			{0, 0},
			{1, 15},
			{2, 30},
			{3, 45}
		};

		private static readonly Dictionary<ushort, bool> s_IndexAm = new Dictionary<ushort, bool>
		{
			{0, true},
			{1, false}
		};

		private ushort m_HourIndex;
		private ushort m_MinuteIndex;
		private ushort m_AmPmIndex;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="hoursSmartObject"></param>
		/// <param name="minutesSmartObject"></param>
		/// <param name="amPmSmartObject"></param>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public SpinnerListClockWidget(ushort hoursSmartObject, ushort minutesSmartObject, ushort amPmSmartObject,
									  IPanelDevice panel, IVtProParent parent)
			: base(panel, parent)
		{
			m_HourList = new VtProSpinnerList(hoursSmartObject, panel, parent);
			m_MinuteList = new VtProSpinnerList(minutesSmartObject, panel, parent);
			m_AmPmList = new VtProSpinnerList(amPmSmartObject, panel, parent);

			m_HourList.OnItemSelected += HourListOnItemSelected;
			m_MinuteList.OnItemSelected += MinuteListOnItemSelected;
			m_AmPmList.OnItemSelected += AmPmListOnItemSelected;
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnSelectedTimeChanged = null;

			base.Dispose();

			m_HourList.OnItemSelected -= HourListOnItemSelected;
			m_MinuteList.OnItemSelected -= MinuteListOnItemSelected;
			m_AmPmList.OnItemSelected -= AmPmListOnItemSelected;
		}

		/// <summary>
		/// Sets the current selected time.
		/// We choose the next closest time in increments of 15 mins.
		/// </summary>
		/// <param name="time"></param>
		public void SetTime(DateTime time)
		{
			int minute = time.Minute; // 0 - 59

			int hour = time.Hour; // 0 - 23

			// 0 - 11
			bool am = hour < 12;
			if (!am)
				hour = hour - 12;

			// Rotate to 1 - 12
			if (hour == 0)
				hour += 12;

			minute = MathUtils.RoundToNearest(minute, s_IndexMinutes.Values);

			ushort hourIndex = s_IndexHours.GetKey(hour);
			ushort minuteIndex = s_IndexMinutes.GetKey(minute);
			ushort amPmIndex = s_IndexAm.GetKey(am);

			m_HourIndex = hourIndex;
			m_MinuteIndex = minuteIndex;
			m_AmPmIndex = amPmIndex;

			m_HourList.SelectItem(m_HourIndex);
			m_MinuteList.SelectItem(m_MinuteIndex);
			m_AmPmList.SelectItem(m_AmPmIndex);

			if (hourIndex == m_HourIndex && minuteIndex == m_MinuteIndex && amPmIndex == m_AmPmIndex)
				return;

			RaiseOnSelectedTimeChanged();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called when the AM/PM list selection changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void AmPmListOnItemSelected(object sender, UShortEventArgs args)
		{
			m_AmPmIndex = args.Data;
			RaiseOnSelectedTimeChanged();
		}

		/// <summary>
		/// Called when the minute list selection changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void MinuteListOnItemSelected(object sender, UShortEventArgs args)
		{
			m_MinuteIndex = args.Data;
			RaiseOnSelectedTimeChanged();
		}

		/// <summary>
		/// Called when the hour list selection 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void HourListOnItemSelected(object sender, UShortEventArgs args)
		{
			m_HourIndex = args.Data;
			RaiseOnSelectedTimeChanged();
		}

		/// <summary>
		/// Raises the OnSelectedTimeChanged event.
		/// </summary>
		private void RaiseOnSelectedTimeChanged()
		{
			int hour = s_IndexHours[m_HourIndex];
			int minute = s_IndexMinutes[m_MinuteIndex];
			bool am = s_IndexAm[m_AmPmIndex];

			DateTime dateTime = FromTime(hour, minute, am);

			TimeChangedCallback handler = OnSelectedTimeChanged;
			if (handler != null)
				handler(this, dateTime);
		}

		/// <summary>
		/// Returns the 12 hour clock time as a DateTime.
		/// </summary>
		/// <param name="hour">Hour in the range 1-12 inclusive</param>
		/// <param name="minute">Minute in the range 0-59 inclusive</param>
		/// <param name="am">True for AM, false for PM</param>
		/// <returns></returns>
		private static DateTime FromTime(int hour, int minute, bool am)
		{
			while (hour >= 12)
				hour -= 12;

			if (!am)
				hour += 12;

			DateTime now = IcdEnvironment.GetLocalTime();

			return new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
		}

		/// <summary>
		/// Gets the child controls that make up this widget.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetControls()
		{
			yield return m_HourList;
			yield return m_MinuteList;
			yield return m_AmPmList;
		}

		#endregion
	}
}
