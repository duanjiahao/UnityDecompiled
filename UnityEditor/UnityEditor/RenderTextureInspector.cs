using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(RenderTexture))]
	internal class RenderTextureInspector : TextureInspector
	{
		private static readonly GUIContent[] kRenderTextureAntiAliasing = new GUIContent[]
		{
			new GUIContent("None"),
			new GUIContent("2 samples"),
			new GUIContent("4 samples"),
			new GUIContent("8 samples")
		};

		private static readonly int[] kRenderTextureAntiAliasingValues = new int[]
		{
			1,
			2,
			4,
			8
		};

		private SerializedProperty m_Width;

		private SerializedProperty m_Height;

		private SerializedProperty m_ColorFormat;

		private SerializedProperty m_DepthFormat;

		private SerializedProperty m_AntiAliasing;

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_Width = base.serializedObject.FindProperty("m_Width");
			this.m_Height = base.serializedObject.FindProperty("m_Height");
			this.m_AntiAliasing = base.serializedObject.FindProperty("m_AntiAliasing");
			this.m_ColorFormat = base.serializedObject.FindProperty("m_ColorFormat");
			this.m_DepthFormat = base.serializedObject.FindProperty("m_DepthFormat");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			GUI.changed = false;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Size", EditorStyles.popup);
			EditorGUILayout.DelayedIntField(this.m_Width, GUIContent.none, new GUILayoutOption[]
			{
				GUILayout.MinWidth(40f)
			});
			GUILayout.Label("x", new GUILayoutOption[0]);
			EditorGUILayout.DelayedIntField(this.m_Height, GUIContent.none, new GUILayoutOption[]
			{
				GUILayout.MinWidth(40f)
			});
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			EditorGUILayout.IntPopup(this.m_AntiAliasing, RenderTextureInspector.kRenderTextureAntiAliasing, RenderTextureInspector.kRenderTextureAntiAliasingValues, EditorGUIUtility.TempContent("Anti-Aliasing"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ColorFormat, EditorGUIUtility.TempContent("Color Format"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_DepthFormat, EditorGUIUtility.TempContent("Depth Buffer"), new GUILayoutOption[0]);
			RenderTexture renderTexture = base.target as RenderTexture;
			if (GUI.changed && renderTexture != null)
			{
				renderTexture.Release();
			}
			base.isInspectorDirty = true;
			EditorGUILayout.Space();
			base.DoWrapModePopup();
			base.DoFilterModePopup();
			using (new EditorGUI.DisabledScope(this.RenderTextureHasDepth()))
			{
				base.DoAnisoLevelSlider();
			}
			if (this.RenderTextureHasDepth())
			{
				this.m_Aniso.intValue = 0;
				EditorGUILayout.HelpBox("RenderTextures with depth must have an Aniso Level of 0.", MessageType.Info);
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		private bool RenderTextureHasDepth()
		{
			return TextureUtil.IsDepthRTFormat((RenderTextureFormat)this.m_ColorFormat.enumValueIndex) || this.m_DepthFormat.enumValueIndex != 0;
		}

		public override string GetInfoString()
		{
			RenderTexture renderTexture = base.target as RenderTexture;
			string text = renderTexture.width + "x" + renderTexture.height;
			if (!renderTexture.isPowerOfTwo)
			{
				text += "(NPOT)";
			}
			text = text + "  " + renderTexture.format;
			return text + "  " + EditorUtility.FormatBytes(TextureUtil.GetRuntimeMemorySize(renderTexture));
		}
	}
}
