using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(Shader))]
	internal class ShaderInspector : Editor
	{
		internal class Styles
		{
			public static Texture2D errorIcon = EditorGUIUtility.LoadIcon("console.erroricon.sml");

			public static Texture2D warningIcon = EditorGUIUtility.LoadIcon("console.warnicon.sml");

			public static GUIContent showSurface = EditorGUIUtility.TextContent("Show generated code|Show generated code of a surface shader");

			public static GUIContent showFF = EditorGUIUtility.TextContent("Show generated code|Show generated code of a fixed function shader");

			public static GUIContent showCurrent = new GUIContent("Compile and show code | ▾");

			public static GUIStyle messageStyle = "CN StatusInfo";

			public static GUIStyle evenBackground = "CN EntryBackEven";

			public static GUIContent no = EditorGUIUtility.TextContent("no");

			public static GUIContent builtinShader = EditorGUIUtility.TextContent("Built-in shader");
		}

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
			"Any texture: ",
			"2D: ",
			"3D: ",
			"Cube: ",
			"2DArray: ",
			"CubeArray: "
		};

		private const float kSpace = 5f;

		private static readonly int kErrorViewHash = "ShaderErrorView".GetHashCode();

		private Vector2 m_ScrollPosition = Vector2.zero;

		public virtual void OnEnable()
		{
			Shader s = base.target as Shader;
			ShaderUtil.FetchCachedErrors(s);
		}

		private static string GetPropertyType(Shader s, int index)
		{
			ShaderUtil.ShaderPropertyType propertyType = ShaderUtil.GetPropertyType(s, index);
			string result;
			if (propertyType == ShaderUtil.ShaderPropertyType.TexEnv)
			{
				result = ShaderInspector.kTextureTypes[(int)ShaderUtil.GetTexDim(s, index)];
			}
			else
			{
				result = ShaderInspector.kPropertyTypes[(int)propertyType];
			}
			return result;
		}

		public override void OnInspectorGUI()
		{
			Shader shader = base.target as Shader;
			if (!(shader == null))
			{
				GUI.enabled = true;
				EditorGUI.indentLevel = 0;
				this.ShowShaderCodeArea(shader);
				if (shader.isSupported)
				{
					EditorGUILayout.LabelField("Cast shadows", (!ShaderUtil.HasShadowCasterPass(shader)) ? "no" : "yes", new GUILayoutOption[0]);
					EditorGUILayout.LabelField("Render queue", ShaderUtil.GetRenderQueue(shader).ToString(CultureInfo.InvariantCulture), new GUILayoutOption[0]);
					EditorGUILayout.LabelField("LOD", ShaderUtil.GetLOD(shader).ToString(CultureInfo.InvariantCulture), new GUILayoutOption[0]);
					EditorGUILayout.LabelField("Ignore projector", (!ShaderUtil.DoesIgnoreProjector(shader)) ? "no" : "yes", new GUILayoutOption[0]);
					string label;
					switch (shader.disableBatching)
					{
					case DisableBatchingType.False:
						label = "no";
						break;
					case DisableBatchingType.True:
						label = "yes";
						break;
					case DisableBatchingType.WhenLODFading:
						label = "when LOD fading is on";
						break;
					default:
						label = "unknown";
						break;
					}
					EditorGUILayout.LabelField("Disable batching", label, new GUILayoutOption[0]);
					ShaderInspector.ShowShaderProperties(shader);
				}
			}
		}

		private void ShowShaderCodeArea(Shader s)
		{
			ShaderInspector.ShowSurfaceShaderButton(s);
			ShaderInspector.ShowFixedFunctionShaderButton(s);
			this.ShowCompiledCodeButton(s);
			this.ShowShaderErrors(s);
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

		internal static void ShaderErrorListUI(UnityEngine.Object shader, ShaderError[] errors, ref Vector2 scrollPosition)
		{
			int num = errors.Length;
			GUILayout.Space(5f);
			GUILayout.Label(string.Format("Errors ({0}):", num), EditorStyles.boldLabel, new GUILayoutOption[0]);
			int controlID = GUIUtility.GetControlID(ShaderInspector.kErrorViewHash, FocusType.Passive);
			float minHeight = Mathf.Min((float)num * 20f + 40f, 150f);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUISkin.current.box, new GUILayoutOption[]
			{
				GUILayout.MinHeight(minHeight)
			});
			EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
			float height = ShaderInspector.Styles.messageStyle.CalcHeight(EditorGUIUtility.TempContent(ShaderInspector.Styles.errorIcon), 100f);
			Event current = Event.current;
			for (int i = 0; i < num; i++)
			{
				Rect controlRect = EditorGUILayout.GetControlRect(false, height, new GUILayoutOption[0]);
				string message = errors[i].message;
				string platform = errors[i].platform;
				bool flag = errors[i].warning != 0;
				string lastPathNameComponent = FileUtil.GetLastPathNameComponent(errors[i].file);
				int line = errors[i].line;
				if (current.type == EventType.MouseDown && current.button == 0 && controlRect.Contains(current.mousePosition))
				{
					GUIUtility.keyboardControl = controlID;
					if (current.clickCount == 2)
					{
						string file = errors[i].file;
						UnityEngine.Object @object = (!string.IsNullOrEmpty(file)) ? AssetDatabase.LoadMainAssetAtPath(file) : null;
						if (@object == null && Path.IsPathRooted(file))
						{
							ShaderUtil.OpenSystemShaderIncludeError(file, line);
						}
						else
						{
							AssetDatabase.OpenAsset(@object ?? shader, line);
						}
						GUIUtility.ExitGUI();
					}
					current.Use();
				}
				if (current.type == EventType.ContextClick && controlRect.Contains(current.mousePosition))
				{
					current.Use();
					GenericMenu genericMenu = new GenericMenu();
					int errorIndex = i;
					genericMenu.AddItem(new GUIContent("Copy error text"), false, delegate
					{
						string text = errors[errorIndex].message;
						if (!string.IsNullOrEmpty(errors[errorIndex].messageDetails))
						{
							text += '\n';
							text += errors[errorIndex].messageDetails;
						}
						EditorGUIUtility.systemCopyBuffer = text;
					});
					genericMenu.ShowAsContext();
				}
				if (current.type == EventType.Repaint)
				{
					if ((i & 1) == 0)
					{
						GUIStyle evenBackground = ShaderInspector.Styles.evenBackground;
						evenBackground.Draw(controlRect, false, false, false, false);
					}
				}
				Rect rect = controlRect;
				rect.xMin = rect.xMax;
				if (line > 0)
				{
					GUIContent content;
					if (string.IsNullOrEmpty(lastPathNameComponent))
					{
						content = EditorGUIUtility.TempContent(line.ToString(CultureInfo.InvariantCulture));
					}
					else
					{
						content = EditorGUIUtility.TempContent(lastPathNameComponent + ":" + line.ToString(CultureInfo.InvariantCulture));
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
				if (platform.Length > 0)
				{
					GUIContent content2 = EditorGUIUtility.TempContent(platform);
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
				GUI.Label(position2, EditorGUIUtility.TempContent(message, (!flag) ? ShaderInspector.Styles.errorIcon : ShaderInspector.Styles.warningIcon), ShaderInspector.Styles.messageStyle);
			}
			EditorGUIUtility.SetIconSize(Vector2.zero);
			GUILayout.EndScrollView();
		}

		private void ShowShaderErrors(Shader s)
		{
			int shaderErrorCount = ShaderUtil.GetShaderErrorCount(s);
			if (shaderErrorCount >= 1)
			{
				ShaderInspector.ShaderErrorListUI(s, ShaderUtil.GetShaderErrors(s), ref this.m_ScrollPosition);
			}
		}

		private void ShowCompiledCodeButton(Shader s)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Compiled code", EditorStyles.miniButton);
			bool flag = ShaderUtil.HasShaderSnippets(s) || ShaderUtil.HasSurfaceShaders(s) || ShaderUtil.HasFixedFunctionShaders(s);
			if (flag)
			{
				GUIContent showCurrent = ShaderInspector.Styles.showCurrent;
				Rect rect = GUILayoutUtility.GetRect(showCurrent, EditorStyles.miniButton, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				Rect position = new Rect(rect.xMax - 16f, rect.y, 16f, rect.height);
				if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, GUIStyle.none))
				{
					Rect last = GUILayoutUtility.topLevel.GetLast();
					PopupWindow.Show(last, new ShaderInspectorPlatformsPopup(s));
					GUIUtility.ExitGUI();
				}
				if (GUI.Button(rect, showCurrent, EditorStyles.miniButton))
				{
					ShaderUtil.OpenCompiledShader(s, ShaderInspectorPlatformsPopup.currentMode, ShaderInspectorPlatformsPopup.currentPlatformMask, ShaderInspectorPlatformsPopup.currentVariantStripping == 0);
					GUIUtility.ExitGUI();
				}
			}
			else
			{
				GUILayout.Button("none (precompiled shader)", GUI.skin.label, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndHorizontal();
		}

		private static void ShowSurfaceShaderButton(Shader s)
		{
			bool flag = ShaderUtil.HasSurfaceShaders(s);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Surface shader", EditorStyles.miniButton);
			if (flag)
			{
				if (!(AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(s)) == null))
				{
					if (GUILayout.Button(ShaderInspector.Styles.showSurface, EditorStyles.miniButton, new GUILayoutOption[]
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
					GUILayout.Button(ShaderInspector.Styles.builtinShader, GUI.skin.label, new GUILayoutOption[0]);
				}
			}
			else
			{
				GUILayout.Button(ShaderInspector.Styles.no, GUI.skin.label, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndHorizontal();
		}

		private static void ShowFixedFunctionShaderButton(Shader s)
		{
			bool flag = ShaderUtil.HasFixedFunctionShaders(s);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Fixed function", EditorStyles.miniButton);
			if (flag)
			{
				if (!(AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(s)) == null))
				{
					if (GUILayout.Button(ShaderInspector.Styles.showFF, EditorStyles.miniButton, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					}))
					{
						ShaderUtil.OpenGeneratedFixedFunctionShader(s);
						GUIUtility.ExitGUI();
					}
				}
				else
				{
					GUILayout.Button(ShaderInspector.Styles.builtinShader, GUI.skin.label, new GUILayoutOption[0]);
				}
			}
			else
			{
				GUILayout.Button(ShaderInspector.Styles.no, GUI.skin.label, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
