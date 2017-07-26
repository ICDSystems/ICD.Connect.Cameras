using ICD.Common.EventArguments;

namespace ICD.Connect.UI.EventArguments
{
	public sealed class DPadEventArgs : GenericEventArgs<DPadEventArgs.eDirection>
	{
		public enum eDirection
		{
			Up,
			Down,
			Left,
			Right,
			Center
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="direction"></param>
		public DPadEventArgs(eDirection direction)
			: base(direction)
		{
		}
	}
}
