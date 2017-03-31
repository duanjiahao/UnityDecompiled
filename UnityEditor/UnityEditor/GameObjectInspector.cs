using System;
using System.Collections;
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

			public GUIContent staticContent = EditorGUIUtility.TextContent("Static");

			public float tagFieldWidth = EditorStyles.boldLabel.CalcSize(EditorGUIUtility.TempContent("Tag")).x;

			public float layerFieldWidth = EditorStyles.boldLabel.CalcSize(EditorGUIUtility.TempContent("Layer")).x;

			public GUIStyle staticDropdown = "StaticDropdown";

			public GUIStyle header = new GUIStyle("IN GameObjectHeader");

			public GUIStyle layerPopup = new GUIStyle(EditorStyles.popup);

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
				this.layerPopup.margin.right = 0;
				this.header.padding.bottom -= 3;
			}
		}

		private SerializedProperty m_Name;

		private SerializedProperty m_IsActive;

		private SerializedProperty m_Layer;

		private SerializedProperty m_Tag;

		private SerializedProperty m_StaticEditorFlags;

		private SerializedProperty m_Icon;

		private static GameObjectInspector.Styles s_Styles;

		private const float kIconSize = 24f;

		private Vector2 previewDir;

		private PreviewRenderUtility m_PreviewUtility;

		private List<GameObject> m_PreviewInstances;

		private bool m_HasInstance = false;

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
			IEnumerator enumerator = Enum.GetValues(typeof(StaticEditorFlags)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					num2 += 1u;
					if ((mask & (StaticEditorFlags)current) > (StaticEditorFlags)0)
					{
						num += 1u;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return num > 0u && num != num2;
		}

		protected override void OnHeaderGUI()
		{
			if (GameObjectInspector.s_Styles == null)
			{
				GameObjectInspector.s_Styles = new GameObjectInspector.Styles();
			}
			bool enabled = GUI.enabled;
			GUI.enabled = true;
			EditorGUILayout.BeginVertical(GameObjectInspector.s_Styles.header, new GUILayoutOption[0]);
			GUI.enabled = enabled;
			this.DrawInspector();
			EditorGUILayout.EndVertical();
		}

		public override void OnInspectorGUI()
		{
		}

		internal bool DrawInspector()
		{
			base.serializedObject.Update();
			GameObject gameObject = base.target as GameObject;
			GUIContent gUIContent = null;
			PrefabType prefabType = PrefabType.None;
			if (this.m_AllOfSamePrefabType)
			{
				prefabType = PrefabUtility.GetPrefabType(gameObject);
				switch (prefabType)
				{
				case PrefabType.None:
					gUIContent = GameObjectInspector.s_Styles.goIcon;
					break;
				case PrefabType.Prefab:
				case PrefabType.PrefabInstance:
				case PrefabType.DisconnectedPrefabInstance:
					gUIContent = GameObjectInspector.s_Styles.prefabIcon;
					break;
				case PrefabType.ModelPrefab:
				case PrefabType.ModelPrefabInstance:
				case PrefabType.DisconnectedModelPrefabInstance:
					gUIContent = GameObjectInspector.s_Styles.modelIcon;
					break;
				case PrefabType.MissingPrefabInstance:
					gUIContent = GameObjectInspector.s_Styles.prefabIcon;
					break;
				}
			}
			else
			{
				gUIContent = GameObjectInspector.s_Styles.typelessIcon;
			}
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUI.ObjectIconDropDown(GUILayoutUtility.GetRect(24f, 24f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}), base.targets, true, gUIContent.image as Texture2D, this.m_Icon);
			using (new EditorGUI.DisabledScope(prefabType == PrefabType.ModelPrefab))
			{
				EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[]
				{
					GUILayout.Width(GameObjectInspector.s_Styles.tagFieldWidth)
				});
				GUILayout.FlexibleSpace();
				EditorGUI.PropertyField(GUILayoutUtility.GetRect((float)EditorStyles.toggle.padding.left, EditorGUIUtility.singleLineHeight, EditorStyles.toggle, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}), this.m_IsActive, GUIContent.none);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.DelayedTextField(this.m_Name, GUIContent.none, new GUILayoutOption[0]);
				this.DoStaticToggleField(gameObject);
				this.DoStaticFlagsDropDown(gameObject);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.DoTagsField(gameObject);
				this.DoLayerField(gameObject);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(2f);
			using (new EditorGUI.DisabledScope(prefabType == PrefabType.ModelPrefab))
			{
				this.DoPrefabButtons(prefabType, gameObject);
			}
			base.serializedObject.ApplyModifiedProperties();
			return true;
		}

		private void DoPrefabButtons(PrefabType prefabType, GameObject go)
		{
			if (this.m_HasInstance)
			{
				using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
				{
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUIContent gUIContent = (base.targets.Length <= 1) ? GameObjectInspector.s_Styles.goTypeLabel[(int)prefabType] : GameObjectInspector.s_Styles.goTypeLabelMultiple;
					if (gUIContent != null)
					{
						EditorGUILayout.BeginHorizontal(new GUILayoutOption[]
						{
							GUILayout.Width(24f + GameObjectInspector.s_Styles.tagFieldWidth)
						});
						GUILayout.FlexibleSpace();
						if (prefabType == PrefabType.DisconnectedModelPrefabInstance || prefabType == PrefabType.MissingPrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
						{
							GUI.contentColor = GUI.skin.GetStyle("CN StatusWarn").normal.textColor;
							GUILayout.Label(gUIContent, EditorStyles.whiteLabel, new GUILayoutOption[]
							{
								GUILayout.ExpandWidth(false)
							});
							GUI.contentColor = Color.white;
						}
						else
						{
							GUILayout.Label(gUIContent, new GUILayoutOption[]
							{
								GUILayout.ExpandWidth(false)
							});
						}
						EditorGUILayout.EndHorizontal();
					}
					if (base.targets.Length > 1)
					{
						GUILayout.Label("Instance Management Disabled", GameObjectInspector.s_Styles.instanceManagementInfo, new GUILayoutOption[0]);
					}
					else
					{
						if (prefabType != PrefabType.MissingPrefabInstance)
						{
							if (GUILayout.Button("Select", "MiniButtonLeft", new GUILayoutOption[0]))
							{
								Selection.activeObject = PrefabUtility.GetPrefabParent(base.target);
								EditorGUIUtility.PingObject(Selection.activeObject);
							}
						}
						if (prefabType == PrefabType.DisconnectedModelPrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
						{
							if (GUILayout.Button("Revert", "MiniButtonMid", new GUILayoutOption[0]))
							{
								List<UnityEngine.Object> hierarchy = new List<UnityEngine.Object>();
								this.GetObjectListFromHierarchy(hierarchy, go);
								Undo.RegisterFullObjectHierarchyUndo(go, "Revert to prefab");
								PrefabUtility.ReconnectToLastPrefab(go);
								Undo.RegisterCreatedObjectUndo(PrefabUtility.GetPrefabObject(go), "Revert to prefab");
								PrefabUtility.RevertPrefabInstance(go);
								this.CalculatePrefabStatus();
								List<UnityEngine.Object> list = new List<UnityEngine.Object>();
								this.GetObjectListFromHierarchy(list, go);
								this.RegisterNewComponents(list, hierarchy);
							}
						}
						using (new EditorGUI.DisabledScope(AnimationMode.InAnimationMode()))
						{
							if (prefabType == PrefabType.ModelPrefabInstance || prefabType == PrefabType.PrefabInstance)
							{
								if (GUILayout.Button("Revert", "MiniButtonMid", new GUILayoutOption[0]))
								{
									this.RevertAndCheckForNewComponents(go);
								}
							}
							if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
							{
								GameObject gameObject = PrefabUtility.FindValidUploadPrefabInstanceRoot(go);
								GUI.enabled = (gameObject != null && !AnimationMode.InAnimationMode());
								if (GUILayout.Button("Apply", "MiniButtonRight", new GUILayoutOption[0]))
								{
									UnityEngine.Object prefabParent = PrefabUtility.GetPrefabParent(gameObject);
									string assetPath = AssetDatabase.GetAssetPath(prefabParent);
									bool flag = Provider.PromptAndCheckoutIfNeeded(new string[]
									{
										assetPath
									}, "The version control requires you to check out the prefab before applying changes.");
									if (flag)
									{
										PrefabUtility.ReplacePrefab(gameObject, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
										this.CalculatePrefabStatus();
										EditorSceneManager.MarkSceneDirty(gameObject.scene);
										GUIUtility.ExitGUI();
									}
								}
							}
						}
						if (prefabType == PrefabType.DisconnectedModelPrefabInstance || prefabType == PrefabType.ModelPrefabInstance)
						{
							if (GUILayout.Button("Open", "MiniButtonRight", new GUILayoutOption[0]))
							{
								AssetDatabase.OpenAsset(PrefabUtility.GetPrefabParent(base.target));
								GUIUtility.ExitGUI();
							}
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		public void RevertAndCheckForNewComponents(GameObject gameObject)
		{
			List<UnityEngine.Object> hierarchy = new List<UnityEngine.Object>();
			this.GetObjectListFromHierarchy(hierarchy, gameObject);
			Undo.RegisterFullObjectHierarchyUndo(gameObject, "Revert Prefab Instance");
			PrefabUtility.RevertPrefabInstance(gameObject);
			this.CalculatePrefabStatus();
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			this.GetObjectListFromHierarchy(list, gameObject);
			this.RegisterNewComponents(list, hierarchy);
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
			List<Component> list = new List<Component>();
			foreach (UnityEngine.Object current in newHierarchy)
			{
				bool flag = false;
				foreach (UnityEngine.Object current2 in hierarchy)
				{
					if (current2.GetInstanceID() == current.GetInstanceID())
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(current as Component);
				}
			}
			HashSet<Type> hashSet = new HashSet<Type>
			{
				typeof(Transform)
			};
			bool flag2 = false;
			while (list.Count > 0 && !flag2)
			{
				flag2 = true;
				for (int i = 0; i < list.Count; i++)
				{
					Component component = list[i];
					object[] customAttributes = component.GetType().GetCustomAttributes(typeof(RequireComponent), true);
					bool flag3 = true;
					object[] array = customAttributes;
					for (int j = 0; j < array.Length; j++)
					{
						RequireComponent requireComponent = (RequireComponent)array[j];
						if ((requireComponent.m_Type0 != null && !hashSet.Contains(requireComponent.m_Type0)) || (requireComponent.m_Type1 != null && !hashSet.Contains(requireComponent.m_Type1)) || (requireComponent.m_Type2 != null && !hashSet.Contains(requireComponent.m_Type2)))
						{
							flag3 = false;
							break;
						}
					}
					if (flag3)
					{
						Undo.RegisterCreatedObjectUndo(component, "Dangling component");
						hashSet.Add(component.GetType());
						list.RemoveAt(i);
						i--;
						flag2 = false;
					}
				}
			}
			foreach (Component current3 in list)
			{
				Undo.RegisterCreatedObjectUndo(current3, "Dangling component");
			}
		}

		private void DoLayerField(GameObject go)
		{
			EditorGUIUtility.labelWidth = GameObjectInspector.s_Styles.layerFieldWidth;
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GameObjectInspector.s_Styles.layerPopup);
			EditorGUI.BeginProperty(rect, GUIContent.none, this.m_Layer);
			EditorGUI.BeginChangeCheck();
			int num = EditorGUI.LayerField(rect, EditorGUIUtility.TempContent("Layer"), go.layer, GameObjectInspector.s_Styles.layerPopup);
			if (EditorGUI.EndChangeCheck())
			{
				GameObjectUtility.ShouldIncludeChildren shouldIncludeChildren = GameObjectUtility.DisplayUpdateChildrenDialogIfNeeded(base.targets.OfType<GameObject>(), "Change Layer", "Do you want to set layer to " + InternalEditorUtility.GetLayerName(num) + " for all child objects as well?");
				if (shouldIncludeChildren != GameObjectUtility.ShouldIncludeChildren.Cancel)
				{
					this.m_Layer.intValue = num;
					this.SetLayer(num, shouldIncludeChildren == GameObjectUtility.ShouldIncludeChildren.IncludeChildren);
				}
			}
			EditorGUI.EndProperty();
		}

		private void DoTagsField(GameObject go)
		{
			string tag = null;
			try
			{
				tag = go.tag;
			}
			catch (Exception)
			{
				tag = "Undefined";
			}
			EditorGUIUtility.labelWidth = GameObjectInspector.s_Styles.tagFieldWidth;
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.popup);
			EditorGUI.BeginProperty(rect, GUIContent.none, this.m_Tag);
			EditorGUI.BeginChangeCheck();
			string text = EditorGUI.TagField(rect, EditorGUIUtility.TempContent("Tag"), tag);
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
		}

		private void DoStaticFlagsDropDown(GameObject go)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_StaticEditorFlags.hasMultipleDifferentValues;
			int changedFlags;
			bool flagValue;
			EditorGUI.EnumMaskField(GUILayoutUtility.GetRect(GUIContent.none, GameObjectInspector.s_Styles.staticDropdown, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}), GameObjectUtility.GetStaticEditorFlags(go), GameObjectInspector.s_Styles.staticDropdown, out changedFlags, out flagValue);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				SceneModeUtility.SetStaticFlags(base.targets, changedFlags, flagValue);
				base.serializedObject.SetIsDifferentCacheDirty();
			}
		}

		private void DoStaticToggleField(GameObject go)
		{
			Rect rect = GUILayoutUtility.GetRect(GameObjectInspector.s_Styles.staticContent, EditorStyles.toggle, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
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
			bool flagValue = EditorGUI.ToggleLeft(position, GameObjectInspector.s_Styles.staticContent, go.isStatic);
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
			if (this.m_PreviewInstances != null && this.m_PreviewInstances.Count != 0)
			{
				foreach (GameObject current in this.m_PreviewInstances)
				{
					UnityEngine.Object.DestroyImmediate(current);
				}
				this.m_PreviewInstances.Clear();
			}
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

		public static bool HasRenderableParts(GameObject go)
		{
			MeshRenderer[] componentsInChildren = go.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = componentsInChildren;
			bool result;
			for (int i = 0; i < array.Length; i++)
			{
				MeshRenderer meshRenderer = array[i];
				MeshFilter component = meshRenderer.gameObject.GetComponent<MeshFilter>();
				if (component && component.sharedMesh)
				{
					result = true;
					return result;
				}
			}
			SkinnedMeshRenderer[] componentsInChildren2 = go.GetComponentsInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] array2 = componentsInChildren2;
			for (int j = 0; j < array2.Length; j++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = array2[j];
				if (skinnedMeshRenderer.sharedMesh)
				{
					result = true;
					return result;
				}
			}
			SpriteRenderer[] componentsInChildren3 = go.GetComponentsInChildren<SpriteRenderer>();
			SpriteRenderer[] array3 = componentsInChildren3;
			for (int k = 0; k < array3.Length; k++)
			{
				SpriteRenderer spriteRenderer = array3[k];
				if (spriteRenderer.sprite)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
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
			IEnumerator enumerator = go.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					GameObjectInspector.GetRenderableBoundsRecurse(ref bounds, transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		private static float GetRenderableCenterRecurse(ref Vector3 center, GameObject go, int depth, int minDepth, int maxDepth)
		{
			float result;
			if (depth > maxDepth)
			{
				result = 0f;
			}
			else
			{
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
					else if (spriteRenderer != null)
					{
						if (Vector3.Distance(spriteRenderer.bounds.center, go.transform.position) < 0.01f)
						{
							num = 1f;
							center += go.transform.position;
						}
					}
				}
				depth++;
				IEnumerator enumerator = go.transform.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Transform transform = (Transform)enumerator.Current;
						num += GameObjectInspector.GetRenderableCenterRecurse(ref center, transform.gameObject, depth, minDepth, maxDepth);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				result = num;
			}
			return result;
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
			return EditorUtility.IsPersistent(base.target) && this.HasStaticPreview();
		}

		private bool HasStaticPreview()
		{
			bool result;
			if (base.targets.Length > 1)
			{
				result = true;
			}
			else if (base.target == null)
			{
				result = false;
			}
			else
			{
				GameObject gameObject = base.target as GameObject;
				Camera exists = gameObject.GetComponent(typeof(Camera)) as Camera;
				result = (exists || GameObjectInspector.HasRenderableParts(gameObject));
			}
			return result;
		}

		public override void OnPreviewSettings()
		{
			if (ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				GUI.enabled = true;
				this.InitPreview();
			}
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
			Texture2D result;
			if (!this.HasStaticPreview() || !ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				result = null;
			}
			else
			{
				this.InitPreview();
				this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float)width, (float)height));
				this.DoRenderPreview();
				result = this.m_PreviewUtility.EndStaticPreview();
			}
			return result;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Preview requires\nrender texture support");
				}
			}
			else
			{
				this.InitPreview();
				this.previewDir = PreviewGUI.Drag2D(this.previewDir, r);
				if (Event.current.type == EventType.Repaint)
				{
					this.m_PreviewUtility.BeginPreview(r, background);
					this.DoRenderPreview();
					this.m_PreviewUtility.EndAndDrawPreview(r);
				}
			}
		}

		public void OnSceneDrag(SceneView sceneView)
		{
			GameObject gameObject = base.target as GameObject;
			PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
			if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab)
			{
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
						GameObjectInspector.dragObject.hideFlags = HideFlags.HideInHierarchy;
						GameObjectInspector.dragObject.name = gameObject.name;
					}
					if (HandleUtility.ignoreRaySnapObjects == null)
					{
						HandleUtility.ignoreRaySnapObjects = GameObjectInspector.dragObject.GetComponentsInChildren<Transform>();
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
}
