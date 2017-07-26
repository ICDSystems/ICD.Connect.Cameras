using System;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views
{
	public static class VtProColors
	{
		/// <summary>
		/// Converts the color enum to a hex string in the format #FFFFFF.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string ColorToHexString(eColor color)
		{
			switch (color)
			{
				case eColor.Default:
					return "#FBFBFB";
				case eColor.Green:
					return "#43BF6C";
				case eColor.Yellow:
					return "#F2C609";
				case eColor.Red:
					return "#d95050";
				case eColor.Blue:
					return "#6E91B3";
				default:
					throw new ArgumentOutOfRangeException("color");
			}
		}
	}
}
