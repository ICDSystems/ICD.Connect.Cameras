using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Lights
{
	public sealed partial class LightsView : AbstractView, ILightsView
	{
		public event EventHandler<BoolEventArgs> OnLightListScrolling;
		public event EventHandler<BoolEventArgs> OnShadeListScrolling;
		public event EventHandler<BoolEventArgs> OnPresetsListScrolling;
		public event EventHandler<UShortEventArgs> OnPresetButtonPressed;

		private readonly List<ILightComponentView> m_ChildLights;
		private readonly List<IShadeComponentView> m_ChildShades;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public LightsView(ISigInputOutput panel)
			: base(panel)
		{
			m_ChildLights = new List<ILightComponentView>();
			m_ChildShades = new List<IShadeComponentView>();
		}

		#region Methods

		/// <summary>
		/// Sets the labels for the light preset buttons.
		/// </summary>
		/// <param name="labels"></param>
		public void SetLightPresetsLabels(IEnumerable<string> labels)
		{
			string[] namesArray = labels.Take(m_LightPresetsButtonList.MaxSize).ToArray();

			m_LightPresetsButtonList.SetNumberOfItems((ushort)namesArray.Length);

			for (ushort index = 0; index < (ushort)namesArray.Length; index++)
			{
				m_LightPresetsButtonList.SetItemVisible(index, true);
				m_LightPresetsButtonList.SetItemLabel(index, namesArray[index]);
			}
		}

		/// <summary>
		/// Sets the selected state of the preset button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetLightPresetSelected(ushort index, bool selected)
		{
			if (index < m_LightPresetsButtonList.MaxSize)
				m_LightPresetsButtonList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Returns child views for light items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<ILightComponentView> GetChildLightViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_LightsSubpageReferenceList, m_ChildLights, count);
		}

		/// <summary>
		/// Returns child views for shade items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IShadeComponentView> GetChildShadeViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ShadesSubpageReferenceList, m_ChildShades, count);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_LightsSubpageReferenceList.OnIsMovingChanged += LightsSubpageReferenceListOnIsMovingChanged;
			m_ShadesSubpageReferenceList.OnIsMovingChanged += ShadesSubpageReferenceListOnIsMovingChanged;
			m_LightPresetsButtonList.OnButtonClicked += LightPresetsButtonListOnButtonClicked;
			m_LightPresetsButtonList.OnIsMovingChanged += LightPresetsButtonListOnIsMovingChanged;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_LightsSubpageReferenceList.OnIsMovingChanged -= LightsSubpageReferenceListOnIsMovingChanged;
			m_ShadesSubpageReferenceList.OnIsMovingChanged -= ShadesSubpageReferenceListOnIsMovingChanged;
			m_LightPresetsButtonList.OnButtonClicked -= LightPresetsButtonListOnButtonClicked;
			m_LightPresetsButtonList.OnIsMovingChanged -= LightPresetsButtonListOnIsMovingChanged;
		}

		/// <summary>
		/// Called when the user starts/stops scrolling the shades list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ShadesSubpageReferenceListOnIsMovingChanged(object sender, BoolEventArgs args)
		{
			OnShadeListScrolling.Raise(this, new BoolEventArgs(args.Data));
		}

		/// <summary>
		/// Called when the user starts/stops scrolling the lights list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LightsSubpageReferenceListOnIsMovingChanged(object sender, BoolEventArgs args)
		{
			OnLightListScrolling.Raise(this, new BoolEventArgs(args.Data));
		}

		/// <summary>
		/// Called when the user clicks a preset button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LightPresetsButtonListOnButtonClicked(object sender, UShortEventArgs args)
		{
			OnPresetButtonPressed.Raise(this, new UShortEventArgs(args.Data));
		}

		/// <summary>
		/// Called when the user starts/stops scrolling the presets list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LightPresetsButtonListOnIsMovingChanged(object sender, BoolEventArgs args)
		{
			OnPresetsListScrolling.Raise(this, new BoolEventArgs(args.Data));
		}

		#endregion
	}
}
