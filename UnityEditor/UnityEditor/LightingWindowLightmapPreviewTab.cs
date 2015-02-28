using System;
using System.Collections;
using UnityEngine;
namespace UnityEditor
{
	internal class LightingWindowLightmapPreviewTab
	{
		private class Styles
		{
			public GUIStyle selectedLightmapHighlight = "LightmapEditorSelectedHighlight";
			public GUIContent LightProbes = EditorGUIUtility.TextContent("LightmapEditor.LightProbes");
			public GUIContent LightmapSnapshot = EditorGUIUtility.TextContent("LightmapEditor.LightmapSnapshot");
			public GUIContent MapsArraySize = EditorGUIUtility.TextContent("LightmapEditor.MapsArraySize");
		}
		private Vector2 m_ScrollPositionLightmaps = Vector2.zero;
		private Vector2 m_ScrollPositionMaps = Vector2.zero;
		private int m_SelectedLightmap = -1;
		private static LightingWindowLightmapPreviewTab.Styles s_Styles;
		private static void Header(ref Rect rect, float headerHeight, float headerLeftMargin, float maxLightmaps)
		{
			Rect rect2 = GUILayoutUtility.GetRect(rect.width, headerHeight);
			rect2.width = rect.width / maxLightmaps;
			rect2.y -= rect.height;
			rect.y += headerHeight;
			rect2.x += headerLeftMargin;
			EditorGUI.DropShadowLabel(rect2, "Intensity");
			rect2.x += rect2.width;
			EditorGUI.DropShadowLabel(rect2, "Directionality");
		}
		private void MenuSelectLightmapUsers(Rect rect, int lightmapIndex)
		{
			if (Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))
			{
				string[] texts = new string[]
				{
					"Select Lightmap Users"
				};
				Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
				EditorUtility.DisplayCustomMenu(position, EditorGUIUtility.TempContent(texts), -1, new EditorUtility.SelectMenuItemFunction(this.SelectLightmapUsers), lightmapIndex);
				Event.current.Use();
			}
		}
		private void SelectLightmapUsers(object userData, string[] options, int selected)
		{
			int num = (int)userData;
			ArrayList arrayList = new ArrayList();
			MeshRenderer[] array = UnityEngine.Object.FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
			MeshRenderer[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				MeshRenderer meshRenderer = array2[i];
				if (meshRenderer != null && meshRenderer.lightmapIndex == num)
				{
					arrayList.Add(meshRenderer.gameObject);
				}
			}
			Terrain[] array3 = UnityEngine.Object.FindObjectsOfType(typeof(Terrain)) as Terrain[];
			Terrain[] array4 = array3;
			for (int j = 0; j < array4.Length; j++)
			{
				Terrain terrain = array4[j];
				if (terrain != null && terrain.lightmapIndex == num)
				{
					arrayList.Add(terrain.gameObject);
				}
			}
			Selection.objects = (arrayList.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[]);
		}
		public void LightmapPreview(Rect r)
		{
			if (LightingWindowLightmapPreviewTab.s_Styles == null)
			{
				LightingWindowLightmapPreviewTab.s_Styles = new LightingWindowLightmapPreviewTab.Styles();
			}
			bool flag = true;
			GUI.Box(r, string.Empty, "PreBackground");
			this.m_ScrollPositionLightmaps = EditorGUILayout.BeginScrollView(this.m_ScrollPositionLightmaps, new GUILayoutOption[]
			{
				GUILayout.Height(r.height)
			});
			int num = 0;
			float num2 = 2f;
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			for (int i = 0; i < lightmaps.Length; i++)
			{
				LightmapData lightmapData = lightmaps[i];
				if (lightmapData.lightmapFar == null && lightmapData.lightmapNear == null)
				{
					num++;
				}
				else
				{
					int num3 = (!lightmapData.lightmapFar) ? -1 : Math.Max(lightmapData.lightmapFar.width, lightmapData.lightmapFar.height);
					int num4 = (!lightmapData.lightmapNear) ? -1 : Math.Max(lightmapData.lightmapNear.width, lightmapData.lightmapNear.height);
					Texture2D texture2D = (num3 <= num4) ? lightmapData.lightmapNear : lightmapData.lightmapFar;
					GUILayoutOption[] options = new GUILayoutOption[]
					{
						GUILayout.MaxWidth((float)texture2D.width * num2),
						GUILayout.MaxHeight((float)texture2D.height)
					};
					Rect aspectRect = GUILayoutUtility.GetAspectRect((float)texture2D.width * num2 / (float)texture2D.height, options);
					if (flag)
					{
						LightingWindowLightmapPreviewTab.Header(ref aspectRect, 20f, 6f, num2);
						flag = false;
					}
					aspectRect.width /= num2;
					EditorGUI.DrawPreviewTexture(aspectRect, lightmapData.lightmapFar);
					this.MenuSelectLightmapUsers(aspectRect, num);
					if (lightmapData.lightmapNear)
					{
						aspectRect.x += aspectRect.width;
						EditorGUI.DrawPreviewTexture(aspectRect, lightmapData.lightmapNear);
						this.MenuSelectLightmapUsers(aspectRect, num);
					}
					num++;
				}
			}
			EditorGUILayout.EndScrollView();
		}
		public void UpdateLightmapSelection()
		{
			Terrain terrain = null;
			MeshRenderer component;
			if (Selection.activeGameObject == null || ((component = Selection.activeGameObject.GetComponent<MeshRenderer>()) == null && (terrain = Selection.activeGameObject.GetComponent<Terrain>()) == null))
			{
				this.m_SelectedLightmap = -1;
				return;
			}
			this.m_SelectedLightmap = ((!(component != null)) ? terrain.lightmapIndex : component.lightmapIndex);
		}
		public void Maps()
		{
			if (LightingWindowLightmapPreviewTab.s_Styles == null)
			{
				LightingWindowLightmapPreviewTab.s_Styles = new LightingWindowLightmapPreviewTab.Styles();
			}
			GUI.changed = false;
			if (Lightmapping.giWorkflowMode == Lightmapping.GIWorkflowMode.OnDemand)
			{
				SerializedObject serializedObject = new SerializedObject(LightmapEditorSettings.GetLightmapSettings());
				SerializedProperty property = serializedObject.FindProperty("m_LightmapSnapshot");
				EditorGUILayout.PropertyField(property, LightingWindowLightmapPreviewTab.s_Styles.LightmapSnapshot, new GUILayoutOption[0]);
				serializedObject.ApplyModifiedProperties();
			}
			GUILayout.Space(10f);
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			EditorGUI.BeginDisabledGroup(true);
			this.m_ScrollPositionMaps = GUILayout.BeginScrollView(this.m_ScrollPositionMaps, new GUILayoutOption[0]);
			for (int i = 0; i < lightmaps.Length; i++)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.Label(i.ToString(), new GUILayoutOption[0]);
				GUILayout.Space(5f);
				lightmaps[i].lightmapFar = this.LightmapField(lightmaps[i].lightmapFar, i);
				GUILayout.Space(10f);
				lightmaps[i].lightmapNear = this.LightmapField(lightmaps[i].lightmapNear, i);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			EditorGUI.EndDisabledGroup();
		}
		private Texture2D LightmapField(Texture2D lightmap, int index)
		{
			Rect rect = GUILayoutUtility.GetRect(100f, 100f, EditorStyles.objectField);
			this.MenuSelectLightmapUsers(rect, index);
			Texture2D result = EditorGUI.ObjectField(rect, lightmap, typeof(Texture2D), false) as Texture2D;
			if (index == this.m_SelectedLightmap && Event.current.type == EventType.Repaint)
			{
				LightingWindowLightmapPreviewTab.s_Styles.selectedLightmapHighlight.Draw(rect, false, false, false, false);
			}
			return result;
		}
	}
}
