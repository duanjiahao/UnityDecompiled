using System;
using UnityEngine;
using UnityEngine.Sprites;

namespace UnityEditor.UI
{
	internal class SpriteDrawUtility
	{
		private static Texture2D s_ContrastTex;

		private static Texture2D contrastTexture
		{
			get
			{
				if (SpriteDrawUtility.s_ContrastTex == null)
				{
					SpriteDrawUtility.s_ContrastTex = SpriteDrawUtility.CreateCheckerTex(new Color(0f, 0f, 0f, 0.5f), new Color(1f, 1f, 1f, 0.5f));
				}
				return SpriteDrawUtility.s_ContrastTex;
			}
		}

		private static Texture2D CreateCheckerTex(Color c0, Color c1)
		{
			Texture2D texture2D = new Texture2D(16, 16);
			texture2D.name = "[Generated] Checker Texture";
			texture2D.hideFlags = HideFlags.DontSave;
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					texture2D.SetPixel(j, i, c1);
				}
			}
			for (int k = 8; k < 16; k++)
			{
				for (int l = 0; l < 8; l++)
				{
					texture2D.SetPixel(l, k, c0);
				}
			}
			for (int m = 0; m < 8; m++)
			{
				for (int n = 8; n < 16; n++)
				{
					texture2D.SetPixel(n, m, c0);
				}
			}
			for (int num = 8; num < 16; num++)
			{
				for (int num2 = 8; num2 < 16; num2++)
				{
					texture2D.SetPixel(num2, num, c1);
				}
			}
			texture2D.Apply();
			texture2D.filterMode = FilterMode.Point;
			return texture2D;
		}

		private static Texture2D CreateGradientTex()
		{
			Texture2D texture2D = new Texture2D(1, 16);
			texture2D.name = "[Generated] Gradient Texture";
			texture2D.hideFlags = HideFlags.DontSave;
			Color a = new Color(1f, 1f, 1f, 0f);
			Color b = new Color(1f, 1f, 1f, 0.4f);
			for (int i = 0; i < 16; i++)
			{
				float num = Mathf.Abs((float)i / 15f * 2f - 1f);
				num *= num;
				texture2D.SetPixel(0, i, Color.Lerp(a, b, num));
			}
			texture2D.Apply();
			texture2D.filterMode = FilterMode.Bilinear;
			return texture2D;
		}

		private static void DrawTiledTexture(Rect rect, Texture tex)
		{
			float width = rect.width / (float)tex.width;
			float height = rect.height / (float)tex.height;
			Rect texCoords = new Rect(0f, 0f, width, height);
			TextureWrapMode wrapMode = tex.wrapMode;
			tex.wrapMode = TextureWrapMode.Repeat;
			GUI.DrawTextureWithTexCoords(rect, tex, texCoords);
			tex.wrapMode = wrapMode;
		}

		public static void DrawSprite(Sprite sprite, Rect drawArea, Color color)
		{
			if (!(sprite == null))
			{
				Texture2D texture = sprite.texture;
				if (!(texture == null))
				{
					Rect rect = sprite.rect;
					Rect inner = rect;
					inner.xMin += sprite.border.x;
					inner.yMin += sprite.border.y;
					inner.xMax -= sprite.border.z;
					inner.yMax -= sprite.border.w;
					Vector4 outerUV = DataUtility.GetOuterUV(sprite);
					Rect uv = new Rect(outerUV.x, outerUV.y, outerUV.z - outerUV.x, outerUV.w - outerUV.y);
					Vector4 padding = DataUtility.GetPadding(sprite);
					padding.x /= rect.width;
					padding.y /= rect.height;
					padding.z /= rect.width;
					padding.w /= rect.height;
					SpriteDrawUtility.DrawSprite(texture, drawArea, padding, rect, inner, uv, color, null);
				}
			}
		}

		public static void DrawSprite(Texture tex, Rect drawArea, Rect outer, Rect uv, Color color)
		{
			SpriteDrawUtility.DrawSprite(tex, drawArea, Vector4.zero, outer, outer, uv, color, null);
		}

		private static void DrawSprite(Texture tex, Rect drawArea, Vector4 padding, Rect outer, Rect inner, Rect uv, Color color, Material mat)
		{
			Rect position = drawArea;
			position.width = Mathf.Abs(outer.width);
			position.height = Mathf.Abs(outer.height);
			if (position.width > 0f)
			{
				float num = drawArea.width / position.width;
				position.width *= num;
				position.height *= num;
			}
			if (drawArea.height > position.height)
			{
				position.y += (drawArea.height - position.height) * 0.5f;
			}
			else if (position.height > drawArea.height)
			{
				float num2 = drawArea.height / position.height;
				position.width *= num2;
				position.height *= num2;
			}
			if (drawArea.width > position.width)
			{
				position.x += (drawArea.width - position.width) * 0.5f;
			}
			EditorGUI.DrawTextureTransparent(position, null, ScaleMode.ScaleToFit, outer.width / outer.height);
			GUI.color = color;
			Rect position2 = new Rect(position.x + position.width * padding.x, position.y + position.height * padding.w, position.width - position.width * (padding.z + padding.x), position.height - position.height * (padding.w + padding.y));
			if (mat == null)
			{
				GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
				GUI.DrawTextureWithTexCoords(position2, tex, uv, true);
				GL.sRGBWrite = false;
			}
			else
			{
				EditorGUI.DrawPreviewTexture(position2, tex, mat);
			}
			GUI.BeginGroup(position);
			tex = SpriteDrawUtility.contrastTexture;
			GUI.color = Color.white;
			if (inner.xMin != outer.xMin)
			{
				float x = (inner.xMin - outer.xMin) / outer.width * position.width - 1f;
				SpriteDrawUtility.DrawTiledTexture(new Rect(x, 0f, 1f, position.height), tex);
			}
			if (inner.xMax != outer.xMax)
			{
				float x2 = (inner.xMax - outer.xMin) / outer.width * position.width - 1f;
				SpriteDrawUtility.DrawTiledTexture(new Rect(x2, 0f, 1f, position.height), tex);
			}
			if (inner.yMin != outer.yMin)
			{
				float num3 = (inner.yMin - outer.yMin) / outer.height * position.height - 1f;
				SpriteDrawUtility.DrawTiledTexture(new Rect(0f, position.height - num3, position.width, 1f), tex);
			}
			if (inner.yMax != outer.yMax)
			{
				float num4 = (inner.yMax - outer.yMin) / outer.height * position.height - 1f;
				SpriteDrawUtility.DrawTiledTexture(new Rect(0f, position.height - num4, position.width, 1f), tex);
			}
			GUI.EndGroup();
		}
	}
}
