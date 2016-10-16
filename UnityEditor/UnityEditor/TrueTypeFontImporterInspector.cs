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

		private SerializedProperty m_FallbackFontReferencesArraySize;

		private SerializedProperty m_CustomCharacters;

		private SerializedProperty m_FontRenderingMode;

		private SerializedProperty m_AscentCalculationMode;

		private string m_FontNamesString = string.Empty;

		private string m_DefaultFontNamesString = string.Empty;

		private bool? m_FormatSupported;

		private bool m_ReferencesExpanded;

		private bool m_GotFontNamesFromImporter;

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

		private static GUIContent[] kAscentCalculationModeStrings = new GUIContent[]
		{
			new GUIContent("Legacy version 2 mode (glyph bounding boxes)"),
			new GUIContent("Face ascender metric"),
			new GUIContent("Face bounding box metric")
		};

		private static int[] kAscentCalculationModeValues = new int[]
		{
			0,
			1,
			2
		};

		private void OnEnable()
		{
			this.m_FontSize = base.serializedObject.FindProperty("m_FontSize");
			this.m_TextureCase = base.serializedObject.FindProperty("m_ForceTextureCase");
			this.m_IncludeFontData = base.serializedObject.FindProperty("m_IncludeFontData");
			this.m_FontNamesArraySize = base.serializedObject.FindProperty("m_FontNames.Array.size");
			this.m_CustomCharacters = base.serializedObject.FindProperty("m_CustomCharacters");
			this.m_FontRenderingMode = base.serializedObject.FindProperty("m_FontRenderingMode");
			this.m_AscentCalculationMode = base.serializedObject.FindProperty("m_AscentCalculationMode");
			this.m_FallbackFontReferencesArraySize = base.serializedObject.FindProperty("m_FallbackFontReferences.Array.size");
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
			else
			{
				this.m_GotFontNamesFromImporter = true;
			}
			return text;
		}

		private void SetFontNames(string fontNames)
		{
			string[] array;
			if (!this.m_GotFontNamesFromImporter)
			{
				array = new string[0];
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
			TrueTypeFontImporter trueTypeFontImporter = this.target as TrueTypeFontImporter;
			Font[] array2 = trueTypeFontImporter.LookupFallbackFontReferences(array);
			this.m_FallbackFontReferencesArraySize.intValue = array2.Length;
			SerializedProperty serializedProperty2 = this.m_FallbackFontReferencesArraySize.Copy();
			for (int k = 0; k < array2.Length; k++)
			{
				serializedProperty2.Next(false);
				serializedProperty2.objectReferenceValue = array2[k];
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
			EditorGUILayout.IntPopup(this.m_AscentCalculationMode, TrueTypeFontImporterInspector.kAscentCalculationModeStrings, TrueTypeFontImporterInspector.kAscentCalculationModeValues, new GUIContent("Ascent Calculation Mode"), new GUILayoutOption[0]);
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
						using (new EditorGUI.DisabledScope(this.m_FontNamesString == this.m_DefaultFontNamesString))
						{
							if (GUILayout.Button("Reset", "MiniButton", new GUILayoutOption[0]))
							{
								GUI.changed = true;
								if (GUI.GetNameOfFocusedControl() == "fontnames")
								{
									GUIUtility.keyboardControl = 0;
								}
								this.m_FontNamesString = this.m_DefaultFontNamesString;
							}
						}
						GUILayout.EndHorizontal();
						if (EditorGUI.EndChangeCheck())
						{
							this.SetFontNames(this.m_FontNamesString);
						}
						this.m_ReferencesExpanded = EditorGUILayout.Foldout(this.m_ReferencesExpanded, "References to other fonts in project");
						if (this.m_ReferencesExpanded)
						{
							EditorGUILayout.HelpBox("These are automatically generated by the inspector if any of the font names you supplied match fonts present in your project, which will then be used as fallbacks for this font.", MessageType.Info);
							if (this.m_FallbackFontReferencesArraySize.intValue > 0)
							{
								SerializedProperty serializedProperty = this.m_FallbackFontReferencesArraySize.Copy();
								while (serializedProperty.NextVisible(true) && serializedProperty.depth == 1)
								{
									EditorGUILayout.PropertyField(serializedProperty, true, new GUILayoutOption[0]);
								}
							}
							else
							{
								using (new EditorGUI.DisabledScope(true))
								{
									GUILayout.Label("No references to other fonts in project.", new GUILayoutOption[0]);
								}
							}
						}
					}
				}
			}
			base.ApplyRevertGUI();
		}
	}
}
