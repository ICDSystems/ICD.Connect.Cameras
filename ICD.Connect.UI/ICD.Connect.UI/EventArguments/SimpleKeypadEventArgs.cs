using ICD.Common.EventArguments;

namespace ICD.Connect.UI.EventArguments
{
	/// <summary>
	/// Args for a keypad press event.
	/// </summary>
	public sealed class SimpleKeypadEventArgs : GenericEventArgs<SimpleKeypadEventArgs.eButton>
	{
		public enum eButton
		{
			Zero = 0,
			One = 1,
			Two = 2,
			Three = 3,
			Four = 4,
			Five = 5,
			Six = 6,
			Seven = 7,
			Eight = 8,
			Nine = 9,

			MiscButtonOne = 11,
			MiscButtonTwo = 12
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="button"></param>
		public SimpleKeypadEventArgs(eButton button)
			: base(button)
		{
		}
	}
}
