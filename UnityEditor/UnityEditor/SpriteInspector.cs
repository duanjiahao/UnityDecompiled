using System;
using UnityEditor.Sprites;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Sprites;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Sprite))]
	internal class SpriteInspector : Editor
	{
		public readonly GUIContent[] spriteAlignmentOptions = new GUIContent[]
		{
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.Center"),
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.TopLeft"),
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.Top"),
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.TopRight"),
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.Left"),
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.Right"),
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.BottomLeft"),
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.Bottom"),
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.BottomRight"),
			EditorGUIUtility.TextContent("SpriteInspector.Pivot.Custom")
		};
		public readonly GUIContent spriteAlignment = EditorGUIUtility.TextContent("SpriteInspector.Pivot");
		private Sprite sprite
		{
			get
			{
				return this.target as Sprite;
			}
		}
		private SpriteMetaData GetMetaData(string name)
		{
			string assetPath = AssetDatabase.GetAssetPath(this.sprite);
			TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if (!(textureImporter != null))
			{
				return default(SpriteMetaData);
			}
			if (textureImporter.spriteImportMode == SpriteImportMode.Single)
			{
				return SpriteInspector.GetMetaDataInSingleMode(name, textureImporter);
			}
			return SpriteInspector.GetMetaDataInMultipleMode(name, textureImporter);
		}
		private static SpriteMetaData GetMetaDataInMultipleMode(string name, TextureImporter textureImporter)
		{
			SpriteMetaData[] spritesheet = textureImporter.spritesheet;
			for (int i = 0; i < spritesheet.Length; i++)
			{
				if (spritesheet[i].name.Equals(name))
				{
					return spritesheet[i];
				}
			}
			return default(SpriteMetaData);
		}
		private static SpriteMetaData GetMetaDataInSingleMode(string name, TextureImporter textureImporter)
		{
			SpriteMetaData result = default(SpriteMetaData);
			result.border = textureImporter.spriteBorder;
			result.name = name;
			result.pivot = textureImporter.spritePivot;
			result.rect = new Rect(0f, 0f, 1f, 1f);
			TextureImporterSettings textureImporterSettings = new TextureImporterSettings();
			textureImporter.ReadTextureSettings(textureImporterSettings);
			result.alignment = textureImporterSettings.spriteAlignment;
			return result;
		}
		public override void OnInspectorGUI()
		{
			bool flag;
			bool flag2;
			bool flag3;
			this.UnifiedValues(out flag, out flag2, out flag3);
			if (flag)
			{
				EditorGUILayout.LabelField("Name", this.sprite.name, new GUILayoutOption[0]);
			}
			else
			{
				EditorGUILayout.LabelField("Name", "-", new GUILayoutOption[0]);
			}
			if (flag2)
			{
				int alignment = this.GetMetaData(this.sprite.name).alignment;
				EditorGUILayout.LabelField(this.spriteAlignment, this.spriteAlignmentOptions[alignment], new GUILayoutOption[0]);
			}
			else
			{
				EditorGUILayout.LabelField(this.spriteAlignment.text, "-", new GUILayoutOption[0]);
			}
			if (flag3)
			{
				Vector4 border = this.GetMetaData(this.sprite.name).border;
				EditorGUILayout.LabelField("Border", string.Format("L:{0} B:{1} R:{2} T:{3}", new object[]
				{
					border.x,
					border.y,
					border.z,
					border.w
				}), new GUILayoutOption[0]);
			}
			else
			{
				EditorGUILayout.LabelField("Border", "-", new GUILayoutOption[0]);
			}
		}
		private void UnifiedValues(out bool name, out bool alignment, out bool border)
		{
			name = true;
			alignment = true;
			border = true;
			if (base.targets.Length < 2)
			{
				return;
			}
			string assetPath = AssetDatabase.GetAssetPath(this.sprite);
			TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			SpriteMetaData[] spritesheet = textureImporter.spritesheet;
			string text = null;
			int num = -1;
			Vector4? vector = null;
			for (int i = 0; i < base.targets.Length; i++)
			{
				Sprite sprite = base.targets[i] as Sprite;
				for (int j = 0; j < spritesheet.Length; j++)
				{
					if (spritesheet[j].name.Equals(sprite.name))
					{
						if (spritesheet[j].alignment != num && num > 0)
						{
							alignment = false;
						}
						else
						{
							num = spritesheet[j].alignment;
						}
						if (spritesheet[j].name != text && text != null)
						{
							name = false;
						}
						else
						{
							text = spritesheet[j].name;
						}
						if (spritesheet[j].border != vector && vector.HasValue)
						{
							border = false;
						}
						else
						{
							vector = new Vector4?(spritesheet[j].border);
						}
					}
				}
			}
		}
		private static Texture2D BuildPreviewTexture(int width, int height, Sprite sprite, Material spriteRendererMaterial)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return null;
			}
			float width2 = sprite.rect.width;
			float height2 = sprite.rect.height;
			Texture2D spriteTexture = UnityEditor.Sprites.DataUtility.GetSpriteTexture(sprite, false);
			PreviewHelpers.AdjustWidthAndHeightForStaticPreview((int)width2, (int)height2, ref width, ref height);
			EditorUtility.SetTemporarilyAllowIndieRenderTexture(true);
			SavedRenderTargetState savedRenderTargetState = new SavedRenderTargetState();
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
			RenderTexture.active = temporary;
			GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
			GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
			Texture texture = null;
			Vector4 vector = new Vector4(0f, 0f, 0f, 0f);
			bool flag = false;
			bool flag2 = false;
			if (spriteRendererMaterial != null)
			{
				flag = spriteRendererMaterial.HasProperty("_MainTex");
				flag2 = spriteRendererMaterial.HasProperty("_MainTex_TexelSize");
			}
			Material material = null;
			if (spriteRendererMaterial != null)
			{
				if (flag)
				{
					texture = spriteRendererMaterial.GetTexture("_MainTex");
					spriteRendererMaterial.SetTexture("_MainTex", spriteTexture);
				}
				if (flag2)
				{
					vector = spriteRendererMaterial.GetVector("_MainTex_TexelSize");
					spriteRendererMaterial.SetVector("_MainTex_TexelSize", TextureUtil.GetTexelSizeVector(spriteTexture));
				}
				spriteRendererMaterial.SetPass(0);
			}
			else
			{
				material = new Material(Shader.Find("Hidden/BlitCopy"));
				material.mainTexture = spriteTexture;
				material.SetPass(0);
			}
			Vector4 spriteOuterUV = InternalEditorUtility.GetSpriteOuterUV(sprite, false);
			Vector4 padding = UnityEngine.Sprites.DataUtility.GetPadding(sprite);
			Rect textureRect = sprite.textureRect;
			float width3 = textureRect.width;
			float height3 = textureRect.height;
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Begin(7);
			GL.Color(new Color(1f, 1f, 1f, 1f));
			GL.TexCoord(new Vector3(spriteOuterUV.x, spriteOuterUV.w, 0f));
			GL.Vertex3(padding.x / width2, (padding.y + height3) / height2, 0f);
			GL.TexCoord(new Vector3(spriteOuterUV.z, spriteOuterUV.w, 0f));
			GL.Vertex3((padding.x + width3) / width2, (padding.y + height3) / height2, 0f);
			GL.TexCoord(new Vector3(spriteOuterUV.z, spriteOuterUV.y, 0f));
			GL.Vertex3((padding.x + width3) / width2, padding.y / height2, 0f);
			GL.TexCoord(new Vector3(spriteOuterUV.x, spriteOuterUV.y, 0f));
			GL.Vertex3(padding.x / width2, padding.y / height2, 0f);
			GL.End();
			GL.PopMatrix();
			GL.sRGBWrite = false;
			if (spriteRendererMaterial != null)
			{
				if (flag)
				{
					spriteRendererMaterial.SetTexture("_MainTex", texture);
				}
				if (flag2)
				{
					spriteRendererMaterial.SetVector("_MainTex_TexelSize", vector);
				}
			}
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
			texture2D.hideFlags = HideFlags.HideAndDontSave;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
			texture2D.Apply();
			RenderTexture.ReleaseTemporary(temporary);
			savedRenderTargetState.Restore();
			EditorUtility.SetTemporarilyAllowIndieRenderTexture(false);
			if (material != null)
			{
				UnityEngine.Object.DestroyImmediate(material);
			}
			return texture2D;
		}
		public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
		{
			return SpriteInspector.BuildPreviewTexture(width, height, this.sprite, null);
		}
		public override bool HasPreviewGUI()
		{
			return this.target != null;
		}
		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (this.target == null)
			{
				return;
			}
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			SpriteInspector.DrawPreview(r, this.sprite, null);
		}
		public static void DrawPreview(Rect r, Sprite frame, Material spriteRendererMaterial)
		{
			if (frame == null)
			{
				return;
			}
			float num = Mathf.Min(r.width / frame.rect.width, r.height / frame.rect.height);
			Rect position = new Rect(r.x, r.y, frame.rect.width * num, frame.rect.height * num);
			position.center = r.center;
			Texture2D texture2D = SpriteInspector.BuildPreviewTexture((int)position.width, (int)position.height, frame, spriteRendererMaterial);
			EditorGUI.DrawTextureTransparent(position, texture2D, ScaleMode.ScaleToFit);
			Vector4 border = frame.border;
			if (!Mathf.Approximately((border * num).sqrMagnitude, 0f))
			{
				SpriteEditorUtility.BeginLines(new Color(0f, 1f, 0f, 0.7f));
				SpriteEditorUtility.EndLines();
			}
			UnityEngine.Object.DestroyImmediate(texture2D);
		}
		public override string GetInfoString()
		{
			if (this.target == null)
			{
				return string.Empty;
			}
			Sprite sprite = this.target as Sprite;
			return string.Format("({0}x{1})", (int)sprite.rect.width, (int)sprite.rect.height);
		}
	}
}
