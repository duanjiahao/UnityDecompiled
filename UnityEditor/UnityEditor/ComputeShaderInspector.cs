using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CustomEditor(typeof(ComputeShader))]
	internal class ComputeShaderInspector : Editor
	{
		private class KernelInfo
		{
			internal string name;

			internal string platforms;
		}

		internal class Styles
		{
			public static GUIContent showCompiled = EditorGUIUtility.TextContent("Show compiled code");

			public static GUIContent kernelsHeading = EditorGUIUtility.TextContent("Kernels:");
		}

		private const float kSpace = 5f;

		private Vector2 m_ScrollPosition = Vector2.zero;

		private static List<ComputeShaderInspector.KernelInfo> GetKernelDisplayInfo(ComputeShader cs)
		{
			List<ComputeShaderInspector.KernelInfo> list = new List<ComputeShaderInspector.KernelInfo>();
			int computeShaderPlatformCount = ShaderUtil.GetComputeShaderPlatformCount(cs);
			for (int i = 0; i < computeShaderPlatformCount; i++)
			{
				GraphicsDeviceType computeShaderPlatformType = ShaderUtil.GetComputeShaderPlatformType(cs, i);
				int computeShaderPlatformKernelCount = ShaderUtil.GetComputeShaderPlatformKernelCount(cs, i);
				for (int j = 0; j < computeShaderPlatformKernelCount; j++)
				{
					string computeShaderPlatformKernelName = ShaderUtil.GetComputeShaderPlatformKernelName(cs, i, j);
					bool flag = false;
					foreach (ComputeShaderInspector.KernelInfo current in list)
					{
						if (current.name == computeShaderPlatformKernelName)
						{
							ComputeShaderInspector.KernelInfo expr_6C = current;
							expr_6C.platforms += ' ';
							ComputeShaderInspector.KernelInfo expr_85 = current;
							expr_85.platforms += computeShaderPlatformType.ToString();
							flag = true;
						}
					}
					if (!flag)
					{
						list.Add(new ComputeShaderInspector.KernelInfo
						{
							name = computeShaderPlatformKernelName,
							platforms = computeShaderPlatformType.ToString()
						});
					}
				}
			}
			return list;
		}

		public override void OnInspectorGUI()
		{
			ComputeShader computeShader = base.target as ComputeShader;
			if (!(computeShader == null))
			{
				GUI.enabled = true;
				EditorGUI.indentLevel = 0;
				this.ShowKernelInfoSection(computeShader);
				this.ShowCompiledCodeSection(computeShader);
				this.ShowShaderErrors(computeShader);
			}
		}

		private void ShowKernelInfoSection(ComputeShader cs)
		{
			GUILayout.Label(ComputeShaderInspector.Styles.kernelsHeading, EditorStyles.boldLabel, new GUILayoutOption[0]);
			List<ComputeShaderInspector.KernelInfo> kernelDisplayInfo = ComputeShaderInspector.GetKernelDisplayInfo(cs);
			foreach (ComputeShaderInspector.KernelInfo current in kernelDisplayInfo)
			{
				EditorGUILayout.LabelField(current.name, current.platforms, new GUILayoutOption[0]);
			}
		}

		private void ShowCompiledCodeSection(ComputeShader cs)
		{
			GUILayout.Space(5f);
			if (GUILayout.Button(ComputeShaderInspector.Styles.showCompiled, EditorStyles.miniButton, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				ShaderUtil.OpenCompiledComputeShader(cs, true);
				GUIUtility.ExitGUI();
			}
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
