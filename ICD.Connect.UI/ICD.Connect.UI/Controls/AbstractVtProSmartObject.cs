using ICD.Common.Properties;
using ICD.Connect.Panels;
using ICD.Connect.Panels.SmartObjects;

namespace ICD.Connect.UI.Controls
{
	public abstract class AbstractVtProSmartObject : AbstractVtProControl<IPanelDevice>
	{
		[PublicAPI]
		public ushort VisibilityDigitalJoin { get { return DigitalVisibilityJoin; } set { DigitalVisibilityJoin = value; } }

		/// <summary>
		/// Gets the smart object.
		/// </summary>
		public ISmartObject SmartObject { get; private set; }

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		protected AbstractVtProSmartObject(uint smartObjectId, IPanelDevice panel)
			: this(smartObjectId, panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		protected AbstractVtProSmartObject(uint smartObjectId, IPanelDevice panel, IVtProParent parent)
			: base(panel, parent)
		{
			SmartObject = panel.SmartObjects[smartObjectId];
			Subscribe(SmartObject);
		}

		#endregion

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();
			Unsubscribe(SmartObject);
		}

		/// <summary>
		/// Subscribe to the smart object events.
		/// </summary>
		/// <param name="smartObject"></param>
		protected virtual void Subscribe(ISmartObject smartObject)
		{
		}

		/// <summary>
		/// Unsubscribe from the smart object events.
		/// </summary>
		/// <param name="smartObject"></param>
		protected virtual void Unsubscribe(ISmartObject smartObject)
		{
		}
	}
}
