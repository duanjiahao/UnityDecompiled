using System;

namespace UnityEditor
{
	internal class LightModeValidator
	{
		[Flags]
		internal enum Receivers
		{
			None = 0,
			StaticMesh = 1,
			LightProbe = 2
		}

		[Flags]
		internal enum Emitters
		{
			None = 0,
			RealtimeLight = 2,
			RealtimeAmbient = 4,
			RealtimeEmissive = 8,
			BakedLight = 16,
			BakedAmbient = 32,
			BakedEmissive = 64,
			Realtime = 14,
			Baked = 112
		}

		internal struct Stats
		{
			public LightModeValidator.Receivers receiverMask;

			public LightModeValidator.Emitters emitterMask;

			public int realtimeMode;

			public int mixedMode;

			public int bakedMode;

			public int ambientMode;

			public bool requiresRealtimeGI;

			public bool requiresLightmaps;

			public LightingStats enabled;

			public LightingStats active;

			public LightingStats inactive;

			public void Reset()
			{
				this.receiverMask = LightModeValidator.Receivers.None;
				this.emitterMask = LightModeValidator.Emitters.None;
				this.realtimeMode = 0;
				this.mixedMode = 0;
				this.bakedMode = 0;
				this.ambientMode = 0;
				this.requiresRealtimeGI = false;
				this.requiresLightmaps = false;
				this.enabled.Reset();
				this.active.Reset();
				this.inactive.Reset();
			}
		}

		internal static void AnalyzeScene(int realtimeMode, int mixedMode, int bakedMode, int ambientMode, ref LightModeValidator.Stats stats)
		{
			stats.Reset();
			stats.realtimeMode = realtimeMode;
			stats.mixedMode = mixedMode;
			stats.bakedMode = bakedMode;
			stats.ambientMode = ambientMode;
			LightmapEditorSettings.AnalyzeLighting(out stats.enabled, out stats.active, out stats.inactive);
			stats.emitterMask = LightModeValidator.Emitters.None;
			stats.emitterMask |= ((stats.enabled.realtimeLightsCount <= 0u) ? LightModeValidator.Emitters.None : LightModeValidator.Emitters.RealtimeLight);
			stats.emitterMask |= ((stats.enabled.staticMeshesRealtimeEmissive <= 0u) ? LightModeValidator.Emitters.None : LightModeValidator.Emitters.RealtimeEmissive);
			stats.emitterMask |= ((!LightModeValidator.IsAmbientRealtime(ref stats)) ? LightModeValidator.Emitters.None : LightModeValidator.Emitters.RealtimeAmbient);
			stats.emitterMask |= ((stats.enabled.bakedLightsCount <= 0u) ? LightModeValidator.Emitters.None : LightModeValidator.Emitters.BakedLight);
			stats.emitterMask |= ((stats.enabled.staticMeshesBakedEmissive <= 0u) ? LightModeValidator.Emitters.None : LightModeValidator.Emitters.BakedEmissive);
			stats.emitterMask |= ((!LightModeValidator.IsAmbientBaked(ref stats)) ? LightModeValidator.Emitters.None : LightModeValidator.Emitters.BakedAmbient);
			stats.receiverMask = LightModeValidator.Receivers.None;
			stats.receiverMask |= ((stats.enabled.lightProbeGroupsCount <= 0u) ? LightModeValidator.Receivers.None : LightModeValidator.Receivers.LightProbe);
			stats.receiverMask |= ((stats.enabled.staticMeshesCount <= 0u) ? LightModeValidator.Receivers.None : LightModeValidator.Receivers.StaticMesh);
			if (stats.receiverMask == LightModeValidator.Receivers.None)
			{
				stats.requiresRealtimeGI = false;
				stats.requiresLightmaps = false;
			}
			else
			{
				stats.requiresRealtimeGI = LightModeValidator.IsRealtimeGI(ref stats);
				stats.requiresLightmaps = ((stats.emitterMask & LightModeValidator.Emitters.Baked) != LightModeValidator.Emitters.None);
			}
		}

		private static bool IsRealtimeGI(ref LightModeValidator.Stats stats)
		{
			return stats.realtimeMode == 0;
		}

		private static bool IsAmbientRealtime(ref LightModeValidator.Stats stats)
		{
			return stats.ambientMode == 0;
		}

		private static bool IsAmbientBaked(ref LightModeValidator.Stats stats)
		{
			return stats.ambientMode == 1;
		}
	}
}
