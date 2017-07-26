using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.SigCollections
{
	public interface IDeviceBooleanInputCollection : ISigCollectionBase<IBoolInputSig>
	{
	}

	public interface IDeviceBooleanOutputCollection : ISigCollectionBase<IBoolOutputSig>
	{
	}

	public interface IDeviceStringInputCollection : ISigCollectionBase<IStringInputSig>
	{
	}

	public interface IDeviceStringOutputCollection : ISigCollectionBase<IStringOutputSig>
	{
	}

	public interface IDeviceUShortInputCollection : ISigCollectionBase<IUShortInputSig>
	{
	}

	public interface IDeviceUShortOutputCollection : ISigCollectionBase<IUShortOutputSig>
	{
	}
}
