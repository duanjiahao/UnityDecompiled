using System;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(Image), true)]
	public class ImageEditor : GraphicEditor
	{
		private SerializedProperty m_FillMethod;

		private SerializedProperty m_FillOrigin;

		private SerializedProperty m_FillAmount;

		private SerializedProperty m_FillClockwise;

		private SerializedProperty m_Type;

		private SerializedProperty m_FillCenter;

		private SerializedProperty m_Sprite;

		private SerializedProperty m_PreserveAspect;

		private GUIContent m_SpriteContent;

		private GUIContent m_SpriteTypeContent;

		private GUIContent m_ClockwiseContent;

		private AnimBool m_ShowSlicedOrTiled;

		private AnimBool m_ShowSliced;

		private AnimBool m_ShowTiled;

		private AnimBool m_ShowFilled;

		private AnimBool m_ShowType;

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_SpriteContent = new GUIContent("Source Image");
			this.m_SpriteTypeContent = new GUIContent("Image Type");
			this.m_ClockwiseContent = new GUIContent("Clockwise");
			this.m_Sprite = base.serializedObject.FindProperty("m_Sprite");
			this.m_Type = base.serializedObject.FindProperty("m_Type");
			this.m_FillCenter = base.serializedObject.FindProperty("m_FillCenter");
			this.m_FillMethod = base.serializedObject.FindProperty("m_FillMethod");
			this.m_FillOrigin = base.serializedObject.FindProperty("m_FillOrigin");
			this.m_FillClockwise = base.serializedObject.FindProperty("m_FillClockwise");
			this.m_FillAmount = base.serializedObject.FindProperty("m_FillAmount");
			this.m_PreserveAspect = base.serializedObject.FindProperty("m_PreserveAspect");
			this.m_ShowType = new AnimBool(this.m_Sprite.objectReferenceValue != null);
			this.m_ShowType.valueChanged.AddListener(new UnityAction(base.Repaint));
			Image.Type enumValueIndex = (Image.Type)this.m_Type.enumValueIndex;
			this.m_ShowSlicedOrTiled = new AnimBool(!this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Sliced);
			this.m_ShowSliced = new AnimBool(!this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Sliced);
			this.m_ShowTiled = new AnimBool(!this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Tiled);
			this.m_ShowFilled = new AnimBool(!this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Filled);
			this.m_ShowSlicedOrTiled.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowSliced.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowTiled.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowFilled.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.SetShowNativeSize(true);
		}

		protected override void OnDisable()
		{
			this.m_ShowType.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowSlicedOrTiled.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowSliced.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowTiled.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowFilled.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.SpriteGUI();
			base.AppearanceControlsGUI();
			base.RaycastControlsGUI();
			this.m_ShowType.target = (this.m_Sprite.objectReferenceValue != null);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowType.faded))
			{
				this.TypeGUI();
			}
			EditorGUILayout.EndFadeGroup();
			this.SetShowNativeSize(false);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowNativeSize.faded))
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.m_PreserveAspect, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			base.NativeSizeButtonGUI();
			base.serializedObject.ApplyModifiedProperties();
		}

		private void SetShowNativeSize(bool instant)
		{
			Image.Type enumValueIndex = (Image.Type)this.m_Type.enumValueIndex;
			bool show = (enumValueIndex == Image.Type.Simple || enumValueIndex == Image.Type.Filled) && this.m_Sprite.objectReferenceValue != null;
			base.SetShowNativeSize(show, instant);
		}

		protected void SpriteGUI()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_Sprite, this.m_SpriteContent, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				Sprite sprite = this.m_Sprite.objectReferenceValue as Sprite;
				if (sprite)
				{
					Image.Type enumValueIndex = (Image.Type)this.m_Type.enumValueIndex;
					if (sprite.border.SqrMagnitude() > 0f)
					{
						this.m_Type.enumValueIndex = 1;
					}
					else if (enumValueIndex == Image.Type.Sliced)
					{
						this.m_Type.enumValueIndex = 0;
					}
				}
			}
		}

		protected void TypeGUI()
		{
			EditorGUILayout.PropertyField(this.m_Type, this.m_SpriteTypeContent, new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			Image.Type enumValueIndex = (Image.Type)this.m_Type.enumValueIndex;
			bool flag = !this.m_Type.hasMultipleDifferentValues && (enumValueIndex == Image.Type.Sliced || enumValueIndex == Image.Type.Tiled);
			if (flag && base.targets.Length > 1)
			{
				flag = (from obj in base.targets
				select obj as Image).All((Image img) => img.hasBorder);
			}
			this.m_ShowSlicedOrTiled.target = flag;
			this.m_ShowSliced.target = (flag && !this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Sliced);
			this.m_ShowTiled.target = (flag && !this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Tiled);
			this.m_ShowFilled.target = (!this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Filled);
			Image image = base.target as Image;
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowSlicedOrTiled.faded))
			{
				if (image.hasBorder)
				{
					EditorGUILayout.PropertyField(this.m_FillCenter, new GUILayoutOption[0]);
				}
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowSliced.faded))
			{
				if (image.sprite != null && !image.hasBorder)
				{
					EditorGUILayout.HelpBox("This Image doesn't have a border.", MessageType.Warning);
				}
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowTiled.faded))
			{
				if (image.sprite != null && !image.hasBorder && (image.sprite.texture.wrapMode != TextureWrapMode.Repeat || image.sprite.packed))
				{
					EditorGUILayout.HelpBox("It looks like you want to tile a sprite with no border. It would be more efficient to convert the Sprite to an Advanced texture, clear the Packing tag and set the Wrap mode to Repeat.", MessageType.Warning);
				}
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowFilled.faded))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.m_FillMethod, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_FillOrigin.intValue = 0;
				}
				switch (this.m_FillMethod.enumValueIndex)
				{
				case 0:
					this.m_FillOrigin.intValue = (int)((Image.OriginHorizontal)EditorGUILayout.EnumPopup("Fill Origin", (Image.OriginHorizontal)this.m_FillOrigin.intValue, new GUILayoutOption[0]));
					break;
				case 1:
					this.m_FillOrigin.intValue = (int)((Image.OriginVertical)EditorGUILayout.EnumPopup("Fill Origin", (Image.OriginVertical)this.m_FillOrigin.intValue, new GUILayoutOption[0]));
					break;
				case 2:
					this.m_FillOrigin.intValue = (int)((Image.Origin90)EditorGUILayout.EnumPopup("Fill Origin", (Image.Origin90)this.m_FillOrigin.intValue, new GUILayoutOption[0]));
					break;
				case 3:
					this.m_FillOrigin.intValue = (int)((Image.Origin180)EditorGUILayout.EnumPopup("Fill Origin", (Image.Origin180)this.m_FillOrigin.intValue, new GUILayoutOption[0]));
					break;
				case 4:
					this.m_FillOrigin.intValue = (int)((Image.Origin360)EditorGUILayout.EnumPopup("Fill Origin", (Image.Origin360)this.m_FillOrigin.intValue, new GUILayoutOption[0]));
					break;
				}
				EditorGUILayout.PropertyField(this.m_FillAmount, new GUILayoutOption[0]);
				if (this.m_FillMethod.enumValueIndex > 1)
				{
					EditorGUILayout.PropertyField(this.m_FillClockwise, this.m_ClockwiseContent, new GUILayoutOption[0]);
				}
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUI.indentLevel--;
		}

		public override bool HasPreviewGUI()
		{
			return true;
		}

		public override void OnPreviewGUI(Rect rect, GUIStyle background)
		{
			Image image = base.target as Image;
			if (!(image == null))
			{
				Sprite sprite = image.sprite;
				if (!(sprite == null))
				{
					SpriteDrawUtility.DrawSprite(sprite, rect, image.canvasRenderer.GetColor());
				}
			}
		}

		public override string GetInfoString()
		{
			Image image = base.target as Image;
			Sprite sprite = image.sprite;
			int num = (!(sprite != null)) ? 0 : Mathf.RoundToInt(sprite.rect.width);
			int num2 = (!(sprite != null)) ? 0 : Mathf.RoundToInt(sprite.rect.height);
			return string.Format("Image Size: {0}x{1}", num, num2);
		}
	}
}
