using System;
using UnityEngine;

namespace UnityEditor
{
	internal class LineRendererCurveEditor
	{
		private class Styles
		{
			public static GUIContent widthMultiplier = EditorGUIUtility.TextContent("Width|The multiplier applied to the curve, describing the width (in world space) along the line.");
		}

		private bool m_Refresh = false;

		private CurveEditor m_Editor = null;

		private CurveEditorSettings m_Settings = new CurveEditorSettings();

		private SerializedProperty m_WidthMultiplier;

		private SerializedProperty m_WidthCurve;

		public void OnEnable(SerializedObject serializedObject)
		{
			this.m_WidthMultiplier = serializedObject.FindProperty("m_Parameters.widthMultiplier");
			this.m_WidthCurve = serializedObject.FindProperty("m_Parameters.widthCurve");
			this.m_Settings.hRangeMin = 0f;
			this.m_Settings.vRangeMin = 0f;
			this.m_Settings.vRangeMax = 1f;
			this.m_Settings.hRangeMax = 1f;
			this.m_Settings.vSlider = false;
			this.m_Settings.hSlider = false;
			TickStyle tickStyle = new TickStyle();
			tickStyle.tickColor.color = new Color(0f, 0f, 0f, 0.15f);
			tickStyle.distLabel = 30;
			this.m_Settings.hTickStyle = tickStyle;
			TickStyle tickStyle2 = new TickStyle();
			tickStyle2.tickColor.color = new Color(0f, 0f, 0f, 0.15f);
			tickStyle2.distLabel = 20;
			this.m_Settings.vTickStyle = tickStyle2;
			this.m_Settings.undoRedoSelection = true;
			this.m_Editor = new CurveEditor(new Rect(0f, 0f, 1000f, 100f), new CurveWrapper[0], false);
			this.m_Editor.settings = this.m_Settings;
			this.m_Editor.margin = 25f;
			this.m_Editor.SetShownHRangeInsideMargins(0f, 1f);
			this.m_Editor.SetShownVRangeInsideMargins(0f, 1f);
			this.m_Editor.ignoreScrollWheelUntilClicked = true;
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		public void OnDisable()
		{
			this.m_Editor.OnDisable();
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		private CurveWrapper GetCurveWrapper(AnimationCurve curve)
		{
			float num = EditorGUIUtility.isProSkin ? 1f : 0.9f;
			Color b = new Color(num, num, num, 1f);
			CurveWrapper curveWrapper = new CurveWrapper();
			curveWrapper.id = 0;
			curveWrapper.groupId = -1;
			curveWrapper.color = new Color(1f, 0f, 0f, 1f) * b;
			curveWrapper.hidden = false;
			curveWrapper.readOnly = false;
			curveWrapper.renderer = new NormalCurveRenderer(curve);
			curveWrapper.renderer.SetCustomRange(0f, 1f);
			curveWrapper.getAxisUiScalarsCallback = new CurveWrapper.GetAxisScalarsCallback(this.GetAxisScalars);
			return curveWrapper;
		}

		public Vector2 GetAxisScalars()
		{
			return new Vector2(1f, this.m_WidthMultiplier.floatValue);
		}

		private void UndoRedoPerformed()
		{
			this.m_Refresh = true;
		}

		public void CheckCurveChangedExternally()
		{
			CurveWrapper curveWrapperFromID = this.m_Editor.GetCurveWrapperFromID(0);
			if (this.m_WidthCurve != null)
			{
				AnimationCurve animationCurveValue = this.m_WidthCurve.animationCurveValue;
				if (curveWrapperFromID == null != this.m_WidthCurve.hasMultipleDifferentValues)
				{
					this.m_Refresh = true;
				}
				else if (curveWrapperFromID != null)
				{
					if (curveWrapperFromID.curve.length == 0)
					{
						this.m_Refresh = true;
					}
					else if (animationCurveValue.length >= 1 && animationCurveValue.keys[0].value != curveWrapperFromID.curve.keys[0].value)
					{
						this.m_Refresh = true;
					}
				}
			}
			else if (curveWrapperFromID != null)
			{
				this.m_Refresh = true;
			}
		}

		public void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_WidthMultiplier, LineRendererCurveEditor.Styles.widthMultiplier, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_Refresh = true;
			}
			Rect aspectRect = GUILayoutUtility.GetAspectRect(2.5f, GUI.skin.textField);
			aspectRect.xMin += EditorGUI.indent;
			if (Event.current.type != EventType.Layout && Event.current.type != EventType.Used)
			{
				this.m_Editor.rect = new Rect(aspectRect.x, aspectRect.y, aspectRect.width, aspectRect.height);
			}
			if (this.m_Refresh)
			{
				this.m_Editor.animationCurves = new CurveWrapper[]
				{
					this.GetCurveWrapper(this.m_WidthCurve.animationCurveValue)
				};
				this.m_Refresh = false;
			}
			GUI.Label(this.m_Editor.drawRect, GUIContent.none, "TextField");
			this.m_Editor.hRangeLocked = Event.current.shift;
			this.m_Editor.vRangeLocked = EditorGUI.actionKey;
			this.m_Editor.OnGUI();
			if (this.m_Editor.GetCurveWrapperFromID(0) != null && this.m_Editor.GetCurveWrapperFromID(0).changed)
			{
				AnimationCurve curve = this.m_Editor.GetCurveWrapperFromID(0).curve;
				if (curve.length > 0)
				{
					this.m_WidthCurve.animationCurveValue = curve;
					this.m_Editor.GetCurveWrapperFromID(0).changed = false;
				}
			}
		}
	}
}
