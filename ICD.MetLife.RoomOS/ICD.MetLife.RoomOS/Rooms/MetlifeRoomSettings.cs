using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Xml;
using ICD.Connect.Conferencing.Cisco.Components.Directory.Tree;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.MetLife.RoomOS.VolumePoints;

namespace ICD.MetLife.RoomOS.Rooms
{
	/// <summary>
	/// Settings for the MetlifeRoom.
	/// </summary>
	public sealed class MetlifeRoomSettings : AbstractRoomSettings
	{
		private const int DEFAULT_INACTIVITY_SECONDS = 10 * 60;

		private const string FACTORY_NAME = "MetlifeRoom";

		private const string PREFIX_ELEMENT = "Prefix";
		private const string NUMBER_ELEMENT = "Number";
		private const string PHONE_NUMBER_ELEMENT = "PhoneNumber";

		private const string OWNER_ELEMENT = "Owner";
		private const string OWNER_NAME_ELEMENT = "Name";
		private const string OWNER_PHONE_ELEMENT = "Phone";
		private const string OWNER_EMAIL_ELEMENT = "Email";

		private const string DIALINGPLAN_ELEMENT = "DialingPlan";
		private const string PHONEBOOKTYPE_ELEMENT = "PhonebookType";
		private const string TVPRESETS_ELEMENT = "TvPresets";
		private const string VOLUME_POINTS_ELEMENT = "VolumePoints";

		private const string INACTIVITY_SECONDS_ELEMENT = "InactivitySeconds";

		public event EventHandler<StringEventArgs> OnPrefixChanged;
		public event EventHandler<StringEventArgs> OnNumberChanged;
		public event EventHandler<StringEventArgs> OnPhoneNumberChanged;
		public event EventHandler<StringEventArgs> OnOwnerNameChanged;

		private readonly IcdHashSet<VolumePoint> m_VolumePoints;
		private readonly SafeCriticalSection m_VolumePointsSection;

		private string m_Prefix;
		private string m_Number;
		private string m_PhoneNumber;
		private string m_OwnerName;

		#region Properties

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(MetlifeRoom); } }

		/// <summary>
		/// Gets/sets the prefix (the building name)
		/// </summary>
		public string Prefix
		{
			get { return m_Prefix; }
			set
			{
				if (value == m_Prefix)
					return;

				m_Prefix = value;

				OnPrefixChanged.Raise(this, new StringEventArgs(m_Prefix));
			}
		}

		/// <summary>
		/// Gets/sets the room number (suffix).
		/// </summary>
		public string Number
		{
			get { return m_Number; }
			set
			{
				if (value == m_Number)
					return;

				m_Number = value;

				OnNumberChanged.Raise(this, new StringEventArgs(m_Number));
			}
		}

		/// <summary>
		/// Gets/sets the room phone number.
		/// </summary>
		public string PhoneNumber
		{
			get { return m_PhoneNumber; }
			set
			{
				if (value == m_PhoneNumber)
					return;

				m_PhoneNumber = value;

				OnPhoneNumberChanged.Raise(this, new StringEventArgs(m_PhoneNumber));
			}
		}

		public DialingPlanInfo DialingPlan { get; set; }
		public ePhonebookType PhonebookType { get; set; }
		public string TvPresets { get; set; }

		/// <summary>
		/// Gets/sets the owner name.
		/// </summary>
		public string OwnerName
		{
			get { return m_OwnerName; }
			set
			{
				if (value == m_OwnerName)
					return;

				m_OwnerName = value;

				OnOwnerNameChanged.Raise(this, new StringEventArgs(m_OwnerName));
			}
		}

		public string OwnerPhone { get; set; }
		public string OwnerEmail { get; set; }

		public int InactivitySeconds { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public MetlifeRoomSettings()
		{
			m_VolumePoints = new IcdHashSet<VolumePoint>();
			m_VolumePointsSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			writer.WriteElementString(PREFIX_ELEMENT, Prefix);
			writer.WriteElementString(NUMBER_ELEMENT, Number);
			writer.WriteElementString(PHONE_NUMBER_ELEMENT, PhoneNumber);

			WriteOwnerElement(writer);

			DialingPlan.WriteToXml(writer, DIALINGPLAN_ELEMENT);
			writer.WriteElementString(PHONEBOOKTYPE_ELEMENT, PhonebookType.ToString());
			writer.WriteElementString(TVPRESETS_ELEMENT, TvPresets);

			if (InactivitySeconds != DEFAULT_INACTIVITY_SECONDS)
				writer.WriteElementString(INACTIVITY_SECONDS_ELEMENT, IcdXmlConvert.ToString(InactivitySeconds));

			WriteVolumePointsElement(writer);

			base.WriteElements(writer);
		}

		/// <summary>
		/// Instantiates room settings from an xml element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[XmlRoomSettingsFactoryMethod(FACTORY_NAME)]
		public static MetlifeRoomSettings FromXml(string xml)
		{
			string prefix = XmlUtils.TryReadChildElementContentAsString(xml, PREFIX_ELEMENT);
			string number = XmlUtils.TryReadChildElementContentAsString(xml, NUMBER_ELEMENT);
			string phoneNumber = XmlUtils.TryReadChildElementContentAsString(xml, PHONE_NUMBER_ELEMENT);
			string phonebookType = XmlUtils.TryReadChildElementContentAsString(xml, PHONEBOOKTYPE_ELEMENT);
			string tvPresets = XmlUtils.TryReadChildElementContentAsString(xml, TVPRESETS_ELEMENT);

			string volumePoints;
			XmlUtils.TryGetChildElementAsString(xml, VOLUME_POINTS_ELEMENT, out volumePoints);
			string owner;
			XmlUtils.TryGetChildElementAsString(xml, OWNER_ELEMENT, out owner);

			int? inactivitySeconds = XmlUtils.TryReadChildElementContentAsInt(xml, INACTIVITY_SECONDS_ELEMENT);

			ePhonebookType phonebookTypeEnum = (ePhonebookType)Enum.Parse(typeof(ePhonebookType), phonebookType, true);
			string dialingPlan;
			XmlUtils.TryGetChildElementAsString(xml, DIALINGPLAN_ELEMENT, out dialingPlan);
			DialingPlanInfo dialingPlanInfo = string.IsNullOrEmpty(dialingPlan)
				                                  ? new DialingPlanInfo()
				                                  : DialingPlanInfo.FromXml(dialingPlan);

			MetlifeRoomSettings output = new MetlifeRoomSettings
			{
				Prefix = prefix,
				Number = number,
				PhoneNumber = phoneNumber,
				DialingPlan = dialingPlanInfo,
				PhonebookType = phonebookTypeEnum,
				TvPresets = tvPresets,
				InactivitySeconds = inactivitySeconds == null ? DEFAULT_INACTIVITY_SECONDS : (int)inactivitySeconds
			};

			if (volumePoints != null)
				output.ParseVolumePointsXml(volumePoints);
			if (owner != null)
				output.ParseOwnerXml(owner);

			ParseXml(output, xml);
			return output;
		}

		#endregion

		#region Methods

		public void SetVolumePoints(IEnumerable<VolumePoint> volumePoints)
		{
			m_VolumePointsSection.Enter();

			try
			{
				m_VolumePoints.Clear();
				m_VolumePoints.AddRange(volumePoints);
			}
			finally
			{
				m_VolumePointsSection.Leave();
			}
		}

		/// <summary>
		/// Gets the volume points.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<VolumePoint> GetVolumePoints()
		{
			return m_VolumePointsSection.Execute(() => m_VolumePoints.OrderBy(v => v.DeviceId).ThenBy(v => v.ControlId).ToArray());
		}

		/// <summary>
		/// Adds the volume point.
		/// </summary>
		/// <param name="volumePoint"></param>
		/// <returns></returns>
		public bool AddVolumePoint(VolumePoint volumePoint)
		{
			return m_VolumePointsSection.Execute(() => m_VolumePoints.Add(volumePoint));
		}

		/// <summary>
		/// Removes the volume point.
		/// </summary>
		/// <param name="volumePoint"></param>
		/// <returns></returns>
		public bool RemoveVolumePoint(VolumePoint volumePoint)
		{
			return m_VolumePointsSection.Execute(() => m_VolumePoints.Remove(volumePoint));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Parses the xml for owner information.
		/// </summary>
		/// <param name="xml"></param>
		private void ParseOwnerXml(string xml)
		{
			OwnerName = XmlUtils.TryReadChildElementContentAsString(xml, OWNER_NAME_ELEMENT);
			OwnerPhone = XmlUtils.TryReadChildElementContentAsString(xml, OWNER_PHONE_ELEMENT);
			OwnerEmail = XmlUtils.TryReadChildElementContentAsString(xml, OWNER_EMAIL_ELEMENT);
		}

		private void ParseVolumePointsXml(string xml)
		{
			IEnumerable<VolumePoint> volumePoints = XmlUtils.GetChildElementsAsString(xml)
			                                                .Select(x => VolumePoint.FromXml(x));
			SetVolumePoints(volumePoints);
		}

		/// <summary>
		/// Writes the owner details to an xml element.
		/// </summary>
		/// <param name="writer"></param>
		private void WriteOwnerElement(IcdXmlTextWriter writer)
		{
			writer.WriteStartElement(OWNER_ELEMENT);
			{
				writer.WriteElementString(OWNER_NAME_ELEMENT, OwnerName);
				writer.WriteElementString(OWNER_PHONE_ELEMENT, OwnerName);
				writer.WriteElementString(OWNER_EMAIL_ELEMENT, OwnerEmail);
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Writes the volume points to xml.
		/// </summary>
		/// <param name="writer"></param>
		private void WriteVolumePointsElement(IcdXmlTextWriter writer)
		{
			writer.WriteStartElement(VOLUME_POINTS_ELEMENT);
			{
				foreach (VolumePoint volumePoint in GetVolumePoints())
					volumePoint.WriteElement(writer);
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}
