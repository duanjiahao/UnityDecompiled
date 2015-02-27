using System;
using System.Globalization;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(Shader))]
	internal class ShaderInspector : Editor
	{
		internal class Styles
		{
			public Texture2D errorIcon = EditorGUIUtility.LoadIcon("console.erroricon.sml");
			public Texture2D warningIcon = EditorGUIUtility.LoadIcon("console.warnicon.sml");
			public GUIContent showSurface = EditorGUIUtility.TextContent("ShaderInspector.ShowSurface");
			public GUIContent showCurrent = EditorGUIUtility.TextContent("ShaderInspector.ShowCurrent");
			public GUIContent showAll = EditorGUIUtility.TextContent("ShaderInspector.ShowAll");
			public GUIStyle messageStyle = "CN StatusInfo";
			public GUIStyle evenBackground = "CN EntryBackEven";
		}
		internal class ShaderContextData
		{
			internal Shader shader;
			internal int index;
			internal ShaderContextData(Shader s, int i)
			{
				this.shader = s;
				this.index = i;
			}
		}
		private const float kSpace = 5f;
		private static readonly string[] kPropertyTypes = new string[]
		{
			"Color: ",
			"Vector: ",
			"Float: ",
			"Range: ",
			"Texture: "
		};
		private static readonly string[] kTextureTypes = new string[]
		{
			"No Texture?: ",
			"1D texture: ",
			"Texture: ",
			"Volume: ",
			"Cubemap: ",
			"Any texture: "
		};
		private static readonly string[] kShaderLevels = new string[]
		{
			"Fixed function",
			"SM1.x",
			"SM2.0",
			"SM3.0",
			"SM4.0",
			"SM5.0"
		};
		internal static ShaderInspector.Styles ms_Styles;
		private static readonly int kErrorViewHash = "ShaderErrorView".GetHashCode();
		private Vector2 m_ScrollPosition = Vector2.zero;
		public virtual void OnEnable()
		{
			Shader s = this.target as Shader;
			ShaderUtil.FetchCachedErrors(s);
		}
		private static string GetPropertyType(Shader s, int index)
		{
			ShaderUtil.ShaderPropertyType propertyType = ShaderUtil.GetPropertyType(s, index);
			if (propertyType == ShaderUtil.ShaderPropertyType.TexEnv)
			{
				return ShaderInspector.kTextureTypes[(int)ShaderUtil.GetTexDim(s, index)];
			}
			return ShaderInspector.kPropertyTypes[(int)propertyType];
		}
		public override void OnInspectorGUI()
		{
			if (ShaderInspector.ms_Styles == null)
			{
				ShaderInspector.ms_Styles = new ShaderInspector.Styles();
			}
			Shader shader = this.target as Shader;
			if (shader == null)
			{
				return;
			}
			GUI.enabled = true;
			EditorGUI.indentLevel = 0;
			if (!shader.isSupported)
			{
				GUILayout.Label("Shader has errors or is not supported by your graphics card", EditorStyles.helpBox, new GUILayoutOption[0]);
			}
			ShaderInspector.ShowSurfaceShaderButton(shader);
			if (shader.isSupported)
			{
				EditorGUILayout.LabelField("Cast shadows", (!ShaderUtil.HasShadowCasterPass(shader) || !ShaderUtil.HasShadowCollectorPass(shader)) ? "no" : "yes", new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Render queue", ShaderUtil.GetRenderQueue(shader).ToString(CultureInfo.InvariantCulture), new GUILayoutOption[0]);
				EditorGUILayout.LabelField("LOD", ShaderUtil.GetLOD(shader).ToString(CultureInfo.InvariantCulture), new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Geometry", ShaderUtil.GetSourceChannels(shader), new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Vertex shader", ShaderInspector.kShaderLevels[(int)ShaderUtil.GetVertexModel(shader)], new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Fragment shader", ShaderInspector.kShaderLevels[(int)ShaderUtil.GetFragmentModel(shader)], new GUILayoutOption[0]);
				EditorGUILayout.LabelField("Ignore projector", (!ShaderUtil.DoesIgnoreProjector(shader)) ? "no" : "yes", new GUILayoutOption[0]);
				ShaderInspector.ShowShaderProperties(shader);
			}
			this.ShowShaderErrors(shader);
			ShaderInspector.ShowDebuggingData(shader);
		}
		private static void ShowShaderProperties(Shader s)
		{
			GUILayout.Space(5f);
			GUILayout.Label("Properties:", EditorStyles.boldLabel, new GUILayoutOption[0]);
			int propertyCount = ShaderUtil.GetPropertyCount(s);
			for (int i = 0; i < propertyCount; i++)
			{
				string propertyName = ShaderUtil.GetPropertyName(s, i);
				string label = ShaderInspector.GetPropertyType(s, i) + ShaderUtil.GetPropertyDescription(s, i);
				EditorGUILayout.LabelField(propertyName, label, new GUILayoutOption[0]);
			}
		}
		private static void ShowShaderVariantsUI(Shader s)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Variants", EditorStyles.miniButton);
			GUILayout.Label(ShaderUtil.GetComboCount(s).ToString(CultureInfo.InvariantCulture), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			if (GUILayout.Button("Show", EditorStyles.miniButton, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				ShaderUtil.OpenShaderCombinations(s);
				GUIUtility.ExitGUI();
			}
			EditorGUILayout.EndHorizontal();
		}
		private static void ShowCompiledShaderButtons(Shader s)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Compiled code", EditorStyles.miniButton);
			if (GUILayout.Button(ShaderInspector.ms_Styles.showCurrent, EditorStyles.miniButton, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				ShaderUtil.OpenCompiledShader(s, false);
				GUIUtility.ExitGUI();
			}
			if (GUILayout.Button(ShaderInspector.ms_Styles.showAll, EditorStyles.miniButton, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				ShaderUtil.OpenCompiledShader(s, true);
				GUIUtility.ExitGUI();
			}
			EditorGUILayout.EndHorizontal();
		}
		private void CopyShaderError(object o)
		{
			ShaderInspector.ShaderContextData shaderContextData = o as ShaderInspector.ShaderContextData;
			if (shaderContextData == null)
			{
				return;
			}
			string shaderErrorMessage = ShaderUtil.GetShaderErrorMessage(shaderContextData.shader, shaderContextData.index);
			EditorGUIUtility.systemCopyBuffer = shaderErrorMessage;
		}
		private void ShowShaderErrors(Shader s)
		{
			int errorCount = ShaderUtil.GetErrorCount(s);
			if (errorCount < 1)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label("Errors:", EditorStyles.boldLabel, new GUILayoutOption[0]);
			int controlID = GUIUtility.GetControlID(ShaderInspector.kErrorViewHash, FocusType.Native);
			float minHeight = Mathf.Min((float)errorCount * 20f + 40f, 150f);
			this.m_ScrollPosition = GUILayout.BeginScrollView(this.m_ScrollPosition, GUISkin.current.box, new GUILayoutOption[]
			{
				GUILayout.MinHeight(minHeight)
			});
			EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
			float height = ShaderInspector.ms_Styles.messageStyle.CalcHeight(EditorGUIUtility.TempContent(ShaderInspector.ms_Styles.errorIcon), 100f);
			Event current = Event.current;
			for (int i = 0; i < errorCount; i++)
			{
				Rect controlRect = EditorGUILayout.GetControlRect(false, height, new GUILayoutOption[0]);
				string shaderErrorMessage = ShaderUtil.GetShaderErrorMessage(s, i);
				string shaderErrorPlatform = ShaderUtil.GetShaderErrorPlatform(s, i);
				bool shaderErrorWarning = ShaderUtil.GetShaderErrorWarning(s, i);
				string shaderErrorFile = ShaderUtil.GetShaderErrorFile(s, i, true);
				int shaderErrorLine = ShaderUtil.GetShaderErrorLine(s, i);
				if (current.type == EventType.MouseDown && current.button == 0 && controlRect.Contains(current.mousePosition))
				{
					GUIUtility.keyboardControl = controlID;
					if (current.clickCount == 2)
					{
						string shaderErrorFile2 = ShaderUtil.GetShaderErrorFile(s, i, false);
						UnityEngine.Object @object = (!string.IsNullOrEmpty(shaderErrorFile2)) ? AssetDatabase.LoadMainAssetAtPath(shaderErrorFile2) : null;
						if (@object != null)
						{
							AssetDatabase.OpenAsset(@object, shaderErrorLine);
						}
						else
						{
							AssetDatabase.OpenAsset(s, shaderErrorLine);
						}
						GUIUtility.ExitGUI();
					}
					current.Use();
				}
				if (current.type == EventType.ContextClick && controlRect.Contains(current.mousePosition))
				{
					current.Use();
					GenericMenu genericMenu = new GenericMenu();
					genericMenu.AddItem(new GUIContent("Copy error text"), false, new GenericMenu.MenuFunction2(this.CopyShaderError), new ShaderInspector.ShaderContextData(s, i));
					genericMenu.ShowAsContext();
				}
				if (current.type == EventType.Repaint && (i & 1) == 0)
				{
					GUIStyle evenBackground = ShaderInspector.ms_Styles.evenBackground;
					evenBackground.Draw(controlRect, false, false, false, false);
				}
				Rect rect = controlRect;
				rect.xMin = rect.xMax;
				if (shaderErrorLine > 0)
				{
					GUIContent content;
					if (string.IsNullOrEmpty(shaderErrorFile))
					{
						content = EditorGUIUtility.TempContent(shaderErrorLine.ToString(CultureInfo.InvariantCulture));
					}
					else
					{
						content = EditorGUIUtility.TempContent(shaderErrorFile + ":" + shaderErrorLine.ToString(CultureInfo.InvariantCulture));
					}
					Vector2 vector = EditorStyles.miniLabel.CalcSize(content);
					rect.xMin -= vector.x;
					GUI.Label(rect, content, EditorStyles.miniLabel);
					rect.xMin -= 2f;
					if (rect.width < 30f)
					{
						rect.xMin = rect.xMax - 30f;
					}
				}
				Rect position = rect;
				position.width = 0f;
				if (shaderErrorPlatform.Length > 0)
				{
					GUIContent content2 = EditorGUIUtility.TempContent(shaderErrorPlatform);
					Vector2 vector2 = EditorStyles.miniLabel.CalcSize(content2);
					position.xMin -= vector2.x;
					Color contentColor = GUI.contentColor;
					GUI.contentColor = new Color(1f, 1f, 1f, 0.5f);
					GUI.Label(position, content2, EditorStyles.miniLabel);
					GUI.contentColor = contentColor;
					position.xMin -= 2f;
				}
				Rect position2 = controlRect;
				position2.xMax = position.xMin;
				GUI.Label(position2, EditorGUIUtility.TempContent(shaderErrorMessage, (!shaderErrorWarning) ? ShaderInspector.ms_Styles.errorIcon : ShaderInspector.ms_Styles.warningIcon), ShaderInspector.ms_Styles.messageStyle);
			}
			EditorGUIUtility.SetIconSize(Vector2.zero);
			GUILayout.EndScrollView();
		}
		private static void ShowDebuggingData(Shader s)
		{
			GUILayout.Space(5f);
			GUILayout.Label("Debugging:", EditorStyles.boldLabel, new GUILayoutOption[0]);
			ShaderInspector.ShowCompiledShaderButtons(s);
			ShaderInspector.ShowShaderVariantsUI(s);
		}
		private static void ShowSurfaceShaderButton(Shader s)
		{
			bool flag = ShaderUtil.HasSurfaceShaders(s);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Surface shader", EditorStyles.miniButton);
			if (flag)
			{
				if (GUILayout.Button(ShaderInspector.ms_Styles.showSurface, EditorStyles.miniButton, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					ShaderUtil.OpenParsedSurfaceShader(s);
					GUIUtility.ExitGUI();
				}
			}
			else
			{
				GUILayout.Label("no", new GUILayoutOption[0]);
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
