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

		public bool supportsEditorMode;

		public bool inListByDefault;
	}
}
