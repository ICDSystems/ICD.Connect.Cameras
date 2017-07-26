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
	public sealed class VtProDPad : AbstractVtProSmartObject
	{
		private const ushort UP = 1;
		private const ushort DOWN = 2;
		private const ushort LEFT = 3;
		private const ushort RIGHT = 4;
		private const ushort CENTER = 5;

		public event EventHandler<DPadEventArgs> OnButtonPressed;
		public event EventHandler<DPadEventArgs> OnButtonReleased;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProDPad(uint smartObjectId, IPanelDevice panel, IVtProParent parent)
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
		/// Simulates a Press on the directional button.
		/// </summary>
		/// <param name="direction"></param>
		[PublicAPI]
		public void Press(DPadEventArgs.eDirection direction)
		{
			if (IsVisibleRecursive)
				OnButtonPressed.Raise(this, new DPadEventArgs(direction));
		}

		/// <summary>
		/// Simulates a release on the directional button.
		/// </summary>
		/// <param name="direction"></param>
		[PublicAPI]
		public void Release(DPadEventArgs.eDirection direction)
		{
			if (IsVisibleRecursive)
				OnButtonReleased.Raise(this, new DPadEventArgs(direction));
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

			smartObject.RegisterOutputSigChangeCallback(UP, eSigType.Digital, UpClicked);
			smartObject.RegisterOutputSigChangeCallback(DOWN, eSigType.Digital, DownClicked);
			smartObject.RegisterOutputSigChangeCallback(LEFT, eSigType.Digital, LeftClicked);
			smartObject.RegisterOutputSigChangeCallback(RIGHT, eSigType.Digital, RightClicked);
			smartObject.RegisterOutputSigChangeCallback(CENTER, eSigType.Digital, CenterClicked);
		}

		/// <summary>
		/// Unsubscribe from the smart object events.
		/// </summary>
		/// <param name="smartObject"></param>
		protected override void Unsubscribe(ISmartObject smartObject)
		{
			base.Unsubscribe(smartObject);

			smartObject.RegisterOutputSigChangeCallback(UP, eSigType.Digital, UpClicked);
			smartObject.RegisterOutputSigChangeCallback(DOWN, eSigType.Digital, DownClicked);
			smartObject.RegisterOutputSigChangeCallback(LEFT, eSigType.Digital, LeftClicked);
			smartObject.RegisterOutputSigChangeCallback(RIGHT, eSigType.Digital, RightClicked);
			smartObject.RegisterOutputSigChangeCallback(CENTER, eSigType.Digital, CenterClicked);
		}

		/// <summary>
		/// Called when the up button is clicked.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void UpClicked(object parent, SigAdapterEventArgs args)
		{
			Press(DPadEventArgs.eDirection.Up, args);
		}

		/// <summary>
		/// Called when the down button is clicked.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void DownClicked(object parent, SigAdapterEventArgs args)
		{
			Press(DPadEventArgs.eDirection.Down, args);
		}

		/// <summary>
		/// Called when the left button is clicked.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void LeftClicked(object parent, SigAdapterEventArgs args)
		{
			Press(DPadEventArgs.eDirection.Left, args);
		}

		/// <summary>
		/// Called when the right button is clicked.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void RightClicked(object parent, SigAdapterEventArgs args)
		{
			Press(DPadEventArgs.eDirection.Right, args);
		}

		/// <summary>
		/// Called when the center button is clicked.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void CenterClicked(object parent, SigAdapterEventArgs args)
		{
			Press(DPadEventArgs.eDirection.Center, args);
		}

		private void Press(DPadEventArgs.eDirection direction, SigAdapterEventArgs args)
		{
			if (args.Data.GetBoolValue())
				Press(direction);
			else
				Release(direction);
		}

		#endregion
	}
}
