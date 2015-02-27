using System;
using UnityEngine;
namespace UnityEditor
{
	internal class GenericInspector : Editor
	{
		private AudioFilterGUI m_AudioFilterGUI;
		internal override bool GetOptimizedGUIBlock(bool isDirty, bool isVisible, out OptimizedGUIBlock block, out float height)
		{
			bool optimizedGUIBlockImplementation = base.GetOptimizedGUIBlockImplementation(isDirty, isVisible, out block, out height);
			return (!(this.target is MonoBehaviour) || !AudioUtil.HaveAudioCallback(this.target as MonoBehaviour) || AudioUtil.GetCustomFilterChannelCount(this.target as MonoBehaviour) <= 0) && !this.IsMissingMonoBehaviourTarget() && optimizedGUIBlockImplementation;
		}
		internal override void OnHeaderIconGUI(Rect iconRect)
		{
			if (this.target.GetType() != typeof(GUISkin) && this.target.GetType().IsSubclassOf(typeof(ScriptableObject)))
			{
				Texture2D image = EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D;
				GUI.Label(iconRect, image);
			}
			else
			{
				base.OnHeaderIconGUI(iconRect);
			}
		}
		internal override bool OnOptimizedInspectorGUI(Rect contentRect)
		{
			return base.OptimizedInspectorGUIImplementation(contentRect);
		}
		public bool MissingMonoBehaviourGUI()
		{
			base.serializedObject.Update();
			SerializedProperty serializedProperty = base.serializedObject.FindProperty("m_Script");
			if (serializedProperty == null)
			{
				return false;
			}
			EditorGUILayout.PropertyField(serializedProperty, new GUILayoutOption[0]);
			MonoScript monoScript = serializedProperty.objectReferenceValue as MonoScript;
			bool flag = true;
			if (monoScript != null && monoScript.GetScriptTypeWasJustCreatedFromComponentMenu())
			{
				flag = false;
			}
			if (flag)
			{
				GUIContent gUIContent = EditorGUIUtility.TextContent("GenericInspector.ScriptIsInvalid");
				EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning, true);
			}
			if (base.serializedObject.ApplyModifiedProperties())
			{
				EditorUtility.ForceRebuildInspectors();
			}
			return true;
		}
		private bool IsMissingMonoBehaviourTarget()
		{
			return this.target.GetType() == typeof(MonoBehaviour) || this.target.GetType() == typeof(ScriptableObject);
		}
		public override void OnInspectorGUI()
		{
			if (this.IsMissingMonoBehaviourTarget() && this.MissingMonoBehaviourGUI())
			{
				return;
			}
			base.OnInspectorGUI();
			if (this.target is MonoBehaviour && AudioUtil.HaveAudioCallback(this.target as MonoBehaviour) && AudioUtil.GetCustomFilterChannelCount(this.target as MonoBehaviour) > 0)
			{
				if (this.m_AudioFilterGUI == null)
				{
					this.m_AudioFilterGUI = new AudioFilterGUI();
				}
				this.m_AudioFilterGUI.DrawAudioFilterGUI(this.target as MonoBehaviour);
			}
		}
	}
}
