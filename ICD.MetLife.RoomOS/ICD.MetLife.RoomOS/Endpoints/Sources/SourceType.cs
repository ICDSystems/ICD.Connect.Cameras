using System;

namespace ICD.MetLife.RoomOS.Endpoints.Sources
{
	public enum eSourceType
	{
		Laptop,
		Wireless,
		Wallplate,
		Floorbox,
		Game,
		Podium,
		Pc,
		Audio,
		CableBox,
		Camera,
	}

	/// <summary>
	/// Extension methods for the eSourceType enum.
	/// </summary>
	public static class SourceTypeExtensions
	{
		/// <summary>
		/// Gets the verb for the given source type, e.g. Play
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string GetSourceVerb(this eSourceType extends)
		{
			switch (extends)
			{
				case eSourceType.Laptop:
				case eSourceType.Wireless:
				case eSourceType.Wallplate:
				case eSourceType.Floorbox:
				case eSourceType.Podium:
				case eSourceType.Pc:
				case eSourceType.Camera:
					return "View";

				case eSourceType.Game:
				case eSourceType.Audio:
					return "Play";

				case eSourceType.CableBox:
					return "Watch";

				default:
					throw new ArgumentOutOfRangeException("extends");
			}
		}

		/// <summary>
		/// Gets the present participle for the given source type, e.g. Playing.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string GetSourcePresentParticiple(this eSourceType extends)
		{
			string verb = GetSourceVerb(extends);
			return string.Format("{0}ing", verb);
		}
	}
}
