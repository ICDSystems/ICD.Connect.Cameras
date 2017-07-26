using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.UI;
using ICD.SimplSharp.Common.UiPro.Settings;
using ICD.SimplSharp.Common.UiPro.TriListAdapters;

namespace ICD.SimplSharp.Common.UiPro
{
	/// <summary>
	/// Tsw750Adapter wraps a Tsw750 for use with the UIPro library.
	/// </summary>
	public sealed class Tsw750Adapter : AbstractTriListAdapter<Tsw750, Tsw750AdapterSettings>
	{
		/// <summary>
		/// Creates an instance of the wrapped trilist.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="controlSystem"></param>
		/// <returns></returns>
		protected override Tsw750 InstantiateTriList(byte ipid, CrestronControlSystem controlSystem)
		{
			return new Tsw750(ipid, controlSystem);
		}
	}
}
