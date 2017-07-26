using ICD.Common.Properties;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.PanelClients;

namespace ICD.Connect.UI.ClientControls.Buttons
{
	[PublicAPI]
	public sealed class ClientButton : AbstractClientButton
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ClientButton(ISigInputOutputClient client)
			: base(client)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		public ClientButton(ISigInputOutputClient client, IJoinOffsets parent)
			: base(client, parent)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ClientButton(ISigInputOutputClient client, IJoinOffsets parent, ushort index)
			: base(client, parent, index)
		{
		}
	}
}
