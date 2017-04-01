using System;
using UnityEngine;

namespace UnityEditor
{
	internal class LightModeUtil
	{
		internal enum ShadowMaskMode
		{
			None,
			Full,
			PastShadowDistance
		}

		internal enum LightmapMixedBakeMode
		{
			IndirectOnly,
			LightmapsWithSubtractiveShadows,
			DirectAndIndirect,
			ShadowMaskAndIndirect
		}

		internal enum MixedLightModeRowIndex
		{
			IndirectOnly,
			ShadowMaskPastShadowDistance,
			ShadowMaskAllTheWay,
			Subtractive
		}

		public static readonly GUIContent s_enableBaked = EditorGUIUtility.TextContent("Baked Global Illumination|Controls whether Mixed and Baked lights will use baked Global Illumination. If enabled, Mixed lights are baked using the specified Lighting Mode and Baked lights will be completely baked and not adjustable at runtime.");

		public static readonly string[] s_typenames = new string[]
		{
			"Realtime",
			"Mixed",
			"Baked"
		};

		private static readonly GUIContent[] s_modes = new GUIContent[]
		{
			new GUIContent(LightModeUtil.s_typenames[0]),
			new GUIContent(LightModeUtil.s_typenames[1]),
			new GUIContent(LightModeUtil.s_typenames[2])
		};

		private int[] m_modeVals = new int[3];

		private UnityEngine.Object m_cachedObject = null;

		private SerializedObject m_so = null;

		private SerializedProperty m_enableRealtimeGI = null;

		private SerializedProperty m_mixedBakeMode = null;

		private SerializedProperty m_shadowMaskMode = null;

		private SerializedProperty m_enabledBakedGI = null;

		private SerializedProperty m_workflowMode = null;

		private SerializedProperty m_environmentMode = null;

		private static LightModeUtil gs_ptr = null;

		private LightModeUtil()
		{
			this.Load();
		}

		public static LightModeUtil Get()
		{
			if (LightModeUtil.gs_ptr == null)
			{
				LightModeUtil.gs_ptr = new LightModeUtil();
			}
			return LightModeUtil.gs_ptr;
		}

		public void GetModes(out int realtimeMode, out int mixedMode)
		{
			realtimeMode = this.m_modeVals[0];
			mixedMode = this.m_modeVals[1];
		}

		public bool AreBakedLightmapsEnabled()
		{
			return this.m_enabledBakedGI != null && this.m_enabledBakedGI.boolValue;
		}

		public bool IsRealtimeGIEnabled()
		{
			return this.m_enableRealtimeGI != null && this.m_enableRealtimeGI.boolValue;
		}

		public bool IsAnyGIEnabled()
		{
			return this.IsRealtimeGIEnabled() || this.AreBakedLightmapsEnabled();
		}

		public bool GetAmbientLightingMode(out int mode)
		{
			bool result;
			if (this.AreBakedLightmapsEnabled() && this.IsRealtimeGIEnabled())
			{
				mode = this.m_environmentMode.intValue;
				result = true;
			}
			else
			{
				mode = ((!this.AreBakedLightmapsEnabled()) ? 0 : 1);
				result = false;
			}
			return result;
		}

		public int GetAmbientLightingMode()
		{
			int result;
			this.GetAmbientLightingMode(out result);
			return result;
		}

		public bool IsSubtractiveModeEnabled()
		{
			return this.m_modeVals[1] == 3;
		}

		public bool IsWorkflowAuto()
		{
			return this.m_workflowMode.intValue == 0;
		}

		public void SetWorkflow(bool bAutoEnabled)
		{
			this.m_workflowMode.intValue = ((!bAutoEnabled) ? 1 : 0);
		}

		public void GetProps(out SerializedProperty o_enableRealtimeGI, out SerializedProperty o_enableBakedGI, out SerializedProperty o_mixedBakeMode, out SerializedProperty o_shadowMaskMode)
		{
			o_enableRealtimeGI = this.m_enableRealtimeGI;
			o_enableBakedGI = this.m_enabledBakedGI;
			o_mixedBakeMode = this.m_mixedBakeMode;
			o_shadowMaskMode = this.m_shadowMaskMode;
		}

		public LightModeUtil.MixedLightModeRowIndex MapMixedSettingsToRowIndex(LightModeUtil.LightmapMixedBakeMode bakeMode, LightModeUtil.ShadowMaskMode maskMode)
		{
			LightModeUtil.MixedLightModeRowIndex result;
			if (bakeMode != LightModeUtil.LightmapMixedBakeMode.IndirectOnly)
			{
				if (bakeMode != LightModeUtil.LightmapMixedBakeMode.ShadowMaskAndIndirect)
				{
					if (bakeMode != LightModeUtil.LightmapMixedBakeMode.LightmapsWithSubtractiveShadows)
					{
						Debug.LogError("Unkown Mixed bake mode in LightModeUtil.MapSettings()");
						result = LightModeUtil.MixedLightModeRowIndex.IndirectOnly;
					}
					else
					{
						result = LightModeUtil.MixedLightModeRowIndex.Subtractive;
					}
				}
				else
				{
					result = ((maskMode != LightModeUtil.ShadowMaskMode.PastShadowDistance) ? LightModeUtil.MixedLightModeRowIndex.ShadowMaskAllTheWay : LightModeUtil.MixedLightModeRowIndex.ShadowMaskPastShadowDistance);
				}
			}
			else
			{
				result = LightModeUtil.MixedLightModeRowIndex.IndirectOnly;
			}
			return result;
		}

		public bool Load()
		{
			bool result;
			if (!this.CheckCachedObject())
			{
				result = false;
			}
			else
			{
				int realtimeMode = (!this.m_enableRealtimeGI.boolValue) ? 1 : 0;
				int mixedMode = (int)this.MapMixedSettingsToRowIndex((LightModeUtil.LightmapMixedBakeMode)this.m_mixedBakeMode.intValue, (LightModeUtil.ShadowMaskMode)this.m_shadowMaskMode.intValue);
				this.Update(realtimeMode, mixedMode);
				result = true;
			}
			return result;
		}

		public void Store(int realtimeMode, int mixedMode)
		{
			this.Update(realtimeMode, mixedMode);
			if (this.CheckCachedObject())
			{
				this.m_enableRealtimeGI.boolValue = (this.m_modeVals[0] == 0);
				LightModeUtil.LightmapMixedBakeMode[] array = new LightModeUtil.LightmapMixedBakeMode[]
				{
					LightModeUtil.LightmapMixedBakeMode.IndirectOnly,
					LightModeUtil.LightmapMixedBakeMode.ShadowMaskAndIndirect,
					LightModeUtil.LightmapMixedBakeMode.ShadowMaskAndIndirect,
					LightModeUtil.LightmapMixedBakeMode.LightmapsWithSubtractiveShadows
				};
				this.m_mixedBakeMode.intValue = (int)array[this.m_modeVals[1]];
				LightModeUtil.ShadowMaskMode[] expr_5C = new LightModeUtil.ShadowMaskMode[4];
				expr_5C[1] = LightModeUtil.ShadowMaskMode.PastShadowDistance;
				expr_5C[2] = LightModeUtil.ShadowMaskMode.Full;
				LightModeUtil.ShadowMaskMode[] array2 = expr_5C;
				this.m_shadowMaskMode.intValue = (int)array2[this.m_modeVals[1]];
			}
		}

		public bool Flush()
		{
			return this.m_so.ApplyModifiedProperties();
		}

		public void DrawElement(Rect r, SerializedProperty property, SerializedProperty dependency)
		{
			bool disabled = dependency.enumValueIndex == 3;
			using (new EditorGUI.DisabledScope(disabled))
			{
				EditorGUI.BeginChangeCheck();
				int intValue = EditorGUI.IntPopup(r, property.intValue, LightModeUtil.s_modes, new int[]
				{
					4,
					1,
					2
				});
				if (EditorGUI.EndChangeCheck())
				{
					property.intValue = intValue;
				}
			}
		}

		public void DrawElement(SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginChangeCheck();
			int intValue = EditorGUILayout.IntPopup(label, property.intValue, LightModeUtil.s_modes, new int[]
			{
				4,
				1,
				2
			}, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				property.intValue = intValue;
			}
		}

		public void DrawBakedGIElement()
		{
			EditorGUILayout.PropertyField(this.m_enabledBakedGI, LightModeUtil.s_enableBaked, new GUILayoutOption[0]);
		}

		public void AnalyzeScene(ref LightModeValidator.Stats stats)
		{
			LightModeValidator.AnalyzeScene(this.m_modeVals[0], this.m_modeVals[1], this.m_modeVals[2], this.GetAmbientLightingMode(), ref stats);
		}

		private bool CheckCachedObject()
		{
			UnityEngine.Object lightmapSettings = LightmapEditorSettings.GetLightmapSettings();
			bool result;
			if (lightmapSettings == null)
			{
				result = false;
			}
			else if (lightmapSettings == this.m_cachedObject)
			{
				this.m_so.UpdateIfRequiredOrScript();
				result = true;
			}
			else
			{
				this.m_cachedObject = lightmapSettings;
				this.m_so = new SerializedObject(lightmapSettings);
				this.m_enableRealtimeGI = this.m_so.FindProperty("m_GISettings.m_EnableRealtimeLightmaps");
				this.m_mixedBakeMode = this.m_so.FindProperty("m_LightmapEditorSettings.m_MixedBakeMode");
				this.m_shadowMaskMode = this.m_so.FindProperty("m_ShadowMaskMode");
				this.m_enabledBakedGI = this.m_so.FindProperty("m_GISettings.m_EnableBakedLightmaps");
				this.m_workflowMode = this.m_so.FindProperty("m_GIWorkflowMode");
				this.m_environmentMode = this.m_so.FindProperty("m_GISettings.m_EnvironmentLightingMode");
				result = true;
			}
			return result;
		}

		private void Update(int realtimeMode, int mixedMode)
		{
			this.m_modeVals[0] = realtimeMode;
			this.m_modeVals[1] = mixedMode;
			this.m_modeVals[2] = 0;
		}
	}
}
