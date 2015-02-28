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
			public static Texture2D errorIcon = EditorGUIUtility.LoadIcon("console.erroricon.sml");
			public static Texture2D warningIcon = EditorGUIUtility.LoadIcon("console.warnicon.sml");
			public static GUIContent showSurface = EditorGUIUtility.TextContent("ShaderInspector.ShowSurface");
			public static GUIContent showCurrent = new GUIContent("Compile and show code | ▾");
			public static GUIStyle messageStyle = "CN StatusInfo";
			public static GUIStyle evenBackground = "CN EntryBackEven";
			public static GUIContent no = EditorGUIUtility.TextContent("ShaderInspector.No");
			public static GUIContent builtinSurfaceShader = EditorGUIUtility.TextContent("ShaderInspector.BuiltinSurfaceShader");
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
			Shader shader = this.target as Shader;
			if (shader == null)
			{
				return;
			}
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
		private void ShowShaderCodeArea(Shader s)
		{
			ShaderInspector.ShowSurfaceShaderButton(s);
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
			float height = ShaderInspector.Styles.messageStyle.CalcHeight(EditorGUIUtility.TempContent(ShaderInspector.Styles.errorIcon), 100f);
			Event current = Event.current;
			for (int i = 0; i < errorCount; i++)
			{
				Rect controlRect = EditorGUILayout.GetControlRect(false, height, new GUILayoutOption[0]);
				string shaderErrorMessage = ShaderUtil.GetShaderErrorMessage(s, i, false);
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
						AssetDatabase.OpenAsset(@object ?? s, shaderErrorLine);
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
						string shaderErrorMessage2 = ShaderUtil.GetShaderErrorMessage(s, errorIndex, true);
						EditorGUIUtility.systemCopyBuffer = shaderErrorMessage2;
					});
					genericMenu.ShowAsContext();
				}
				if (current.type == EventType.Repaint && (i & 1) == 0)
				{
					GUIStyle evenBackground = ShaderInspector.Styles.evenBackground;
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
				GUI.Label(position2, EditorGUIUtility.TempContent(shaderErrorMessage, (!shaderErrorWarning) ? ShaderInspector.Styles.errorIcon : ShaderInspector.Styles.warningIcon), ShaderInspector.Styles.messageStyle);
			}
			EditorGUIUtility.SetIconSize(Vector2.zero);
			GUILayout.EndScrollView();
		}
		private void ShowCompiledCodeButton(Shader s)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Compiled code", EditorStyles.miniButton);
			bool flag = ShaderUtil.HasShaderSnippets(s) || ShaderUtil.HasSurfaceShaders(s);
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
				GUILayout.Button("none (fixed function shader)", GUI.skin.label, new GUILayoutOption[0]);
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
					GUILayout.Button(ShaderInspector.Styles.builtinSurfaceShader, GUI.skin.label, new GUILayoutOption[0]);
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
