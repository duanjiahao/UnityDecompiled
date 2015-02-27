using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class PackageExport : EditorWindow
	{
		internal class Constants
		{
			public GUIStyle ConsoleEntryBackEven = "CN EntryBackEven";
			public GUIStyle ConsoleEntryBackOdd = "CN EntryBackOdd";
			public GUIStyle title = "OL Title";
			public Color lineColor;
			public Constants()
			{
				this.lineColor = ((!EditorGUIUtility.isProSkin) ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.1f, 0.1f, 0.1f));
			}
		}
		[SerializeField]
		private AssetsItem[] m_assets;
		[SerializeField]
		private bool m_bIncludeDependencies = true;
		[SerializeField]
		private int m_LeastIndent = 999999;
		[SerializeField]
		private ListViewState m_ListView;
		private static PackageExport.Constants ms_Constants;
		public PackageExport()
		{
			this.m_ListView = new ListViewState(0, 18);
			base.position = new Rect(100f, 100f, 400f, 300f);
			base.minSize = new Vector2(400f, 200f);
		}
		public void OnGUI()
		{
			if (PackageExport.ms_Constants == null)
			{
				PackageExport.ms_Constants = new PackageExport.Constants();
			}
			if (this.m_assets == null)
			{
				return;
			}
			if (this.m_LeastIndent == 999999)
			{
				int num = this.m_LeastIndent;
				for (int i = 0; i < this.m_assets.Length; i++)
				{
					int num2 = PackageExport.CountOccurencesOfChar(this.m_assets[i].pathName, '/');
					if (num > num2)
					{
						num = num2;
					}
				}
				this.m_LeastIndent = num - 1;
			}
			if (this.m_assets != null)
			{
				this.SetupListView();
				bool flag = Event.current.type == EventType.Repaint;
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Label("Items to Export", PackageExport.ms_Constants.title, new GUILayoutOption[0]);
				GUILayout.Space(1f);
				EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
				foreach (ListViewElement listViewElement in ListViewGUI.ListView(this.m_ListView, GUIStyle.none, new GUILayoutOption[0]))
				{
					AssetsItem assetsItem = this.m_assets[listViewElement.row];
					Rect position = listViewElement.position;
					position = new Rect(position.x + 1f, position.y, position.width - 2f, position.height);
					int num3 = PackageExport.CountOccurencesOfChar(assetsItem.pathName, '/') - this.m_LeastIndent;
					if (flag && this.m_ListView.row == listViewElement.row)
					{
						PackageExport.ms_Constants.ConsoleEntryBackEven.Draw(position, false, false, true, false);
					}
					float y = listViewElement.position.y;
					position.x += 3f;
					int enabled = assetsItem.enabled;
					assetsItem.enabled = ((!GUI.Toggle(new Rect(position.x, position.y, 16f, 16f), assetsItem.enabled != 0, string.Empty)) ? 0 : 1);
					if (enabled != assetsItem.enabled)
					{
						this.m_ListView.row = listViewElement.row;
						GUIUtility.keyboardControl = this.m_ListView.ID;
						this.CheckChildren(assetsItem);
					}
					if (flag)
					{
						Rect position2 = new Rect(position.x + (float)(15 * num3), y + 1f, 16f, 16f);
						Texture cachedIcon = AssetDatabase.GetCachedIcon(assetsItem.pathName);
						if (cachedIcon != null)
						{
							GUI.DrawTexture(position2, cachedIcon);
						}
					}
					position = new Rect(position.x + 20f + (float)(15 * num3), listViewElement.position.y, position.width - (float)(20 + 15 * num3), position.height);
					GUI.Label(position, assetsItem.pathName);
				}
				this.FrameLastGUIRect();
				GUILayout.EndVertical();
				if (this.m_ListView.row != -1 && GUIUtility.keyboardControl == this.m_ListView.ID && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
				{
					this.m_assets[this.m_ListView.row].enabled = ((this.m_assets[this.m_ListView.row].enabled != 0) ? 0 : 1);
					this.CheckChildren(this.m_assets[this.m_ListView.row]);
					Event.current.Use();
				}
				EditorGUIUtility.SetIconSize(Vector2.zero);
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(10f);
				if (GUILayout.Button(EditorGUIUtility.TextContent("All"), new GUILayoutOption[]
				{
					GUILayout.Width(50f)
				}))
				{
					for (int j = 0; j < this.m_assets.Length; j++)
					{
						this.m_assets[j].enabled = 1;
					}
				}
				if (GUILayout.Button(EditorGUIUtility.TextContent("None"), new GUILayoutOption[]
				{
					GUILayout.Width(50f)
				}))
				{
					for (int k = 0; k < this.m_assets.Length; k++)
					{
						this.m_assets[k].enabled = 0;
					}
				}
				GUILayout.Space(10f);
				bool flag2 = GUILayout.Toggle(this.m_bIncludeDependencies, "Include dependencies", new GUILayoutOption[0]);
				if (flag2 != this.m_bIncludeDependencies)
				{
					this.m_bIncludeDependencies = flag2;
					this.BuildAssetList();
				}
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(EditorGUIUtility.TextContent("Export..."), new GUILayoutOption[0]))
				{
					this.Export();
					GUIUtility.ExitGUI();
				}
				GUILayout.Space(10f);
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
			}
		}
		private void BuildAssetList()
		{
			this.m_assets = PackageExport.GetAssetItemsForExport(Selection.assetGUIDsDeepSelection, this.m_bIncludeDependencies).ToArray<AssetsItem>();
		}
		private void SetupListView()
		{
			if (this.m_assets != null)
			{
				this.m_ListView.totalRows = this.m_assets.Length;
			}
		}
		private void Export()
		{
			string text = EditorUtility.SaveFilePanel("Export package ...", string.Empty, string.Empty, "unitypackage");
			if (text != string.Empty)
			{
				List<string> list = new List<string>();
				AssetsItem[] assets = this.m_assets;
				for (int i = 0; i < assets.Length; i++)
				{
					AssetsItem assetsItem = assets[i];
					if (assetsItem.enabled != 0)
					{
						list.Add(assetsItem.guid);
					}
				}
				AssetServer.ExportPackage(list.ToArray(), text);
				base.Close();
				GUIUtility.ExitGUI();
			}
		}
		private static void ShowExportPackage()
		{
			PackageExport window = EditorWindow.GetWindow<PackageExport>(true, "Exporting package");
			window.BuildAssetList();
			window.Repaint();
		}
		private void CheckChildren(AssetsItem parentAI)
		{
			AssetsItem[] assets = this.m_assets;
			for (int i = 0; i < assets.Length; i++)
			{
				AssetsItem assetsItem = assets[i];
				if (assetsItem.parentGuid == parentAI.guid)
				{
					assetsItem.enabled = parentAI.enabled;
					this.CheckChildren(assetsItem);
				}
			}
		}
		private void FrameLastGUIRect()
		{
			Rect lastRect = GUILayoutUtility.GetLastRect();
			HandleUtility.handleWireMaterial.SetPass(0);
			GL.Begin(1);
			GL.Color(PackageExport.ms_Constants.lineColor);
			GL.Vertex3(lastRect.xMax + 1f, lastRect.y, 0f);
			GL.Vertex3(lastRect.xMax + 1f, lastRect.yMax, 0f);
			GL.Vertex3(lastRect.xMax + 1f, lastRect.yMax, 0f);
			GL.Vertex3(lastRect.x + 1f, lastRect.yMax, 0f);
			GL.Vertex3(lastRect.x + 1f, lastRect.yMax, 0f);
			GL.Vertex3(lastRect.x + 1f, lastRect.y, 0f);
			GL.End();
		}
		private static int CountOccurencesOfChar(string instance, char c)
		{
			int num = 0;
			for (int i = 0; i < instance.Length; i++)
			{
				char c2 = instance[i];
				if (c == c2)
				{
					num++;
				}
			}
			return num;
		}
		internal static IEnumerable<AssetsItem> GetAssetItemsForExport(ICollection<string> guids, bool includeDependencies)
		{
			if (guids.Count == 0)
			{
				string[] collection = new string[0];
				guids = new HashSet<string>(AssetServer.CollectAllChildren(AssetServer.GetRootGUID(), collection));
			}
			AssetsItem[] array = AssetServer.BuildExportPackageAssetListAssetsItems(guids.ToArray<string>(), includeDependencies);
			if (includeDependencies)
			{
				if (array.Any((AssetsItem asset) => InternalEditorUtility.IsScriptOrAssembly(asset.pathName)))
				{
					array = AssetServer.BuildExportPackageAssetListAssetsItems(guids.Union(InternalEditorUtility.GetAllScriptGUIDs()).ToArray<string>(), includeDependencies);
				}
			}
			return array;
		}
	}
}
