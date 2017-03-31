using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;

namespace UnityEditor
{
	public sealed class EditorGUILayout
	{
		public class ToggleGroupScope : GUI.Scope
		{
			public bool enabled
			{
				get;
				protected set;
			}

			public ToggleGroupScope(string label, bool toggle)
			{
				this.enabled = EditorGUILayout.BeginToggleGroup(label, toggle);
			}

			public ToggleGroupScope(GUIContent label, bool toggle)
			{
				this.enabled = EditorGUILayout.BeginToggleGroup(label, toggle);
			}

			protected override void CloseScope()
			{
				EditorGUILayout.EndToggleGroup();
			}
		}

		public class HorizontalScope : GUI.Scope
		{
			public Rect rect
			{
				get;
				protected set;
			}

			public HorizontalScope(params GUILayoutOption[] options)
			{
				this.rect = EditorGUILayout.BeginHorizontal(options);
			}

			public HorizontalScope(GUIStyle style, params GUILayoutOption[] options)
			{
				this.rect = EditorGUILayout.BeginHorizontal(style, options);
			}

			internal HorizontalScope(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
			{
				this.rect = EditorGUILayout.BeginHorizontal(content, style, options);
			}

			protected override void CloseScope()
			{
				EditorGUILayout.EndHorizontal();
			}
		}

		public class VerticalScope : GUI.Scope
		{
			public Rect rect
			{
				get;
				protected set;
			}

			public VerticalScope(params GUILayoutOption[] options)
			{
				this.rect = EditorGUILayout.BeginVertical(options);
			}

			public VerticalScope(GUIStyle style, params GUILayoutOption[] options)
			{
				this.rect = EditorGUILayout.BeginVertical(style, options);
			}

			internal VerticalScope(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
			{
				this.rect = EditorGUILayout.BeginVertical(content, style, options);
			}

			protected override void CloseScope()
			{
				EditorGUILayout.EndVertical();
			}
		}

		public class ScrollViewScope : GUI.Scope
		{
			public Vector2 scrollPosition
			{
				get;
				protected set;
			}

			public bool handleScrollWheel
			{
				get;
				set;
			}

			public ScrollViewScope(Vector2 scrollPosition, params GUILayoutOption[] options)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, options);
			}

			public ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options);
			}

			public ScrollViewScope(Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, horizontalScrollbar, verticalScrollbar, options);
			}

			public ScrollViewScope(Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, style, options);
			}

			public ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options);
			}

			internal ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, options);
			}

			protected override void CloseScope()
			{
				EditorGUILayout.EndScrollView(this.handleScrollWheel);
			}
		}

		internal class VerticalScrollViewScope : GUI.Scope
		{
			public Vector2 scrollPosition
			{
				get;
				protected set;
			}

			public bool handleScrollWheel
			{
				get;
				set;
			}

			public VerticalScrollViewScope(Vector2 scrollPosition, params GUILayoutOption[] options)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = EditorGUILayout.BeginVerticalScrollView(scrollPosition, options);
			}

			public VerticalScrollViewScope(Vector2 scrollPosition, bool alwaysShowVertical, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = EditorGUILayout.BeginVerticalScrollView(scrollPosition, alwaysShowVertical, verticalScrollbar, background, options);
			}

			protected override void CloseScope()
			{
				EditorGUILayout.EndScrollView(this.handleScrollWheel);
			}
		}

		internal class HorizontalScrollViewScope : GUI.Scope
		{
			public Vector2 scrollPosition
			{
				get;
				protected set;
			}

			public bool handleScrollWheel
			{
				get;
				set;
			}

			public HorizontalScrollViewScope(Vector2 scrollPosition, params GUILayoutOption[] options)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = EditorGUILayout.BeginHorizontalScrollView(scrollPosition, options);
			}

			public HorizontalScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, GUIStyle horizontalScrollbar, GUIStyle background, params GUILayoutOption[] options)
			{
				this.handleScrollWheel = true;
				this.scrollPosition = EditorGUILayout.BeginHorizontalScrollView(scrollPosition, alwaysShowHorizontal, horizontalScrollbar, background, options);
			}

			protected override void CloseScope()
			{
				EditorGUILayout.EndScrollView(this.handleScrollWheel);
			}
		}

		public class FadeGroupScope : GUI.Scope
		{
			public bool visible
			{
				get;
				protected set;
			}

			public FadeGroupScope(float value)
			{
				this.visible = EditorGUILayout.BeginFadeGroup(value);
			}

			protected override void CloseScope()
			{
				EditorGUILayout.EndFadeGroup();
			}
		}

		internal static Rect s_LastRect;

		internal const float kPlatformTabWidth = 30f;

		internal static SavedBool s_SelectedDefault = new SavedBool("Platform.ShownDefaultTab", true);

		[CompilerGenerated]
		private static TargetChoiceHandler.TargetChoiceMenuFunction <>f__mg$cache0;

		internal static float kLabelFloatMinW
		{
			get
			{
				return EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth + 5f;
			}
		}

		internal static float kLabelFloatMaxW
		{
			get
			{
				return EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth + 5f;
			}
		}

		[ExcludeFromDocs]
		public static bool Foldout(bool foldout, string content)
		{
			GUIStyle foldout2 = EditorStyles.foldout;
			return EditorGUILayout.Foldout(foldout, content, foldout2);
		}

		public static bool Foldout(bool foldout, string content, [DefaultValue("EditorStyles.foldout")] GUIStyle style)
		{
			return EditorGUILayout.Foldout(foldout, EditorGUIUtility.TempContent(content), false, style);
		}

		[ExcludeFromDocs]
		public static bool Foldout(bool foldout, GUIContent content)
		{
			GUIStyle foldout2 = EditorStyles.foldout;
			return EditorGUILayout.Foldout(foldout, content, foldout2);
		}

		public static bool Foldout(bool foldout, GUIContent content, [DefaultValue("EditorStyles.foldout")] GUIStyle style)
		{
			return EditorGUILayout.Foldout(foldout, content, false, style);
		}

		[ExcludeFromDocs]
		public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick)
		{
			GUIStyle foldout2 = EditorStyles.foldout;
			return EditorGUILayout.Foldout(foldout, content, toggleOnLabelClick, foldout2);
		}

		public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick, [DefaultValue("EditorStyles.foldout")] GUIStyle style)
		{
			return EditorGUILayout.Foldout(foldout, EditorGUIUtility.TempContent(content), toggleOnLabelClick, style);
		}

		[ExcludeFromDocs]
		public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick)
		{
			GUIStyle foldout2 = EditorStyles.foldout;
			return EditorGUILayout.Foldout(foldout, content, toggleOnLabelClick, foldout2);
		}

		public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick, [DefaultValue("EditorStyles.foldout")] GUIStyle style)
		{
			return EditorGUILayout.FoldoutInternal(foldout, content, toggleOnLabelClick, style);
		}

		[ExcludeFromDocs]
		public static void PrefixLabel(string label)
		{
			GUIStyle followingStyle = "Button";
			EditorGUILayout.PrefixLabel(label, followingStyle);
		}

		public static void PrefixLabel(string label, [DefaultValue("\"Button\"")] GUIStyle followingStyle)
		{
			EditorGUILayout.PrefixLabel(EditorGUIUtility.TempContent(label), followingStyle, EditorStyles.label);
		}

		public static void PrefixLabel(string label, GUIStyle followingStyle, GUIStyle labelStyle)
		{
			EditorGUILayout.PrefixLabel(EditorGUIUtility.TempContent(label), followingStyle, labelStyle);
		}

		[ExcludeFromDocs]
		public static void PrefixLabel(GUIContent label)
		{
			GUIStyle followingStyle = "Button";
			EditorGUILayout.PrefixLabel(label, followingStyle);
		}

		public static void PrefixLabel(GUIContent label, [DefaultValue("\"Button\"")] GUIStyle followingStyle)
		{
			EditorGUILayout.PrefixLabel(label, followingStyle, EditorStyles.label);
		}

		public static void PrefixLabel(GUIContent label, GUIStyle followingStyle, GUIStyle labelStyle)
		{
			EditorGUILayout.PrefixLabelInternal(label, followingStyle, labelStyle);
		}

		public static void LabelField(string label, params GUILayoutOption[] options)
		{
			EditorGUILayout.LabelField(GUIContent.none, EditorGUIUtility.TempContent(label), EditorStyles.label, options);
		}

		public static void LabelField(string label, GUIStyle style, params GUILayoutOption[] options)
		{
			EditorGUILayout.LabelField(GUIContent.none, EditorGUIUtility.TempContent(label), style, options);
		}

		public static void LabelField(GUIContent label, params GUILayoutOption[] options)
		{
			EditorGUILayout.LabelField(GUIContent.none, label, EditorStyles.label, options);
		}

		public static void LabelField(GUIContent label, GUIStyle style, params GUILayoutOption[] options)
		{
			EditorGUILayout.LabelField(GUIContent.none, label, style, options);
		}

		public static void LabelField(string label, string label2, params GUILayoutOption[] options)
		{
			EditorGUILayout.LabelField(new GUIContent(label), EditorGUIUtility.TempContent(label2), EditorStyles.label, options);
		}

		public static void LabelField(string label, string label2, GUIStyle style, params GUILayoutOption[] options)
		{
			EditorGUILayout.LabelField(new GUIContent(label), EditorGUIUtility.TempContent(label2), style, options);
		}

		public static void LabelField(GUIContent label, GUIContent label2, params GUILayoutOption[] options)
		{
			EditorGUILayout.LabelField(label, label2, EditorStyles.label, options);
		}

		public static void LabelField(GUIContent label, GUIContent label2, GUIStyle style, params GUILayoutOption[] options)
		{
			if (!style.wordWrap)
			{
				Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, options);
				EditorGUI.LabelField(position, label, label2, style);
			}
			else
			{
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PrefixLabel(label, style);
				Rect rect = GUILayoutUtility.GetRect(label2, style, options);
				int indentLevel = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;
				EditorGUI.LabelField(rect, label2, style);
				EditorGUI.indentLevel = indentLevel;
				EditorGUILayout.EndHorizontal();
			}
		}

		internal static bool LinkLabel(string label, params GUILayoutOption[] options)
		{
			return EditorGUILayout.LinkLabel(EditorGUIUtility.TempContent(label), options);
		}

		internal static bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(label, EditorStyles.linkLabel, options);
			Handles.BeginGUI();
			Handles.color = EditorStyles.linkLabel.normal.textColor;
			Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
			Handles.color = Color.white;
			Handles.EndGUI();
			EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
			return GUI.Button(position, label, EditorStyles.linkLabel);
		}

		public static bool Toggle(bool value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetToggleRect(false, options);
			return EditorGUI.Toggle(position, value);
		}

		public static bool Toggle(string label, bool value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.Toggle(EditorGUIUtility.TempContent(label), value, options);
		}

		public static bool Toggle(GUIContent label, bool value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetToggleRect(true, options);
			return EditorGUI.Toggle(position, label, value);
		}

		public static bool Toggle(bool value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetToggleRect(false, options);
			return EditorGUI.Toggle(position, value, style);
		}

		public static bool Toggle(string label, bool value, GUIStyle style, params GUILayoutOption[] options)
		{
			return EditorGUILayout.Toggle(EditorGUIUtility.TempContent(label), value, style, options);
		}

		public static bool Toggle(GUIContent label, bool value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetToggleRect(true, options);
			return EditorGUI.Toggle(position, label, value, style);
		}

		public static bool ToggleLeft(string label, bool value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.ToggleLeft(EditorGUIUtility.TempContent(label), value, options);
		}

		public static bool ToggleLeft(GUIContent label, bool value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, options);
			return EditorGUI.ToggleLeft(position, label, value);
		}

		public static bool ToggleLeft(string label, bool value, GUIStyle labelStyle, params GUILayoutOption[] options)
		{
			return EditorGUILayout.ToggleLeft(EditorGUIUtility.TempContent(label), value, labelStyle, options);
		}

		public static bool ToggleLeft(GUIContent label, bool value, GUIStyle labelStyle, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, options);
			return EditorGUI.ToggleLeft(position, label, value, labelStyle);
		}

		public static string TextField(string text, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.textField, options);
			return EditorGUI.TextField(position, text);
		}

		public static string TextField(string text, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.TextField(position, text, style);
		}

		public static string TextField(string label, string text, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.textField, options);
			return EditorGUI.TextField(position, label, text);
		}

		public static string TextField(string label, string text, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.TextField(position, label, text, style);
		}

		public static string TextField(GUIContent label, string text, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.textField, options);
			return EditorGUI.TextField(position, label, text);
		}

		public static string TextField(GUIContent label, string text, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.TextField(position, label, text, style);
		}

		public static string DelayedTextField(string text, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.textField, options);
			return EditorGUI.DelayedTextField(position, text);
		}

		public static string DelayedTextField(string text, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.DelayedTextField(position, text, style);
		}

		public static string DelayedTextField(string label, string text, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.textField, options);
			return EditorGUI.DelayedTextField(position, label, text);
		}

		public static string DelayedTextField(string label, string text, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.DelayedTextField(position, label, text, style);
		}

		public static string DelayedTextField(GUIContent label, string text, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.textField, options);
			return EditorGUI.DelayedTextField(position, label, text);
		}

		public static string DelayedTextField(GUIContent label, string text, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.DelayedTextField(position, label, text, style);
		}

		public static void DelayedTextField(SerializedProperty property, params GUILayoutOption[] options)
		{
			EditorGUILayout.DelayedTextField(property, null, options);
		}

		public static void DelayedTextField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(EditorGUI.LabelHasContent(label), 16f, EditorStyles.textField, options);
			EditorGUI.DelayedTextField(position, property, label);
		}

		internal static string ToolbarSearchField(string text, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(0f, EditorGUILayout.kLabelFloatMaxW * 1.5f, 16f, 16f, EditorStyles.toolbarSearchField, options);
			int num = 0;
			return EditorGUI.ToolbarSearchField(position, null, ref num, text);
		}

		internal static string ToolbarSearchField(string text, string[] searchModes, ref int searchMode, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(0f, EditorGUILayout.kLabelFloatMaxW * 1.5f, 16f, 16f, EditorStyles.toolbarSearchField, options);
			return EditorGUI.ToolbarSearchField(position, searchModes, ref searchMode, text);
		}

		public static string TextArea(string text, params GUILayoutOption[] options)
		{
			return EditorGUILayout.TextArea(text, EditorStyles.textField, options);
		}

		public static string TextArea(string text, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(EditorGUIUtility.TempContent(text), style, options);
			return EditorGUI.TextArea(position, text, style);
		}

		public static void SelectableLabel(string text, params GUILayoutOption[] options)
		{
			EditorGUILayout.SelectableLabel(text, EditorStyles.label, options);
		}

		public static void SelectableLabel(string text, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 32f, style, options);
			EditorGUI.SelectableLabel(position, text, style);
		}

		internal static Event KeyEventField(Event e, params GUILayoutOption[] options)
		{
			GUIContent content = EditorGUIUtility.TempContent("[Please press a key]");
			Rect rect = GUILayoutUtility.GetRect(content, GUI.skin.textField, options);
			return EditorGUI.KeyEventField(rect, e);
		}

		public static string PasswordField(string password, params GUILayoutOption[] options)
		{
			return EditorGUILayout.PasswordField(password, EditorStyles.textField, options);
		}

		public static string PasswordField(string password, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.PasswordField(position, password, style);
		}

		public static string PasswordField(string label, string password, params GUILayoutOption[] options)
		{
			return EditorGUILayout.PasswordField(EditorGUIUtility.TempContent(label), password, EditorStyles.textField, options);
		}

		public static string PasswordField(string label, string password, GUIStyle style, params GUILayoutOption[] options)
		{
			return EditorGUILayout.PasswordField(EditorGUIUtility.TempContent(label), password, style, options);
		}

		public static string PasswordField(GUIContent label, string password, params GUILayoutOption[] options)
		{
			return EditorGUILayout.PasswordField(label, password, EditorStyles.textField, options);
		}

		public static string PasswordField(GUIContent label, string password, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.PasswordField(position, label, password, style);
		}

		internal static void VUMeterHorizontal(float value, float peak, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.numberField, options);
			EditorGUI.VUMeter.HorizontalMeter(position, value, peak, EditorGUI.VUMeter.horizontalVUTexture, Color.grey);
		}

		internal static void VUMeterHorizontal(float value, ref EditorGUI.VUMeter.SmoothingData data, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.numberField, options);
			EditorGUI.VUMeter.HorizontalMeter(position, value, ref data, EditorGUI.VUMeter.horizontalVUTexture, Color.grey);
		}

		public static float FloatField(float value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.numberField, options);
			return EditorGUI.FloatField(position, value);
		}

		public static float FloatField(float value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.FloatField(position, value, style);
		}

		public static float FloatField(string label, float value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, options);
			return EditorGUI.FloatField(position, label, value);
		}

		public static float FloatField(string label, float value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.FloatField(position, label, value, style);
		}

		public static float FloatField(GUIContent label, float value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, options);
			return EditorGUI.FloatField(position, label, value);
		}

		public static float FloatField(GUIContent label, float value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.FloatField(position, label, value, style);
		}

		public static float DelayedFloatField(float value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.numberField, options);
			return EditorGUI.DelayedFloatField(position, value);
		}

		public static float DelayedFloatField(float value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.DelayedFloatField(position, value, style);
		}

		public static float DelayedFloatField(string label, float value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, options);
			return EditorGUI.DelayedFloatField(position, label, value);
		}

		public static float DelayedFloatField(string label, float value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.DelayedFloatField(position, label, value, style);
		}

		public static float DelayedFloatField(GUIContent label, float value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, options);
			return EditorGUI.DelayedFloatField(position, label, value);
		}

		public static float DelayedFloatField(GUIContent label, float value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.DelayedFloatField(position, label, value, style);
		}

		public static void DelayedFloatField(SerializedProperty property, params GUILayoutOption[] options)
		{
			EditorGUILayout.DelayedFloatField(property, null, options);
		}

		public static void DelayedFloatField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(EditorGUI.LabelHasContent(label), 16f, EditorStyles.numberField, options);
			EditorGUI.DelayedFloatField(position, property, label);
		}

		public static double DoubleField(double value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.numberField, options);
			return EditorGUI.DoubleField(position, value);
		}

		public static double DoubleField(double value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.DoubleField(position, value, style);
		}

		public static double DoubleField(string label, double value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, options);
			return EditorGUI.DoubleField(position, label, value);
		}

		public static double DoubleField(string label, double value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.DoubleField(position, label, value, style);
		}

		public static double DoubleField(GUIContent label, double value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, options);
			return EditorGUI.DoubleField(position, label, value);
		}

		public static double DoubleField(GUIContent label, double value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.DoubleField(position, label, value, style);
		}

		public static double DelayedDoubleField(double value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.numberField, options);
			return EditorGUI.DelayedDoubleField(position, value);
		}

		public static double DelayedDoubleField(double value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.DelayedDoubleField(position, value, style);
		}

		public static double DelayedDoubleField(string label, double value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, options);
			return EditorGUI.DelayedDoubleField(position, label, value);
		}

		public static double DelayedDoubleField(string label, double value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.DelayedDoubleField(position, label, value, style);
		}

		public static double DelayedDoubleField(GUIContent label, double value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, options);
			return EditorGUI.DelayedDoubleField(position, label, value);
		}

		public static double DelayedDoubleField(GUIContent label, double value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.DelayedDoubleField(position, label, value, style);
		}

		public static int IntField(int value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.IntField(value, EditorStyles.numberField, options);
		}

		public static int IntField(int value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.IntField(position, value, style);
		}

		public static int IntField(string label, int value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.IntField(label, value, EditorStyles.numberField, options);
		}

		public static int IntField(string label, int value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.IntField(position, label, value, style);
		}

		public static int IntField(GUIContent label, int value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.IntField(label, value, EditorStyles.numberField, options);
		}

		public static int IntField(GUIContent label, int value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.IntField(position, label, value, style);
		}

		public static int DelayedIntField(int value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.DelayedIntField(value, EditorStyles.numberField, options);
		}

		public static int DelayedIntField(int value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.DelayedIntField(position, value, style);
		}

		public static int DelayedIntField(string label, int value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.DelayedIntField(label, value, EditorStyles.numberField, options);
		}

		public static int DelayedIntField(string label, int value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.DelayedIntField(position, label, value, style);
		}

		public static int DelayedIntField(GUIContent label, int value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.DelayedIntField(label, value, EditorStyles.numberField, options);
		}

		public static int DelayedIntField(GUIContent label, int value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.DelayedIntField(position, label, value, style);
		}

		public static void DelayedIntField(SerializedProperty property, params GUILayoutOption[] options)
		{
			EditorGUILayout.DelayedIntField(property, null, options);
		}

		public static void DelayedIntField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(EditorGUI.LabelHasContent(label), 16f, EditorStyles.numberField, options);
			EditorGUI.DelayedIntField(position, property, label);
		}

		public static long LongField(long value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.LongField(value, EditorStyles.numberField, options);
		}

		public static long LongField(long value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.LongField(position, value, style);
		}

		public static long LongField(string label, long value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.LongField(label, value, EditorStyles.numberField, options);
		}

		public static long LongField(string label, long value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.LongField(position, label, value, style);
		}

		public static long LongField(GUIContent label, long value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.LongField(label, value, EditorStyles.numberField, options);
		}

		public static long LongField(GUIContent label, long value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.LongField(position, label, value, style);
		}

		public static float Slider(float value, float leftValue, float rightValue, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(false, options);
			return EditorGUI.Slider(position, value, leftValue, rightValue);
		}

		public static float Slider(string label, float value, float leftValue, float rightValue, params GUILayoutOption[] options)
		{
			return EditorGUILayout.Slider(EditorGUIUtility.TempContent(label), value, leftValue, rightValue, options);
		}

		public static float Slider(GUIContent label, float value, float leftValue, float rightValue, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(true, options);
			return EditorGUI.Slider(position, label, value, leftValue, rightValue);
		}

		public static void Slider(SerializedProperty property, float leftValue, float rightValue, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(false, options);
			EditorGUI.Slider(position, property, leftValue, rightValue);
		}

		internal static void SliderWithTexture(GUIContent label, SerializedProperty property, float leftValue, float rightValue, float power, GUIStyle sliderStyle, GUIStyle thumbStyle, Texture2D sliderBackground, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(false, options);
			EditorGUI.SliderWithTexture(position, label, property, leftValue, rightValue, power, sliderStyle, thumbStyle, sliderBackground);
		}

		public static void Slider(SerializedProperty property, float leftValue, float rightValue, string label, params GUILayoutOption[] options)
		{
			EditorGUILayout.Slider(property, leftValue, rightValue, EditorGUIUtility.TempContent(label), options);
		}

		public static void Slider(SerializedProperty property, float leftValue, float rightValue, GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(true, options);
			EditorGUI.Slider(position, property, leftValue, rightValue, label);
		}

		internal static float PowerSlider(string label, float value, float leftValue, float rightValue, float power, params GUILayoutOption[] options)
		{
			return EditorGUILayout.PowerSlider(EditorGUIUtility.TempContent(label), value, leftValue, rightValue, power, options);
		}

		internal static float PowerSlider(GUIContent label, float value, float leftValue, float rightValue, float power, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(true, options);
			return EditorGUI.PowerSlider(position, label, value, leftValue, rightValue, power);
		}

		public static int IntSlider(int value, int leftValue, int rightValue, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(false, options);
			return EditorGUI.IntSlider(position, value, leftValue, rightValue);
		}

		public static int IntSlider(string label, int value, int leftValue, int rightValue, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(true, options);
			return EditorGUI.IntSlider(position, label, value, leftValue, rightValue);
		}

		public static int IntSlider(GUIContent label, int value, int leftValue, int rightValue, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(true, options);
			return EditorGUI.IntSlider(position, label, value, leftValue, rightValue);
		}

		public static void IntSlider(SerializedProperty property, int leftValue, int rightValue, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(false, options);
			EditorGUI.IntSlider(position, property, leftValue, rightValue, property.displayName);
		}

		public static void IntSlider(SerializedProperty property, int leftValue, int rightValue, string label, params GUILayoutOption[] options)
		{
			EditorGUILayout.IntSlider(property, leftValue, rightValue, EditorGUIUtility.TempContent(label), options);
		}

		public static void IntSlider(SerializedProperty property, int leftValue, int rightValue, GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(true, options);
			EditorGUI.IntSlider(position, property, leftValue, rightValue, label);
		}

		public static void MinMaxSlider(ref float minValue, ref float maxValue, float minLimit, float maxLimit, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(false, options);
			EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, minLimit, maxLimit);
		}

		public static void MinMaxSlider(string label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(true, options);
			EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minLimit, maxLimit);
		}

		public static void MinMaxSlider(GUIContent label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetSliderRect(true, options);
			EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minLimit, maxLimit);
		}

		public static int Popup(int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
		{
			return EditorGUILayout.Popup(selectedIndex, displayedOptions, EditorStyles.popup, options);
		}

		public static int Popup(int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.Popup(position, selectedIndex, displayedOptions, style);
		}

		public static int Popup(int selectedIndex, GUIContent[] displayedOptions, params GUILayoutOption[] options)
		{
			return EditorGUILayout.Popup(selectedIndex, displayedOptions, EditorStyles.popup, options);
		}

		public static int Popup(int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.Popup(position, selectedIndex, displayedOptions, style);
		}

		public static int Popup(string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options)
		{
			return EditorGUILayout.Popup(label, selectedIndex, displayedOptions, EditorStyles.popup, options);
		}

		public static int Popup(string label, int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.Popup(position, label, selectedIndex, displayedOptions, style);
		}

		public static int Popup(GUIContent label, int selectedIndex, GUIContent[] displayedOptions, params GUILayoutOption[] options)
		{
			return EditorGUILayout.Popup(label, selectedIndex, displayedOptions, EditorStyles.popup, options);
		}

		public static int Popup(GUIContent label, int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.Popup(position, label, selectedIndex, displayedOptions, style);
		}

		internal static void Popup(SerializedProperty property, GUIContent[] displayedOptions, params GUILayoutOption[] options)
		{
			EditorGUILayout.Popup(property, displayedOptions, null, options);
		}

		internal static void Popup(SerializedProperty property, GUIContent[] displayedOptions, GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.popup, options);
			EditorGUI.Popup(position, property, displayedOptions, label);
		}

		public static Enum EnumPopup(Enum selected, params GUILayoutOption[] options)
		{
			return EditorGUILayout.EnumPopup(selected, EditorStyles.popup, options);
		}

		public static Enum EnumPopup(Enum selected, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.EnumPopup(position, selected, style);
		}

		public static Enum EnumPopup(string label, Enum selected, params GUILayoutOption[] options)
		{
			return EditorGUILayout.EnumPopup(label, selected, EditorStyles.popup, options);
		}

		public static Enum EnumPopup(string label, Enum selected, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.EnumPopup(position, GUIContent.Temp(label), selected, style);
		}

		public static Enum EnumPopup(GUIContent label, Enum selected, params GUILayoutOption[] options)
		{
			return EditorGUILayout.EnumPopup(label, selected, EditorStyles.popup, options);
		}

		public static Enum EnumPopup(GUIContent label, Enum selected, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.EnumPopup(position, label, selected, style);
		}

		public static Enum EnumMaskPopup(string label, Enum selected, params GUILayoutOption[] options)
		{
			int num;
			bool flag;
			return EditorGUILayout.EnumMaskPopup(EditorGUIUtility.TempContent(label), selected, out num, out flag, options);
		}

		public static Enum EnumMaskPopup(string label, Enum selected, GUIStyle style, params GUILayoutOption[] options)
		{
			int num;
			bool flag;
			return EditorGUILayout.EnumMaskPopup(EditorGUIUtility.TempContent(label), selected, out num, out flag, style, options);
		}

		public static Enum EnumMaskPopup(GUIContent label, Enum selected, params GUILayoutOption[] options)
		{
			int num;
			bool flag;
			return EditorGUILayout.EnumMaskPopup(label, selected, out num, out flag, options);
		}

		public static Enum EnumMaskPopup(GUIContent label, Enum selected, GUIStyle style, params GUILayoutOption[] options)
		{
			int num;
			bool flag;
			return EditorGUILayout.EnumMaskPopup(label, selected, out num, out flag, style, options);
		}

		internal static Enum EnumMaskPopup(string label, Enum selected, out int changedFlags, out bool changedToValue, params GUILayoutOption[] options)
		{
			return EditorGUILayout.EnumMaskPopup(EditorGUIUtility.TempContent(label), selected, out changedFlags, out changedToValue, options);
		}

		internal static Enum EnumMaskPopup(string label, Enum selected, out int changedFlags, out bool changedToValue, GUIStyle style, params GUILayoutOption[] options)
		{
			return EditorGUILayout.EnumMaskPopup(EditorGUIUtility.TempContent(label), selected, out changedFlags, out changedToValue, style, options);
		}

		internal static Enum EnumMaskPopup(GUIContent label, Enum selected, out int changedFlags, out bool changedToValue, params GUILayoutOption[] options)
		{
			return EditorGUILayout.EnumMaskPopup(label, selected, out changedFlags, out changedToValue, EditorStyles.popup, options);
		}

		internal static Enum EnumMaskPopup(GUIContent label, Enum selected, out int changedFlags, out bool changedToValue, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.EnumMaskPopup(position, label, selected, out changedFlags, out changedToValue, style);
		}

		public static int IntPopup(int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options)
		{
			return EditorGUILayout.IntPopup(selectedValue, displayedOptions, optionValues, EditorStyles.popup, options);
		}

		public static int IntPopup(int selectedValue, string[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.IntPopup(position, selectedValue, displayedOptions, optionValues, style);
		}

		public static int IntPopup(int selectedValue, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options)
		{
			return EditorGUILayout.IntPopup(selectedValue, displayedOptions, optionValues, EditorStyles.popup, options);
		}

		public static int IntPopup(int selectedValue, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.IntPopup(position, GUIContent.none, selectedValue, displayedOptions, optionValues, style);
		}

		public static int IntPopup(string label, int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options)
		{
			return EditorGUILayout.IntPopup(label, selectedValue, displayedOptions, optionValues, EditorStyles.popup, options);
		}

		public static int IntPopup(string label, int selectedValue, string[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.IntPopup(position, label, selectedValue, displayedOptions, optionValues, style);
		}

		public static int IntPopup(GUIContent label, int selectedValue, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options)
		{
			return EditorGUILayout.IntPopup(label, selectedValue, displayedOptions, optionValues, EditorStyles.popup, options);
		}

		public static int IntPopup(GUIContent label, int selectedValue, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.IntPopup(position, label, selectedValue, displayedOptions, optionValues, style);
		}

		public static void IntPopup(SerializedProperty property, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options)
		{
			EditorGUILayout.IntPopup(property, displayedOptions, optionValues, null, options);
		}

		public static void IntPopup(SerializedProperty property, GUIContent[] displayedOptions, int[] optionValues, GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.popup, options);
			EditorGUI.IntPopup(position, property, displayedOptions, optionValues, label);
		}

		[Obsolete("This function is obsolete and the style is not used.")]
		public static void IntPopup(SerializedProperty property, GUIContent[] displayedOptions, int[] optionValues, GUIContent label, GUIStyle style, params GUILayoutOption[] options)
		{
			EditorGUILayout.IntPopup(property, displayedOptions, optionValues, label, options);
		}

		public static string TagField(string tag, params GUILayoutOption[] options)
		{
			return EditorGUILayout.TagField(tag, EditorStyles.popup, options);
		}

		public static string TagField(string tag, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.TagField(position, tag, style);
		}

		public static string TagField(string label, string tag, params GUILayoutOption[] options)
		{
			return EditorGUILayout.TagField(EditorGUIUtility.TempContent(label), tag, EditorStyles.popup, options);
		}

		public static string TagField(string label, string tag, GUIStyle style, params GUILayoutOption[] options)
		{
			return EditorGUILayout.TagField(EditorGUIUtility.TempContent(label), tag, style, options);
		}

		public static string TagField(GUIContent label, string tag, params GUILayoutOption[] options)
		{
			return EditorGUILayout.TagField(label, tag, EditorStyles.popup, options);
		}

		public static string TagField(GUIContent label, string tag, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.TagField(position, label, tag, style);
		}

		public static int LayerField(int layer, params GUILayoutOption[] options)
		{
			return EditorGUILayout.LayerField(layer, EditorStyles.popup, options);
		}

		public static int LayerField(int layer, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.LayerField(position, layer, style);
		}

		public static int LayerField(string label, int layer, params GUILayoutOption[] options)
		{
			return EditorGUILayout.LayerField(EditorGUIUtility.TempContent(label), layer, EditorStyles.popup, options);
		}

		public static int LayerField(string label, int layer, GUIStyle style, params GUILayoutOption[] options)
		{
			return EditorGUILayout.LayerField(EditorGUIUtility.TempContent(label), layer, style, options);
		}

		public static int LayerField(GUIContent label, int layer, params GUILayoutOption[] options)
		{
			return EditorGUILayout.LayerField(label, layer, EditorStyles.popup, options);
		}

		public static int LayerField(GUIContent label, int layer, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.LayerField(position, label, layer, style);
		}

		public static int MaskField(GUIContent label, int mask, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.MaskField(position, label, mask, displayedOptions, style);
		}

		public static int MaskField(string label, int mask, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.MaskField(position, label, mask, displayedOptions, style);
		}

		public static int MaskField(GUIContent label, int mask, string[] displayedOptions, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.popup, options);
			return EditorGUI.MaskField(position, label, mask, displayedOptions, EditorStyles.popup);
		}

		public static int MaskField(string label, int mask, string[] displayedOptions, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.popup, options);
			return EditorGUI.MaskField(position, label, mask, displayedOptions, EditorStyles.popup);
		}

		public static int MaskField(int mask, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.MaskField(position, mask, displayedOptions, style);
		}

		public static int MaskField(int mask, string[] displayedOptions, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.popup, options);
			return EditorGUI.MaskField(position, mask, displayedOptions, EditorStyles.popup);
		}

		public static Enum EnumMaskField(GUIContent label, Enum enumValue, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.EnumMaskField(position, label, enumValue, style);
		}

		public static Enum EnumMaskField(string label, Enum enumValue, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
			return EditorGUI.EnumMaskField(position, label, enumValue, style);
		}

		public static Enum EnumMaskField(GUIContent label, Enum enumValue, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.popup, options);
			return EditorGUI.EnumMaskField(position, label, enumValue, EditorStyles.popup);
		}

		public static Enum EnumMaskField(string label, Enum enumValue, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.popup, options);
			return EditorGUI.EnumMaskField(position, label, enumValue, EditorStyles.popup);
		}

		public static Enum EnumMaskField(Enum enumValue, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			return EditorGUI.EnumMaskField(position, enumValue, style);
		}

		public static Enum EnumMaskField(Enum enumValue, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.popup, options);
			return EditorGUI.EnumMaskField(position, enumValue, EditorStyles.popup);
		}

		[Obsolete("Check the docs for the usage of the new parameter 'allowSceneObjects'.")]
		public static UnityEngine.Object ObjectField(UnityEngine.Object obj, Type objType, params GUILayoutOption[] options)
		{
			return EditorGUILayout.ObjectField(obj, objType, true, options);
		}

		public static UnityEngine.Object ObjectField(UnityEngine.Object obj, Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, options);
			return EditorGUI.ObjectField(position, obj, objType, allowSceneObjects);
		}

		[Obsolete("Check the docs for the usage of the new parameter 'allowSceneObjects'.")]
		public static UnityEngine.Object ObjectField(string label, UnityEngine.Object obj, Type objType, params GUILayoutOption[] options)
		{
			return EditorGUILayout.ObjectField(label, obj, objType, true, options);
		}

		public static UnityEngine.Object ObjectField(string label, UnityEngine.Object obj, Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
		{
			return EditorGUILayout.ObjectField(EditorGUIUtility.TempContent(label), obj, objType, allowSceneObjects, options);
		}

		[Obsolete("Check the docs for the usage of the new parameter 'allowSceneObjects'.")]
		public static UnityEngine.Object ObjectField(GUIContent label, UnityEngine.Object obj, Type objType, params GUILayoutOption[] options)
		{
			return EditorGUILayout.ObjectField(label, obj, objType, true, options);
		}

		public static UnityEngine.Object ObjectField(GUIContent label, UnityEngine.Object obj, Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
		{
			float height;
			if (EditorGUIUtility.HasObjectThumbnail(objType))
			{
				height = 64f;
			}
			else
			{
				height = 16f;
			}
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, height, options);
			return EditorGUI.ObjectField(position, label, obj, objType, allowSceneObjects);
		}

		public static void ObjectField(SerializedProperty property, params GUILayoutOption[] options)
		{
			EditorGUILayout.ObjectField(property, null, options);
		}

		public static void ObjectField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.objectField, options);
			EditorGUI.ObjectField(position, property, label);
		}

		public static void ObjectField(SerializedProperty property, Type objType, params GUILayoutOption[] options)
		{
			EditorGUILayout.ObjectField(property, objType, null, options);
		}

		public static void ObjectField(SerializedProperty property, Type objType, GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.objectField, options);
			EditorGUI.ObjectField(position, property, objType, label);
		}

		internal static UnityEngine.Object MiniThumbnailObjectField(GUIContent label, UnityEngine.Object obj, Type objType, EditorGUI.ObjectFieldValidator validator, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, options);
			return EditorGUI.MiniThumbnailObjectField(position, label, obj, objType, validator);
		}

		public static Vector2 Vector2Field(string label, Vector2 value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.Vector2Field(EditorGUIUtility.TempContent(label), value, options);
		}

		public static Vector2 Vector2Field(GUIContent label, Vector2 value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, label), EditorStyles.numberField, options);
			return EditorGUI.Vector2Field(position, label, value);
		}

		public static Vector3 Vector3Field(string label, Vector3 value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.Vector3Field(EditorGUIUtility.TempContent(label), value, options);
		}

		public static Vector3 Vector3Field(GUIContent label, Vector3 value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label), EditorStyles.numberField, options);
			return EditorGUI.Vector3Field(position, label, value);
		}

		public static Vector4 Vector4Field(string label, Vector4 value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.Vector4Field(EditorGUIUtility.TempContent(label), value, options);
		}

		public static Vector4 Vector4Field(GUIContent label, Vector4 value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, label), EditorStyles.numberField, options);
			return EditorGUI.Vector4Field(position, label, value);
		}

		public static Rect RectField(Rect value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, EditorGUI.GetPropertyHeight(SerializedPropertyType.Rect, GUIContent.none), EditorStyles.numberField, options);
			return EditorGUI.RectField(position, value);
		}

		public static Rect RectField(string label, Rect value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.RectField(EditorGUIUtility.TempContent(label), value, options);
		}

		public static Rect RectField(GUIContent label, Rect value, params GUILayoutOption[] options)
		{
			bool hasLabel = EditorGUI.LabelHasContent(label);
			float propertyHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Rect, label);
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(hasLabel, propertyHeight, EditorStyles.numberField, options);
			return EditorGUI.RectField(position, label, value);
		}

		public static Bounds BoundsField(Bounds value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, EditorGUI.GetPropertyHeight(SerializedPropertyType.Bounds, GUIContent.none), EditorStyles.numberField, options);
			return EditorGUI.BoundsField(position, value);
		}

		public static Bounds BoundsField(string label, Bounds value, params GUILayoutOption[] options)
		{
			return EditorGUILayout.BoundsField(EditorGUIUtility.TempContent(label), value, options);
		}

		public static Bounds BoundsField(GUIContent label, Bounds value, params GUILayoutOption[] options)
		{
			bool hasLabel = EditorGUI.LabelHasContent(label);
			float propertyHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Bounds, label);
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(hasLabel, propertyHeight, EditorStyles.numberField, options);
			return EditorGUI.BoundsField(position, label, value);
		}

		internal static void PropertiesField(GUIContent label, SerializedProperty[] properties, GUIContent[] propertyLabels, float propertyLabelsWidth, params GUILayoutOption[] options)
		{
			bool hasLabel = EditorGUI.LabelHasContent(label);
			float height = 16f * (float)properties.Length + (float)(0 * (properties.Length - 1));
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(hasLabel, height, EditorStyles.numberField, options);
			EditorGUI.PropertiesField(position, label, properties, propertyLabels, propertyLabelsWidth);
		}

		internal static int CycleButton(int selected, GUIContent[] options, GUIStyle style)
		{
			if (GUILayout.Button(options[selected], style, new GUILayoutOption[0]))
			{
				selected++;
				if (selected >= options.Length)
				{
					selected = 0;
				}
			}
			return selected;
		}

		public static Color ColorField(Color value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.colorField, options);
			return EditorGUI.ColorField(position, value);
		}

		public static Color ColorField(string label, Color value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField, options);
			return EditorGUI.ColorField(position, label, value);
		}

		public static Color ColorField(GUIContent label, Color value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField, options);
			return EditorGUI.ColorField(position, label, value);
		}

		public static Color ColorField(GUIContent label, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField, options);
			return EditorGUI.ColorField(position, label, value, showEyedropper, showAlpha, hdr, hdrConfig);
		}

		public static AnimationCurve CurveField(AnimationCurve value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.colorField, options);
			return EditorGUI.CurveField(position, value);
		}

		public static AnimationCurve CurveField(string label, AnimationCurve value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField, options);
			return EditorGUI.CurveField(position, label, value);
		}

		public static AnimationCurve CurveField(GUIContent label, AnimationCurve value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField, options);
			return EditorGUI.CurveField(position, label, value);
		}

		public static AnimationCurve CurveField(AnimationCurve value, Color color, Rect ranges, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.colorField, options);
			return EditorGUI.CurveField(position, value, color, ranges);
		}

		public static AnimationCurve CurveField(string label, AnimationCurve value, Color color, Rect ranges, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField, options);
			return EditorGUI.CurveField(position, label, value, color, ranges);
		}

		public static AnimationCurve CurveField(GUIContent label, AnimationCurve value, Color color, Rect ranges, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField, options);
			return EditorGUI.CurveField(position, label, value, color, ranges);
		}

		public static void CurveField(SerializedProperty property, Color color, Rect ranges, params GUILayoutOption[] options)
		{
			EditorGUILayout.CurveField(property, color, ranges, null, options);
		}

		public static void CurveField(SerializedProperty property, Color color, Rect ranges, GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, EditorStyles.colorField, options);
			EditorGUI.CurveField(position, property, color, ranges, label);
		}

		public static bool InspectorTitlebar(bool foldout, UnityEngine.Object targetObj)
		{
			return EditorGUILayout.InspectorTitlebar(foldout, targetObj, true);
		}

		public static bool InspectorTitlebar(bool foldout, UnityEngine.Object targetObj, bool expandable)
		{
			return EditorGUI.InspectorTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), foldout, targetObj, expandable);
		}

		public static bool InspectorTitlebar(bool foldout, UnityEngine.Object[] targetObjs)
		{
			return EditorGUILayout.InspectorTitlebar(foldout, targetObjs, true);
		}

		public static bool InspectorTitlebar(bool foldout, UnityEngine.Object[] targetObjs, bool expandable)
		{
			return EditorGUI.InspectorTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), foldout, targetObjs, expandable);
		}

		public static void InspectorTitlebar(UnityEngine.Object[] targetObjs)
		{
			EditorGUI.InspectorTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), targetObjs);
		}

		internal static bool ToggleTitlebar(bool foldout, GUIContent label, ref bool toggleValue)
		{
			return EditorGUI.ToggleTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), label, foldout, ref toggleValue);
		}

		internal static bool ToggleTitlebar(bool foldout, GUIContent label, SerializedProperty property)
		{
			bool boolValue = property.boolValue;
			EditorGUI.BeginChangeCheck();
			foldout = EditorGUI.ToggleTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), label, foldout, ref boolValue);
			if (EditorGUI.EndChangeCheck())
			{
				property.boolValue = boolValue;
			}
			return foldout;
		}

		internal static bool FoldoutTitlebar(bool foldout, GUIContent label, bool skipIconSpacing)
		{
			return EditorGUI.FoldoutTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), label, foldout, skipIconSpacing);
		}

		internal static bool FoldoutInternal(bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style)
		{
			Rect position = EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUIUtility.fieldWidth, 16f, 16f, style);
			return EditorGUI.Foldout(position, foldout, content, toggleOnLabelClick, style);
		}

		internal static void LayerMaskField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, options);
			EditorGUI.LayerMaskField(position, property, label);
		}

		public static void HelpBox(string message, MessageType type)
		{
			EditorGUILayout.LabelField(GUIContent.none, EditorGUIUtility.TempContent(message, EditorGUIUtility.GetHelpIcon(type)), EditorStyles.helpBox, new GUILayoutOption[0]);
		}

		public static void HelpBox(string message, MessageType type, bool wide)
		{
			EditorGUILayout.LabelField((!wide) ? EditorGUIUtility.blankContent : GUIContent.none, EditorGUIUtility.TempContent(message, EditorGUIUtility.GetHelpIcon(type)), EditorStyles.helpBox, new GUILayoutOption[0]);
		}

		internal static void PrefixLabelInternal(GUIContent label, GUIStyle followingStyle, GUIStyle labelStyle)
		{
			float num = (float)followingStyle.margin.left;
			if (!EditorGUI.LabelHasContent(label))
			{
				GUILayoutUtility.GetRect(EditorGUI.indent - num, 16f, followingStyle, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
			}
			else
			{
				Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth - num, 16f, followingStyle, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				rect.xMin += EditorGUI.indent;
				EditorGUI.HandlePrefixLabel(rect, rect, label, 0, labelStyle);
			}
		}

		public static void Space()
		{
			GUILayoutUtility.GetRect(6f, 6f);
		}

		public static void Separator()
		{
			EditorGUILayout.Space();
		}

		public static bool BeginToggleGroup(string label, bool toggle)
		{
			return EditorGUILayout.BeginToggleGroup(EditorGUIUtility.TempContent(label), toggle);
		}

		public static bool BeginToggleGroup(GUIContent label, bool toggle)
		{
			toggle = EditorGUILayout.ToggleLeft(label, toggle, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUI.BeginDisabled(!toggle);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			return toggle;
		}

		public static void EndToggleGroup()
		{
			GUILayout.EndVertical();
			EditorGUI.EndDisabled();
		}

		public static Rect BeginHorizontal(params GUILayoutOption[] options)
		{
			return EditorGUILayout.BeginHorizontal(GUIContent.none, GUIStyle.none, options);
		}

		public static Rect BeginHorizontal(GUIStyle style, params GUILayoutOption[] options)
		{
			return EditorGUILayout.BeginHorizontal(GUIContent.none, style, options);
		}

		internal static Rect BeginHorizontal(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			GUILayoutGroup gUILayoutGroup = GUILayoutUtility.BeginLayoutGroup(style, options, typeof(GUILayoutGroup));
			gUILayoutGroup.isVertical = false;
			if (style != GUIStyle.none || content != GUIContent.none)
			{
				GUI.Box(gUILayoutGroup.rect, GUIContent.none, style);
			}
			return gUILayoutGroup.rect;
		}

		public static void EndHorizontal()
		{
			GUILayout.EndHorizontal();
		}

		public static Rect BeginVertical(params GUILayoutOption[] options)
		{
			return EditorGUILayout.BeginVertical(GUIContent.none, GUIStyle.none, options);
		}

		public static Rect BeginVertical(GUIStyle style, params GUILayoutOption[] options)
		{
			return EditorGUILayout.BeginVertical(GUIContent.none, style, options);
		}

		internal static Rect BeginVertical(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			GUILayoutGroup gUILayoutGroup = GUILayoutUtility.BeginLayoutGroup(style, options, typeof(GUILayoutGroup));
			gUILayoutGroup.isVertical = true;
			if (style != GUIStyle.none || content != GUIContent.none)
			{
				GUI.Box(gUILayoutGroup.rect, GUIContent.none, style);
			}
			return gUILayoutGroup.rect;
		}

		public static void EndVertical()
		{
			GUILayout.EndVertical();
		}

		public static Vector2 BeginScrollView(Vector2 scrollPosition, params GUILayoutOption[] options)
		{
			return EditorGUILayout.BeginScrollView(scrollPosition, false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);
		}

		public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options)
		{
			return EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);
		}

		public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
		{
			return EditorGUILayout.BeginScrollView(scrollPosition, false, false, horizontalScrollbar, verticalScrollbar, GUI.skin.scrollView, options);
		}

		public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
		{
			string name = style.name;
			GUIStyle gUIStyle = GUI.skin.FindStyle(name + "VerticalScrollbar");
			if (gUIStyle == null)
			{
				gUIStyle = GUI.skin.verticalScrollbar;
			}
			GUIStyle gUIStyle2 = GUI.skin.FindStyle(name + "HorizontalScrollbar");
			if (gUIStyle2 == null)
			{
				gUIStyle2 = GUI.skin.horizontalScrollbar;
			}
			return EditorGUILayout.BeginScrollView(scrollPosition, false, false, gUIStyle2, gUIStyle, style, options);
		}

		internal static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
		{
			return EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, GUI.skin.scrollView, options);
		}

		public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
		{
			GUIScrollGroup gUIScrollGroup = (GUIScrollGroup)GUILayoutUtility.BeginLayoutGroup(background, null, typeof(GUIScrollGroup));
			EventType type = Event.current.type;
			if (type == EventType.Layout)
			{
				gUIScrollGroup.resetCoords = true;
				gUIScrollGroup.isVertical = true;
				gUIScrollGroup.stretchWidth = 1;
				gUIScrollGroup.stretchHeight = 1;
				gUIScrollGroup.verticalScrollbar = verticalScrollbar;
				gUIScrollGroup.horizontalScrollbar = horizontalScrollbar;
				gUIScrollGroup.ApplyOptions(options);
			}
			return EditorGUIInternal.DoBeginScrollViewForward(gUIScrollGroup.rect, scrollPosition, new Rect(0f, 0f, gUIScrollGroup.clientWidth, gUIScrollGroup.clientHeight), alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
		}

		internal static Vector2 BeginVerticalScrollView(Vector2 scrollPosition, params GUILayoutOption[] options)
		{
			return EditorGUILayout.BeginVerticalScrollView(scrollPosition, false, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);
		}

		internal static Vector2 BeginVerticalScrollView(Vector2 scrollPosition, bool alwaysShowVertical, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
		{
			GUIScrollGroup gUIScrollGroup = (GUIScrollGroup)GUILayoutUtility.BeginLayoutGroup(background, null, typeof(GUIScrollGroup));
			EventType type = Event.current.type;
			if (type == EventType.Layout)
			{
				gUIScrollGroup.resetCoords = true;
				gUIScrollGroup.isVertical = true;
				gUIScrollGroup.stretchWidth = 1;
				gUIScrollGroup.stretchHeight = 1;
				gUIScrollGroup.verticalScrollbar = verticalScrollbar;
				gUIScrollGroup.horizontalScrollbar = GUIStyle.none;
				gUIScrollGroup.allowHorizontalScroll = false;
				gUIScrollGroup.ApplyOptions(options);
			}
			return EditorGUIInternal.DoBeginScrollViewForward(gUIScrollGroup.rect, scrollPosition, new Rect(0f, 0f, gUIScrollGroup.clientWidth, gUIScrollGroup.clientHeight), false, alwaysShowVertical, GUI.skin.horizontalScrollbar, verticalScrollbar, background);
		}

		internal static Vector2 BeginHorizontalScrollView(Vector2 scrollPosition, params GUILayoutOption[] options)
		{
			return EditorGUILayout.BeginHorizontalScrollView(scrollPosition, false, GUI.skin.horizontalScrollbar, GUI.skin.scrollView, options);
		}

		internal static Vector2 BeginHorizontalScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, GUIStyle horizontalScrollbar, GUIStyle background, params GUILayoutOption[] options)
		{
			GUIScrollGroup gUIScrollGroup = (GUIScrollGroup)GUILayoutUtility.BeginLayoutGroup(background, null, typeof(GUIScrollGroup));
			EventType type = Event.current.type;
			if (type == EventType.Layout)
			{
				gUIScrollGroup.resetCoords = true;
				gUIScrollGroup.isVertical = true;
				gUIScrollGroup.stretchWidth = 1;
				gUIScrollGroup.stretchHeight = 1;
				gUIScrollGroup.verticalScrollbar = GUIStyle.none;
				gUIScrollGroup.horizontalScrollbar = horizontalScrollbar;
				gUIScrollGroup.allowHorizontalScroll = true;
				gUIScrollGroup.allowVerticalScroll = false;
				gUIScrollGroup.ApplyOptions(options);
			}
			return EditorGUIInternal.DoBeginScrollViewForward(gUIScrollGroup.rect, scrollPosition, new Rect(0f, 0f, gUIScrollGroup.clientWidth, gUIScrollGroup.clientHeight), alwaysShowHorizontal, false, horizontalScrollbar, GUI.skin.verticalScrollbar, background);
		}

		public static void EndScrollView()
		{
			GUILayout.EndScrollView(true);
		}

		internal static void EndScrollView(bool handleScrollWheel)
		{
			GUILayout.EndScrollView(handleScrollWheel);
		}

		public static bool PropertyField(SerializedProperty property, params GUILayoutOption[] options)
		{
			return EditorGUILayout.PropertyField(property, null, false, options);
		}

		public static bool PropertyField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
		{
			return EditorGUILayout.PropertyField(property, label, false, options);
		}

		public static bool PropertyField(SerializedProperty property, bool includeChildren, params GUILayoutOption[] options)
		{
			return EditorGUILayout.PropertyField(property, null, includeChildren, options);
		}

		public static bool PropertyField(SerializedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options)
		{
			return ScriptAttributeUtility.GetHandler(property).OnGUILayout(property, label, includeChildren, options);
		}

		public static Rect GetControlRect(params GUILayoutOption[] options)
		{
			return EditorGUILayout.GetControlRect(true, 16f, EditorStyles.layerMaskField, options);
		}

		public static Rect GetControlRect(bool hasLabel, params GUILayoutOption[] options)
		{
			return EditorGUILayout.GetControlRect(hasLabel, 16f, EditorStyles.layerMaskField, options);
		}

		public static Rect GetControlRect(bool hasLabel, float height, params GUILayoutOption[] options)
		{
			return EditorGUILayout.GetControlRect(hasLabel, height, EditorStyles.layerMaskField, options);
		}

		public static Rect GetControlRect(bool hasLabel, float height, GUIStyle style, params GUILayoutOption[] options)
		{
			return GUILayoutUtility.GetRect((!hasLabel) ? EditorGUIUtility.fieldWidth : EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, height, height, style, options);
		}

		internal static Rect GetSliderRect(bool hasLabel, params GUILayoutOption[] options)
		{
			return GUILayoutUtility.GetRect((!hasLabel) ? EditorGUIUtility.fieldWidth : EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW + 5f + 100f, 16f, 16f, EditorStyles.numberField, options);
		}

		internal static Rect GetToggleRect(bool hasLabel, params GUILayoutOption[] options)
		{
			float num = 10f - EditorGUIUtility.fieldWidth;
			return GUILayoutUtility.GetRect((!hasLabel) ? (EditorGUIUtility.fieldWidth + num) : (EditorGUILayout.kLabelFloatMinW + num), EditorGUILayout.kLabelFloatMaxW + num, 16f, 16f, EditorStyles.numberField, options);
		}

		public static bool BeginFadeGroup(float value)
		{
			bool result;
			if (value == 0f)
			{
				result = false;
			}
			else if (value == 1f)
			{
				result = true;
			}
			else
			{
				GUILayoutFadeGroup gUILayoutFadeGroup = (GUILayoutFadeGroup)GUILayoutUtility.BeginLayoutGroup(GUIStyle.none, null, typeof(GUILayoutFadeGroup));
				gUILayoutFadeGroup.isVertical = true;
				gUILayoutFadeGroup.resetCoords = true;
				gUILayoutFadeGroup.fadeValue = value;
				gUILayoutFadeGroup.wasGUIEnabled = GUI.enabled;
				gUILayoutFadeGroup.guiColor = GUI.color;
				if (value != 0f && value != 1f && Event.current.type == EventType.MouseDown)
				{
					Event.current.Use();
				}
				EditorGUIUtility.LockContextWidth();
				GUI.BeginGroup(gUILayoutFadeGroup.rect);
				result = (value != 0f);
			}
			return result;
		}

		public static void EndFadeGroup()
		{
			GUILayoutFadeGroup gUILayoutFadeGroup = EditorGUILayoutUtilityInternal.topLevel as GUILayoutFadeGroup;
			if (gUILayoutFadeGroup != null)
			{
				GUI.EndGroup();
				EditorGUIUtility.UnlockContextWidth();
				GUI.enabled = gUILayoutFadeGroup.wasGUIEnabled;
				GUI.color = gUILayoutFadeGroup.guiColor;
				GUILayoutUtility.EndLayoutGroup();
			}
		}

		internal static int BeginPlatformGrouping(BuildPlayerWindow.BuildPlatform[] platforms, GUIContent defaultTab)
		{
			return EditorGUILayout.BeginPlatformGrouping(platforms, defaultTab, GUI.skin.box);
		}

		internal static int BeginPlatformGrouping(BuildPlayerWindow.BuildPlatform[] platforms, GUIContent defaultTab, GUIStyle style)
		{
			int num = -1;
			for (int i = 0; i < platforms.Length; i++)
			{
				if (platforms[i].targetGroup == EditorUserBuildSettings.selectedBuildTargetGroup)
				{
					num = i;
				}
			}
			if (num == -1)
			{
				EditorGUILayout.s_SelectedDefault.value = true;
				num = 0;
			}
			int num2 = (defaultTab != null) ? ((!EditorGUILayout.s_SelectedDefault.value) ? num : -1) : num;
			bool enabled = GUI.enabled;
			GUI.enabled = true;
			EditorGUI.BeginChangeCheck();
			Rect rect = EditorGUILayout.BeginVertical(style, new GUILayoutOption[0]);
			rect.width -= 1f;
			int num3 = platforms.Length;
			int num4 = 18;
			GUIStyle toolbarButton = EditorStyles.toolbarButton;
			if (defaultTab != null)
			{
				if (GUI.Toggle(new Rect(rect.x, rect.y, rect.width - (float)num3 * 30f, (float)num4), num2 == -1, defaultTab, toolbarButton))
				{
					num2 = -1;
				}
			}
			for (int j = 0; j < num3; j++)
			{
				Rect position;
				if (defaultTab != null)
				{
					position = new Rect(rect.xMax - (float)(num3 - j) * 30f, rect.y, 30f, (float)num4);
				}
				else
				{
					int num5 = Mathf.RoundToInt((float)j * rect.width / (float)num3);
					int num6 = Mathf.RoundToInt((float)(j + 1) * rect.width / (float)num3);
					position = new Rect(rect.x + (float)num5, rect.y, (float)(num6 - num5), (float)num4);
				}
				if (GUI.Toggle(position, num2 == j, new GUIContent(platforms[j].smallIcon, platforms[j].tooltip), toolbarButton))
				{
					num2 = j;
				}
			}
			GUILayoutUtility.GetRect(10f, (float)num4);
			GUI.enabled = enabled;
			if (EditorGUI.EndChangeCheck())
			{
				if (defaultTab == null)
				{
					EditorUserBuildSettings.selectedBuildTargetGroup = platforms[num2].targetGroup;
				}
				else if (num2 < 0)
				{
					EditorGUILayout.s_SelectedDefault.value = true;
				}
				else
				{
					EditorUserBuildSettings.selectedBuildTargetGroup = platforms[num2].targetGroup;
					EditorGUILayout.s_SelectedDefault.value = false;
				}
				UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(BuildPlayerWindow));
				for (int k = 0; k < array.Length; k++)
				{
					BuildPlayerWindow buildPlayerWindow = array[k] as BuildPlayerWindow;
					if (buildPlayerWindow != null)
					{
						buildPlayerWindow.Repaint();
					}
				}
			}
			return num2;
		}

		internal static void EndPlatformGrouping()
		{
			EditorGUILayout.EndVertical();
		}

		internal static void MultiSelectionObjectTitleBar(UnityEngine.Object[] objects)
		{
			string text = objects[0].name + " (" + ObjectNames.NicifyVariableName(ObjectNames.GetTypeName(objects[0])) + ")";
			if (objects.Length > 1)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					" and ",
					objects.Length - 1,
					" other",
					(objects.Length <= 2) ? "" : "s"
				});
			}
			GUILayoutOption[] options = new GUILayoutOption[]
			{
				GUILayout.Height(16f)
			};
			GUILayout.Label(EditorGUIUtility.TempContent(text, AssetPreview.GetMiniThumbnail(objects[0])), EditorStyles.boldLabel, options);
		}

		internal static bool BitToggleField(string label, SerializedProperty bitFieldProperty, int flag)
		{
			bool flag2 = (bitFieldProperty.intValue & flag) != 0;
			bool flag3 = (bitFieldProperty.hasMultipleDifferentValuesBitwise & flag) != 0;
			EditorGUI.showMixedValue = flag3;
			EditorGUI.BeginChangeCheck();
			flag2 = EditorGUILayout.Toggle(label, flag2, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				if (flag3)
				{
					flag2 = true;
				}
				flag3 = false;
				int index = -1;
				for (int i = 0; i < 32; i++)
				{
					if ((1 << i & flag) != 0)
					{
						index = i;
						break;
					}
				}
				bitFieldProperty.SetBitAtIndexForAllTargetsImmediate(index, flag2);
			}
			EditorGUI.showMixedValue = false;
			return flag2 && !flag3;
		}

		internal static void SortingLayerField(GUIContent label, SerializedProperty layerID, GUIStyle style)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, new GUILayoutOption[0]);
			EditorGUI.SortingLayerField(position, label, layerID, style, EditorStyles.label);
		}

		internal static string TextFieldDropDown(string text, string[] dropDownElement)
		{
			return EditorGUILayout.TextFieldDropDown(GUIContent.none, text, dropDownElement);
		}

		internal static string TextFieldDropDown(GUIContent label, string text, string[] dropDownElement)
		{
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textField);
			return EditorGUI.TextFieldDropDown(rect, label, text, dropDownElement);
		}

		internal static string DelayedTextFieldDropDown(string text, string[] dropDownElement)
		{
			return EditorGUILayout.DelayedTextFieldDropDown(GUIContent.none, text, dropDownElement);
		}

		internal static string DelayedTextFieldDropDown(GUIContent label, string text, string[] dropDownElement)
		{
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textFieldDropDownText);
			return EditorGUI.DelayedTextFieldDropDown(rect, label, text, dropDownElement);
		}

		public static bool DropdownButton(GUIContent content, FocusType focusType, params GUILayoutOption[] options)
		{
			return EditorGUILayout.DropdownButton(content, focusType, "MiniPullDown", options);
		}

		public static bool DropdownButton(GUIContent content, FocusType focusType, GUIStyle style, params GUILayoutOption[] options)
		{
			EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(content, style, options);
			return EditorGUI.DropdownButton(EditorGUILayout.s_LastRect, content, focusType, style);
		}

		internal static Color ColorBrightnessField(GUIContent label, Color value, float minBrightness, float maxBrightness, params GUILayoutOption[] options)
		{
			Rect r = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, options);
			return EditorGUI.ColorBrightnessField(r, label, value, minBrightness, maxBrightness);
		}

		internal static Gradient GradientField(Gradient value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
			return EditorGUI.GradientField(position, value);
		}

		internal static Gradient GradientField(string label, Gradient value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
			return EditorGUI.GradientField(label, position, value);
		}

		internal static Gradient GradientField(GUIContent label, Gradient value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
			return EditorGUI.GradientField(label, position, value);
		}

		internal static Gradient GradientField(SerializedProperty value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
			return EditorGUI.GradientField(position, value);
		}

		internal static Gradient GradientField(string label, SerializedProperty value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
			return EditorGUI.GradientField(label, position, value);
		}

		internal static Gradient GradientField(GUIContent label, SerializedProperty value, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
			return EditorGUI.GradientField(label, position, value);
		}

		internal static Color HexColorTextField(GUIContent label, Color color, bool showAlpha, params GUILayoutOption[] options)
		{
			return EditorGUILayout.HexColorTextField(label, color, showAlpha, EditorStyles.textField, options);
		}

		internal static Color HexColorTextField(GUIContent label, Color color, bool showAlpha, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, options);
			return EditorGUI.HexColorTextField(rect, label, color, showAlpha, style);
		}

		internal static bool IconButton(int id, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			EditorGUILayout.s_LastRect = GUILayoutUtility.GetRect(content, style, options);
			return EditorGUI.IconButton(id, EditorGUILayout.s_LastRect, content, style);
		}

		internal static void GameViewSizePopup(GameViewSizeGroupType groupType, int selectedIndex, IGameViewSizeMenuUser gameView, GUIStyle style, params GUILayoutOption[] options)
		{
			EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, options);
			EditorGUI.GameViewSizePopup(EditorGUILayout.s_LastRect, groupType, selectedIndex, gameView, style);
		}

		internal static void SortingLayerField(GUIContent label, SerializedProperty layerID, GUIStyle style, GUIStyle labelStyle)
		{
			EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, 16f, style, new GUILayoutOption[0]);
			EditorGUI.SortingLayerField(EditorGUILayout.s_LastRect, label, layerID, style, labelStyle);
		}

		public static float Knob(Vector2 knobSize, float value, float minValue, float maxValue, string unit, Color backgroundColor, Color activeColor, bool showValue, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(false, knobSize.y, options);
			return EditorGUI.Knob(position, knobSize, value, minValue, maxValue, unit, backgroundColor, activeColor, showValue, GUIUtility.GetControlID("Knob".GetHashCode(), FocusType.Passive, position));
		}

		internal static void TargetChoiceField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
		{
			if (EditorGUILayout.<>f__mg$cache0 == null)
			{
				EditorGUILayout.<>f__mg$cache0 = new TargetChoiceHandler.TargetChoiceMenuFunction(TargetChoiceHandler.SetToValueOfTarget);
			}
			EditorGUILayout.TargetChoiceField(property, label, EditorGUILayout.<>f__mg$cache0, options);
		}

		internal static void TargetChoiceField(SerializedProperty property, GUIContent label, TargetChoiceHandler.TargetChoiceMenuFunction func, params GUILayoutOption[] options)
		{
			Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.popup, options);
			EditorGUI.TargetChoiceField(rect, property, label, func);
		}
	}
}
