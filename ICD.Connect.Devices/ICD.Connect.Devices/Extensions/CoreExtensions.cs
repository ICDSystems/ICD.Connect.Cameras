using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Devices.Extensions
{
	/// <summary>
	/// Extension methods for ICores.
	/// </summary>
	public static class CoreExtensions
	{
		/// <summary>
		/// Gets the control with the given device and control ids.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="deviceId"></param>
		/// <param name="controlId"></param>
		/// <returns></returns>
		[NotNull]
		public static IDeviceControl GetControl(this ICore extends, int deviceId, int controlId)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			IOriginator originator;
			if (!extends.Originators.TryGetChild(deviceId, out originator))
				throw new KeyNotFoundException(string.Format("No device with id {0}", deviceId));

			IDeviceBase device = originator as IDeviceBase;
			if (device == null)
				throw new KeyNotFoundException(string.Format("{0} is not of type {1}", originator, typeof(IDeviceBase).Name));

			IDeviceControl control;
			if (!device.Controls.TryGetControl(controlId, out control))
				throw new KeyNotFoundException(string.Format("{0} does not contain a control with id {1}", originator, controlId));

			return control;
		}

		/// <summary>
		/// Gets the control with the given device and control ids.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="device"></param>
		/// <param name="control"></param>
		/// <returns></returns>
		[NotNull]
		public static T GetControl<T>(this ICore extends, int device, int control)
			where T : IDeviceControl
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			IDeviceControl output = extends.GetControl(device, control);
			if (output is T)
				return (T)output;

			string message = string.Format("{0} can not be cast to {1}", output.GetType().Name, typeof(T).Name);
			throw new InvalidCastException(message);
		}
	}
}
