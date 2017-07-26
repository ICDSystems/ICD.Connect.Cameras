using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Utils;

namespace ICD.Connect.UI.Controls.TextControls
{
	public abstract class AbstractVtProLabel : AbstractVtProControl<ISigInputOutput>
	{
		private readonly SafeCriticalSection m_SerialSection;
		private readonly SafeCriticalSection m_AnalogSection;
		private readonly SafeCriticalSection m_DigitalSection;

		private List<ushort> m_SerialLabelJoins;
		private List<ushort> m_AnalogLabelJoins;
		private List<ushort> m_DigitalLabelJoins;

		private Dictionary<ushort, string> m_SerialLabelsCache;
		private Dictionary<ushort, ushort> m_AnalogLabelsCache;
		private Dictionary<ushort, bool> m_DigitalLabelsCache;

		#region Properties

		[PublicAPI]
		public ushort IndirectTextJoin { get; set; }

		public List<ushort> SerialLabelJoins
		{
			get { return m_SerialLabelJoins ?? (m_SerialLabelJoins = new List<ushort>()); }
		}

		public List<ushort> AnalogLabelJoins
		{
			get { return m_AnalogLabelJoins ?? (m_AnalogLabelJoins = new List<ushort>()); }
		}

		public List<ushort> DigitalLabelJoins
		{
			get { return m_DigitalLabelJoins ?? (m_DigitalLabelJoins = new List<ushort>()); }
		}

		private Dictionary<ushort, string> SerialLabelsCache
		{
			get { return m_SerialLabelsCache ?? (m_SerialLabelsCache = new Dictionary<ushort, string>()); }
		}

		private Dictionary<ushort, ushort> AnalogLabelsCache
		{
			get { return m_AnalogLabelsCache ?? (m_AnalogLabelsCache = new Dictionary<ushort, ushort>()); }
		}

		private Dictionary<ushort, bool> DigitalLabelsCache
		{
			get { return m_DigitalLabelsCache ?? (m_DigitalLabelsCache = new Dictionary<ushort, bool>()); }
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		protected AbstractVtProLabel(ISigInputOutput panel)
			: this(panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		protected AbstractVtProLabel(ISigInputOutput panel, IVtProParent parent)
			: base(panel, parent)
		{
			m_AnalogSection = new SafeCriticalSection();
			m_DigitalSection = new SafeCriticalSection();
			m_SerialSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Sets the label text. Throws InvalidOperationException if there is no indirect text join.
		/// </summary>
		/// <param name="text"></param>
		[PublicAPI]
		public void SetLabelText(string text)
		{
			SetLabelTextAtJoin(IndirectTextJoin, text);
		}

		/// <summary>
		/// Sets the label text at the given join.
		/// </summary>
		/// <param name="join"></param>
		/// <param name="text"></param>
		public void SetLabelTextAtJoin(ushort join, string text)
		{
			m_SerialSection.Enter();

			try
			{
				if (join == 0)
					throw new InvalidOperationException();

				join = Parent == null ? join : Parent.GetSerialJoin(join, this);

				// Replace newline chars with html
				text = Regex.Replace(text ?? string.Empty, @"\n\r|\r\n|\n|\r", HtmlUtils.NEWLINE);

				string cache = SerialLabelsCache.GetDefault(join, string.Empty);
				if (text == cache)
					return;

				SerialLabelsCache[join] = text;
				Panel.SendInputSerial(join, text);
			}
			finally
			{
				m_SerialSection.Leave();
			}
		}

		/// <summary>
		/// Sets the label text at the given join.
		/// </summary>
		/// <param name="join"></param>
		/// <param name="value"></param>
		public void SetLabelTextAtJoin(ushort join, ushort value)
		{
			m_AnalogSection.Enter();

			try
			{
				if (join == 0)
					throw new InvalidOperationException();

				join = Parent == null ? join : Parent.GetAnalogJoin(join, this);

				ushort cache = AnalogLabelsCache.GetDefault(join, (ushort)0);
				if (value == cache)
					return;

				AnalogLabelsCache[join] = value;
				Panel.SendInputAnalog(join, value);
			}
			finally
			{
				m_AnalogSection.Leave();
			}
		}

		/// <summary>
		/// Sets the label text at the given join.
		/// </summary>
		/// <param name="join"></param>
		/// <param name="value"></param>
		public void SetLabelTextAtJoin(ushort join, bool value)
		{
			m_DigitalSection.Enter();

			try
			{
				if (join == 0)
					throw new InvalidOperationException();

				join = Parent == null ? join : Parent.GetDigitalJoin(join, this);

				bool cache = DigitalLabelsCache.GetDefault(join, false);
				if (value == cache)
					return;

				DigitalLabelsCache[join] = value;
				Panel.SendInputDigital(join, value);
			}
			finally
			{
				m_DigitalSection.Leave();
			}
		}

		#endregion
	}
}
