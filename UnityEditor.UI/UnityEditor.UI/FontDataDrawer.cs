using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CustomPropertyDrawer(typeof(FontData), true)]
	public class FontDataDrawer : PropertyDrawer
	{
		private static class Styles
		{
			public static GUIStyle alignmentButtonLeft;

			public static GUIStyle alignmentButtonMid;

			public static GUIStyle alignmentButtonRight;

			public static GUIContent m_EncodingContent;

			public static GUIContent m_LeftAlignText;

			public static GUIContent m_CenterAlignText;

			public static GUIContent m_RightAlignText;

			public static GUIContent m_TopAlignText;

			public static GUIContent m_MiddleAlignText;

			public static GUIContent m_BottomAlignText;

			public static GUIContent m_LeftAlignTextActive;

			public static GUIContent m_CenterAlignTextActive;

			public static GUIContent m_RightAlignTextActive;

			public static GUIContent m_TopAlignTextActive;

			public static GUIContent m_MiddleAlignTextActive;

			public static GUIContent m_BottomAlignTextActive;

			static Styles()
			{
				FontDataDrawer.Styles.alignmentButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
				FontDataDrawer.Styles.alignmentButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
				FontDataDrawer.Styles.alignmentButtonRight = new GUIStyle(EditorStyles.miniButtonRight);
				FontDataDrawer.Styles.m_EncodingContent = new GUIContent("Rich Text", "Use emoticons and colors");
				FontDataDrawer.Styles.m_LeftAlignText = EditorGUIUtility.IconContent("GUISystem/align_horizontally_left", "Left Align");
				FontDataDrawer.Styles.m_CenterAlignText = EditorGUIUtility.IconContent("GUISystem/align_horizontally_center", "Center Align");
				FontDataDrawer.Styles.m_RightAlignText = EditorGUIUtility.IconContent("GUISystem/align_horizontally_right", "Right Align");
				FontDataDrawer.Styles.m_LeftAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_horizontally_left_active", "Left Align");
				FontDataDrawer.Styles.m_CenterAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_horizontally_center_active", "Center Align");
				FontDataDrawer.Styles.m_RightAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_horizontally_right_active", "Right Align");
				FontDataDrawer.Styles.m_TopAlignText = EditorGUIUtility.IconContent("GUISystem/align_vertically_top", "Top Align");
				FontDataDrawer.Styles.m_MiddleAlignText = EditorGUIUtility.IconContent("GUISystem/align_vertically_center", "Middle Align");
				FontDataDrawer.Styles.m_BottomAlignText = EditorGUIUtility.IconContent("GUISystem/align_vertically_bottom", "Bottom Align");
				FontDataDrawer.Styles.m_TopAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_vertically_top_active", "Top Align");
				FontDataDrawer.Styles.m_MiddleAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_vertically_center_active", "Middle Align");
				FontDataDrawer.Styles.m_BottomAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_vertically_bottom_active", "Bottom Align");
				FontDataDrawer.Styles.FixAlignmentButtonStyles(new GUIStyle[]
				{
					FontDataDrawer.Styles.alignmentButtonLeft,
					FontDataDrawer.Styles.alignmentButtonMid,
					FontDataDrawer.Styles.alignmentButtonRight
				});
			}

			private static void FixAlignmentButtonStyles(params GUIStyle[] styles)
			{
				for (int i = 0; i < styles.Length; i++)
				{
					GUIStyle gUIStyle = styles[i];
					gUIStyle.padding.left = 2;
					gUIStyle.padding.right = 2;
				}
			}
		}

		private enum VerticalTextAligment
		{
			Top,
			Middle,
			Bottom
		}

		private enum HorizontalTextAligment
		{
			Left,
			Center,
			Right
		}

		private const int kAlignmentButtonWidth = 20;

		private static int s_TextAlignmentHash = "DoTextAligmentControl".GetHashCode();

		private SerializedProperty m_SupportEncoding;

		private SerializedProperty m_Font;

		private SerializedProperty m_FontSize;

		private SerializedProperty m_LineSpacing;

		private SerializedProperty m_FontStyle;

		private SerializedProperty m_ResizeTextForBestFit;

		private SerializedProperty m_ResizeTextMinSize;

		private SerializedProperty m_ResizeTextMaxSize;

		private SerializedProperty m_HorizontalOverflow;

		private SerializedProperty m_VerticalOverflow;

		private SerializedProperty m_Alignment;

		private SerializedProperty m_AlignByGeometry;

		private float m_FontFieldfHeight = 0f;

		private float m_FontStyleHeight = 0f;

		private float m_FontSizeHeight = 0f;

		private float m_LineSpacingHeight = 0f;

		private float m_EncodingHeight = 0f;

		private float m_ResizeTextForBestFitHeight = 0f;

		private float m_ResizeTextMinSizeHeight = 0f;

		private float m_ResizeTextMaxSizeHeight = 0f;

		private float m_HorizontalOverflowHeight = 0f;

		private float m_VerticalOverflowHeight = 0f;

		private float m_AlignByGeometryHeight = 0f;

		protected void Init(SerializedProperty property)
		{
			this.m_SupportEncoding = property.FindPropertyRelative("m_RichText");
			this.m_Font = property.FindPropertyRelative("m_Font");
			this.m_FontSize = property.FindPropertyRelative("m_FontSize");
			this.m_LineSpacing = property.FindPropertyRelative("m_LineSpacing");
			this.m_FontStyle = property.FindPropertyRelative("m_FontStyle");
			this.m_ResizeTextForBestFit = property.FindPropertyRelative("m_BestFit");
			this.m_ResizeTextMinSize = property.FindPropertyRelative("m_MinSize");
			this.m_ResizeTextMaxSize = property.FindPropertyRelative("m_MaxSize");
			this.m_HorizontalOverflow = property.FindPropertyRelative("m_HorizontalOverflow");
			this.m_VerticalOverflow = property.FindPropertyRelative("m_VerticalOverflow");
			this.m_Alignment = property.FindPropertyRelative("m_Alignment");
			this.m_AlignByGeometry = property.FindPropertyRelative("m_AlignByGeometry");
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			this.Init(property);
			this.m_FontFieldfHeight = EditorGUI.GetPropertyHeight(this.m_Font);
			this.m_FontStyleHeight = EditorGUI.GetPropertyHeight(this.m_FontStyle);
			this.m_FontSizeHeight = EditorGUI.GetPropertyHeight(this.m_FontSize);
			this.m_LineSpacingHeight = EditorGUI.GetPropertyHeight(this.m_LineSpacing);
			this.m_EncodingHeight = EditorGUI.GetPropertyHeight(this.m_SupportEncoding);
			this.m_ResizeTextForBestFitHeight = EditorGUI.GetPropertyHeight(this.m_ResizeTextForBestFit);
			this.m_ResizeTextMinSizeHeight = EditorGUI.GetPropertyHeight(this.m_ResizeTextMinSize);
			this.m_ResizeTextMaxSizeHeight = EditorGUI.GetPropertyHeight(this.m_ResizeTextMaxSize);
			this.m_HorizontalOverflowHeight = EditorGUI.GetPropertyHeight(this.m_HorizontalOverflow);
			this.m_VerticalOverflowHeight = EditorGUI.GetPropertyHeight(this.m_VerticalOverflow);
			this.m_AlignByGeometryHeight = EditorGUI.GetPropertyHeight(this.m_AlignByGeometry);
			float num = this.m_FontFieldfHeight + this.m_FontStyleHeight + this.m_FontSizeHeight + this.m_LineSpacingHeight + this.m_EncodingHeight + this.m_ResizeTextForBestFitHeight + this.m_HorizontalOverflowHeight + this.m_VerticalOverflowHeight + EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 10f + this.m_AlignByGeometryHeight;
			if (this.m_ResizeTextForBestFit.boolValue)
			{
				num += this.m_ResizeTextMinSizeHeight + this.m_ResizeTextMaxSizeHeight + EditorGUIUtility.standardVerticalSpacing * 2f;
			}
			return num;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			this.Init(property);
			Rect position2 = position;
			position2.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField(position2, "Character", EditorStyles.boldLabel);
			position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.indentLevel++;
			Font font = this.m_Font.objectReferenceValue as Font;
			position2.height = this.m_FontFieldfHeight;
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position2, this.m_Font);
			if (EditorGUI.EndChangeCheck())
			{
				font = (this.m_Font.objectReferenceValue as Font);
				if (font != null && !font.dynamic)
				{
					this.m_FontSize.intValue = font.fontSize;
				}
			}
			position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
			position2.height = this.m_FontStyleHeight;
			using (new EditorGUI.DisabledScope(!this.m_Font.hasMultipleDifferentValues && font != null && !font.dynamic))
			{
				EditorGUI.PropertyField(position2, this.m_FontStyle);
			}
			position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
			position2.height = this.m_FontSizeHeight;
			EditorGUI.PropertyField(position2, this.m_FontSize);
			position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
			position2.height = this.m_LineSpacingHeight;
			EditorGUI.PropertyField(position2, this.m_LineSpacing);
			position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
			position2.height = this.m_EncodingHeight;
			EditorGUI.PropertyField(position2, this.m_SupportEncoding, FontDataDrawer.Styles.m_EncodingContent);
			EditorGUI.indentLevel--;
			position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
			position2.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField(position2, "Paragraph", EditorStyles.boldLabel);
			position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.indentLevel++;
			position2.height = EditorGUIUtility.singleLineHeight;
			this.DoTextAligmentControl(position2, this.m_Alignment);
			position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
			position2.height = this.m_HorizontalOverflowHeight;
			EditorGUI.PropertyField(position2, this.m_AlignByGeometry);
			position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
			position2.height = this.m_HorizontalOverflowHeight;
			EditorGUI.PropertyField(position2, this.m_HorizontalOverflow);
			position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
			position2.height = this.m_VerticalOverflowHeight;
			EditorGUI.PropertyField(position2, this.m_VerticalOverflow);
			position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
			position2.height = this.m_ResizeTextMaxSizeHeight;
			EditorGUI.PropertyField(position2, this.m_ResizeTextForBestFit);
			if (this.m_ResizeTextForBestFit.boolValue)
			{
				EditorGUILayout.EndFadeGroup();
				position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
				position2.height = this.m_ResizeTextMinSizeHeight;
				EditorGUI.PropertyField(position2, this.m_ResizeTextMinSize);
				position2.y += position2.height + EditorGUIUtility.standardVerticalSpacing;
				position2.height = this.m_ResizeTextMaxSizeHeight;
				EditorGUI.PropertyField(position2, this.m_ResizeTextMaxSize);
			}
			EditorGUI.indentLevel--;
		}

		private void DoTextAligmentControl(Rect position, SerializedProperty alignment)
		{
			GUIContent label = new GUIContent("Alignment");
			int controlID = GUIUtility.GetControlID(FontDataDrawer.s_TextAlignmentHash, FocusType.Keyboard, position);
			EditorGUIUtility.SetIconSize(new Vector2(15f, 15f));
			EditorGUI.BeginProperty(position, label, alignment);
			Rect rect = EditorGUI.PrefixLabel(position, controlID, label);
			float num = 60f;
			float num2 = Mathf.Clamp(rect.width - num * 2f, 2f, 10f);
			Rect position2 = new Rect(rect.x, rect.y, num, rect.height);
			Rect position3 = new Rect(position2.xMax + num2, rect.y, num, rect.height);
			FontDataDrawer.DoHorizontalAligmentControl(position2, alignment);
			FontDataDrawer.DoVerticalAligmentControl(position3, alignment);
			EditorGUI.EndProperty();
			EditorGUIUtility.SetIconSize(Vector2.zero);
		}

		private static void DoHorizontalAligmentControl(Rect position, SerializedProperty alignment)
		{
			TextAnchor intValue = (TextAnchor)alignment.intValue;
			FontDataDrawer.HorizontalTextAligment horizontalAlignment = FontDataDrawer.GetHorizontalAlignment(intValue);
			bool flag = horizontalAlignment == FontDataDrawer.HorizontalTextAligment.Left;
			bool flag2 = horizontalAlignment == FontDataDrawer.HorizontalTextAligment.Center;
			bool flag3 = horizontalAlignment == FontDataDrawer.HorizontalTextAligment.Right;
			if (alignment.hasMultipleDifferentValues)
			{
				UnityEngine.Object[] targetObjects = alignment.serializedObject.targetObjects;
				for (int i = 0; i < targetObjects.Length; i++)
				{
					UnityEngine.Object @object = targetObjects[i];
					Text text = @object as Text;
					horizontalAlignment = FontDataDrawer.GetHorizontalAlignment(text.alignment);
					flag = (flag || horizontalAlignment == FontDataDrawer.HorizontalTextAligment.Left);
					flag2 = (flag2 || horizontalAlignment == FontDataDrawer.HorizontalTextAligment.Center);
					flag3 = (flag3 || horizontalAlignment == FontDataDrawer.HorizontalTextAligment.Right);
				}
			}
			position.width = 20f;
			EditorGUI.BeginChangeCheck();
			FontDataDrawer.EditorToggle(position, flag, (!flag) ? FontDataDrawer.Styles.m_LeftAlignText : FontDataDrawer.Styles.m_LeftAlignTextActive, FontDataDrawer.Styles.alignmentButtonLeft);
			if (EditorGUI.EndChangeCheck())
			{
				FontDataDrawer.SetHorizontalAlignment(alignment, FontDataDrawer.HorizontalTextAligment.Left);
			}
			position.x += position.width;
			EditorGUI.BeginChangeCheck();
			FontDataDrawer.EditorToggle(position, flag2, (!flag2) ? FontDataDrawer.Styles.m_CenterAlignText : FontDataDrawer.Styles.m_CenterAlignTextActive, FontDataDrawer.Styles.alignmentButtonMid);
			if (EditorGUI.EndChangeCheck())
			{
				FontDataDrawer.SetHorizontalAlignment(alignment, FontDataDrawer.HorizontalTextAligment.Center);
			}
			position.x += position.width;
			EditorGUI.BeginChangeCheck();
			FontDataDrawer.EditorToggle(position, flag3, (!flag3) ? FontDataDrawer.Styles.m_RightAlignText : FontDataDrawer.Styles.m_RightAlignTextActive, FontDataDrawer.Styles.alignmentButtonRight);
			if (EditorGUI.EndChangeCheck())
			{
				FontDataDrawer.SetHorizontalAlignment(alignment, FontDataDrawer.HorizontalTextAligment.Right);
			}
		}

		private static void DoVerticalAligmentControl(Rect position, SerializedProperty alignment)
		{
			TextAnchor intValue = (TextAnchor)alignment.intValue;
			FontDataDrawer.VerticalTextAligment verticalAlignment = FontDataDrawer.GetVerticalAlignment(intValue);
			bool flag = verticalAlignment == FontDataDrawer.VerticalTextAligment.Top;
			bool flag2 = verticalAlignment == FontDataDrawer.VerticalTextAligment.Middle;
			bool flag3 = verticalAlignment == FontDataDrawer.VerticalTextAligment.Bottom;
			if (alignment.hasMultipleDifferentValues)
			{
				UnityEngine.Object[] targetObjects = alignment.serializedObject.targetObjects;
				for (int i = 0; i < targetObjects.Length; i++)
				{
					UnityEngine.Object @object = targetObjects[i];
					Text text = @object as Text;
					TextAnchor alignment2 = text.alignment;
					verticalAlignment = FontDataDrawer.GetVerticalAlignment(alignment2);
					flag = (flag || verticalAlignment == FontDataDrawer.VerticalTextAligment.Top);
					flag2 = (flag2 || verticalAlignment == FontDataDrawer.VerticalTextAligment.Middle);
					flag3 = (flag3 || verticalAlignment == FontDataDrawer.VerticalTextAligment.Bottom);
				}
			}
			position.width = 20f;
			EditorGUI.BeginChangeCheck();
			FontDataDrawer.EditorToggle(position, flag, (!flag) ? FontDataDrawer.Styles.m_TopAlignText : FontDataDrawer.Styles.m_TopAlignTextActive, FontDataDrawer.Styles.alignmentButtonLeft);
			if (EditorGUI.EndChangeCheck())
			{
				FontDataDrawer.SetVerticalAlignment(alignment, FontDataDrawer.VerticalTextAligment.Top);
			}
			position.x += position.width;
			EditorGUI.BeginChangeCheck();
			FontDataDrawer.EditorToggle(position, flag2, (!flag2) ? FontDataDrawer.Styles.m_MiddleAlignText : FontDataDrawer.Styles.m_MiddleAlignTextActive, FontDataDrawer.Styles.alignmentButtonMid);
			if (EditorGUI.EndChangeCheck())
			{
				FontDataDrawer.SetVerticalAlignment(alignment, FontDataDrawer.VerticalTextAligment.Middle);
			}
			position.x += position.width;
			EditorGUI.BeginChangeCheck();
			FontDataDrawer.EditorToggle(position, flag3, (!flag3) ? FontDataDrawer.Styles.m_BottomAlignText : FontDataDrawer.Styles.m_BottomAlignTextActive, FontDataDrawer.Styles.alignmentButtonRight);
			if (EditorGUI.EndChangeCheck())
			{
				FontDataDrawer.SetVerticalAlignment(alignment, FontDataDrawer.VerticalTextAligment.Bottom);
			}
		}

		private static bool EditorToggle(Rect position, bool value, GUIContent content, GUIStyle style)
		{
			int hashCode = "AlignToggle".GetHashCode();
			int controlID = GUIUtility.GetControlID(hashCode, FocusType.Keyboard, position);
			Event current = Event.current;
			if (GUIUtility.keyboardControl == controlID && current.type == EventType.KeyDown && (current.keyCode == KeyCode.Space || current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter))
			{
				value = !value;
				current.Use();
				GUI.changed = true;
			}
			if (current.type == EventType.KeyDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition))
			{
				GUIUtility.keyboardControl = controlID;
				EditorGUIUtility.editingTextField = false;
				HandleUtility.Repaint();
			}
			return GUI.Toggle(position, controlID, value, content, style);
		}

		private static FontDataDrawer.HorizontalTextAligment GetHorizontalAlignment(TextAnchor ta)
		{
			FontDataDrawer.HorizontalTextAligment result;
			switch (ta)
			{
			case TextAnchor.UpperLeft:
			case TextAnchor.MiddleLeft:
			case TextAnchor.LowerLeft:
				result = FontDataDrawer.HorizontalTextAligment.Left;
				break;
			case TextAnchor.UpperCenter:
			case TextAnchor.MiddleCenter:
			case TextAnchor.LowerCenter:
				result = FontDataDrawer.HorizontalTextAligment.Center;
				break;
			case TextAnchor.UpperRight:
			case TextAnchor.MiddleRight:
			case TextAnchor.LowerRight:
				result = FontDataDrawer.HorizontalTextAligment.Right;
				break;
			default:
				result = FontDataDrawer.HorizontalTextAligment.Left;
				break;
			}
			return result;
		}

		private static FontDataDrawer.VerticalTextAligment GetVerticalAlignment(TextAnchor ta)
		{
			FontDataDrawer.VerticalTextAligment result;
			switch (ta)
			{
			case TextAnchor.UpperLeft:
			case TextAnchor.UpperCenter:
			case TextAnchor.UpperRight:
				result = FontDataDrawer.VerticalTextAligment.Top;
				break;
			case TextAnchor.MiddleLeft:
			case TextAnchor.MiddleCenter:
			case TextAnchor.MiddleRight:
				result = FontDataDrawer.VerticalTextAligment.Middle;
				break;
			case TextAnchor.LowerLeft:
			case TextAnchor.LowerCenter:
			case TextAnchor.LowerRight:
				result = FontDataDrawer.VerticalTextAligment.Bottom;
				break;
			default:
				result = FontDataDrawer.VerticalTextAligment.Top;
				break;
			}
			return result;
		}

		private static void SetHorizontalAlignment(SerializedProperty alignment, FontDataDrawer.HorizontalTextAligment horizontalAlignment)
		{
			UnityEngine.Object[] targetObjects = alignment.serializedObject.targetObjects;
			for (int i = 0; i < targetObjects.Length; i++)
			{
				UnityEngine.Object @object = targetObjects[i];
				Text text = @object as Text;
				FontDataDrawer.VerticalTextAligment verticalAlignment = FontDataDrawer.GetVerticalAlignment(text.alignment);
				Undo.RecordObject(text, "Horizontal Alignment");
				text.alignment = FontDataDrawer.GetAnchor(verticalAlignment, horizontalAlignment);
				EditorUtility.SetDirty(@object);
			}
		}

		private static void SetVerticalAlignment(SerializedProperty alignment, FontDataDrawer.VerticalTextAligment verticalAlignment)
		{
			UnityEngine.Object[] targetObjects = alignment.serializedObject.targetObjects;
			for (int i = 0; i < targetObjects.Length; i++)
			{
				UnityEngine.Object @object = targetObjects[i];
				Text text = @object as Text;
				FontDataDrawer.HorizontalTextAligment horizontalAlignment = FontDataDrawer.GetHorizontalAlignment(text.alignment);
				Undo.RecordObject(text, "Vertical Alignment");
				text.alignment = FontDataDrawer.GetAnchor(verticalAlignment, horizontalAlignment);
				EditorUtility.SetDirty(@object);
			}
		}

		private static TextAnchor GetAnchor(FontDataDrawer.VerticalTextAligment verticalTextAligment, FontDataDrawer.HorizontalTextAligment horizontalTextAligment)
		{
			TextAnchor result;
			if (horizontalTextAligment != FontDataDrawer.HorizontalTextAligment.Left)
			{
				if (horizontalTextAligment != FontDataDrawer.HorizontalTextAligment.Center)
				{
					if (verticalTextAligment != FontDataDrawer.VerticalTextAligment.Bottom)
					{
						if (verticalTextAligment != FontDataDrawer.VerticalTextAligment.Middle)
						{
							result = TextAnchor.UpperRight;
						}
						else
						{
							result = TextAnchor.MiddleRight;
						}
					}
					else
					{
						result = TextAnchor.LowerRight;
					}
				}
				else if (verticalTextAligment != FontDataDrawer.VerticalTextAligment.Bottom)
				{
					if (verticalTextAligment != FontDataDrawer.VerticalTextAligment.Middle)
					{
						result = TextAnchor.UpperCenter;
					}
					else
					{
						result = TextAnchor.MiddleCenter;
					}
				}
				else
				{
					result = TextAnchor.LowerCenter;
				}
			}
			else if (verticalTextAligment != FontDataDrawer.VerticalTextAligment.Bottom)
			{
				if (verticalTextAligment != FontDataDrawer.VerticalTextAligment.Middle)
				{
					result = TextAnchor.UpperLeft;
				}
				else
				{
					result = TextAnchor.MiddleLeft;
				}
			}
			else
			{
				result = TextAnchor.LowerLeft;
			}
			return result;
		}
	}
}
