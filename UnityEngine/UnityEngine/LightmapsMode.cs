using System;
using System.ComponentModel;

namespace UnityEngine
{
	public enum LightmapsMode
	{
		NonDirectional,
		CombinedDirectional,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.SeparateDirectional has been removed. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", true)]
		SeparateDirectional,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Single has been removed. Use NonDirectional instead (UnityUpgradable) -> NonDirectional", true)]
		Single = 0,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Dual has been removed. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", true)]
		Dual,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Directional has been removed. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", true)]
		Directional
	}
}
