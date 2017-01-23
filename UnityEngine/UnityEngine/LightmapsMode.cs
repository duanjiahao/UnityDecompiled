using System;
using System.ComponentModel;

namespace UnityEngine
{
	public enum LightmapsMode
	{
		NonDirectional,
		CombinedDirectional,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.SeparateDirectional has been deprecated and will be removed in a future version. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", false)]
		SeparateDirectional,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Single has been deprecated. Use NonDirectional instead (UnityUpgradable) -> NonDirectional", true)]
		Single = 0,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Dual has been deprecated. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", true)]
		Dual,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Directional has been deprecated. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", true)]
		Directional
	}
}
