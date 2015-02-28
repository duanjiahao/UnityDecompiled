using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AudioLowPassFilter))]
	internal class AudioLowPassFilterInspector : Editor
	{
		private SerializedProperty m_LowpassResonanceQ;
		private SerializedProperty m_CutoffFrequency;
		private SerializedProperty m_LowpassLevelCustomCurve;
		private void OnEnable()
		{
			this.m_LowpassResonanceQ = base.serializedObject.FindProperty("m_LowpassResonanceQ");
			this.m_CutoffFrequency = base.serializedObject.FindProperty("m_CutoffFrequency");
			this.m_LowpassLevelCustomCurve = base.serializedObject.FindProperty("lowpassLevelCustomCurve");
		}
		public override void OnInspectorGUI()
		{
			if (!Application.HasAdvancedLicense())
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUIContent content = new GUIContent("This is only available in the Pro version of Unity.");
				GUILayout.Label(content, EditorStyles.helpBox, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
			}
			base.serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			AudioSourceInspector.AnimProp(new GUIContent("Cutoff Frequency"), this.m_LowpassLevelCustomCurve, 22000f, 0f, true);
			if (EditorGUI.EndChangeCheck())
			{
				AnimationCurve animationCurveValue = this.m_LowpassLevelCustomCurve.animationCurveValue;
				if (animationCurveValue.length > 0)
				{
					this.m_CutoffFrequency.floatValue = Mathf.Lerp(22000f, 0f, animationCurveValue.keys[0].value);
				}
			}
			EditorGUILayout.PropertyField(this.m_LowpassResonanceQ, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
