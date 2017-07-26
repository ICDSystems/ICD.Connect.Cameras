﻿using System;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Connect.Protocol.Data;
using ICD.Connect.Protocol.Ports;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICD.Connect.Protocol.Network.Direct
{
	[Serializable]
	public class AbstractMessage : ISerialData
	{
		public const char DELIMITER = (char)0xff;
		public string Type { get { return GetType().AssemblyQualifiedName; } }

		public Guid MessageId { get; set; }

		public HostInfo MessageFrom { get; set; }

		/// <summary>
		/// For messages recieved, tag them with a client ID so the handler can respond later to the correct client
		/// </summary>
		public uint ClientId { get; set; }

		public string Serialize()
		{
			return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }) + DELIMITER;
		}

		public static AbstractMessage Deserialize(string serial)
		{
			try
			{
				JObject obj = JObject.Parse(serial);
				Type type = System.Type.GetType(obj.SelectToken("Type").ToString());
				if (type == null)
					return null;
				return JsonConvert.DeserializeObject(serial, type, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto}) as AbstractMessage;
			}
			catch (JsonSerializationException e)
			{
				ServiceProvider.TryGetService<ILoggerService>()
				               .AddEntry(eSeverity.Error, "AbstractMessage failed to deserialize: {0}", e.Message);
				return null;
			}
		}
	}
}