using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Connect.Panels;

namespace ICD.Connect.UI.Controls.Lists
{
	/// <summary>
	/// Base class for button lists.
	/// </summary>
	public abstract class AbstractVtProButtonList : AbstractVtProList
	{
		private readonly SafeCriticalSection m_SetLabelsSection;
		private readonly SafeCriticalSection m_SetIconsSection;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		protected AbstractVtProButtonList(uint smartObjectId, IPanelDevice panel)
			: this(smartObjectId, panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		protected AbstractVtProButtonList(uint smartObjectId, IPanelDevice panel, IVtProParent parent)
			: base(smartObjectId, panel, parent)
		{
			m_SetLabelsSection = new SafeCriticalSection();
			m_SetIconsSection = new SafeCriticalSection();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sets the selected state for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		[PublicAPI]
		public abstract void SetItemSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the button labels.
		/// </summary>
		/// <param name="labels"></param>
		public void SetItemLabels(string[] labels)
		{
			m_SetLabelsSection.Enter();

			try
			{
				SetNumberOfItems((ushort)labels.Length);

				for (ushort index = 0; index < labels.Length; index++)
					SetItemLabel(index, labels[index]);
			}
			finally
			{
				m_SetLabelsSection.Leave();
			}
		}

		/// <summary>
		/// Sets the button label.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="text"></param>
		public abstract void SetItemLabel(ushort index, string text);

		/// <summary>
		/// Sets the button icons.
		/// </summary>
		/// <param name="icons"></param>
		[PublicAPI]
		public void SetItemIcons(string[] icons)
		{
			m_SetIconsSection.Enter();

			try
			{
				SetNumberOfItems((ushort)icons.Length);

				for (ushort index = 0; index < icons.Length; index++)
					SetItemIcon(index, icons[index]);
			}
			finally
			{
				m_SetIconsSection.Leave();
			}
		}

		/// <summary>
		/// Sets the button icon.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		public abstract void SetItemIcon(ushort index, string icon);

		#endregion
	}
}
