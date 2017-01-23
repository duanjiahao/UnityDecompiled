using System;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(MaskableGraphic), false)]
	public class GraphicEditor : Editor
	{
		protected SerializedProperty m_Script;

		protected SerializedProperty m_Color;

		protected SerializedProperty m_Material;

		protected SerializedProperty m_RaycastTarget;

		private GUIContent m_CorrectButtonContent;

		protected AnimBool m_ShowNativeSize;

		protected virtual void OnDisable()
		{
			Tools.hidden = false;
			this.m_ShowNativeSize.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		protected virtual void OnEnable()
		{
			this.m_CorrectButtonContent = new GUIContent("Set Native Size", "Sets the size to match the content.");
			this.m_Script = base.serializedObject.FindProperty("m_Script");
			this.m_Color = base.serializedObject.FindProperty("m_Color");
			this.m_Material = base.serializedObject.FindProperty("m_Material");
			this.m_RaycastTarget = base.serializedObject.FindProperty("m_RaycastTarget");
			this.m_ShowNativeSize = new AnimBool(false);
			this.m_ShowNativeSize.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Script, new GUILayoutOption[0]);
			this.AppearanceControlsGUI();
			this.RaycastControlsGUI();
			base.serializedObject.ApplyModifiedProperties();
		}

		protected void SetShowNativeSize(bool show, bool instant)
		{
			if (instant)
			{
				this.m_ShowNativeSize.value = show;
			}
			else
			{
				this.m_ShowNativeSize.target = show;
			}
		}

		protected void NativeSizeButtonGUI()
		{
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowNativeSize.faded))
			{
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(EditorGUIUtility.labelWidth);
				if (GUILayout.Button(this.m_CorrectButtonContent, EditorStyles.miniButton, new GUILayoutOption[0]))
				{
					foreach (Graphic current in from obj in base.targets
					select obj as Graphic)
					{
						Undo.RecordObject(current.rectTransform, "Set Native Size");
						current.SetNativeSize();
						EditorUtility.SetDirty(current);
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndFadeGroup();
		}

		protected void AppearanceControlsGUI()
		{
			EditorGUILayout.PropertyField(this.m_Color, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
		}

		protected void RaycastControlsGUI()
		{
			EditorGUILayout.PropertyField(this.m_RaycastTarget, new GUILayoutOption[0]);
		}
	}
}
