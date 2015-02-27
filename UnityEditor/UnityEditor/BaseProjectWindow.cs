using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.ProjectWindowCallback;
using UnityEditorInternal;
using UnityEditorInternal.VersionControl;
using UnityEngine;
namespace UnityEditor
{
	internal class BaseProjectWindow : SearchableEditorWindow
	{
		private class DropData
		{
			public int[] expandedArrayBeforeDrag;
			public int lastControlID;
			public int dropOnControlID;
			public int dropPreviousControlID;
			public double expandItemBeginTimer;
		}
		internal class Styles
		{
			public GUIStyle foldout = "IN Foldout";
			public GUIStyle insertion = "PR Insertion";
			public GUIStyle label = "PR Label";
			public GUIStyle hiLabel = "HI Label";
			public GUIStyle ping = "PR Ping";
		}
		private enum NameEditMode
		{
			None,
			Found,
			Renaming,
			PreImportNaming
		}
		private const double kDropExpandTimeout = 0.7;
		private const float kFoldoutSize = 14f;
		private const float kIndent = 16f;
		private const float kBaseIndent = 17f;
		private static float m_RowHeight = 16f;
		private static float m_DropNextToAreaHeight = 6f;
		[SerializeField]
		private int[] m_ExpandedArray = new int[0];
		private bool m_ProjectViewInternalSelectionChange;
		public Vector2 m_ScrollPosition;
		public Vector2 m_ScrollPositionFiltered;
		[NonSerialized]
		private Rect m_ScreenRect;
		private PingData m_Ping = new PingData();
		private float m_FocusTime;
		private List<int> m_CurrentDragSelectionIDs = new List<int>();
		private BaseProjectWindow.DropData m_DropData;
		private bool m_StillWantsNameEditing;
		private static Color[] s_HierarchyColors = new Color[]
		{
			Color.black,
			new Color(0f, 0.15f, 0.51f, 1f),
			new Color(0.25f, 0.05f, 0.05f, 1f),
			Color.black,
			Color.white,
			new Color(0.67f, 0.76f, 1f),
			new Color(1f, 0.71f, 0.71f, 1f),
			Color.white
		};
		private static Color[] s_DarkColors = new Color[]
		{
			new Color(0.705f, 0.705f, 0.705f, 1f),
			new Color(0.3f, 0.5f, 0.85f, 1f),
			new Color(0.7f, 0.4f, 0.4f, 1f),
			new Color(0.705f, 0.705f, 0.705f, 1f),
			Color.white,
			new Color(0.67f, 0.76f, 1f),
			new Color(1f, 0.71f, 0.71f, 1f),
			Color.white
		};
		private Rect m_NameEditRect = new Rect(0f, 0f, 1f, 1f);
		private string m_NameEditString = string.Empty;
		private int m_EditNameInstanceID;
		private BaseProjectWindow.NameEditMode m_RealEditNameMode;
		private EndNameEditAction m_EndNameEditAction;
		private string m_NewAssetFolder;
		private string m_NewAssetName;
		private Texture2D m_NewAssetIcon;
		private string m_NewAssetExtension;
		private int m_NewAssetIndent;
		private int m_NewAssetSortInstanceID;
		private string m_NewAssetResourceFile;
		private bool m_DidSelectSearchResult;
		private bool m_NeedsRelayout;
		private int m_FrameAfterRelayout;
		private FilteredHierarchy m_FilteredHierarchy;
		private static BaseProjectWindow.Styles ms_Styles;
		private float IconWidth
		{
			get
			{
				return (float)((this.m_HierarchyType != HierarchyType.Assets) ? -3 : 14);
			}
		}
		private BaseProjectWindow.NameEditMode m_EditNameMode
		{
			get
			{
				return this.m_RealEditNameMode;
			}
			set
			{
				if (value == this.m_RealEditNameMode)
				{
					return;
				}
				this.m_RealEditNameMode = value;
			}
		}
		private GUIStyle labelStyle
		{
			get
			{
				return (this.m_HierarchyType != HierarchyType.Assets) ? BaseProjectWindow.ms_Styles.hiLabel : BaseProjectWindow.ms_Styles.label;
			}
		}
		public IHierarchyProperty GetNewHierarchyProperty()
		{
			if (this.m_FilteredHierarchy == null)
			{
				this.m_FilteredHierarchy = new FilteredHierarchy(this.m_HierarchyType);
				this.m_FilteredHierarchy.searchFilter = SearchableEditorWindow.CreateFilter(this.m_SearchFilter, this.m_SearchMode);
			}
			return FilteredHierarchyProperty.CreateHierarchyPropertyForFilter(this.m_FilteredHierarchy);
		}
		private void SetExpanded(int instanceID, bool expand)
		{
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i < this.m_ExpandedArray.Length; i++)
			{
				hashtable.Add(this.m_ExpandedArray[i], null);
			}
			if (expand != hashtable.Contains(instanceID))
			{
				if (expand)
				{
					hashtable.Add(instanceID, null);
				}
				else
				{
					hashtable.Remove(instanceID);
				}
				this.m_ExpandedArray = new int[hashtable.Count];
				int num = 0;
				foreach (int num2 in hashtable.Keys)
				{
					this.m_ExpandedArray[num] = num2;
					num++;
				}
			}
			if (this.m_HierarchyType == HierarchyType.Assets)
			{
				InternalEditorUtility.expandedProjectWindowItems = this.m_ExpandedArray;
			}
		}
		public void SetExpandedRecurse(int instanceID, bool expand)
		{
			IHierarchyProperty hierarchyProperty = new HierarchyProperty(this.m_HierarchyType);
			if (hierarchyProperty.Find(instanceID, this.m_ExpandedArray))
			{
				this.SetExpanded(instanceID, expand);
				int depth = hierarchyProperty.depth;
				while (hierarchyProperty.Next(null) && hierarchyProperty.depth > depth)
				{
					this.SetExpanded(hierarchyProperty.instanceID, expand);
				}
			}
		}
		private void OnFocus()
		{
			this.m_FocusTime = Time.realtimeSinceStartup;
		}
		private void OnLostFocus()
		{
			this.m_StillWantsNameEditing = false;
			this.EndNameEditing();
		}
		public override void OnEnable()
		{
			base.OnEnable();
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
		}
		public override void OnDisable()
		{
			base.OnDisable();
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
		}
		private IHierarchyProperty GetFirstSelected()
		{
			int num = 1000000000;
			IHierarchyProperty result = null;
			int[] instanceIDs = Selection.instanceIDs;
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				int instanceID = instanceIDs[i];
				IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
				if (newHierarchyProperty.Find(instanceID, this.m_ExpandedArray) && newHierarchyProperty.row < num)
				{
					num = newHierarchyProperty.row;
					result = newHierarchyProperty;
				}
			}
			return result;
		}
		private void OnProjectChange()
		{
			if (this.m_HierarchyType == HierarchyType.Assets)
			{
				this.CancelNameEditing();
				if (this.m_FilteredHierarchy != null)
				{
					this.m_FilteredHierarchy.ResultsChanged();
				}
				base.Repaint();
			}
		}
		private void OnSelectionChange()
		{
			if (Selection.activeInstanceID != this.m_EditNameInstanceID)
			{
				this.EndNameEditing();
			}
			this.m_StillWantsNameEditing = false;
			this.RevealObjects(Selection.instanceIDs);
			if (this.m_ProjectViewInternalSelectionChange)
			{
				this.m_ProjectViewInternalSelectionChange = false;
			}
			else
			{
				if (this.m_HierarchyType == HierarchyType.GameObjects)
				{
					this.FrameObject(Selection.activeInstanceID);
				}
			}
			base.Repaint();
		}
		private void OnHierarchyChange()
		{
			if (this.m_HierarchyType == HierarchyType.GameObjects)
			{
				this.EndNameEditing();
				if (this.m_FilteredHierarchy != null)
				{
					this.m_FilteredHierarchy.ResultsChanged();
				}
				base.Repaint();
			}
		}
		private IHierarchyProperty GetLastSelected()
		{
			int num = -1;
			IHierarchyProperty result = null;
			int[] instanceIDs = Selection.instanceIDs;
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				int instanceID = instanceIDs[i];
				IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
				if (newHierarchyProperty.Find(instanceID, this.m_ExpandedArray) && newHierarchyProperty.row > num)
				{
					num = newHierarchyProperty.row;
					result = newHierarchyProperty;
				}
			}
			return result;
		}
		private IHierarchyProperty GetActiveSelected()
		{
			return this.GetFirstSelected();
		}
		private IHierarchyProperty GetLast()
		{
			IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
			int num = newHierarchyProperty.CountRemaining(this.m_ExpandedArray);
			if (num == 0)
			{
				return null;
			}
			newHierarchyProperty.Reset();
			if (newHierarchyProperty.Skip(num, this.m_ExpandedArray))
			{
				return newHierarchyProperty;
			}
			return null;
		}
		private IHierarchyProperty GetFirst()
		{
			IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
			if (newHierarchyProperty.Next(this.m_ExpandedArray))
			{
				return newHierarchyProperty;
			}
			return null;
		}
		private void OpenAssetSelection()
		{
			int[] instanceIDs = Selection.instanceIDs;
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				int instanceID = instanceIDs[i];
				if (AssetDatabase.Contains(instanceID))
				{
					AssetDatabase.OpenAsset(instanceID);
				}
			}
		}
		internal override void SetSearchFilter(string searchFilter, SearchableEditorWindow.SearchMode searchMode, bool setAll)
		{
			int num = 0;
			if (this.m_DidSelectSearchResult && this.GetFirstSelected() != null)
			{
				num = this.GetFirstSelected().instanceID;
			}
			base.SetSearchFilter(searchFilter, searchMode, setAll);
			if (this.m_FilteredHierarchy == null)
			{
				this.GetNewHierarchyProperty();
			}
			this.m_FilteredHierarchy.searchFilter = SearchableEditorWindow.CreateFilter(searchFilter, searchMode);
			if (this.m_DidSelectSearchResult)
			{
				if (searchFilter == string.Empty)
				{
					this.m_DidSelectSearchResult = false;
					this.m_NeedsRelayout = true;
				}
				if (num != 0)
				{
					this.FrameObject(num, false);
				}
			}
		}
		internal override void ClickedSearchField()
		{
			this.EndNameEditing();
		}
		private void ProjectWindowTitle()
		{
			GUILayout.BeginHorizontal("Toolbar", new GUILayoutOption[0]);
			if (this.m_HierarchyType == HierarchyType.Assets)
			{
				this.CreateAssetPopup();
				GUILayout.Space(6f);
			}
			else
			{
				this.CreateGameObjectPopup();
				GUILayout.Space(6f);
			}
			if (this.m_FilteredHierarchy == null)
			{
				this.GetNewHierarchyProperty();
			}
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
			if (Event.current.GetTypeForControl(controlID) == EventType.KeyDown)
			{
				KeyCode keyCode = Event.current.keyCode;
				switch (keyCode)
				{
				case KeyCode.KeypadEnter:
				case KeyCode.UpArrow:
				case KeyCode.DownArrow:
					goto IL_C2;
				case KeyCode.KeypadEquals:
					IL_9F:
					if (keyCode != KeyCode.Return)
					{
						goto IL_C8;
					}
					goto IL_C2;
				case KeyCode.RightArrow:
				case KeyCode.LeftArrow:
					if (!base.hasSearchFilter)
					{
						EditorGUILayout.EndHorizontal();
						return;
					}
					goto IL_C8;
				}
				goto IL_9F;
				IL_C2:
				EditorGUILayout.EndHorizontal();
				return;
			}
			IL_C8:
			GUILayout.FlexibleSpace();
			base.SearchFieldGUI();
			GUILayout.EndHorizontal();
		}
		private void SearchResultPathGUI()
		{
			if (!base.hasSearchFilter)
			{
				return;
			}
			EditorGUILayout.BeginVertical(EditorStyles.inspectorBig, new GUILayoutOption[0]);
			GUILayout.Label("Path:", new GUILayoutOption[0]);
			IHierarchyProperty activeSelected = this.GetActiveSelected();
			if (activeSelected != null)
			{
				IHierarchyProperty hierarchyProperty = new HierarchyProperty(this.m_HierarchyType);
				hierarchyProperty.Find(activeSelected.instanceID, null);
				do
				{
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label(hierarchyProperty.icon, new GUILayoutOption[0]);
					GUILayout.Label(hierarchyProperty.name, new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
				}
				while (hierarchyProperty.Parent());
			}
			EditorGUILayout.EndVertical();
			GUILayout.Space(0f);
		}
		private void OnGUI()
		{
			if (BaseProjectWindow.ms_Styles == null)
			{
				BaseProjectWindow.ms_Styles = new BaseProjectWindow.Styles();
			}
			this.ProjectWindowTitle();
			Rect rect = GUILayoutUtility.GetRect(0f, (float)Screen.width, 0f, (float)Screen.height);
			if (this.m_HierarchyType == HierarchyType.Assets && AssetDatabase.isLocked)
			{
				Debug.LogError("Repainting while performing asset operations!");
				GUILayout.Label("Performing Asset operations", new GUILayoutOption[0]);
				return;
			}
			this.m_ScreenRect = rect;
			EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
			this.HierarchyView();
			this.SearchResultPathGUI();
			EditorGUIUtility.SetIconSize(Vector2.zero);
			if (Event.current.type == EventType.Repaint && this.m_NeedsRelayout)
			{
				this.m_NeedsRelayout = false;
				if (this.m_FrameAfterRelayout != 0)
				{
					this.FrameObject(this.m_FrameAfterRelayout);
					this.m_FrameAfterRelayout = 0;
				}
			}
		}
		private void EndNameEditing()
		{
			BaseProjectWindow.NameEditMode editNameMode = this.m_EditNameMode;
			if (editNameMode == BaseProjectWindow.NameEditMode.Renaming)
			{
				this.m_EditNameMode = BaseProjectWindow.NameEditMode.None;
				ObjectNames.SetNameSmartWithInstanceID(this.m_EditNameInstanceID, this.m_NameEditString);
				return;
			}
			if (editNameMode != BaseProjectWindow.NameEditMode.PreImportNaming)
			{
				return;
			}
			this.m_EditNameMode = BaseProjectWindow.NameEditMode.None;
			if (this.m_NameEditString == string.Empty)
			{
				this.m_NameEditString = this.m_NewAssetName;
			}
			ProjectWindowUtil.EndNameEditAction(this.m_EndNameEditAction, this.m_EditNameInstanceID, this.m_NewAssetFolder + "/" + this.m_NameEditString + this.m_NewAssetExtension, this.m_NewAssetResourceFile);
			this.m_EndNameEditAction = null;
		}
		private void CancelNameEditing()
		{
			this.m_EditNameMode = BaseProjectWindow.NameEditMode.None;
			this.m_EndNameEditAction = null;
		}
		private void BeginNameEditing(int instanceID)
		{
			if (this.NamingAsset())
			{
				this.EndNameEditing();
			}
			if (base.hasSearchFilter)
			{
				this.m_EditNameMode = BaseProjectWindow.NameEditMode.None;
				return;
			}
			IHierarchyProperty hierarchyProperty = new HierarchyProperty(this.m_HierarchyType);
			hierarchyProperty.Find(instanceID, null);
			if (hierarchyProperty.isValid)
			{
				this.m_EditNameMode = BaseProjectWindow.NameEditMode.Renaming;
				this.m_EditNameInstanceID = instanceID;
				EditorGUI.s_RecycledEditor.content.text = hierarchyProperty.name;
				EditorGUI.s_RecycledEditor.SelectAll();
			}
			else
			{
				this.m_EditNameMode = BaseProjectWindow.NameEditMode.None;
			}
		}
		private void OnPlayModeStateChanged()
		{
			this.EndNameEditing();
		}
		internal void BeginPreimportedNameEditing(int instanceID, EndNameEditAction endAction, string pathName, Texture2D icon, string resourceFile)
		{
			if (this.NamingAsset())
			{
				this.EndNameEditing();
			}
			this.m_EndNameEditAction = endAction;
			if (!pathName.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
			{
				pathName = AssetDatabase.GetUniquePathNameAtSelectedPath(pathName);
			}
			else
			{
				pathName = AssetDatabase.GenerateUniqueAssetPath(pathName);
			}
			this.m_NewAssetFolder = Path.GetDirectoryName(pathName);
			this.m_NewAssetIcon = icon;
			this.m_NewAssetName = Path.GetFileNameWithoutExtension(pathName);
			this.m_NewAssetExtension = Path.GetExtension(pathName);
			this.m_NewAssetIndent = 0;
			this.m_NewAssetSortInstanceID = 0;
			this.m_NewAssetResourceFile = resourceFile;
			Selection.activeObject = EditorUtility.InstanceIDToObject(instanceID);
			if (base.hasSearchFilter)
			{
				this.m_SearchFilter = string.Empty;
				this.m_SearchMode = SearchableEditorWindow.SearchMode.All;
			}
			int instanceID2 = AssetDatabase.LoadAssetAtPath(this.m_NewAssetFolder, typeof(UnityEngine.Object)).GetInstanceID();
			this.RevealObject(instanceID2);
			this.SetExpanded(instanceID2, true);
			IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
			newHierarchyProperty.Reset();
			float num = 0f;
			if (!this.m_NewAssetFolder.Equals("assets", StringComparison.CurrentCultureIgnoreCase))
			{
				while (newHierarchyProperty.Next(this.m_ExpandedArray))
				{
					string assetPath = AssetDatabase.GetAssetPath(newHierarchyProperty.instanceID);
					num += BaseProjectWindow.m_RowHeight;
					if (string.Equals(assetPath, this.m_NewAssetFolder, StringComparison.CurrentCultureIgnoreCase))
					{
						num = this.FindPositionInsideDir(newHierarchyProperty, num);
						break;
					}
				}
			}
			else
			{
				num = this.FindPositionInsideDir(newHierarchyProperty, num);
			}
			float left = 17f + 16f * (float)this.m_NewAssetIndent + this.IconWidth;
			this.m_NameEditRect = new Rect(left, num, 100f, BaseProjectWindow.m_RowHeight);
			float num2 = num;
			float min = num2 - base.position.height + 20f + BaseProjectWindow.m_RowHeight;
			this.m_ScrollPosition.y = Mathf.Clamp(this.m_ScrollPosition.y, min, num2);
			this.m_EditNameMode = BaseProjectWindow.NameEditMode.PreImportNaming;
			this.m_EditNameInstanceID = instanceID;
			EditorGUI.s_RecycledEditor.content.text = (this.m_NameEditString = this.m_NewAssetName);
			EditorGUI.s_RecycledEditor.SelectAll();
		}
		private float FindPositionInsideDir(IHierarchyProperty property, float yPos)
		{
			this.m_NewAssetIndent = property.depth + 1;
			string text = this.m_NewAssetFolder + "/";
			while (property.Next(this.m_ExpandedArray))
			{
				string assetPath = AssetDatabase.GetAssetPath(property.instanceID);
				string text2 = assetPath + "/";
				if (text2.Length < text.Length || !text2.Substring(0, text.Length).Equals(text, StringComparison.CurrentCultureIgnoreCase))
				{
					this.m_NewAssetSortInstanceID = property.instanceID;
					break;
				}
				if (this.m_NewAssetFolder.Equals(Path.GetDirectoryName(assetPath), StringComparison.CurrentCultureIgnoreCase) && EditorUtility.NaturalCompare(Path.GetFileNameWithoutExtension(assetPath), this.m_NewAssetName) > 0)
				{
					this.m_NewAssetIndent = property.depth;
					this.m_NewAssetSortInstanceID = property.instanceID;
					break;
				}
				yPos += BaseProjectWindow.m_RowHeight;
			}
			return yPos;
		}
		private bool NamingAsset()
		{
			return this.m_EditNameMode == BaseProjectWindow.NameEditMode.Renaming || this.m_EditNameMode == BaseProjectWindow.NameEditMode.PreImportNaming;
		}
		private void EditName()
		{
			if (!this.NamingAsset())
			{
				return;
			}
			Event current = Event.current;
			if (current.type == EventType.KeyDown)
			{
				if (current.keyCode == KeyCode.Escape)
				{
					current.Use();
					this.CancelNameEditing();
				}
				if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
				{
					current.Use();
					this.EndNameEditing();
					GUIUtility.ExitGUI();
				}
			}
			GUI.changed = false;
			GUIUtility.GetControlID(67897456, FocusType.Passive);
			GUI.SetNextControlName("ProjectWindowRenameField");
			EditorGUI.FocusTextInControl("ProjectWindowRenameField");
			GUIStyle textField = EditorStyles.textField;
			EditorStyles.s_Current.m_TextField = "PR TextField";
			this.m_NameEditRect.xMax = GUIClip.visibleRect.width;
			Rect nameEditRect = this.m_NameEditRect;
			this.m_NameEditString = EditorGUI.TextField(nameEditRect, this.m_NameEditString);
			EditorStyles.s_Current.m_TextField = textField;
			if (current.type == EventType.ScrollWheel)
			{
				current.Use();
			}
		}
		private void HierarchyView()
		{
			if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.KeyDown)
			{
				this.m_StillWantsNameEditing = false;
			}
			bool hasFocus = this.m_Parent.hasFocus;
			Hashtable hashtable = new Hashtable();
			int[] instanceIDs = Selection.instanceIDs;
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				int num = instanceIDs[i];
				hashtable.Add(num, null);
			}
			IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
			int num2 = newHierarchyProperty.CountRemaining(this.m_ExpandedArray);
			newHierarchyProperty.Reset();
			Rect viewRect = new Rect(0f, 0f, 1f, (float)num2 * BaseProjectWindow.m_RowHeight);
			if (this.m_EditNameMode == BaseProjectWindow.NameEditMode.PreImportNaming)
			{
				viewRect.height += BaseProjectWindow.m_RowHeight;
			}
			int num3;
			if (base.hasSearchFilter)
			{
				this.m_ScrollPositionFiltered = GUI.BeginScrollView(this.m_ScreenRect, this.m_ScrollPositionFiltered, viewRect);
				num3 = Mathf.RoundToInt(this.m_ScrollPositionFiltered.y) / Mathf.RoundToInt(BaseProjectWindow.m_RowHeight);
			}
			else
			{
				this.m_ScrollPosition = GUI.BeginScrollView(this.m_ScreenRect, this.m_ScrollPosition, viewRect);
				num3 = Mathf.RoundToInt(this.m_ScrollPosition.y) / Mathf.RoundToInt(BaseProjectWindow.m_RowHeight);
			}
			this.EditName();
			if (Event.current.type == EventType.ExecuteCommand || Event.current.type == EventType.ValidateCommand)
			{
				this.ExecuteCommandGUI();
				if (Event.current.type == EventType.ValidateCommand)
				{
					GUI.EndScrollView();
					return;
				}
			}
			this.KeyboardGUI();
			if (Event.current.type == EventType.Layout)
			{
				GUI.EndScrollView();
				return;
			}
			int num4 = this.ControlIDForProperty(null);
			bool flag = false;
			float num5 = (float)num3 * BaseProjectWindow.m_RowHeight;
			float num6 = this.m_ScreenRect.height + num5 + 16f;
			newHierarchyProperty.Skip(num3, this.m_ExpandedArray);
			bool flag2 = false;
			Rect position = new Rect(0f, 0f, 0f, 0f);
			Vector2 mousePosition = Event.current.mousePosition;
			GUIContent gUIContent = new GUIContent();
			Event current = Event.current;
			int activeInstanceID = Selection.activeInstanceID;
			if (!this.NamingAsset())
			{
				this.m_EditNameMode = BaseProjectWindow.NameEditMode.None;
			}
			GUIStyle labelStyle = this.labelStyle;
			Color[] array = (!EditorGUIUtility.isProSkin) ? BaseProjectWindow.s_HierarchyColors : BaseProjectWindow.s_DarkColors;
			while (newHierarchyProperty.Next(this.m_ExpandedArray) && num5 <= num6)
			{
				Rect rect = new Rect(0f, num5, GUIClip.visibleRect.width, BaseProjectWindow.m_RowHeight);
				if (this.m_EditNameMode == BaseProjectWindow.NameEditMode.PreImportNaming && !flag && newHierarchyProperty.instanceID == this.m_NewAssetSortInstanceID)
				{
					flag = true;
					num5 += BaseProjectWindow.m_RowHeight;
					rect = new Rect(0f, num5, GUIClip.visibleRect.width, BaseProjectWindow.m_RowHeight);
					this.DrawPreImportedIcon(num5);
				}
				int instanceID = newHierarchyProperty.instanceID;
				int num7 = this.ControlIDForProperty(newHierarchyProperty);
				float num8 = 17f + 16f * (float)newHierarchyProperty.depth;
				if ((current.type == EventType.MouseUp || current.type == EventType.KeyDown) && activeInstanceID == instanceID && Selection.instanceIDs.Length == 1)
				{
					this.m_NameEditString = newHierarchyProperty.name;
					this.m_NameEditRect = new Rect(rect.x + num8 + this.IconWidth, rect.y, rect.width - num8, rect.height);
					this.m_EditNameInstanceID = instanceID;
					if (this.m_EditNameMode == BaseProjectWindow.NameEditMode.None && newHierarchyProperty.isMainRepresentation)
					{
						this.m_EditNameMode = BaseProjectWindow.NameEditMode.Found;
					}
				}
				if (this.m_EditNameMode == BaseProjectWindow.NameEditMode.Renaming && this.m_EditNameInstanceID == instanceID)
				{
					this.m_NameEditRect = new Rect(rect.x + num8 + this.IconWidth, rect.y, rect.width - num8, rect.height);
				}
				if (current.type == EventType.ContextClick && rect.Contains(current.mousePosition))
				{
					this.m_NameEditRect = new Rect(rect.x + num8 + this.IconWidth, rect.y, rect.width - num8, rect.height);
					this.m_EditNameInstanceID = instanceID;
					this.m_NameEditString = newHierarchyProperty.name;
					if (this.m_EditNameMode == BaseProjectWindow.NameEditMode.None && newHierarchyProperty.isMainRepresentation)
					{
						this.m_EditNameMode = BaseProjectWindow.NameEditMode.Found;
					}
				}
				if (Event.current.type == EventType.Repaint)
				{
					if (this.m_HierarchyType == HierarchyType.GameObjects)
					{
						int colorCode = newHierarchyProperty.colorCode;
						Color textColor = array[colorCode & 3];
						Color textColor2 = array[(colorCode & 3) + 4];
						if (colorCode >= 4)
						{
							textColor.a = (textColor2.a = 0.6f);
						}
						else
						{
							textColor.a = (textColor2.a = 1f);
						}
						labelStyle.normal.textColor = textColor;
						labelStyle.focused.textColor = textColor;
						labelStyle.hover.textColor = textColor;
						labelStyle.active.textColor = textColor;
						labelStyle.onNormal.textColor = textColor2;
						labelStyle.onHover.textColor = textColor2;
						labelStyle.onActive.textColor = textColor2;
						labelStyle.onFocused.textColor = textColor2;
					}
					bool flag3 = this.m_DropData != null && this.m_DropData.dropPreviousControlID == num7;
					bool flag4 = this.m_DropData != null && this.m_DropData.dropOnControlID == num7;
					gUIContent.text = newHierarchyProperty.name;
					gUIContent.image = newHierarchyProperty.icon;
					labelStyle.padding.left = (int)num8;
					bool flag5 = this.m_CurrentDragSelectionIDs.Contains(instanceID);
					bool on = (this.m_CurrentDragSelectionIDs.Count == 0 && hashtable.Contains(instanceID)) || flag5 || (flag5 && (current.control || current.shift) && hashtable.Contains(instanceID)) || (flag5 && hashtable.Contains(instanceID) && hashtable.Contains(this.m_CurrentDragSelectionIDs));
					if (this.m_EditNameMode == BaseProjectWindow.NameEditMode.Renaming && instanceID == this.m_EditNameInstanceID)
					{
						gUIContent.text = string.Empty;
						on = false;
					}
					labelStyle.Draw(rect, gUIContent, flag4, flag4, on, hasFocus);
					if (flag3)
					{
						flag2 = true;
						position = new Rect(rect.x + num8, rect.y - BaseProjectWindow.m_RowHeight, rect.width - num8, rect.height);
					}
				}
				Rect rect2 = rect;
				rect2.x += num8;
				rect2.width -= num8;
				if (this.m_HierarchyType == HierarchyType.Assets)
				{
					ProjectHooks.OnProjectWindowItem(newHierarchyProperty.guid, rect2);
					if (EditorApplication.projectWindowItemOnGUI != null)
					{
						EditorApplication.projectWindowItemOnGUI(newHierarchyProperty.guid, rect2);
					}
				}
				if (this.m_HierarchyType == HierarchyType.GameObjects && EditorApplication.hierarchyWindowItemOnGUI != null)
				{
					EditorApplication.hierarchyWindowItemOnGUI(newHierarchyProperty.instanceID, rect2);
				}
				if (newHierarchyProperty.hasChildren && !base.hasSearchFilter)
				{
					bool flag6 = newHierarchyProperty.IsExpanded(this.m_ExpandedArray);
					GUI.changed = false;
					Rect position2 = new Rect(17f + 16f * (float)newHierarchyProperty.depth - 14f, num5, 14f, BaseProjectWindow.m_RowHeight);
					flag6 = GUI.Toggle(position2, flag6, GUIContent.none, BaseProjectWindow.ms_Styles.foldout);
					if (GUI.changed)
					{
						this.EndNameEditing();
						if (Event.current.alt)
						{
							this.SetExpandedRecurse(instanceID, flag6);
						}
						else
						{
							this.SetExpanded(instanceID, flag6);
						}
					}
				}
				if (current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
				{
					if (Event.current.clickCount == 2)
					{
						AssetDatabase.OpenAsset(instanceID);
						if (this.m_HierarchyType != HierarchyType.Assets && SceneView.lastActiveSceneView != null)
						{
							SceneView.lastActiveSceneView.FrameSelected();
						}
						GUIUtility.ExitGUI();
					}
					else
					{
						this.EndNameEditing();
						this.m_CurrentDragSelectionIDs = this.GetSelection(newHierarchyProperty, true);
						GUIUtility.hotControl = num7;
						GUIUtility.keyboardControl = 0;
						DragAndDropDelay dragAndDropDelay = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), num7);
						dragAndDropDelay.mouseDownPosition = Event.current.mousePosition;
					}
					current.Use();
				}
				else
				{
					if (current.type == EventType.MouseDrag && GUIUtility.hotControl == num7)
					{
						DragAndDropDelay dragAndDropDelay2 = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), num7);
						if (dragAndDropDelay2.CanStartDrag())
						{
							this.StartDrag(newHierarchyProperty);
							GUIUtility.hotControl = 0;
						}
						current.Use();
					}
					else
					{
						if (current.type == EventType.MouseUp && GUIUtility.hotControl == num7)
						{
							if (rect.Contains(current.mousePosition))
							{
								if (newHierarchyProperty.isMainRepresentation && Selection.activeInstanceID == newHierarchyProperty.instanceID && Time.realtimeSinceStartup - this.m_FocusTime > 0.5f && !EditorGUIUtility.HasHolddownKeyModifiers(current))
								{
									this.m_StillWantsNameEditing = true;
									EditorApplication.CallDelayed(new EditorApplication.CallbackFunction(this.BeginMouseEditing), 0.5f);
								}
								else
								{
									this.SelectionClick(newHierarchyProperty);
								}
								GUIUtility.hotControl = 0;
							}
							this.m_CurrentDragSelectionIDs.Clear();
							current.Use();
						}
						else
						{
							if (current.type == EventType.ContextClick && rect.Contains(current.mousePosition))
							{
								current.Use();
								if (this.m_HierarchyType == HierarchyType.GameObjects)
								{
									this.SelectionClickContextMenu(newHierarchyProperty);
									GenericMenu genericMenu = new GenericMenu();
									genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupCopy"), false, new GenericMenu.MenuFunction(this.CopyGO));
									genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupPaste"), false, new GenericMenu.MenuFunction(this.PasteGO));
									genericMenu.AddSeparator(string.Empty);
									if (this.m_EditNameMode == BaseProjectWindow.NameEditMode.Found && !base.hasSearchFilter)
									{
										genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupRename"), false, new GenericMenu.MenuFunction2(this.RenameGO), newHierarchyProperty.pptrValue);
									}
									else
									{
										genericMenu.AddDisabledItem(EditorGUIUtility.TextContent("HierarchyPopupRename"));
									}
									genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupDuplicate"), false, new GenericMenu.MenuFunction(this.DuplicateGO));
									genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupDelete"), false, new GenericMenu.MenuFunction(this.DeleteGO));
									genericMenu.AddSeparator(string.Empty);
									UnityEngine.Object prefab = PrefabUtility.GetPrefabParent(newHierarchyProperty.pptrValue);
									if (prefab != null)
									{
										genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupSelectPrefab"), false, delegate
										{
											Selection.activeObject = prefab;
											EditorGUIUtility.PingObject(prefab.GetInstanceID());
										});
									}
									else
									{
										genericMenu.AddDisabledItem(EditorGUIUtility.TextContent("HierarchyPopupSelectPrefab"));
									}
									genericMenu.ShowAsContext();
								}
							}
							else
							{
								if (current.type == EventType.DragUpdated || current.type == EventType.DragPerform)
								{
									Rect rect3 = rect;
									rect3.yMin -= BaseProjectWindow.m_DropNextToAreaHeight * 2f;
									if (rect3.Contains(mousePosition))
									{
										if (mousePosition.y - rect.y < BaseProjectWindow.m_DropNextToAreaHeight * 0.5f)
										{
											this.DragElement(newHierarchyProperty, false);
										}
										else
										{
											this.DragElement(newHierarchyProperty, true);
										}
										GUIUtility.hotControl = 0;
									}
								}
							}
						}
					}
				}
				num5 += BaseProjectWindow.m_RowHeight;
			}
			if (flag2)
			{
				GUIStyle insertion = BaseProjectWindow.ms_Styles.insertion;
				if (current.type == EventType.Repaint)
				{
					insertion.Draw(position, false, false, false, false);
				}
			}
			if (this.m_EditNameMode == BaseProjectWindow.NameEditMode.PreImportNaming && this.m_NewAssetSortInstanceID == 0)
			{
				this.DrawPreImportedIcon(num5 + 16f);
			}
			this.HandlePing();
			GUI.EndScrollView();
			EventType type = current.type;
			switch (type)
			{
			case EventType.DragUpdated:
				if (this.m_SearchFilter == string.Empty)
				{
					this.DragElement(null, true);
				}
				else
				{
					if (this.m_DropData == null)
					{
						this.m_DropData = new BaseProjectWindow.DropData();
					}
					this.m_DropData.dropOnControlID = 0;
					this.m_DropData.dropPreviousControlID = 0;
				}
				return;
			case EventType.DragPerform:
				this.m_CurrentDragSelectionIDs.Clear();
				this.DragElement(null, true);
				return;
			case EventType.Ignore:
			case EventType.Used:
			case EventType.ValidateCommand:
			case EventType.ExecuteCommand:
				IL_CB2:
				switch (type)
				{
				case EventType.MouseDown:
					if (current.button == 0 && this.m_ScreenRect.Contains(current.mousePosition))
					{
						GUIUtility.hotControl = num4;
						Selection.activeObject = null;
						this.EndNameEditing();
						current.Use();
					}
					return;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == num4)
					{
						GUIUtility.hotControl = 0;
						current.Use();
					}
					return;
				case EventType.MouseMove:
				case EventType.MouseDrag:
					return;
				case EventType.KeyDown:
					if (this.m_EditNameMode == BaseProjectWindow.NameEditMode.Found && (((current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter) && Application.platform == RuntimePlatform.OSXEditor) || (current.keyCode == KeyCode.F2 && Application.platform == RuntimePlatform.WindowsEditor)))
					{
						this.BeginNameEditing(Selection.activeInstanceID);
						current.Use();
					}
					return;
				default:
					return;
				}
				break;
			case EventType.DragExited:
				this.m_CurrentDragSelectionIDs.Clear();
				this.DragCleanup(true);
				return;
			case EventType.ContextClick:
				if (this.m_HierarchyType == HierarchyType.Assets && this.m_ScreenRect.Contains(current.mousePosition))
				{
					EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/", null);
					current.Use();
				}
				return;
			}
			goto IL_CB2;
		}
		private void HandlePing()
		{
			this.m_Ping.HandlePing();
			if (this.m_Ping.isPinging)
			{
				base.Repaint();
			}
		}
		private void DrawPreImportedIcon(float offset)
		{
			if (Event.current.type == EventType.Repaint && this.m_NewAssetIcon)
			{
				BaseProjectWindow.ms_Styles.label.padding.left = 0;
				BaseProjectWindow.ms_Styles.label.Draw(new Rect((float)(this.m_NewAssetIndent * 16) + 17f, offset - 16f, 16f, 16f), EditorGUIUtility.TempContent(this.m_NewAssetIcon), 0);
			}
		}
		private void CopyGO()
		{
			Unsupported.CopyGameObjectsToPasteboard();
		}
		private void PasteGO()
		{
			Unsupported.PasteGameObjectsFromPasteboard();
		}
		private void DuplicateGO()
		{
			Unsupported.DuplicateGameObjectsUsingPasteboard();
		}
		private void RenameGO(object obj)
		{
			GameObject activeObject = (GameObject)obj;
			Selection.activeObject = activeObject;
			this.BeginNameEditing(Selection.activeInstanceID);
			base.Repaint();
		}
		private void DeleteGO()
		{
			Unsupported.DeleteGameObjectSelection();
		}
		private void BeginMouseEditing()
		{
			if (this.m_StillWantsNameEditing)
			{
				this.BeginNameEditing(Selection.activeInstanceID);
			}
			base.Repaint();
		}
		private void StartDrag(IHierarchyProperty property)
		{
			DragAndDrop.PrepareStartDrag();
			DragAndDrop.objectReferences = this.GetDragAndDropObjects(property.pptrValue);
			DragAndDrop.paths = this.GetDragAndDropPaths(property.instanceID);
			if (DragAndDrop.objectReferences.Length > 1)
			{
				DragAndDrop.StartDrag("<Multiple>");
			}
			else
			{
				DragAndDrop.StartDrag(ObjectNames.GetDragAndDropTitle(property.pptrValue));
			}
		}
		private void DragCleanup(bool revertExpanded)
		{
			if (this.m_DropData != null)
			{
				if (this.m_DropData.expandedArrayBeforeDrag != null && revertExpanded)
				{
					this.m_ExpandedArray = this.m_DropData.expandedArrayBeforeDrag;
				}
				this.m_DropData = null;
				base.Repaint();
			}
		}
		private void DragElement(IHierarchyProperty property, bool dropOnTopOfElement)
		{
			if (Event.current.type == EventType.DragPerform)
			{
				IHierarchyProperty hierarchyProperty = property;
				DragAndDropVisualMode dragAndDropVisualMode = DragAndDropVisualMode.None;
				if (dropOnTopOfElement)
				{
					dragAndDropVisualMode = this.DoDrag(hierarchyProperty, true);
				}
				if (dragAndDropVisualMode == DragAndDropVisualMode.None && property != null)
				{
					hierarchyProperty = this.GetParentProperty(hierarchyProperty);
					dragAndDropVisualMode = this.DoDrag(hierarchyProperty, true);
				}
				if (dragAndDropVisualMode != DragAndDropVisualMode.None)
				{
					DragAndDrop.AcceptDrag();
					this.DragCleanup(false);
					ArrayList arrayList = new ArrayList();
					UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
					for (int i = 0; i < objectReferences.Length; i++)
					{
						UnityEngine.Object @object = objectReferences[i];
						IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
						if (newHierarchyProperty != null && @object != null && newHierarchyProperty.Find(@object.GetInstanceID(), null))
						{
							IHierarchyProperty parentProperty = this.GetParentProperty(newHierarchyProperty);
							if (parentProperty != null && (hierarchyProperty == null || parentProperty.pptrValue == hierarchyProperty.pptrValue))
							{
								arrayList.Add(@object);
							}
						}
					}
					if (arrayList.Count > 0)
					{
						Selection.objects = (UnityEngine.Object[])arrayList.ToArray(typeof(UnityEngine.Object));
						this.RevealObjects(Selection.instanceIDs);
						this.FrameObject(Selection.activeInstanceID);
					}
					GUIUtility.ExitGUI();
				}
				else
				{
					this.DragCleanup(true);
				}
			}
			else
			{
				if (this.m_DropData == null)
				{
					this.m_DropData = new BaseProjectWindow.DropData();
				}
				this.m_DropData.dropOnControlID = 0;
				this.m_DropData.dropPreviousControlID = 0;
				int num = this.ControlIDForProperty(property);
				if (num != this.m_DropData.lastControlID)
				{
					this.m_DropData.lastControlID = this.ControlIDForProperty(property);
					this.m_DropData.expandItemBeginTimer = (double)Time.realtimeSinceStartup;
				}
				bool flag = (double)Time.realtimeSinceStartup - this.m_DropData.expandItemBeginTimer > 0.7;
				if (property != null && property.hasChildren && flag && !property.IsExpanded(this.m_ExpandedArray))
				{
					if (this.m_DropData.expandedArrayBeforeDrag == null)
					{
						this.m_DropData.expandedArrayBeforeDrag = this.m_ExpandedArray;
					}
					this.SetExpanded(property.instanceID, true);
				}
				DragAndDropVisualMode dragAndDropVisualMode2 = DragAndDropVisualMode.None;
				if (dropOnTopOfElement)
				{
					dragAndDropVisualMode2 = this.DoDrag(property, false);
				}
				if (dragAndDropVisualMode2 != DragAndDropVisualMode.None)
				{
					this.m_DropData.dropOnControlID = num;
					DragAndDrop.visualMode = dragAndDropVisualMode2;
				}
				else
				{
					if (property != null && this.m_SearchFilter == string.Empty)
					{
						IHierarchyProperty parentProperty2 = this.GetParentProperty(property);
						dragAndDropVisualMode2 = this.DoDrag(parentProperty2, false);
						if (dragAndDropVisualMode2 != DragAndDropVisualMode.None)
						{
							this.m_DropData.dropPreviousControlID = num;
							this.m_DropData.dropOnControlID = this.ControlIDForProperty(parentProperty2);
							DragAndDrop.visualMode = dragAndDropVisualMode2;
						}
					}
				}
				base.Repaint();
			}
			Event.current.Use();
		}
		private DragAndDropVisualMode DoDrag(IHierarchyProperty property, bool perform)
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(this.m_HierarchyType);
			if (property == null || !hierarchyProperty.Find(property.instanceID, null))
			{
				hierarchyProperty = null;
			}
			if (this.HandleSpriteDragIntoHierarchy(property, perform))
			{
				return DragAndDropVisualMode.Link;
			}
			if (this.m_HierarchyType == HierarchyType.Assets)
			{
				return InternalEditorUtility.ProjectWindowDrag(hierarchyProperty, perform);
			}
			return InternalEditorUtility.HierarchyWindowDrag(hierarchyProperty, perform, InternalEditorUtility.HierarchyDropMode.kHierarchyDragNormal);
		}
		private bool HandleSpriteDragIntoHierarchy(IHierarchyProperty property, bool perform)
		{
			Sprite[] spritesFromDraggedObjects = SpriteUtility.GetSpritesFromDraggedObjects();
			if (spritesFromDraggedObjects.Length == 1)
			{
				return SpriteUtility.HandleSingleSpriteDragIntoHierarchy(property, spritesFromDraggedObjects[0], perform);
			}
			return spritesFromDraggedObjects.Length > 1 && SpriteUtility.HandleMultipleSpritesDragIntoHierarchy(property, spritesFromDraggedObjects, perform);
		}
		private IHierarchyProperty GetPreviousParentProperty(IHierarchyProperty property)
		{
			IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
			if (newHierarchyProperty.Find(property.instanceID, this.m_ExpandedArray) && newHierarchyProperty.Previous(this.m_ExpandedArray))
			{
				return this.GetParentProperty(newHierarchyProperty);
			}
			return null;
		}
		private IHierarchyProperty GetParentProperty(IHierarchyProperty property)
		{
			IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
			if (newHierarchyProperty.Find(property.instanceID, this.m_ExpandedArray) && newHierarchyProperty.Parent())
			{
				int instanceID = newHierarchyProperty.instanceID;
				newHierarchyProperty.Reset();
				if (newHierarchyProperty.Find(instanceID, this.m_ExpandedArray))
				{
					return newHierarchyProperty;
				}
			}
			return null;
		}
		private int ControlIDForProperty(IHierarchyProperty property)
		{
			if (property != null)
			{
				return property.instanceID + 10000000;
			}
			return -1;
		}
		private string[] GetDragAndDropPaths(int dragged)
		{
			string assetPath = AssetDatabase.GetAssetPath(dragged);
			if (this.m_HierarchyType != HierarchyType.Assets)
			{
				return new string[0];
			}
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(BaseProjectWindow.GetMainSelectedPaths());
			if (arrayList.Contains(assetPath))
			{
				return arrayList.ToArray(typeof(string)) as string[];
			}
			return new string[]
			{
				assetPath
			};
		}
		private UnityEngine.Object[] GetDragAndDropObjects(UnityEngine.Object dragged)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(this.GetSelectedReferences());
			if (arrayList.Contains(dragged))
			{
				return arrayList.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[];
			}
			return new UnityEngine.Object[]
			{
				dragged
			};
		}
		private UnityEngine.Object[] GetSelectedReferences()
		{
			ArrayList arrayList = new ArrayList();
			UnityEngine.Object[] objects = Selection.objects;
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object @object = objects[i];
				if (AssetDatabase.Contains(@object) != (this.m_HierarchyType == HierarchyType.GameObjects))
				{
					arrayList.Add(@object);
				}
			}
			return arrayList.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[];
		}
		private static string[] GetMainSelectedPaths()
		{
			ArrayList arrayList = new ArrayList();
			int[] instanceIDs = Selection.instanceIDs;
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				int instanceID = instanceIDs[i];
				if (AssetDatabase.IsMainAsset(instanceID))
				{
					string assetPath = AssetDatabase.GetAssetPath(instanceID);
					arrayList.Add(assetPath);
				}
			}
			return arrayList.ToArray(typeof(string)) as string[];
		}
		private void ExecuteCommandGUIHierarchy()
		{
			bool flag = Event.current.type == EventType.ExecuteCommand;
			if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
			{
				if (flag)
				{
					Unsupported.DeleteGameObjectSelection();
				}
				Event.current.Use();
				GUIUtility.ExitGUI();
			}
			else
			{
				if (Event.current.commandName == "Duplicate")
				{
					if (flag)
					{
						Unsupported.DuplicateGameObjectsUsingPasteboard();
					}
					Event.current.Use();
					GUIUtility.ExitGUI();
				}
				else
				{
					if (Event.current.commandName == "Copy")
					{
						if (flag)
						{
							Unsupported.CopyGameObjectsToPasteboard();
						}
						Event.current.Use();
						GUIUtility.ExitGUI();
					}
					else
					{
						if (Event.current.commandName == "Paste")
						{
							if (flag)
							{
								Unsupported.PasteGameObjectsFromPasteboard();
							}
							Event.current.Use();
							GUIUtility.ExitGUI();
						}
						else
						{
							if (Event.current.commandName == "SelectAll")
							{
								if (flag)
								{
									this.SelectAll();
								}
								Event.current.Use();
								GUIUtility.ExitGUI();
							}
						}
					}
				}
			}
		}
		internal static void DuplicateSelectedAssets()
		{
			AssetDatabase.Refresh();
			UnityEngine.Object[] objects = Selection.objects;
			bool flag = true;
			UnityEngine.Object[] array = objects;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object @object = array[i];
				AnimationClip animationClip = @object as AnimationClip;
				if (animationClip == null || (animationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None || !AssetDatabase.Contains(animationClip))
				{
					flag = false;
				}
			}
			ArrayList arrayList = new ArrayList();
			bool flag2 = false;
			if (flag)
			{
				UnityEngine.Object[] array2 = objects;
				for (int j = 0; j < array2.Length; j++)
				{
					UnityEngine.Object object2 = array2[j];
					AnimationClip animationClip2 = object2 as AnimationClip;
					if (animationClip2 != null && (animationClip2.hideFlags & HideFlags.NotEditable) != HideFlags.None)
					{
						string path = AssetDatabase.GetAssetPath(object2);
						path = Path.Combine(Path.GetDirectoryName(path), animationClip2.name) + ".anim";
						string text = AssetDatabase.GenerateUniqueAssetPath(path);
						AnimationClip animationClip3 = new AnimationClip();
						EditorUtility.CopySerialized(animationClip2, animationClip3);
						AssetDatabase.CreateAsset(animationClip3, text);
						arrayList.Add(text);
					}
				}
			}
			else
			{
				UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
				UnityEngine.Object[] array3 = filtered;
				for (int k = 0; k < array3.Length; k++)
				{
					UnityEngine.Object assetObject = array3[k];
					string assetPath = AssetDatabase.GetAssetPath(assetObject);
					string text2 = AssetDatabase.GenerateUniqueAssetPath(assetPath);
					if (text2.Length != 0)
					{
						flag2 |= !AssetDatabase.CopyAsset(assetPath, text2);
					}
					else
					{
						flag2 |= true;
					}
					if (!flag2)
					{
						arrayList.Add(text2);
					}
				}
			}
			AssetDatabase.Refresh();
			UnityEngine.Object[] array4 = new UnityEngine.Object[arrayList.Count];
			for (int l = 0; l < arrayList.Count; l++)
			{
				array4[l] = AssetDatabase.LoadMainAssetAtPath(arrayList[l] as string);
			}
			Selection.objects = array4;
		}
		private void ExecuteCommandGUIProject()
		{
			bool flag = Event.current.type == EventType.ExecuteCommand;
			if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
			{
				Event.current.Use();
				if (flag)
				{
					bool askIfSure = Event.current.commandName == "SoftDelete";
					BaseProjectWindow.DeleteSelectedAssets(askIfSure);
				}
				GUIUtility.ExitGUI();
			}
			else
			{
				if (Event.current.commandName == "Duplicate")
				{
					if (flag)
					{
						Event.current.Use();
						BaseProjectWindow.DuplicateSelectedAssets();
						GUIUtility.ExitGUI();
					}
					else
					{
						UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
						if (filtered.Length != 0)
						{
							Event.current.Use();
						}
					}
				}
				else
				{
					if (Event.current.commandName == "FocusProjectWindow")
					{
						if (Event.current.type == EventType.ExecuteCommand)
						{
							this.FrameObject(Selection.activeInstanceID);
							Event.current.Use();
							base.Focus();
							GUIUtility.ExitGUI();
						}
						else
						{
							Event.current.Use();
						}
					}
					else
					{
						if (Event.current.commandName == "SelectAll")
						{
							if (Event.current.type == EventType.ExecuteCommand)
							{
								this.SelectAll();
							}
							Event.current.Use();
						}
					}
				}
			}
		}
		internal static void DeleteSelectedAssets(bool askIfSure)
		{
			string[] mainSelectedPaths = BaseProjectWindow.GetMainSelectedPaths();
			if (mainSelectedPaths.Length == 0)
			{
				return;
			}
			if (askIfSure)
			{
				string text = "Delete selected asset";
				if (mainSelectedPaths.Length > 1)
				{
					text += "s";
				}
				text += "?";
				if (!EditorUtility.DisplayDialog(text, "You cannot undo this action.", "Delete", "Cancel"))
				{
					return;
				}
			}
			AssetDatabase.StartAssetEditing();
			string[] array = mainSelectedPaths;
			for (int i = 0; i < array.Length; i++)
			{
				string path = array[i];
				AssetDatabase.MoveAssetToTrash(path);
			}
			AssetDatabase.StopAssetEditing();
		}
		private void SelectAll()
		{
			IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
			List<int> list = new List<int>();
			while (newHierarchyProperty.Next(this.m_ExpandedArray))
			{
				list.Add(newHierarchyProperty.instanceID);
			}
			Selection.instanceIDs = list.ToArray();
		}
		internal void PingTargetObject(int targetInstanceID)
		{
			if (targetInstanceID == 0)
			{
				return;
			}
			if (BaseProjectWindow.ms_Styles == null)
			{
				BaseProjectWindow.ms_Styles = new BaseProjectWindow.Styles();
			}
			if (this.FrameObject(targetInstanceID))
			{
				IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
				int num = newHierarchyProperty.CountRemaining(this.m_ExpandedArray);
				IHierarchyProperty newHierarchyProperty2 = this.GetNewHierarchyProperty();
				if (newHierarchyProperty2.Find(targetInstanceID, this.m_ExpandedArray))
				{
					int row = newHierarchyProperty2.row;
					float top = BaseProjectWindow.m_RowHeight * (float)row;
					this.m_Ping.m_TimeStart = Time.realtimeSinceStartup;
					this.m_Ping.m_PingStyle = BaseProjectWindow.ms_Styles.ping;
					float num2 = ((float)num * BaseProjectWindow.m_RowHeight <= this.m_ScreenRect.height) ? 0f : -16f;
					this.m_Ping.m_AvailableWidth = base.position.width + num2;
					GUIContent pingContent = new GUIContent(newHierarchyProperty2.name, newHierarchyProperty2.icon);
					Vector2 vector = BaseProjectWindow.ms_Styles.ping.CalcSize(pingContent);
					vector.y += 3f;
					this.m_Ping.m_ContentRect = new Rect(17f + 16f * (float)newHierarchyProperty2.depth, top, vector.x, vector.y);
					this.m_Ping.m_ContentDraw = delegate(Rect r)
					{
						this.DrawPingContent(r, pingContent);
					};
					base.Repaint();
					return;
				}
			}
			targetInstanceID = InternalEditorUtility.GetGameObjectInstanceIDFromComponent(targetInstanceID);
			if (targetInstanceID != 0)
			{
				this.PingTargetObject(targetInstanceID);
			}
		}
		private void DrawPingContent(Rect rect, GUIContent content)
		{
			GUIStyle labelStyle = this.labelStyle;
			labelStyle.padding.left = 0;
			labelStyle.Draw(rect, content, false, false, false, false);
		}
		private void ExecuteCommandGUI()
		{
			if (this.m_HierarchyType == HierarchyType.Assets)
			{
				this.ExecuteCommandGUIProject();
			}
			else
			{
				this.ExecuteCommandGUIHierarchy();
			}
			if (Event.current.commandName == "FrameSelected")
			{
				if (Event.current.type == EventType.ExecuteCommand)
				{
					this.FrameObject(Selection.activeInstanceID);
				}
				Event.current.Use();
				GUIUtility.ExitGUI();
			}
			else
			{
				if (Event.current.commandName == "Find")
				{
					if (Event.current.type == EventType.ExecuteCommand)
					{
						base.FocusSearchField();
					}
					Event.current.Use();
				}
			}
		}
		public void SelectPrevious()
		{
			IHierarchyProperty firstSelected = this.GetFirstSelected();
			if (firstSelected != null)
			{
				if (firstSelected.Previous(this.m_ExpandedArray))
				{
					this.FrameObject(firstSelected.instanceID);
					this.SelectionClick(firstSelected);
				}
			}
			else
			{
				if (this.GetLast() != null)
				{
					Selection.activeInstanceID = this.GetLast().instanceID;
					this.FrameObject(Selection.activeInstanceID);
				}
			}
		}
		public void SelectNext()
		{
			IHierarchyProperty lastSelected = this.GetLastSelected();
			if (lastSelected != null)
			{
				if (lastSelected.Next(this.m_ExpandedArray))
				{
					this.SelectionClick(lastSelected);
					this.FrameObject(lastSelected.instanceID);
				}
			}
			else
			{
				if (this.GetFirst() != null)
				{
					Selection.activeInstanceID = this.GetFirst().instanceID;
					this.FrameObject(Selection.activeInstanceID);
				}
			}
		}
		private void KeyboardGUI()
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
			if (Event.current.GetTypeForControl(controlID) != EventType.KeyDown)
			{
				return;
			}
			KeyCode keyCode = Event.current.keyCode;
			switch (keyCode)
			{
			case KeyCode.KeypadEnter:
				goto IL_353;
			case KeyCode.KeypadEquals:
			case KeyCode.Insert:
				IL_5E:
				if (keyCode != KeyCode.Return)
				{
					return;
				}
				goto IL_353;
			case KeyCode.UpArrow:
				Event.current.Use();
				this.SelectPrevious();
				return;
			case KeyCode.DownArrow:
				Event.current.Use();
				if (Application.platform == RuntimePlatform.OSXEditor && Event.current.command)
				{
					this.OpenAssetSelection();
					GUIUtility.ExitGUI();
				}
				else
				{
					this.SelectNext();
				}
				return;
			case KeyCode.RightArrow:
			{
				int[] instanceIDs = Selection.instanceIDs;
				for (int i = 0; i < instanceIDs.Length; i++)
				{
					this.SetExpanded(instanceIDs[i], true);
				}
				Event.current.Use();
				return;
			}
			case KeyCode.LeftArrow:
			{
				int[] instanceIDs2 = Selection.instanceIDs;
				for (int j = 0; j < instanceIDs2.Length; j++)
				{
					this.SetExpanded(instanceIDs2[j], false);
				}
				Event.current.Use();
				return;
			}
			case KeyCode.Home:
				if (this.GetFirst() != null)
				{
					Selection.activeObject = this.GetFirst().pptrValue;
					this.FrameObject(Selection.activeInstanceID);
				}
				return;
			case KeyCode.End:
				if (this.GetLast() != null)
				{
					Selection.activeObject = this.GetLast().pptrValue;
					this.FrameObject(Selection.activeInstanceID);
				}
				return;
			case KeyCode.PageUp:
				Event.current.Use();
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					this.m_ScrollPosition.y = this.m_ScrollPosition.y - this.m_ScreenRect.height;
					if (this.m_ScrollPosition.y < 0f)
					{
						this.m_ScrollPosition.y = 0f;
					}
				}
				else
				{
					IHierarchyProperty hierarchyProperty = this.GetFirstSelected();
					if (hierarchyProperty != null)
					{
						int num = 0;
						while ((float)num < this.m_ScreenRect.height / BaseProjectWindow.m_RowHeight)
						{
							if (!hierarchyProperty.Previous(this.m_ExpandedArray))
							{
								hierarchyProperty = this.GetFirst();
								break;
							}
							num++;
						}
						int instanceID = hierarchyProperty.instanceID;
						this.SelectionClick(hierarchyProperty);
						this.FrameObject(instanceID);
					}
					else
					{
						if (this.GetFirst() != null)
						{
							Selection.activeObject = this.GetFirst().pptrValue;
							this.FrameObject(Selection.activeInstanceID);
						}
					}
				}
				return;
			case KeyCode.PageDown:
				Event.current.Use();
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					this.m_ScrollPosition.y = this.m_ScrollPosition.y + this.m_ScreenRect.height;
				}
				else
				{
					IHierarchyProperty hierarchyProperty2 = this.GetLastSelected();
					if (hierarchyProperty2 != null)
					{
						int num2 = 0;
						while ((float)num2 < this.m_ScreenRect.height / BaseProjectWindow.m_RowHeight)
						{
							if (!hierarchyProperty2.Next(this.m_ExpandedArray))
							{
								hierarchyProperty2 = this.GetLast();
								break;
							}
							num2++;
						}
						int instanceID2 = hierarchyProperty2.instanceID;
						this.SelectionClick(hierarchyProperty2);
						this.FrameObject(instanceID2);
					}
					else
					{
						if (this.GetLast() != null)
						{
							Selection.activeObject = this.GetLast().pptrValue;
							this.FrameObject(Selection.activeInstanceID);
						}
					}
				}
				return;
			}
			goto IL_5E;
			IL_353:
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				this.OpenAssetSelection();
				GUIUtility.ExitGUI();
			}
		}
		private bool RevealObject(int targetInstanceID)
		{
			return this.RevealObject(targetInstanceID, true);
		}
		private bool RevealObject(int targetInstanceID, bool allowClearSearchFilter)
		{
			IHierarchyProperty hierarchyProperty = this.GetNewHierarchyProperty();
			if (hierarchyProperty.Find(targetInstanceID, null))
			{
				while (hierarchyProperty.Parent())
				{
					this.SetExpanded(hierarchyProperty.instanceID, true);
				}
				return true;
			}
			if (allowClearSearchFilter)
			{
				hierarchyProperty = new HierarchyProperty(this.m_HierarchyType);
				if (hierarchyProperty.Find(targetInstanceID, null))
				{
					base.ClearSearchFilter();
					return this.RevealObject(targetInstanceID, true);
				}
			}
			return false;
		}
		private void RevealObjects(int[] targetInstanceIDs)
		{
			IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
			int[] array = newHierarchyProperty.FindAllAncestors(targetInstanceIDs);
			for (int i = 0; i < array.Length; i++)
			{
				this.SetExpanded(array[i], true);
			}
		}
		internal bool FrameObject(int targetInstanceID)
		{
			return this.FrameObject(targetInstanceID, true);
		}
		internal bool FrameObject(int targetInstanceID, bool allowClearSearchFilter)
		{
			IHierarchyProperty newHierarchyProperty = this.GetNewHierarchyProperty();
			if (this.m_NeedsRelayout)
			{
				this.m_FrameAfterRelayout = targetInstanceID;
				return newHierarchyProperty.Find(targetInstanceID, this.m_ExpandedArray);
			}
			this.RevealObject(targetInstanceID, allowClearSearchFilter);
			newHierarchyProperty = this.GetNewHierarchyProperty();
			if (newHierarchyProperty.Find(targetInstanceID, this.m_ExpandedArray))
			{
				int row = newHierarchyProperty.row;
				float num = BaseProjectWindow.m_RowHeight * (float)row;
				float min = num - this.m_ScreenRect.height + BaseProjectWindow.m_RowHeight;
				if (!base.hasSearchFilter)
				{
					this.m_ScrollPosition.y = Mathf.Clamp(this.m_ScrollPosition.y, min, num);
				}
				else
				{
					this.m_ScrollPositionFiltered.y = Mathf.Clamp(this.m_ScrollPositionFiltered.y, min, num);
				}
				base.Repaint();
				return true;
			}
			return false;
		}
		private void SelectionClickContextMenu(IHierarchyProperty property)
		{
			if (!Selection.Contains(property.instanceID))
			{
				Selection.activeInstanceID = property.instanceID;
			}
		}
		private List<int> GetSelection(IHierarchyProperty property, bool mouseDown)
		{
			List<int> list = new List<int>();
			if (EditorGUI.actionKey)
			{
				list.AddRange(Selection.instanceIDs);
				if (list.Contains(property.instanceID))
				{
					list.Remove(property.instanceID);
				}
				else
				{
					list.Add(property.instanceID);
				}
				return list;
			}
			if (!Event.current.shift)
			{
				if (mouseDown)
				{
					list.AddRange(Selection.instanceIDs);
					if (list.Contains(property.instanceID))
					{
						return list;
					}
					list.Clear();
				}
				list.Add(property.instanceID);
				return list;
			}
			IHierarchyProperty firstSelected = this.GetFirstSelected();
			IHierarchyProperty lastSelected = this.GetLastSelected();
			if (firstSelected == null || !firstSelected.isValid)
			{
				list.Add(property.instanceID);
				return list;
			}
			IHierarchyProperty hierarchyProperty;
			IHierarchyProperty hierarchyProperty2;
			if (property.row > lastSelected.row)
			{
				hierarchyProperty = firstSelected;
				hierarchyProperty2 = property;
			}
			else
			{
				hierarchyProperty = property;
				hierarchyProperty2 = lastSelected;
			}
			list.Add(hierarchyProperty.instanceID);
			while (hierarchyProperty.Next(this.m_ExpandedArray))
			{
				list.Add(hierarchyProperty.instanceID);
				if (hierarchyProperty.instanceID == hierarchyProperty2.instanceID)
				{
					break;
				}
			}
			return list;
		}
		private void SelectionClick(IHierarchyProperty property)
		{
			List<int> selection = this.GetSelection(property, false);
			if (selection.Count == 1)
			{
				Selection.activeInstanceID = selection[0];
			}
			else
			{
				Selection.instanceIDs = selection.ToArray();
			}
			this.m_ProjectViewInternalSelectionChange = true;
			if (base.hasSearchFilter)
			{
				this.m_DidSelectSearchResult = true;
				this.m_NeedsRelayout = true;
			}
		}
		private void CreateAssetPopup()
		{
			GUIContent content = new GUIContent("Create");
			Rect rect = GUILayoutUtility.GetRect(content, EditorStyles.toolbarDropDown, null);
			if (Event.current.type == EventType.Repaint)
			{
				EditorStyles.toolbarDropDown.Draw(rect, content, false, false, false, false);
			}
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
			{
				GUIUtility.hotControl = 0;
				EditorUtility.DisplayPopupMenu(rect, "Assets/Create", null);
				Event.current.Use();
			}
		}
		private void CreateGameObjectPopup()
		{
			GUIContent content = new GUIContent("Create");
			Rect rect = GUILayoutUtility.GetRect(content, EditorStyles.toolbarDropDown, null);
			if (Event.current.type == EventType.Repaint)
			{
				EditorStyles.toolbarDropDown.Draw(rect, content, false, false, false, false);
			}
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
			{
				GUIUtility.hotControl = 0;
				EditorUtility.DisplayPopupMenu(rect, "GameObject/Create Other", null);
				Event.current.Use();
			}
		}
		private void Awake()
		{
			if (this.m_HierarchyType == HierarchyType.Assets)
			{
				this.m_ExpandedArray = InternalEditorUtility.expandedProjectWindowItems;
			}
		}
		public new void Show()
		{
			this.Awake();
			base.Show();
		}
	}
}
