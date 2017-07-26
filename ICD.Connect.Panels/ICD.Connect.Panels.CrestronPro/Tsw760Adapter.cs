using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.UI;
using ICD.SimplSharp.Common.UiPro.Settings;
using ICD.SimplSharp.Common.UiPro.TriListAdapters;

namespace ICD.SimplSharp.Common.UiPro
{
	public sealed class Tsw760Adapter : AbstractTriListAdapter<Tsw760, Tsw760AdapterSettings>
	{
		/// <summary>
		/// Creates an instance of the wrapped trilist.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="controlSystem"></param>
		/// <returns></returns>
		protected override Tsw760 InstantiateTriList(byte ipid, CrestronControlSystem controlSystem)
		{
			return new Tsw760(ipid, controlSystem);
		}
	}
}
