using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;

namespace ICD.Connect.UI.Widgets
{
	public abstract class AbstractWidget<T> : IVtProControl
		where T : class, ISigInputOutput
	{
		private readonly T m_Panel;
		private readonly IVtProParent m_Parent;
		private readonly ushort m_Index;

		#region Properties

		/// <summary>
		/// Gets the panel this control is a part of.
		/// </summary>
		protected T Panel { get { return m_Panel; } }

		/// <summary>
		/// Gets the visibility state of the widget.
		/// </summary>
		public virtual bool IsVisible { get { return GetControls().Any(c => c.IsVisible); } }

		/// <summary>
		/// Returns true if this widget, and all parent controls are visible.
		/// </summary>
		public bool IsVisibleRecursive { get { return IsVisible && (m_Parent == null || m_Parent.IsVisibleRecursive); } }

		/// <summary>
		/// Gets the enabled state of the widget.
		/// </summary>
		public virtual bool IsEnabled { get { return GetControls().Any(c => c.IsEnabled); } }

		/// <summary>
		/// Gets the index of the widget in a list.
		/// </summary>
		public ushort Index { get { return m_Index; } }

		/// <summary>
		/// Gets/sets the parent for cumulative join offsets.
		/// </summary>
		public IJoinOffsets Parent { get { return m_Parent; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractWidget(T panel)
			: this(panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		protected AbstractWidget(T panel, IVtProParent parent)
			: this(panel, parent, 0)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractWidget(T panel, IVtProParent parent, ushort index)
		{
			if (panel == null)
				throw new InvalidOperationException(string.Format("Creating {0} with null panel", GetType().Name));

			m_Panel = panel;
			m_Parent = parent;
			m_Index = index;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			GetControls().ForEach(c => c.Dispose());
		}

		/// <summary>
		/// Enables/disables the button. Throws InvalidOperationException if there is no enable join.
		/// </summary>
		public virtual void Enable(bool state)
		{
			GetControls().ForEach(c => c.Enable(state));
		}

		/// <summary>
		/// Shows/hides the control. Throws InvalidOperationException if there is no visibility join.
		/// </summary>
		/// <param name="state"></param>
		public virtual void Show(bool state)
		{
			GetControls().ForEach(c => c.Show(state));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the child controls that make up this widget.
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerable<IVtProControl> GetControls();

		#endregion
	}
}
