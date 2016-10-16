using System;
using System.ComponentModel;

namespace UnityEditor
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("UnityEditor.AndroidBuildSubtarget has been deprecated. Use UnityEditor.MobileTextureSubtarget instead (UnityUpgradable) -> MobileTextureSubtarget", true)]
	public enum AndroidBuildSubtarget
	{
		Generic = -1,
		DXT = -1,
		PVRTC = -1,
		ATC = -1,
		ETC = -1,
		ETC2 = -1,
		ASTC = -1
	}
}
