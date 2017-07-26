using ICD.Connect.UI.Controls;
using ICD.Connect.UI.PanelClients;

namespace ICD.Connect.UI.ClientControls.TextControls
{
	public sealed class ClientFormattedText : AbstractClientLabel
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ClientFormattedText(ISigInputOutputClient client)
			: base(client)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		public ClientFormattedText(ISigInputOutputClient client, IJoinOffsets parent)
			: base(client, parent)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ClientFormattedText(ISigInputOutputClient client, IJoinOffsets parent, ushort index)
			: base(client, parent, index)
		{
		}
	}
}
