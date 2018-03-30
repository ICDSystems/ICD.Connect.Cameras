using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.EventArguments;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class GenericCameraRouteSourceControl<TCameraDevice> : AbstractRouteSourceControl<TCameraDevice>
		where TCameraDevice : ICameraDevice
	{
		public override event EventHandler<TransmissionStateEventArgs> OnActiveTransmissionStateChanged;

		public GenericCameraRouteSourceControl(TCameraDevice parent, int id)
			: base(parent, id)
		{
		}

		protected override void DisposeFinal(bool disposing)
		{
			OnActiveTransmissionStateChanged = null;

			base.DisposeFinal(disposing);
		}

		public override bool GetActiveTransmissionState(int output, eConnectionType type)
		{
			if (EnumUtils.HasMultipleFlags(type))
			{
				return
					EnumUtils.GetFlagsExceptNone(type)
					         .Select(f => GetActiveTransmissionState(output, f))
					         .Unanimous(false);
			}

			if (output != 1)
			{
				string message = string.Format("{0} has no {1} output at address {2}", this, type, output);
				throw new KeyNotFoundException(message);
			}

			switch (type)
			{
				case eConnectionType.Video:
					return true;
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}

		public override IEnumerable<ConnectorInfo> GetOutputs()
		{
			yield return new ConnectorInfo(1, eConnectionType.Video);
		}
	}
}
