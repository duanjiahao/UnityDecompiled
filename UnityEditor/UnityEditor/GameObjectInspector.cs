using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(GameObject))]
	internal class GameObjectInspector : Editor
	{
		private class Styles
		{
			public GUIContent goIcon = EditorGUIUtility.IconContent("GameObject Icon");

			public GUIContent typelessIcon = EditorGUIUtility.IconContent("Prefab Icon");

			public GUIContent prefabIcon = EditorGUIUtility.IconContent("PrefabNormal Icon");

			public GUIContent modelIcon = EditorGUIUtility.IconContent("PrefabModel Icon");

			public GUIContent dataTemplateIcon = EditorGUIUtility.IconContent("PrefabNormal Icon");

			public GUIContent dropDownIcon = EditorGUIUtility.IconContent("Icon Dropdown");

			public float staticFieldToggleWidth = EditorStyles.toggle.CalcSize(EditorGUIUtility.TempContent("Static")).x + 6f;

			public float tagFieldWidth = EditorStyles.boldLabel.CalcSize(EditorGUIUtility.TempContent("Tag")).x;

			public float layerFieldWidth = EditorStyles.boldLabel.CalcSize(EditorGUIUtility.TempContent("Layer")).x;

			public float navLayerFieldWidth = EditorStyles.boldLabel.CalcSize(EditorGUIUtility.TempContent("Nav Layer")).x;

			public GUIStyle staticDropdown = "StaticDropdown";

			public GUIStyle instanceManagementInfo = new GUIStyle(EditorStyles.helpBox);

			public GUIContent goTypeLabelMultiple = new GUIContent("Multiple");

			public GUIContent[] goTypeLabel = new GUIContent[]
			{
				null,
				EditorGUIUtility.TextContent("Prefab"),
				EditorGUIUtility.TextContent("Model"),
				EditorGUIUtility.TextContent("Prefab"),
				EditorGUIUtility.TextContent("Model"),
				EditorGUIUtility.TextContent("Missing|The source Prefab or Model has been deleted."),
				EditorGUIUtility.TextContent("Prefab|You have broken the prefab connection. Changes to the prefab will not be applied to this object before you Apply or Revert."),
				EditorGUIUtility.TextContent("Model|You have broken the prefab connection. Changes to the model will not be applied to this object before you Revert.")
			};

			public Styles()
			{
				GUIStyle gUIStyle = "MiniButtonMid";
				this.instanceManagementInfo.padding = gUIStyle.padding;
				this.instanceManagementInfo.alignment = gUIStyle.alignment;
			}
		}

		private const float kTop = 4f;

		private const float kTop2 = 24f;

		private const float kTop3 = 44f;

		private const float kIconSize = 24f;

		private const float kLeft = 52f;

		private const float kToggleSize = 14f;

		private SerializedProperty m_Name;

		private SerializedProperty m_IsActive;

		private SerializedProperty m_Layer;

		private SerializedProperty m_Tag;

		private SerializedProperty m_StaticEditorFlags;

		private SerializedProperty m_Icon;

		private static GameObjectInspector.Styles s_styles;

		private Vector2 previewDir;

		private PreviewRenderUtility m_PreviewUtility;

		private List<GameObject> m_PreviewInstances;

		private bool m_HasInstance;

		private bool m_AllOfSamePrefabType = true;

		public static GameObject dragObject;

		public void OnEnable()
		{
			if (EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D)
			{
				this.previewDir = new Vector2(0f, 0f);
			}
			else
			{
				this.previewDir = new Vector2(120f, -20f);
			}
			this.m_Name = base.serializedObject.FindProperty("m_Name");
			this.m_IsActive = base.serializedObject.FindProperty("m_IsActive");
			this.m_Layer = base.serializedObject.FindProperty("m_Layer");
			this.m_Tag = base.serializedObject.FindProperty("m_TagString");
			this.m_StaticEditorFlags = base.serializedObject.FindProperty("m_StaticEditorFlags");
			this.m_Icon = base.serializedObject.FindProperty("m_Icon");
			this.CalculatePrefabStatus();
		}

		private void CalculatePrefabStatus()
		{
			this.m_HasInstance = false;
			this.m_AllOfSamePrefabType = true;
			PrefabType prefabType = PrefabUtility.GetPrefabType(base.targets[0] as GameObject);
			UnityEngine.Object[] targets = base.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				GameObject target = (GameObject)targets[i];
				PrefabType prefabType2 = PrefabUtility.GetPrefabType(target);
				if (prefabType2 != prefabType)
				{
					this.m_AllOfSamePrefabType = false;
				}
				if (prefabType2 != PrefabType.None && prefabType2 != PrefabType.Prefab && prefabType2 != PrefabType.ModelPrefab)
				{
					this.m_HasInstance = true;
				}
			}
		}

		private void OnDisable()
		{
		}

		private static bool ShowMixedStaticEditorFlags(StaticEditorFlags mask)
		{
			uint num = 0u;
			uint num2 = 0u;
			foreach (object current in Enum.GetValues(typeof(StaticEditorFlags)))
			{
				num2 += 1u;
				if ((mask & (StaticEditorFlags)((int)current)) > (StaticEditorFlags)0)
				{
					num += 1u;
				}
			}
			return num > 0u && num != num2;
		}

		protected override void OnHeaderGUI()
		{
			Rect rect = GUILayoutUtility.GetRect(0f, (float)((!this.m_HasInstance) ? 40 : 60));
			this.DrawInspector(rect);
		}

		public override void OnInspectorGUI()
		{
		}

		internal bool DrawInspector(Rect contentRect)
		{
			if (GameObjectInspector.s_styles == null)
			{
				GameObjectInspector.s_styles = new GameObjectInspector.Styles();
			}
			base.serializedObject.Update();
			GameObject gameObject = this.target as GameObject;
			EditorGUIUtility.labelWidth = 52f;
			bool enabled = GUI.enabled;
			GUI.enabled = true;
			GUI.Label(new Rect(contentRect.x, contentRect.y, contentRect.width, contentRect.height + 3f), GUIContent.none, EditorStyles.inspectorBig);
			GUI.enabled = enabled;
			float width = contentRect.width;
			float y = contentRect.y;
			GUIContent gUIContent = null;
			PrefabType prefabType = PrefabType.None;
			if (this.m_AllOfSamePrefabType)
			{
				prefabType = PrefabUtility.GetPrefabType(gameObject);
				switch (prefabType)
				{
				case PrefabType.None:
					gUIContent = GameObjectInspector.s_styles.goIcon;
					break;
				case PrefabType.Prefab:
				case PrefabType.PrefabInstance:
				case PrefabType.DisconnectedPrefabInstance:
					gUIContent = GameObjectInspector.s_styles.prefabIcon;
					break;
				case PrefabType.ModelPrefab:
				case PrefabType.ModelPrefabInstance:
				case PrefabType.DisconnectedModelPrefabInstance:
					gUIContent = GameObjectInspector.s_styles.modelIcon;
					break;
				case PrefabType.MissingPrefabInstance:
					gUIContent = GameObjectInspector.s_styles.prefabIcon;
					break;
				}
			}
			else
			{
				gUIContent = GameObjectInspector.s_styles.typelessIcon;
			}
			EditorGUI.ObjectIconDropDown(new Rect(3f, 4f + y, 24f, 24f), base.targets, true, gUIContent.image as Texture2D, this.m_Icon);
			using (new EditorGUI.DisabledScope(prefabType == PrefabType.ModelPrefab))
			{
				EditorGUI.PropertyField(new Rect(34f, 4f + y, 14f, 14f), this.m_IsActive, GUIContent.none);
				float num = GameObjectInspector.s_styles.staticFieldToggleWidth + 15f;
				float width2 = width - 52f - num - 5f;
				EditorGUI.DelayedTextField(new Rect(52f, 4f + y + 1f, width2, 16f), this.m_Name, GUIContent.none);
				Rect rect = new Rect(width - num, 4f + y, GameObjectInspector.s_styles.staticFieldToggleWidth, 16f);
				EditorGUI.BeginProperty(rect, GUIContent.none, this.m_StaticEditorFlags);
				EditorGUI.BeginChangeCheck();
				Rect position = rect;
				EditorGUI.showMixedValue |= GameObjectInspector.ShowMixedStaticEditorFlags((StaticEditorFlags)this.m_StaticEditorFlags.intValue);
				Event current = Event.current;
				EventType type = current.type;
				bool flag = current.type == EventType.MouseDown && current.button != 0;
				if (flag)
				{
					current.type = EventType.Ignore;
				}
				bool flagValue = EditorGUI.ToggleLeft(position, "Static", gameObject.isStatic);
				if (flag)
				{
					current.type = type;
				}
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					SceneModeUtility.SetStaticFlags(base.targets, -1, flagValue);
					base.serializedObject.SetIsDifferentCacheDirty();
				}
				EditorGUI.EndProperty();
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = this.m_StaticEditorFlags.hasMultipleDifferentValues;
				int changedFlags;
				bool flagValue2;
				EditorGUI.EnumMaskField(new Rect(rect.x + GameObjectInspector.s_styles.staticFieldToggleWidth, rect.y, 10f, 14f), GameObjectUtility.GetStaticEditorFlags(gameObject), GameObjectInspector.s_styles.staticDropdown, out changedFlags, out flagValue2);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					SceneModeUtility.SetStaticFlags(base.targets, changedFlags, flagValue2);
					base.serializedObject.SetIsDifferentCacheDirty();
				}
				float num2 = 4f;
				float num3 = 4f;
				EditorGUIUtility.fieldWidth = (width - num2 - 52f - GameObjectInspector.s_styles.layerFieldWidth - num3) / 2f;
				string tag = null;
				try
				{
					tag = gameObject.tag;
				}
				catch (Exception)
				{
					tag = "Undefined";
				}
				EditorGUIUtility.labelWidth = GameObjectInspector.s_styles.tagFieldWidth;
				Rect rect2 = new Rect(52f - EditorGUIUtility.labelWidth, 24f + y, EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth, 16f);
				EditorGUI.BeginProperty(rect2, GUIContent.none, this.m_Tag);
				EditorGUI.BeginChangeCheck();
				string text = EditorGUI.TagField(rect2, EditorGUIUtility.TempContent("Tag"), tag);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_Tag.stringValue = text;
					Undo.RecordObjects(base.targets, "Change Tag of " + this.targetTitle);
					UnityEngine.Object[] targets = base.targets;
					for (int i = 0; i < targets.Length; i++)
					{
						UnityEngine.Object @object = targets[i];
						(@object as GameObject).tag = text;
					}
				}
				EditorGUI.EndProperty();
				EditorGUIUtility.labelWidth = GameObjectInspector.s_styles.layerFieldWidth;
				rect2 = new Rect(52f + EditorGUIUtility.fieldWidth + num2, 24f + y, EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth, 16f);
				EditorGUI.BeginProperty(rect2, GUIContent.none, this.m_Layer);
				EditorGUI.BeginChangeCheck();
				int num4 = EditorGUI.LayerField(rect2, EditorGUIUtility.TempContent("Layer"), gameObject.layer);
				if (EditorGUI.EndChangeCheck())
				{
					GameObjectUtility.ShouldIncludeChildren shouldIncludeChildren = GameObjectUtility.DisplayUpdateChildrenDialogIfNeeded(base.targets.OfType<GameObject>(), "Change Layer", "Do you want to set layer to " + InternalEditorUtility.GetLayerName(num4) + " for all child objects as well?");
					if (shouldIncludeChildren != GameObjectUtility.ShouldIncludeChildren.Cancel)
					{
						this.m_Layer.intValue = num4;
						this.SetLayer(num4, shouldIncludeChildren == GameObjectUtility.ShouldIncludeChildren.IncludeChildren);
					}
				}
				EditorGUI.EndProperty();
				if (this.m_HasInstance)
				{
					using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
					{
						float num5 = (width - 52f - 5f) / 3f;
						Rect position2 = new Rect(52f + num5 * 0f, 44f + y, num5, 15f);
						Rect position3 = new Rect(52f + num5 * 1f, 44f + y, num5, 15f);
						Rect position4 = new Rect(52f + num5 * 2f, 44f + y, num5, 15f);
						Rect position5 = new Rect(52f, 44f + y, num5 * 3f, 15f);
						GUIContent gUIContent2 = (base.targets.Length <= 1) ? GameObjectInspector.s_styles.goTypeLabel[(int)prefabType] : GameObjectInspector.s_styles.goTypeLabelMultiple;
						if (gUIContent2 != null)
						{
							float x = GUI.skin.label.CalcSize(gUIContent2).x;
							if (prefabType == PrefabType.DisconnectedModelPrefabInstance || prefabType == PrefabType.MissingPrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
							{
								GUI.contentColor = GUI.skin.GetStyle("CN StatusWarn").normal.textColor;
								if (prefabType == PrefabType.MissingPrefabInstance)
								{
									GUI.Label(new Rect(52f, 44f + y, width - 52f - 5f, 18f), gUIContent2, EditorStyles.whiteLabel);
								}
								else
								{
									GUI.Label(new Rect(52f - x - 5f, 44f + y, width - 52f - 5f, 18f), gUIContent2, EditorStyles.whiteLabel);
								}
								GUI.contentColor = Color.white;
							}
							else
							{
								Rect position6 = new Rect(52f - x - 5f, 44f + y, x, 18f);
								GUI.Label(position6, gUIContent2);
							}
						}
						if (base.targets.Length > 1)
						{
							GUI.Label(position5, "Instance Management Disabled", GameObjectInspector.s_styles.instanceManagementInfo);
						}
						else
						{
							if (prefabType != PrefabType.MissingPrefabInstance && GUI.Button(position2, "Select", "MiniButtonLeft"))
							{
								Selection.activeObject = PrefabUtility.GetPrefabParent(this.target);
								EditorGUIUtility.PingObject(Selection.activeObject);
							}
							if ((prefabType == PrefabType.DisconnectedModelPrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance) && GUI.Button(position3, "Revert", "MiniButtonMid"))
							{
								List<UnityEngine.Object> hierarchy = new List<UnityEngine.Object>();
								this.GetObjectListFromHierarchy(hierarchy, gameObject);
								Undo.RegisterFullObjectHierarchyUndo(gameObject, "Revert to prefab");
								PrefabUtility.ReconnectToLastPrefab(gameObject);
								Undo.RegisterCreatedObjectUndo(PrefabUtility.GetPrefabObject(gameObject), "Revert to prefab");
								PrefabUtility.RevertPrefabInstance(gameObject);
								this.CalculatePrefabStatus();
								List<UnityEngine.Object> list = new List<UnityEngine.Object>();
								this.GetObjectListFromHierarchy(list, gameObject);
								this.RegisterNewComponents(list, hierarchy);
							}
							using (new EditorGUI.DisabledScope(AnimationMode.InAnimationMode()))
							{
								if ((prefabType == PrefabType.ModelPrefabInstance || prefabType == PrefabType.PrefabInstance) && GUI.Button(position3, "Revert", "MiniButtonMid"))
								{
									List<UnityEngine.Object> hierarchy2 = new List<UnityEngine.Object>();
									this.GetObjectListFromHierarchy(hierarchy2, gameObject);
									Undo.RegisterFullObjectHierarchyUndo(gameObject, "Revert Prefab Instance");
									PrefabUtility.RevertPrefabInstance(gameObject);
									this.CalculatePrefabStatus();
									List<UnityEngine.Object> list2 = new List<UnityEngine.Object>();
									this.GetObjectListFromHierarchy(list2, gameObject);
									this.RegisterNewComponents(list2, hierarchy2);
								}
								if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
								{
									GameObject gameObject2 = PrefabUtility.FindValidUploadPrefabInstanceRoot(gameObject);
									GUI.enabled = (gameObject2 != null && !AnimationMode.InAnimationMode());
									if (GUI.Button(position4, "Apply", "MiniButtonRight"))
									{
										UnityEngine.Object prefabParent = PrefabUtility.GetPrefabParent(gameObject2);
										string assetPath = AssetDatabase.GetAssetPath(prefabParent);
										bool flag2 = Provider.PromptAndCheckoutIfNeeded(new string[]
										{
											assetPath
										}, "The version control requires you to check out the prefab before applying changes.");
										if (flag2)
										{
											PrefabUtility.ReplacePrefab(gameObject2, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
											this.CalculatePrefabStatus();
											EditorSceneManager.MarkSceneDirty(gameObject2.scene);
											GUIUtility.ExitGUI();
										}
									}
								}
							}
							if ((prefabType == PrefabType.DisconnectedModelPrefabInstance || prefabType == PrefabType.ModelPrefabInstance) && GUI.Button(position4, "Open", "MiniButtonRight"))
							{
								AssetDatabase.OpenAsset(PrefabUtility.GetPrefabParent(this.target));
								GUIUtility.ExitGUI();
							}
						}
					}
				}
			}
			base.serializedObject.ApplyModifiedProperties();
			return true;
		}

		private void GetObjectListFromHierarchy(List<UnityEngine.Object> hierarchy, GameObject gameObject)
		{
			Transform transform = null;
			List<Component> list = new List<Component>();
			gameObject.GetComponents<Component>(list);
			foreach (Component current in list)
			{
				if (current is Transform)
				{
					transform = (current as Transform);
				}
				else
				{
					hierarchy.Add(current);
				}
			}
			if (transform != null)
			{
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					this.GetObjectListFromHierarchy(hierarchy, transform.GetChild(i).gameObject);
				}
			}
		}

		private void RegisterNewComponents(List<UnityEngine.Object> newHierarchy, List<UnityEngine.Object> hierarchy)
		{
			for (int i = newHierarchy.Count - 1; i >= 0; i--)
			{
				bool flag = false;
				UnityEngine.Object @object = newHierarchy[i];
				for (int j = 0; j < hierarchy.Count; j++)
				{
					if (hierarchy[j].GetInstanceID() == @object.GetInstanceID())
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Undo.RegisterCreatedObjectUndo(newHierarchy[i], "Dangly component");
				}
			}
		}

		private UnityEngine.Object[] GetObjects(bool includeChildren)
		{
			return SceneModeUtility.GetObjects(base.targets, includeChildren);
		}

		private void SetLayer(int layer, bool includeChildren)
		{
			UnityEngine.Object[] objects = this.GetObjects(includeChildren);
			Undo.RecordObjects(objects, "Change Layer of " + this.targetTitle);
			UnityEngine.Object[] array = objects;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = (GameObject)array[i];
				gameObject.layer = layer;
			}
		}

		public static void SetEnabledRecursive(GameObject go, bool enabled)
		{
			Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i];
				renderer.enabled = enabled;
			}
		}

		public override void ReloadPreviewInstances()
		{
			this.CreatePreviewInstances();
		}

		private void CreatePreviewInstances()
		{
			this.DestroyPreviewInstances();
			if (this.m_PreviewInstances == null)
			{
				this.m_PreviewInstances = new List<GameObject>(base.targets.Length);
			}
			for (int i = 0; i < base.targets.Length; i++)
			{
				GameObject gameObject = EditorUtility.InstantiateForAnimatorPreview(base.targets[i]);
				GameObjectInspector.SetEnabledRecursive(gameObject, false);
				this.m_PreviewInstances.Add(gameObject);
			}
		}

		private void DestroyPreviewInstances()
		{
			if (this.m_PreviewInstances == null || this.m_PreviewInstances.Count == 0)
			{
				return;
			}
			foreach (GameObject current in this.m_PreviewInstances)
			{
				UnityEngine.Object.DestroyImmediate(current);
			}
			this.m_PreviewInstances.Clear();
		}

		private void InitPreview()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility(true);
				this.m_PreviewUtility.m_CameraFieldOfView = 30f;
				this.m_PreviewUtility.m_Camera.cullingMask = 1 << Camera.PreviewCullingLayer;
				this.CreatePreviewInstances();
			}
		}

		public void OnDestroy()
		{
			this.DestroyPreviewInstances();
			if (this.m_PreviewUtility != null)
			{
				this.m_PreviewUtility.Cleanup();
				this.m_PreviewUtility = null;
			}
		}

		public static bool HasRenderablePartsRecurse(GameObject go)
		{
			MeshRenderer exists = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
			MeshFilter meshFilter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
			if (exists && meshFilter && meshFilter.sharedMesh)
			{
				return true;
			}
			SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
			if (skinnedMeshRenderer && skinnedMeshRenderer.sharedMesh)
			{
				return true;
			}
			SpriteRenderer spriteRenderer = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
			if (spriteRenderer && spriteRenderer.sprite)
			{
				return true;
			}
			foreach (Transform transform in go.transform)
			{
				if (GameObjectInspector.HasRenderablePartsRecurse(transform.gameObject))
				{
					return true;
				}
			}
			return false;
		}

		public static void GetRenderableBoundsRecurse(ref Bounds bounds, GameObject go)
		{
			MeshRenderer meshRenderer = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
			MeshFilter meshFilter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
			if (meshRenderer && meshFilter && meshFilter.sharedMesh)
			{
				if (bounds.extents == Vector3.zero)
				{
					bounds = meshRenderer.bounds;
				}
				else
				{
					bounds.Encapsulate(meshRenderer.bounds);
				}
			}
			SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
			if (skinnedMeshRenderer && skinnedMeshRenderer.sharedMesh)
			{
				if (bounds.extents == Vector3.zero)
				{
					bounds = skinnedMeshRenderer.bounds;
				}
				else
				{
					bounds.Encapsulate(skinnedMeshRenderer.bounds);
				}
			}
			SpriteRenderer spriteRenderer = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
			if (spriteRenderer && spriteRenderer.sprite)
			{
				if (bounds.extents == Vector3.zero)
				{
					bounds = spriteRenderer.bounds;
				}
				else
				{
					bounds.Encapsulate(spriteRenderer.bounds);
				}
			}
			foreach (Transform transform in go.transform)
			{
				GameObjectInspector.GetRenderableBoundsRecurse(ref bounds, transform.gameObject);
			}
		}

		private static float GetRenderableCenterRecurse(ref Vector3 center, GameObject go, int depth, int minDepth, int maxDepth)
		{
			if (depth > maxDepth)
			{
				return 0f;
			}
			float num = 0f;
			if (depth > minDepth)
			{
				MeshRenderer meshRenderer = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
				MeshFilter x = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
				SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
				SpriteRenderer spriteRenderer = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
				if (meshRenderer == null && x == null && skinnedMeshRenderer == null && spriteRenderer == null)
				{
					num = 1f;
					center += go.transform.position;
				}
				else if (meshRenderer != null && x != null)
				{
					if (Vector3.Distance(meshRenderer.bounds.center, go.transform.position) < 0.01f)
					{
						num = 1f;
						center += go.transform.position;
					}
				}
				else if (skinnedMeshRenderer != null)
				{
					if (Vector3.Distance(skinnedMeshRenderer.bounds.center, go.transform.position) < 0.01f)
					{
						num = 1f;
						center += go.transform.position;
					}
				}
				else if (spriteRenderer != null && Vector3.Distance(spriteRenderer.bounds.center, go.transform.position) < 0.01f)
				{
					num = 1f;
					center += go.transform.position;
				}
			}
			depth++;
			foreach (Transform transform in go.transform)
			{
				num += GameObjectInspector.GetRenderableCenterRecurse(ref center, transform.gameObject, depth, minDepth, maxDepth);
			}
			return num;
		}

		public static Vector3 GetRenderableCenterRecurse(GameObject go, int minDepth, int maxDepth)
		{
			Vector3 vector = Vector3.zero;
			float renderableCenterRecurse = GameObjectInspector.GetRenderableCenterRecurse(ref vector, go, 0, minDepth, maxDepth);
			if (renderableCenterRecurse > 0f)
			{
				vector /= renderableCenterRecurse;
			}
			else
			{
				vector = go.transform.position;
			}
			return vector;
		}

		public override bool HasPreviewGUI()
		{
			return EditorUtility.IsPersistent(this.target) && this.HasStaticPreview();
		}

		private bool HasStaticPreview()
		{
			if (base.targets.Length > 1)
			{
				return true;
			}
			if (this.target == null)
			{
				return false;
			}
			GameObject gameObject = this.target as GameObject;
			Camera exists = gameObject.GetComponent(typeof(Camera)) as Camera;
			return exists || GameObjectInspector.HasRenderablePartsRecurse(gameObject);
		}

		public override void OnPreviewSettings()
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return;
			}
			GUI.enabled = true;
			this.InitPreview();
		}

		private void DoRenderPreview()
		{
			GameObject gameObject = this.m_PreviewInstances[this.referenceTargetIndex];
			Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);
			GameObjectInspector.GetRenderableBoundsRecurse(ref bounds, gameObject);
			float num = Mathf.Max(bounds.extents.magnitude, 0.0001f);
			float num2 = num * 3.8f;
			Quaternion quaternion = Quaternion.Euler(-this.previewDir.y, -this.previewDir.x, 0f);
			Vector3 position = bounds.center - quaternion * (Vector3.forward * num2);
			this.m_PreviewUtility.m_Camera.transform.position = position;
			this.m_PreviewUtility.m_Camera.transform.rotation = quaternion;
			this.m_PreviewUtility.m_Camera.nearClipPlane = num2 - num * 1.1f;
			this.m_PreviewUtility.m_Camera.farClipPlane = num2 + num * 1.1f;
			this.m_PreviewUtility.m_Light[0].intensity = 0.7f;
			this.m_PreviewUtility.m_Light[0].transform.rotation = quaternion * Quaternion.Euler(40f, 40f, 0f);
			this.m_PreviewUtility.m_Light[1].intensity = 0.7f;
			this.m_PreviewUtility.m_Light[1].transform.rotation = quaternion * Quaternion.Euler(340f, 218f, 177f);
			Color ambient = new Color(0.1f, 0.1f, 0.1f, 0f);
			InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
			bool fog = RenderSettings.fog;
			Unsupported.SetRenderSettingsUseFogNoDirty(false);
			GameObjectInspector.SetEnabledRecursive(gameObject, true);
			this.m_PreviewUtility.m_Camera.Render();
			GameObjectInspector.SetEnabledRecursive(gameObject, false);
			Unsupported.SetRenderSettingsUseFogNoDirty(fog);
			InternalEditorUtility.RemoveCustomLighting();
		}

		public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
		{
			if (!this.HasStaticPreview() || !ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return null;
			}
			this.InitPreview();
			this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float)width, (float)height));
			this.DoRenderPreview();
			return this.m_PreviewUtility.EndStaticPreview();
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Preview requires\nrender texture support");
				}
				return;
			}
			this.InitPreview();
			this.previewDir = PreviewGUI.Drag2D(this.previewDir, r);
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			this.m_PreviewUtility.BeginPreview(r, background);
			this.DoRenderPreview();
			this.m_PreviewUtility.EndAndDrawPreview(r);
		}

		public void OnSceneDrag(SceneView sceneView)
		{
			GameObject gameObject = this.target as GameObject;
			PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
			if (prefabType != PrefabType.Prefab && prefabType != PrefabType.ModelPrefab)
			{
				return;
			}
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.DragUpdated)
			{
				if (type != EventType.DragPerform)
				{
					if (type == EventType.DragExited)
					{
						if (GameObjectInspector.dragObject)
						{
							UnityEngine.Object.DestroyImmediate(GameObjectInspector.dragObject, false);
							HandleUtility.ignoreRaySnapObjects = null;
							GameObjectInspector.dragObject = null;
							current.Use();
						}
					}
				}
				else
				{
					string uniqueNameForSibling = GameObjectUtility.GetUniqueNameForSibling(null, GameObjectInspector.dragObject.name);
					GameObjectInspector.dragObject.hideFlags = HideFlags.None;
					Undo.RegisterCreatedObjectUndo(GameObjectInspector.dragObject, "Place " + GameObjectInspector.dragObject.name);
					EditorUtility.SetDirty(GameObjectInspector.dragObject);
					DragAndDrop.AcceptDrag();
					Selection.activeObject = GameObjectInspector.dragObject;
					HandleUtility.ignoreRaySnapObjects = null;
					EditorWindow.mouseOverWindow.Focus();
					GameObjectInspector.dragObject.name = uniqueNameForSibling;
					GameObjectInspector.dragObject = null;
					current.Use();
				}
			}
			else
			{
				if (GameObjectInspector.dragObject == null)
				{
					GameObjectInspector.dragObject = (GameObject)PrefabUtility.InstantiatePrefab(PrefabUtility.FindPrefabRoot(gameObject));
					HandleUtility.ignoreRaySnapObjects = GameObjectInspector.dragObject.GetComponentsInChildren<Transform>();
					GameObjectInspector.dragObject.hideFlags = HideFlags.HideInHierarchy;
					GameObjectInspector.dragObject.name = gameObject.name;
				}
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				object obj = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(current.mousePosition));
				if (obj != null)
				{
					RaycastHit raycastHit = (RaycastHit)obj;
					float d = 0f;
					if (Tools.pivotMode == PivotMode.Center)
					{
						float num = HandleUtility.CalcRayPlaceOffset(HandleUtility.ignoreRaySnapObjects, raycastHit.normal);
						if (num != float.PositiveInfinity)
						{
							d = Vector3.Dot(GameObjectInspector.dragObject.transform.position, raycastHit.normal) - num;
						}
					}
					GameObjectInspector.dragObject.transform.position = Matrix4x4.identity.MultiplyPoint(raycastHit.point + raycastHit.normal * d);
				}
				else
				{
					GameObjectInspector.dragObject.transform.position = HandleUtility.GUIPointToWorldRay(current.mousePosition).GetPoint(10f);
				}
				if (sceneView.in2DMode)
				{
					Vector3 position = GameObjectInspector.dragObject.transform.position;
					position.z = PrefabUtility.FindPrefabRoot(gameObject).transform.position.z;
					GameObjectInspector.dragObject.transform.position = position;
				}
				current.Use();
			}
		}
	}
}
