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

		private SerializedProperty m_AscentCalculationMode;

		private string m_FontNamesString = "";

		private string m_DefaultFontNamesString = "";

		private bool? m_FormatSupported = null;

		private bool m_ReferencesExpanded = false;

		private bool m_GotFontNamesFromImporter = false;

		private Font[] m_FallbackFontReferences = null;

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
			if (base.targets.Length == 1)
			{
				this.m_DefaultFontNamesString = this.GetDefaultFontNames();
				this.m_FontNamesString = this.GetFontNames();
			}
		}

		private string GetDefaultFontNames()
		{
			return ((TrueTypeFontImporter)base.target).fontTTFName;
		}

		private string GetFontNames()
		{
			TrueTypeFontImporter trueTypeFontImporter = (TrueTypeFontImporter)base.target;
			string text = string.Join(", ", trueTypeFontImporter.fontNames);
			if (string.IsNullOrEmpty(text))
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
			TrueTypeFontImporter trueTypeFontImporter = (TrueTypeFontImporter)base.target;
			this.m_FallbackFontReferences = trueTypeFontImporter.LookupFallbackFontReferences(array);
		}

		private void ShowFormatUnsupportedGUI()
		{
			GUILayout.Space(5f);
			EditorGUILayout.HelpBox("Format of selected font is not supported by Unity.", MessageType.Warning);
		}

		private static string GetUniquePath(string basePath, string extension)
		{
			string result;
			for (int i = 0; i < 10000; i++)
			{
				string text = string.Format("{0}{1}.{2}", basePath, (i != 0) ? i.ToString() : string.Empty, extension);
				if (!File.Exists(text))
				{
					result = text;
					return result;
				}
			}
			result = "";
			return result;
		}

		[MenuItem("CONTEXT/TrueTypeFontImporter/Create Editable Copy")]
		private static void CreateEditableCopy(MenuCommand command)
		{
			TrueTypeFontImporter trueTypeFontImporter = (TrueTypeFontImporter)command.context;
			if (trueTypeFontImporter.fontTextureCase == FontTextureCase.Dynamic)
			{
				EditorUtility.DisplayDialog("Cannot generate editable font asset for dynamic fonts", "Please reimport the font in a different mode.", "Ok");
			}
			else
			{
				string str = Path.Combine(Path.GetDirectoryName(trueTypeFontImporter.assetPath), Path.GetFileNameWithoutExtension(trueTypeFontImporter.assetPath));
				EditorGUIUtility.PingObject(trueTypeFontImporter.GenerateEditableFont(TrueTypeFontImporterInspector.GetUniquePath(str + "_copy", "fontsettings")));
			}
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
			}
			else
			{
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
							string source = EditorGUILayout.TextArea(this.m_CustomCharacters.stringValue, GUI.skin.textArea, new GUILayoutOption[]
							{
								GUILayout.MinHeight(32f)
							});
							EditorGUI.showMixedValue = false;
							GUILayout.EndHorizontal();
							if (EditorGUI.EndChangeCheck())
							{
								this.m_CustomCharacters.stringValue = new string((from c in source.Distinct<char>()
								where c != '\n' && c != '\r'
								select c).ToArray<char>());
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
							this.m_ReferencesExpanded = EditorGUILayout.Foldout(this.m_ReferencesExpanded, "References to other fonts in project", true);
							if (this.m_ReferencesExpanded)
							{
								EditorGUILayout.HelpBox("These are automatically generated by the inspector if any of the font names you supplied match fonts present in your project, which will then be used as fallbacks for this font.", MessageType.Info);
								using (new EditorGUI.DisabledScope(true))
								{
									if (this.m_FallbackFontReferences != null && this.m_FallbackFontReferences.Length > 0)
									{
										for (int j = 0; j < this.m_FallbackFontReferences.Length; j++)
										{
											EditorGUILayout.ObjectField(this.m_FallbackFontReferences[j], typeof(Font), false, new GUILayoutOption[0]);
										}
									}
									else
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
}
