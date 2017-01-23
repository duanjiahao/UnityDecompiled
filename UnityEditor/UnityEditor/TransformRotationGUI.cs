using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class TransformRotationGUI
	{
		private GUIContent rotationContent = new GUIContent("Rotation", "The local rotation of this Game Object relative to the parent.");

		private Vector3 m_EulerAngles;

		private Vector3 m_OldEulerAngles = new Vector3(1000000f, 1E+07f, 1000000f);

		private RotationOrder m_OldRotationOrder = RotationOrder.OrderZXY;

		private SerializedProperty m_Rotation;

		private UnityEngine.Object[] targets;

		private static int s_FoldoutHash = "Foldout".GetHashCode();

		public void OnEnable(SerializedProperty m_Rotation, GUIContent label)
		{
			this.m_Rotation = m_Rotation;
			this.targets = m_Rotation.serializedObject.targetObjects;
			this.m_OldRotationOrder = (this.targets[0] as Transform).rotationOrder;
			this.rotationContent = label;
		}

		public void RotationField()
		{
			this.RotationField(false);
		}

		public void RotationField(bool disabled)
		{
			Transform transform = this.targets[0] as Transform;
			Vector3 localEulerAngles = transform.GetLocalEulerAngles(transform.rotationOrder);
			if (this.m_OldEulerAngles.x != localEulerAngles.x || this.m_OldEulerAngles.y != localEulerAngles.y || this.m_OldEulerAngles.z != localEulerAngles.z || this.m_OldRotationOrder != transform.rotationOrder)
			{
				this.m_EulerAngles = transform.GetLocalEulerAngles(transform.rotationOrder);
				this.m_OldRotationOrder = transform.rotationOrder;
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 1; i < this.targets.Length; i++)
			{
				Transform transform2 = this.targets[i] as Transform;
				Vector3 localEulerAngles2 = transform2.GetLocalEulerAngles(transform2.rotationOrder);
				flag |= (localEulerAngles2.x != localEulerAngles.x || localEulerAngles2.y != localEulerAngles.y || localEulerAngles2.z != localEulerAngles.z);
				flag2 |= (transform2.rotationOrder != transform.rotationOrder);
			}
			Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * (float)((!EditorGUIUtility.wideMode) ? 2 : 1), new GUILayoutOption[0]);
			GUIContent gUIContent = EditorGUI.BeginProperty(rect, this.rotationContent, this.m_Rotation);
			EditorGUI.showMixedValue = flag;
			EditorGUI.BeginChangeCheck();
			int controlID = GUIUtility.GetControlID(TransformRotationGUI.s_FoldoutHash, FocusType.Keyboard, rect);
			if (AnimationMode.InAnimationMode() && transform.rotationOrder != RotationOrder.OrderZXY)
			{
				string text;
				if (flag2)
				{
					text = "Mixed";
				}
				else
				{
					text = transform.rotationOrder.ToString();
					text = text.Substring(text.Length - 3);
				}
				gUIContent.text = gUIContent.text + " (" + text + ")";
			}
			rect = EditorGUI.MultiFieldPrefixLabel(rect, controlID, gUIContent, 3);
			rect.height = EditorGUIUtility.singleLineHeight;
			using (new EditorGUI.DisabledScope(disabled))
			{
				this.m_EulerAngles = EditorGUI.Vector3Field(rect, GUIContent.none, this.m_EulerAngles);
			}
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObjects(this.targets, "Inspector");
				UnityEngine.Object[] array = this.targets;
				for (int j = 0; j < array.Length; j++)
				{
					Transform transform3 = (Transform)array[j];
					transform3.SetLocalEulerAngles(this.m_EulerAngles, transform3.rotationOrder);
					if (transform3.parent != null)
					{
						transform3.SendTransformChangedScale();
					}
				}
				this.m_Rotation.serializedObject.SetIsDifferentCacheDirty();
			}
			EditorGUI.showMixedValue = false;
			if (flag2)
			{
				EditorGUILayout.HelpBox("Transforms have different rotation orders, keyframes saved will have the same value but not the same local rotation", MessageType.Warning);
			}
			EditorGUI.EndProperty();
		}
	}
}
