using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.UI;
using ICD.SimplSharp.Common.UiPro.Settings;
using ICD.SimplSharp.Common.UiPro.TriListAdapters;

namespace ICD.SimplSharp.Common.UiPro
{
	public sealed class Tsw1052Adapter : AbstractTriListAdapter<Tsw1052, Tsw1052AdapterSettings>
	{
		/// <summary>
		/// Creates an instance of the wrapped trilist.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="controlSystem"></param>
		/// <returns></returns>
		protected override Tsw1052 InstantiateTriList(byte ipid, CrestronControlSystem controlSystem)
		{
			return new Tsw1052(ipid, controlSystem);
		}
	}
}
