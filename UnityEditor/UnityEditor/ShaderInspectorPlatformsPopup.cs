using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class ShaderInspectorPlatformsPopup : PopupWindowContent
	{
		private class Styles
		{
			public static readonly GUIStyle menuItem = "MenuItem";

			public static readonly GUIStyle separator = "sv_iconselector_sep";
		}

		internal static readonly string[] s_PlatformModes = new string[]
		{
			"Current graphics device",
			"Current build platform",
			"All platforms",
			"Custom:"
		};

		private static string[] s_ShaderPlatformNames;

		private static int[] s_ShaderPlatformIndices;

		private const float kFrameWidth = 1f;

		private const float kSeparatorHeight = 6f;

		private readonly Shader m_Shader;

		private static int s_CurrentMode = -1;

		private static int s_CurrentPlatformMask = -1;

		private static int s_CurrentVariantStripping = -1;

		public static int currentMode
		{
			get
			{
				if (ShaderInspectorPlatformsPopup.s_CurrentMode < 0)
				{
					ShaderInspectorPlatformsPopup.s_CurrentMode = EditorPrefs.GetInt("ShaderInspectorPlatformMode", 1);
				}
				return ShaderInspectorPlatformsPopup.s_CurrentMode;
			}
			set
			{
				ShaderInspectorPlatformsPopup.s_CurrentMode = value;
				EditorPrefs.SetInt("ShaderInspectorPlatformMode", value);
			}
		}

		public static int currentPlatformMask
		{
			get
			{
				if (ShaderInspectorPlatformsPopup.s_CurrentPlatformMask < 0)
				{
					ShaderInspectorPlatformsPopup.s_CurrentPlatformMask = EditorPrefs.GetInt("ShaderInspectorPlatformMask", 1048575);
				}
				return ShaderInspectorPlatformsPopup.s_CurrentPlatformMask;
			}
			set
			{
				ShaderInspectorPlatformsPopup.s_CurrentPlatformMask = value;
				EditorPrefs.SetInt("ShaderInspectorPlatformMask", value);
			}
		}

		public static int currentVariantStripping
		{
			get
			{
				if (ShaderInspectorPlatformsPopup.s_CurrentVariantStripping < 0)
				{
					ShaderInspectorPlatformsPopup.s_CurrentVariantStripping = EditorPrefs.GetInt("ShaderInspectorVariantStripping", 1);
				}
				return ShaderInspectorPlatformsPopup.s_CurrentVariantStripping;
			}
			set
			{
				ShaderInspectorPlatformsPopup.s_CurrentVariantStripping = value;
				EditorPrefs.SetInt("ShaderInspectorVariantStripping", value);
			}
		}

		public ShaderInspectorPlatformsPopup(Shader shader)
		{
			this.m_Shader = shader;
			ShaderInspectorPlatformsPopup.InitializeShaderPlatforms();
		}

		private static void InitializeShaderPlatforms()
		{
			if (ShaderInspectorPlatformsPopup.s_ShaderPlatformNames == null)
			{
				int availableShaderCompilerPlatforms = ShaderUtil.GetAvailableShaderCompilerPlatforms();
				List<string> list = new List<string>();
				List<int> list2 = new List<int>();
				for (int i = 0; i < 32; i++)
				{
					if ((availableShaderCompilerPlatforms & 1 << i) != 0)
					{
						List<string> arg_4D_0 = list;
						ShaderUtil.ShaderCompilerPlatformType shaderCompilerPlatformType = (ShaderUtil.ShaderCompilerPlatformType)i;
						arg_4D_0.Add(shaderCompilerPlatformType.ToString());
						list2.Add(i);
					}
				}
				ShaderInspectorPlatformsPopup.s_ShaderPlatformNames = list.ToArray();
				ShaderInspectorPlatformsPopup.s_ShaderPlatformIndices = list2.ToArray();
			}
		}

		public override Vector2 GetWindowSize()
		{
			int num = ShaderInspectorPlatformsPopup.s_PlatformModes.Length + ShaderInspectorPlatformsPopup.s_ShaderPlatformNames.Length + 2;
			float num2 = (float)num * 16f + 18f;
			num2 += 2f;
			Vector2 result = new Vector2(210f, num2);
			return result;
		}

		public override void OnGUI(Rect rect)
		{
			if (!(this.m_Shader == null))
			{
				if (Event.current.type != EventType.Layout)
				{
					this.Draw(base.editorWindow, rect.width);
					if (Event.current.type == EventType.MouseMove)
					{
						Event.current.Use();
					}
					if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
					{
						base.editorWindow.Close();
						GUIUtility.ExitGUI();
					}
				}
			}
		}

		private void DrawSeparator(ref Rect rect)
		{
			GUI.Label(new Rect(rect.x + 5f, rect.y + 3f, rect.width - 10f, 3f), GUIContent.none, ShaderInspectorPlatformsPopup.Styles.separator);
			rect.y += 6f;
		}

		private void Draw(EditorWindow caller, float listElementWidth)
		{
			Rect rect = new Rect(0f, 0f, listElementWidth, 16f);
			for (int i = 0; i < ShaderInspectorPlatformsPopup.s_PlatformModes.Length; i++)
			{
				this.DoOneMode(rect, i);
				rect.y += 16f;
			}
			Color color = GUI.color;
			if (ShaderInspectorPlatformsPopup.currentMode != 3)
			{
				GUI.color *= new Color(1f, 1f, 1f, 0.7f);
			}
			rect.xMin += 16f;
			for (int j = 0; j < ShaderInspectorPlatformsPopup.s_ShaderPlatformNames.Length; j++)
			{
				this.DoCustomPlatformBit(rect, j);
				rect.y += 16f;
			}
			GUI.color = color;
			rect.xMin -= 16f;
			this.DrawSeparator(ref rect);
			this.DoShaderVariants(caller, ref rect);
		}

		private void DoOneMode(Rect rect, int index)
		{
			EditorGUI.BeginChangeCheck();
			GUI.Toggle(rect, ShaderInspectorPlatformsPopup.currentMode == index, EditorGUIUtility.TempContent(ShaderInspectorPlatformsPopup.s_PlatformModes[index]), ShaderInspectorPlatformsPopup.Styles.menuItem);
			if (EditorGUI.EndChangeCheck())
			{
				ShaderInspectorPlatformsPopup.currentMode = index;
			}
		}

		private void DoCustomPlatformBit(Rect rect, int index)
		{
			EditorGUI.BeginChangeCheck();
			int num = 1 << ShaderInspectorPlatformsPopup.s_ShaderPlatformIndices[index];
			bool flag = (ShaderInspectorPlatformsPopup.currentPlatformMask & num) != 0;
			flag = GUI.Toggle(rect, flag, EditorGUIUtility.TempContent(ShaderInspectorPlatformsPopup.s_ShaderPlatformNames[index]), ShaderInspectorPlatformsPopup.Styles.menuItem);
			if (EditorGUI.EndChangeCheck())
			{
				if (flag)
				{
					ShaderInspectorPlatformsPopup.currentPlatformMask |= num;
				}
				else
				{
					ShaderInspectorPlatformsPopup.currentPlatformMask &= ~num;
				}
				ShaderInspectorPlatformsPopup.currentMode = 3;
			}
		}

		private static string FormatCount(ulong count)
		{
			string result;
			if (count > 1000000000uL)
			{
				result = (count / 1000000000.0).ToString("f2") + "B";
			}
			else if (count > 1000000uL)
			{
				result = (count / 1000000.0).ToString("f2") + "M";
			}
			else if (count > 1000uL)
			{
				result = (count / 1000.0).ToString("f2") + "k";
			}
			else
			{
				result = count.ToString();
			}
			return result;
		}

		private void DoShaderVariants(EditorWindow caller, ref Rect drawPos)
		{
			EditorGUI.BeginChangeCheck();
			bool flag = GUI.Toggle(drawPos, ShaderInspectorPlatformsPopup.currentVariantStripping == 1, EditorGUIUtility.TempContent("Skip unused shader_features"), ShaderInspectorPlatformsPopup.Styles.menuItem);
			drawPos.y += 16f;
			if (EditorGUI.EndChangeCheck())
			{
				ShaderInspectorPlatformsPopup.currentVariantStripping = ((!flag) ? 0 : 1);
			}
			drawPos.y += 6f;
			ulong variantCount = ShaderUtil.GetVariantCount(this.m_Shader, flag);
			string text = ShaderInspectorPlatformsPopup.FormatCount(variantCount) + ((!flag) ? " variants total" : " variants included");
			Rect position = drawPos;
			position.x += (float)ShaderInspectorPlatformsPopup.Styles.menuItem.padding.left;
			position.width -= (float)(ShaderInspectorPlatformsPopup.Styles.menuItem.padding.left + 4);
			GUI.Label(position, text);
			position.xMin = position.xMax - 40f;
			if (GUI.Button(position, "Show", EditorStyles.miniButton))
			{
				ShaderUtil.OpenShaderCombinations(this.m_Shader, flag);
				caller.Close();
				GUIUtility.ExitGUI();
			}
		}
	}
}
