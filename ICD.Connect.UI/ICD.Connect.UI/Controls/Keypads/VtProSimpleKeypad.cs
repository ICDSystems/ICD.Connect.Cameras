using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.Protocol.Sigs;
using ICD.Connect.UI.EventArguments;

namespace ICD.Connect.UI.Controls.Keypads
{
	public sealed class VtProSimpleKeypad : AbstractVtProSmartObject
	{
		private const ushort SIG_RANGE_START = 1;
		private const ushort SIG_RANGE_END = 12;

		public event EventHandler<SimpleKeypadEventArgs> OnButtonPressed;
		public event EventHandler<SimpleKeypadEventArgs> OnButtonReleased;

		// Defaults from VTPro
		private char m_MiscButtonOneChar = '*';
		private char m_MiscButtonTwoChar = '#';

		#region Properties

		/// <summary>
		/// The custom character for the bottom left button.
		/// </summary>
		[PublicAPI]
		public char MiscButtonOneChar { get { return m_MiscButtonOneChar; } set { m_MiscButtonOneChar = value; } }

		/// <summary>
		/// The custom character for the bottom right button.
		/// </summary>
		[PublicAPI]
		public char MiscButtonTwoChar { get { return m_MiscButtonTwoChar; } set { m_MiscButtonTwoChar = value; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		public VtProSimpleKeypad(uint smartObjectId, IPanelDevice panel)
			: base(smartObjectId, panel)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProSimpleKeypad(uint smartObjectId, IPanelDevice panel, IVtProParent parent)
			: base(smartObjectId, panel, parent)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnButtonPressed = null;
			OnButtonReleased = null;

			base.Dispose();
		}

		/// <summary>
		/// Simulates a Press on the button.
		/// </summary>
		/// <param name="button"></param>
		[PublicAPI]
		public void Press(SimpleKeypadEventArgs.eButton button)
		{
			if (IsVisibleRecursive)
				OnButtonPressed.Raise(this, new SimpleKeypadEventArgs(button));
		}

		/// <summary>
		/// Simulates a release on the button.
		/// </summary>
		/// <param name="button"></param>
		[PublicAPI]
		public void Release(SimpleKeypadEventArgs.eButton button)
		{
			if (IsVisibleRecursive)
				OnButtonReleased.Raise(this, new SimpleKeypadEventArgs(button));
		}

		/// <summary>
		/// Gives the character representation of the button.
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		public char GetButtonChar(SimpleKeypadEventArgs.eButton button)
		{
			switch (button)
			{
				case SimpleKeypadEventArgs.eButton.Zero:
				case SimpleKeypadEventArgs.eButton.One:
				case SimpleKeypadEventArgs.eButton.Two:
				case SimpleKeypadEventArgs.eButton.Three:
				case SimpleKeypadEventArgs.eButton.Four:
				case SimpleKeypadEventArgs.eButton.Five:
				case SimpleKeypadEventArgs.eButton.Six:
				case SimpleKeypadEventArgs.eButton.Seven:
				case SimpleKeypadEventArgs.eButton.Eight:
				case SimpleKeypadEventArgs.eButton.Nine:
					return ((int)button).ToString()[0];

				case SimpleKeypadEventArgs.eButton.MiscButtonOne:
					return m_MiscButtonOneChar;
				case SimpleKeypadEventArgs.eButton.MiscButtonTwo:
					return m_MiscButtonTwoChar;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribe to the smart object events.
		/// </summary>
		/// <param name="smartObject"></param>
		protected override void Subscribe(ISmartObject smartObject)
		{
			base.Subscribe(smartObject);

			for (ushort index = SIG_RANGE_START; index <= SIG_RANGE_END; index++)
				smartObject.RegisterOutputSigChangeCallback(index, eSigType.Digital, ButtonPressed);
		}

		/// <summary>
		/// Unsubscribe from the smart object events.
		/// </summary>
		/// <param name="smartObject"></param>
		protected override void Unsubscribe(ISmartObject smartObject)
		{
			base.Unsubscribe(smartObject);

			for (ushort index = SIG_RANGE_START; index <= SIG_RANGE_END; index++)
				smartObject.UnregisterOutputSigChangeCallback(index, eSigType.Digital, ButtonPressed);
		}

		/// <summary>
		/// Called when a button sig is received from the smart object.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void ButtonPressed(object parent, SigAdapterEventArgs args)
		{
			uint number = args.Data.Number;

			// The zero button has an index of 10
			if (number == 10)
				number = 0;

			SimpleKeypadEventArgs.eButton button = (SimpleKeypadEventArgs.eButton)number;

			if (args.Data.GetBoolValue())
				Press(button);
			else
				Release(button);
		}

		#endregion
	}
}
