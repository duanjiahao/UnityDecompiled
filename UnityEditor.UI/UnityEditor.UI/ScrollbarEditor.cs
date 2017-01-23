using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(Scrollbar), true)]
	public class ScrollbarEditor : SelectableEditor
	{
		private SerializedProperty m_HandleRect;

		private SerializedProperty m_Direction;

		private SerializedProperty m_Value;

		private SerializedProperty m_Size;

		private SerializedProperty m_NumberOfSteps;

		private SerializedProperty m_OnValueChanged;

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_HandleRect = base.serializedObject.FindProperty("m_HandleRect");
			this.m_Direction = base.serializedObject.FindProperty("m_Direction");
			this.m_Value = base.serializedObject.FindProperty("m_Value");
			this.m_Size = base.serializedObject.FindProperty("m_Size");
			this.m_NumberOfSteps = base.serializedObject.FindProperty("m_NumberOfSteps");
			this.m_OnValueChanged = base.serializedObject.FindProperty("m_OnValueChanged");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			base.serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			RectTransform rectTransform = EditorGUILayout.ObjectField("Handle Rect", this.m_HandleRect.objectReferenceValue, typeof(RectTransform), true, new GUILayoutOption[0]) as RectTransform;
			if (EditorGUI.EndChangeCheck())
			{
				List<UnityEngine.Object> list = new List<UnityEngine.Object>();
				list.Add(rectTransform);
				UnityEngine.Object[] targetObjects = this.m_HandleRect.serializedObject.targetObjects;
				for (int i = 0; i < targetObjects.Length; i++)
				{
					UnityEngine.Object @object = targetObjects[i];
					MonoBehaviour monoBehaviour = @object as MonoBehaviour;
					if (!(monoBehaviour == null))
					{
						list.Add(monoBehaviour);
						list.Add(monoBehaviour.GetComponent<RectTransform>());
					}
				}
				Undo.RecordObjects(list.ToArray(), "Change Handle Rect");
				this.m_HandleRect.objectReferenceValue = rectTransform;
			}
			if (this.m_HandleRect.objectReferenceValue != null)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.m_Direction, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					Scrollbar.Direction enumValueIndex = (Scrollbar.Direction)this.m_Direction.enumValueIndex;
					UnityEngine.Object[] targetObjects2 = base.serializedObject.targetObjects;
					for (int j = 0; j < targetObjects2.Length; j++)
					{
						UnityEngine.Object object2 = targetObjects2[j];
						Scrollbar scrollbar = object2 as Scrollbar;
						scrollbar.SetDirection(enumValueIndex, true);
					}
				}
				EditorGUILayout.PropertyField(this.m_Value, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_NumberOfSteps, new GUILayoutOption[0]);
				bool flag = false;
				UnityEngine.Object[] targetObjects3 = base.serializedObject.targetObjects;
				for (int k = 0; k < targetObjects3.Length; k++)
				{
					UnityEngine.Object object3 = targetObjects3[k];
					Scrollbar scrollbar2 = object3 as Scrollbar;
					Scrollbar.Direction direction = scrollbar2.direction;
					if (direction == Scrollbar.Direction.LeftToRight || direction == Scrollbar.Direction.RightToLeft)
					{
						flag = (scrollbar2.navigation.mode != Navigation.Mode.Automatic && (scrollbar2.FindSelectableOnLeft() != null || scrollbar2.FindSelectableOnRight() != null));
					}
					else
					{
						flag = (scrollbar2.navigation.mode != Navigation.Mode.Automatic && (scrollbar2.FindSelectableOnDown() != null || scrollbar2.FindSelectableOnUp() != null));
					}
				}
				if (flag)
				{
					EditorGUILayout.HelpBox("The selected scrollbar direction conflicts with navigation. Not all navigation options may work.", MessageType.Warning);
				}
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(this.m_OnValueChanged, new GUILayoutOption[0]);
			}
			else
			{
				EditorGUILayout.HelpBox("Specify a RectTransform for the scrollbar handle. It must have a parent RectTransform that the handle can slide within.", MessageType.Info);
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
