using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Devices.Controls
{
	public sealed class DeviceControlsCollection : IEnumerable<IDeviceControl>
	{
		private readonly Dictionary<int, IDeviceControl> m_DeviceControls;
		private readonly SafeCriticalSection m_DeviceControlsSection;

		#region Properties

		public int Count { get { return m_DeviceControlsSection.Execute(() => m_DeviceControls.Count); } }

		/// <summary>
		/// Returns true if this instance has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		public IDeviceControl this[int control] { get { return GetControl(control); } }

		#endregion

		public DeviceControlsCollection()
		{
			m_DeviceControls = new Dictionary<int, IDeviceControl>();
			m_DeviceControlsSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		public void Add(IDeviceControl item)
		{
			m_DeviceControlsSection.Execute(() => m_DeviceControls.Add(item.Id, item));
		}

		public void Clear()
		{
			m_DeviceControlsSection.Execute(() => m_DeviceControls.Clear());
		}

		public IDeviceControl GetControl(int id)
		{
			m_DeviceControlsSection.Enter();

			try
			{
				if (!m_DeviceControls.ContainsKey(id))
					throw new KeyNotFoundException(string.Format("{0} does not contain control with id {1}", GetType().Name, id));
				return m_DeviceControls[id];
			}
			finally
			{
				m_DeviceControlsSection.Leave();
			}
		}

		public bool TryGetControl(int id, out IDeviceControl control)
		{
			m_DeviceControlsSection.Enter();

			try
			{
				return m_DeviceControls.TryGetValue(id, out control);
			}
			finally
			{
				m_DeviceControlsSection.Leave();
			}
		}

		public bool Remove(int id)
		{
			return m_DeviceControlsSection.Execute(() => m_DeviceControls.Remove(id));
		}

		public bool Contains(int id)
		{
			return m_DeviceControlsSection.Execute(() => m_DeviceControls.ContainsKey(id));
		}

		/// <summary>
		/// Gets the first control of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[CanBeNull]
		public T GetControl<T>()
			where T : IDeviceControl
		{
			return GetControls<T>().FirstOrDefault();
		}

		/// <summary>
		/// Gets the controls of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IEnumerable<T> GetControls<T>()
			where T : IDeviceControl
		{
			return this.OfType<T>();
		}

		/// <summary>
		/// Gets the control with the given id and type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public T GetControl<T>(int id)
			where T : IDeviceControl
		{
			IDeviceControl control = GetControl(id);
			if (control is T)
				return (T)control;

			string message = string.Format("{0} control {1} is of type {2}", GetType().Name, id, typeof(T).Name);
			throw new InvalidOperationException(message);
		}

		public IEnumerator<IDeviceControl> GetEnumerator()
		{
			m_DeviceControlsSection.Enter();

			try
			{
				return m_DeviceControls.OrderValuesByKey()
				                       .ToList()
				                       .GetEnumerator();
			}
			finally
			{
				m_DeviceControlsSection.Leave();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Releases resources but also allows for finalizing without touching managed resources.
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			if (!IsDisposed)
				DisposeFinal(disposing);
			IsDisposed = IsDisposed || disposing;
		}

		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		private void DisposeFinal(bool disposing)
		{
			if (!disposing)
				return;

			m_DeviceControlsSection.Enter();

			try
			{
				foreach (IDeviceControl control in m_DeviceControls.Values)
					control.Dispose();
				Clear();
			}
			finally
			{
				m_DeviceControlsSection.Leave();
			}
		}

		#endregion
	}
}
