using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[InitializeOnLoad]
	internal class AddComponentWindow : EditorWindow
	{
		internal enum Language
		{
			CSharp,
			JavaScript
		}

		private class Element : IComparable
		{
			public int level;

			public GUIContent content;

			public string name
			{
				get
				{
					return this.content.text;
				}
			}

			public virtual int CompareTo(object o)
			{
				return this.name.CompareTo((o as AddComponentWindow.Element).name);
			}
		}

		private class ComponentElement : AddComponentWindow.Element
		{
			public string typeName;

			public string menuPath;

			public bool isLegacy;

			private GUIContent m_LegacyContentCache;

			public GUIContent legacyContent
			{
				get
				{
					if (this.m_LegacyContentCache == null)
					{
						this.m_LegacyContentCache = new GUIContent(this.content);
						GUIContent expr_24 = this.m_LegacyContentCache;
						expr_24.text += " (Legacy)";
					}
					return this.m_LegacyContentCache;
				}
			}

			public ComponentElement(int level, string name, string menuPath, string commandString)
			{
				this.level = level;
				this.typeName = name.Replace(" ", "");
				this.menuPath = menuPath;
				this.isLegacy = menuPath.Contains("Legacy");
				if (commandString.StartsWith("SCRIPT"))
				{
					int instanceID = int.Parse(commandString.Substring(6));
					UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceID);
					Texture miniThumbnail = AssetPreview.GetMiniThumbnail(obj);
					this.content = new GUIContent(name, miniThumbnail);
				}
				else
				{
					int classID = int.Parse(commandString);
					this.content = new GUIContent(name, AssetPreview.GetMiniTypeThumbnailFromClassID(classID));
				}
			}

			public override int CompareTo(object o)
			{
				int result;
				if (o is AddComponentWindow.ComponentElement)
				{
					AddComponentWindow.ComponentElement componentElement = (AddComponentWindow.ComponentElement)o;
					if (this.isLegacy && !componentElement.isLegacy)
					{
						result = 1;
						return result;
					}
					if (!this.isLegacy && componentElement.isLegacy)
					{
						result = -1;
						return result;
					}
				}
				result = base.CompareTo(o);
				return result;
			}
		}

		[Serializable]
		private class GroupElement : AddComponentWindow.Element
		{
			public Vector2 scroll;

			public int selectedIndex = 0;

			public GroupElement(int level, string name)
			{
				this.level = level;
				this.content = new GUIContent(name);
			}
		}

		private class NewScriptElement : AddComponentWindow.GroupElement
		{
			private char[] kInvalidPathChars = new char[]
			{
				'<',
				'>',
				':',
				'"',
				'|',
				'?',
				'*',
				'\0'
			};

			private char[] kPathSepChars = new char[]
			{
				'/',
				'\\'
			};

			private const string kResourcesTemplatePath = "Resources/ScriptTemplates";

			private string m_Directory = string.Empty;

			private string extension
			{
				get
				{
					AddComponentWindow.Language s_Lang = AddComponentWindow.s_Lang;
					string result;
					if (s_Lang != AddComponentWindow.Language.CSharp)
					{
						if (s_Lang != AddComponentWindow.Language.JavaScript)
						{
							throw new ArgumentOutOfRangeException();
						}
						result = "js";
					}
					else
					{
						result = "cs";
					}
					return result;
				}
			}

			private string templatePath
			{
				get
				{
					string path = Path.Combine(EditorApplication.applicationContentsPath, "Resources/ScriptTemplates");
					AddComponentWindow.Language s_Lang = AddComponentWindow.s_Lang;
					string result;
					if (s_Lang != AddComponentWindow.Language.JavaScript)
					{
						if (s_Lang != AddComponentWindow.Language.CSharp)
						{
							throw new ArgumentOutOfRangeException();
						}
						result = Path.Combine(path, "81-C# Script-NewBehaviourScript.cs.txt");
					}
					else
					{
						result = Path.Combine(path, "82-Javascript-NewBehaviourScript.js.txt");
					}
					return result;
				}
			}

			public NewScriptElement() : base(1, "New Script")
			{
			}

			public void OnGUI()
			{
				GUILayout.Label("Name", EditorStyles.label, new GUILayoutOption[0]);
				EditorGUI.FocusTextInControl("NewScriptName");
				GUI.SetNextControlName("NewScriptName");
				AddComponentWindow.className = EditorGUILayout.TextField(AddComponentWindow.className, new GUILayoutOption[0]);
				EditorGUILayout.Space();
				AddComponentWindow.Language language = (AddComponentWindow.Language)EditorGUILayout.EnumPopup("Language", AddComponentWindow.s_Lang, new GUILayoutOption[0]);
				if (language != AddComponentWindow.s_Lang)
				{
					AddComponentWindow.s_Lang = language;
					EditorPrefs.SetInt("NewScriptLanguage", (int)language);
				}
				EditorGUILayout.Space();
				bool flag = this.CanCreate();
				if (!flag && AddComponentWindow.className != "")
				{
					GUILayout.Label(this.GetError(), EditorStyles.helpBox, new GUILayoutOption[0]);
				}
				GUILayout.FlexibleSpace();
				using (new EditorGUI.DisabledScope(!flag))
				{
					if (GUILayout.Button("Create and Add", new GUILayoutOption[0]))
					{
						this.Create();
					}
				}
				EditorGUILayout.Space();
			}

			public bool CanCreate()
			{
				return AddComponentWindow.className.Length > 0 && !File.Exists(this.TargetPath()) && !this.ClassAlreadyExists() && !this.ClassNameIsInvalid() && !this.InvalidTargetPath();
			}

			private string GetError()
			{
				string result = string.Empty;
				if (AddComponentWindow.className != string.Empty)
				{
					if (File.Exists(this.TargetPath()))
					{
						result = "A script called \"" + AddComponentWindow.className + "\" already exists at that path.";
					}
					else if (this.ClassAlreadyExists())
					{
						result = "A class called \"" + AddComponentWindow.className + "\" already exists.";
					}
					else if (this.ClassNameIsInvalid())
					{
						result = "The script name may only consist of a-z, A-Z, 0-9, _.";
					}
					else if (this.InvalidTargetPath())
					{
						result = "The folder path contains invalid characters.";
					}
				}
				return result;
			}

			public void Create()
			{
				if (this.CanCreate())
				{
					this.CreateScript();
					GameObject[] gameObjects = AddComponentWindow.gameObjects;
					for (int i = 0; i < gameObjects.Length; i++)
					{
						GameObject gameObject = gameObjects[i];
						MonoScript monoScript = AssetDatabase.LoadAssetAtPath(this.TargetPath(), typeof(MonoScript)) as MonoScript;
						monoScript.SetScriptTypeWasJustCreatedFromComponentMenu();
						InternalEditorUtility.AddScriptComponentUncheckedUndoable(gameObject, monoScript);
					}
					AddComponentWindow.s_AddComponentWindow.Close();
				}
			}

			private bool InvalidTargetPath()
			{
				return this.m_Directory.IndexOfAny(this.kInvalidPathChars) >= 0 || this.TargetDir().Split(this.kPathSepChars, StringSplitOptions.None).Contains(string.Empty);
			}

			public string TargetPath()
			{
				return Path.Combine(this.TargetDir(), AddComponentWindow.className + "." + this.extension);
			}

			private string TargetDir()
			{
				return Path.Combine("Assets", this.m_Directory.Trim(this.kPathSepChars));
			}

			private bool ClassNameIsInvalid()
			{
				return !CodeGenerator.IsValidLanguageIndependentIdentifier(AddComponentWindow.className);
			}

			private bool ClassExists(string className)
			{
				return AppDomain.CurrentDomain.GetAssemblies().Any((Assembly a) => a.GetType(className, false) != null);
			}

			private bool ClassAlreadyExists()
			{
				return !(AddComponentWindow.className == string.Empty) && this.ClassExists(AddComponentWindow.className);
			}

			private void CreateScript()
			{
				ProjectWindowUtil.CreateScriptAssetFromTemplate(this.TargetPath(), this.templatePath);
				AssetDatabase.Refresh();
			}
		}

		private class Styles
		{
			public GUIStyle header = new GUIStyle(EditorStyles.inspectorBig);

			public GUIStyle componentButton = new GUIStyle("PR Label");

			public GUIStyle groupButton;

			public GUIStyle background = "grey_border";

			public GUIStyle previewBackground = "PopupCurveSwatchBackground";

			public GUIStyle previewHeader = new GUIStyle(EditorStyles.label);

			public GUIStyle previewText = new GUIStyle(EditorStyles.wordWrappedLabel);

			public GUIStyle rightArrow = "AC RightArrow";

			public GUIStyle leftArrow = "AC LeftArrow";

			public Styles()
			{
				this.header.font = EditorStyles.boldLabel.font;
				this.componentButton.alignment = TextAnchor.MiddleLeft;
				this.componentButton.padding.left -= 15;
				this.componentButton.fixedHeight = 20f;
				this.groupButton = new GUIStyle(this.componentButton);
				this.groupButton.padding.left += 17;
				this.previewText.padding.left += 3;
				this.previewText.padding.right += 3;
				this.previewHeader.padding.left++;
				this.previewHeader.padding.right += 3;
				this.previewHeader.padding.top += 3;
				this.previewHeader.padding.bottom += 2;
			}
		}

		private const AddComponentWindow.Language kDefaultLanguage = AddComponentWindow.Language.CSharp;

		private const int kHeaderHeight = 30;

		private const int kWindowHeight = 320;

		private const int kHelpHeight = 0;

		private const string kLanguageEditorPrefName = "NewScriptLanguage";

		private const string kComponentSearch = "ComponentSearchString";

		private static AddComponentWindow.Styles s_Styles;

		private static AddComponentWindow s_AddComponentWindow;

		private static long s_LastClosedTime;

		internal static AddComponentWindow.Language s_Lang;

		private static bool s_DirtyList;

		private string m_ClassName = "";

		private GameObject[] m_GameObjects;

		private AddComponentWindow.Element[] m_Tree;

		private AddComponentWindow.Element[] m_SearchResultTree;

		private List<AddComponentWindow.GroupElement> m_Stack = new List<AddComponentWindow.GroupElement>();

		private float m_Anim = 1f;

		private int m_AnimTarget = 1;

		private long m_LastTime = 0L;

		private bool m_ScrollToSelected = false;

		private string m_DelayedSearch = null;

		private string m_Search = "";

		private const string kSearchHeader = "Search";

		internal static string className
		{
			get
			{
				return AddComponentWindow.s_AddComponentWindow.m_ClassName;
			}
			set
			{
				AddComponentWindow.s_AddComponentWindow.m_ClassName = value;
			}
		}

		internal static GameObject[] gameObjects
		{
			get
			{
				return AddComponentWindow.s_AddComponentWindow.m_GameObjects;
			}
		}

		private bool hasSearch
		{
			get
			{
				return !string.IsNullOrEmpty(this.m_Search);
			}
		}

		private AddComponentWindow.GroupElement activeParent
		{
			get
			{
				return this.m_Stack[this.m_Stack.Count - 2 + this.m_AnimTarget];
			}
		}

		private AddComponentWindow.Element[] activeTree
		{
			get
			{
				return (!this.hasSearch) ? this.m_Tree : this.m_SearchResultTree;
			}
		}

		private AddComponentWindow.Element activeElement
		{
			get
			{
				AddComponentWindow.Element result;
				if (this.activeTree == null)
				{
					result = null;
				}
				else
				{
					List<AddComponentWindow.Element> children = this.GetChildren(this.activeTree, this.activeParent);
					if (children.Count == 0)
					{
						result = null;
					}
					else
					{
						result = children[this.activeParent.selectedIndex];
					}
				}
				return result;
			}
		}

		private bool isAnimating
		{
			get
			{
				return this.m_Anim != (float)this.m_AnimTarget;
			}
		}

		static AddComponentWindow()
		{
			AddComponentWindow.s_AddComponentWindow = null;
			AddComponentWindow.s_DirtyList = false;
			AddComponentWindow.s_DirtyList = true;
		}

		private void OnEnable()
		{
			AddComponentWindow.s_AddComponentWindow = this;
			AddComponentWindow.s_Lang = (AddComponentWindow.Language)EditorPrefs.GetInt("NewScriptLanguage", 0);
			if (!Enum.IsDefined(typeof(AddComponentWindow.Language), AddComponentWindow.s_Lang))
			{
				EditorPrefs.SetInt("NewScriptLanguage", 0);
				AddComponentWindow.s_Lang = AddComponentWindow.Language.CSharp;
			}
			this.m_Search = EditorPrefs.GetString("ComponentSearchString", "");
		}

		private void OnDisable()
		{
			AddComponentWindow.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
			AddComponentWindow.s_AddComponentWindow = null;
		}

		private static InspectorWindow FirstInspectorWithGameObject()
		{
			InspectorWindow result;
			foreach (InspectorWindow current in InspectorWindow.GetInspectors())
			{
				if (current.GetInspectedObject() is GameObject)
				{
					result = current;
					return result;
				}
			}
			result = null;
			return result;
		}

		internal static bool ValidateAddComponentMenuItem()
		{
			return AddComponentWindow.FirstInspectorWithGameObject() != null;
		}

		internal static void ExecuteAddComponentMenuItem()
		{
			InspectorWindow inspectorWindow = AddComponentWindow.FirstInspectorWithGameObject();
			if (inspectorWindow != null)
			{
				inspectorWindow.SendEvent(EditorGUIUtility.CommandEvent("OpenAddComponentDropdown"));
			}
		}

		internal static bool Show(Rect rect, GameObject[] gos)
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(AddComponentWindow));
			bool result;
			if (array.Length > 0)
			{
				((EditorWindow)array[0]).Close();
				result = false;
			}
			else
			{
				long num = DateTime.Now.Ticks / 10000L;
				if (num >= AddComponentWindow.s_LastClosedTime + 50L)
				{
					Event.current.Use();
					if (AddComponentWindow.s_AddComponentWindow == null)
					{
						AddComponentWindow.s_AddComponentWindow = ScriptableObject.CreateInstance<AddComponentWindow>();
					}
					AddComponentWindow.s_AddComponentWindow.Init(rect);
					AddComponentWindow.s_AddComponentWindow.m_GameObjects = gos;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private void Init(Rect buttonRect)
		{
			buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
			this.CreateComponentTree();
			base.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 320f), null, ShowMode.PopupMenuWithKeyboardFocus);
			base.Focus();
			this.m_Parent.AddToAuxWindowList();
			base.wantsMouseMove = true;
		}

		private void CreateComponentTree()
		{
			string[] submenus = Unsupported.GetSubmenus("Component");
			string[] submenusCommands = Unsupported.GetSubmenusCommands("Component");
			List<string> list = new List<string>();
			List<AddComponentWindow.Element> list2 = new List<AddComponentWindow.Element>();
			for (int i = 0; i < submenus.Length; i++)
			{
				if (!(submenusCommands[i] == "ADD"))
				{
					string text = submenus[i];
					string[] array = text.Split(new char[]
					{
						'/'
					});
					while (array.Length - 1 < list.Count)
					{
						list.RemoveAt(list.Count - 1);
					}
					while (list.Count > 0 && array[list.Count - 1] != list[list.Count - 1])
					{
						list.RemoveAt(list.Count - 1);
					}
					while (array.Length - 1 > list.Count)
					{
						list2.Add(new AddComponentWindow.GroupElement(list.Count, LocalizationDatabase.GetLocalizedString(array[list.Count])));
						list.Add(array[list.Count]);
					}
					list2.Add(new AddComponentWindow.ComponentElement(list.Count, LocalizationDatabase.GetLocalizedString(array[array.Length - 1]), text, submenusCommands[i]));
				}
			}
			list2.Add(new AddComponentWindow.NewScriptElement());
			this.m_Tree = list2.ToArray();
			if (this.m_Stack.Count == 0)
			{
				this.m_Stack.Add(this.m_Tree[0] as AddComponentWindow.GroupElement);
			}
			else
			{
				AddComponentWindow.GroupElement groupElement = this.m_Tree[0] as AddComponentWindow.GroupElement;
				int level = 0;
				while (true)
				{
					AddComponentWindow.GroupElement groupElement2 = this.m_Stack[level];
					this.m_Stack[level] = groupElement;
					this.m_Stack[level].selectedIndex = groupElement2.selectedIndex;
					this.m_Stack[level].scroll = groupElement2.scroll;
					level++;
					if (level == this.m_Stack.Count)
					{
						break;
					}
					List<AddComponentWindow.Element> children = this.GetChildren(this.activeTree, groupElement);
					AddComponentWindow.Element element = children.FirstOrDefault((AddComponentWindow.Element c) => c.name == this.m_Stack[level].name);
					if (element != null && element is AddComponentWindow.GroupElement)
					{
						groupElement = (element as AddComponentWindow.GroupElement);
					}
					else
					{
						while (this.m_Stack.Count > level)
						{
							this.m_Stack.RemoveAt(level);
						}
					}
				}
			}
			AddComponentWindow.s_DirtyList = false;
			this.RebuildSearch();
		}

		internal void OnGUI()
		{
			if (AddComponentWindow.s_Styles == null)
			{
				AddComponentWindow.s_Styles = new AddComponentWindow.Styles();
			}
			GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, AddComponentWindow.s_Styles.background);
			if (AddComponentWindow.s_DirtyList)
			{
				this.CreateComponentTree();
			}
			this.HandleKeyboard();
			GUILayout.Space(7f);
			if (!(this.activeParent is AddComponentWindow.NewScriptElement))
			{
				EditorGUI.FocusTextInControl("ComponentSearch");
			}
			Rect rect = GUILayoutUtility.GetRect(10f, 20f);
			rect.x += 8f;
			rect.width -= 16f;
			GUI.SetNextControlName("ComponentSearch");
			using (new EditorGUI.DisabledScope(this.activeParent is AddComponentWindow.NewScriptElement))
			{
				string text = EditorGUI.SearchField(rect, this.m_DelayedSearch ?? this.m_Search);
				if (text != this.m_Search || this.m_DelayedSearch != null)
				{
					if (!this.isAnimating)
					{
						this.m_Search = (this.m_DelayedSearch ?? text);
						EditorPrefs.SetString("ComponentSearchString", this.m_Search);
						this.RebuildSearch();
						this.m_DelayedSearch = null;
					}
					else
					{
						this.m_DelayedSearch = text;
					}
				}
			}
			this.ListGUI(this.activeTree, this.m_Anim, this.GetElementRelative(0), this.GetElementRelative(-1));
			if (this.m_Anim < 1f)
			{
				this.ListGUI(this.activeTree, this.m_Anim + 1f, this.GetElementRelative(-1), this.GetElementRelative(-2));
			}
			if (this.isAnimating && Event.current.type == EventType.Repaint)
			{
				long ticks = DateTime.Now.Ticks;
				float num = (float)(ticks - this.m_LastTime) / 1E+07f;
				this.m_LastTime = ticks;
				this.m_Anim = Mathf.MoveTowards(this.m_Anim, (float)this.m_AnimTarget, num * 4f);
				if (this.m_AnimTarget == 0 && this.m_Anim == 0f)
				{
					this.m_Anim = 1f;
					this.m_AnimTarget = 1;
					this.m_Stack.RemoveAt(this.m_Stack.Count - 1);
				}
				base.Repaint();
			}
		}

		private void HandleKeyboard()
		{
			Event current = Event.current;
			if (current.type == EventType.KeyDown)
			{
				if (this.activeParent is AddComponentWindow.NewScriptElement)
				{
					if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
					{
						(this.activeParent as AddComponentWindow.NewScriptElement).Create();
						current.Use();
						GUIUtility.ExitGUI();
					}
					if (current.keyCode == KeyCode.Escape)
					{
						this.GoToParent();
						current.Use();
					}
				}
				else
				{
					if (current.keyCode == KeyCode.DownArrow)
					{
						this.activeParent.selectedIndex++;
						this.activeParent.selectedIndex = Mathf.Min(this.activeParent.selectedIndex, this.GetChildren(this.activeTree, this.activeParent).Count - 1);
						this.m_ScrollToSelected = true;
						current.Use();
					}
					if (current.keyCode == KeyCode.UpArrow)
					{
						this.activeParent.selectedIndex--;
						this.activeParent.selectedIndex = Mathf.Max(this.activeParent.selectedIndex, 0);
						this.m_ScrollToSelected = true;
						current.Use();
					}
					if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
					{
						this.GoToChild(this.activeElement, true);
						current.Use();
					}
					if (!this.hasSearch)
					{
						if (current.keyCode == KeyCode.LeftArrow || current.keyCode == KeyCode.Backspace)
						{
							this.GoToParent();
							current.Use();
						}
						if (current.keyCode == KeyCode.RightArrow)
						{
							this.GoToChild(this.activeElement, false);
							current.Use();
						}
						if (current.keyCode == KeyCode.Escape)
						{
							base.Close();
							current.Use();
						}
					}
				}
			}
		}

		private void RebuildSearch()
		{
			if (!this.hasSearch)
			{
				this.m_SearchResultTree = null;
				if (this.m_Stack[this.m_Stack.Count - 1].name == "Search")
				{
					this.m_Stack.Clear();
					this.m_Stack.Add(this.m_Tree[0] as AddComponentWindow.GroupElement);
				}
				this.m_AnimTarget = 1;
				this.m_LastTime = DateTime.Now.Ticks;
				this.m_ClassName = "NewBehaviourScript";
			}
			else
			{
				this.m_ClassName = this.m_Search;
				string[] array = this.m_Search.ToLower().Split(new char[]
				{
					' '
				});
				List<AddComponentWindow.Element> list = new List<AddComponentWindow.Element>();
				List<AddComponentWindow.Element> list2 = new List<AddComponentWindow.Element>();
				AddComponentWindow.Element[] tree = this.m_Tree;
				for (int i = 0; i < tree.Length; i++)
				{
					AddComponentWindow.Element element = tree[i];
					if (element is AddComponentWindow.ComponentElement)
					{
						string text = element.name.ToLower().Replace(" ", "");
						bool flag = true;
						bool flag2 = false;
						for (int j = 0; j < array.Length; j++)
						{
							string value = array[j];
							if (!text.Contains(value))
							{
								flag = false;
								break;
							}
							if (j == 0 && text.StartsWith(value))
							{
								flag2 = true;
							}
						}
						if (flag)
						{
							if (flag2)
							{
								list.Add(element);
							}
							else
							{
								list2.Add(element);
							}
						}
					}
				}
				list.Sort();
				list2.Sort();
				List<AddComponentWindow.Element> list3 = new List<AddComponentWindow.Element>();
				list3.Add(new AddComponentWindow.GroupElement(0, "Search"));
				list3.AddRange(list);
				list3.AddRange(list2);
				list3.Add(this.m_Tree[this.m_Tree.Length - 1]);
				this.m_SearchResultTree = list3.ToArray();
				this.m_Stack.Clear();
				this.m_Stack.Add(this.m_SearchResultTree[0] as AddComponentWindow.GroupElement);
				if (this.GetChildren(this.activeTree, this.activeParent).Count >= 1)
				{
					this.activeParent.selectedIndex = 0;
				}
				else
				{
					this.activeParent.selectedIndex = -1;
				}
			}
		}

		private AddComponentWindow.GroupElement GetElementRelative(int rel)
		{
			int num = this.m_Stack.Count + rel - 1;
			AddComponentWindow.GroupElement result;
			if (num < 0)
			{
				result = null;
			}
			else
			{
				result = this.m_Stack[num];
			}
			return result;
		}

		private void GoToParent()
		{
			if (this.m_Stack.Count > 1)
			{
				this.m_AnimTarget = 0;
				this.m_LastTime = DateTime.Now.Ticks;
			}
		}

		private void GoToChild(AddComponentWindow.Element e, bool addIfComponent)
		{
			if (e is AddComponentWindow.NewScriptElement)
			{
				if (!this.hasSearch)
				{
					this.m_ClassName = AssetDatabase.GenerateUniqueAssetPath((e as AddComponentWindow.NewScriptElement).TargetPath());
					this.m_ClassName = Path.GetFileNameWithoutExtension(this.m_ClassName);
				}
			}
			if (e is AddComponentWindow.ComponentElement)
			{
				if (addIfComponent)
				{
					EditorApplication.ExecuteMenuItemOnGameObjects(((AddComponentWindow.ComponentElement)e).menuPath, this.m_GameObjects);
					base.Close();
				}
			}
			else if (!this.hasSearch || e is AddComponentWindow.NewScriptElement)
			{
				this.m_LastTime = DateTime.Now.Ticks;
				if (this.m_AnimTarget == 0)
				{
					this.m_AnimTarget = 1;
				}
				else if (this.m_Anim == 1f)
				{
					this.m_Anim = 0f;
					this.m_Stack.Add(e as AddComponentWindow.GroupElement);
				}
			}
		}

		private void ListGUI(AddComponentWindow.Element[] tree, float anim, AddComponentWindow.GroupElement parent, AddComponentWindow.GroupElement grandParent)
		{
			anim = Mathf.Floor(anim) + Mathf.SmoothStep(0f, 1f, Mathf.Repeat(anim, 1f));
			Rect position = base.position;
			position.x = base.position.width * (1f - anim) + 1f;
			position.y = 30f;
			position.height -= 30f;
			position.width -= 2f;
			GUILayout.BeginArea(position);
			Rect rect = GUILayoutUtility.GetRect(10f, 25f);
			string name = parent.name;
			GUI.Label(rect, name, AddComponentWindow.s_Styles.header);
			if (grandParent != null)
			{
				Rect position2 = new Rect(rect.x + 4f, rect.y + 7f, 13f, 13f);
				if (Event.current.type == EventType.Repaint)
				{
					AddComponentWindow.s_Styles.leftArrow.Draw(position2, false, false, false, false);
				}
				if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
				{
					this.GoToParent();
					Event.current.Use();
				}
			}
			if (parent is AddComponentWindow.NewScriptElement)
			{
				(parent as AddComponentWindow.NewScriptElement).OnGUI();
			}
			else
			{
				this.ListGUI(tree, parent);
			}
			GUILayout.EndArea();
		}

		private void ListGUI(AddComponentWindow.Element[] tree, AddComponentWindow.GroupElement parent)
		{
			parent.scroll = GUILayout.BeginScrollView(parent.scroll, new GUILayoutOption[0]);
			EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
			List<AddComponentWindow.Element> children = this.GetChildren(tree, parent);
			Rect rect = default(Rect);
			for (int i = 0; i < children.Count; i++)
			{
				AddComponentWindow.Element element = children[i];
				Rect rect2 = GUILayoutUtility.GetRect(16f, 20f, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDown)
				{
					if (parent.selectedIndex != i && rect2.Contains(Event.current.mousePosition))
					{
						parent.selectedIndex = i;
						base.Repaint();
					}
				}
				bool flag = false;
				if (i == parent.selectedIndex)
				{
					flag = true;
					rect = rect2;
				}
				if (Event.current.type == EventType.Repaint)
				{
					GUIStyle gUIStyle = AddComponentWindow.s_Styles.groupButton;
					GUIContent content = element.content;
					bool flag2 = element is AddComponentWindow.ComponentElement;
					if (flag2)
					{
						AddComponentWindow.ComponentElement componentElement = (AddComponentWindow.ComponentElement)element;
						gUIStyle = AddComponentWindow.s_Styles.componentButton;
						if (componentElement.isLegacy && this.hasSearch)
						{
							content = componentElement.legacyContent;
						}
					}
					gUIStyle.Draw(rect2, content, false, false, flag, flag);
					if (!flag2)
					{
						Rect position = new Rect(rect2.x + rect2.width - 13f, rect2.y + 4f, 13f, 13f);
						AddComponentWindow.s_Styles.rightArrow.Draw(position, false, false, false, false);
					}
				}
				if (Event.current.type == EventType.MouseDown && rect2.Contains(Event.current.mousePosition))
				{
					Event.current.Use();
					parent.selectedIndex = i;
					this.GoToChild(element, true);
				}
			}
			EditorGUIUtility.SetIconSize(Vector2.zero);
			GUILayout.EndScrollView();
			if (this.m_ScrollToSelected && Event.current.type == EventType.Repaint)
			{
				this.m_ScrollToSelected = false;
				Rect lastRect = GUILayoutUtility.GetLastRect();
				if (rect.yMax - lastRect.height > parent.scroll.y)
				{
					parent.scroll.y = rect.yMax - lastRect.height;
					base.Repaint();
				}
				if (rect.y < parent.scroll.y)
				{
					parent.scroll.y = rect.y;
					base.Repaint();
				}
			}
		}

		private List<AddComponentWindow.Element> GetChildren(AddComponentWindow.Element[] tree, AddComponentWindow.Element parent)
		{
			List<AddComponentWindow.Element> list = new List<AddComponentWindow.Element>();
			int num = -1;
			int i;
			for (i = 0; i < tree.Length; i++)
			{
				if (tree[i] == parent)
				{
					num = parent.level + 1;
					i++;
					break;
				}
			}
			List<AddComponentWindow.Element> result;
			if (num == -1)
			{
				result = list;
			}
			else
			{
				while (i < tree.Length)
				{
					AddComponentWindow.Element element = tree[i];
					if (element.level < num)
					{
						break;
					}
					if (element.level <= num || this.hasSearch)
					{
						list.Add(element);
					}
					i++;
				}
				result = list;
			}
			return result;
		}
	}
}
