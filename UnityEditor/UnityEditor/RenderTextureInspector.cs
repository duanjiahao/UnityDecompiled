using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(RenderTexture))]
	internal class RenderTextureInspector : TextureInspector
	{
		private static GUIContent[] kRenderTextureAntiAliasing = new GUIContent[]
		{
			new GUIContent("None"),
			new GUIContent("2 samples"),
			new GUIContent("4 samples"),
			new GUIContent("8 samples")
		};
		private static int[] kRenderTextureAntiAliasingValues = new int[]
		{
			1,
			2,
			4,
			8
		};
		private static GUIContent[] kRenderTextureDepths = new GUIContent[]
		{
			new GUIContent("None"),
			new GUIContent("16 bit"),
			new GUIContent("24 bit")
		};
		private static int[] kRenderTextureDepthsValues = new int[]
		{
			0,
			1,
			2
		};
		private SerializedProperty m_Width;
		private SerializedProperty m_Height;
		private SerializedProperty m_DepthFormat;
		private SerializedProperty m_AntiAliasing;
		private static string[] kTextureFormatsStrings = new string[]
		{
			"RGBA 32bit",
			"Depth",
			"RGBA 64bit FP",
			"Shadowmap",
			"RGB 16bit",
			"RGBA 16bit",
			"RGBA 16bit (5-1)",
			string.Empty,
			"RGBA 32bit (10-2)",
			string.Empty,
			"RGBA 64bit",
			"RGBA 128bit FP",
			"RG 64bit FP",
			"RG 32bit FP",
			"R 32bit FP",
			"R 16bit FP",
			"R 8bit"
		};
		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_Width = base.serializedObject.FindProperty("m_Width");
			this.m_Height = base.serializedObject.FindProperty("m_Height");
			this.m_AntiAliasing = base.serializedObject.FindProperty("m_AntiAliasing");
			this.m_DepthFormat = base.serializedObject.FindProperty("m_DepthFormat");
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			RenderTexture renderTexture = this.target as RenderTexture;
			GUI.changed = false;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Size", "MiniPopup");
			EditorGUILayout.PropertyField(this.m_Width, GUIContent.none, new GUILayoutOption[]
			{
				GUILayout.MinWidth(40f)
			});
			GUILayout.Label("x", new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Height, GUIContent.none, new GUILayoutOption[]
			{
				GUILayout.MinWidth(40f)
			});
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			EditorGUILayout.IntPopup(this.m_AntiAliasing, RenderTextureInspector.kRenderTextureAntiAliasing, RenderTextureInspector.kRenderTextureAntiAliasingValues, EditorGUIUtility.TempContent("Anti-Aliasing"), new GUILayoutOption[0]);
			EditorGUILayout.IntPopup(this.m_DepthFormat, RenderTextureInspector.kRenderTextureDepths, RenderTextureInspector.kRenderTextureDepthsValues, EditorGUIUtility.TempContent("Depth Buffer"), new GUILayoutOption[0]);
			if (GUI.changed)
			{
				renderTexture.Release();
			}
			base.isInspectorDirty = true;
			base.serializedObject.ApplyModifiedProperties();
			EditorGUILayout.Space();
			base.OnInspectorGUI();
		}
		public override string GetInfoString()
		{
			RenderTexture renderTexture = this.target as RenderTexture;
			string str = renderTexture.width.ToString() + "x" + renderTexture.height.ToString();
			if (!renderTexture.isPowerOfTwo)
			{
				str += "(NPOT)";
			}
			str = str + "  " + RenderTextureInspector.kTextureFormatsStrings[(int)renderTexture.format];
			return str + "  " + EditorUtility.FormatBytes(TextureUtil.GetRuntimeMemorySize(renderTexture));
		}
	}
}
