using ICD.Common.EventArguments;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.EventArguments
{
	public sealed class SigAdapterEventArgs : GenericEventArgs<ISig>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sig"></param>
		public SigAdapterEventArgs(ISig sig)
			: base(sig)
		{
		}
	}
}
