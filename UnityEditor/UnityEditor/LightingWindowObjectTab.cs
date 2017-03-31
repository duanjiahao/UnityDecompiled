using System;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngineInternal;

namespace UnityEditor
{
	internal class LightingWindowObjectTab
	{
		private GITextureType[] kObjectPreviewTextureTypes = new GITextureType[]
		{
			GITextureType.Albedo,
			GITextureType.Emissive,
			GITextureType.Irradiance,
			GITextureType.Directionality,
			GITextureType.Charting,
			GITextureType.Baked,
			GITextureType.BakedDirectional,
			GITextureType.BakedCharting,
			GITextureType.BakedShadowMask
		};

		private static GUIContent[] kObjectPreviewTextureOptions = new GUIContent[]
		{
			EditorGUIUtility.TextContent("Albedo"),
			EditorGUIUtility.TextContent("Emissive"),
			EditorGUIUtility.TextContent("Realtime Intensity"),
			EditorGUIUtility.TextContent("Realtime Direction"),
			EditorGUIUtility.TextContent("Realtime Charting"),
			EditorGUIUtility.TextContent("Baked Intensity"),
			EditorGUIUtility.TextContent("Baked Direction"),
			EditorGUIUtility.TextContent("Baked Charting"),
			EditorGUIUtility.TextContent("Baked Shadowmask")
		};

		private ZoomableArea m_ZoomablePreview;

		private GUIContent m_SelectedObjectPreviewTexture;

		private int m_PreviousSelection;

		private AnimBool m_ShowClampedSize = new AnimBool();

		private Editor m_LightEditor;

		private Editor m_LightmapParametersEditor;

		public void OnEnable(EditorWindow window)
		{
			this.m_ShowClampedSize.value = false;
			this.m_ShowClampedSize.valueChanged.AddListener(new UnityAction(window.Repaint));
		}

		public void OnDisable()
		{
			UnityEngine.Object.DestroyImmediate(this.m_LightEditor);
			UnityEngine.Object.DestroyImmediate(this.m_LightmapParametersEditor);
		}

		public void ObjectPreview(Rect r)
		{
			if (r.height > 0f)
			{
				List<Texture2D> list = new List<Texture2D>();
				GITextureType[] array = this.kObjectPreviewTextureTypes;
				for (int i = 0; i < array.Length; i++)
				{
					GITextureType textureType = array[i];
					list.Add(LightmapVisualizationUtility.GetGITexture(textureType));
				}
				if (list.Count != 0)
				{
					if (this.m_ZoomablePreview == null)
					{
						this.m_ZoomablePreview = new ZoomableArea(true);
						this.m_ZoomablePreview.hRangeMin = 0f;
						this.m_ZoomablePreview.vRangeMin = 0f;
						this.m_ZoomablePreview.hRangeMax = 1f;
						this.m_ZoomablePreview.vRangeMax = 1f;
						this.m_ZoomablePreview.SetShownHRange(0f, 1f);
						this.m_ZoomablePreview.SetShownVRange(0f, 1f);
						this.m_ZoomablePreview.uniformScale = true;
						this.m_ZoomablePreview.scaleWithWindow = true;
					}
					GUI.Box(r, "", "PreBackground");
					Rect position = new Rect(r);
					position.y += 1f;
					position.height = 18f;
					GUI.Box(position, "", EditorStyles.toolbar);
					Rect position2 = new Rect(r);
					position2.y += 1f;
					position2.height = 18f;
					position2.width = 120f;
					Rect rect = new Rect(r);
					rect.yMin += position2.height;
					rect.yMax -= 14f;
					rect.width -= 11f;
					int num = Array.IndexOf<GUIContent>(LightingWindowObjectTab.kObjectPreviewTextureOptions, this.m_SelectedObjectPreviewTexture);
					if (num < 0)
					{
						num = 0;
					}
					num = EditorGUI.Popup(position2, num, LightingWindowObjectTab.kObjectPreviewTextureOptions, EditorStyles.toolbarPopup);
					if (num >= LightingWindowObjectTab.kObjectPreviewTextureOptions.Length)
					{
						num = 0;
					}
					this.m_SelectedObjectPreviewTexture = LightingWindowObjectTab.kObjectPreviewTextureOptions[num];
					LightmapType lightmapType = (this.kObjectPreviewTextureTypes[num] != GITextureType.BakedShadowMask && this.kObjectPreviewTextureTypes[num] != GITextureType.Baked && this.kObjectPreviewTextureTypes[num] != GITextureType.BakedDirectional && this.kObjectPreviewTextureTypes[num] != GITextureType.BakedCharting) ? LightmapType.DynamicLightmap : LightmapType.StaticLightmap;
					Event current = Event.current;
					EventType type = current.type;
					if (type != EventType.ValidateCommand && type != EventType.ExecuteCommand)
					{
						if (type == EventType.Repaint)
						{
							Texture2D texture2D = list[num];
							if (texture2D && Event.current.type == EventType.Repaint)
							{
								Rect rect2 = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
								rect2 = this.ResizeRectToFit(rect2, rect);
								rect2 = this.CenterToRect(rect2, rect);
								rect2 = this.ScaleRectByZoomableArea(rect2, this.m_ZoomablePreview);
								Rect position3 = new Rect(rect2);
								position3.x += 3f;
								position3.y += rect.y + 20f;
								Rect drawableArea = new Rect(rect);
								drawableArea.y += position2.height + 3f;
								float num2 = drawableArea.y - 14f;
								position3.y -= num2;
								drawableArea.y -= num2;
								FilterMode filterMode = texture2D.filterMode;
								texture2D.filterMode = FilterMode.Point;
								GITextureType textureType2 = this.kObjectPreviewTextureTypes[num];
								LightmapVisualizationUtility.DrawTextureWithUVOverlay(texture2D, Selection.activeGameObject, drawableArea, position3, textureType2);
								texture2D.filterMode = filterMode;
							}
						}
					}
					else if (Event.current.commandName == "FrameSelected")
					{
						Vector4 lightmapTilingOffset = LightmapVisualizationUtility.GetLightmapTilingOffset(lightmapType);
						Vector2 vector = new Vector2(lightmapTilingOffset.z, lightmapTilingOffset.w);
						Vector2 lhs = vector + new Vector2(lightmapTilingOffset.x, lightmapTilingOffset.y);
						vector = Vector2.Max(vector, Vector2.zero);
						lhs = Vector2.Min(lhs, Vector2.one);
						float y = 1f - vector.y;
						vector.y = 1f - lhs.y;
						lhs.y = y;
						Rect shownArea = new Rect(vector.x, vector.y, lhs.x - vector.x, lhs.y - vector.y);
						shownArea.x -= Mathf.Clamp(shownArea.height - shownArea.width, 0f, 3.40282347E+38f) / 2f;
						shownArea.y -= Mathf.Clamp(shownArea.width - shownArea.height, 0f, 3.40282347E+38f) / 2f;
						float num3 = Mathf.Max(shownArea.width, shownArea.height);
						shownArea.height = num3;
						shownArea.width = num3;
						this.m_ZoomablePreview.shownArea = shownArea;
						Event.current.Use();
					}
					if (this.m_PreviousSelection != Selection.activeInstanceID)
					{
						this.m_PreviousSelection = Selection.activeInstanceID;
						this.m_ZoomablePreview.SetShownHRange(0f, 1f);
						this.m_ZoomablePreview.SetShownVRange(0f, 1f);
					}
					Rect rect3 = new Rect(r);
					rect3.yMin += position2.height;
					this.m_ZoomablePreview.rect = rect3;
					this.m_ZoomablePreview.BeginViewGUI();
					this.m_ZoomablePreview.EndViewGUI();
					GUILayoutUtility.GetRect(r.width, r.height);
				}
			}
		}

		private Rect ResizeRectToFit(Rect rect, Rect to)
		{
			float a = to.width / rect.width;
			float b = to.height / rect.height;
			float num = Mathf.Min(a, b);
			float width = (float)((int)Mathf.Round(rect.width * num));
			float height = (float)((int)Mathf.Round(rect.height * num));
			return new Rect(rect.x, rect.y, width, height);
		}

		private Rect CenterToRect(Rect rect, Rect to)
		{
			float num = Mathf.Clamp((float)((int)(to.width - rect.width)) / 2f, 0f, 2.14748365E+09f);
			float num2 = Mathf.Clamp((float)((int)(to.height - rect.height)) / 2f, 0f, 2.14748365E+09f);
			return new Rect(rect.x + num, rect.y + num2, rect.width, rect.height);
		}

		private Rect ScaleRectByZoomableArea(Rect rect, ZoomableArea zoomableArea)
		{
			float num = -(zoomableArea.shownArea.x / zoomableArea.shownArea.width) * rect.width;
			float num2 = (zoomableArea.shownArea.y - (1f - zoomableArea.shownArea.height)) / zoomableArea.shownArea.height * rect.height;
			float width = rect.width / zoomableArea.shownArea.width;
			float height = rect.height / zoomableArea.shownArea.height;
			return new Rect(rect.x + num, rect.y + num2, width, height);
		}
	}
}
