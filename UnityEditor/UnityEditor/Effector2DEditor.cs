using System;
using System.Linq;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Effector2D), true)]
	internal class Effector2DEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			Effector2D effector2D = this.target as Effector2D;
			if (effector2D.GetComponents<Collider2D>().Any((Collider2D collider) => collider.enabled && collider.usedByEffector))
			{
				return;
			}
			if (effector2D.requiresCollider)
			{
				EditorGUILayout.HelpBox("This effector will not function until there is at least one enabled 2D collider with 'Used by Effector' checked on this GameObject.", MessageType.Warning);
			}
			else
			{
				EditorGUILayout.HelpBox("This effector can optionally work without a 2D collider.", MessageType.Info);
			}
		}
		public static void CheckEffectorWarnings(Collider2D collider)
		{
			if (!collider.usedByEffector)
			{
				return;
			}
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
			else
			{
				if (component.designedForTrigger && !collider.isTrigger)
				{
					EditorGUILayout.HelpBox("This collider has 'Is Trigger' unchecked but this should be checked when used with the '" + component.GetType().Name + "' component which is designed to work with triggers.", MessageType.Warning);
				}
			}
		}
	}
}
