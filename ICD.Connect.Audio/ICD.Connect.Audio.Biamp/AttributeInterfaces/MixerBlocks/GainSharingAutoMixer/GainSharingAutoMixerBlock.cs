using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Audio.Biamp.Controls;
using ICD.Connect.Audio.Biamp.TesiraTextProtocol.Codes;
using ICD.Connect.Audio.Biamp.TesiraTextProtocol.Parsing;

namespace ICD.Connect.Audio.Biamp.AttributeInterfaces.MixerBlocks.GainSharingAutoMixer
{
	public sealed class GainSharingAutoMixerBlock : AbstractMixerBlock, IVolumeAttributeInterface
	{
		private const string GAIN_RESPONSE_TIME_ATTRIBUTE = "gainResponseTimeMs";
		private const string MIC_ISOLATION_FACTOR_ATTRIBUTE = "micIsolationFactor";
		private const string MIX_OUTPUT_LABEL_ATTRIBUTE = "mixOutputLabel";
		private const string INPUT_COUNT_ATTRIBUTE = "numInputs";
		private const string OUTPUT_LEVEL_ATTRIBUTE = "outputLevel";
		private const string OUTPUT_MIN_LEVEL_ATTRIBUTE = "outputMinLevel";
		private const string OUTPUT_MAX_LEVEL_ATTRIBUTE = "outputMaxLevel";
		private const string OUTPUT_MUTE_ATTRIBUTE = "outputMute";

		[PublicAPI]
		public event EventHandler<IntEventArgs> OnGainResponseTimeChanged;

		[PublicAPI]
		public event EventHandler<FloatEventArgs> OnMicIsolationFactorChanged;

		[PublicAPI]
		public event EventHandler<StringEventArgs> OnMixOutputLabelChanged; 

		/// <summary>
		/// Raised when the system informs of an input count change.
		/// </summary>
		[PublicAPI]
		public event EventHandler<IntEventArgs> OnInputCountChanged;

		[PublicAPI]
		public event EventHandler<FloatEventArgs> OnLevelChanged;

		[PublicAPI]
		public event EventHandler<FloatEventArgs> OnMinLevelChanged;

		[PublicAPI]
		public event EventHandler<FloatEventArgs> OnMaxLevelChanged;

		[PublicAPI]
		public event EventHandler<BoolEventArgs> OnMuteChanged; 

		private readonly Dictionary<int, GainSharingAutoMixerChannel> m_Channels;
		private readonly SafeCriticalSection m_ChannelsSection;

		private int m_GainResponseTime;
		private float m_MicIsolationFactor;
		private string m_MixOutputLabel;
		private int m_InputCount;
		private float m_Level;
		private float m_MinLevel;
		private float m_MaxLevel;
		private bool m_Mute;

		#region Properties

		[PublicAPI]
		public int GainResponseTime
		{
			get { return m_GainResponseTime; }
			private set
			{
				if (value == m_GainResponseTime)
					return;

				m_GainResponseTime = value;

				OnGainResponseTimeChanged.Raise(this, new IntEventArgs(m_GainResponseTime));
			}
		}

		[PublicAPI]
		public float MicIsolationFactor
		{
			get { return m_MicIsolationFactor; }
			private set
			{
				if (value == m_MicIsolationFactor)
					return;

				m_MicIsolationFactor = value;

				OnMicIsolationFactorChanged.Raise(this, new FloatEventArgs(m_MicIsolationFactor));
			}
		}

		[PublicAPI]
		public string MixOutputLabel
		{
			get { return m_MixOutputLabel; }
			private set
			{
				if (value == m_MixOutputLabel)
					return;

				m_MixOutputLabel = value;

				OnMixOutputLabelChanged.Raise(this, new StringEventArgs(m_MixOutputLabel));
			}
		}

		/// <summary>
		/// Gets the number of inputs.
		/// </summary>
		[PublicAPI]
		public int InputCount
		{
			get { return m_InputCount; }
			private set
			{
				if (value == m_InputCount)
					return;

				m_InputCount = value;

				RebuildInputs();

				OnInputCountChanged.Raise(this, new IntEventArgs(m_InputCount));
			}
		}

		[PublicAPI]
		public float Level
		{
			get { return m_Level; }
			private set
			{
				if (value == m_Level)
					return;

				m_Level = value;

				OnLevelChanged.Raise(this, new FloatEventArgs(m_Level));
			}
		}

		[PublicAPI]
		public float MinLevel
		{
			get { return m_MinLevel; }
			private set
			{
				if (value == m_MinLevel)
					return;

				m_MinLevel = value;

				OnMinLevelChanged.Raise(this, new FloatEventArgs(m_MinLevel));
			}
		}

		[PublicAPI]
		public float MaxLevel
		{
			get { return m_MaxLevel; }
			private set
			{
				if (value == m_MaxLevel)
					return;

				m_MaxLevel = value;

				OnMaxLevelChanged.Raise(this, new FloatEventArgs(m_MaxLevel));
			}
		}

		[PublicAPI]
		public bool Mute
		{
			get { return m_Mute; }
			private set
			{
				if (value == m_Mute)
					return;

				m_Mute = value;

				OnMuteChanged.Raise(this, new BoolEventArgs(m_Mute));
			}
		}

		public float AttributeMinLevel { get { return -100.0f; } }

		public float AttributeMaxLevel { get { return 12.0f; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="instanceTag"></param>
		public GainSharingAutoMixerBlock(BiampTesiraDevice device, string instanceTag)
			: base(device, instanceTag)
		{
			m_Channels = new Dictionary<int, GainSharingAutoMixerChannel>();
			m_ChannelsSection = new SafeCriticalSection();

			if (device.Initialized)
				Initialize();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnGainResponseTimeChanged = null;
			OnMicIsolationFactorChanged = null;
			OnMixOutputLabelChanged = null;
			OnInputCountChanged = null;
			OnLevelChanged = null;
			OnMinLevelChanged = null;
			OnMaxLevelChanged = null;
			OnMuteChanged = null;

			base.Dispose();

			// Unsubscribe
			RequestAttribute(LevelFeedback, AttributeCode.eCommand.Unsubscribe, OUTPUT_LEVEL_ATTRIBUTE, null);
			RequestAttribute(MuteFeedback, AttributeCode.eCommand.Unsubscribe, OUTPUT_MUTE_ATTRIBUTE, null);

			DisposeInputs();
		}

		/// <summary>
		/// Override to request initial values from the device, and subscribe for feedback.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			// Get initial values
			RequestAttribute(GainResponseTimeFeedback, AttributeCode.eCommand.Get, GAIN_RESPONSE_TIME_ATTRIBUTE, null);
			RequestAttribute(MicIsolationFactorFeedback, AttributeCode.eCommand.Get, MIC_ISOLATION_FACTOR_ATTRIBUTE, null);
			RequestAttribute(MixOutputLabelFeedback, AttributeCode.eCommand.Get, MIX_OUTPUT_LABEL_ATTRIBUTE, null);
			RequestAttribute(InputCountFeedback, AttributeCode.eCommand.Get, INPUT_COUNT_ATTRIBUTE, null);
			RequestAttribute(LevelFeedback, AttributeCode.eCommand.Get, OUTPUT_LEVEL_ATTRIBUTE, null);
			RequestAttribute(OutputMinLevelFeedback, AttributeCode.eCommand.Get, OUTPUT_MIN_LEVEL_ATTRIBUTE, null);
			RequestAttribute(OutputMaxLevelFeedback, AttributeCode.eCommand.Get, OUTPUT_MAX_LEVEL_ATTRIBUTE, null);
			RequestAttribute(MuteFeedback, AttributeCode.eCommand.Get, OUTPUT_MUTE_ATTRIBUTE, null);

			// Subscribe
			RequestAttribute(LevelFeedback, AttributeCode.eCommand.Subscribe, OUTPUT_LEVEL_ATTRIBUTE, null);
			RequestAttribute(MuteFeedback, AttributeCode.eCommand.Subscribe, OUTPUT_MUTE_ATTRIBUTE, null);
		}

		/// <summary>
		/// Gets the inputs for this mixer block.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public IEnumerable<GainSharingAutoMixerChannel> GetInputs()
		{
			return m_ChannelsSection.Execute(() => m_Channels.OrderValuesByKey().ToArray());
		}

		/// <summary>
		/// Gets the child attribute interface at the given path.
		/// </summary>
		/// <param name="channelType"></param>
		/// <param name="indices"></param>
		/// <returns></returns>
		public override IAttributeInterface GetAttributeInterface(eChannelType channelType, params int[] indices)
		{
			switch (channelType)
			{
				case eChannelType.Input:
					if (indices.Length != 1)
						throw new ArgumentOutOfRangeException("indices");
					return LazyLoadChannel(indices[0]);

				default:
					return base.GetAttributeInterface(channelType, indices);
			}
		}

		[PublicAPI]
		public void SetGainResponseTime(int milliseconds)
		{
			RequestAttribute(GainResponseTimeFeedback, AttributeCode.eCommand.Set, GAIN_RESPONSE_TIME_ATTRIBUTE, new Value(milliseconds));
		}

		[PublicAPI]
		public void IncrementGainResponseTime()
		{
			RequestAttribute(GainResponseTimeFeedback, AttributeCode.eCommand.Increment, GAIN_RESPONSE_TIME_ATTRIBUTE, null);
		}

		[PublicAPI]
		public void DecrementGainResponseTime()
		{
			RequestAttribute(GainResponseTimeFeedback, AttributeCode.eCommand.Decrement, GAIN_RESPONSE_TIME_ATTRIBUTE, null);
		}

		[PublicAPI]
		public void SetMicIsolationFactor(float factor)
		{
			RequestAttribute(MicIsolationFactorFeedback, AttributeCode.eCommand.Set, MIC_ISOLATION_FACTOR_ATTRIBUTE, new Value(factor));
		}

		[PublicAPI]
		public void IncrementMicIsolationFactor()
		{
			RequestAttribute(MicIsolationFactorFeedback, AttributeCode.eCommand.Increment, MIC_ISOLATION_FACTOR_ATTRIBUTE, null);
		}

		[PublicAPI]
		public void DecrementMicIsolationFactor()
		{
			RequestAttribute(MicIsolationFactorFeedback, AttributeCode.eCommand.Decrement, MIC_ISOLATION_FACTOR_ATTRIBUTE, null);
		}

		[PublicAPI]
		public void SetMixOutputLabel(string label)
		{
			RequestAttribute(MixOutputLabelFeedback, AttributeCode.eCommand.Set, MIX_OUTPUT_LABEL_ATTRIBUTE, new Value(label));
		}

		[PublicAPI]
		public void SetLevel(float level)
		{
			RequestAttribute(LevelFeedback, AttributeCode.eCommand.Set, OUTPUT_LEVEL_ATTRIBUTE, new Value(level));
		}

		[PublicAPI]
		public void IncrementLevel()
		{
			RequestAttribute(LevelFeedback, AttributeCode.eCommand.Increment, OUTPUT_LEVEL_ATTRIBUTE, null);
		}

		[PublicAPI]
		public void DecrementLevel()
		{
			RequestAttribute(LevelFeedback, AttributeCode.eCommand.Decrement, OUTPUT_LEVEL_ATTRIBUTE, null);
		}

		[PublicAPI]
		public void SetMinLevel(float level)
		{
			RequestAttribute(OutputMinLevelFeedback, AttributeCode.eCommand.Set, OUTPUT_MIN_LEVEL_ATTRIBUTE, new Value(level));
		}

		[PublicAPI]
		public void IncrementMinLevel()
		{
			RequestAttribute(OutputMinLevelFeedback, AttributeCode.eCommand.Increment, OUTPUT_MIN_LEVEL_ATTRIBUTE, null);
		}

		[PublicAPI]
		public void DecrementMinLevel()
		{
			RequestAttribute(OutputMinLevelFeedback, AttributeCode.eCommand.Decrement, OUTPUT_MIN_LEVEL_ATTRIBUTE, null);
		}

		[PublicAPI]
		public void SetMaxLevel(float level)
		{
			RequestAttribute(OutputMaxLevelFeedback, AttributeCode.eCommand.Set, OUTPUT_MAX_LEVEL_ATTRIBUTE, new Value(level));
		}

		[PublicAPI]
		public void IncrementMaxLevel()
		{
			RequestAttribute(OutputMaxLevelFeedback, AttributeCode.eCommand.Increment, OUTPUT_MAX_LEVEL_ATTRIBUTE, null);
		}

		[PublicAPI]
		public void DecrementMaxLevel()
		{
			RequestAttribute(OutputMaxLevelFeedback, AttributeCode.eCommand.Decrement, OUTPUT_MAX_LEVEL_ATTRIBUTE, null);
		}

		[PublicAPI]
		public void SetMute(bool mute)
		{
			RequestAttribute(MuteFeedback, AttributeCode.eCommand.Set, OUTPUT_MUTE_ATTRIBUTE, new Value(mute));
		}

		[PublicAPI]
		public void ToggleMute()
		{
			RequestAttribute(MuteFeedback, AttributeCode.eCommand.Toggle, OUTPUT_MUTE_ATTRIBUTE, null);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Disposes the existing inputs and rebuilds from input count.
		/// </summary>
		private void RebuildInputs()
		{
			m_ChannelsSection.Enter();

			try
			{
				DisposeInputs();
				Enumerable.Range(1, InputCount).ForEach(i => LazyLoadChannel(i));
			}
			finally
			{
				m_ChannelsSection.Leave();
			}
		}

		/// <summary>
		/// Gets the channel at the given index. If the channel doesn't exist, creates a new one.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private GainSharingAutoMixerChannel LazyLoadChannel(int index)
		{
			m_ChannelsSection.Enter();

			try
			{
				if (!m_Channels.ContainsKey(index))
					m_Channels[index] = new GainSharingAutoMixerChannel(this, index);
				return m_Channels[index];
			}
			finally
			{
				m_ChannelsSection.Leave();
			}
		}

		/// <summary>
		/// Disposes the existing inputs.
		/// </summary>
		private void DisposeInputs()
		{
			m_ChannelsSection.Enter();

			try
			{
				m_Channels.Values.ForEach(c => c.Dispose());
				m_Channels.Clear();
			}
			finally
			{
				m_ChannelsSection.Leave();
			}
		}

		#endregion

		#region Subscription Callbacks

		private void GainResponseTimeFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				GainResponseTime = innerValue.IntValue;
		}

		private void MicIsolationFactorFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				MicIsolationFactor = innerValue.FloatValue;
		}

		private void MixOutputLabelFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				MixOutputLabel = innerValue.StringValue;
		}

		private void InputCountFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				InputCount = innerValue.IntValue;
		}

		private void OutputMinLevelFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				MinLevel = innerValue.FloatValue;
		}

		private void OutputMaxLevelFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				MaxLevel = innerValue.FloatValue;
		}

		private void LevelFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				Level = innerValue.FloatValue;
		}

		private void MuteFeedback(BiampTesiraDevice sender, ControlValue value)
		{
			Value innerValue = value["value"] as Value;
			if (innerValue != null)
				Mute = innerValue.BoolValue;
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

			addRow("Gain Response Time", GainResponseTime);
			addRow("Mic Isolation Factor", MicIsolationFactor);
			addRow("Mix Output Label", MixOutputLabel);
			addRow("Input Count", InputCount);
			addRow("Output Level", Level);
			addRow("Min Output Level", MinLevel);
			addRow("Max Output Level", MaxLevel);
			addRow("Output Mute", Mute);
		}

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			yield return ConsoleNodeGroup.KeyNodeMap("Inputs", GetInputs(), i => (uint)i.Index);
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<int>("SetGainResponseTime", "SetGainResponseTime <MILLISECONDS>", i => SetGainResponseTime(i));
			yield return new ConsoleCommand("IncrementGainResponseTime", "", () => IncrementGainResponseTime());
			yield return new ConsoleCommand("DecrementGainResponseTime", "", () => DecrementGainResponseTime());

			yield return new GenericConsoleCommand<float>("SetMicIsolationFactor", "SetMicIsolationFactor <FACTOR>", f => SetMicIsolationFactor(f));
			yield return new ConsoleCommand("IncrementMicIsolationFactor", "", () => IncrementMicIsolationFactor());
			yield return new ConsoleCommand("DecrementMicIsolationFactor", "", () => DecrementMicIsolationFactor());

			yield return new GenericConsoleCommand<string>("SetMixOutputLabel", "SetMixOutputLabel <LABEL>", s => SetMixOutputLabel(s));

			yield return new GenericConsoleCommand<float>("SetLevel", "SetLevel <LEVEL>", f => SetLevel(f));
			yield return new ConsoleCommand("IncrementLevel", "", () => IncrementLevel());
			yield return new ConsoleCommand("DecrementLevel", "", () => DecrementLevel());

			yield return new GenericConsoleCommand<float>("SetMinLevel", "SetMinLevel <LEVEL>", f => SetMinLevel(f));
			yield return new ConsoleCommand("IncrementMinLevel", "", () => IncrementMinLevel());
			yield return new ConsoleCommand("DecrementMinLevel", "", () => DecrementMinLevel());

			yield return new GenericConsoleCommand<float>("SetMaxLevel", "SetMaxLevel <LEVEL>", f => SetMaxLevel(f));
			yield return new ConsoleCommand("IncrementMaxLevel", "", () => IncrementMaxLevel());
			yield return new ConsoleCommand("DecrementMaxLevel", "", () => DecrementMaxLevel());

			yield return new GenericConsoleCommand<bool>("SetMute", "SetMute <true/false>", b => SetMute(b));
			yield return new ConsoleCommand("ToggleMute", "", () => ToggleMute());
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
