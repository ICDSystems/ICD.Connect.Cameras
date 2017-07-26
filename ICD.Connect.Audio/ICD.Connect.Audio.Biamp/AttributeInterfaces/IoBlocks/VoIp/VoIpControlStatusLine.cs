﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Audio.Biamp.TesiraTextProtocol.Codes;
using ICD.Connect.Audio.Biamp.TesiraTextProtocol.Parsing;

namespace ICD.Connect.Audio.Biamp.AttributeInterfaces.IoBlocks.VoIp
{
	public sealed class VoIpControlStatusLine : AbstractAttributeChild<VoIpControlStatusBlock>
	{
		public enum eAutoAnswerRingCount
		{
			Immediately,
			OneRing,
			TwoRings,
			ThreeRings
		}

		public enum eRingType
		{
			Classic,
			Silent
		}

		private static readonly Dictionary<string, eAutoAnswerRingCount> s_AutoAnswerRingCountSerials =
			new Dictionary<string, eAutoAnswerRingCount>(StringComparer.InvariantCultureIgnoreCase)
			{
				{"AA_IMMEDIATELY", eAutoAnswerRingCount.Immediately},
				{"AA_ONE_RING", eAutoAnswerRingCount.OneRing},
				{"AA_TWO_RINGS", eAutoAnswerRingCount.TwoRings},
				{"AA_THREE_RINGS", eAutoAnswerRingCount.ThreeRings}
			};

		private static readonly Dictionary<string, eRingType> s_RingTypeSerials =
			new Dictionary<string, eRingType>(StringComparer.InvariantCultureIgnoreCase)
			{
				{"RING_TYPE_CLASSIC", eRingType.Classic},
				{"RING_TYPE_SILENT", eRingType.Silent}
			};

		private const string DTMF_SERVICE = "dtmf";

		private const string AUTO_ANSWER_ATTRIBUTE = "autoAnswer";
		private const string AUTO_ANSWER_RING_COUNT_ATTRIBUTE = "autoAnswerRingCount";
		private const string CALL_PROGRESS_TONE_LEVEL_ATTRIBUTE = "cptLevel";
		private const string DIALING_TIMEOUT_ATTRIBUTE = "dialingTimeOut";
		private const string DTMF_OFF_TIME_ATTRIBUTE = "dtmfOffTime";
		private const string DTMF_ON_TIME_ATTRIBUTE = "dtmfOnTime";
		private const string LAST_NUMBER_DIALED_ATTRIBUTE = "lastNum";
		private const string LINE_READY_ATTRIBUTE = "lineReady";
		private const string DTMF_LOCAL_MUTE_ATTRIBUTE = "localDtmfMute";
		private const string DTMF_LOCAL_LEVEL_ATTRIBUTE = "localDtmfToneLevel";
		private const string REDIAL_ENABLED_ATTRIBUTE = "redialEnable";
		private const string RING_TYPE_ATTRIBUTE = "ringType";

		public delegate void AutoAnswerRingCountCallback(VoIpControlStatusLine sender, eAutoAnswerRingCount count);

		public delegate void RingTypeCallback(VoIpControlStatusLine sender, eRingType ringType);

		[PublicAPI]
		public event EventHandler<BoolEventArgs> OnAutoAnswerChanged;

		[PublicAPI]
		public event AutoAnswerRingCountCallback OnAutoAnswerRingCountChanged;

		[PublicAPI]
		public event EventHandler<FloatEventArgs> OnCallProgressToneLevelChanged;

		[PublicAPI]
		public event EventHandler<IntEventArgs> OnDialingTimeoutChanged;

		[PublicAPI]
		public event EventHandler<IntEventArgs> OnDtmfOffTimeChanged;

		[PublicAPI]
		public event EventHandler<IntEventArgs> OnDtmfOnTimeChanged;

		[PublicAPI]
		public event EventHandler<StringEventArgs> OnLastNumberDialedChanged;

		[PublicAPI]
		public event EventHandler<BoolEventArgs> OnLineReadyChanged;

		[PublicAPI]
		public event EventHandler<BoolEventArgs> OnDtmfLocalMuteChanged;

		[PublicAPI]
		public event EventHandler<FloatEventArgs> OnDtmfLocalLevelChanged;

		[PublicAPI]
		public event EventHandler<BoolEventArgs> OnRedialEnabledChanged;

		[PublicAPI]
		public event RingTypeCallback OnRingTypeChanged;

		private readonly Dictionary<int, VoIpControlStatusCallAppearance> m_CallAppearances;
		private readonly SafeCriticalSection m_CallAppearancesSection;

		private bool m_AutoAnswer;
		private eAutoAnswerRingCount m_AutoAnswerRingCount;
		private float m_CallProgressToneLevel;
		private int m_DialingTimeout;
		private int m_DtmfOffTime;
		private int m_DtmfOnTime;
		private string m_LastNumberDialed;
		private bool m_LineReady;
		private bool m_DtmfLocalMute;
		private float m_DtmfLocalLevel;
		private bool m_RedialEnabled;
		private eRingType m_RingType;

		#region Properties

		/// <summary>
		/// Gets the number of call appearances.
		/// </summary>
		[PublicAPI]
		public int CallAppearanceCount { get { return 6; } }

		[PublicAPI]
		public bool AutoAnswer
		{
			get { return m_AutoAnswer; }
			private set
			{
				if (value == m_AutoAnswer)
					return;

				m_AutoAnswer = value;

				OnAutoAnswerChanged.Raise(this, new BoolEventArgs(m_AutoAnswer));
			}
		}

		[PublicAPI]
		public eAutoAnswerRingCount AutoAnswerRingCount
		{
			get { return m_AutoAnswerRingCount; }
			private set
			{
				if (value == m_AutoAnswerRingCount)
					return;

				m_AutoAnswerRingCount = value;

				AutoAnswerRingCountCallback handler = OnAutoAnswerRingCountChanged;
				if (handler != null)
					handler(this, m_AutoAnswerRingCount);
			}
		}

		[PublicAPI]
		public float CallProgressToneLevel
		{
			get { return m_CallProgressToneLevel; }
			private set
			{
				if (value == m_CallProgressToneLevel)
					return;

				m_CallProgressToneLevel = value;

				OnCallProgressToneLevelChanged.Raise(this, new FloatEventArgs(m_CallProgressToneLevel));
			}
		}

		[PublicAPI]
		public int DialingTimeout
		{
			get { return m_DialingTimeout; }
			private set
			{
				if (value == m_DialingTimeout)
					return;

				m_DialingTimeout = value;

				OnDialingTimeoutChanged.Raise(this, new IntEventArgs(m_DialingTimeout));
			}
		}

		[PublicAPI]
		public int DtmfOffTime
		{
			get { return m_DtmfOffTime; }
			private set
			{
				if (value == m_DtmfOffTime)
					return;

				m_DtmfOffTime = value;

				OnDtmfOffTimeChanged.Raise(this, new IntEventArgs(m_DtmfOffTime));
			}
		}

		[PublicAPI]
		public int DtmfOnTime
		{
			get { return m_DtmfOnTime; }
			private set
			{
				if (value == m_DtmfOnTime)
					return;

				m_DtmfOnTime = value;

				OnDtmfOnTimeChanged.Raise(this, new IntEventArgs(m_DtmfOnTime));
			}
		}

		[PublicAPI]
		public string LastNumberDialed
		{
			get { return m_LastNumberDialed; }
			private set
			{
				if (value == m_LastNumberDialed)
					return;

				m_LastNumberDialed = value;

				OnLastNumberDialedChanged.Raise(this, new StringEventArgs(m_LastNumberDialed));
			}
		}

		[PublicAPI]
		public bool LineReady
		{
			get { return m_LineReady; }
			private set
			{
				if (value == m_LineReady)
					return;

				m_LineReady = value;

				OnLineReadyChanged.Raise(this, new BoolEventArgs(m_LineReady));
			}
		}

		[PublicAPI]
		public bool DtmfLocalMute
		{
			get { return m_DtmfLocalMute; }
			private set
			{
				if (value == m_DtmfLocalMute)
					return;

				m_DtmfLocalMute = value;

				OnDtmfLocalMuteChanged.Raise(this, new BoolEventArgs(m_DtmfLocalMute));
			}
		}

		[PublicAPI]
		public float DtmfLocalLevel
		{
			get { return m_DtmfLocalLevel; }
			private set
			{
				if (value == m_DtmfLocalLevel)
					return;

				m_DtmfLocalLevel = value;

				OnDtmfLocalLevelChanged.Raise(this, new FloatEventArgs(m_DtmfLocalLevel));
			}
		}

		[PublicAPI]
		public bool RedialEnabled
		{
			get { return m_RedialEnabled; }
			private set
			{
				if (value == m_RedialEnabled)
					return;

				m_RedialEnabled = value;

				OnRedialEnabledChanged.Raise(this, new BoolEventArgs(m_RedialEnabled));
			}
		}

		[PublicAPI]
		public eRingType RingType
		{
			get { return m_RingType; }
			private set
			{
				if (value == m_RingType)
					return;

				m_RingType = value;

				RingTypeCallback handler = OnRingTypeChanged;
				if (handler != null)
					handler(this, m_RingType);
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public VoIpControlStatusLine(VoIpControlStatusBlock parent, int index)
			: base(parent, index)
		{
			m_CallAppearances = new Dictionary<int, VoIpControlStatusCallAppearance>();
			m_CallAppearancesSection = new SafeCriticalSection();

			if (Device.Initialized)
				Initialize();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnAutoAnswerChanged = null;
			OnAutoAnswerRingCountChanged = null;
			OnCallProgressToneLevelChanged = null;
			OnDialingTimeoutChanged = null;
			OnDtmfOffTimeChanged = null;
			OnDtmfOnTimeChanged = null;
			OnLastNumberDialedChanged = null;
			OnLineReadyChanged = null;
			OnDtmfLocalMuteChanged = null;
			OnDtmfLocalLevelChanged = null;
			OnRedialEnabledChanged = null;
			OnRingTypeChanged = null;

			base.Dispose();

			// Unsubscribe
			RequestAttribute(LastNumberDialedFeedback, AttributeCode.eCommand.Unsubscribe, LAST_NUMBER_DIALED_ATTRIBUTE, null, Index);
			RequestAttribute(LineReadyFeedback, AttributeCode.eCommand.Unsubscribe, LINE_READY_ATTRIBUTE, null, Index);

			DisposeCallAppearances();
		}

		/// <summary>
		/// Override to request initial values from the device, and subscribe for feedback.
		/// </summary>
		public override void Initialize()
		{
			RebuildCallAppearances();

			base.Initialize();

			// Get initial values
			RequestAttribute(AutoAnswerFeedback, AttributeCode.eCommand.Get, AUTO_ANSWER_ATTRIBUTE, null, Index);
			RequestAttribute(AutoAnswerRingCountFeedback, AttributeCode.eCommand.Get, AUTO_ANSWER_RING_COUNT_ATTRIBUTE, null, Index);
			RequestAttribute(CallProgressToneLevelFeedback, AttributeCode.eCommand.Get, CALL_PROGRESS_TONE_LEVEL_ATTRIBUTE, null, Index);
			RequestAttribute(DialingTimeoutFeedback, AttributeCode.eCommand.Get, DIALING_TIMEOUT_ATTRIBUTE, null, Index);
			RequestAttribute(DtmfOffTimeFeedback, AttributeCode.eCommand.Get, DTMF_OFF_TIME_ATTRIBUTE, null, Index);
			RequestAttribute(DtmfOnTimeFeedback, AttributeCode.eCommand.Get, DTMF_ON_TIME_ATTRIBUTE, null, Index);
			RequestAttribute(LastNumberDialedFeedback, AttributeCode.eCommand.Get, LAST_NUMBER_DIALED_ATTRIBUTE, null, Index);
			RequestAttribute(LineReadyFeedback, AttributeCode.eCommand.Get, LINE_READY_ATTRIBUTE, null, Index);
			RequestAttribute(DtmfLocalMuteFeedback, AttributeCode.eCommand.Get, DTMF_LOCAL_MUTE_ATTRIBUTE, null, Index);
			RequestAttribute(DtmfLocalLevelFeedback, AttributeCode.eCommand.Get, DTMF_LOCAL_LEVEL_ATTRIBUTE, null, Index);
			RequestAttribute(RedialEnabledFeedback, AttributeCode.eCommand.Get, REDIAL_ENABLED_ATTRIBUTE, null, Index);
			RequestAttribute(RingTypeFeedback, AttributeCode.eCommand.Get, RING_TYPE_ATTRIBUTE, null, Index);

			// Subscribe
			RequestAttribute(LastNumberDialedFeedback, AttributeCode.eCommand.Subscribe, LAST_NUMBER_DIALED_ATTRIBUTE, null, Index);
			RequestAttribute(LineReadyFeedback, AttributeCode.eCommand.Subscribe, LINE_READY_ATTRIBUTE, null, Index);
		}

		/// <summary>
		/// Pulls call state info from the device response.
		/// </summary>
		/// <param name="callState"></param>
		internal void ParseCallState(ControlValue callState)
		{
			Value innerValue = (callState["value"] as Value);
			if (innerValue == null)
				return;

			int callId = innerValue.IntValue;
			VoIpControlStatusCallAppearance callAppearance = LazyLoadCallAppearance(callId + 1);
			callAppearance.ParseCallState(callState);
		}

		[PublicAPI]
		public VoIpControlStatusCallAppearance GetCallAppearance(int index)
		{
			return LazyLoadCallAppearance(index);
		}

		[PublicAPI]
		public IEnumerable<VoIpControlStatusCallAppearance> GetCallAppearances()
		{
			return m_CallAppearancesSection.Execute(() => m_CallAppearances.OrderValuesByKey().ToArray());
		}

		[PublicAPI]
		public void Dtmf(char key)
		{
			RequestService(DTMF_SERVICE, new Value(key), Index);
		}

		[PublicAPI]
		public void SetAutoAnswer(bool autoAnswer)
		{
			RequestAttribute(AutoAnswerFeedback, AttributeCode.eCommand.Toggle, AUTO_ANSWER_ATTRIBUTE, new Value(autoAnswer), Index);
		}

		[PublicAPI]
		public void ToggleAutoAnswer()
		{
			RequestAttribute(AutoAnswerFeedback, AttributeCode.eCommand.Toggle, AUTO_ANSWER_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void SetAutoAnswerRingCount(eAutoAnswerRingCount count)
		{
			Value value = Value.FromObject(count, s_AutoAnswerRingCountSerials);
			RequestAttribute(AutoAnswerRingCountFeedback, AttributeCode.eCommand.Set, AUTO_ANSWER_RING_COUNT_ATTRIBUTE, value, Index);
		}

		[PublicAPI]
		public void SetCallProgressToneLevel(float level)
		{
			RequestAttribute(CallProgressToneLevelFeedback, AttributeCode.eCommand.Set, DIALING_TIMEOUT_ATTRIBUTE, new Value(level), Index);
		}

		[PublicAPI]
		public void IncrementCallProgressToneLevel()
		{
			RequestAttribute(CallProgressToneLevelFeedback, AttributeCode.eCommand.Increment, DIALING_TIMEOUT_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void DecrementCallProgressTimeLevel()
		{
			RequestAttribute(CallProgressToneLevelFeedback, AttributeCode.eCommand.Decrement, DIALING_TIMEOUT_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void SetDialingTimeout(int seconds)
		{
			RequestAttribute(DialingTimeoutFeedback, AttributeCode.eCommand.Set, DIALING_TIMEOUT_ATTRIBUTE, new Value(seconds), Index);
		}

		[PublicAPI]
		public void IncrementDialingTimeout()
		{
			RequestAttribute(DialingTimeoutFeedback, AttributeCode.eCommand.Increment, DIALING_TIMEOUT_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void DecrementDialingTimeout()
		{
			RequestAttribute(DialingTimeoutFeedback, AttributeCode.eCommand.Decrement, DIALING_TIMEOUT_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void SetDtmfOnTime(int milliseconds)
		{
			RequestAttribute(DtmfOnTimeFeedback, AttributeCode.eCommand.Set, DTMF_ON_TIME_ATTRIBUTE, new Value(milliseconds), Index);
		}

		[PublicAPI]
		public void IncrementDtmfOnTime()
		{
			RequestAttribute(DtmfOnTimeFeedback, AttributeCode.eCommand.Increment, DTMF_ON_TIME_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void DecrementDtmfOnTime()
		{
			RequestAttribute(DtmfOnTimeFeedback, AttributeCode.eCommand.Decrement, DTMF_ON_TIME_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void SetDtmfOffTime(int milliseconds)
		{
			RequestAttribute(DtmfOffTimeFeedback, AttributeCode.eCommand.Set, DTMF_OFF_TIME_ATTRIBUTE, new Value(milliseconds), Index);
		}

		[PublicAPI]
		public void IncrementDtmfOffTime()
		{
			RequestAttribute(DtmfOffTimeFeedback, AttributeCode.eCommand.Increment, DTMF_OFF_TIME_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void DecrementDtmfOffTime()
		{
			RequestAttribute(DtmfOffTimeFeedback, AttributeCode.eCommand.Decrement, DTMF_OFF_TIME_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void SetDtmfLocalMute(bool mute)
		{
			RequestAttribute(DtmfLocalMuteFeedback, AttributeCode.eCommand.Set, DTMF_LOCAL_MUTE_ATTRIBUTE, new Value(mute), Index);
		}

		[PublicAPI]
		public void ToggleDtmfLocalMute()
		{
			RequestAttribute(DtmfLocalMuteFeedback, AttributeCode.eCommand.Toggle, DTMF_LOCAL_MUTE_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void SetDtmfLocalLevel(float level)
		{
			RequestAttribute(DtmfLocalLevelFeedback, AttributeCode.eCommand.Set, DTMF_LOCAL_LEVEL_ATTRIBUTE, new Value(level), Index);
		}

		[PublicAPI]
		public void IncrementDtmfLocalLevel()
		{
			RequestAttribute(DtmfLocalLevelFeedback, AttributeCode.eCommand.Increment, DTMF_LOCAL_LEVEL_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void DecrementDtmfLocalLevel()
		{
			RequestAttribute(DtmfLocalLevelFeedback, AttributeCode.eCommand.Decrement, DTMF_LOCAL_LEVEL_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void SetRedialEnabled(bool enabled)
		{
			RequestAttribute(RedialEnabledFeedback, AttributeCode.eCommand.Set, REDIAL_ENABLED_ATTRIBUTE, new Value(enabled), Index);
		}

		[PublicAPI]
		public void ToggleRedialEnabled()
		{
			RequestAttribute(RedialEnabledFeedback, AttributeCode.eCommand.Toggle, REDIAL_ENABLED_ATTRIBUTE, null, Index);
		}

		[PublicAPI]
		public void SetRingType(eRingType ringType)
		{
			Value value = Value.FromObject(ringType, s_RingTypeSerials);
			RequestAttribute(RingTypeFeedback, AttributeCode.eCommand.Set, RING_TYPE_ATTRIBUTE, value, Index);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Disposes the existing call appearances and rebuilds from call appearance count.
		/// </summary>
		private void RebuildCallAppearances()
		{
			m_CallAppearancesSection.Enter();

			try
			{
				Enumerable.Range(1, CallAppearanceCount).ForEach(i => LazyLoadCallAppearance(i));
			}
			finally
			{
				m_CallAppearancesSection.Leave();
			}
		}

		private VoIpControlStatusCallAppearance LazyLoadCallAppearance(int index)
		{
			m_CallAppearancesSection.Enter();

			try
			{
				if (!m_CallAppearances.ContainsKey(index))
					m_CallAppearances[index] = new VoIpControlStatusCallAppearance(this, index);
				return m_CallAppearances[index];
			}
			finally
			{
				m_CallAppearancesSection.Leave();
			}
		}

		/// <summary>
		/// Disposes the existing call appearances.
		/// </summary>
		private void DisposeCallAppearances()
		{
			m_CallAppearancesSection.Enter();

			try
			{
				m_CallAppearances.Values.ForEach(c => c.Dispose());
				m_CallAppearances.Clear();
			}
			finally
			{
				m_CallAppearancesSection.Leave();
			}
		}

		#endregion

		#region Subscription Callbacks

		private void AutoAnswerFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				AutoAnswer = innerValue.BoolValue;
		}

		private void AutoAnswerRingCountFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				AutoAnswerRingCount = innerValue.GetObjectValue(s_AutoAnswerRingCountSerials);
		}

		private void CallProgressToneLevelFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				CallProgressToneLevel = innerValue.FloatValue;
		}

		private void DialingTimeoutFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				DialingTimeout = innerValue.IntValue;
		}

		private void DtmfOffTimeFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				DtmfOffTime = innerValue.IntValue;
		}

		private void DtmfOnTimeFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				DtmfOnTime = innerValue.IntValue;
		}

		private void DtmfLocalMuteFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				DtmfLocalMute = innerValue.BoolValue;
		}

		private void DtmfLocalLevelFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				DtmfLocalLevel = innerValue.FloatValue;
		}

		private void RedialEnabledFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				RedialEnabled = innerValue.BoolValue;
		}

		private void RingTypeFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				RingType = innerValue.GetObjectValue(s_RingTypeSerials);
		}

		private void LineReadyFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				LineReady = innerValue.BoolValue;
		}

		private void LastNumberDialedFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				LastNumberDialed = innerValue.StringValue;
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Call Appearance Count", CallAppearanceCount);
			addRow("Auto Answer", AutoAnswer);
			addRow("Auto Answer Ring Count", AutoAnswerRingCount);
			addRow("Call Progress Tone Level", CallProgressToneLevel);
			addRow("Dialing Timeout", DialingTimeout);
			addRow("DTMF Off-Time", DtmfOffTime);
			addRow("DTMF On-Time", DtmfOnTime);
			addRow("Last Number Dialed", LastNumberDialed);
			addRow("Line Ready", LineReady);
			addRow("DTMF Local Mute", DtmfLocalMute);
			addRow("DTMF Local Level", DtmfLocalLevel);
			addRow("Redial Enabled", RedialEnabled);
			addRow("Ring Type", RingType);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<char>("Dtmf", "Dtmf <KEY>", c => Dtmf(c));

			yield return new GenericConsoleCommand<bool>("SetAutoAnswer", "SetAutoAnswer <true/false>", b => SetAutoAnswer(b));
			yield return new ConsoleCommand("ToggleAutoAnswer", "", () => ToggleAutoAnswer());

			string setAutoAnswerRingCountHelp =
				string.Format("SetAutoAnswerRingCount <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eAutoAnswerRingCount>()));
			yield return
				new GenericConsoleCommand<eAutoAnswerRingCount>("SetAutoAnswerRingCount", setAutoAnswerRingCountHelp,
				                                                e => SetAutoAnswerRingCount(e));

			yield return new GenericConsoleCommand<float>("SetCallProgressToneLevel", "SetCallProgressToneLevel <LEVEL>", f => SetCallProgressToneLevel(f));
			yield return new ConsoleCommand("IncrementCallProgressToneLevel", "", () => IncrementCallProgressToneLevel());
			yield return new ConsoleCommand("DecrementCallProgressTimeLevel", "", () => DecrementCallProgressTimeLevel());

			yield return new GenericConsoleCommand<int>("SetDialingTimeout", "SetDialingTimeout <SECONDS>", i => SetDialingTimeout(i));
			yield return new ConsoleCommand("IncrementDialingTimeout", "", () => IncrementDialingTimeout());
			yield return new ConsoleCommand("DecrementDialingTimeout", "", () => DecrementDialingTimeout());

			yield return new GenericConsoleCommand<int>("SetDtmfOnTime", "SetDtmfOnTime <MILLISECONDS>", i => SetDtmfOnTime(i));
			yield return new ConsoleCommand("IncrementDtmfOnTime", "", () => IncrementDtmfOnTime());
			yield return new ConsoleCommand("DecrementDtmfOnTime", "", () => DecrementDtmfOnTime());

			yield return new GenericConsoleCommand<int>("SetDtmfOffTime", "SetDtmfOffTime <MILLISECONDS>", i => SetDtmfOffTime(i));
			yield return new ConsoleCommand("IncrementDtmfOffTime", "", () => IncrementDtmfOffTime());
			yield return new ConsoleCommand("DecrementDtmfOffTime", "", () => DecrementDtmfOffTime());

			yield return new GenericConsoleCommand<bool>("SetDtmfLocalMute", "SetDtmfLocalMute <true/false>", b => SetDtmfLocalMute(b));
			yield return new ConsoleCommand("ToggleDtmfLocalMute", "", () => ToggleDtmfLocalMute());

			yield return new GenericConsoleCommand<float>("SetDtmfLocalLevel", "SetDtmfLocalLevel <LEVEL>", f => SetDtmfLocalLevel(f));
			yield return new ConsoleCommand("IncrementDtmfLocalLevel", "", () => IncrementDtmfLocalLevel());
			yield return new ConsoleCommand("DecrementDtmfLocalLevel", "", () => DecrementDtmfLocalLevel());

			yield return new GenericConsoleCommand<bool>("SetRedialEnabled", "SetRedialEnabled <true/false>", b => SetRedialEnabled(b));
			yield return new ConsoleCommand("ToggleRedialEnabled", "", () => ToggleRedialEnabled());

			string setRingTypeHelp =
				string.Format("SetRingType <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eRingType>()));
			yield return new GenericConsoleCommand<eRingType>("SetRingType", setRingTypeHelp, e => SetRingType(e));
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			yield return ConsoleNodeGroup.KeyNodeMap("CallAppearances", GetCallAppearances(), c => (uint)c.Index);
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		#endregion
	}
}
