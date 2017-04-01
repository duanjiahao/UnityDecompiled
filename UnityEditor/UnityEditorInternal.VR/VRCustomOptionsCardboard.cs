using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	internal class VRCustomOptionsCardboard : VRCustomOptionsGoogleVR
	{
		private static GUIContent s_EnableTransitionVewLabel = new GUIContent("Enable Tansition View");

		private SerializedProperty m_EnableTransitionView;

		public override void Initialize(SerializedObject settings)
		{
			this.Initialize(settings, "cardboard");
		}

		public override void Initialize(SerializedObject settings, string propertyName)
		{
			base.Initialize(settings, propertyName);
			this.m_EnableTransitionView = base.FindPropertyAssert("enableTransitionView");
		}

		public override Rect Draw(Rect rect)
		{
			rect = base.Draw(rect);
			rect.height = EditorGUIUtility.singleLineHeight;
			GUIContent label = EditorGUI.BeginProperty(rect, VRCustomOptionsCardboard.s_EnableTransitionVewLabel, this.m_EnableTransitionView);
			EditorGUI.BeginChangeCheck();
			bool boolValue = EditorGUI.Toggle(rect, label, this.m_EnableTransitionView.boolValue);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_EnableTransitionView.boolValue = boolValue;
			}
			EditorGUI.EndProperty();
			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			return rect;
		}

		public override float GetHeight()
		{
			return base.GetHeight() + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}
