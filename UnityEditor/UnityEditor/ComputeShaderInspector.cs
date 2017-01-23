using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(ComputeShader))]
	internal class ComputeShaderInspector : Editor
	{
		internal class Styles
		{
			public static GUIContent showAll = EditorGUIUtility.TextContent("Show code");
		}

		private const float kSpace = 5f;

		private Vector2 m_ScrollPosition = Vector2.zero;

		public virtual void OnEnable()
		{
		}

		public override void OnInspectorGUI()
		{
			ComputeShader computeShader = base.target as ComputeShader;
			if (!(computeShader == null))
			{
				GUI.enabled = true;
				EditorGUI.indentLevel = 0;
				this.ShowDebuggingData(computeShader);
				this.ShowShaderErrors(computeShader);
			}
		}

		private void ShowDebuggingData(ComputeShader cs)
		{
			GUILayout.Space(5f);
			GUILayout.Label("Compiled code:", EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("All variants", EditorStyles.miniButton);
			if (GUILayout.Button(ComputeShaderInspector.Styles.showAll, EditorStyles.miniButton, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				ShaderUtil.OpenCompiledComputeShader(cs, true);
				GUIUtility.ExitGUI();
			}
			EditorGUILayout.EndHorizontal();
		}

		private void ShowShaderErrors(ComputeShader s)
		{
			int computeShaderErrorCount = ShaderUtil.GetComputeShaderErrorCount(s);
			if (computeShaderErrorCount >= 1)
			{
				ShaderInspector.ShaderErrorListUI(s, ShaderUtil.GetComputeShaderErrors(s), ref this.m_ScrollPosition);
			}
		}
	}
}
