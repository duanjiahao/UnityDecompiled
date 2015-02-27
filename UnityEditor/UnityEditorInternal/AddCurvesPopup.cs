using System;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class AddCurvesPopup : EditorWindow
	{
		private const float k_WindowPadding = 3f;
		internal static AnimationWindowState s_State;
		private static AddCurvesPopup s_AddCurvesPopup;
		private static long s_LastClosedTime;
		private static AddCurvesPopupHierarchy s_Hierarchy;
		private static Vector2 windowSize = new Vector2(240f, 250f);
		internal static UnityEngine.Object animatableObject
		{
			get;
			set;
		}
		internal static GameObject gameObject
		{
			get;
			set;
		}
		internal static string path
		{
			get
			{
				return AnimationUtility.CalculateTransformPath(AddCurvesPopup.gameObject.transform, AddCurvesPopup.s_State.m_RootGameObject.transform);
			}
		}
		private void Init(Rect buttonRect)
		{
			buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
			base.ShowAsDropDown(buttonRect, AddCurvesPopup.windowSize, new PopupLocationHelper.PopupLocation[]
			{
				PopupLocationHelper.PopupLocation.Right
			});
		}
		private void OnDisable()
		{
			AddCurvesPopup.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
			AddCurvesPopup.s_AddCurvesPopup = null;
			AddCurvesPopup.s_Hierarchy = null;
		}
		internal static void AddNewCurve(AddCurvesPopupPropertyNode node)
		{
			if (!AnimationWindow.EnsureAllHaveClips())
			{
				return;
			}
			AnimationWindowUtility.CreateDefaultCurves(AddCurvesPopup.s_State, node.curveBindings);
			TreeViewItem treeViewItem = (!(node.parent.displayName == "GameObject")) ? node.parent.parent : node.parent;
			AddCurvesPopup.s_State.m_hierarchyState.selectedIDs.Clear();
			AddCurvesPopup.s_State.m_hierarchyState.selectedIDs.Add(treeViewItem.id);
			AddCurvesPopup.s_State.m_HierarchyData.SetExpanded(treeViewItem, true);
			AddCurvesPopup.s_State.m_HierarchyData.SetExpanded(node.parent.id, true);
			AddCurvesPopup.s_State.m_CurveEditorIsDirty = true;
		}
		internal static bool ShowAtPosition(Rect buttonRect, AnimationWindowState state)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num >= AddCurvesPopup.s_LastClosedTime + 50L)
			{
				Event.current.Use();
				if (AddCurvesPopup.s_AddCurvesPopup == null)
				{
					AddCurvesPopup.s_AddCurvesPopup = ScriptableObject.CreateInstance<AddCurvesPopup>();
				}
				AddCurvesPopup.s_State = state;
				AddCurvesPopup.s_AddCurvesPopup.Init(buttonRect);
				return true;
			}
			return false;
		}
		internal void OnGUI()
		{
			if (Event.current.type == EventType.Layout)
			{
				return;
			}
			if (AddCurvesPopup.s_Hierarchy == null)
			{
				AddCurvesPopup.s_Hierarchy = new AddCurvesPopupHierarchy(AddCurvesPopup.s_State);
			}
			Rect position = new Rect(1f, 1f, AddCurvesPopup.windowSize.x - 3f, AddCurvesPopup.windowSize.y - 3f);
			GUI.Box(new Rect(0f, 0f, AddCurvesPopup.windowSize.x, AddCurvesPopup.windowSize.y), GUIContent.none, new GUIStyle("grey_border"));
			AddCurvesPopup.s_Hierarchy.OnGUI(position, this);
		}
	}
}
