using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal abstract class ModuleUI : SerializedModule
	{
		public enum VisibilityState
		{
			NotVisible,
			VisibleAndFolded,
			VisibleAndFoldedOut
		}

		public delegate bool CurveFieldMouseDownCallback(int button, Rect drawRect, Rect curveRanges);

		private class CurveStateCallbackData
		{
			public SerializedMinMaxCurve[] minMaxCurves;

			public MinMaxCurveState selectedState;

			public CurveStateCallbackData(MinMaxCurveState state, SerializedMinMaxCurve[] curves)
			{
				this.minMaxCurves = curves;
				this.selectedState = state;
			}
		}

		private class GradientCallbackData
		{
			public SerializedMinMaxGradient gradientProp;

			public MinMaxGradientState selectedState;

			public GradientCallbackData(MinMaxGradientState state, SerializedMinMaxGradient p)
			{
				this.gradientProp = p;
				this.selectedState = state;
			}
		}

		private class ColorCallbackData
		{
			public SerializedProperty boolProp;

			public bool selectedState;

			public ColorCallbackData(bool state, SerializedProperty bp)
			{
				this.boolProp = bp;
				this.selectedState = state;
			}
		}

		public ParticleSystemUI m_ParticleSystemUI;

		private string m_DisplayName;

		protected string m_ToolTip = "";

		private SerializedProperty m_Enabled;

		private ModuleUI.VisibilityState m_VisibilityState;

		public List<SerializedProperty> m_ModuleCurves = new List<SerializedProperty>();

		private List<SerializedProperty> m_CurvesRemovedWhenFolded = new List<SerializedProperty>();

		protected static readonly bool kUseSignedRange = true;

		protected static readonly Rect kUnsignedRange = new Rect(0f, 0f, 1f, 1f);

		protected static readonly Rect kSignedRange = new Rect(0f, -1f, 1f, 2f);

		protected const int kSingleLineHeight = 13;

		protected const float k_minMaxToggleWidth = 13f;

		protected const float k_toggleWidth = 9f;

		protected const float kDragSpace = 20f;

		protected const int kPlusAddRemoveButtonWidth = 12;

		protected const int kPlusAddRemoveButtonSpacing = 5;

		protected const int kSpacingSubLabel = 4;

		protected const int kSubLabelWidth = 10;

		protected const string kFormatString = "g7";

		public static float k_CompactFixedModuleWidth = 295f;

		public static float k_SpaceBetweenModules = 5f;

		private static readonly GUIStyle s_ControlRectStyle = new GUIStyle
		{
			margin = new RectOffset(0, 0, 2, 2)
		};

		[CompilerGenerated]
		private static GenericMenu.MenuFunction2 <>f__mg$cache0;

		[CompilerGenerated]
		private static GenericMenu.MenuFunction2 <>f__mg$cache1;

		[CompilerGenerated]
		private static GenericMenu.MenuFunction2 <>f__mg$cache2;

		public bool visibleUI
		{
			get
			{
				return this.m_VisibilityState != ModuleUI.VisibilityState.NotVisible;
			}
			set
			{
				this.SetVisibilityState((!value) ? ModuleUI.VisibilityState.NotVisible : ModuleUI.VisibilityState.VisibleAndFolded);
			}
		}

		public bool foldout
		{
			get
			{
				return this.m_VisibilityState == ModuleUI.VisibilityState.VisibleAndFoldedOut;
			}
			set
			{
				this.SetVisibilityState((!value) ? ModuleUI.VisibilityState.VisibleAndFolded : ModuleUI.VisibilityState.VisibleAndFoldedOut);
			}
		}

		public bool enabled
		{
			get
			{
				return this.m_Enabled.boolValue;
			}
			set
			{
				if (this.m_Enabled.boolValue != value)
				{
					this.m_Enabled.boolValue = value;
					if (value)
					{
						this.OnModuleEnable();
					}
					else
					{
						this.OnModuleDisable();
					}
				}
			}
		}

		public string displayName
		{
			get
			{
				return this.m_DisplayName;
			}
		}

		public string toolTip
		{
			get
			{
				return this.m_ToolTip;
			}
		}

		public ModuleUI(ParticleSystemUI owner, SerializedObject o, string name, string displayName) : base(o, name)
		{
			this.Setup(owner, o, name, displayName, ModuleUI.VisibilityState.NotVisible);
		}

		public ModuleUI(ParticleSystemUI owner, SerializedObject o, string name, string displayName, ModuleUI.VisibilityState initialVisibilityState) : base(o, name)
		{
			this.Setup(owner, o, name, displayName, initialVisibilityState);
		}

		private void Setup(ParticleSystemUI owner, SerializedObject o, string name, string displayName, ModuleUI.VisibilityState defaultVisibilityState)
		{
			this.m_ParticleSystemUI = owner;
			this.m_DisplayName = displayName;
			if (this is RendererModuleUI)
			{
				this.m_Enabled = base.GetProperty0("m_Enabled");
			}
			else
			{
				this.m_Enabled = base.GetProperty("enabled");
			}
			this.m_VisibilityState = (ModuleUI.VisibilityState)SessionState.GetInt(base.GetUniqueModuleName(), (int)defaultVisibilityState);
			this.CheckVisibilityState();
			if (this.foldout)
			{
				this.Init();
			}
		}

		protected abstract void Init();

		public virtual void Validate()
		{
		}

		public virtual float GetXAxisScalar()
		{
			return 1f;
		}

		public abstract void OnInspectorGUI(ParticleSystem s);

		public virtual void OnSceneGUI(ParticleSystem s, InitialModuleUI initial)
		{
		}

		public virtual void UpdateCullingSupportedString(ref string text)
		{
		}

		protected virtual void OnModuleEnable()
		{
			this.Init();
		}

		protected virtual void OnModuleDisable()
		{
			ParticleSystemCurveEditor particleSystemCurveEditor = this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();
			foreach (SerializedProperty current in this.m_ModuleCurves)
			{
				if (particleSystemCurveEditor.IsAdded(current))
				{
					particleSystemCurveEditor.RemoveCurve(current);
				}
			}
		}

		internal void CheckVisibilityState()
		{
			if (!(this is RendererModuleUI) && !this.m_Enabled.boolValue && !ParticleEffectUI.GetAllModulesVisible())
			{
				this.SetVisibilityState(ModuleUI.VisibilityState.NotVisible);
			}
			if (this.m_Enabled.boolValue && !this.visibleUI)
			{
				this.SetVisibilityState(ModuleUI.VisibilityState.VisibleAndFolded);
			}
		}

		protected virtual void SetVisibilityState(ModuleUI.VisibilityState newState)
		{
			if (newState != this.m_VisibilityState)
			{
				if (newState == ModuleUI.VisibilityState.VisibleAndFolded)
				{
					ParticleSystemCurveEditor particleSystemCurveEditor = this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();
					foreach (SerializedProperty current in this.m_ModuleCurves)
					{
						if (particleSystemCurveEditor.IsAdded(current))
						{
							this.m_CurvesRemovedWhenFolded.Add(current);
							particleSystemCurveEditor.SetVisible(current, false);
						}
					}
					particleSystemCurveEditor.Refresh();
				}
				else if (newState == ModuleUI.VisibilityState.VisibleAndFoldedOut)
				{
					ParticleSystemCurveEditor particleSystemCurveEditor2 = this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();
					foreach (SerializedProperty current2 in this.m_CurvesRemovedWhenFolded)
					{
						particleSystemCurveEditor2.SetVisible(current2, true);
					}
					this.m_CurvesRemovedWhenFolded.Clear();
					particleSystemCurveEditor2.Refresh();
				}
				this.m_VisibilityState = newState;
				SessionState.SetInt(base.GetUniqueModuleName(), (int)this.m_VisibilityState);
				if (newState == ModuleUI.VisibilityState.VisibleAndFoldedOut)
				{
					this.Init();
				}
			}
		}

		protected ParticleSystem GetParticleSystem()
		{
			return this.m_Enabled.serializedObject.targetObject as ParticleSystem;
		}

		public ParticleSystemCurveEditor GetParticleSystemCurveEditor()
		{
			return this.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor();
		}

		public void AddToModuleCurves(SerializedProperty curveProp)
		{
			this.m_ModuleCurves.Add(curveProp);
			if (!this.foldout)
			{
				this.m_CurvesRemovedWhenFolded.Add(curveProp);
			}
		}

		private static void Label(Rect rect, GUIContent guiContent)
		{
			GUI.Label(rect, guiContent, ParticleSystemStyles.Get().label);
		}

		protected static Rect GetControlRect(int height, params GUILayoutOption[] layoutOptions)
		{
			return GUILayoutUtility.GetRect(0f, (float)height, ModuleUI.s_ControlRectStyle, layoutOptions);
		}

		private static Rect PrefixLabel(Rect totalPosition, GUIContent label)
		{
			Rect result;
			if (!EditorGUI.LabelHasContent(label))
			{
				result = EditorGUI.IndentedRect(totalPosition);
			}
			else
			{
				Rect labelPosition = new Rect(totalPosition.x + EditorGUI.indent, totalPosition.y, EditorGUIUtility.labelWidth - EditorGUI.indent, 13f);
				Rect rect = new Rect(totalPosition.x + EditorGUIUtility.labelWidth, totalPosition.y, totalPosition.width - EditorGUIUtility.labelWidth, totalPosition.height);
				EditorGUI.HandlePrefixLabel(totalPosition, labelPosition, label, 0, ParticleSystemStyles.Get().label);
				result = rect;
			}
			return result;
		}

		private static Rect SubtractPopupWidth(Rect position)
		{
			position.width -= 14f;
			return position;
		}

		private static Rect GetPopupRect(Rect position)
		{
			position.xMin = position.xMax - 13f;
			return position;
		}

		protected static bool PlusButton(Rect position)
		{
			return GUI.Button(new Rect(position.x - 2f, position.y - 2f, 12f, 13f), GUIContent.none, "OL Plus");
		}

		protected static bool MinusButton(Rect position)
		{
			return GUI.Button(new Rect(position.x - 2f, position.y - 2f, 12f, 13f), GUIContent.none, "OL Minus");
		}

		private static float FloatDraggable(Rect rect, SerializedProperty floatProp, float remap, float dragWidth)
		{
			return ModuleUI.FloatDraggable(rect, floatProp, remap, dragWidth, "g7");
		}

		public static float FloatDraggable(Rect rect, float floatValue, float remap, float dragWidth, string formatString)
		{
			int controlID = GUIUtility.GetControlID(1658656233, FocusType.Keyboard, rect);
			Rect dragHotZone = rect;
			dragHotZone.width = dragWidth;
			Rect position = rect;
			position.xMin += dragWidth;
			return EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, position, dragHotZone, controlID, floatValue * remap, formatString, ParticleSystemStyles.Get().numberField, true) / remap;
		}

		public static float FloatDraggable(Rect rect, SerializedProperty floatProp, float remap, float dragWidth, string formatString)
		{
			Color color = GUI.color;
			if (floatProp.isAnimated)
			{
				GUI.color = AnimationMode.animatedPropertyColor;
			}
			float floatValue = floatProp.floatValue;
			float num = ModuleUI.FloatDraggable(rect, floatValue, remap, dragWidth, formatString);
			if (num != floatValue)
			{
				floatProp.floatValue = num;
			}
			GUI.color = color;
			return num;
		}

		public static Vector3 GUIVector3Field(GUIContent guiContent, SerializedProperty vecProp, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, guiContent);
			GUIContent[] array = new GUIContent[]
			{
				new GUIContent("X"),
				new GUIContent("Y"),
				new GUIContent("Z")
			};
			float num = (rect.width - 8f) / 3f;
			rect.width = num;
			Vector3 vector3Value = vecProp.vector3Value;
			for (int i = 0; i < 3; i++)
			{
				ModuleUI.Label(rect, array[i]);
				vector3Value[i] = ModuleUI.FloatDraggable(rect, vector3Value[i], 1f, 25f, "g5");
				rect.x += num + 4f;
			}
			vecProp.vector3Value = vector3Value;
			return vector3Value;
		}

		public static float GUIFloat(string label, SerializedProperty floatProp, params GUILayoutOption[] layoutOptions)
		{
			return ModuleUI.GUIFloat(GUIContent.Temp(label), floatProp, layoutOptions);
		}

		public static float GUIFloat(GUIContent guiContent, SerializedProperty floatProp, params GUILayoutOption[] layoutOptions)
		{
			return ModuleUI.GUIFloat(guiContent, floatProp, "g7", layoutOptions);
		}

		public static float GUIFloat(GUIContent guiContent, SerializedProperty floatProp, string formatString, params GUILayoutOption[] layoutOptions)
		{
			Rect controlRect = ModuleUI.GetControlRect(13, layoutOptions);
			ModuleUI.PrefixLabel(controlRect, guiContent);
			return ModuleUI.FloatDraggable(controlRect, floatProp, 1f, EditorGUIUtility.labelWidth, formatString);
		}

		public static float GUIFloat(GUIContent guiContent, float floatValue, string formatString, params GUILayoutOption[] layoutOptions)
		{
			Rect controlRect = ModuleUI.GetControlRect(13, layoutOptions);
			ModuleUI.PrefixLabel(controlRect, guiContent);
			return ModuleUI.FloatDraggable(controlRect, floatValue, 1f, EditorGUIUtility.labelWidth, formatString);
		}

		private static bool Toggle(Rect rect, SerializedProperty boolProp)
		{
			Color color = GUI.color;
			if (boolProp.isAnimated)
			{
				GUI.color = AnimationMode.animatedPropertyColor;
			}
			bool boolValue = boolProp.boolValue;
			bool flag = EditorGUI.Toggle(rect, boolValue, ParticleSystemStyles.Get().toggle);
			if (boolValue != flag)
			{
				boolProp.boolValue = flag;
			}
			GUI.color = color;
			return flag;
		}

		public static bool GUIToggle(string label, SerializedProperty boolProp, params GUILayoutOption[] layoutOptions)
		{
			return ModuleUI.GUIToggle(GUIContent.Temp(label), boolProp, layoutOptions);
		}

		public static bool GUIToggle(GUIContent guiContent, SerializedProperty boolProp, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, guiContent);
			return ModuleUI.Toggle(rect, boolProp);
		}

		public static void GUILayerMask(GUIContent guiContent, SerializedProperty boolProp, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, guiContent);
			EditorGUI.LayerMaskField(rect, boolProp, GUIContent.none, ParticleSystemStyles.Get().popup);
		}

		public static bool GUIToggle(GUIContent guiContent, bool boolValue, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, guiContent);
			boolValue = EditorGUI.Toggle(rect, boolValue, ParticleSystemStyles.Get().toggle);
			return boolValue;
		}

		public static void GUIToggleWithFloatField(string name, SerializedProperty boolProp, SerializedProperty floatProp, bool invertToggle, params GUILayoutOption[] layoutOptions)
		{
			ModuleUI.GUIToggleWithFloatField(EditorGUIUtility.TempContent(name), boolProp, floatProp, invertToggle, layoutOptions);
		}

		public static void GUIToggleWithFloatField(GUIContent guiContent, SerializedProperty boolProp, SerializedProperty floatProp, bool invertToggle, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = GUILayoutUtility.GetRect(0f, 13f, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, guiContent);
			Rect rect2 = rect;
			rect2.xMax = rect2.x + 9f;
			bool flag = ModuleUI.Toggle(rect2, boolProp);
			flag = ((!invertToggle) ? flag : (!flag));
			if (flag)
			{
				float dragWidth = 25f;
				Rect rect3 = new Rect(rect.x + EditorGUIUtility.labelWidth + 9f, rect.y, rect.width - 9f, rect.height);
				ModuleUI.FloatDraggable(rect3, floatProp, 1f, dragWidth);
			}
		}

		public static void GUIToggleWithIntField(string name, SerializedProperty boolProp, SerializedProperty floatProp, bool invertToggle, params GUILayoutOption[] layoutOptions)
		{
			ModuleUI.GUIToggleWithIntField(EditorGUIUtility.TempContent(name), boolProp, floatProp, invertToggle, layoutOptions);
		}

		public static void GUIToggleWithIntField(GUIContent guiContent, SerializedProperty boolProp, SerializedProperty intProp, bool invertToggle, params GUILayoutOption[] layoutOptions)
		{
			Rect controlRect = ModuleUI.GetControlRect(13, layoutOptions);
			Rect rect = ModuleUI.PrefixLabel(controlRect, guiContent);
			Rect rect2 = rect;
			rect2.xMax = rect2.x + 9f;
			bool flag = ModuleUI.Toggle(rect2, boolProp);
			flag = ((!invertToggle) ? flag : (!flag));
			if (flag)
			{
				float dragWidth = 25f;
				Rect rect3 = new Rect(rect2.xMax, controlRect.y, controlRect.width - rect2.xMax + 9f, controlRect.height);
				intProp.intValue = ModuleUI.IntDraggable(rect3, null, intProp.intValue, dragWidth);
			}
		}

		public static void GUIObject(GUIContent label, SerializedProperty objectProp, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, label);
			EditorGUI.ObjectField(rect, objectProp, null, GUIContent.none, ParticleSystemStyles.Get().objectField);
		}

		public static void GUIObjectFieldAndToggle(GUIContent label, SerializedProperty objectProp, SerializedProperty boolProp, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, label);
			rect.xMax -= 19f;
			EditorGUI.ObjectField(rect, objectProp, GUIContent.none);
			if (boolProp != null)
			{
				rect.x += rect.width + 10f;
				rect.width = 9f;
				ModuleUI.Toggle(rect, boolProp);
			}
		}

		internal UnityEngine.Object ParticleSystemValidator(UnityEngine.Object[] references, Type objType, SerializedProperty property)
		{
			UnityEngine.Object result;
			for (int i = 0; i < references.Length; i++)
			{
				UnityEngine.Object @object = references[i];
				if (@object != null)
				{
					GameObject gameObject = @object as GameObject;
					if (gameObject != null)
					{
						ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
						if (component)
						{
							result = component;
							return result;
						}
					}
				}
			}
			result = null;
			return result;
		}

		public int GUIListOfFloatObjectToggleFields(GUIContent label, SerializedProperty[] objectProps, EditorGUI.ObjectFieldValidator validator, GUIContent buttonTooltip, bool allowCreation, params GUILayoutOption[] layoutOptions)
		{
			int result = -1;
			int num = objectProps.Length;
			Rect rect = GUILayoutUtility.GetRect(0f, (float)(15 * num), layoutOptions);
			rect.height = 13f;
			float num2 = 10f;
			float num3 = 35f;
			float num4 = 10f;
			float width = rect.width - num2 - num3 - num4 * 2f - 9f;
			ModuleUI.PrefixLabel(rect, label);
			for (int i = 0; i < num; i++)
			{
				SerializedProperty serializedProperty = objectProps[i];
				Rect rect2 = new Rect(rect.x + num2 + num3 + num4, rect.y, width, rect.height);
				int controlID = GUIUtility.GetControlID(1235498, FocusType.Keyboard, rect2);
				EditorGUI.DoObjectField(rect2, rect2, controlID, null, null, serializedProperty, validator, true, ParticleSystemStyles.Get().objectField);
				if (serializedProperty.objectReferenceValue == null)
				{
					rect2 = new Rect(rect.xMax - 9f, rect.y + 3f, 9f, 9f);
					if (!allowCreation || GUI.Button(rect2, buttonTooltip ?? GUIContent.none, ParticleSystemStyles.Get().plus))
					{
						result = i;
					}
				}
				rect.y += 15f;
			}
			return result;
		}

		public static int GUIIntDraggable(GUIContent label, SerializedProperty intProp, params GUILayoutOption[] layoutOptions)
		{
			return ModuleUI.GUIIntDraggable(label, intProp.intValue, layoutOptions);
		}

		public static int GUIIntDraggable(GUIContent label, int intValue, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = GUILayoutUtility.GetRect(0f, 13f, layoutOptions);
			return ModuleUI.IntDraggable(rect, label, intValue, EditorGUIUtility.labelWidth);
		}

		public static void GUIIntDraggableX2(GUIContent mainLabel, GUIContent label1, SerializedProperty intProp1, GUIContent label2, SerializedProperty intProp2, params GUILayoutOption[] layoutOptions)
		{
			Rect totalPosition = ModuleUI.GetControlRect(13, layoutOptions);
			totalPosition = ModuleUI.PrefixLabel(totalPosition, mainLabel);
			float num = (totalPosition.width - 4f) * 0.5f;
			Rect rect = new Rect(totalPosition.x, totalPosition.y, num, totalPosition.height);
			ModuleUI.IntDraggable(rect, label1, intProp1, 10f);
			rect.x += num + 4f;
			ModuleUI.IntDraggable(rect, label2, intProp2, 10f);
		}

		public static int IntDraggable(Rect rect, GUIContent label, SerializedProperty intProp, float dragWidth)
		{
			intProp.intValue = ModuleUI.IntDraggable(rect, label, intProp.intValue, dragWidth);
			return intProp.intValue;
		}

		public static int GUIInt(GUIContent guiContent, int intValue, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = GUILayoutUtility.GetRect(0f, 13f, layoutOptions);
			ModuleUI.PrefixLabel(rect, guiContent);
			return ModuleUI.IntDraggable(rect, null, intValue, EditorGUIUtility.labelWidth);
		}

		public static int GUIInt(GUIContent guiContent, SerializedProperty intProp, params GUILayoutOption[] layoutOptions)
		{
			intProp.intValue = ModuleUI.GUIInt(guiContent, intProp.intValue, layoutOptions);
			return intProp.intValue;
		}

		public static int IntDraggable(Rect rect, GUIContent label, int value, float dragWidth)
		{
			float width = rect.width;
			Rect rect2 = rect;
			rect2.width = width;
			int controlID = GUIUtility.GetControlID(16586232, FocusType.Keyboard, rect2);
			Rect rect3 = rect2;
			rect3.width = dragWidth;
			if (label != null && !string.IsNullOrEmpty(label.text))
			{
				ModuleUI.Label(rect3, label);
			}
			Rect position = rect2;
			position.x += dragWidth;
			position.width = width - dragWidth;
			float dragSensitivity = Mathf.Max(1f, Mathf.Pow(Mathf.Abs((float)value), 0.5f) * 0.03f);
			return (int)EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, position, rect3, controlID, (float)value, EditorGUI.kIntFieldFormatString, ParticleSystemStyles.Get().numberField, true, dragSensitivity);
		}

		public static void GUIMinMaxRange(GUIContent label, SerializedProperty vec2Prop, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.SubtractPopupWidth(rect);
			rect = ModuleUI.PrefixLabel(rect, label);
			float num = (rect.width - 20f) * 0.5f;
			Vector2 vector2Value = vec2Prop.vector2Value;
			rect.width = num;
			rect.xMin -= 20f;
			vector2Value.x = ModuleUI.FloatDraggable(rect, vector2Value.x, 1f, 20f, "g7");
			vector2Value.x = Mathf.Clamp(vector2Value.x, 0f, vector2Value.y - 0.01f);
			rect.x += num + 20f;
			vector2Value.y = ModuleUI.FloatDraggable(rect, vector2Value.y, 1f, 20f, "g7");
			vector2Value.y = Mathf.Max(vector2Value.x + 0.01f, vector2Value.y);
			vec2Prop.vector2Value = vector2Value;
		}

		public static void GUISlider(SerializedProperty floatProp, float a, float b, float remap)
		{
			ModuleUI.GUISlider("", floatProp, a, b, remap);
		}

		public static void GUISlider(string name, SerializedProperty floatProp, float a, float b, float remap)
		{
			floatProp.floatValue = EditorGUILayout.Slider(name, floatProp.floatValue * remap, a, b, new GUILayoutOption[]
			{
				GUILayout.MinWidth(300f)
			}) / remap;
		}

		public static void GUIMinMaxSlider(GUIContent label, SerializedProperty vec2Prop, float a, float b, params GUILayoutOption[] layoutOptions)
		{
			Rect controlRect = ModuleUI.GetControlRect(26, layoutOptions);
			Rect rect = controlRect;
			rect.height = 13f;
			rect.y += 3f;
			ModuleUI.PrefixLabel(rect, label);
			Vector2 vector2Value = vec2Prop.vector2Value;
			rect.y += 13f;
			EditorGUI.MinMaxSlider(rect, ref vector2Value.x, ref vector2Value.y, a, b);
			vec2Prop.vector2Value = vector2Value;
		}

		public static bool GUIBoolAsPopup(GUIContent label, SerializedProperty boolProp, string[] options, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, label);
			int num = (!boolProp.boolValue) ? 0 : 1;
			int num2 = EditorGUI.Popup(rect, num, options, ParticleSystemStyles.Get().popup);
			if (num2 != num)
			{
				boolProp.boolValue = (num2 > 0);
			}
			return num2 > 0;
		}

		public static Enum GUIEnumMask(GUIContent label, Enum enumValue, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, label);
			return EditorGUI.EnumMaskField(rect, enumValue, ParticleSystemStyles.Get().popup);
		}

		public static int GUIMask(GUIContent label, int intValue, string[] options, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, label);
			return EditorGUI.MaskField(rect, label, intValue, options, ParticleSystemStyles.Get().popup);
		}

		public static int GUIPopup(string name, SerializedProperty intProp, string[] options, params GUILayoutOption[] layoutOptions)
		{
			return ModuleUI.GUIPopup(GUIContent.Temp(name), intProp, options, layoutOptions);
		}

		public static int GUIPopup(GUIContent label, SerializedProperty intProp, string[] options, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, label);
			intProp.intValue = EditorGUI.Popup(rect, intProp.intValue, options, ParticleSystemStyles.Get().popup);
			return intProp.intValue;
		}

		public static int GUIPopup(GUIContent label, int intValue, string[] options, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			rect = ModuleUI.PrefixLabel(rect, label);
			return EditorGUI.Popup(rect, intValue, options, ParticleSystemStyles.Get().popup);
		}

		private static Color GetColor(SerializedMinMaxCurve mmCurve)
		{
			return mmCurve.m_Module.m_ParticleSystemUI.m_ParticleEffectUI.GetParticleSystemCurveEditor().GetCurveColor(mmCurve.maxCurve);
		}

		private static void GUICurveField(Rect position, SerializedProperty maxCurve, SerializedProperty minCurve, Color color, Rect ranges, ModuleUI.CurveFieldMouseDownCallback mouseDownCallback)
		{
			int controlID = GUIUtility.GetControlID(1321321231, FocusType.Keyboard, position);
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(controlID);
			if (typeForControl != EventType.Repaint)
			{
				if (typeForControl != EventType.ValidateCommand)
				{
					if (typeForControl == EventType.MouseDown)
					{
						if (position.Contains(current.mousePosition))
						{
							if (mouseDownCallback != null && mouseDownCallback(current.button, position, ranges))
							{
								current.Use();
							}
						}
					}
				}
				else if (current.commandName == "UndoRedoPerformed")
				{
					AnimationCurvePreviewCache.ClearCache();
				}
			}
			else
			{
				Rect position2 = position;
				if (minCurve == null)
				{
					EditorGUIUtility.DrawCurveSwatch(position2, null, maxCurve, color, EditorGUI.kCurveBGColor, ranges);
				}
				else
				{
					EditorGUIUtility.DrawRegionSwatch(position2, maxCurve, minCurve, color, EditorGUI.kCurveBGColor, ranges);
				}
				EditorStyles.colorPickerBox.Draw(position2, GUIContent.none, controlID, false);
			}
		}

		public static void GUIMinMaxCurve(string label, SerializedMinMaxCurve mmCurve, params GUILayoutOption[] layoutOptions)
		{
			ModuleUI.GUIMinMaxCurve(GUIContent.Temp(label), mmCurve, layoutOptions);
		}

		public static void GUIMinMaxCurve(GUIContent label, SerializedMinMaxCurve mmCurve, params GUILayoutOption[] layoutOptions)
		{
			Rect rect = ModuleUI.GetControlRect(13, layoutOptions);
			Rect popupRect = ModuleUI.GetPopupRect(rect);
			rect = ModuleUI.SubtractPopupWidth(rect);
			Rect rect2 = ModuleUI.PrefixLabel(rect, label);
			MinMaxCurveState state = mmCurve.state;
			if (state == MinMaxCurveState.k_Scalar)
			{
				float a = ModuleUI.FloatDraggable(rect, mmCurve.scalar, mmCurve.m_RemapValue, EditorGUIUtility.labelWidth);
				if (!mmCurve.signedRange)
				{
					mmCurve.scalar.floatValue = Mathf.Max(a, 0f);
				}
			}
			else if (state == MinMaxCurveState.k_TwoScalars)
			{
				Rect rect3 = rect2;
				rect3.width = (rect2.width - 20f) * 0.5f;
				float num = mmCurve.minConstant;
				float num2 = mmCurve.maxConstant;
				Rect rect4 = rect3;
				rect4.xMin -= 20f;
				EditorGUI.BeginChangeCheck();
				num = ModuleUI.FloatDraggable(rect4, num, mmCurve.m_RemapValue, 20f, "g5");
				if (EditorGUI.EndChangeCheck())
				{
					mmCurve.minConstant = num;
				}
				rect4.x += rect3.width + 20f;
				EditorGUI.BeginChangeCheck();
				num2 = ModuleUI.FloatDraggable(rect4, num2, mmCurve.m_RemapValue, 20f, "g5");
				if (EditorGUI.EndChangeCheck())
				{
					mmCurve.maxConstant = num2;
				}
			}
			else
			{
				Rect ranges = (!mmCurve.signedRange) ? ModuleUI.kUnsignedRange : ModuleUI.kSignedRange;
				SerializedProperty minCurve = (state != MinMaxCurveState.k_TwoCurves) ? null : mmCurve.minCurve;
				ModuleUI.GUICurveField(rect2, mmCurve.maxCurve, minCurve, ModuleUI.GetColor(mmCurve), ranges, new ModuleUI.CurveFieldMouseDownCallback(mmCurve.OnCurveAreaMouseDown));
			}
			ModuleUI.GUIMMCurveStateList(popupRect, mmCurve);
		}

		public void GUIMinMaxGradient(GUIContent label, SerializedMinMaxGradient minMaxGradient, params GUILayoutOption[] layoutOptions)
		{
			MinMaxGradientState state = minMaxGradient.state;
			bool flag = state == MinMaxGradientState.k_RandomBetweenTwoColors || state == MinMaxGradientState.k_RandomBetweenTwoGradients;
			Rect rect = GUILayoutUtility.GetRect(0f, (float)((!flag) ? 13 : 26), layoutOptions);
			Rect popupRect = ModuleUI.GetPopupRect(rect);
			rect = ModuleUI.SubtractPopupWidth(rect);
			Rect rect2 = ModuleUI.PrefixLabel(rect, label);
			rect2.height = 13f;
			switch (state)
			{
			case MinMaxGradientState.k_Color:
				ModuleUI.GUIColor(rect2, minMaxGradient.m_MaxColor);
				break;
			case MinMaxGradientState.k_Gradient:
			case MinMaxGradientState.k_RandomColor:
				EditorGUI.GradientField(rect2, minMaxGradient.m_MaxGradient);
				break;
			case MinMaxGradientState.k_RandomBetweenTwoColors:
				ModuleUI.GUIColor(rect2, minMaxGradient.m_MaxColor);
				rect2.y += rect2.height;
				ModuleUI.GUIColor(rect2, minMaxGradient.m_MinColor);
				break;
			case MinMaxGradientState.k_RandomBetweenTwoGradients:
				EditorGUI.GradientField(rect2, minMaxGradient.m_MaxGradient);
				rect2.y += rect2.height;
				EditorGUI.GradientField(rect2, minMaxGradient.m_MinGradient);
				break;
			}
			ModuleUI.GUIMMGradientPopUp(popupRect, minMaxGradient);
		}

		private static void GUIGradientAsColor(Rect rect, SerializedProperty gradientProp)
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			Color color = SerializedMinMaxGradient.GetGradientAsColor(gradientProp);
			color = EditorGUI.ColorField(rect, color, false, true);
			if (GUI.changed)
			{
				SerializedMinMaxGradient.SetGradientAsColor(gradientProp, color);
			}
			GUI.changed |= changed;
		}

		private static void GUIColor(Rect rect, SerializedProperty colorProp)
		{
			colorProp.colorValue = EditorGUI.ColorField(rect, colorProp.colorValue, false, true);
		}

		public void GUIMinMaxColor(GUIContent label, SerializedMinMaxColor minMaxColor, params GUILayoutOption[] layoutOptions)
		{
			Rect totalPosition = ModuleUI.GetControlRect(13, layoutOptions);
			totalPosition = ModuleUI.PrefixLabel(totalPosition, label);
			float num = totalPosition.width - 13f - 5f;
			if (!minMaxColor.minMax.boolValue)
			{
				Rect rect = new Rect(totalPosition.x, totalPosition.y, num, totalPosition.height);
				ModuleUI.GUIColor(rect, minMaxColor.maxColor);
			}
			else
			{
				Rect rect2 = new Rect(totalPosition.x, totalPosition.y, num * 0.5f - 2f, totalPosition.height);
				ModuleUI.GUIColor(rect2, minMaxColor.minColor);
				rect2.x += rect2.width + 4f;
				ModuleUI.GUIColor(rect2, minMaxColor.maxColor);
			}
			Rect rect3 = new Rect(totalPosition.xMax - 13f, totalPosition.y, 13f, 13f);
			ModuleUI.GUIMMColorPopUp(rect3, minMaxColor.minMax);
		}

		public void GUITripleMinMaxCurve(GUIContent label, GUIContent x, SerializedMinMaxCurve xCurve, GUIContent y, SerializedMinMaxCurve yCurve, GUIContent z, SerializedMinMaxCurve zCurve, SerializedProperty randomizePerFrame, params GUILayoutOption[] layoutOptions)
		{
			MinMaxCurveState state = xCurve.state;
			bool flag = label != GUIContent.none;
			int num = (!flag) ? 1 : 2;
			if (state == MinMaxCurveState.k_TwoScalars)
			{
				num++;
			}
			Rect rect = ModuleUI.GetControlRect(13 * num, layoutOptions);
			Rect popupRect = ModuleUI.GetPopupRect(rect);
			rect = ModuleUI.SubtractPopupWidth(rect);
			Rect rect2 = rect;
			float num2 = (rect.width - 8f) / 3f;
			if (num > 1)
			{
				rect2.height = 13f;
			}
			if (flag)
			{
				ModuleUI.PrefixLabel(rect, label);
				rect2.y += rect2.height;
			}
			rect2.width = num2;
			GUIContent[] array = new GUIContent[]
			{
				x,
				y,
				z
			};
			SerializedMinMaxCurve[] array2 = new SerializedMinMaxCurve[]
			{
				xCurve,
				yCurve,
				zCurve
			};
			if (state == MinMaxCurveState.k_Scalar)
			{
				for (int i = 0; i < array2.Length; i++)
				{
					ModuleUI.Label(rect2, array[i]);
					float a = ModuleUI.FloatDraggable(rect2, array2[i].scalar, array2[i].m_RemapValue, 10f);
					if (!array2[i].signedRange)
					{
						array2[i].scalar.floatValue = Mathf.Max(a, 0f);
					}
					rect2.x += num2 + 4f;
				}
			}
			else if (state == MinMaxCurveState.k_TwoScalars)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					ModuleUI.Label(rect2, array[j]);
					float num3 = array2[j].minConstant;
					float num4 = array2[j].maxConstant;
					EditorGUI.BeginChangeCheck();
					num4 = ModuleUI.FloatDraggable(rect2, num4, array2[j].m_RemapValue, 10f, "g5");
					if (EditorGUI.EndChangeCheck())
					{
						array2[j].maxConstant = num4;
					}
					rect2.y += 13f;
					EditorGUI.BeginChangeCheck();
					num3 = ModuleUI.FloatDraggable(rect2, num3, array2[j].m_RemapValue, 10f, "g5");
					if (EditorGUI.EndChangeCheck())
					{
						array2[j].minConstant = num3;
					}
					rect2.x += num2 + 4f;
					rect2.y -= 13f;
				}
			}
			else
			{
				rect2.width = num2;
				Rect ranges = (!xCurve.signedRange) ? ModuleUI.kUnsignedRange : ModuleUI.kSignedRange;
				for (int k = 0; k < array2.Length; k++)
				{
					ModuleUI.Label(rect2, array[k]);
					Rect position = rect2;
					position.xMin += 10f;
					SerializedProperty minCurve = (state != MinMaxCurveState.k_TwoCurves) ? null : array2[k].minCurve;
					ModuleUI.GUICurveField(position, array2[k].maxCurve, minCurve, ModuleUI.GetColor(array2[k]), ranges, new ModuleUI.CurveFieldMouseDownCallback(array2[k].OnCurveAreaMouseDown));
					rect2.x += num2 + 4f;
				}
			}
			ModuleUI.GUIMMCurveStateList(popupRect, array2);
		}

		private static void SelectMinMaxCurveStateCallback(object obj)
		{
			ModuleUI.CurveStateCallbackData curveStateCallbackData = (ModuleUI.CurveStateCallbackData)obj;
			SerializedMinMaxCurve[] minMaxCurves = curveStateCallbackData.minMaxCurves;
			for (int i = 0; i < minMaxCurves.Length; i++)
			{
				SerializedMinMaxCurve serializedMinMaxCurve = minMaxCurves[i];
				serializedMinMaxCurve.state = curveStateCallbackData.selectedState;
			}
		}

		public static void GUIMMCurveStateList(Rect rect, SerializedMinMaxCurve minMaxCurves)
		{
			SerializedMinMaxCurve[] minMaxCurves2 = new SerializedMinMaxCurve[]
			{
				minMaxCurves
			};
			ModuleUI.GUIMMCurveStateList(rect, minMaxCurves2);
		}

		public static void GUIMMCurveStateList(Rect rect, SerializedMinMaxCurve[] minMaxCurves)
		{
			if (EditorGUI.ButtonMouseDown(rect, GUIContent.none, FocusType.Passive, ParticleSystemStyles.Get().minMaxCurveStateDropDown))
			{
				if (minMaxCurves.Length != 0)
				{
					GUIContent[] array = new GUIContent[]
					{
						new GUIContent("Constant"),
						new GUIContent("Curve"),
						new GUIContent("Random Between Two Constants"),
						new GUIContent("Random Between Two Curves")
					};
					MinMaxCurveState[] array2 = new MinMaxCurveState[]
					{
						MinMaxCurveState.k_Scalar,
						MinMaxCurveState.k_Curve,
						MinMaxCurveState.k_TwoScalars,
						MinMaxCurveState.k_TwoCurves
					};
					bool[] array3 = new bool[]
					{
						minMaxCurves[0].m_AllowConstant,
						minMaxCurves[0].m_AllowCurves,
						minMaxCurves[0].m_AllowRandom,
						minMaxCurves[0].m_AllowRandom && minMaxCurves[0].m_AllowCurves
					};
					GenericMenu genericMenu = new GenericMenu();
					for (int i = 0; i < array.Length; i++)
					{
						if (array3[i])
						{
							GenericMenu arg_10C_0 = genericMenu;
							GUIContent arg_10C_1 = array[i];
							bool arg_10C_2 = minMaxCurves[0].state == array2[i];
							if (ModuleUI.<>f__mg$cache0 == null)
							{
								ModuleUI.<>f__mg$cache0 = new GenericMenu.MenuFunction2(ModuleUI.SelectMinMaxCurveStateCallback);
							}
							arg_10C_0.AddItem(arg_10C_1, arg_10C_2, ModuleUI.<>f__mg$cache0, new ModuleUI.CurveStateCallbackData(array2[i], minMaxCurves));
						}
					}
					genericMenu.DropDown(rect);
					Event.current.Use();
				}
			}
		}

		private static void SelectMinMaxGradientStateCallback(object obj)
		{
			ModuleUI.GradientCallbackData gradientCallbackData = (ModuleUI.GradientCallbackData)obj;
			gradientCallbackData.gradientProp.state = gradientCallbackData.selectedState;
		}

		public static void GUIMMGradientPopUp(Rect rect, SerializedMinMaxGradient gradientProp)
		{
			if (EditorGUI.ButtonMouseDown(rect, GUIContent.none, FocusType.Passive, ParticleSystemStyles.Get().minMaxCurveStateDropDown))
			{
				GUIContent[] array = new GUIContent[]
				{
					new GUIContent("Color"),
					new GUIContent("Gradient"),
					new GUIContent("Random Between Two Colors"),
					new GUIContent("Random Between Two Gradients"),
					new GUIContent("Random Color")
				};
				MinMaxGradientState[] array2 = new MinMaxGradientState[]
				{
					MinMaxGradientState.k_Color,
					MinMaxGradientState.k_Gradient,
					MinMaxGradientState.k_RandomBetweenTwoColors,
					MinMaxGradientState.k_RandomBetweenTwoGradients,
					MinMaxGradientState.k_RandomColor
				};
				bool[] array3 = new bool[]
				{
					gradientProp.m_AllowColor,
					gradientProp.m_AllowGradient,
					gradientProp.m_AllowRandomBetweenTwoColors,
					gradientProp.m_AllowRandomBetweenTwoGradients,
					gradientProp.m_AllowRandomColor
				};
				GenericMenu genericMenu = new GenericMenu();
				for (int i = 0; i < array.Length; i++)
				{
					if (array3[i])
					{
						GenericMenu arg_FB_0 = genericMenu;
						GUIContent arg_FB_1 = array[i];
						bool arg_FB_2 = gradientProp.state == array2[i];
						if (ModuleUI.<>f__mg$cache1 == null)
						{
							ModuleUI.<>f__mg$cache1 = new GenericMenu.MenuFunction2(ModuleUI.SelectMinMaxGradientStateCallback);
						}
						arg_FB_0.AddItem(arg_FB_1, arg_FB_2, ModuleUI.<>f__mg$cache1, new ModuleUI.GradientCallbackData(array2[i], gradientProp));
					}
				}
				genericMenu.ShowAsContext();
				Event.current.Use();
			}
		}

		private static void SelectMinMaxColorStateCallback(object obj)
		{
			ModuleUI.ColorCallbackData colorCallbackData = (ModuleUI.ColorCallbackData)obj;
			colorCallbackData.boolProp.boolValue = colorCallbackData.selectedState;
		}

		public static void GUIMMColorPopUp(Rect rect, SerializedProperty boolProp)
		{
			if (EditorGUI.ButtonMouseDown(rect, GUIContent.none, FocusType.Passive, ParticleSystemStyles.Get().minMaxCurveStateDropDown))
			{
				GenericMenu genericMenu = new GenericMenu();
				GUIContent[] array = new GUIContent[]
				{
					new GUIContent("Constant Color"),
					new GUIContent("Random Between Two Colors")
				};
				bool[] array2 = new bool[]
				{
					default(bool),
					true
				};
				for (int i = 0; i < array.Length; i++)
				{
					GenericMenu arg_8C_0 = genericMenu;
					GUIContent arg_8C_1 = array[i];
					bool arg_8C_2 = boolProp.boolValue == array2[i];
					if (ModuleUI.<>f__mg$cache2 == null)
					{
						ModuleUI.<>f__mg$cache2 = new GenericMenu.MenuFunction2(ModuleUI.SelectMinMaxColorStateCallback);
					}
					arg_8C_0.AddItem(arg_8C_1, arg_8C_2, ModuleUI.<>f__mg$cache2, new ModuleUI.ColorCallbackData(array2[i], boolProp));
				}
				genericMenu.ShowAsContext();
				Event.current.Use();
			}
		}
	}
}
