using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace UnityEditor
{
	internal class RendererEditorBase : Editor
	{
		internal class Probes
		{
			internal SerializedProperty m_UseLightProbes;
			internal SerializedProperty m_ReflectionProbeUsage;
			internal SerializedProperty m_ProbeAnchor;
			private GUIContent m_UseLightProbesStyle = EditorGUIUtility.TextContent("Renderer.UseLightProbes");
			private GUIContent m_ReflectionProbeUsageStyle = EditorGUIUtility.TextContent("Renderer.ReflectionProbeUsage");
			private GUIContent m_ProbeAnchorStyle = EditorGUIUtility.TextContent("Renderer.ProbeAnchor");
			private List<ReflectionProbeBlendInfo> m_BlendInfo = new List<ReflectionProbeBlendInfo>();
			internal void Initialize(SerializedObject serializedObject, bool initializeLightProbes)
			{
				if (initializeLightProbes)
				{
					this.m_UseLightProbes = serializedObject.FindProperty("m_UseLightProbes");
				}
				this.m_ReflectionProbeUsage = serializedObject.FindProperty("m_ReflectionProbeUsage");
				this.m_ProbeAnchor = serializedObject.FindProperty("m_ProbeAnchor");
			}
			internal void Initialize(SerializedObject serializedObject)
			{
				this.Initialize(serializedObject, true);
			}
			internal void OnGUI(Renderer renderer, bool useMiniStyle)
			{
				if (this.m_UseLightProbes != null)
				{
					EditorGUI.BeginDisabledGroup(LightmapEditorSettings.IsLightmappedOrDynamicLightmappedForRendering(renderer));
					if (!useMiniStyle)
					{
						EditorGUILayout.PropertyField(this.m_UseLightProbes, this.m_UseLightProbesStyle, new GUILayoutOption[0]);
					}
					else
					{
						ModuleUI.GUIToggle(this.m_UseLightProbesStyle, this.m_UseLightProbes);
					}
					EditorGUI.EndDisabledGroup();
				}
				if (!useMiniStyle)
				{
					this.m_ReflectionProbeUsage.intValue = (int)((ReflectionProbeUsage)EditorGUILayout.EnumPopup(this.m_ReflectionProbeUsageStyle, (ReflectionProbeUsage)this.m_ReflectionProbeUsage.intValue, new GUILayoutOption[0]));
				}
				else
				{
					ModuleUI.GUIPopup(this.m_ReflectionProbeUsageStyle, this.m_ReflectionProbeUsage, Enum.GetNames(typeof(ReflectionProbeUsage)));
				}
				bool flag = this.m_ReflectionProbeUsage.intValue != 0;
				if ((this.m_UseLightProbes != null && this.m_UseLightProbes.boolValue) || flag)
				{
					EditorGUI.indentLevel++;
					if (!useMiniStyle)
					{
						EditorGUILayout.PropertyField(this.m_ProbeAnchor, this.m_ProbeAnchorStyle, new GUILayoutOption[0]);
					}
					else
					{
						ModuleUI.GUIObject(this.m_ProbeAnchorStyle, this.m_ProbeAnchor);
					}
					if (flag)
					{
						renderer.GetClosestReflectionProbes(this.m_BlendInfo);
						RendererEditorBase.Probes.ShowClosestReflectionProbes(this.m_BlendInfo);
					}
					EditorGUI.indentLevel--;
				}
			}
			internal static void ShowClosestReflectionProbes(List<ReflectionProbeBlendInfo> blendInfos)
			{
				float num = 20f;
				float num2 = 60f;
				EditorGUI.BeginDisabledGroup(true);
				for (int i = 0; i < blendInfos.Count; i++)
				{
					Rect rect = GUILayoutUtility.GetRect(0f, 16f);
					rect = EditorGUI.IndentedRect(rect);
					float width = rect.width - num - num2;
					Rect position = rect;
					position.width = num;
					GUI.Label(position, "#" + i, EditorStyles.miniLabel);
					position.x += position.width;
					position.width = width;
					EditorGUI.ObjectField(position, blendInfos[i].probe, typeof(ReflectionProbe), true);
					position.x += position.width;
					position.width = num2;
					GUI.Label(position, "Weight " + blendInfos[i].weight.ToString("f2"), EditorStyles.miniLabel);
				}
				EditorGUI.EndDisabledGroup();
			}
			internal static string[] GetFieldsStringArray()
			{
				return new string[]
				{
					"m_UseLightProbes",
					"m_ReflectionProbeUsage",
					"m_ProbeAnchor"
				};
			}
		}
		private SerializedProperty m_SortingOrder;
		private SerializedProperty m_SortingLayerID;
		private GUIContent m_SortingLayerStyle = EditorGUIUtility.TextContent("Renderer.SortingLayer");
		private GUIContent m_SortingOrderStyle = EditorGUIUtility.TextContent("Renderer.SortingOrder");
		protected RendererEditorBase.Probes m_Probes;
		public virtual void OnEnable()
		{
			this.m_SortingOrder = base.serializedObject.FindProperty("m_SortingOrder");
			this.m_SortingLayerID = base.serializedObject.FindProperty("m_SortingLayerID");
		}
		protected void RenderSortingLayerFields()
		{
			EditorGUILayout.Space();
			EditorGUILayout.SortingLayerField(this.m_SortingLayerStyle, this.m_SortingLayerID, EditorStyles.popup, EditorStyles.label);
			EditorGUILayout.PropertyField(this.m_SortingOrder, this.m_SortingOrderStyle, new GUILayoutOption[0]);
		}
		protected void InitializeProbeFields()
		{
			this.m_Probes = new RendererEditorBase.Probes();
			this.m_Probes.Initialize(base.serializedObject);
		}
		protected void RenderProbeFields()
		{
			this.m_Probes.OnGUI((Renderer)this.target, false);
		}
	}
}
