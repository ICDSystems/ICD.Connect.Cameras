namespace ICD.Connect.UI.Utils
{
	/// <summary>
	/// Utility methods for UIs.
	/// </summary>
	public static class HtmlUtils
	{
		/// <summary>
		/// Newline HTML for labels.
		/// </summary>
		public const string NEWLINE = "<br>";

		/// <summary>
		/// Formats the text to the given color.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="colorHex">String in the format #FFFFFF</param>
		/// <returns></returns>
		public static string FormatColoredText(string text, string colorHex)
		{
			return string.Format("<FONT color=\"{0}\">{1}</FONT>", colorHex, text);
		}
	}
}
