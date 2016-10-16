using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetInspector
	{
		private static AssetInspector s_Instance;

		private GUIContent[] m_Menu = new GUIContent[]
		{
			EditorGUIUtility.TextContent("Show Diff"),
			EditorGUIUtility.TextContent("Show History"),
			EditorGUIUtility.TextContent("Discard")
		};

		private GUIContent[] m_UnmodifiedMenu = new GUIContent[]
		{
			EditorGUIUtility.TextContent("Show History")
		};

		private AssetInspector()
		{
		}

		internal static bool IsAssetServerSetUp()
		{
			return InternalEditorUtility.HasTeamLicense() && ASEditorBackend.SettingsAreValid();
		}

		private bool HasFlag(ChangeFlags flags, ChangeFlags flagToCheck)
		{
			return (flagToCheck & flags) != ChangeFlags.None;
		}

		public static AssetInspector Get()
		{
			if (AssetInspector.s_Instance == null)
			{
				AssetInspector.s_Instance = new AssetInspector();
			}
			return AssetInspector.s_Instance;
		}

		private string AddChangesetFlag(string str, string strToAdd)
		{
			if (str != string.Empty)
			{
				str += ", ";
				str += strToAdd;
			}
			else
			{
				str = strToAdd;
			}
			return str;
		}

		private string GetGUID()
		{
			if (Selection.objects.Length == 0)
			{
				return string.Empty;
			}
			return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Selection.objects[0]));
		}

		private void DoShowDiff(string guid)
		{
			List<string> list = new List<string>();
			List<CompareInfo> list2 = new List<CompareInfo>();
			int workingItemChangeset = AssetServer.GetWorkingItemChangeset(guid);
			list.Add(guid);
			list2.Add(new CompareInfo(workingItemChangeset, -1, 0, 1));
			Debug.Log("Comparing asset revisions " + workingItemChangeset.ToString() + " and Local");
			AssetServer.CompareFiles(list.ToArray(), list2.ToArray());
		}

		private void ContextMenuClick(object userData, string[] options, int selected)
		{
			if ((bool)userData && selected == 0)
			{
				selected = 1;
			}
			switch (selected)
			{
			case 0:
				this.DoShowDiff(this.GetGUID());
				break;
			case 1:
				ASEditorBackend.DoAS();
				ASEditorBackend.ASWin.ShowHistory();
				break;
			case 2:
				if (!ASEditorBackend.SettingsIfNeeded())
				{
					Debug.Log("Asset Server connection for current project is not set up");
				}
				if (EditorUtility.DisplayDialog("Discard changes", "Are you sure you want to discard local changes of selected asset?", "Discard", "Cancel"))
				{
					AssetServer.DoUpdateWithoutConflictResolutionOnNextTick(new string[]
					{
						this.GetGUID()
					});
				}
				break;
			}
		}

		private ChangeFlags GetChangeFlags()
		{
			string gUID = this.GetGUID();
			if (gUID == string.Empty)
			{
				return ChangeFlags.None;
			}
			return AssetServer.GetChangeFlags(gUID);
		}

		private string GetModificationString(ChangeFlags flags)
		{
			string text = string.Empty;
			if (this.HasFlag(flags, ChangeFlags.Undeleted))
			{
				text = this.AddChangesetFlag(text, "undeleted");
			}
			if (this.HasFlag(flags, ChangeFlags.Modified))
			{
				text = this.AddChangesetFlag(text, "modified");
			}
			if (this.HasFlag(flags, ChangeFlags.Renamed))
			{
				text = this.AddChangesetFlag(text, "renamed");
			}
			if (this.HasFlag(flags, ChangeFlags.Moved))
			{
				text = this.AddChangesetFlag(text, "moved");
			}
			if (this.HasFlag(flags, ChangeFlags.Created))
			{
				text = this.AddChangesetFlag(text, "created");
			}
			return text;
		}

		public void OnAssetStatusGUI(Rect r, int id, UnityEngine.Object target, GUIStyle style)
		{
			if (target == null)
			{
				return;
			}
			ChangeFlags changeFlags = this.GetChangeFlags();
			string modificationString = this.GetModificationString(changeFlags);
			GUIContent content;
			if (modificationString == string.Empty)
			{
				content = EditorGUIUtility.TextContent("Asset is unchanged");
			}
			else
			{
				content = new GUIContent("Locally " + modificationString);
			}
			if (EditorGUI.DoToggle(r, id, false, content, style))
			{
				GUIUtility.hotControl = 0;
				r = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
				EditorUtility.DisplayCustomMenu(r, (!(modificationString == string.Empty)) ? this.m_Menu : this.m_UnmodifiedMenu, -1, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), modificationString == string.Empty);
			}
		}
	}
}
