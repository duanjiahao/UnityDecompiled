using System;
using System.IO;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor
{
	internal class PackageImport : EditorWindow
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
				RectOffset arg_D8_0 = this.topBarBg.border;
				int num = 2;
				this.topBarBg.border.bottom = num;
				arg_D8_0.top = num;
				this.title.fontStyle = FontStyle.Bold;
				this.title.alignment = TextAnchor.MiddleLeft;
			}
		}

		[SerializeField]
		private ImportPackageItem[] m_ImportPackageItems;

		[SerializeField]
		private string m_PackageName;

		[SerializeField]
		private string m_PackageIconPath;

		[SerializeField]
		private TreeViewState m_TreeViewState;

		[NonSerialized]
		private PackageImportTreeView m_Tree;

		private bool m_ShowReInstall;

		private bool m_ReInstallPackage;

		private static Texture2D s_PackageIcon;

		private static Texture2D s_Preview;

		private static string s_LastPreviewPath;

		private static readonly char[] s_InvalidPathChars = Path.GetInvalidPathChars();

		private static PackageImport.Constants ms_Constants;

		public bool canReInstall
		{
			get
			{
				return this.m_ShowReInstall;
			}
		}

		public bool doReInstall
		{
			get
			{
				return this.m_ShowReInstall && this.m_ReInstallPackage;
			}
		}

		public ImportPackageItem[] packageItems
		{
			get
			{
				return this.m_ImportPackageItems;
			}
		}

		public PackageImport()
		{
			base.minSize = new Vector2(350f, 350f);
		}

		public static void ShowImportPackage(string packagePath, ImportPackageItem[] items, string packageIconPath, bool allowReInstall)
		{
			if (PackageImport.ValidateInput(items))
			{
				PackageImport window = EditorWindow.GetWindow<PackageImport>(true, "Import Unity Package");
				window.Init(packagePath, items, packageIconPath, allowReInstall);
			}
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

		private void Init(string packagePath, ImportPackageItem[] items, string packageIconPath, bool allowReInstall)
		{
			this.DestroyCreatedIcons();
			this.m_ShowReInstall = allowReInstall;
			this.m_ReInstallPackage = true;
			this.m_TreeViewState = null;
			this.m_Tree = null;
			this.m_ImportPackageItems = items;
			this.m_PackageName = Path.GetFileNameWithoutExtension(packagePath);
			this.m_PackageIconPath = packageIconPath;
			base.Repaint();
		}

		private bool ShowTreeGUI(bool reInstalling, ImportPackageItem[] items)
		{
			bool result;
			if (reInstalling)
			{
				result = true;
			}
			else if (items.Length == 0)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < items.Length; i++)
				{
					if (!items[i].isFolder && items[i].assetChanged)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		public void OnGUI()
		{
			if (PackageImport.ms_Constants == null)
			{
				PackageImport.ms_Constants = new PackageImport.Constants();
			}
			if (this.m_TreeViewState == null)
			{
				this.m_TreeViewState = new TreeViewState();
			}
			if (this.m_Tree == null)
			{
				this.m_Tree = new PackageImportTreeView(this, this.m_TreeViewState, default(Rect));
			}
			if (this.m_ImportPackageItems != null && this.ShowTreeGUI(this.doReInstall, this.m_ImportPackageItems))
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
				GUILayout.BeginVertical(PackageImport.ms_Constants.bottomBarBg, new GUILayoutOption[0]);
				GUILayout.Space(8f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				this.ReInstallToggle();
				if (GUILayout.Button("OK", new GUILayoutOption[0]))
				{
					base.Close();
					GUIUtility.ExitGUI();
				}
				GUILayout.Space(10f);
				GUILayout.EndHorizontal();
				GUILayout.Space(5f);
				GUILayout.EndVertical();
			}
		}

		private void ReInstallToggle()
		{
			if (this.m_ShowReInstall)
			{
				EditorGUI.BeginChangeCheck();
				bool reInstallPackage = GUILayout.Toggle(this.m_ReInstallPackage, "Re-Install Package", new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_ReInstallPackage = reInstallPackage;
				}
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
				this.m_Tree.SetAllEnabled(PackageImportTreeView.EnabledState.All);
			}
			if (GUILayout.Button(EditorGUIUtility.TextContent("None"), new GUILayoutOption[]
			{
				GUILayout.Width(50f)
			}))
			{
				this.m_Tree.SetAllEnabled(PackageImportTreeView.EnabledState.None);
			}
			this.ReInstallToggle();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(EditorGUIUtility.TextContent("Cancel"), new GUILayoutOption[0]))
			{
				PopupWindowWithoutFocus.Hide();
				base.Close();
				GUIUtility.ExitGUI();
			}
			if (GUILayout.Button(EditorGUIUtility.TextContent("Import"), new GUILayoutOption[0]))
			{
				bool flag = true;
				if (this.doReInstall)
				{
					flag = EditorUtility.DisplayDialog("Re-Install?", "Highlighted folders will be completely deleted first! Recommend backing up your project first. Are you sure?", "Do It", "Cancel");
				}
				if (flag)
				{
					if (this.m_ImportPackageItems != null)
					{
						PackageUtility.ImportPackageAssets(this.m_PackageName, this.m_ImportPackageItems, this.doReInstall);
					}
					PopupWindowWithoutFocus.Hide();
					base.Close();
					GUIUtility.ExitGUI();
				}
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
			if (filepath == "" || array == null || !texture.LoadImage(array))
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
			if (!(tex == null))
			{
				float num = (float)tex.width;
				float num2 = (float)tex.height;
				if (num >= num2 && num > r.width)
				{
					num2 = num2 * r.width / num;
					num = r.width;
				}
				else if (num2 > num && num2 > r.height)
				{
					num = num * r.height / num2;
					num2 = r.height;
				}
				float x = r.x + Mathf.Round((r.width - num) / 2f);
				float y = r.y + Mathf.Round((r.height - num2) / 2f);
				r = new Rect(x, y, num, num2);
				if (useDropshadow && Event.current.type == EventType.Repaint)
				{
					Rect position = new RectOffset(1, 1, 1, 1).Remove(PackageImport.ms_Constants.textureIconDropShadow.border.Add(r));
					PackageImport.ms_Constants.textureIconDropShadow.Draw(position, GUIContent.none, false, false, false, false);
				}
				GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit, true);
			}
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

		private static bool ValidateInput(ImportPackageItem[] items)
		{
			string text;
			bool result;
			if (!PackageImport.IsAllFilePathsValid(items, out text))
			{
				text += "\nDo you want to import the valid file paths of the package or cancel importing?";
				result = EditorUtility.DisplayDialog("Invalid file path found", text, "Import", "Cancel importing");
			}
			else
			{
				result = true;
			}
			return result;
		}

		private static bool IsAllFilePathsValid(ImportPackageItem[] assetItems, out string errorMessage)
		{
			bool result;
			for (int i = 0; i < assetItems.Length; i++)
			{
				ImportPackageItem importPackageItem = assetItems[i];
				if (!importPackageItem.isFolder)
				{
					char c;
					int num;
					if (PackageImport.HasInvalidCharInFilePath(importPackageItem.destinationAssetPath, out c, out num))
					{
						errorMessage = string.Format("Invalid character found in file path: '{0}'. Invalid ascii value: {1} (at character index {2}).", importPackageItem.destinationAssetPath, (int)c, num);
						result = false;
						return result;
					}
				}
			}
			errorMessage = "";
			result = true;
			return result;
		}

		private static bool HasInvalidCharInFilePath(string filePath, out char invalidChar, out int invalidCharIndex)
		{
			bool result;
			for (int i = 0; i < filePath.Length; i++)
			{
				char c = filePath[i];
				if (PackageImport.s_InvalidPathChars.Contains(c))
				{
					invalidChar = c;
					invalidCharIndex = i;
					result = true;
					return result;
				}
			}
			invalidChar = ' ';
			invalidCharIndex = -1;
			result = false;
			return result;
		}

		public static bool HasInvalidCharInFilePath(string filePath)
		{
			char c;
			int num;
			return PackageImport.HasInvalidCharInFilePath(filePath, out c, out num);
		}
	}
}
