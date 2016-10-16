using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetSaveDialog : EditorWindow
	{
		private class Styles
		{
			public GUIStyle selected = "ServerUpdateChangesetOn";

			public GUIStyle box = "OL Box";

			public GUIStyle button = "LargeButton";

			public GUIContent saveSelected = EditorGUIUtility.TextContent("Save Selected");

			public GUIContent saveAll = EditorGUIUtility.TextContent("Save All");

			public GUIContent dontSave = EditorGUIUtility.TextContent("Don't Save");

			public GUIContent close = EditorGUIUtility.TextContent("Close");

			public float buttonWidth;

			public Styles()
			{
				this.buttonWidth = Mathf.Max(Mathf.Max(this.button.CalcSize(this.saveSelected).x, this.button.CalcSize(this.saveAll).x), this.button.CalcSize(this.dontSave).x);
			}
		}

		private static AssetSaveDialog.Styles s_Styles;

		private List<string> m_Assets;

		private List<string> m_AssetsToSave;

		private ListViewState m_LV = new ListViewState();

		private int m_InitialSelectedItem = -1;

		private bool[] m_SelectedItems;

		private List<GUIContent> m_Content;

		private void SetAssets(string[] assets)
		{
			this.m_Assets = new List<string>(assets);
			this.RebuildLists(true);
			this.m_AssetsToSave = new List<string>();
		}

		public static void ShowWindow(string[] inAssets, out string[] assetsThatShouldBeSaved)
		{
			int num = 0;
			for (int i = 0; i < inAssets.Length; i++)
			{
				string text = inAssets[i];
				if (text.EndsWith("meta"))
				{
					num++;
				}
			}
			int num2 = inAssets.Length - num;
			if (num2 == 0)
			{
				assetsThatShouldBeSaved = inAssets;
				return;
			}
			string[] array = new string[num2];
			string[] array2 = new string[num];
			num2 = 0;
			num = 0;
			for (int j = 0; j < inAssets.Length; j++)
			{
				string text2 = inAssets[j];
				if (text2.EndsWith("meta"))
				{
					array2[num++] = text2;
				}
				else
				{
					array[num2++] = text2;
				}
			}
			AssetSaveDialog windowDontShow = EditorWindow.GetWindowDontShow<AssetSaveDialog>();
			windowDontShow.titleContent = EditorGUIUtility.TextContent("Save Assets");
			windowDontShow.SetAssets(array);
			windowDontShow.ShowUtility();
			windowDontShow.ShowModal();
			assetsThatShouldBeSaved = new string[windowDontShow.m_AssetsToSave.Count + num];
			windowDontShow.m_AssetsToSave.CopyTo(assetsThatShouldBeSaved, 0);
			array2.CopyTo(assetsThatShouldBeSaved, windowDontShow.m_AssetsToSave.Count);
		}

		public static GUIContent GetContentForAsset(string path)
		{
			Texture cachedIcon = AssetDatabase.GetCachedIcon(path);
			if (path.StartsWith("Library/"))
			{
				path = ObjectNames.NicifyVariableName(AssetDatabase.LoadMainAssetAtPath(path).name);
			}
			if (path.StartsWith("Assets/"))
			{
				path = path.Substring(7);
			}
			return new GUIContent(path, cachedIcon);
		}

		private void HandleKeyboard()
		{
		}

		private void OnGUI()
		{
			if (AssetSaveDialog.s_Styles == null)
			{
				AssetSaveDialog.s_Styles = new AssetSaveDialog.Styles();
				base.minSize = new Vector2(500f, 300f);
				base.position = new Rect(base.position.x, base.position.y, base.minSize.x, base.minSize.y);
			}
			this.HandleKeyboard();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label("Unity is about to save the following modified files. Unsaved changes will be lost!", new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			int row = this.m_LV.row;
			int num = 0;
			foreach (ListViewElement listViewElement in ListViewGUILayout.ListView(this.m_LV, AssetSaveDialog.s_Styles.box, new GUILayoutOption[0]))
			{
				if (this.m_SelectedItems[listViewElement.row] && Event.current.type == EventType.Repaint)
				{
					Rect position = listViewElement.position;
					position.x += 1f;
					position.y += 1f;
					position.width -= 1f;
					position.height -= 1f;
					AssetSaveDialog.s_Styles.selected.Draw(position, false, false, false, false);
				}
				GUILayout.Label(this.m_Content[listViewElement.row], new GUILayoutOption[0]);
				if (ListViewGUILayout.HasMouseUp(listViewElement.position))
				{
					Event.current.command = true;
					Event.current.control = true;
					ListViewGUILayout.MultiSelection(row, listViewElement.row, ref this.m_InitialSelectedItem, ref this.m_SelectedItems);
				}
				if (this.m_SelectedItems[listViewElement.row])
				{
					num++;
				}
			}
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			if (GUILayout.Button(AssetSaveDialog.s_Styles.close, AssetSaveDialog.s_Styles.button, new GUILayoutOption[]
			{
				GUILayout.Width(AssetSaveDialog.s_Styles.buttonWidth)
			}))
			{
				this.CloseWindow();
			}
			GUILayout.FlexibleSpace();
			GUI.enabled = (num > 0);
			bool flag = num == this.m_Assets.Count;
			if (GUILayout.Button(AssetSaveDialog.s_Styles.dontSave, AssetSaveDialog.s_Styles.button, new GUILayoutOption[]
			{
				GUILayout.Width(AssetSaveDialog.s_Styles.buttonWidth)
			}))
			{
				this.IgnoreSelectedAssets();
			}
			if (GUILayout.Button((!flag) ? AssetSaveDialog.s_Styles.saveSelected : AssetSaveDialog.s_Styles.saveAll, AssetSaveDialog.s_Styles.button, new GUILayoutOption[]
			{
				GUILayout.Width(AssetSaveDialog.s_Styles.buttonWidth)
			}))
			{
				this.SaveSelectedAssets();
			}
			if (this.m_Assets.Count == 0)
			{
				this.CloseWindow();
			}
			GUI.enabled = true;
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
		}

		private void Cancel()
		{
			base.Close();
			GUIUtility.ExitGUI();
		}

		private void CloseWindow()
		{
			base.Close();
			GUIUtility.ExitGUI();
		}

		private void SaveSelectedAssets()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.m_SelectedItems.Length; i++)
			{
				if (this.m_SelectedItems[i])
				{
					this.m_AssetsToSave.Add(this.m_Assets[i]);
				}
				else
				{
					list.Add(this.m_Assets[i]);
				}
			}
			this.m_Assets = list;
			this.RebuildLists(false);
		}

		private void IgnoreSelectedAssets()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.m_SelectedItems.Length; i++)
			{
				if (!this.m_SelectedItems[i])
				{
					list.Add(this.m_Assets[i]);
				}
			}
			this.m_Assets = list;
			this.RebuildLists(false);
			if (this.m_Assets.Count == 0)
			{
				this.CloseWindow();
			}
		}

		private void RebuildLists(bool selected)
		{
			this.m_LV.totalRows = this.m_Assets.Count;
			this.m_SelectedItems = new bool[this.m_Assets.Count];
			this.m_Content = new List<GUIContent>(this.m_Assets.Count);
			for (int i = 0; i < this.m_Assets.Count; i++)
			{
				this.m_SelectedItems[i] = selected;
				this.m_Content.Add(AssetSaveDialog.GetContentForAsset(this.m_Assets[i]));
			}
		}
	}
}
