using ICD.Common.Properties;
using ICD.Connect.Panels.SigCollections;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.Mock
{
	[PublicAPI]
	public sealed class MockStringInputCollection :
		AbstractMockSigCollection<IStringInputSig>, IDeviceStringInputCollection
	{
	}

	[PublicAPI]
	public sealed class MockStringOutputCollection :
		AbstractMockSigCollection<IStringOutputSig>, IDeviceStringOutputCollection
	{
	}

	[PublicAPI]
	public sealed class MockUShortInputCollection :
		AbstractMockSigCollection<IUShortInputSig>, IDeviceUShortInputCollection
	{
	}

	[PublicAPI]
	public sealed class MockUShortOutputCollection :
		AbstractMockSigCollection<IUShortOutputSig>, IDeviceUShortOutputCollection
	{
	}

	[PublicAPI]
	public sealed class MockBooleanInputCollection :
		AbstractMockSigCollection<IBoolInputSig>, IDeviceBooleanInputCollection
	{
	}

	[PublicAPI]
	public sealed class MockBooleanOutputCollection :
		AbstractMockSigCollection<IBoolOutputSig>, IDeviceBooleanOutputCollection
	{
	}
}
