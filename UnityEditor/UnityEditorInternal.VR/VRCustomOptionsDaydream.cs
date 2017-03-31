using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	internal class VRCustomOptionsDaydream : VRCustomOptionsGoogleVR
	{
		private static GUIContent s_ForegroundIconLabel = EditorGUIUtility.TextContent("Foreground Icon|Icon should be a Texture with dimensions of 512px by 512px and a 1:1 aspect ratio.");

		private static GUIContent s_BackgroundIconLabel = EditorGUIUtility.TextContent("Background Icon|Icon should be a Texture with dimensions of 512px by 512px and a 1:1 aspect ratio.");

		private static GUIContent s_SustainedPerformanceModeLabel = EditorGUIUtility.TextContent("Use Sustained Performance Mode|Sustained Performance Mode is intended to provide a consistent level of performance for a prolonged amount of time");

		private SerializedProperty m_DaydreamOnly;

		private SerializedProperty m_DaydreamIcon;

		private SerializedProperty m_DaydreamIconBackground;

		private SerializedProperty m_DaydreamUseSustainedPerformanceMode;

		public override void Initialize(SerializedObject settings)
		{
			this.Initialize(settings, "daydream");
		}

		public override void Initialize(SerializedObject settings, string propertyName)
		{
			base.Initialize(settings, propertyName);
			this.m_DaydreamIcon = base.FindPropertyAssert("daydreamIconForeground");
			this.m_DaydreamIconBackground = base.FindPropertyAssert("daydreamIconBackground");
			this.m_DaydreamUseSustainedPerformanceMode = base.FindPropertyAssert("useSustainedPerformanceMode");
		}

		private Rect DrawTextureUI(Rect rect, GUIContent propLabel, SerializedProperty prop)
		{
			rect.height = 64f;
			GUIContent label = EditorGUI.BeginProperty(rect, propLabel, prop);
			EditorGUI.BeginChangeCheck();
			Texture2D objectReferenceValue = EditorGUI.ObjectField(rect, label, (Texture2D)prop.objectReferenceValue, typeof(Texture2D), false) as Texture2D;
			if (EditorGUI.EndChangeCheck())
			{
				prop.objectReferenceValue = objectReferenceValue;
			}
			EditorGUI.EndProperty();
			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			return rect;
		}

		public override Rect Draw(Rect rect)
		{
			rect = base.Draw(rect);
			rect = this.DrawTextureUI(rect, VRCustomOptionsDaydream.s_ForegroundIconLabel, this.m_DaydreamIcon);
			rect = this.DrawTextureUI(rect, VRCustomOptionsDaydream.s_BackgroundIconLabel, this.m_DaydreamIconBackground);
			rect.height = EditorGUIUtility.singleLineHeight;
			GUIContent label = EditorGUI.BeginProperty(rect, VRCustomOptionsDaydream.s_SustainedPerformanceModeLabel, this.m_DaydreamUseSustainedPerformanceMode);
			EditorGUI.BeginChangeCheck();
			bool boolValue = EditorGUI.Toggle(rect, label, this.m_DaydreamUseSustainedPerformanceMode.boolValue);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_DaydreamUseSustainedPerformanceMode.boolValue = boolValue;
			}
			EditorGUI.EndProperty();
			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			return rect;
		}

		public override float GetHeight()
		{
			return base.GetHeight() + EditorGUIUtility.singleLineHeight * 1f + 128f + EditorGUIUtility.standardVerticalSpacing * 3f;
		}
	}
}
