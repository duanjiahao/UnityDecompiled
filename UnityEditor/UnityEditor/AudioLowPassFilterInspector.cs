using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AudioLowPassFilter))]
	internal class AudioLowPassFilterInspector : Editor
	{
		private SerializedProperty m_LowpassResonanceQ;

		private SerializedProperty m_LowpassLevelCustomCurve;

		private void OnEnable()
		{
			this.m_LowpassResonanceQ = base.serializedObject.FindProperty("m_LowpassResonanceQ");
			this.m_LowpassLevelCustomCurve = base.serializedObject.FindProperty("lowpassLevelCustomCurve");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			AudioSourceInspector.AnimProp(new GUIContent("Cutoff Frequency"), this.m_LowpassLevelCustomCurve, 0f, 22000f, true);
			EditorGUILayout.PropertyField(this.m_LowpassResonanceQ, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
