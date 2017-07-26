using ICD.Common.Properties;
using ICD.Connect.Panels;

namespace ICD.Connect.UI.Controls.Pages
{
	public abstract class AbstractVtProPage<T> : AbstractVtProControl<T>, IVtProParent
		where T : class, ISigInputOutput
	{
		#region Properties

		/// <summary>
		/// The digital join offset is added to all child control digital joins.
		/// </summary>
		[PublicAPI]
		public ushort DigitalJoinOffset { get; set; }

		/// <summary>
		/// The analog join offset is added to all child control analog joins.
		/// </summary>
		[PublicAPI]
		public ushort AnalogJoinOffset { get; set; }

		/// <summary>
		/// The serial join offset is added to all child control serial joins.
		/// </summary>
		[PublicAPI]
		public ushort SerialJoinOffset { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractVtProPage(T panel)
			: base(panel)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		protected AbstractVtProPage(T panel, IVtProParent parent)
			: base(panel, parent)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractVtProPage(T panel, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the digital join offset for the given control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public ushort GetDigitalJoinOffset(IIndexed control)
		{
			return Parent == null ? DigitalJoinOffset : (ushort)(Parent.GetDigitalJoinOffset(this) + DigitalJoinOffset);
		}

		/// <summary>
		/// Gets the analog join offset for the given control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public ushort GetAnalogJoinOffset(IIndexed control)
		{
			return Parent == null ? AnalogJoinOffset : (ushort)(Parent.GetAnalogJoinOffset(this) + AnalogJoinOffset);
		}

		/// <summary>
		/// Gets the serial join offset for the given control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public ushort GetSerialJoinOffset(IIndexed control)
		{
			return Parent == null ? SerialJoinOffset : (ushort)(Parent.GetSerialJoinOffset(this) + SerialJoinOffset);
		}

		#endregion
	}
}
