using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
	[CustomEditor(typeof(Avatar))]
	internal class AvatarEditor : Editor
	{
		private class Styles
		{
			public GUIContent[] tabs = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Mapping"),
				EditorGUIUtility.TextContent("Muscles & Settings")
			};

			public GUIContent editCharacter = EditorGUIUtility.TextContent("Configure Avatar");

			public GUIContent reset = EditorGUIUtility.TextContent("Reset");
		}

		private enum EditMode
		{
			NotEditing,
			Starting,
			Editing,
			Stopping
		}

		[Serializable]
		protected class SceneStateCache
		{
			public SceneView view;

			public SceneView.SceneViewState state;
		}

		private static AvatarEditor.Styles s_Styles;

		protected int m_TabIndex;

		internal GameObject m_GameObject;

		internal Dictionary<Transform, bool> m_ModelBones = null;

		private AvatarEditor.EditMode m_EditMode = AvatarEditor.EditMode.NotEditing;

		internal bool m_CameFromImportSettings = false;

		private bool m_SwitchToEditMode = false;

		internal static bool s_EditImmediatelyOnNextOpen = false;

		private SceneSetup[] sceneSetup;

		protected bool m_InspectorLocked;

		protected List<AvatarEditor.SceneStateCache> m_SceneStates;

		private AvatarMuscleEditor m_MuscleEditor;

		private AvatarHandleEditor m_HandleEditor;

		private AvatarColliderEditor m_ColliderEditor;

		private AvatarMappingEditor m_MappingEditor;

		private const int sMappingTab = 0;

		private const int sMuscleTab = 1;

		private const int sHandleTab = 2;

		private const int sColliderTab = 3;

		private const int sDefaultTab = 0;

		private GameObject m_PrefabInstance;

		private static AvatarEditor.Styles styles
		{
			get
			{
				if (AvatarEditor.s_Styles == null)
				{
					AvatarEditor.s_Styles = new AvatarEditor.Styles();
				}
				return AvatarEditor.s_Styles;
			}
		}

		internal Avatar avatar
		{
			get
			{
				return base.target as Avatar;
			}
		}

		protected AvatarSubEditor editor
		{
			get
			{
				AvatarSubEditor result;
				switch (this.m_TabIndex)
				{
				case 1:
					result = this.m_MuscleEditor;
					return result;
				case 2:
					result = this.m_HandleEditor;
					return result;
				case 3:
					result = this.m_ColliderEditor;
					return result;
				}
				result = this.m_MappingEditor;
				return result;
			}
			set
			{
				switch (this.m_TabIndex)
				{
				case 1:
					this.m_MuscleEditor = (value as AvatarMuscleEditor);
					return;
				case 2:
					this.m_HandleEditor = (value as AvatarHandleEditor);
					return;
				case 3:
					this.m_ColliderEditor = (value as AvatarColliderEditor);
					return;
				}
				this.m_MappingEditor = (value as AvatarMappingEditor);
			}
		}

		public GameObject prefab
		{
			get
			{
				string assetPath = AssetDatabase.GetAssetPath(base.target);
				return AssetDatabase.LoadMainAssetAtPath(assetPath) as GameObject;
			}
		}

		internal override SerializedObject GetSerializedObjectInternal()
		{
			if (this.m_SerializedObject == null)
			{
				this.m_SerializedObject = SerializedObject.LoadFromCache(base.GetInstanceID());
			}
			if (this.m_SerializedObject == null)
			{
				this.m_SerializedObject = new SerializedObject(AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(base.target)));
			}
			return this.m_SerializedObject;
		}

		private void OnEnable()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			this.m_SwitchToEditMode = false;
			if (this.m_EditMode == AvatarEditor.EditMode.Editing)
			{
				this.m_ModelBones = AvatarSetupTool.GetModelBones(this.m_GameObject.transform, false, null);
				this.editor.Enable(this);
			}
			else if (this.m_EditMode == AvatarEditor.EditMode.NotEditing)
			{
				this.editor = null;
				if (AvatarEditor.s_EditImmediatelyOnNextOpen)
				{
					this.m_CameFromImportSettings = true;
					AvatarEditor.s_EditImmediatelyOnNextOpen = false;
				}
			}
		}

		private void OnDisable()
		{
			if (this.m_EditMode == AvatarEditor.EditMode.Editing)
			{
				this.editor.Disable();
			}
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			if (this.m_SerializedObject != null)
			{
				this.m_SerializedObject.Cache(base.GetInstanceID());
				this.m_SerializedObject = null;
			}
		}

		private void OnDestroy()
		{
			if (this.m_EditMode == AvatarEditor.EditMode.Editing)
			{
				this.SwitchToAssetMode();
			}
		}

		private void SelectAsset()
		{
			UnityEngine.Object activeObject;
			if (this.m_CameFromImportSettings)
			{
				string assetPath = AssetDatabase.GetAssetPath(base.target);
				activeObject = AssetDatabase.LoadMainAssetAtPath(assetPath);
			}
			else
			{
				activeObject = base.target;
			}
			Selection.activeObject = activeObject;
		}

		protected void CreateEditor()
		{
			switch (this.m_TabIndex)
			{
			case 1:
				this.editor = ScriptableObject.CreateInstance<AvatarMuscleEditor>();
				goto IL_63;
			case 2:
				this.editor = ScriptableObject.CreateInstance<AvatarHandleEditor>();
				goto IL_63;
			case 3:
				this.editor = ScriptableObject.CreateInstance<AvatarColliderEditor>();
				goto IL_63;
			}
			this.editor = ScriptableObject.CreateInstance<AvatarMappingEditor>();
			IL_63:
			this.editor.hideFlags = HideFlags.HideAndDontSave;
			this.editor.Enable(this);
		}

		protected void DestroyEditor()
		{
			this.editor.OnDestroy();
			this.editor = null;
		}

		public override bool UseDefaultMargins()
		{
			return false;
		}

		public override void OnInspectorGUI()
		{
			GUI.enabled = true;
			EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins, new GUILayoutOption[0]);
			if (this.m_EditMode == AvatarEditor.EditMode.Editing)
			{
				this.EditingGUI();
			}
			else if (!this.m_CameFromImportSettings)
			{
				this.EditButtonGUI();
			}
			else if (this.m_EditMode == AvatarEditor.EditMode.NotEditing && Event.current.type == EventType.Repaint)
			{
				this.m_SwitchToEditMode = true;
			}
			EditorGUILayout.EndVertical();
		}

		private void EditButtonGUI()
		{
			if (!(this.avatar == null) && this.avatar.isHuman)
			{
				string assetPath = AssetDatabase.GetAssetPath(this.avatar);
				ModelImporter x = AssetImporter.GetAtPath(assetPath) as ModelImporter;
				if (!(x == null))
				{
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(AvatarEditor.styles.editCharacter, new GUILayoutOption[]
					{
						GUILayout.Width(120f)
					}) && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
					{
						this.SwitchToEditMode();
						GUIUtility.ExitGUI();
					}
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		private void EditingGUI()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			int num = this.m_TabIndex;
			bool enabled = GUI.enabled;
			GUI.enabled = (!(this.avatar == null) && this.avatar.isHuman);
			num = GUILayout.Toolbar(num, AvatarEditor.styles.tabs, new GUILayoutOption[0]);
			GUI.enabled = enabled;
			if (num != this.m_TabIndex)
			{
				this.DestroyEditor();
				if (this.avatar != null && this.avatar.isHuman)
				{
					this.m_TabIndex = num;
				}
				this.CreateEditor();
			}
			GUILayout.EndHorizontal();
			this.editor.OnInspectorGUI();
		}

		public void OnSceneGUI()
		{
			if (this.m_EditMode == AvatarEditor.EditMode.Editing)
			{
				this.editor.OnSceneGUI();
			}
		}

		internal void SwitchToEditMode()
		{
			this.m_EditMode = AvatarEditor.EditMode.Starting;
			this.ChangeInspectorLock(true);
			this.sceneSetup = EditorSceneManager.GetSceneManagerSetup();
			EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects).name = "Avatar Configuration";
			this.m_GameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
			if (base.serializedObject.FindProperty("m_OptimizeGameObjects").boolValue)
			{
				AnimatorUtility.DeoptimizeTransformHierarchy(this.m_GameObject);
			}
			Animator component = this.m_GameObject.GetComponent<Animator>();
			if (component != null && component.runtimeAnimatorController == null)
			{
				AnimatorController animatorController = new AnimatorController();
				animatorController.hideFlags = HideFlags.DontSave;
				animatorController.AddLayer("preview");
				animatorController.layers[0].stateMachine.hideFlags = HideFlags.DontSave;
				component.runtimeAnimatorController = animatorController;
			}
			Dictionary<Transform, bool> modelBones = AvatarSetupTool.GetModelBones(this.m_GameObject.transform, true, null);
			AvatarSetupTool.BoneWrapper[] humanBones = AvatarSetupTool.GetHumanBones(base.serializedObject, modelBones);
			this.m_ModelBones = AvatarSetupTool.GetModelBones(this.m_GameObject.transform, false, humanBones);
			Selection.activeObject = this.m_GameObject;
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow));
			for (int i = 0; i < array.Length; i++)
			{
				SceneHierarchyWindow sceneHierarchyWindow = (SceneHierarchyWindow)array[i];
				sceneHierarchyWindow.SetExpandedRecursive(this.m_GameObject.GetInstanceID(), true);
			}
			this.CreateEditor();
			this.m_EditMode = AvatarEditor.EditMode.Editing;
			this.m_SceneStates = new List<AvatarEditor.SceneStateCache>();
			IEnumerator enumerator = SceneView.sceneViews.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SceneView sceneView = (SceneView)enumerator.Current;
					this.m_SceneStates.Add(new AvatarEditor.SceneStateCache
					{
						state = new SceneView.SceneViewState(sceneView.m_SceneViewState),
						view = sceneView
					});
					sceneView.m_SceneViewState.showFlares = false;
					sceneView.m_SceneViewState.showMaterialUpdate = false;
					sceneView.m_SceneViewState.showFog = false;
					sceneView.m_SceneViewState.showSkybox = false;
					sceneView.m_SceneViewState.showImageEffects = false;
					sceneView.FrameSelected();
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

		internal void SwitchToAssetMode()
		{
			foreach (AvatarEditor.SceneStateCache current in this.m_SceneStates)
			{
				if (!(current.view == null))
				{
					current.view.m_SceneViewState.showFog = current.state.showFog;
					current.view.m_SceneViewState.showFlares = current.state.showFlares;
					current.view.m_SceneViewState.showMaterialUpdate = current.state.showMaterialUpdate;
					current.view.m_SceneViewState.showSkybox = current.state.showSkybox;
					current.view.m_SceneViewState.showImageEffects = current.state.showImageEffects;
				}
			}
			this.m_EditMode = AvatarEditor.EditMode.Stopping;
			this.DestroyEditor();
			this.ChangeInspectorLock(this.m_InspectorLocked);
			if (!EditorApplication.isUpdating && !Unsupported.IsDestroyScriptableObject(this))
			{
				string path = SceneManager.GetActiveScene().path;
				if (path.Length <= 0)
				{
					if (this.sceneSetup != null && this.sceneSetup.Length > 0)
					{
						EditorSceneManager.RestoreSceneManagerSetup(this.sceneSetup);
						this.sceneSetup = null;
					}
					else
					{
						EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
					}
				}
			}
			else if (Unsupported.IsDestroyScriptableObject(this))
			{
				EditorApplication.CallbackFunction CleanUpSceneOnDestroy = null;
				CleanUpSceneOnDestroy = delegate
				{
					string path2 = SceneManager.GetActiveScene().path;
					if (path2.Length <= 0)
					{
						if (this.sceneSetup != null && this.sceneSetup.Length > 0)
						{
							EditorSceneManager.RestoreSceneManagerSetup(this.sceneSetup);
							this.sceneSetup = null;
						}
						else
						{
							EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
						}
					}
					EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, CleanUpSceneOnDestroy);
				};
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, CleanUpSceneOnDestroy);
			}
			this.m_GameObject = null;
			this.m_ModelBones = null;
			this.SelectAsset();
			if (!this.m_CameFromImportSettings)
			{
				this.m_EditMode = AvatarEditor.EditMode.NotEditing;
			}
		}

		private void ChangeInspectorLock(bool locked)
		{
			InspectorWindow[] allInspectorWindows = InspectorWindow.GetAllInspectorWindows();
			for (int i = 0; i < allInspectorWindows.Length; i++)
			{
				InspectorWindow inspectorWindow = allInspectorWindows[i];
				ActiveEditorTracker tracker = inspectorWindow.GetTracker();
				Editor[] activeEditors = tracker.activeEditors;
				for (int j = 0; j < activeEditors.Length; j++)
				{
					Editor x = activeEditors[j];
					if (x == this)
					{
						this.m_InspectorLocked = inspectorWindow.isLocked;
						inspectorWindow.isLocked = locked;
					}
				}
			}
		}

		public void Update()
		{
			if (this.m_SwitchToEditMode)
			{
				this.m_SwitchToEditMode = false;
				this.SwitchToEditMode();
				base.Repaint();
			}
			if (this.m_EditMode == AvatarEditor.EditMode.Editing)
			{
				if (this.m_GameObject == null || this.m_ModelBones == null)
				{
					this.SwitchToAssetMode();
				}
				else if (EditorApplication.isPlaying)
				{
					this.SwitchToAssetMode();
				}
				else if (this.m_ModelBones != null)
				{
					foreach (KeyValuePair<Transform, bool> current in this.m_ModelBones)
					{
						if (current.Key == null)
						{
							this.SwitchToAssetMode();
							break;
						}
					}
				}
			}
		}

		public bool HasFrameBounds()
		{
			bool result;
			if (this.m_ModelBones != null)
			{
				foreach (KeyValuePair<Transform, bool> current in this.m_ModelBones)
				{
					if (current.Key == Selection.activeTransform)
					{
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}

		public Bounds OnGetFrameBounds()
		{
			Transform activeTransform = Selection.activeTransform;
			Bounds result = new Bounds(activeTransform.position, new Vector3(0f, 0f, 0f));
			IEnumerator enumerator = activeTransform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					result.Encapsulate(transform.position);
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
			if (activeTransform.parent)
			{
				result.Encapsulate(activeTransform.parent.position);
			}
			return result;
		}
	}
}
