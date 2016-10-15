using System;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace UnityEditor.VersionControl
{
	internal class WindowResolve : EditorWindow
	{
		private ListControl resolveList = new ListControl();

		private AssetList assetList = new AssetList();

		private bool cancelled;

		public void OnEnable()
		{
			base.position = new Rect(100f, 100f, 650f, 330f);
			base.minSize = new Vector2(650f, 330f);
		}

		public void OnDisable()
		{
			if (!this.cancelled)
			{
				WindowPending.UpdateAllWindows();
			}
		}

		public static void Open(ChangeSet change)
		{
			Task task = Provider.ChangeSetStatus(change);
			task.Wait();
			WindowResolve window = WindowResolve.GetWindow();
			window.DoOpen(task.assetList);
		}

		public static void Open(AssetList assets)
		{
			Task task = Provider.Status(assets);
			task.Wait();
			WindowResolve window = WindowResolve.GetWindow();
			window.DoOpen(task.assetList);
		}

		private static WindowResolve GetWindow()
		{
			return EditorWindow.GetWindow<WindowResolve>(true, "Version Control Resolve");
		}

		private void DoOpen(AssetList resolve)
		{
			bool includeFolder = true;
			this.assetList = resolve.Filter(includeFolder, new Asset.States[]
			{
				Asset.States.Conflicted
			});
			this.RefreshList();
		}

		private void RefreshList()
		{
			this.resolveList.Clear();
			bool flag = true;
			foreach (Asset current in this.assetList)
			{
				ListItem item = this.resolveList.Add(null, current.prettyPath, current);
				if (flag)
				{
					this.resolveList.SelectedSet(item);
					flag = false;
				}
				else
				{
					this.resolveList.SelectedAdd(item);
				}
			}
			if (this.assetList.Count == 0)
			{
				ChangeSet changeSet = new ChangeSet("no files to resolve");
				ListItem listItem = this.resolveList.Add(null, changeSet.description, changeSet);
				listItem.Dummy = true;
			}
			this.resolveList.Refresh();
			base.Repaint();
		}

		private void OnGUI()
		{
			this.cancelled = false;
			GUILayout.Label("Conflicting files to resolve", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			Rect screenRect = new Rect(6f, 40f, base.position.width - 12f, base.position.height - 112f);
			GUILayout.BeginArea(screenRect);
			GUILayout.Box(string.Empty, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			GUILayout.EndArea();
			bool flag = this.resolveList.OnGUI(new Rect(screenRect.x + 2f, screenRect.y + 2f, screenRect.width - 4f, screenRect.height - 4f), true);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.enabled = (this.assetList.Count > 0);
			GUILayout.Label("Resolve selection by:", new GUILayoutOption[0]);
			if (GUILayout.Button("using local version", new GUILayoutOption[0]))
			{
				AssetList selectedAssets = this.resolveList.SelectedAssets;
				Provider.Resolve(selectedAssets, ResolveMethod.UseMine).Wait();
				AssetDatabase.Refresh();
				base.Close();
			}
			if (GUILayout.Button("using incoming version", new GUILayoutOption[0]))
			{
				AssetList selectedAssets2 = this.resolveList.SelectedAssets;
				Provider.Resolve(selectedAssets2, ResolveMethod.UseTheirs).Wait();
				AssetDatabase.Refresh();
				base.Close();
			}
			MergeMethod mergeMethod = MergeMethod.MergeNone;
			if (GUILayout.Button("merging", new GUILayoutOption[0]))
			{
				mergeMethod = MergeMethod.MergeAll;
			}
			if (mergeMethod != MergeMethod.MergeNone)
			{
				Task task = Provider.Merge(this.resolveList.SelectedAssets, mergeMethod);
				task.Wait();
				if (task.success)
				{
					task = Provider.Resolve(task.assetList, ResolveMethod.UseMerged);
					task.Wait();
					if (task.success)
					{
						task = Provider.Status(this.assetList);
						task.Wait();
						this.DoOpen(task.assetList);
						if (task.success && this.assetList.Count == 0)
						{
							base.Close();
						}
					}
					else
					{
						EditorUtility.DisplayDialog("Error resolving", "Error during resolve of files. Inspect log for details", "Close");
						AssetDatabase.Refresh();
					}
				}
				else
				{
					EditorUtility.DisplayDialog("Error merging", "Error during merge of files. Inspect log for details", "Close");
					AssetDatabase.Refresh();
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(12f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUI.enabled = true;
			if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
			{
				this.cancelled = true;
				base.Close();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(12f);
			if (flag)
			{
				base.Repaint();
			}
		}
	}
}
