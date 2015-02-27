using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor.Modules;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class PreferencesWindow : EditorWindow
	{
		internal class Constants
		{
			public GUIStyle sectionScrollView = "PreferencesSectionBox";
			public GUIStyle settingsBoxTitle = "OL Title";
			public GUIStyle settingsBox = "OL Box";
			public GUIStyle errorLabel = "WordWrappedLabel";
			public GUIStyle sectionElement = "PreferencesSection";
			public GUIStyle evenRow = "CN EntryBackEven";
			public GUIStyle oddRow = "CN EntryBackOdd";
			public GUIStyle selected = "ServerUpdateChangesetOn";
			public GUIStyle keysElement = "PreferencesKeysElement";
			public GUIStyle sectionHeader = new GUIStyle(EditorStyles.largeLabel);
			public Constants()
			{
				this.sectionScrollView = new GUIStyle(this.sectionScrollView);
				this.sectionScrollView.overflow.bottom++;
				this.sectionHeader.fontStyle = FontStyle.Bold;
				this.sectionHeader.fontSize = 18;
				this.sectionHeader.margin.top = 10;
				this.sectionHeader.margin.left++;
				if (!EditorGUIUtility.isProSkin)
				{
					this.sectionHeader.normal.textColor = new Color(0.4f, 0.4f, 0.4f, 1f);
				}
				else
				{
					this.sectionHeader.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
				}
			}
		}
		private class Section
		{
			public GUIContent content;
			public PreferencesWindow.OnGUIDelegate guiFunc;
			public Section(string name, PreferencesWindow.OnGUIDelegate guiFunc)
			{
				this.content = new GUIContent(name);
				this.guiFunc = guiFunc;
			}
			public Section(string name, Texture2D icon, PreferencesWindow.OnGUIDelegate guiFunc)
			{
				this.content = new GUIContent(name, icon);
				this.guiFunc = guiFunc;
			}
			public Section(GUIContent content, PreferencesWindow.OnGUIDelegate guiFunc)
			{
				this.content = content;
				this.guiFunc = guiFunc;
			}
		}
		private class RefString
		{
			public string str;
			public RefString(string s)
			{
				this.str = s;
			}
			public override string ToString()
			{
				return this.str;
			}
			public static implicit operator string(PreferencesWindow.RefString s)
			{
				return s.str;
			}
		}
		private class AppsListUserData
		{
			public string[] paths;
			public PreferencesWindow.RefString str;
			public Action onChanged;
			public AppsListUserData(string[] paths, PreferencesWindow.RefString str, Action onChanged)
			{
				this.paths = paths;
				this.str = str;
				this.onChanged = onChanged;
			}
		}
		private delegate void OnGUIDelegate();
		private const string kRecentScriptAppsKey = "RecentlyUsedScriptApp";
		private const string kRecentImageAppsKey = "RecentlyUsedImageApp";
		private const string m_ExpressNotSupportedMessage = "Unfortunately Visual Studio Express does not allow itself to be controlled by external applications. You can still use it by manually opening the Visual Studio project file, but Unity cannot automatically open files for you when you doubleclick them. \n(This does work with Visual Studio Pro)";
		private const int kRecentAppsCount = 10;
		private List<PreferencesWindow.Section> m_Sections;
		private int m_SelectedSectionIndex;
		private static PreferencesWindow.Constants constants = null;
		private List<IPreferenceWindowExtension> prefWinExtensions;
		private bool m_AutoRefresh;
		private bool m_AlwaysShowProjectWizard;
		private bool m_CompressAssetsOnImport;
		private bool m_UseOSColorPicker;
		private bool m_EnableEditorAnalytics;
		private bool m_ShowAssetStoreSearchHits;
		private bool m_VerifySavingAssets;
		private bool m_AllowAttachedDebuggingOfEditor;
		private bool m_AllowAttachedDebuggingOfEditorStateChangedThisSession;
		private PreferencesWindow.RefString m_ScriptEditorPath = new PreferencesWindow.RefString(string.Empty);
		private string m_ScriptEditorArgs = string.Empty;
		private PreferencesWindow.RefString m_ImageAppPath = new PreferencesWindow.RefString(string.Empty);
		private int m_DiffToolIndex;
		private bool m_AllowAlphaNumericHierarchy;
		private string m_AndroidSdkPath = string.Empty;
		private string[] m_ScriptApps;
		private string[] m_ImageApps;
		private string[] m_DiffTools;
		private string m_noDiffToolsMessage = string.Empty;
		private bool m_RefreshCustomPreferences;
		private string[] m_ScriptAppDisplayNames;
		private string[] m_ImageAppDisplayNames;
		private Vector2 m_KeyScrollPos;
		private Vector2 m_SectionScrollPos;
		private PrefKey m_SelectedKey;
		private SortedDictionary<string, List<KeyValuePair<string, PrefColor>>> s_CachedColors;
		private static Vector2 s_ColorScrollPos = Vector2.zero;
		private int currentPage;
		private static int s_KeysControlHash = "KeysControlHash".GetHashCode();
		private int selectedSectionIndex
		{
			get
			{
				return this.m_SelectedSectionIndex;
			}
			set
			{
				this.m_SelectedSectionIndex = value;
				if (this.m_SelectedSectionIndex >= this.m_Sections.Count)
				{
					this.m_SelectedSectionIndex = 0;
				}
				else
				{
					if (this.m_SelectedSectionIndex < 0)
					{
						this.m_SelectedSectionIndex = this.m_Sections.Count - 1;
					}
				}
			}
		}
		private PreferencesWindow.Section selectedSection
		{
			get
			{
				return this.m_Sections[this.m_SelectedSectionIndex];
			}
		}
		private static void ShowPreferencesWindow()
		{
			EditorWindow windowWithRect = EditorWindow.GetWindowWithRect<PreferencesWindow>(new Rect(100f, 100f, 500f, 400f), true, "Unity Preferences");
			windowWithRect.m_Parent.window.m_DontSaveToLayout = true;
		}
		private void OnEnable()
		{
			this.prefWinExtensions = ModuleManager.GetPreferenceWindowExtensions();
			this.ReadPreferences();
			this.m_Sections = new List<PreferencesWindow.Section>();
			this.m_Sections.Add(new PreferencesWindow.Section("General", new PreferencesWindow.OnGUIDelegate(this.ShowGeneral)));
			this.m_Sections.Add(new PreferencesWindow.Section("External Tools", new PreferencesWindow.OnGUIDelegate(this.ShowExternalApplications)));
			this.m_Sections.Add(new PreferencesWindow.Section("Colors", new PreferencesWindow.OnGUIDelegate(this.ShowColors)));
			this.m_Sections.Add(new PreferencesWindow.Section("Keys", new PreferencesWindow.OnGUIDelegate(this.ShowKeys)));
			this.m_RefreshCustomPreferences = true;
		}
		private void AddCustomSections()
		{
			Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
			for (int i = 0; i < loadedAssemblies.Length; i++)
			{
				Assembly assembly = loadedAssemblies[i];
				Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
				Type[] array = typesFromAssembly;
				for (int j = 0; j < array.Length; j++)
				{
					Type type = array[j];
					MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					for (int k = 0; k < methods.Length; k++)
					{
						MethodInfo methodInfo = methods[k];
						PreferenceItem preferenceItem = Attribute.GetCustomAttribute(methodInfo, typeof(PreferenceItem)) as PreferenceItem;
						if (preferenceItem != null)
						{
							PreferencesWindow.OnGUIDelegate onGUIDelegate = Delegate.CreateDelegate(typeof(PreferencesWindow.OnGUIDelegate), methodInfo) as PreferencesWindow.OnGUIDelegate;
							if (onGUIDelegate != null)
							{
								this.m_Sections.Add(new PreferencesWindow.Section(preferenceItem.name, onGUIDelegate));
							}
						}
					}
				}
			}
		}
		private void OnGUI()
		{
			if (this.m_RefreshCustomPreferences)
			{
				this.AddCustomSections();
				this.m_RefreshCustomPreferences = false;
			}
			EditorGUIUtility.labelWidth = 180f;
			if (PreferencesWindow.constants == null)
			{
				PreferencesWindow.constants = new PreferencesWindow.Constants();
			}
			this.HandleKeys();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.m_SectionScrollPos = GUILayout.BeginScrollView(this.m_SectionScrollPos, PreferencesWindow.constants.sectionScrollView, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			GUILayout.Space(40f);
			for (int i = 0; i < this.m_Sections.Count; i++)
			{
				PreferencesWindow.Section section = this.m_Sections[i];
				Rect rect = GUILayoutUtility.GetRect(section.content, PreferencesWindow.constants.sectionElement, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				if (section == this.selectedSection && Event.current.type == EventType.Repaint)
				{
					PreferencesWindow.constants.selected.Draw(rect, false, false, false, false);
				}
				EditorGUI.BeginChangeCheck();
				if (GUI.Toggle(rect, this.m_SelectedSectionIndex == i, section.content, PreferencesWindow.constants.sectionElement))
				{
					this.m_SelectedSectionIndex = i;
				}
				if (EditorGUI.EndChangeCheck())
				{
					GUIUtility.keyboardControl = 0;
				}
			}
			GUILayout.EndScrollView();
			GUILayout.Space(10f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label(this.selectedSection.content, PreferencesWindow.constants.sectionHeader, new GUILayoutOption[0]);
			this.selectedSection.guiFunc();
			GUILayout.Space(5f);
			GUILayout.EndVertical();
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
		}
		private void HandleKeys()
		{
			if (Event.current.type != EventType.KeyDown || GUIUtility.keyboardControl != 0)
			{
				return;
			}
			KeyCode keyCode = Event.current.keyCode;
			if (keyCode != KeyCode.UpArrow)
			{
				if (keyCode == KeyCode.DownArrow)
				{
					this.selectedSectionIndex++;
					Event.current.Use();
				}
			}
			else
			{
				this.selectedSectionIndex--;
				Event.current.Use();
			}
		}
		private void ShowExternalApplications()
		{
			GUILayout.Space(10f);
			this.FilePopup("External Script Editor", this.m_ScriptEditorPath, ref this.m_ScriptAppDisplayNames, ref this.m_ScriptApps, this.m_ScriptEditorPath, new Action(this.OnScriptEditorChanged));
			if (!this.IsSelectedScriptEditorSpecial() && Application.platform != RuntimePlatform.OSXEditor)
			{
				string scriptEditorArgs = this.m_ScriptEditorArgs;
				this.m_ScriptEditorArgs = EditorGUILayout.TextField("External Script Editor Args", this.m_ScriptEditorArgs, new GUILayoutOption[0]);
				if (scriptEditorArgs != this.m_ScriptEditorArgs)
				{
					this.OnScriptEditorArgsChanged();
				}
			}
			bool allowAttachedDebuggingOfEditor = this.m_AllowAttachedDebuggingOfEditor;
			this.m_AllowAttachedDebuggingOfEditor = EditorGUILayout.Toggle("Editor Attaching", this.m_AllowAttachedDebuggingOfEditor, new GUILayoutOption[0]);
			if (allowAttachedDebuggingOfEditor != this.m_AllowAttachedDebuggingOfEditor)
			{
				this.m_AllowAttachedDebuggingOfEditorStateChangedThisSession = true;
			}
			if (this.m_AllowAttachedDebuggingOfEditorStateChangedThisSession)
			{
				GUILayout.Label("Changing this setting requires a restart to take effect.", EditorStyles.helpBox, new GUILayoutOption[0]);
			}
			if (this.m_ScriptEditorPath.str.Contains("VCSExpress"))
			{
				GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
				GUILayout.Label(string.Empty, "CN EntryWarn", new GUILayoutOption[0]);
				GUILayout.Label("Unfortunately Visual Studio Express does not allow itself to be controlled by external applications. You can still use it by manually opening the Visual Studio project file, but Unity cannot automatically open files for you when you doubleclick them. \n(This does work with Visual Studio Pro)", PreferencesWindow.constants.errorLabel, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
			}
			GUILayout.Space(10f);
			this.FilePopup("Image application", this.m_ImageAppPath, ref this.m_ImageAppDisplayNames, ref this.m_ImageApps, this.m_ImageAppPath, null);
			GUILayout.Space(10f);
			EditorGUI.BeginDisabledGroup(!InternalEditorUtility.HasMaint());
			this.m_DiffToolIndex = EditorGUILayout.Popup("Revision Control Diff/Merge", this.m_DiffToolIndex, this.m_DiffTools, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			if (this.m_noDiffToolsMessage != string.Empty)
			{
				GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
				GUILayout.Label(string.Empty, "CN EntryWarn", new GUILayoutOption[0]);
				GUILayout.Label(this.m_noDiffToolsMessage, PreferencesWindow.constants.errorLabel, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
			}
			GUILayout.Space(10f);
			this.AndroidSdkLocation();
			foreach (IPreferenceWindowExtension current in this.prefWinExtensions)
			{
				if (current.HasExternalApplications())
				{
					GUILayout.Space(10f);
					current.ShowExternalApplications();
				}
			}
			this.ApplyChangesToPrefs();
		}
		private bool IsSelectedScriptEditorSpecial()
		{
			string text = this.m_ScriptEditorPath.str.ToLower();
			return text == string.Empty || text.EndsWith("monodevelop.exe") || text.EndsWith("devenv.exe") || text.EndsWith("vcsexpress.exe");
		}
		private void OnScriptEditorChanged()
		{
			if (this.IsSelectedScriptEditorSpecial())
			{
				this.m_ScriptEditorArgs = string.Empty;
			}
			else
			{
				this.m_ScriptEditorArgs = EditorPrefs.GetString("kScriptEditorArgs" + this.m_ScriptEditorPath.str, "\"$(File)\"");
			}
			EditorPrefs.SetString("kScriptEditorArgs", this.m_ScriptEditorArgs);
		}
		private void OnScriptEditorArgsChanged()
		{
			EditorPrefs.SetString("kScriptEditorArgs" + this.m_ScriptEditorPath.str, this.m_ScriptEditorArgs);
			EditorPrefs.SetString("kScriptEditorArgs", this.m_ScriptEditorArgs);
		}
		private void ShowGeneral()
		{
			GUILayout.Space(10f);
			this.m_AutoRefresh = EditorGUILayout.Toggle("Auto Refresh", this.m_AutoRefresh, new GUILayoutOption[0]);
			this.m_AlwaysShowProjectWizard = EditorGUILayout.Toggle("Always Show Project Wizard", this.m_AlwaysShowProjectWizard, new GUILayoutOption[0]);
			bool compressAssetsOnImport = this.m_CompressAssetsOnImport;
			this.m_CompressAssetsOnImport = EditorGUILayout.Toggle("Compress Assets on Import", compressAssetsOnImport, new GUILayoutOption[0]);
			if (GUI.changed && this.m_CompressAssetsOnImport != compressAssetsOnImport)
			{
				Unsupported.SetApplicationSettingCompressAssetsOnImport(this.m_CompressAssetsOnImport);
			}
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				this.m_UseOSColorPicker = EditorGUILayout.Toggle("OS X Color Picker", this.m_UseOSColorPicker, new GUILayoutOption[0]);
			}
			this.m_EnableEditorAnalytics = EditorGUILayout.Toggle("Editor Analytics", this.m_EnableEditorAnalytics, new GUILayoutOption[0]);
			bool flag = false;
			EditorGUI.BeginChangeCheck();
			this.m_ShowAssetStoreSearchHits = EditorGUILayout.Toggle("Show Asset Store search hits", this.m_ShowAssetStoreSearchHits, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				flag = true;
			}
			this.m_VerifySavingAssets = EditorGUILayout.Toggle("Verify Saving Assets", this.m_VerifySavingAssets, new GUILayoutOption[0]);
			EditorGUI.BeginDisabledGroup(!InternalEditorUtility.HasPro());
			int num = EditorGUILayout.Popup("Skin (Pro Only)", EditorGUIUtility.isProSkin ? 1 : 0, new string[]
			{
				"Light",
				"Dark"
			}, new GUILayoutOption[0]);
			if ((EditorGUIUtility.isProSkin ? 1 : 0) != num)
			{
				InternalEditorUtility.SwitchSkinAndRepaintAllViews();
			}
			EditorGUI.EndDisabledGroup();
			bool allowAlphaNumericHierarchy = this.m_AllowAlphaNumericHierarchy;
			this.m_AllowAlphaNumericHierarchy = EditorGUILayout.Toggle("Enable Alpha Numeric Sorting", this.m_AllowAlphaNumericHierarchy, new GUILayoutOption[0]);
			this.ApplyChangesToPrefs();
			if (allowAlphaNumericHierarchy != this.m_AllowAlphaNumericHierarchy)
			{
				EditorApplication.DirtyHierarchyWindowSorting();
			}
			if (flag)
			{
				ProjectBrowser.ShowAssetStoreHitsWhileSearchingLocalAssetsChanged();
			}
		}
		private void ApplyChangesToPrefs()
		{
			if (GUI.changed)
			{
				this.WritePreferences();
				this.ReadPreferences();
				base.Repaint();
			}
		}
		private void RevertKeys()
		{
			foreach (KeyValuePair<string, PrefKey> current in Settings.Prefs<PrefKey>())
			{
				current.Value.ResetToDefault();
				EditorPrefs.SetString(current.Value.Name, current.Value.ToUniqueString());
			}
		}
		private SortedDictionary<string, List<KeyValuePair<string, T>>> OrderPrefs<T>(IEnumerable<KeyValuePair<string, T>> input) where T : IPrefType
		{
			SortedDictionary<string, List<KeyValuePair<string, T>>> sortedDictionary = new SortedDictionary<string, List<KeyValuePair<string, T>>>();
			foreach (KeyValuePair<string, T> current in input)
			{
				int num = current.Key.IndexOf('/');
				string key;
				string key2;
				if (num == -1)
				{
					key = "General";
					key2 = current.Key;
				}
				else
				{
					key = current.Key.Substring(0, num);
					key2 = current.Key.Substring(num + 1);
				}
				if (!sortedDictionary.ContainsKey(key))
				{
					sortedDictionary.Add(key, new List<KeyValuePair<string, T>>(new List<KeyValuePair<string, T>>
					{
						new KeyValuePair<string, T>(key2, current.Value)
					}));
				}
				else
				{
					sortedDictionary[key].Add(new KeyValuePair<string, T>(key2, current.Value));
				}
			}
			return sortedDictionary;
		}
		private void ShowKeys()
		{
			int controlID = GUIUtility.GetControlID(PreferencesWindow.s_KeysControlHash, FocusType.Keyboard);
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(185f)
			});
			GUILayout.Label("Actions", PreferencesWindow.constants.settingsBoxTitle, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			this.m_KeyScrollPos = GUILayout.BeginScrollView(this.m_KeyScrollPos, PreferencesWindow.constants.settingsBox);
			PrefKey prefKey = null;
			PrefKey prefKey2 = null;
			bool flag = false;
			foreach (KeyValuePair<string, PrefKey> current in Settings.Prefs<PrefKey>())
			{
				if (!flag)
				{
					if (current.Value == this.m_SelectedKey)
					{
						flag = true;
					}
					else
					{
						prefKey = current.Value;
					}
				}
				else
				{
					if (prefKey2 == null)
					{
						prefKey2 = current.Value;
					}
				}
				EditorGUI.BeginChangeCheck();
				if (GUILayout.Toggle(current.Value == this.m_SelectedKey, current.Key, PreferencesWindow.constants.keysElement, new GUILayoutOption[0]))
				{
					this.m_SelectedKey = current.Value;
				}
				if (EditorGUI.EndChangeCheck())
				{
					GUIUtility.keyboardControl = controlID;
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.Space(10f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (this.m_SelectedKey != null)
			{
				Event @event = this.m_SelectedKey.KeyboardEvent;
				GUI.changed = false;
				string[] array = this.m_SelectedKey.Name.Split(new char[]
				{
					'/'
				});
				Assert.AreEqual(array.Length, 2, "Unexpected Split: " + this.m_SelectedKey.Name);
				GUILayout.Label(array[0], "boldLabel", new GUILayoutOption[0]);
				GUILayout.Label(array[1], "boldLabel", new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Key:", new GUILayoutOption[0]);
				@event = EditorGUILayout.KeyEventField(@event, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Modifiers:", new GUILayoutOption[0]);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					@event.command = GUILayout.Toggle(@event.command, "Command", new GUILayoutOption[0]);
				}
				@event.control = GUILayout.Toggle(@event.control, "Control", new GUILayoutOption[0]);
				@event.shift = GUILayout.Toggle(@event.shift, "Shift", new GUILayoutOption[0]);
				@event.alt = GUILayout.Toggle(@event.alt, "Alt", new GUILayoutOption[0]);
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				if (GUI.changed)
				{
					this.m_SelectedKey.KeyboardEvent = @event;
					Settings.Set<PrefKey>(this.m_SelectedKey.Name, this.m_SelectedKey);
				}
				else
				{
					if (GUIUtility.keyboardControl == controlID && Event.current.type == EventType.KeyDown)
					{
						KeyCode keyCode = Event.current.keyCode;
						if (keyCode != KeyCode.UpArrow)
						{
							if (keyCode == KeyCode.DownArrow)
							{
								if (prefKey2 != null)
								{
									this.m_SelectedKey = prefKey2;
								}
								Event.current.Use();
							}
						}
						else
						{
							if (prefKey != null)
							{
								this.m_SelectedKey = prefKey;
							}
							Event.current.Use();
						}
					}
				}
			}
			GUILayout.EndVertical();
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			if (GUILayout.Button("Use Defaults", new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			}))
			{
				this.RevertKeys();
			}
		}
		private void RevertColors()
		{
			foreach (KeyValuePair<string, PrefColor> current in Settings.Prefs<PrefColor>())
			{
				current.Value.ResetToDefault();
				EditorPrefs.SetString(current.Value.Name, current.Value.ToUniqueString());
			}
		}
		private void ShowColors()
		{
			if (this.s_CachedColors == null)
			{
				this.s_CachedColors = this.OrderPrefs<PrefColor>(Settings.Prefs<PrefColor>());
			}
			bool flag = false;
			PreferencesWindow.s_ColorScrollPos = EditorGUILayout.BeginScrollView(PreferencesWindow.s_ColorScrollPos, new GUILayoutOption[0]);
			GUILayout.Space(10f);
			PrefColor prefColor = null;
			foreach (KeyValuePair<string, List<KeyValuePair<string, PrefColor>>> current in this.s_CachedColors)
			{
				GUILayout.Label(current.Key, EditorStyles.boldLabel, new GUILayoutOption[0]);
				foreach (KeyValuePair<string, PrefColor> current2 in current.Value)
				{
					EditorGUI.BeginChangeCheck();
					Color color = EditorGUILayout.ColorField(current2.Key, current2.Value.Color, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						prefColor = current2.Value;
						prefColor.Color = color;
						flag = true;
					}
				}
				if (prefColor != null)
				{
					Settings.Set<PrefColor>(prefColor.Name, prefColor);
				}
			}
			GUILayout.EndScrollView();
			GUILayout.Space(5f);
			if (GUILayout.Button("Use Defaults", new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			}))
			{
				this.RevertColors();
				flag = true;
			}
			if (flag)
			{
				EditorApplication.RequestRepaintAllViews();
			}
		}
		private void WriteRecentAppsList(string[] paths, string path, string prefsKey)
		{
			int num = 0;
			if (path.Length != 0)
			{
				EditorPrefs.SetString(prefsKey + num, path);
				num++;
			}
			for (int i = 0; i < paths.Length; i++)
			{
				if (num >= 10)
				{
					break;
				}
				if (paths[i].Length != 0)
				{
					if (!(paths[i] == path))
					{
						EditorPrefs.SetString(prefsKey + num, paths[i]);
						num++;
					}
				}
			}
		}
		private void WritePreferences()
		{
			EditorPrefs.SetString("kScriptsDefaultApp", this.m_ScriptEditorPath);
			EditorPrefs.SetString("kScriptEditorArgs", this.m_ScriptEditorArgs);
			EditorPrefs.SetString("kImagesDefaultApp", this.m_ImageAppPath);
			EditorPrefs.SetString("kDiffsDefaultApp", (this.m_DiffTools.Length != 0) ? this.m_DiffTools[this.m_DiffToolIndex] : string.Empty);
			EditorPrefs.SetString("AndroidSdkRoot", this.m_AndroidSdkPath);
			this.WriteRecentAppsList(this.m_ScriptApps, this.m_ScriptEditorPath, "RecentlyUsedScriptApp");
			this.WriteRecentAppsList(this.m_ImageApps, this.m_ImageAppPath, "RecentlyUsedImageApp");
			EditorPrefs.SetBool("kAutoRefresh", this.m_AutoRefresh);
			EditorPrefs.SetBool("AlwaysShowProjectWizard", this.m_AlwaysShowProjectWizard);
			EditorPrefs.SetBool("UseOSColorPicker", this.m_UseOSColorPicker);
			EditorPrefs.SetBool("EnableEditorAnalytics", this.m_EnableEditorAnalytics);
			EditorPrefs.SetBool("ShowAssetStoreSearchHits", this.m_ShowAssetStoreSearchHits);
			EditorPrefs.SetBool("VerifySavingAssets", this.m_VerifySavingAssets);
			EditorPrefs.SetBool("AllowAttachedDebuggingOfEditor", this.m_AllowAttachedDebuggingOfEditor);
			EditorPrefs.SetBool("AllowAlphaNumericHierarchy", this.m_AllowAlphaNumericHierarchy);
			foreach (IPreferenceWindowExtension current in this.prefWinExtensions)
			{
				current.WritePreferences();
			}
		}
		private static void SetupDefaultPreferences()
		{
		}
		private static string GetProgramFilesFolder()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
			if (environmentVariable != null)
			{
				return environmentVariable;
			}
			return Environment.GetEnvironmentVariable("ProgramFiles");
		}
		private void ReadPreferences()
		{
			this.m_ScriptEditorPath.str = EditorPrefs.GetString("kScriptsDefaultApp");
			this.m_ScriptEditorArgs = EditorPrefs.GetString("kScriptEditorArgs", "\"$(File)\"");
			this.m_ImageAppPath.str = EditorPrefs.GetString("kImagesDefaultApp");
			this.m_AndroidSdkPath = EditorPrefs.GetString("AndroidSdkRoot");
			this.m_ScriptApps = this.BuildAppPathList(this.m_ScriptEditorPath, "RecentlyUsedScriptApp");
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				foreach (string current in SyncVS.InstalledVisualStudios.Values)
				{
					if (Array.IndexOf<string>(this.m_ScriptApps, current) == -1)
					{
						if (this.m_ScriptApps.Length < 10)
						{
							ArrayUtility.Add<string>(ref this.m_ScriptApps, current);
						}
						else
						{
							this.m_ScriptApps[1] = current;
						}
					}
				}
			}
			this.m_ImageApps = this.BuildAppPathList(this.m_ImageAppPath, "RecentlyUsedImageApp");
			this.m_ScriptAppDisplayNames = this.BuildFriendlyAppNameList(this.m_ScriptApps, "MonoDevelop (built-in)");
			this.m_ImageAppDisplayNames = this.BuildFriendlyAppNameList(this.m_ImageApps, "Open by file extension");
			this.m_DiffTools = InternalEditorUtility.GetAvailableDiffTools();
			if ((this.m_DiffTools == null || this.m_DiffTools.Length == 0) && InternalEditorUtility.HasMaint())
			{
				this.m_noDiffToolsMessage = InternalEditorUtility.GetNoDiffToolsDetectedMessage();
			}
			string @string = EditorPrefs.GetString("kDiffsDefaultApp");
			this.m_DiffToolIndex = ArrayUtility.IndexOf<string>(this.m_DiffTools, @string);
			if (this.m_DiffToolIndex == -1)
			{
				this.m_DiffToolIndex = 0;
			}
			this.m_AutoRefresh = EditorPrefs.GetBool("kAutoRefresh");
			this.m_AlwaysShowProjectWizard = EditorPrefs.GetBool("AlwaysShowProjectWizard");
			this.m_UseOSColorPicker = EditorPrefs.GetBool("UseOSColorPicker");
			this.m_EnableEditorAnalytics = EditorPrefs.GetBool("EnableEditorAnalytics", true);
			this.m_ShowAssetStoreSearchHits = EditorPrefs.GetBool("ShowAssetStoreSearchHits", true);
			this.m_VerifySavingAssets = EditorPrefs.GetBool("VerifySavingAssets", false);
			this.m_AllowAttachedDebuggingOfEditor = EditorPrefs.GetBool("AllowAttachedDebuggingOfEditor", true);
			this.m_AllowAlphaNumericHierarchy = EditorPrefs.GetBool("AllowAlphaNumericHierarchy", false);
			this.m_CompressAssetsOnImport = Unsupported.GetApplicationSettingCompressAssetsOnImport();
			foreach (IPreferenceWindowExtension current2 in this.prefWinExtensions)
			{
				current2.ReadPreferences();
			}
		}
		private void AppsListClick(object userData, string[] options, int selected)
		{
			PreferencesWindow.AppsListUserData appsListUserData = (PreferencesWindow.AppsListUserData)userData;
			if (options[selected] == "Browse...")
			{
				string text = EditorUtility.OpenFilePanel("Browse for application", string.Empty, (Application.platform != RuntimePlatform.OSXEditor) ? "exe" : "app");
				if (text.Length != 0)
				{
					appsListUserData.str.str = text;
					if (appsListUserData.onChanged != null)
					{
						appsListUserData.onChanged();
					}
				}
			}
			else
			{
				appsListUserData.str.str = appsListUserData.paths[selected];
				if (appsListUserData.onChanged != null)
				{
					appsListUserData.onChanged();
				}
			}
			this.WritePreferences();
			this.ReadPreferences();
		}
		private void FilePopup(string label, string selectedString, ref string[] names, ref string[] paths, PreferencesWindow.RefString outString, Action onChanged)
		{
			GUIStyle gUIStyle = "MiniPopup";
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel(label, gUIStyle);
			int[] array = new int[]
			{
				Array.IndexOf<string>(paths, selectedString)
			};
			GUIContent content = new GUIContent(names[array[0]]);
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, gUIStyle);
			PreferencesWindow.AppsListUserData userData = new PreferencesWindow.AppsListUserData(paths, outString, onChanged);
			if (EditorGUI.ButtonMouseDown(rect, content, FocusType.Native, gUIStyle))
			{
				ArrayUtility.Add<string>(ref names, "Browse...");
				EditorUtility.DisplayCustomMenu(rect, names, array, new EditorUtility.SelectMenuItemFunction(this.AppsListClick), userData);
			}
			GUILayout.EndHorizontal();
		}
		private void AndroidSdkLocation()
		{
			GUIStyle gUIStyle = "MiniPopup";
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel("Android SDK Location", gUIStyle);
			string text = (!string.IsNullOrEmpty(this.m_AndroidSdkPath)) ? this.m_AndroidSdkPath : "Browse...";
			GUIContent content = new GUIContent(text);
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, gUIStyle);
			if (EditorGUI.ButtonMouseDown(rect, content, FocusType.Native, gUIStyle))
			{
				string text2 = AndroidSdkRoot.Browse(this.m_AndroidSdkPath);
				if (!string.IsNullOrEmpty(text2))
				{
					this.m_AndroidSdkPath = text2;
					this.WritePreferences();
					this.ReadPreferences();
				}
			}
			GUILayout.EndHorizontal();
		}
		private string[] BuildAppPathList(string userAppPath, string recentAppsKey)
		{
			string[] array = new string[]
			{
				string.Empty
			};
			if (userAppPath != null && userAppPath.Length != 0 && Array.IndexOf<string>(array, userAppPath) == -1)
			{
				ArrayUtility.Add<string>(ref array, userAppPath);
			}
			for (int i = 0; i < 10; i++)
			{
				string text = EditorPrefs.GetString(recentAppsKey + i);
				if (!File.Exists(text))
				{
					text = string.Empty;
					EditorPrefs.SetString(recentAppsKey + i, text);
				}
				if (text.Length != 0 && Array.IndexOf<string>(array, text) == -1)
				{
					ArrayUtility.Add<string>(ref array, text);
				}
			}
			return array;
		}
		private string[] BuildFriendlyAppNameList(string[] appPathList, string defaultBuiltIn)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < appPathList.Length; i++)
			{
				string text = appPathList[i];
				if (text == string.Empty)
				{
					list.Add(defaultBuiltIn);
				}
				else
				{
					list.Add(OSUtil.GetAppFriendlyName(text));
				}
			}
			return list.ToArray();
		}
	}
}
