using System;

namespace UnityEditor
{
	public enum GizmoType
	{
		Pickable = 1,
		NotInSelectionHierarchy,
		NonSelected = 32,
		Selected = 4,
		Active = 8,
		InSelectionHierarchy = 16,
		[Obsolete("Use NotInSelectionHierarchy instead (UnityUpgradable) -> NotInSelectionHierarchy")]
		NotSelected = -127,
		[Obsolete("Use InSelectionHierarchy instead (UnityUpgradable) -> InSelectionHierarchy")]
		SelectedOrChild = -127
	}
}
