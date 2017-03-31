using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEditor.Rendering
{
	public sealed class EditorGraphicsSettings
	{
		public static extern AlbedoSwatchInfo[] albedoSwatches
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static void SetTierSettingsImpl(BuildTargetGroup target, GraphicsTier tier, TierSettings settings)
		{
			EditorGraphicsSettings.INTERNAL_CALL_SetTierSettingsImpl(target, tier, ref settings);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTierSettingsImpl(BuildTargetGroup target, GraphicsTier tier, ref TierSettings settings);

		internal static TierSettings GetTierSettingsImpl(BuildTargetGroup target, GraphicsTier tier)
		{
			TierSettings result;
			EditorGraphicsSettings.INTERNAL_CALL_GetTierSettingsImpl(target, tier, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTierSettingsImpl(BuildTargetGroup target, GraphicsTier tier, out TierSettings value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OnUpdateTierSettingsImpl(BuildTargetGroup target, bool shouldReloadShaders);

		internal static TierSettings GetCurrentTierSettingsImpl()
		{
			TierSettings result;
			EditorGraphicsSettings.INTERNAL_CALL_GetCurrentTierSettingsImpl(out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetCurrentTierSettingsImpl(out TierSettings value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool AreTierSettingsAutomatic(BuildTargetGroup target, GraphicsTier tier);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void MakeTierSettingsAutomatic(BuildTargetGroup target, GraphicsTier tier, bool automatic);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RegisterUndoForGraphicsSettings();

		public static TierSettings GetTierSettings(BuildTargetGroup target, GraphicsTier tier)
		{
			return EditorGraphicsSettings.GetTierSettingsImpl(target, tier);
		}

		public static void SetTierSettings(BuildTargetGroup target, GraphicsTier tier, TierSettings settings)
		{
			if (settings.renderingPath == RenderingPath.UsePlayerSettings)
			{
				throw new ArgumentException("TierSettings.renderingPath must be actual rendering path (not UsePlayerSettings)", "settings");
			}
			EditorGraphicsSettings.SetTierSettingsImpl(target, tier, settings);
			EditorGraphicsSettings.MakeTierSettingsAutomatic(target, tier, false);
			EditorGraphicsSettings.OnUpdateTierSettingsImpl(target, true);
		}

		internal static TierSettings GetCurrentTierSettings()
		{
			return EditorGraphicsSettings.GetCurrentTierSettingsImpl();
		}

		[Obsolete("Use GetTierSettings() instead (UnityUpgradable) -> GetTierSettings(*)", false)]
		public static PlatformShaderSettings GetShaderSettingsForPlatform(BuildTargetGroup target, ShaderHardwareTier tier)
		{
			TierSettings tierSettings = EditorGraphicsSettings.GetTierSettings(target, (GraphicsTier)tier);
			return new PlatformShaderSettings
			{
				cascadedShadowMaps = tierSettings.cascadedShadowMaps,
				standardShaderQuality = tierSettings.standardShaderQuality,
				reflectionProbeBoxProjection = tierSettings.reflectionProbeBoxProjection,
				reflectionProbeBlending = tierSettings.reflectionProbeBlending
			};
		}

		[Obsolete("Use SetTierSettings() instead (UnityUpgradable) -> SetTierSettings(*)", false)]
		public static void SetShaderSettingsForPlatform(BuildTargetGroup target, ShaderHardwareTier tier, PlatformShaderSettings settings)
		{
			TierSettings tierSettings = EditorGraphicsSettings.GetTierSettings(target, (GraphicsTier)tier);
			tierSettings.standardShaderQuality = settings.standardShaderQuality;
			tierSettings.cascadedShadowMaps = settings.cascadedShadowMaps;
			tierSettings.reflectionProbeBoxProjection = settings.reflectionProbeBoxProjection;
			tierSettings.reflectionProbeBlending = settings.reflectionProbeBlending;
			EditorGraphicsSettings.SetTierSettings(target, (GraphicsTier)tier, tierSettings);
		}

		[Obsolete("Use GraphicsTier instead of ShaderHardwareTier enum", false)]
		public static TierSettings GetTierSettings(BuildTargetGroup target, ShaderHardwareTier tier)
		{
			return EditorGraphicsSettings.GetTierSettings(target, (GraphicsTier)tier);
		}

		[Obsolete("Use GraphicsTier instead of ShaderHardwareTier enum", false)]
		public static void SetTierSettings(BuildTargetGroup target, ShaderHardwareTier tier, TierSettings settings)
		{
			EditorGraphicsSettings.SetTierSettings(target, (GraphicsTier)tier, settings);
		}
	}
}
