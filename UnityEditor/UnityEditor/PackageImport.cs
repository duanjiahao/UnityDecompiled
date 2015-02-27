using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace UnityEditor
{
	internal class PackageImport : EditorWindow, ISerializationCallbackReceiver
	{
		internal class Constants
		{
			public GUIStyle ConsoleEntryBackEven = "CN EntryBackEven";
			public GUIStyle ConsoleEntryBackOdd = "CN EntryBackOdd";
			public GUIStyle title = new GUIStyle(EditorStyles.largeLabel);
			public GUIStyle bottomBarBg = "ProjectBrowserBottomBarBg";
			public GUIStyle topBarBg = new GUIStyle("ProjectBrowserHeaderBgTop");
			public GUIStyle textureIconDropShadow = "ProjectBrowserTextureIconDropShadow";
			public Color lineColor;
			public Constants()
			{
				this.lineColor = ((!EditorGUIUtility.isProSkin) ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.1f, 0.1f, 0.1f));
				this.topBarBg.fixedHeight = 0f;
				RectOffset arg_D7_0 = this.topBarBg.border;
				int num = 2;
				this.topBarBg.border.bottom = num;
				arg_D7_0.top = num;
				this.title.fontStyle = FontStyle.Bold;
				this.title.alignment = TextAnchor.MiddleLeft;
			}
		}
		[SerializeField]
		private AssetsItem[] m_Assets;
		[SerializeField]
		private List<string> m_EnabledFolders;
		[SerializeField]
		private string m_PackageName;
		[SerializeField]
		private string m_PackageIconPath;
		[SerializeField]
		private TreeViewState m_TreeViewState;
		[NonSerialized]
		private PackageImportTreeView m_Tree;
		private static Texture2D s_PackageIcon;
		private static Texture2D s_Preview;
		private static string s_LastPreviewPath;
		private static PackageImport.Constants ms_Constants;
		public PackageImport()
		{
			base.minSize = new Vector2(350f, 350f);
		}
		public static void ShowImportPackage(string packagePath, AssetsItem[] items, string packageIconPath)
		{
			PackageImport window = EditorWindow.GetWindow<PackageImport>(true, "Importing package");
			window.Init(packagePath, items, packageIconPath);
		}
		public void OnBeforeSerialize()
		{
			if (this.m_Tree != null)
			{
				if (this.m_EnabledFolders == null)
				{
					this.m_EnabledFolders = new List<string>();
				}
				this.m_EnabledFolders.Clear();
				this.m_Tree.GetEnabledFolders(this.m_EnabledFolders);
			}
		}
		public void OnAfterDeserialize()
		{
		}
		private void OnDisable()
		{
			this.DestroyCreatedIcons();
		}
		private void DestroyCreatedIcons()
		{
			if (PackageImport.s_Preview != null)
			{
				UnityEngine.Object.DestroyImmediate(PackageImport.s_Preview);
				PackageImport.s_Preview = null;
				PackageImport.s_LastPreviewPath = null;
			}
			if (PackageImport.s_PackageIcon != null)
			{
				UnityEngine.Object.DestroyImmediate(PackageImport.s_PackageIcon);
				PackageImport.s_PackageIcon = null;
			}
		}
		private void Init(string packagePath, AssetsItem[] items, string packageIconPath)
		{
			this.DestroyCreatedIcons();
			this.m_TreeViewState = null;
			this.m_Tree = null;
			this.m_EnabledFolders = null;
			this.m_Assets = items;
			this.m_PackageName = Path.GetFileNameWithoutExtension(packagePath);
			this.m_PackageIconPath = packageIconPath;
			base.Repaint();
		}
		public void OnGUI()
		{
			if (PackageImport.ms_Constants == null)
			{
				PackageImport.ms_Constants = new PackageImport.Constants();
			}
			if (this.m_Assets == null)
			{
				return;
			}
			if (this.m_TreeViewState == null)
			{
				this.m_TreeViewState = new TreeViewState();
			}
			if (this.m_Tree == null)
			{
				this.m_Tree = new PackageImportTreeView(this.m_Assets, this.m_EnabledFolders, this.m_TreeViewState, this, default(Rect));
			}
			if (this.m_Assets.Length > 0)
			{
				this.TopArea();
				this.m_Tree.OnGUI(GUILayoutUtility.GetRect(1f, 9999f, 1f, 99999f));
				this.BottomArea();
			}
			else
			{
				GUILayout.Label("Nothing to import!", EditorStyles.boldLabel, new GUILayoutOption[0]);
				GUILayout.Label("All assets from this package are already in your project.", "WordWrappedLabel", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("OK", new GUILayoutOption[0]))
				{
					base.Close();
					GUIUtility.ExitGUI();
				}
				GUILayout.EndHorizontal();
			}
		}
		private void TopArea()
		{
			if (PackageImport.s_PackageIcon == null && !string.IsNullOrEmpty(this.m_PackageIconPath))
			{
				PackageImport.LoadTexture(this.m_PackageIconPath, ref PackageImport.s_PackageIcon);
			}
			bool flag = PackageImport.s_PackageIcon != null;
			float height = (!flag) ? 52f : 84f;
			Rect rect = GUILayoutUtility.GetRect(base.position.width, height);
			GUI.Label(rect, GUIContent.none, PackageImport.ms_Constants.topBarBg);
			Rect position;
			if (flag)
			{
				Rect r = new Rect(rect.x + 10f, rect.y + 10f, 64f, 64f);
				PackageImport.DrawTexture(r, PackageImport.s_PackageIcon, true);
				position = new Rect(r.xMax + 10f, r.yMin, rect.width, r.height);
			}
			else
			{
				position = new Rect(rect.x + 5f, rect.yMin, rect.width, rect.height);
			}
			GUI.Label(position, this.m_PackageName, PackageImport.ms_Constants.title);
		}
		private void BottomArea()
		{
			GUILayout.BeginVertical(PackageImport.ms_Constants.bottomBarBg, new GUILayoutOption[0]);
			GUILayout.Space(8f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			if (GUILayout.Button(EditorGUIUtility.TextContent("All"), new GUILayoutOption[]
			{
				GUILayout.Width(50f)
			}))
			{
				this.m_Tree.SetAllEnabled(1);
			}
			if (GUILayout.Button(EditorGUIUtility.TextContent("None"), new GUILayoutOption[]
			{
				GUILayout.Width(50f)
			}))
			{
				this.m_Tree.SetAllEnabled(0);
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(EditorGUIUtility.TextContent("Cancel"), new GUILayoutOption[0]))
			{
				base.Close();
				GUIUtility.ExitGUI();
			}
			if (GUILayout.Button(EditorGUIUtility.TextContent("Import"), new GUILayoutOption[0]))
			{
				if (this.m_Assets != null)
				{
					AssetServer.ImportPackageStep2(this.m_Assets);
				}
				base.Close();
				GUIUtility.ExitGUI();
			}
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			GUILayout.EndVertical();
		}
		private static void LoadTexture(string filepath, ref Texture2D texture)
		{
			if (!texture)
			{
				texture = new Texture2D(128, 128);
			}
			byte[] array = null;
			try
			{
				array = File.ReadAllBytes(filepath);
			}
			catch
			{
			}
			if (filepath == string.Empty || array == null || !texture.LoadImage(array))
			{
				Color[] pixels = texture.GetPixels();
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = new Color(0.5f, 0.5f, 0.5f, 0f);
				}
				texture.SetPixels(pixels);
				texture.Apply();
			}
		}
		public static void DrawTexture(Rect r, Texture2D tex, bool useDropshadow)
		{
			if (tex == null)
			{
				return;
			}
			float num = (float)tex.width;
			float num2 = (float)tex.height;
			if (num >= num2 && num > r.width)
			{
				num2 = num2 * r.width / num;
				num = r.width;
			}
			else
			{
				if (num2 > num && num2 > r.height)
				{
					num = num * r.height / num2;
					num2 = r.height;
				}
			}
			float left = r.x + Mathf.Round((r.width - num) / 2f);
			float top = r.y + Mathf.Round((r.height - num2) / 2f);
			r = new Rect(left, top, num, num2);
			if (useDropshadow && Event.current.type == EventType.Repaint)
			{
				Rect position = new RectOffset(1, 1, 1, 1).Remove(PackageImport.ms_Constants.textureIconDropShadow.border.Add(r));
				PackageImport.ms_Constants.textureIconDropShadow.Draw(position, GUIContent.none, false, false, false, false);
			}
			GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit, true);
		}
		public static Texture2D GetPreview(string previewPath)
		{
			if (previewPath != PackageImport.s_LastPreviewPath)
			{
				PackageImport.s_LastPreviewPath = previewPath;
				PackageImport.LoadTexture(previewPath, ref PackageImport.s_Preview);
			}
			return PackageImport.s_Preview;
		}
	}
}
