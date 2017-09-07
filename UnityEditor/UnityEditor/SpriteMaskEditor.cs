using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SpriteMask))]
	internal class SpriteMaskEditor : RendererEditorBase
	{
		private static class Contents
		{
			public static readonly GUIContent spriteLabel = EditorGUIUtility.TextContent("Sprite|The Sprite defining the mask");

			public static readonly GUIContent alphaCutoffLabel = EditorGUIUtility.TextContent("Alpha Cutoff|The minimum alpha value used by the mask to select the area of influence defined over the mask's sprite.");

			public static readonly GUIContent isCustomRangeActive = EditorGUIUtility.TextContent("Custom Range|Mask sprites from front to back sorting values only.");

			public static readonly GUIContent createSpriteMaskUndoString = EditorGUIUtility.TextContent("Create Sprite Mask");

			public static readonly GUIContent newSpriteMaskName = EditorGUIUtility.TextContent("New Sprite Mask");

			public static readonly GUIContent frontLabel = EditorGUIUtility.TextContent("Front");

			public static readonly GUIContent backLabel = EditorGUIUtility.TextContent("Back");
		}

		private SerializedProperty m_Sprite;

		private SerializedProperty m_AlphaCutoff;

		private SerializedProperty m_IsCustomRangeActive;

		private SerializedProperty m_FrontSortingOrder;

		private SerializedProperty m_FrontSortingLayerID;

		private SerializedProperty m_BackSortingOrder;

		private SerializedProperty m_BackSortingLayerID;

		private AnimBool m_ShowCustomRangeValues;

		[MenuItem("GameObject/2D Object/Sprite Mask")]
		private static void CreateSpriteMaskGameObject()
		{
			GameObject gameObject = new GameObject("", new Type[]
			{
				typeof(SpriteMask)
			});
			if (Selection.activeObject is Sprite)
			{
				gameObject.GetComponent<SpriteMask>().sprite = (Sprite)Selection.activeObject;
			}
			else if (Selection.activeObject is Texture2D)
			{
				string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
				if (!string.IsNullOrEmpty(assetPath))
				{
					Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
					if (sprite != null)
					{
						gameObject.GetComponent<SpriteMask>().sprite = sprite;
					}
				}
			}
			else if (Selection.activeObject is GameObject)
			{
				GameObject gameObject2 = (GameObject)Selection.activeObject;
				PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject2);
				if (prefabType != PrefabType.Prefab && prefabType != PrefabType.ModelPrefab)
				{
					GameObjectUtility.SetParentAndAlign(gameObject, gameObject2);
				}
			}
			gameObject.name = GameObjectUtility.GetUniqueNameForSibling(gameObject.transform.parent, SpriteMaskEditor.Contents.newSpriteMaskName.text);
			Undo.RegisterCreatedObjectUndo(gameObject, SpriteMaskEditor.Contents.createSpriteMaskUndoString.text);
			Selection.activeGameObject = gameObject;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Sprite = base.serializedObject.FindProperty("m_Sprite");
			this.m_AlphaCutoff = base.serializedObject.FindProperty("m_MaskAlphaCutoff");
			this.m_IsCustomRangeActive = base.serializedObject.FindProperty("m_IsCustomRangeActive");
			this.m_FrontSortingOrder = base.serializedObject.FindProperty("m_FrontSortingOrder");
			this.m_FrontSortingLayerID = base.serializedObject.FindProperty("m_FrontSortingLayerID");
			this.m_BackSortingOrder = base.serializedObject.FindProperty("m_BackSortingOrder");
			this.m_BackSortingLayerID = base.serializedObject.FindProperty("m_BackSortingLayerID");
			this.m_ShowCustomRangeValues = new AnimBool(this.ShouldShowCustomRangeValues());
			this.m_ShowCustomRangeValues.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Sprite, SpriteMaskEditor.Contents.spriteLabel, new GUILayoutOption[0]);
			EditorGUILayout.Slider(this.m_AlphaCutoff, 0f, 1f, SpriteMaskEditor.Contents.alphaCutoffLabel, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_IsCustomRangeActive, SpriteMaskEditor.Contents.isCustomRangeActive, new GUILayoutOption[0]);
			this.m_ShowCustomRangeValues.target = this.ShouldShowCustomRangeValues();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowCustomRangeValues.faded))
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.Space();
				EditorGUILayout.LabelField(SpriteMaskEditor.Contents.frontLabel, new GUILayoutOption[0]);
				SortingLayerEditorUtility.RenderSortingLayerFields(this.m_FrontSortingOrder, this.m_FrontSortingLayerID);
				EditorGUILayout.Space();
				EditorGUILayout.LabelField(SpriteMaskEditor.Contents.backLabel, new GUILayoutOption[0]);
				SortingLayerEditorUtility.RenderSortingLayerFields(this.m_BackSortingOrder, this.m_BackSortingLayerID);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			base.serializedObject.ApplyModifiedProperties();
		}

		private bool ShouldShowCustomRangeValues()
		{
			return this.m_IsCustomRangeActive.boolValue && !this.m_IsCustomRangeActive.hasMultipleDifferentValues;
		}
	}
}
