using System;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Effector2D), true)]
	internal class Effector2DEditor : Editor
	{
		private SerializedProperty m_UseColliderMask;

		private SerializedProperty m_ColliderMask;

		private readonly AnimBool m_ShowColliderMask = new AnimBool();

		public virtual void OnEnable()
		{
			this.m_UseColliderMask = base.serializedObject.FindProperty("m_UseColliderMask");
			this.m_ColliderMask = base.serializedObject.FindProperty("m_ColliderMask");
			this.m_ShowColliderMask.value = (base.target as Effector2D).useColliderMask;
			this.m_ShowColliderMask.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		public virtual void OnDisable()
		{
			this.m_ShowColliderMask.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			Effector2D effector2D = base.target as Effector2D;
			this.m_ShowColliderMask.target = effector2D.useColliderMask;
			EditorGUILayout.PropertyField(this.m_UseColliderMask, new GUILayoutOption[0]);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowColliderMask.faded))
			{
				EditorGUILayout.PropertyField(this.m_ColliderMask, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			base.serializedObject.ApplyModifiedProperties();
			if (!effector2D.GetComponents<Collider2D>().Any((Collider2D collider) => collider.enabled && collider.usedByEffector))
			{
				if (effector2D.requiresCollider)
				{
					EditorGUILayout.HelpBox("This effector will not function until there is at least one enabled 2D collider with 'Used by Effector' checked on this GameObject.", MessageType.Warning);
				}
				else
				{
					EditorGUILayout.HelpBox("This effector can optionally work without a 2D collider.", MessageType.Info);
				}
			}
		}

		public static void CheckEffectorWarnings(Collider2D collider)
		{
			if (collider.usedByEffector && !collider.usedByComposite)
			{
				Effector2D component = collider.GetComponent<Effector2D>();
				if (component == null || !component.enabled)
				{
					EditorGUILayout.HelpBox("This collider will not function with an effector until there is at least one enabled 2D effector on this GameObject.", MessageType.Warning);
					if (component == null)
					{
						return;
					}
				}
				if (component.designedForNonTrigger && collider.isTrigger)
				{
					EditorGUILayout.HelpBox("This collider has 'Is Trigger' checked but this should be unchecked when used with the '" + component.GetType().Name + "' component which is designed to work with collisions.", MessageType.Warning);
				}
				else if (component.designedForTrigger && !collider.isTrigger)
				{
					EditorGUILayout.HelpBox("This collider has 'Is Trigger' unchecked but this should be checked when used with the '" + component.GetType().Name + "' component which is designed to work with triggers.", MessageType.Warning);
				}
			}
		}
	}
}
