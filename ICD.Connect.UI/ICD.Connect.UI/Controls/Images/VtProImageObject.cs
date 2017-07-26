using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Connect.Panels;

namespace ICD.Connect.UI.Controls.Images
{
	public sealed class VtProImageObject : AbstractVtProControl<ISigInputOutput>
	{
		private readonly SafeCriticalSection m_SetImageSection;
		private readonly SafeCriticalSection m_SetModeSection;

		private string m_UrlCache;
		private ushort m_ModeCache;

		#region Properties

		[PublicAPI]
		public ushort SerialGraphicsJoin { get; set; }

		[PublicAPI]
		public ushort ModeAnalogJoin { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public VtProImageObject(ISigInputOutput panel)
			: this(panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sigDevice"></param>
		/// <param name="parent"></param>
		public VtProImageObject(ISigInputOutput sigDevice, IVtProParent parent)
			: base(sigDevice, parent)
		{
			m_SetImageSection = new SafeCriticalSection();
			m_SetModeSection = new SafeCriticalSection();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sets the url for the image.
		/// </summary>
		/// <param name="url"></param>
		[PublicAPI]
		public void SetImageUrl(string url)
		{
			m_SetImageSection.Enter();

			try
			{
				if (SerialGraphicsJoin == 0)
					throw new InvalidOperationException();

				url = url ?? string.Empty;
				if (url == (m_UrlCache ?? string.Empty))
					return;

				ushort join = Parent == null ? SerialGraphicsJoin : Parent.GetSerialJoin(SerialGraphicsJoin, this);

				m_UrlCache = url;
				Panel.SendInputSerial(join, m_UrlCache);
			}
			finally
			{
				m_SetImageSection.Leave();
			}
		}

		/// <summary>
		/// Sets the image mode.
		/// </summary>
		/// <param name="mode"></param>
		public void SetMode(ushort mode)
		{
			m_SetModeSection.Enter();

			try
			{
				if (ModeAnalogJoin == 0)
					throw new InvalidOperationException();

				if (mode == m_ModeCache)
					return;

				ushort join = Parent == null ? ModeAnalogJoin : Parent.GetAnalogJoin(ModeAnalogJoin, this);

				m_ModeCache = mode;
				Panel.SendInputAnalog(join, m_ModeCache);
			}
			finally
			{
				m_SetModeSection.Leave();
			}
		}

		#endregion
	}
}
