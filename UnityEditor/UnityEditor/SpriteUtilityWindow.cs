using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class SpriteUtilityWindow : EditorWindow
	{
		protected class Styles
		{
			public readonly GUIStyle dragdot = "U2D.dragDot";

			public readonly GUIStyle dragdotDimmed = "U2D.dragDotDimmed";

			public readonly GUIStyle dragdotactive = "U2D.dragDotActive";

			public readonly GUIStyle createRect = "U2D.createRect";

			public readonly GUIStyle preToolbar = "preToolbar";

			public readonly GUIStyle preButton = "preButton";

			public readonly GUIStyle preLabel = "preLabel";

			public readonly GUIStyle preSlider = "preSlider";

			public readonly GUIStyle preSliderThumb = "preSliderThumb";

			public readonly GUIStyle preBackground = "preBackground";

			public readonly GUIStyle pivotdotactive = "U2D.pivotDotActive";

			public readonly GUIStyle pivotdot = "U2D.pivotDot";

			public readonly GUIStyle dragBorderdot = new GUIStyle();

			public readonly GUIStyle dragBorderDotActive = new GUIStyle();

			public readonly GUIStyle toolbar;

			public readonly GUIContent alphaIcon;

			public readonly GUIContent RGBIcon;

			public readonly GUIStyle notice;

			public readonly GUIContent smallMip;

			public readonly GUIContent largeMip;

			public static readonly GUIContent[] spriteAlignmentOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Center"),
				EditorGUIUtility.TextContent("Top Left"),
				EditorGUIUtility.TextContent("Top"),
				EditorGUIUtility.TextContent("Top Right"),
				EditorGUIUtility.TextContent("Left"),
				EditorGUIUtility.TextContent("Right"),
				EditorGUIUtility.TextContent("Bottom Left"),
				EditorGUIUtility.TextContent("Bottom"),
				EditorGUIUtility.TextContent("Bottom Right"),
				EditorGUIUtility.TextContent("Custom")
			};

			public static GUIContent s_PivotLabel = EditorGUIUtility.TextContent("Pivot");

			public static GUIContent s_NoSelectionWarning = EditorGUIUtility.TextContent("No texture or sprite selected");

			public static GUIContent s_SpriteTessellationDetail = EditorGUIUtility.TextContent("Tessellation Detail");

			public Styles()
			{
				this.toolbar = new GUIStyle(EditorStyles.inspectorBig);
				this.toolbar.margin.top = 0;
				this.toolbar.margin.bottom = 0;
				this.alphaIcon = EditorGUIUtility.IconContent("PreTextureAlpha");
				this.RGBIcon = EditorGUIUtility.IconContent("PreTextureRGB");
				this.preToolbar.border.top = 0;
				this.createRect.border = new RectOffset(3, 3, 3, 3);
				this.notice = new GUIStyle(GUI.skin.label);
				this.notice.alignment = TextAnchor.MiddleCenter;
				this.notice.normal.textColor = Color.yellow;
				this.dragBorderdot.fixedHeight = 5f;
				this.dragBorderdot.fixedWidth = 5f;
				this.dragBorderdot.normal.background = EditorGUIUtility.whiteTexture;
				this.dragBorderDotActive.fixedHeight = this.dragBorderdot.fixedHeight;
				this.dragBorderDotActive.fixedWidth = this.dragBorderdot.fixedWidth;
				this.dragBorderDotActive.normal.background = EditorGUIUtility.whiteTexture;
				this.smallMip = EditorGUIUtility.IconContent("PreTextureMipMapLow");
				this.largeMip = EditorGUIUtility.IconContent("PreTextureMipMapHigh");
			}
		}

		protected static SpriteUtilityWindow.Styles s_Styles;

		protected const float k_BorderMargin = 10f;

		protected const float k_ScrollbarMargin = 16f;

		protected const float k_InspectorWindowMargin = 8f;

		protected const float k_InspectorWidth = 330f;

		protected const float k_MinZoomPercentage = 0.9f;

		protected const float k_MaxZoom = 50f;

		protected const float k_WheelZoomSpeed = 0.03f;

		protected const float k_MouseZoomSpeed = 0.005f;

		protected Texture2D m_Texture;

		protected Texture2D m_TextureAlphaOverride;

		protected Rect m_TextureViewRect;

		protected Rect m_TextureRect;

		protected bool m_ShowAlpha = false;

		protected float m_Zoom = -1f;

		protected float m_MipLevel = 0f;

		protected Vector2 m_ScrollPosition = default(Vector2);

		protected Rect maxScrollRect
		{
			get
			{
				float num = (float)this.m_Texture.width * 0.5f * this.m_Zoom;
				float num2 = (float)this.m_Texture.height * 0.5f * this.m_Zoom;
				return new Rect(-num, -num2, this.m_TextureViewRect.width + num * 2f, this.m_TextureViewRect.height + num2 * 2f);
			}
		}

		protected Rect maxRect
		{
			get
			{
				float num = this.m_TextureViewRect.width * 0.5f / this.GetMinZoom();
				float num2 = this.m_TextureViewRect.height * 0.5f / this.GetMinZoom();
				float x = -num;
				float y = -num2;
				float width = (float)this.m_Texture.width + num * 2f;
				float height = (float)this.m_Texture.height + num2 * 2f;
				return new Rect(x, y, width, height);
			}
		}

		protected void InitStyles()
		{
			if (SpriteUtilityWindow.s_Styles == null)
			{
				SpriteUtilityWindow.s_Styles = new SpriteUtilityWindow.Styles();
			}
		}

		protected float GetMinZoom()
		{
			float result;
			if (this.m_Texture == null)
			{
				result = 1f;
			}
			else
			{
				result = Mathf.Min(new float[]
				{
					this.m_TextureViewRect.width / (float)this.m_Texture.width,
					this.m_TextureViewRect.height / (float)this.m_Texture.height,
					50f
				}) * 0.9f;
			}
			return result;
		}

		protected void HandleZoom()
		{
			bool flag = Event.current.alt && Event.current.button == 1;
			if (flag)
			{
				EditorGUIUtility.AddCursorRect(this.m_TextureViewRect, MouseCursor.Zoom);
			}
			if (((Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown) && flag) || ((Event.current.type == EventType.KeyUp || Event.current.type == EventType.KeyDown) && Event.current.keyCode == KeyCode.LeftAlt))
			{
				base.Repaint();
			}
			if (Event.current.type == EventType.ScrollWheel || (Event.current.type == EventType.MouseDrag && Event.current.alt && Event.current.button == 1))
			{
				float num = 1f - Event.current.delta.y * ((Event.current.type != EventType.ScrollWheel) ? -0.005f : 0.03f);
				float num2 = this.m_Zoom * num;
				float num3 = Mathf.Clamp(num2, this.GetMinZoom(), 50f);
				if (num3 != this.m_Zoom)
				{
					this.m_Zoom = num3;
					if (num2 != num3)
					{
						num /= num2 / num3;
					}
					this.m_ScrollPosition *= num;
					float num4 = Event.current.mousePosition.x / this.m_TextureViewRect.width - 0.5f;
					float num5 = Event.current.mousePosition.y / this.m_TextureViewRect.height - 0.5f;
					float num6 = num4 * (num - 1f);
					float num7 = num5 * (num - 1f);
					Rect maxScrollRect = this.maxScrollRect;
					this.m_ScrollPosition.x = this.m_ScrollPosition.x + num6 * (maxScrollRect.width / 2f);
					this.m_ScrollPosition.y = this.m_ScrollPosition.y + num7 * (maxScrollRect.height / 2f);
					Event.current.Use();
				}
			}
		}

		protected void HandlePanning()
		{
			bool flag = (!Event.current.alt && Event.current.button > 0) || (Event.current.alt && Event.current.button <= 0);
			if (flag && GUIUtility.hotControl == 0)
			{
				EditorGUIUtility.AddCursorRect(this.m_TextureViewRect, MouseCursor.Pan);
				if (Event.current.type == EventType.MouseDrag)
				{
					this.m_ScrollPosition -= Event.current.delta;
					Event.current.Use();
				}
			}
			if (((Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown) && flag) || ((Event.current.type == EventType.KeyUp || Event.current.type == EventType.KeyDown) && Event.current.keyCode == KeyCode.LeftAlt))
			{
				base.Repaint();
			}
		}

		protected void DrawTexturespaceBackground()
		{
			float num = Mathf.Max(this.maxRect.width, this.maxRect.height);
			Vector2 b = new Vector2(this.maxRect.xMin, this.maxRect.yMin);
			float num2 = num * 0.5f;
			float a = (!EditorGUIUtility.isProSkin) ? 0.08f : 0.15f;
			float num3 = 8f;
			SpriteEditorUtility.BeginLines(new Color(0f, 0f, 0f, a));
			for (float num4 = 0f; num4 <= num; num4 += num3)
			{
				SpriteEditorUtility.DrawLine(new Vector2(-num2 + num4, num2 + num4) + b, new Vector2(num2 + num4, -num2 + num4) + b);
			}
			SpriteEditorUtility.EndLines();
		}

		private float Log2(float x)
		{
			return (float)(Math.Log((double)x) / Math.Log(2.0));
		}

		protected void DrawTexture()
		{
			int num = Mathf.Max(this.m_Texture.width, 1);
			float num2 = Mathf.Min(this.m_MipLevel, (float)(TextureUtil.GetMipmapCount(this.m_Texture) - 1));
			float mipMapBias = this.m_Texture.mipMapBias;
			TextureUtil.SetMipMapBiasNoDirty(this.m_Texture, num2 - this.Log2((float)num / this.m_TextureRect.width));
			FilterMode filterMode = this.m_Texture.filterMode;
			TextureUtil.SetFilterModeNoDirty(this.m_Texture, FilterMode.Point);
			if (this.m_ShowAlpha)
			{
				if (this.m_TextureAlphaOverride != null)
				{
					EditorGUI.DrawTextureTransparent(this.m_TextureRect, this.m_TextureAlphaOverride);
				}
				else
				{
					EditorGUI.DrawTextureAlpha(this.m_TextureRect, this.m_Texture);
				}
			}
			else
			{
				EditorGUI.DrawTextureTransparent(this.m_TextureRect, this.m_Texture);
			}
			TextureUtil.SetMipMapBiasNoDirty(this.m_Texture, mipMapBias);
			TextureUtil.SetFilterModeNoDirty(this.m_Texture, filterMode);
		}

		protected void DrawScreenspaceBackground()
		{
			if (Event.current.type == EventType.Repaint)
			{
				SpriteUtilityWindow.s_Styles.preBackground.Draw(this.m_TextureViewRect, false, false, false, false);
			}
		}

		protected void HandleScrollbars()
		{
			Rect position = new Rect(this.m_TextureViewRect.xMin, this.m_TextureViewRect.yMax, this.m_TextureViewRect.width, 16f);
			this.m_ScrollPosition.x = GUI.HorizontalScrollbar(position, this.m_ScrollPosition.x, this.m_TextureViewRect.width, this.maxScrollRect.xMin, this.maxScrollRect.xMax);
			Rect position2 = new Rect(this.m_TextureViewRect.xMax, this.m_TextureViewRect.yMin, 16f, this.m_TextureViewRect.height);
			this.m_ScrollPosition.y = GUI.VerticalScrollbar(position2, this.m_ScrollPosition.y, this.m_TextureViewRect.height, this.maxScrollRect.yMin, this.maxScrollRect.yMax);
		}

		protected void SetupHandlesMatrix()
		{
			Vector3 pos = new Vector3(this.m_TextureRect.x, this.m_TextureRect.yMax, 0f);
			Vector3 s = new Vector3(this.m_Zoom, -this.m_Zoom, 1f);
			Handles.matrix = Matrix4x4.TRS(pos, Quaternion.identity, s);
		}

		protected void DoAlphaZoomToolbarGUI()
		{
			this.m_ShowAlpha = GUILayout.Toggle(this.m_ShowAlpha, (!this.m_ShowAlpha) ? SpriteUtilityWindow.s_Styles.RGBIcon : SpriteUtilityWindow.s_Styles.alphaIcon, "toolbarButton", new GUILayoutOption[0]);
			this.m_Zoom = GUILayout.HorizontalSlider(this.m_Zoom, this.GetMinZoom(), 50f, SpriteUtilityWindow.s_Styles.preSlider, SpriteUtilityWindow.s_Styles.preSliderThumb, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(64f)
			});
			int num = 1;
			if (this.m_Texture != null)
			{
				num = Mathf.Max(num, TextureUtil.GetMipmapCount(this.m_Texture));
			}
			using (new EditorGUI.DisabledScope(num == 1))
			{
				GUILayout.Box(SpriteUtilityWindow.s_Styles.smallMip, SpriteUtilityWindow.s_Styles.preLabel, new GUILayoutOption[0]);
				this.m_MipLevel = Mathf.Round(GUILayout.HorizontalSlider(this.m_MipLevel, (float)(num - 1), 0f, SpriteUtilityWindow.s_Styles.preSlider, SpriteUtilityWindow.s_Styles.preSliderThumb, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(64f)
				}));
				GUILayout.Box(SpriteUtilityWindow.s_Styles.largeMip, SpriteUtilityWindow.s_Styles.preLabel, new GUILayoutOption[0]);
			}
		}

		protected void DoTextureGUI()
		{
			if (!(this.m_Texture == null))
			{
				if (this.m_Zoom < 0f)
				{
					this.m_Zoom = this.GetMinZoom();
				}
				this.m_TextureRect = new Rect(this.m_TextureViewRect.width / 2f - (float)this.m_Texture.width * this.m_Zoom / 2f, this.m_TextureViewRect.height / 2f - (float)this.m_Texture.height * this.m_Zoom / 2f, (float)this.m_Texture.width * this.m_Zoom, (float)this.m_Texture.height * this.m_Zoom);
				this.HandleScrollbars();
				this.SetupHandlesMatrix();
				this.HandleZoom();
				this.HandlePanning();
				this.DrawScreenspaceBackground();
				GUIClip.Push(this.m_TextureViewRect, -this.m_ScrollPosition, Vector2.zero, false);
				if (Event.current.type == EventType.Repaint)
				{
					this.DrawTexturespaceBackground();
					this.DrawTexture();
					this.DrawGizmos();
				}
				this.DoTextureGUIExtras();
				GUIClip.Pop();
			}
		}

		protected virtual void DoTextureGUIExtras()
		{
		}

		protected virtual void DrawGizmos()
		{
		}

		protected void SetNewTexture(Texture2D texture)
		{
			if (texture != this.m_Texture)
			{
				this.m_Texture = texture;
				this.m_Zoom = -1f;
				this.m_TextureAlphaOverride = null;
			}
		}

		protected void SetAlphaTextureOverride(Texture2D alphaTexture)
		{
			if (alphaTexture != this.m_TextureAlphaOverride)
			{
				this.m_TextureAlphaOverride = alphaTexture;
				this.m_Zoom = -1f;
			}
		}

		internal override void OnResized()
		{
			if (this.m_Texture != null && Event.current != null)
			{
				this.HandleZoom();
			}
		}
	}
}
