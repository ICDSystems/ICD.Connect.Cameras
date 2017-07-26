using System;
using System.Linq;
using ICD.Connect.Panels.SmartObjectCollections;
using ICD.Connect.Settings;

namespace ICD.Connect.Panels
{
	public abstract class AbstractPanelDevice<T> : AbstractSigDeviceBase<T>, IPanelDevice
		where T : IPanelDeviceSettings, new()
	{
		#region Properties

		/// <summary>
		/// Collection containing the loaded SmartObjects of this device.
		/// </summary>
		public abstract ISmartObjectCollection SmartObjects { get; }

		/// <summary>
		/// Gets the time that the user last interacted with the panel.
		/// </summary>
		public override DateTime? LastOutput
		{
			get
			{
				// Return the most recent output time.
				DateTime? output = base.LastOutput;
				DateTime? smartOutput = SmartObjects.Select(p => p.Value)
				                                    .Select(s => s.LastOutput)
				                                    .Where(d => d != null)
				                                    .Max();

				if (output == null)
					return smartOutput;
				if (smartOutput == null)
					return output;

				return output > smartOutput ? output : smartOutput;
			}
		}

		#endregion
	}
}
