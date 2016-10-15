using System;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor.VersionControl
{
	internal class WindowRevert : EditorWindow
	{
		private ListControl revertList = new ListControl();

		private AssetList assetList = new AssetList();

		public void OnEnable()
		{
			base.position = new Rect(100f, 100f, 700f, 230f);
			base.minSize = new Vector2(700f, 230f);
			this.revertList.ReadOnly = true;
		}

		public static void Open(ChangeSet change)
		{
			Task task = Provider.ChangeSetStatus(change);
			task.Wait();
			WindowRevert.GetWindow().DoOpen(task.assetList);
		}

		public static void Open(AssetList assets)
		{
			Task task = Provider.Status(assets);
			task.Wait();
			AssetList revert = task.assetList.Filter(true, new Asset.States[]
			{
				Asset.States.CheckedOutLocal,
				Asset.States.DeletedLocal,
				Asset.States.AddedLocal,
				Asset.States.Missing
			});
			WindowRevert.GetWindow().DoOpen(revert);
		}

		private static WindowRevert GetWindow()
		{
			return EditorWindow.GetWindow<WindowRevert>(true, "Version Control Revert");
		}

		private void DoOpen(AssetList revert)
		{
			this.assetList = revert;
			this.RefreshList();
		}

		private void RefreshList()
		{
			this.revertList.Clear();
			foreach (Asset current in this.assetList)
			{
				this.revertList.Add(null, current.prettyPath, current);
			}
			if (this.assetList.Count == 0)
			{
				ChangeSet changeSet = new ChangeSet("no files to revert");
				ListItem listItem = this.revertList.Add(null, changeSet.description, changeSet);
				listItem.Dummy = true;
			}
			this.revertList.Refresh();
			base.Repaint();
		}

		private void OnGUI()
		{
			GUILayout.Label("Revert Files", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			Rect screenRect = new Rect(6f, 40f, base.position.width - 12f, base.position.height - 82f);
			GUILayout.BeginArea(screenRect);
			GUILayout.Box(string.Empty, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			GUILayout.EndArea();
			this.revertList.OnGUI(new Rect(screenRect.x + 2f, screenRect.y + 2f, screenRect.width - 4f, screenRect.height - 4f), true);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
			{
				base.Close();
			}
			if (this.assetList.Count > 0 && GUILayout.Button("Revert", new GUILayoutOption[0]))
			{
				string text = string.Empty;
				foreach (Asset current in this.assetList)
				{
					Scene sceneByPath = SceneManager.GetSceneByPath(current.path);
					if (sceneByPath.IsValid() && sceneByPath.isLoaded)
					{
						text = text + sceneByPath.path + "\n";
					}
				}
				if (text.Length > 0 && !EditorUtility.DisplayDialog("Revert open scene(s)?", "You are about to revert your currently open scene(s):\n\n" + text + "\nContinuing will remove all unsaved changes.", "Continue", "Cancel"))
				{
					base.Close();
					return;
				}
				Provider.Revert(this.assetList, RevertMode.Normal).Wait();
				WindowPending.UpdateAllWindows();
				AssetDatabase.Refresh();
				base.Close();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(12f);
		}
	}
}
