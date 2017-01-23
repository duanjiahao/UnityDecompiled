using System;
using UnityEngine;

namespace UnityEditor
{
	internal class GenericInspector : Editor
	{
		private AudioFilterGUI m_AudioFilterGUI = null;

		internal override bool GetOptimizedGUIBlock(bool isDirty, bool isVisible, out OptimizedGUIBlock block, out float height)
		{
			bool optimizedGUIBlockImplementation = base.GetOptimizedGUIBlockImplementation(isDirty, isVisible, out block, out height);
			bool result;
			if (base.target is MonoBehaviour)
			{
				if (AudioUtil.HaveAudioCallback(base.target as MonoBehaviour) && AudioUtil.GetCustomFilterChannelCount(base.target as MonoBehaviour) > 0)
				{
					result = false;
					return result;
				}
			}
			result = (!this.IsMissingMonoBehaviourTarget() && optimizedGUIBlockImplementation);
			return result;
		}

		internal override bool OnOptimizedInspectorGUI(Rect contentRect)
		{
			return base.OptimizedInspectorGUIImplementation(contentRect);
		}

		public bool MissingMonoBehaviourGUI()
		{
			base.serializedObject.Update();
			SerializedProperty serializedProperty = base.serializedObject.FindProperty("m_Script");
			bool result;
			if (serializedProperty == null)
			{
				result = false;
			}
			else
			{
				EditorGUILayout.PropertyField(serializedProperty, new GUILayoutOption[0]);
				MonoScript monoScript = serializedProperty.objectReferenceValue as MonoScript;
				bool flag = true;
				if (monoScript != null && monoScript.GetScriptTypeWasJustCreatedFromComponentMenu())
				{
					flag = false;
				}
				if (flag)
				{
					GUIContent gUIContent = EditorGUIUtility.TextContent("The associated script can not be loaded.\nPlease fix any compile errors\nand assign a valid script.");
					EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning, true);
				}
				if (base.serializedObject.ApplyModifiedProperties())
				{
					EditorUtility.ForceRebuildInspectors();
				}
				result = true;
			}
			return result;
		}

		private bool IsMissingMonoBehaviourTarget()
		{
			return base.target.GetType() == typeof(MonoBehaviour) || base.target.GetType() == typeof(ScriptableObject);
		}

		public override void OnInspectorGUI()
		{
			if (!this.IsMissingMonoBehaviourTarget() || !this.MissingMonoBehaviourGUI())
			{
				base.OnInspectorGUI();
				if (base.target is MonoBehaviour)
				{
					if (AudioUtil.HaveAudioCallback(base.target as MonoBehaviour) && AudioUtil.GetCustomFilterChannelCount(base.target as MonoBehaviour) > 0)
					{
						if (this.m_AudioFilterGUI == null)
						{
							this.m_AudioFilterGUI = new AudioFilterGUI();
						}
						this.m_AudioFilterGUI.DrawAudioFilterGUI(base.target as MonoBehaviour);
					}
				}
			}
		}
	}
}
