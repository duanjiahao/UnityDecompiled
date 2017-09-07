using System;
using System.Collections.Generic;
using UnityEditor.Collaboration;
using UnityEditor.Web;
using UnityEngine;

namespace UnityEditor
{
	internal class SoftlockViewController
	{
		private class Cache
		{
			private List<WeakReference> m_EditorReferences = new List<WeakReference>();

			private List<WeakReference> m_CachedWeakReferences = new List<WeakReference>();

			private static Dictionary<int, string> s_CachedStringCount = new Dictionary<int, string>();

			private Dictionary<string, string> m_AssetGUIDToTooltip = new Dictionary<string, string>();

			private Dictionary<string, Dictionary<int, string>> m_NamesListToEllipsedNames = new Dictionary<string, Dictionary<int, string>>();

			public void InvalidateAssetGUIDs(string[] assetGUIDs)
			{
				for (int i = 0; i < assetGUIDs.Length; i++)
				{
					string key = assetGUIDs[i];
					this.m_AssetGUIDToTooltip.Remove(key);
				}
			}

			public bool TryGetEllipsedNames(string allNames, int characterLength, out string ellipsedNames)
			{
				Dictionary<int, string> dictionary;
				bool result;
				if (this.m_NamesListToEllipsedNames.TryGetValue(allNames, out dictionary))
				{
					result = dictionary.TryGetValue(characterLength, out ellipsedNames);
				}
				else
				{
					ellipsedNames = "";
					result = false;
				}
				return result;
			}

			public void StoreEllipsedNames(string allNames, string ellipsedNames, int characterLength)
			{
				Dictionary<int, string> dictionary;
				if (!this.m_NamesListToEllipsedNames.TryGetValue(allNames, out dictionary))
				{
					dictionary = new Dictionary<int, string>();
				}
				dictionary[characterLength] = ellipsedNames;
				this.m_NamesListToEllipsedNames[allNames] = dictionary;
			}

			public bool TryGetTooltipForGUID(string assetGUID, out string tooltipText)
			{
				return this.m_AssetGUIDToTooltip.TryGetValue(assetGUID, out tooltipText);
			}

			public void StoreTooltipForGUID(string assetGUID, string tooltipText)
			{
				this.m_AssetGUIDToTooltip[assetGUID] = tooltipText;
			}

			public bool TryGetDisplayCount(int count, out string displayText)
			{
				return SoftlockViewController.Cache.s_CachedStringCount.TryGetValue(count, out displayText);
			}

			public void StoreDisplayCount(int count, string displayText)
			{
				SoftlockViewController.Cache.s_CachedStringCount.Add(count, displayText);
			}

			public List<Editor> GetEditors()
			{
				List<Editor> list = new List<Editor>();
				for (int i = 0; i < this.m_EditorReferences.Count; i++)
				{
					WeakReference weakReference = this.m_EditorReferences[i];
					Editor editor = weakReference.Target as Editor;
					if (editor == null)
					{
						this.m_EditorReferences.RemoveAt(i);
						this.m_CachedWeakReferences.Add(weakReference);
						i--;
					}
					else
					{
						list.Add(editor);
					}
				}
				return list;
			}

			public void StoreEditor(Editor editor)
			{
				bool flag = true;
				int num = 0;
				while (flag && num < this.m_EditorReferences.Count)
				{
					WeakReference weakReference = this.m_EditorReferences[num];
					Editor x = weakReference.Target as Editor;
					if (x == null)
					{
						this.m_EditorReferences.RemoveAt(num);
						this.m_CachedWeakReferences.Add(weakReference);
						num--;
					}
					else if (x == editor)
					{
						flag = false;
						break;
					}
					num++;
				}
				if (flag)
				{
					WeakReference weakReference2;
					if (this.m_CachedWeakReferences.Count > 0)
					{
						weakReference2 = this.m_CachedWeakReferences[0];
						this.m_CachedWeakReferences.RemoveAt(0);
					}
					else
					{
						weakReference2 = new WeakReference(null);
					}
					weakReference2.Target = editor;
					this.m_EditorReferences.Add(weakReference2);
				}
			}
		}

		private static SoftlockViewController s_Instance;

		public GUIStyle k_Style = null;

		public GUIStyle k_StyleEmpty = new GUIStyle();

		public GUIContent k_Content = null;

		private SoftlockViewController.Cache m_Cache = null;

		private const string k_TooltipHeader = "Unpublished changes by:";

		private const string k_TooltipPrefabHeader = "Unpublished Prefab changes by:";

		private const string k_TooltipNamePrefix = " \n •  ";

		[SerializeField]
		private SoftLockFilters m_SoftLockFilters = new SoftLockFilters();

		public SoftLockFilters softLockFilters
		{
			get
			{
				return this.m_SoftLockFilters;
			}
		}

		public static SoftlockViewController Instance
		{
			get
			{
				if (SoftlockViewController.s_Instance == null)
				{
					SoftlockViewController.s_Instance = new SoftlockViewController();
					SoftlockViewController.s_Instance.m_Cache = new SoftlockViewController.Cache();
				}
				return SoftlockViewController.s_Instance;
			}
		}

		private SoftlockViewController()
		{
		}

		~SoftlockViewController()
		{
		}

		public void TurnOn()
		{
			this.RegisterDataDelegate();
			this.RegisterDrawDelegates();
			this.Repaint();
		}

		public void TurnOff()
		{
			this.UnregisterDataDelegate();
			this.UnregisterDrawDelegates();
		}

		private void UnregisterDataDelegate()
		{
			SoftLockData.SoftlockSubscriber = (SoftLockData.OnSoftlockUpdate)Delegate.Remove(SoftLockData.SoftlockSubscriber, new SoftLockData.OnSoftlockUpdate(SoftlockViewController.Instance.OnSoftlockUpdate));
		}

		private void RegisterDataDelegate()
		{
			this.UnregisterDataDelegate();
			SoftLockData.SoftlockSubscriber = (SoftLockData.OnSoftlockUpdate)Delegate.Combine(SoftLockData.SoftlockSubscriber, new SoftLockData.OnSoftlockUpdate(SoftlockViewController.Instance.OnSoftlockUpdate));
		}

		private void UnregisterDrawDelegates()
		{
			ObjectListArea.postAssetIconDrawCallback -= new ObjectListArea.OnAssetIconDrawDelegate(SoftlockViewController.Instance.DrawProjectBrowserGridUI);
			ObjectListArea.postAssetLabelDrawCallback -= new ObjectListArea.OnAssetLabelDrawDelegate(SoftlockViewController.Instance.DrawProjectBrowserListUI);
			Editor.OnPostIconGUI = (Editor.OnEditorGUIDelegate)Delegate.Remove(Editor.OnPostIconGUI, new Editor.OnEditorGUIDelegate(SoftlockViewController.Instance.DrawInspectorUI));
			GameObjectTreeViewGUI.OnPostHeaderGUI = (GameObjectTreeViewGUI.OnHeaderGUIDelegate)Delegate.Remove(GameObjectTreeViewGUI.OnPostHeaderGUI, new GameObjectTreeViewGUI.OnHeaderGUIDelegate(SoftlockViewController.Instance.DrawSceneUI));
		}

		private void RegisterDrawDelegates()
		{
			this.UnregisterDrawDelegates();
			ObjectListArea.postAssetIconDrawCallback += new ObjectListArea.OnAssetIconDrawDelegate(SoftlockViewController.Instance.DrawProjectBrowserGridUI);
			ObjectListArea.postAssetLabelDrawCallback += new ObjectListArea.OnAssetLabelDrawDelegate(SoftlockViewController.Instance.DrawProjectBrowserListUI);
			Editor.OnPostIconGUI = (Editor.OnEditorGUIDelegate)Delegate.Combine(Editor.OnPostIconGUI, new Editor.OnEditorGUIDelegate(SoftlockViewController.Instance.DrawInspectorUI));
			GameObjectTreeViewGUI.OnPostHeaderGUI = (GameObjectTreeViewGUI.OnHeaderGUIDelegate)Delegate.Combine(GameObjectTreeViewGUI.OnPostHeaderGUI, new GameObjectTreeViewGUI.OnHeaderGUIDelegate(SoftlockViewController.Instance.DrawSceneUI));
		}

		private bool HasSoftlockSupport(Editor editor)
		{
			bool result;
			if (!CollabAccess.Instance.IsServiceEnabled() || editor == null || editor.targets.Length > 1)
			{
				result = false;
			}
			else if (editor.target == null || !SoftLockData.AllowsSoftLocks(editor.target))
			{
				result = false;
			}
			else
			{
				bool flag = true;
				Type type = editor.GetType();
				if (type != typeof(GameObjectInspector) && type != typeof(GenericInspector))
				{
					flag = false;
				}
				result = flag;
			}
			return result;
		}

		private bool HasSoftlocks(string assetGUID)
		{
			bool result;
			if (!CollabAccess.Instance.IsServiceEnabled())
			{
				result = false;
			}
			else
			{
				bool flag2;
				bool flag = SoftLockData.TryHasSoftLocks(assetGUID, out flag2) && flag2;
				result = flag;
			}
			return result;
		}

		public void OnSoftlockUpdate(string[] assetGUIDs)
		{
			this.m_Cache.InvalidateAssetGUIDs(assetGUIDs);
			this.Repaint();
		}

		public void Repaint()
		{
			this.RepaintInspectors();
			this.RepaintSceneHierarchy();
			this.RepaintProjectBrowsers();
		}

		private void RepaintSceneHierarchy()
		{
			List<SceneHierarchyWindow> allSceneHierarchyWindows = SceneHierarchyWindow.GetAllSceneHierarchyWindows();
			foreach (SceneHierarchyWindow current in allSceneHierarchyWindows)
			{
				current.Repaint();
			}
		}

		private void RepaintInspectors()
		{
			foreach (Editor current in this.m_Cache.GetEditors())
			{
				current.Repaint();
			}
		}

		private void RepaintProjectBrowsers()
		{
			foreach (ProjectBrowser current in ProjectBrowser.GetAllProjectBrowsers())
			{
				current.RefreshSearchIfFilterContains("s:");
				current.Repaint();
			}
		}

		public void DrawSceneUI(Rect availableRect, string scenePath)
		{
			string text = AssetDatabase.AssetPathToGUID(scenePath);
			if (this.HasSoftlocks(text))
			{
				int count;
				SoftLockData.TryGetSoftlockCount(text, out count);
				GUIContent gUIContent = this.GetGUIContent();
				gUIContent.image = SoftLockUIData.GetIconForSection(SoftLockUIData.SectionEnum.Scene);
				gUIContent.text = SoftlockViewController.GetDisplayCount(count);
				gUIContent.tooltip = SoftlockViewController.Instance.GetTooltip(text);
				Vector2 size = this.GetStyle().CalcSize(gUIContent);
				Rect position = new Rect(availableRect.position, size);
				position.x = availableRect.width - position.width - 4f;
				EditorGUI.LabelField(position, gUIContent);
			}
		}

		private void DrawInspectorUI(Editor editor, Rect drawRect)
		{
			if (this.HasSoftlockSupport(editor))
			{
				this.m_Cache.StoreEditor(editor);
				string assetGUID = null;
				AssetAccess.TryGetAssetGUIDFromObject(editor.target, out assetGUID);
				if (this.HasSoftlocks(assetGUID))
				{
					Texture iconForSection = SoftLockUIData.GetIconForSection(SoftLockUIData.SectionEnum.ProjectBrowser);
					if (iconForSection != null)
					{
						this.DrawIconWithTooltips(drawRect, iconForSection, assetGUID);
					}
				}
			}
		}

		private void DrawProjectBrowserGridUI(Rect iconRect, string assetGUID, bool isListMode)
		{
			if (!isListMode && this.HasSoftlocks(assetGUID))
			{
				Rect iconRect2 = Rect.zero;
				Texture iconForSection = SoftLockUIData.GetIconForSection(SoftLockUIData.SectionEnum.ProjectBrowser);
				if (iconForSection != null)
				{
					iconRect2 = Overlay.GetRectForBottomRight(iconRect, 0.35);
					this.DrawIconWithTooltips(iconRect2, iconForSection, assetGUID);
				}
			}
		}

		private bool DrawProjectBrowserListUI(Rect drawRect, string assetGUID, bool isListMode)
		{
			bool result;
			if (!this.HasSoftlocks(assetGUID))
			{
				result = false;
			}
			else
			{
				Texture iconForSection = SoftLockUIData.GetIconForSection(SoftLockUIData.SectionEnum.ProjectBrowser);
				bool flag = false;
				if (iconForSection != null)
				{
					Rect iconRect = drawRect;
					iconRect.width = drawRect.height;
					iconRect.x = (float)Math.Round((double)(drawRect.center.x - iconRect.width / 2f));
					this.DrawIconWithTooltips(iconRect, iconForSection, assetGUID);
					flag = true;
				}
				result = flag;
			}
			return result;
		}

		private void DrawIconWithTooltips(Rect iconRect, Texture icon, string assetGUID)
		{
			GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
			this.DrawTooltip(iconRect, this.GetTooltip(assetGUID));
		}

		private void DrawTooltip(Rect frame, string tooltip)
		{
			GUIContent gUIContent = this.GetGUIContent();
			gUIContent.tooltip = tooltip;
			GUI.Label(frame, gUIContent, this.k_StyleEmpty);
		}

		private string GetTooltip(string assetGUID)
		{
			string text;
			if (!this.m_Cache.TryGetTooltipForGUID(assetGUID, out text))
			{
				List<string> locksNamesOnAsset = SoftLockUIData.GetLocksNamesOnAsset(assetGUID);
				string text2 = (!SoftLockData.IsPrefab(assetGUID)) ? "Unpublished changes by:" : "Unpublished Prefab changes by:";
				text = text2;
				foreach (string current in locksNamesOnAsset)
				{
					text = text + " \n •  " + current + " ";
				}
				this.m_Cache.StoreTooltipForGUID(assetGUID, text);
			}
			return text;
		}

		private static string GetDisplayCount(int count)
		{
			string text;
			if (!SoftlockViewController.Instance.m_Cache.TryGetDisplayCount(count, out text))
			{
				text = count.ToString();
				SoftlockViewController.Instance.m_Cache.StoreDisplayCount(count, text);
			}
			return text;
		}

		private string FitTextToWidth(string text, float width, GUIStyle style)
		{
			int numCharactersThatFitWithinWidth = style.GetNumCharactersThatFitWithinWidth(text, width);
			string result;
			if (numCharactersThatFitWithinWidth > 1 && numCharactersThatFitWithinWidth != text.Length)
			{
				int num = numCharactersThatFitWithinWidth - 1;
				string text2;
				if (!SoftlockViewController.Instance.m_Cache.TryGetEllipsedNames(text, num, out text2))
				{
					text2 = text.Substring(0, num) + " …";
					SoftlockViewController.Instance.m_Cache.StoreEllipsedNames(text, text2, num);
				}
				result = text2;
			}
			else
			{
				result = text;
			}
			return result;
		}

		public GUIContent GetGUIContent()
		{
			if (this.k_Content == null)
			{
				this.k_Content = new GUIContent();
			}
			this.k_Content.tooltip = string.Empty;
			this.k_Content.text = null;
			this.k_Content.image = null;
			return this.k_Content;
		}

		public GUIStyle GetStyle()
		{
			if (this.k_Style == null)
			{
				this.k_Style = new GUIStyle(EditorStyles.label);
				this.k_Style.normal.background = null;
			}
			return this.k_Style;
		}
	}
}
