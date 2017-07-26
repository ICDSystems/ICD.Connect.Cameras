using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.UI;
using ICD.SimplSharp.Common.UiPro.Settings;
using ICD.SimplSharp.Common.UiPro.TriListAdapters;

namespace ICD.SimplSharp.Common.UiPro
{
	public sealed class Tsw1060Adapter : AbstractTriListAdapter<Tsw1060, Tsw1060AdapterSettings>
	{
		/// <summary>
		/// Creates an instance of the wrapped trilist.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="controlSystem"></param>
		/// <returns></returns>
		protected override Tsw1060 InstantiateTriList(byte ipid, CrestronControlSystem controlSystem)
		{
			return new Tsw1060(ipid, controlSystem);
		}
	}
}