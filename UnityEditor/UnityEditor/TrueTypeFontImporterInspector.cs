using System;
using System.IO;
using System.Linq;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(TrueTypeFontImporter))]
	internal class TrueTypeFontImporterInspector : AssetImporterInspector
	{
		private SerializedProperty m_FontSize;
		private SerializedProperty m_TextureCase;
		private SerializedProperty m_IncludeFontData;
		private SerializedProperty m_FontNamesArraySize;
		private SerializedProperty m_CustomCharacters;
		private SerializedProperty m_FontRenderingMode;
		private string m_FontNamesString = string.Empty;
		private string m_DefaultFontNamesString = string.Empty;
		private bool? m_FormatSupported;
		private static GUIContent[] kCharacterStrings = new GUIContent[]
		{
			new GUIContent("Dynamic"),
			new GUIContent("Unicode"),
			new GUIContent("ASCII default set"),
			new GUIContent("ASCII upper case"),
			new GUIContent("ASCII lower case"),
			new GUIContent("Custom set")
		};
		private static int[] kCharacterValues = new int[]
		{
			-2,
			-1,
			0,
			1,
			2,
			3
		};
		private static GUIContent[] kRenderingModeStrings = new GUIContent[]
		{
			new GUIContent("Smooth"),
			new GUIContent("Hinted Smooth"),
			new GUIContent("Hinted Raster"),
			new GUIContent("OS Default")
		};
		private static int[] kRenderingModeValues = new int[]
		{
			0,
			1,
			2,
			3
		};
		private void OnEnable()
		{
			this.m_FontSize = base.serializedObject.FindProperty("m_FontSize");
			this.m_TextureCase = base.serializedObject.FindProperty("m_ForceTextureCase");
			this.m_IncludeFontData = base.serializedObject.FindProperty("m_IncludeFontData");
			this.m_FontNamesArraySize = base.serializedObject.FindProperty("m_FontNames.Array.size");
			this.m_CustomCharacters = base.serializedObject.FindProperty("m_CustomCharacters");
			this.m_FontRenderingMode = base.serializedObject.FindProperty("m_FontRenderingMode");
			if (base.targets.Length == 1)
			{
				this.m_DefaultFontNamesString = this.GetDefaultFontNames();
				this.m_FontNamesString = this.GetFontNames();
			}
		}
		private string GetDefaultFontNames()
		{
			TrueTypeFontImporter trueTypeFontImporter = this.target as TrueTypeFontImporter;
			return trueTypeFontImporter.fontTTFName;
		}
		private string GetFontNames()
		{
			TrueTypeFontImporter trueTypeFontImporter = this.target as TrueTypeFontImporter;
			string text = string.Empty;
			string[] fontNames = trueTypeFontImporter.fontNames;
			for (int i = 0; i < fontNames.Length; i++)
			{
				text += fontNames[i];
				if (i < fontNames.Length - 1)
				{
					text += ", ";
				}
			}
			if (text == string.Empty)
			{
				text = this.m_DefaultFontNamesString;
			}
			return text;
		}
		private void SetFontNames(string fontNames)
		{
			string[] array;
			if (fontNames == this.m_DefaultFontNamesString)
			{
				array = new string[]
				{
					string.Empty
				};
			}
			else
			{
				array = fontNames.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Trim();
				}
			}
			this.m_FontNamesArraySize.intValue = array.Length;
			SerializedProperty serializedProperty = this.m_FontNamesArraySize.Copy();
			for (int j = 0; j < array.Length; j++)
			{
				serializedProperty.Next(false);
				serializedProperty.stringValue = array[j];
			}
		}
		private void ShowFormatUnsupportedGUI()
		{
			GUILayout.Space(5f);
			EditorGUILayout.HelpBox("Format of selected font is not supported by Unity.", MessageType.Warning);
		}
		private static string GetUniquePath(string basePath, string extension)
		{
			for (int i = 0; i < 10000; i++)
			{
				string text = basePath + ((i != 0) ? (string.Empty + i) : string.Empty) + "." + extension;
				if (!File.Exists(text))
				{
					return text;
				}
			}
			return string.Empty;
		}
		[MenuItem("CONTEXT/TrueTypeFontImporter/Create Editable Copy")]
		private static void CreateEditableCopy(MenuCommand command)
		{
			TrueTypeFontImporter trueTypeFontImporter = command.context as TrueTypeFontImporter;
			if (trueTypeFontImporter.fontTextureCase == FontTextureCase.Dynamic)
			{
				EditorUtility.DisplayDialog("Cannot generate editabled font asset for dynamic fonts", "Please reimport the font in a different mode.", "Ok");
				return;
			}
			string str = Path.GetDirectoryName(trueTypeFontImporter.assetPath) + "/" + Path.GetFileNameWithoutExtension(trueTypeFontImporter.assetPath);
			EditorGUIUtility.PingObject(trueTypeFontImporter.GenerateEditableFont(TrueTypeFontImporterInspector.GetUniquePath(str + "_copy", "fontsettings")));
		}
		public override void OnInspectorGUI()
		{
			if (!this.m_FormatSupported.HasValue)
			{
				this.m_FormatSupported = new bool?(true);
				UnityEngine.Object[] targets = base.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					UnityEngine.Object @object = targets[i];
					TrueTypeFontImporter trueTypeFontImporter = @object as TrueTypeFontImporter;
					if (trueTypeFontImporter == null || !trueTypeFontImporter.IsFormatSupported())
					{
						this.m_FormatSupported = new bool?(false);
					}
				}
			}
			if (this.m_FormatSupported == false)
			{
				this.ShowFormatUnsupportedGUI();
				return;
			}
			EditorGUILayout.PropertyField(this.m_FontSize, new GUILayoutOption[0]);
			if (this.m_FontSize.intValue < 1)
			{
				this.m_FontSize.intValue = 1;
			}
			if (this.m_FontSize.intValue > 500)
			{
				this.m_FontSize.intValue = 500;
			}
			EditorGUILayout.IntPopup(this.m_FontRenderingMode, TrueTypeFontImporterInspector.kRenderingModeStrings, TrueTypeFontImporterInspector.kRenderingModeValues, new GUIContent("Rendering Mode"), new GUILayoutOption[0]);
			EditorGUILayout.IntPopup(this.m_TextureCase, TrueTypeFontImporterInspector.kCharacterStrings, TrueTypeFontImporterInspector.kCharacterValues, new GUIContent("Character"), new GUILayoutOption[0]);
			if (!this.m_TextureCase.hasMultipleDifferentValues)
			{
				if (this.m_TextureCase.intValue != -2)
				{
					if (this.m_TextureCase.intValue == 3)
					{
						EditorGUI.BeginChangeCheck();
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						EditorGUILayout.PrefixLabel("Custom Chars");
						EditorGUI.showMixedValue = this.m_CustomCharacters.hasMultipleDifferentValues;
						string text = EditorGUILayout.TextArea(this.m_CustomCharacters.stringValue, GUI.skin.textArea, new GUILayoutOption[]
						{
							GUILayout.MinHeight(32f)
						});
						EditorGUI.showMixedValue = false;
						GUILayout.EndHorizontal();
						if (EditorGUI.EndChangeCheck())
						{
							text = new string(text.Distinct<char>().ToArray<char>());
							text = text.Replace("\n", string.Empty);
							text = text.Replace("\r", string.Empty);
							this.m_CustomCharacters.stringValue = text;
						}
					}
				}
				else
				{
					EditorGUILayout.PropertyField(this.m_IncludeFontData, new GUIContent("Incl. Font Data"), new GUILayoutOption[0]);
					if (base.targets.Length == 1)
					{
						EditorGUI.BeginChangeCheck();
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						EditorGUILayout.PrefixLabel("Font Names");
						GUI.SetNextControlName("fontnames");
						this.m_FontNamesString = EditorGUILayout.TextArea(this.m_FontNamesString, "TextArea", new GUILayoutOption[]
						{
							GUILayout.MinHeight(32f)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.FlexibleSpace();
						EditorGUI.BeginDisabledGroup(this.m_FontNamesString == this.m_DefaultFontNamesString);
						if (GUILayout.Button("Reset", "MiniButton", new GUILayoutOption[0]))
						{
							GUI.changed = true;
							if (GUI.GetNameOfFocusedControl() == "fontnames")
							{
								GUIUtility.keyboardControl = 0;
							}
							this.m_FontNamesString = this.m_DefaultFontNamesString;
						}
						EditorGUI.EndDisabledGroup();
						GUILayout.EndHorizontal();
						if (EditorGUI.EndChangeCheck())
						{
							this.SetFontNames(this.m_FontNamesString);
						}
					}
				}
			}
			base.ApplyRevertGUI();
		}
	}
}
