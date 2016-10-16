using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering
{
	public sealed class EditorGraphicsSettings
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetShaderSettingsForPlatformImpl(BuildTargetGroup target, ShaderHardwareTier tier, PlatformShaderSettings settings);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern PlatformShaderSettings GetShaderSettingsForPlatformImpl(BuildTargetGroup target, ShaderHardwareTier tier);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OnUpdateShaderSettingsForPlatformImpl(BuildTargetGroup target, bool shouldReloadShaders);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool AreShaderSettingsAutomatic(BuildTargetGroup target, ShaderHardwareTier tier);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void MakeShaderSettingsAutomatic(BuildTargetGroup target, ShaderHardwareTier tier, bool automatic);

		public static PlatformShaderSettings GetShaderSettingsForPlatform(BuildTargetGroup target, ShaderHardwareTier tier)
		{
			return EditorGraphicsSettings.GetShaderSettingsForPlatformImpl(target, tier);
		}

		public static void SetShaderSettingsForPlatform(BuildTargetGroup target, ShaderHardwareTier tier, PlatformShaderSettings settings)
		{
			EditorGraphicsSettings.SetShaderSettingsForPlatformImpl(target, tier, settings);
			EditorGraphicsSettings.OnUpdateShaderSettingsForPlatformImpl(target, true);
		}
	}
}
