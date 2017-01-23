using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AddCurvesPopup : EditorWindow
	{
		public delegate void OnNewCurveAdded(AddCurvesPopupPropertyNode node);

		private const float k_WindowPadding = 3f;

		internal static IAnimationRecordingState s_State;

		private static AddCurvesPopup s_AddCurvesPopup;

		private static long s_LastClosedTime;

		private static AddCurvesPopupHierarchy s_Hierarchy;

		private static Vector2 windowSize = new Vector2(240f, 250f);

		private static AddCurvesPopup.OnNewCurveAdded NewCurveAddedCallback;

		internal static AnimationWindowSelection selection
		{
			get;
			set;
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
			AnimationWindowUtility.CreateDefaultCurves(AddCurvesPopup.s_State, node.selectionItem, node.curveBindings);
			if (AddCurvesPopup.NewCurveAddedCallback != null)
			{
				AddCurvesPopup.NewCurveAddedCallback(node);
			}
		}

		internal static bool ShowAtPosition(Rect buttonRect, IAnimationRecordingState state, AddCurvesPopup.OnNewCurveAdded newCurveCallback)
		{
			long num = DateTime.Now.Ticks / 10000L;
			bool result;
			if (num >= AddCurvesPopup.s_LastClosedTime + 50L)
			{
				Event.current.Use();
				if (AddCurvesPopup.s_AddCurvesPopup == null)
				{
					AddCurvesPopup.s_AddCurvesPopup = ScriptableObject.CreateInstance<AddCurvesPopup>();
				}
				AddCurvesPopup.NewCurveAddedCallback = newCurveCallback;
				AddCurvesPopup.s_State = state;
				AddCurvesPopup.s_AddCurvesPopup.Init(buttonRect);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal void OnGUI()
		{
			if (Event.current.type != EventType.Layout)
			{
				if (AddCurvesPopup.s_Hierarchy == null)
				{
					AddCurvesPopup.s_Hierarchy = new AddCurvesPopupHierarchy();
				}
				Rect position = new Rect(1f, 1f, AddCurvesPopup.windowSize.x - 3f, AddCurvesPopup.windowSize.y - 3f);
				GUI.Box(new Rect(0f, 0f, AddCurvesPopup.windowSize.x, AddCurvesPopup.windowSize.y), GUIContent.none, new GUIStyle("grey_border"));
				AddCurvesPopup.s_Hierarchy.OnGUI(position, this);
			}
		}
	}
}
