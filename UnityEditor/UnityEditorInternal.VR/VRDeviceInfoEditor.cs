using System;
using UnityEngine.Scripting;

namespace UnityEditorInternal.VR
{
	[RequiredByNativeCode]
	public struct VRDeviceInfoEditor
	{
		public string deviceNameKey;

		public string deviceNameUI;

		public string externalPluginName;

		public string spatializerPluginName;

		public string spatializerEffectName;

		public bool supportsEditorMode;

		public bool inListByDefault;
	}
}
