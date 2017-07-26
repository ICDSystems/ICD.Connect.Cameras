using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls.Buttons;

namespace ICD.Connect.UI.Controls.Images
{
	public sealed class VtProDynamicIconObject : AbstractVtProButton
	{
		private readonly SafeCriticalSection m_SetIconSection;
		private readonly SafeCriticalSection m_SetIconPathSection;

		private string m_IconCache;
		private string m_IconPathCache;

		#region Properties

		[PublicAPI]
		public ushort DynamicIconSerialJoin { get; set; }

		[PublicAPI]
		public ushort IndirectGraphicsPathSerialJoin { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public VtProDynamicIconObject(ISigInputOutput panel)
			: this(panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProDynamicIconObject(ISigInputOutput panel, IVtProParent parent)
			: base(panel, parent)
		{
			m_SetIconSection = new SafeCriticalSection();
			m_SetIconPathSection = new SafeCriticalSection();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sets the icon.
		/// </summary>
		/// <param name="icon"></param>
		public void SetIcon(string icon)
		{
			m_SetIconSection.Enter();

			try
			{
				if (DynamicIconSerialJoin == 0)
					throw new InvalidOperationException();

				icon = icon ?? string.Empty;
				if (icon == (m_IconCache ?? string.Empty))
					return;

				ushort join = Parent == null ? DynamicIconSerialJoin : Parent.GetSerialJoin(DynamicIconSerialJoin, this);

				m_IconCache = icon;
				Panel.SendInputSerial(join, m_IconCache);
			}
			finally
			{
				m_SetIconSection.Leave();
			}
		}

		/// <summary>
		/// Sets the icon from the given path.
		/// </summary>
		/// <param name="path"></param>
		public void SetIconPath(string path)
		{
			m_SetIconPathSection.Enter();

			try
			{
				if (IndirectGraphicsPathSerialJoin == 0)
					throw new InvalidOperationException();

				path = path ?? string.Empty;
				if (path == (m_IconPathCache ?? string.Empty))
					return;

				ushort join = Parent == null
					              ? IndirectGraphicsPathSerialJoin
					              : Parent.GetSerialJoin(IndirectGraphicsPathSerialJoin, this);

				m_IconPathCache = path;
				Panel.SendInputSerial(join, path);
			}
			finally
			{
				m_SetIconPathSection.Leave();
			}
		}

		#endregion
	}
}
