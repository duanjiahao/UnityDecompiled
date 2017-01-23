using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(RawImage), true)]
	public class RawImageEditor : GraphicEditor
	{
		private SerializedProperty m_Texture;

		private SerializedProperty m_UVRect;

		private GUIContent m_UVRectContent;

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_UVRectContent = new GUIContent("UV Rect");
			this.m_Texture = base.serializedObject.FindProperty("m_Texture");
			this.m_UVRect = base.serializedObject.FindProperty("m_UVRect");
			this.SetShowNativeSize(true);
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Texture, new GUILayoutOption[0]);
			base.AppearanceControlsGUI();
			base.RaycastControlsGUI();
			EditorGUILayout.PropertyField(this.m_UVRect, this.m_UVRectContent, new GUILayoutOption[0]);
			this.SetShowNativeSize(false);
			base.NativeSizeButtonGUI();
			base.serializedObject.ApplyModifiedProperties();
		}

		private void SetShowNativeSize(bool instant)
		{
			base.SetShowNativeSize(this.m_Texture.objectReferenceValue != null, instant);
		}

		private static Rect Outer(RawImage rawImage)
		{
			Rect uvRect = rawImage.uvRect;
			uvRect.xMin *= rawImage.rectTransform.rect.width;
			uvRect.xMax *= rawImage.rectTransform.rect.width;
			uvRect.yMin *= rawImage.rectTransform.rect.height;
			uvRect.yMax *= rawImage.rectTransform.rect.height;
			return uvRect;
		}

		public override bool HasPreviewGUI()
		{
			RawImage rawImage = base.target as RawImage;
			bool result;
			if (rawImage == null)
			{
				result = false;
			}
			else
			{
				Rect rect = RawImageEditor.Outer(rawImage);
				result = (rect.width > 0f && rect.height > 0f);
			}
			return result;
		}

		public override void OnPreviewGUI(Rect rect, GUIStyle background)
		{
			RawImage rawImage = base.target as RawImage;
			Texture mainTexture = rawImage.mainTexture;
			if (!(mainTexture == null))
			{
				Rect outer = RawImageEditor.Outer(rawImage);
				SpriteDrawUtility.DrawSprite(mainTexture, rect, outer, rawImage.uvRect, rawImage.canvasRenderer.GetColor());
			}
		}

		public override string GetInfoString()
		{
			RawImage rawImage = base.target as RawImage;
			return string.Format("RawImage Size: {0}x{1}", Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.width)), Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.height)));
		}
	}
}
