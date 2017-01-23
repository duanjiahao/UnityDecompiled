using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal static class ShadowCascadeSplitGUI
	{
		private class DragCache
		{
			public int m_ActivePartition;

			public float m_NormalizedPartitionSize;

			public Vector2 m_LastCachedMousePosition;

			public DragCache(int activePartition, float normalizedPartitionSize, Vector2 currentMousePos)
			{
				this.m_ActivePartition = activePartition;
				this.m_NormalizedPartitionSize = normalizedPartitionSize;
				this.m_LastCachedMousePosition = currentMousePos;
			}
		}

		private const int kSliderbarTopMargin = 2;

		private const int kSliderbarHeight = 24;

		private const int kSliderbarBottomMargin = 2;

		private const int kPartitionHandleWidth = 2;

		private const int kPartitionHandleExtraHitAreaWidth = 2;

		private static readonly Color[] kCascadeColors = new Color[]
		{
			new Color(0.5f, 0.5f, 0.6f, 1f),
			new Color(0.5f, 0.6f, 0.5f, 1f),
			new Color(0.6f, 0.6f, 0.5f, 1f),
			new Color(0.6f, 0.5f, 0.5f, 1f)
		};

		private static readonly GUIStyle s_CascadeSliderBG = "LODSliderRange";

		private static readonly GUIStyle s_TextCenteredStyle = new GUIStyle(EditorStyles.whiteMiniLabel)
		{
			alignment = TextAnchor.MiddleCenter
		};

		private static ShadowCascadeSplitGUI.DragCache s_DragCache;

		private static readonly int s_CascadeSliderId = "s_CascadeSliderId".GetHashCode();

		private static SceneView s_RestoreSceneView;

		private static DrawCameraMode s_OldSceneDrawMode = DrawCameraMode.Textured;

		private static bool s_OldSceneLightingMode;

		public static void HandleCascadeSliderGUI(ref float[] normalizedCascadePartitions)
		{
			GUILayout.Label("Cascade splits", new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, ShadowCascadeSplitGUI.s_CascadeSliderBG, new GUILayoutOption[]
			{
				GUILayout.Height(28f),
				GUILayout.ExpandWidth(true)
			});
			GUI.Box(rect, GUIContent.none);
			float num = rect.x;
			float y = rect.y + 2f;
			float num2 = rect.width - (float)(normalizedCascadePartitions.Length * 2);
			Color color = GUI.color;
			Color backgroundColor = GUI.backgroundColor;
			int num3 = -1;
			float[] array = new float[normalizedCascadePartitions.Length + 1];
			Array.Copy(normalizedCascadePartitions, array, normalizedCascadePartitions.Length);
			array[array.Length - 1] = 1f - normalizedCascadePartitions.Sum();
			int controlID = GUIUtility.GetControlID(ShadowCascadeSplitGUI.s_CascadeSliderId, FocusType.Passive);
			Event current = Event.current;
			int num4 = -1;
			for (int i = 0; i < array.Length; i++)
			{
				float num5 = array[i];
				num3 = (num3 + 1) % ShadowCascadeSplitGUI.kCascadeColors.Length;
				GUI.backgroundColor = ShadowCascadeSplitGUI.kCascadeColors[num3];
				float num6 = num2 * num5;
				Rect rect2 = new Rect(num, y, num6, 24f);
				GUI.Box(rect2, GUIContent.none, ShadowCascadeSplitGUI.s_CascadeSliderBG);
				num += num6;
				GUI.color = Color.white;
				Rect position = rect2;
				string text = string.Format("{0}\n{1:F1}%", i, num5 * 100f);
				GUI.Label(position, GUIContent.Temp(text, text), ShadowCascadeSplitGUI.s_TextCenteredStyle);
				if (i == array.Length - 1)
				{
					break;
				}
				GUI.backgroundColor = Color.black;
				Rect rect3 = rect2;
				rect3.x = num;
				rect3.width = 2f;
				GUI.Box(rect3, GUIContent.none, ShadowCascadeSplitGUI.s_CascadeSliderBG);
				Rect position2 = rect3;
				position2.xMin -= 2f;
				position2.xMax += 2f;
				if (position2.Contains(current.mousePosition))
				{
					num4 = i;
				}
				if (ShadowCascadeSplitGUI.s_DragCache == null)
				{
					EditorGUIUtility.AddCursorRect(position2, MouseCursor.ResizeHorizontal, controlID);
				}
				num += 2f;
			}
			GUI.color = color;
			GUI.backgroundColor = backgroundColor;
			EventType typeForControl = current.GetTypeForControl(controlID);
			if (typeForControl != EventType.MouseDown)
			{
				if (typeForControl != EventType.MouseUp)
				{
					if (typeForControl == EventType.MouseDrag)
					{
						if (GUIUtility.hotControl == controlID)
						{
							float num7 = (current.mousePosition - ShadowCascadeSplitGUI.s_DragCache.m_LastCachedMousePosition).x / num2;
							bool flag = array[ShadowCascadeSplitGUI.s_DragCache.m_ActivePartition] + num7 > 0f;
							bool flag2 = array[ShadowCascadeSplitGUI.s_DragCache.m_ActivePartition + 1] - num7 > 0f;
							if (flag && flag2)
							{
								ShadowCascadeSplitGUI.s_DragCache.m_NormalizedPartitionSize += num7;
								normalizedCascadePartitions[ShadowCascadeSplitGUI.s_DragCache.m_ActivePartition] = ShadowCascadeSplitGUI.s_DragCache.m_NormalizedPartitionSize;
								if (ShadowCascadeSplitGUI.s_DragCache.m_ActivePartition < normalizedCascadePartitions.Length - 1)
								{
									normalizedCascadePartitions[ShadowCascadeSplitGUI.s_DragCache.m_ActivePartition + 1] -= num7;
								}
								GUI.changed = true;
							}
							ShadowCascadeSplitGUI.s_DragCache.m_LastCachedMousePosition = current.mousePosition;
							current.Use();
						}
					}
				}
				else
				{
					if (GUIUtility.hotControl == controlID)
					{
						GUIUtility.hotControl = 0;
						current.Use();
					}
					ShadowCascadeSplitGUI.s_DragCache = null;
					if (ShadowCascadeSplitGUI.s_RestoreSceneView != null)
					{
						ShadowCascadeSplitGUI.s_RestoreSceneView.renderMode = ShadowCascadeSplitGUI.s_OldSceneDrawMode;
						ShadowCascadeSplitGUI.s_RestoreSceneView.m_SceneLighting = ShadowCascadeSplitGUI.s_OldSceneLightingMode;
						ShadowCascadeSplitGUI.s_RestoreSceneView = null;
					}
				}
			}
			else if (num4 >= 0)
			{
				ShadowCascadeSplitGUI.s_DragCache = new ShadowCascadeSplitGUI.DragCache(num4, normalizedCascadePartitions[num4], current.mousePosition);
				if (GUIUtility.hotControl == 0)
				{
					GUIUtility.hotControl = controlID;
				}
				current.Use();
				if (ShadowCascadeSplitGUI.s_RestoreSceneView == null)
				{
					ShadowCascadeSplitGUI.s_RestoreSceneView = SceneView.lastActiveSceneView;
					if (ShadowCascadeSplitGUI.s_RestoreSceneView != null)
					{
						ShadowCascadeSplitGUI.s_OldSceneDrawMode = ShadowCascadeSplitGUI.s_RestoreSceneView.renderMode;
						ShadowCascadeSplitGUI.s_OldSceneLightingMode = ShadowCascadeSplitGUI.s_RestoreSceneView.m_SceneLighting;
						ShadowCascadeSplitGUI.s_RestoreSceneView.renderMode = DrawCameraMode.ShadowCascades;
					}
				}
			}
		}
	}
}
