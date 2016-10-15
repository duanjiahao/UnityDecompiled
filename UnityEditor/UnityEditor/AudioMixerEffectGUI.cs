using System;
using UnityEditor.Audio;
using UnityEngine;

namespace UnityEditor
{
	internal static class AudioMixerEffectGUI
	{
		private class ExposedParamContext
		{
			public AudioMixerController controller;

			public AudioParameterPath path;

			public ExposedParamContext(AudioMixerController controller, AudioParameterPath path)
			{
				this.controller = controller;
				this.path = path;
			}
		}

		private class ParameterTransitionOverrideContext
		{
			public AudioMixerController controller;

			public GUID parameter;

			public ParameterTransitionType type;

			public ParameterTransitionOverrideContext(AudioMixerController controller, GUID parameter, ParameterTransitionType type)
			{
				this.controller = controller;
				this.parameter = parameter;
				this.type = type;
			}
		}

		private class ParameterTransitionOverrideRemoveContext
		{
			public AudioMixerController controller;

			public GUID parameter;

			public ParameterTransitionOverrideRemoveContext(AudioMixerController controller, GUID parameter)
			{
				this.controller = controller;
				this.parameter = parameter;
			}
		}

		private const string kAudioSliderFloatFormat = "F2";

		private const string kExposedParameterUnicodeChar = " ➔";

		private static AudioMixerDrawUtils.Styles styles
		{
			get
			{
				return AudioMixerDrawUtils.styles;
			}
		}

		public static void EffectHeader(string text)
		{
			GUILayout.Label(text, AudioMixerEffectGUI.styles.headerStyle, new GUILayoutOption[0]);
		}

		public static bool Slider(GUIContent label, ref float value, float displayScale, float displayExponent, string unit, float leftValue, float rightValue, AudioMixerController controller, AudioParameterPath path, params GUILayoutOption[] options)
		{
			EditorGUI.BeginChangeCheck();
			float fieldWidth = EditorGUIUtility.fieldWidth;
			string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
			bool flag = controller.ContainsExposedParameter(path.parameter);
			EditorGUIUtility.fieldWidth = 70f;
			EditorGUI.kFloatFieldFormatString = "F2";
			EditorGUI.s_UnitString = unit;
			GUIContent label2 = label;
			if (flag)
			{
				label2 = GUIContent.Temp(label.text + " ➔", label.tooltip);
			}
			float num = value * displayScale;
			num = EditorGUILayout.PowerSlider(label2, num, leftValue * displayScale, rightValue * displayScale, displayExponent, options);
			EditorGUI.s_UnitString = null;
			EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
			EditorGUIUtility.fieldWidth = fieldWidth;
			if (Event.current.type == EventType.ContextClick && GUILayoutUtility.topLevel.GetLast().Contains(Event.current.mousePosition))
			{
				Event.current.Use();
				GenericMenu genericMenu = new GenericMenu();
				if (!flag)
				{
					genericMenu.AddItem(new GUIContent("Expose '" + path.ResolveStringPath(false) + "' to script"), false, new GenericMenu.MenuFunction2(AudioMixerEffectGUI.ExposePopupCallback), new AudioMixerEffectGUI.ExposedParamContext(controller, path));
				}
				else
				{
					genericMenu.AddItem(new GUIContent("Unexpose"), false, new GenericMenu.MenuFunction2(AudioMixerEffectGUI.UnexposePopupCallback), new AudioMixerEffectGUI.ExposedParamContext(controller, path));
				}
				ParameterTransitionType parameterTransitionType;
				bool transitionTypeOverride = controller.TargetSnapshot.GetTransitionTypeOverride(path.parameter, out parameterTransitionType);
				genericMenu.AddSeparator(string.Empty);
				genericMenu.AddItem(new GUIContent("Linear Snapshot Transition"), parameterTransitionType == ParameterTransitionType.Lerp, new GenericMenu.MenuFunction2(AudioMixerEffectGUI.ParameterTransitionOverrideCallback), new AudioMixerEffectGUI.ParameterTransitionOverrideContext(controller, path.parameter, ParameterTransitionType.Lerp));
				genericMenu.AddItem(new GUIContent("Smoothstep Snapshot Transition"), parameterTransitionType == ParameterTransitionType.Smoothstep, new GenericMenu.MenuFunction2(AudioMixerEffectGUI.ParameterTransitionOverrideCallback), new AudioMixerEffectGUI.ParameterTransitionOverrideContext(controller, path.parameter, ParameterTransitionType.Smoothstep));
				genericMenu.AddItem(new GUIContent("Squared Snapshot Transition"), parameterTransitionType == ParameterTransitionType.Squared, new GenericMenu.MenuFunction2(AudioMixerEffectGUI.ParameterTransitionOverrideCallback), new AudioMixerEffectGUI.ParameterTransitionOverrideContext(controller, path.parameter, ParameterTransitionType.Squared));
				genericMenu.AddItem(new GUIContent("SquareRoot Snapshot Transition"), parameterTransitionType == ParameterTransitionType.SquareRoot, new GenericMenu.MenuFunction2(AudioMixerEffectGUI.ParameterTransitionOverrideCallback), new AudioMixerEffectGUI.ParameterTransitionOverrideContext(controller, path.parameter, ParameterTransitionType.SquareRoot));
				genericMenu.AddItem(new GUIContent("BrickwallStart Snapshot Transition"), parameterTransitionType == ParameterTransitionType.BrickwallStart, new GenericMenu.MenuFunction2(AudioMixerEffectGUI.ParameterTransitionOverrideCallback), new AudioMixerEffectGUI.ParameterTransitionOverrideContext(controller, path.parameter, ParameterTransitionType.BrickwallStart));
				genericMenu.AddItem(new GUIContent("BrickwallEnd Snapshot Transition"), parameterTransitionType == ParameterTransitionType.BrickwallEnd, new GenericMenu.MenuFunction2(AudioMixerEffectGUI.ParameterTransitionOverrideCallback), new AudioMixerEffectGUI.ParameterTransitionOverrideContext(controller, path.parameter, ParameterTransitionType.BrickwallEnd));
				genericMenu.AddSeparator(string.Empty);
				genericMenu.ShowAsContext();
			}
			if (EditorGUI.EndChangeCheck())
			{
				value = num / displayScale;
				return true;
			}
			return false;
		}

		public static void ExposePopupCallback(object obj)
		{
			AudioMixerEffectGUI.ExposedParamContext exposedParamContext = (AudioMixerEffectGUI.ExposedParamContext)obj;
			Undo.RecordObject(exposedParamContext.controller, "Expose Mixer Parameter");
			exposedParamContext.controller.AddExposedParameter(exposedParamContext.path);
			AudioMixerUtility.RepaintAudioMixerAndInspectors();
		}

		public static void UnexposePopupCallback(object obj)
		{
			AudioMixerEffectGUI.ExposedParamContext exposedParamContext = (AudioMixerEffectGUI.ExposedParamContext)obj;
			Undo.RecordObject(exposedParamContext.controller, "Unexpose Mixer Parameter");
			exposedParamContext.controller.RemoveExposedParameter(exposedParamContext.path.parameter);
			AudioMixerUtility.RepaintAudioMixerAndInspectors();
		}

		public static void ParameterTransitionOverrideCallback(object obj)
		{
			AudioMixerEffectGUI.ParameterTransitionOverrideContext parameterTransitionOverrideContext = (AudioMixerEffectGUI.ParameterTransitionOverrideContext)obj;
			Undo.RecordObject(parameterTransitionOverrideContext.controller, "Change Parameter Transition Type");
			if (parameterTransitionOverrideContext.type == ParameterTransitionType.Lerp)
			{
				parameterTransitionOverrideContext.controller.TargetSnapshot.ClearTransitionTypeOverride(parameterTransitionOverrideContext.parameter);
			}
			else
			{
				parameterTransitionOverrideContext.controller.TargetSnapshot.SetTransitionTypeOverride(parameterTransitionOverrideContext.parameter, parameterTransitionOverrideContext.type);
			}
		}

		public static bool PopupButton(GUIContent label, GUIContent buttonContent, GUIStyle style, out Rect buttonRect, params GUILayoutOption[] options)
		{
			if (label != null)
			{
				Rect rect = EditorGUILayout.s_LastRect = EditorGUILayout.GetControlRect(true, 16f, style, options);
				int controlID = GUIUtility.GetControlID("EditorPopup".GetHashCode(), EditorGUIUtility.native, rect);
				buttonRect = EditorGUI.PrefixLabel(rect, controlID, label);
			}
			else
			{
				Rect rect2 = GUILayoutUtility.GetRect(buttonContent, style, options);
				buttonRect = rect2;
			}
			return EditorGUI.ButtonMouseDown(buttonRect, buttonContent, FocusType.Passive, style);
		}
	}
}
