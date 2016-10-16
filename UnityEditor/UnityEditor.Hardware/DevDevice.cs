using System;
using UnityEngine.Scripting;

namespace UnityEditor.Hardware
{
	[RequiredByNativeCode]
	public struct DevDevice
	{
		public readonly string id;

		public readonly string name;

		public readonly string type;

		public readonly string module;

		public readonly DevDeviceState state;

		public readonly DevDeviceFeatures features;

		public bool isConnected
		{
			get
			{
				return this.state == DevDeviceState.Connected;
			}
		}

		public static DevDevice none
		{
			get
			{
				return new DevDevice("None", "None", "none", "internal", DevDeviceState.Disconnected, DevDeviceFeatures.None);
			}
		}

		public DevDevice(string id, string name, string type, string module, DevDeviceState state, DevDeviceFeatures features)
		{
			this.id = id;
			this.name = name;
			this.type = type;
			this.module = module;
			this.state = state;
			this.features = features;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.name,
				" (id:",
				this.id,
				", type: ",
				this.type,
				", module: ",
				this.module,
				", state: ",
				this.state,
				", features: ",
				this.features,
				")"
			});
		}
	}
}
