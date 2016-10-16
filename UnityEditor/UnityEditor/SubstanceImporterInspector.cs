using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(SubstanceArchive))]
	internal class SubstanceImporterInspector : Editor
	{
		private class SubstanceStyles
		{
			public GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add substance from prototype.");

			public GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selected substance.");

			public GUIContent iconDuplicate = EditorGUIUtility.IconContent("TreeEditor.Duplicate", "Duplicate selected substance.");

			public GUIStyle resultsGridLabel = "ObjectPickerResultsGridLabel";

			public GUIStyle resultsGrid = "ObjectPickerResultsGrid";

			public GUIStyle gridBackground = "TE NodeBackground";

			public GUIStyle background = "ObjectPickerBackground";

			public GUIStyle toolbar = "TE Toolbar";

			public GUIStyle toolbarButton = "TE toolbarbutton";

			public GUIStyle toolbarDropDown = "TE toolbarDropDown";
		}

		public class SubstanceNameComparer : IComparer
		{
			public int Compare(object o1, object o2)
			{
				UnityEngine.Object @object = o1 as UnityEngine.Object;
				UnityEngine.Object object2 = o2 as UnityEngine.Object;
				return EditorUtility.NaturalCompare(@object.name, object2.name);
			}
		}

		private const float kPreviewWidth = 60f;

		private const float kPreviewHeight = 76f;

		private const int kMaxRows = 2;

		private static SubstanceArchive s_LastSelectedPackage = null;

		private static string s_CachedSelectedMaterialInstanceName = null;

		private string m_SelectedMaterialInstanceName;

		private Vector2 m_ListScroll = Vector2.zero;

		private EditorCache m_EditorCache;

		[NonSerialized]
		private string[] m_PrototypeNames;

		private Editor m_MaterialInspector;

		protected bool m_IsVisible;

		public Vector2 previewDir = new Vector2(0f, -20f);

		public int selectedMesh;

		public int lightMode = 1;

		private PreviewRenderUtility m_PreviewUtility;

		private static Mesh[] s_Meshes = new Mesh[4];

		private static GUIContent[] s_MeshIcons = new GUIContent[4];

		private static GUIContent[] s_LightIcons = new GUIContent[2];

		private SubstanceImporterInspector.SubstanceStyles m_SubstanceStyles;

		private static int previewNoDragDropHash = "PreviewWithoutDragAndDrop".GetHashCode();

		public void OnEnable()
		{
			if (this.target == SubstanceImporterInspector.s_LastSelectedPackage)
			{
				this.m_SelectedMaterialInstanceName = SubstanceImporterInspector.s_CachedSelectedMaterialInstanceName;
			}
			else
			{
				SubstanceImporterInspector.s_LastSelectedPackage = (this.target as SubstanceArchive);
			}
		}

		public void OnDisable()
		{
			if (this.m_EditorCache != null)
			{
				this.m_EditorCache.Dispose();
			}
			if (this.m_MaterialInspector != null)
			{
				ProceduralMaterialInspector proceduralMaterialInspector = (ProceduralMaterialInspector)this.m_MaterialInspector;
				proceduralMaterialInspector.ReimportSubstancesIfNeeded();
				UnityEngine.Object.DestroyImmediate(this.m_MaterialInspector);
			}
			SubstanceImporterInspector.s_CachedSelectedMaterialInstanceName = this.m_SelectedMaterialInstanceName;
			if (this.m_PreviewUtility != null)
			{
				this.m_PreviewUtility.Cleanup();
				this.m_PreviewUtility = null;
			}
		}

		private ProceduralMaterial GetSelectedMaterial()
		{
			SubstanceImporter importer = this.GetImporter();
			if (importer == null)
			{
				return null;
			}
			ProceduralMaterial[] sortedMaterials = this.GetSortedMaterials();
			if (this.m_SelectedMaterialInstanceName != null)
			{
				return Array.Find<ProceduralMaterial>(sortedMaterials, (ProceduralMaterial element) => element.name == this.m_SelectedMaterialInstanceName);
			}
			if (sortedMaterials.Length > 0)
			{
				this.m_SelectedMaterialInstanceName = sortedMaterials[0].name;
				return sortedMaterials[0];
			}
			return null;
		}

		private void SelectNextMaterial()
		{
			SubstanceImporter importer = this.GetImporter();
			if (importer == null)
			{
				return;
			}
			string selectedMaterialInstanceName = null;
			ProceduralMaterial[] sortedMaterials = this.GetSortedMaterials();
			for (int i = 0; i < sortedMaterials.Length; i++)
			{
				if (sortedMaterials[i].name == this.m_SelectedMaterialInstanceName)
				{
					int num = Math.Min(i + 1, sortedMaterials.Length - 1);
					if (num == i)
					{
						num--;
					}
					if (num >= 0)
					{
						selectedMaterialInstanceName = sortedMaterials[num].name;
					}
					break;
				}
			}
			this.m_SelectedMaterialInstanceName = selectedMaterialInstanceName;
		}

		private Editor GetSelectedMaterialInspector()
		{
			ProceduralMaterial selectedMaterial = this.GetSelectedMaterial();
			if (selectedMaterial && this.m_MaterialInspector != null && this.m_MaterialInspector.target == selectedMaterial)
			{
				return this.m_MaterialInspector;
			}
			EditorGUI.EndEditingActiveTextField();
			UnityEngine.Object.DestroyImmediate(this.m_MaterialInspector);
			this.m_MaterialInspector = null;
			if (selectedMaterial)
			{
				this.m_MaterialInspector = Editor.CreateEditor(selectedMaterial);
				if (!(this.m_MaterialInspector is ProceduralMaterialInspector) && this.m_MaterialInspector != null)
				{
					if (selectedMaterial.shader != null)
					{
						Debug.LogError("The shader: '" + selectedMaterial.shader.name + "' is using a custom editor deriving from MaterialEditor, please derive from ShaderGUI instead. Only the ShaderGUI approach works with Procedural Materials. Search the docs for 'ShaderGUI'");
					}
					UnityEngine.Object.DestroyImmediate(this.m_MaterialInspector);
					this.m_MaterialInspector = Editor.CreateEditor(selectedMaterial, typeof(ProceduralMaterialInspector));
				}
				ProceduralMaterialInspector proceduralMaterialInspector = (ProceduralMaterialInspector)this.m_MaterialInspector;
				proceduralMaterialInspector.DisableReimportOnDisable();
			}
			return this.m_MaterialInspector;
		}

		public override void OnInspectorGUI()
		{
			if (this.m_SubstanceStyles == null)
			{
				this.m_SubstanceStyles = new SubstanceImporterInspector.SubstanceStyles();
			}
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			this.MaterialListing();
			this.MaterialManagement();
			EditorGUILayout.EndVertical();
			Editor selectedMaterialInspector = this.GetSelectedMaterialInspector();
			if (selectedMaterialInspector)
			{
				selectedMaterialInspector.DrawHeader();
				selectedMaterialInspector.OnInspectorGUI();
			}
		}

		private SubstanceImporter GetImporter()
		{
			return AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.target)) as SubstanceImporter;
		}

		private void MaterialListing()
		{
			ProceduralMaterial[] sortedMaterials = this.GetSortedMaterials();
			ProceduralMaterial[] array = sortedMaterials;
			for (int i = 0; i < array.Length; i++)
			{
				ProceduralMaterial proceduralMaterial = array[i];
				if (proceduralMaterial.isProcessing)
				{
					base.Repaint();
					SceneView.RepaintAll();
					GameView.RepaintAll();
					break;
				}
			}
			int num = sortedMaterials.Length;
			float num2 = GUIView.current.position.width - 16f - 18f - 2f;
			if (num2 * 2f < (float)num * 60f)
			{
				num2 -= 16f;
			}
			int num3 = Mathf.Max(1, Mathf.FloorToInt(num2 / 60f));
			int num4 = Mathf.CeilToInt((float)num / (float)num3);
			Rect viewRect = new Rect(0f, 0f, (float)num3 * 60f, (float)num4 * 76f);
			Rect rect = GUILayoutUtility.GetRect(viewRect.width, Mathf.Clamp(viewRect.height, 76f, 152f) + 1f);
			Rect position = new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 1f);
			GUI.Box(rect, GUIContent.none, this.m_SubstanceStyles.gridBackground);
			GUI.Box(position, GUIContent.none, this.m_SubstanceStyles.background);
			this.m_ListScroll = GUI.BeginScrollView(position, this.m_ListScroll, viewRect, false, false);
			if (this.m_EditorCache == null)
			{
				this.m_EditorCache = new EditorCache(EditorFeatures.PreviewGUI);
			}
			for (int j = 0; j < sortedMaterials.Length; j++)
			{
				ProceduralMaterial proceduralMaterial2 = sortedMaterials[j];
				if (!(proceduralMaterial2 == null))
				{
					float x = (float)(j % num3) * 60f;
					float y = (float)(j / num3) * 76f;
					Rect rect2 = new Rect(x, y, 60f, 76f);
					bool flag = proceduralMaterial2.name == this.m_SelectedMaterialInstanceName;
					Event current = Event.current;
					int controlID = GUIUtility.GetControlID(SubstanceImporterInspector.previewNoDragDropHash, FocusType.Native, rect2);
					EventType typeForControl = current.GetTypeForControl(controlID);
					if (typeForControl != EventType.MouseDown)
					{
						if (typeForControl == EventType.Repaint)
						{
							Rect position2 = rect2;
							position2.y = rect2.yMax - 16f;
							position2.height = 16f;
							this.m_SubstanceStyles.resultsGridLabel.Draw(position2, EditorGUIUtility.TempContent(proceduralMaterial2.name), false, false, flag, flag);
						}
					}
					else if (current.button == 0)
					{
						if (rect2.Contains(current.mousePosition))
						{
							if (current.clickCount == 1)
							{
								this.m_SelectedMaterialInstanceName = proceduralMaterial2.name;
								current.Use();
							}
							else if (current.clickCount == 2)
							{
								AssetDatabase.OpenAsset(proceduralMaterial2);
								GUIUtility.ExitGUI();
								current.Use();
							}
						}
					}
					rect2.height -= 16f;
					EditorWrapper editorWrapper = this.m_EditorCache[proceduralMaterial2];
					editorWrapper.OnPreviewGUI(rect2, this.m_SubstanceStyles.background);
				}
			}
			GUI.EndScrollView();
		}

		public override bool HasPreviewGUI()
		{
			return this.GetSelectedMaterialInspector() != null;
		}

		public override void OnPreviewGUI(Rect position, GUIStyle style)
		{
			Editor selectedMaterialInspector = this.GetSelectedMaterialInspector();
			if (selectedMaterialInspector)
			{
				selectedMaterialInspector.OnPreviewGUI(position, style);
			}
		}

		public override string GetInfoString()
		{
			Editor selectedMaterialInspector = this.GetSelectedMaterialInspector();
			if (selectedMaterialInspector)
			{
				return selectedMaterialInspector.targetTitle + "\n" + selectedMaterialInspector.GetInfoString();
			}
			return string.Empty;
		}

		public override void OnPreviewSettings()
		{
			Editor selectedMaterialInspector = this.GetSelectedMaterialInspector();
			if (selectedMaterialInspector)
			{
				selectedMaterialInspector.OnPreviewSettings();
			}
		}

		public void InstanciatePrototype(object prototypeName)
		{
			this.m_SelectedMaterialInstanceName = this.GetImporter().InstantiateMaterial(prototypeName as string);
			this.ApplyAndRefresh(false);
		}

		private ProceduralMaterial[] GetSortedMaterials()
		{
			SubstanceImporter importer = this.GetImporter();
			ProceduralMaterial[] materials = importer.GetMaterials();
			Array.Sort(materials, new SubstanceImporterInspector.SubstanceNameComparer());
			return materials;
		}

		private void MaterialManagement()
		{
			SubstanceImporter importer = this.GetImporter();
			if (this.m_PrototypeNames == null)
			{
				this.m_PrototypeNames = importer.GetPrototypeNames();
			}
			ProceduralMaterial selectedMaterial = this.GetSelectedMaterial();
			GUILayout.BeginHorizontal(this.m_SubstanceStyles.toolbar, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
			{
				if (this.m_PrototypeNames.Length > 1)
				{
					Rect rect = GUILayoutUtility.GetRect(this.m_SubstanceStyles.iconToolbarPlus, this.m_SubstanceStyles.toolbarDropDown);
					if (EditorGUI.ButtonMouseDown(rect, this.m_SubstanceStyles.iconToolbarPlus, FocusType.Passive, this.m_SubstanceStyles.toolbarDropDown))
					{
						GenericMenu genericMenu = new GenericMenu();
						for (int i = 0; i < this.m_PrototypeNames.Length; i++)
						{
							genericMenu.AddItem(new GUIContent(this.m_PrototypeNames[i]), false, new GenericMenu.MenuFunction2(this.InstanciatePrototype), this.m_PrototypeNames[i]);
						}
						genericMenu.DropDown(rect);
					}
				}
				else if (this.m_PrototypeNames.Length == 1 && GUILayout.Button(this.m_SubstanceStyles.iconToolbarPlus, this.m_SubstanceStyles.toolbarButton, new GUILayoutOption[0]))
				{
					this.m_SelectedMaterialInstanceName = this.GetImporter().InstantiateMaterial(this.m_PrototypeNames[0]);
					this.ApplyAndRefresh(true);
				}
				using (new EditorGUI.DisabledScope(selectedMaterial == null))
				{
					if (GUILayout.Button(this.m_SubstanceStyles.iconToolbarMinus, this.m_SubstanceStyles.toolbarButton, new GUILayoutOption[0]) && this.GetSortedMaterials().Length > 1)
					{
						this.SelectNextMaterial();
						importer.DestroyMaterial(selectedMaterial);
						this.ApplyAndRefresh(true);
					}
					if (GUILayout.Button(this.m_SubstanceStyles.iconDuplicate, this.m_SubstanceStyles.toolbarButton, new GUILayoutOption[0]))
					{
						string text = importer.CloneMaterial(selectedMaterial);
						if (text != string.Empty)
						{
							this.m_SelectedMaterialInstanceName = text;
							this.ApplyAndRefresh(true);
						}
					}
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private void ApplyAndRefresh(bool exitGUI)
		{
			string assetPath = AssetDatabase.GetAssetPath(this.target);
			AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUncompressedImport);
			if (exitGUI)
			{
				GUIUtility.ExitGUI();
			}
			base.Repaint();
		}

		private void Init()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
			}
			if (SubstanceImporterInspector.s_Meshes[0] == null)
			{
				GameObject gameObject = (GameObject)EditorGUIUtility.LoadRequired("Previews/PreviewMaterials.fbx");
				gameObject.SetActive(false);
				foreach (Transform transform in gameObject.transform)
				{
					MeshFilter component = transform.GetComponent<MeshFilter>();
					string name = transform.name;
					switch (name)
					{
					case "sphere":
						SubstanceImporterInspector.s_Meshes[0] = component.sharedMesh;
						continue;
					case "cube":
						SubstanceImporterInspector.s_Meshes[1] = component.sharedMesh;
						continue;
					case "cylinder":
						SubstanceImporterInspector.s_Meshes[2] = component.sharedMesh;
						continue;
					case "torus":
						SubstanceImporterInspector.s_Meshes[3] = component.sharedMesh;
						continue;
					}
					Debug.Log("Something is wrong, weird object found: " + transform.name);
				}
				SubstanceImporterInspector.s_MeshIcons[0] = EditorGUIUtility.IconContent("PreMatSphere");
				SubstanceImporterInspector.s_MeshIcons[1] = EditorGUIUtility.IconContent("PreMatCube");
				SubstanceImporterInspector.s_MeshIcons[2] = EditorGUIUtility.IconContent("PreMatCylinder");
				SubstanceImporterInspector.s_MeshIcons[3] = EditorGUIUtility.IconContent("PreMatTorus");
				SubstanceImporterInspector.s_LightIcons[0] = EditorGUIUtility.IconContent("PreMatLight0");
				SubstanceImporterInspector.s_LightIcons[1] = EditorGUIUtility.IconContent("PreMatLight1");
			}
		}

		public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return null;
			}
			this.Init();
			this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float)width, (float)height));
			this.DoRenderPreview(subAssets);
			return this.m_PreviewUtility.EndStaticPreview();
		}

		protected void DoRenderPreview(UnityEngine.Object[] subAssets)
		{
			if (this.m_PreviewUtility.m_RenderTexture.width <= 0 || this.m_PreviewUtility.m_RenderTexture.height <= 0)
			{
				return;
			}
			List<ProceduralMaterial> list = new List<ProceduralMaterial>();
			for (int i = 0; i < subAssets.Length; i++)
			{
				UnityEngine.Object @object = subAssets[i];
				if (@object is ProceduralMaterial)
				{
					list.Add(@object as ProceduralMaterial);
				}
			}
			int num = 1;
			while (num * num < list.Count)
			{
				num++;
			}
			int num2 = Mathf.CeilToInt((float)list.Count / (float)num);
			this.m_PreviewUtility.m_Camera.transform.position = -Vector3.forward * 5f * (float)num;
			this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.identity;
			this.m_PreviewUtility.m_Camera.farClipPlane = (float)(5 * num) + 5f;
			this.m_PreviewUtility.m_Camera.nearClipPlane = (float)(5 * num) - 3f;
			Color ambient;
			if (this.lightMode == 0)
			{
				this.m_PreviewUtility.m_Light[0].intensity = 1f;
				this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);
				this.m_PreviewUtility.m_Light[1].intensity = 0f;
				ambient = new Color(0.2f, 0.2f, 0.2f, 0f);
			}
			else
			{
				this.m_PreviewUtility.m_Light[0].intensity = 1f;
				this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
				this.m_PreviewUtility.m_Light[1].intensity = 1f;
				ambient = new Color(0.2f, 0.2f, 0.2f, 0f);
			}
			InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
			for (int j = 0; j < list.Count; j++)
			{
				ProceduralMaterial mat = list[j];
				Vector3 vector = new Vector3((float)(j % num) - (float)(num - 1) * 0.5f, (float)(-(float)j / num) + (float)(num2 - 1) * 0.5f, 0f);
				vector *= Mathf.Tan(this.m_PreviewUtility.m_Camera.fieldOfView * 0.5f * 0.0174532924f) * 5f * 2f;
				this.m_PreviewUtility.DrawMesh(SubstanceImporterInspector.s_Meshes[this.selectedMesh], vector, Quaternion.Euler(this.previewDir.y, 0f, 0f) * Quaternion.Euler(0f, this.previewDir.x, 0f), mat, 0);
			}
			bool fog = RenderSettings.fog;
			Unsupported.SetRenderSettingsUseFogNoDirty(false);
			this.m_PreviewUtility.m_Camera.Render();
			Unsupported.SetRenderSettingsUseFogNoDirty(fog);
			InternalEditorUtility.RemoveCustomLighting();
		}
	}
}
