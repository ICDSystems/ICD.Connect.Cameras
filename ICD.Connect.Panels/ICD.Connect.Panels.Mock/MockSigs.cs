using ICD.Common.Properties;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.Mock
{
	[PublicAPI]
	public sealed class MockBoolInputSig : AbstractMockSig, IBoolInputSig
	{
		/// <summary>
		/// Type of data this sig uses when communicating with the device.
		/// 
		/// </summary>
		public override eSigType Type { get { return eSigType.Digital; } }
	}

	[PublicAPI]
	public sealed class MockBoolOutputSig : AbstractMockSig, IBoolOutputSig
	{
		/// <summary>
		/// Type of data this sig uses when communicating with the device.
		/// 
		/// </summary>
		public override eSigType Type { get { return eSigType.Digital; } }
	}

	[PublicAPI]
	public sealed class MockStringInputSig : AbstractMockSig, IStringInputSig
	{
		/// <summary>
		/// Type of data this sig uses when communicating with the device.
		/// 
		/// </summary>
		public override eSigType Type { get { return eSigType.Serial; } }
	}

	[PublicAPI]
	public sealed class MockStringOutputSig : AbstractMockSig, IStringOutputSig
	{
		/// <summary>
		/// Type of data this sig uses when communicating with the device.
		/// 
		/// </summary>
		public override eSigType Type { get { return eSigType.Serial; } }
	}

	[PublicAPI]
	public sealed class MockUShortInputSig : AbstractMockSig, IUShortInputSig
	{
		/// <summary>
		/// Type of data this sig uses when communicating with the device.
		/// 
		/// </summary>
		public override eSigType Type { get { return eSigType.Analog; } }
	}

	[PublicAPI]
	public sealed class MockUShortOutputSig : AbstractMockSig, IUShortOutputSig
	{
		/// <summary>
		/// Type of data this sig uses when communicating with the device.
		/// 
		/// </summary>
		public override eSigType Type { get { return eSigType.Analog; } }
	}
}
