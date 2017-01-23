using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class CurveEditor : TimeArea, CurveUpdater
	{
		public delegate void CallbackFunction();

		private class SavedCurve
		{
			public class SavedKeyFrame : IComparable
			{
				public Keyframe key;

				public CurveWrapper.SelectionMode selected;

				public SavedKeyFrame(Keyframe key, CurveWrapper.SelectionMode selected)
				{
					this.key = key;
					this.selected = selected;
				}

				public int CompareTo(object _other)
				{
					CurveEditor.SavedCurve.SavedKeyFrame savedKeyFrame = (CurveEditor.SavedCurve.SavedKeyFrame)_other;
					float num = this.key.time - savedKeyFrame.key.time;
					return (num >= 0f) ? ((num <= 0f) ? 0 : 1) : -1;
				}

				public CurveEditor.SavedCurve.SavedKeyFrame Clone()
				{
					return new CurveEditor.SavedCurve.SavedKeyFrame(this.key, this.selected);
				}
			}

			public delegate CurveEditor.SavedCurve.SavedKeyFrame KeyFrameOperation(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve);

			public int curveId;

			public List<CurveEditor.SavedCurve.SavedKeyFrame> keys;
		}

		private enum AxisLock
		{
			None,
			X,
			Y
		}

		private struct KeyFrameCopy
		{
			public float time;

			public float value;

			public float inTangent;

			public float outTangent;

			public int idx;

			public int selectionIdx;

			public KeyFrameCopy(int idx, int selectionIdx, Keyframe source)
			{
				this.idx = idx;
				this.selectionIdx = selectionIdx;
				this.time = source.time;
				this.value = source.value;
				this.inTangent = source.inTangent;
				this.outTangent = source.outTangent;
			}
		}

		internal new class Styles
		{
			public Texture2D pointIcon = EditorGUIUtility.LoadIcon("curvekeyframe");

			public Texture2D pointIconSelected = EditorGUIUtility.LoadIcon("curvekeyframeselected");

			public Texture2D pointIconSelectedOverlay = EditorGUIUtility.LoadIcon("curvekeyframeselectedoverlay");

			public Texture2D pointIconSemiSelectedOverlay = EditorGUIUtility.LoadIcon("curvekeyframesemiselectedoverlay");

			public GUIContent wrapModeMenuIcon = EditorGUIUtility.IconContent("AnimationWrapModeMenu");

			public GUIStyle none = new GUIStyle();

			public GUIStyle labelTickMarksY = "CurveEditorLabelTickMarks";

			public GUIStyle labelTickMarksX;

			public GUIStyle selectionRect = "SelectionRect";

			public GUIStyle dragLabel = "ProfilerBadge";

			public GUIStyle axisLabelNumberField = new GUIStyle(EditorStyles.miniTextField);

			public Styles()
			{
				this.axisLabelNumberField.alignment = TextAnchor.UpperRight;
				this.labelTickMarksY.contentOffset = Vector2.zero;
				this.labelTickMarksX = new GUIStyle(this.labelTickMarksY);
				this.labelTickMarksX.clipping = TextClipping.Overflow;
			}
		}

		internal enum PickMode
		{
			None,
			Click,
			Marquee
		}

		private CurveWrapper[] m_AnimationCurves;

		private static int s_SelectKeyHash = "SelectKeys".GetHashCode();

		public CurveEditor.CallbackFunction curvesUpdated;

		private List<int> m_DrawOrder = new List<int>();

		public ICurveEditorState state;

		internal Bounds m_DefaultBounds = new Bounds(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 1f, 0f));

		private CurveEditorSettings m_Settings = new CurveEditorSettings();

		private Color m_TangentColor = new Color(1f, 1f, 1f, 0.5f);

		public float invSnap = 0f;

		private CurveMenuManager m_MenuManager;

		private static int s_TangentControlIDHash = "s_TangentControlIDHash".GetHashCode();

		[SerializeField]
		private CurveEditorSelection m_Selection;

		private List<CurveSelection> preCurveDragSelection = null;

		private CurveSelection m_SelectedTangentPoint;

		private List<CurveSelection> s_SelectionBackup;

		private float s_TimeRangeSelectionStart;

		private float s_TimeRangeSelectionEnd;

		private bool s_TimeRangeSelectionActive = false;

		private bool m_BoundsAreDirty = true;

		private Bounds m_SelectionBounds = new Bounds(Vector3.zero, Vector3.zero);

		private Bounds m_CurveBounds = new Bounds(Vector3.zero, Vector3.zero);

		private Bounds m_DrawingBounds = new Bounds(Vector3.zero, Vector3.zero);

		private List<CurveEditor.SavedCurve> m_CurveBackups;

		private CurveWrapper m_DraggingKey = null;

		private Vector2 m_DraggedCoord;

		private Vector2 m_MoveCoord;

		private Vector2 m_PreviousDrawPointCenter;

		private CurveEditor.AxisLock m_AxisLock;

		private CurveControlPointRenderer m_PointRenderer;

		private CurveEditorRectangleTool m_RectangleTool;

		private const float kMaxPickDistSqr = 100f;

		private const float kExactPickDistSqr = 16f;

		private const float kCurveTimeEpsilon = 1E-05f;

		internal CurveEditor.Styles ms_Styles;

		private float s_StartClickedTime;

		private Vector2 s_StartMouseDragPosition;

		private Vector2 s_EndMouseDragPosition;

		private Vector2 s_StartKeyDragPosition;

		private CurveEditor.PickMode s_PickMode;

		private string m_AxisLabelFormat = "n1";

		private Vector2 pointEditingFieldPosition;

		private const string kPointValueFieldName = "pointValueField";

		private const string kPointTimeFieldName = "pointTimeField";

		private bool timeWasEdited;

		private bool valueWasEdited;

		private string focusedPointField = null;

		private CurveWrapper[] m_DraggingCurveOrRegion = null;

		public CurveWrapper[] animationCurves
		{
			get
			{
				if (this.m_AnimationCurves == null)
				{
					this.m_AnimationCurves = new CurveWrapper[0];
				}
				return this.m_AnimationCurves;
			}
			set
			{
				if (this.m_AnimationCurves == null)
				{
					this.m_AnimationCurves = new CurveWrapper[0];
				}
				this.m_AnimationCurves = value;
				for (int i = 0; i < this.m_AnimationCurves.Length; i++)
				{
					this.m_AnimationCurves[i].listIndex = i;
				}
				this.SyncDrawOrder();
				this.SyncSelection();
				this.ValidateCurveList();
			}
		}

		public bool syncTimeDuringDrag
		{
			get
			{
				return this.state == null || this.state.syncTimeDuringDrag;
			}
		}

		public float activeTime
		{
			set
			{
				if (this.state != null)
				{
					this.state.currentTime = value;
				}
			}
		}

		public TimeArea.TimeFormat timeFormat
		{
			get
			{
				TimeArea.TimeFormat result;
				if (this.state != null)
				{
					result = this.state.timeFormat;
				}
				else
				{
					result = TimeArea.TimeFormat.None;
				}
				return result;
			}
		}

		public CurveEditorSettings settings
		{
			get
			{
				return this.m_Settings;
			}
			set
			{
				if (value != null)
				{
					this.m_Settings = value;
					this.ApplySettings();
				}
			}
		}

		public Color tangentColor
		{
			get
			{
				return this.m_TangentColor;
			}
			set
			{
				this.m_TangentColor = value;
			}
		}

		internal CurveEditorSelection selection
		{
			get
			{
				if (this.m_Selection == null)
				{
					this.m_Selection = ScriptableObject.CreateInstance<CurveEditorSelection>();
					this.m_Selection.hideFlags = HideFlags.HideAndDontSave;
				}
				return this.m_Selection;
			}
		}

		internal List<CurveSelection> selectedCurves
		{
			get
			{
				return this.selection.selectedCurves;
			}
			set
			{
				this.selection.selectedCurves = value;
			}
		}

		public bool hasSelection
		{
			get
			{
				return this.selectedCurves.Count != 0;
			}
		}

		public Bounds selectionBounds
		{
			get
			{
				this.RecalculateSelectionBounds();
				return this.m_SelectionBounds;
			}
		}

		public Bounds curveBounds
		{
			get
			{
				this.RecalculateBounds();
				return this.m_CurveBounds;
			}
		}

		public override Bounds drawingBounds
		{
			get
			{
				this.RecalculateBounds();
				return this.m_DrawingBounds;
			}
		}

		internal CurveEditor.Styles styles
		{
			get
			{
				if (this.ms_Styles == null)
				{
					this.ms_Styles = new CurveEditor.Styles();
				}
				return this.ms_Styles;
			}
		}

		private bool editingPoints
		{
			get;
			set;
		}

		public CurveEditor(Rect rect, CurveWrapper[] curves, bool minimalGUI) : base(minimalGUI)
		{
			base.rect = rect;
			this.animationCurves = curves;
			float[] tickModulos = new float[]
			{
				1E-07f,
				5E-07f,
				1E-06f,
				5E-06f,
				1E-05f,
				5E-05f,
				0.0001f,
				0.0005f,
				0.001f,
				0.005f,
				0.01f,
				0.05f,
				0.1f,
				0.5f,
				1f,
				5f,
				10f,
				50f,
				100f,
				500f,
				1000f,
				5000f,
				10000f,
				50000f,
				100000f,
				500000f,
				1000000f,
				5000000f,
				1E+07f
			};
			base.hTicks = new TickHandler();
			base.hTicks.SetTickModulos(tickModulos);
			base.vTicks = new TickHandler();
			base.vTicks.SetTickModulos(tickModulos);
			base.margin = 40f;
			this.OnEnable();
		}

		public bool GetTopMostCurveID(out int curveID)
		{
			bool result;
			if (this.m_DrawOrder.Count > 0)
			{
				curveID = this.m_DrawOrder[this.m_DrawOrder.Count - 1];
				result = true;
			}
			else
			{
				curveID = -1;
				result = false;
			}
			return result;
		}

		private void SyncDrawOrder()
		{
			if (this.m_DrawOrder.Count == 0)
			{
				this.m_DrawOrder = (from cw in this.m_AnimationCurves
				select cw.id).ToList<int>();
			}
			else
			{
				List<int> list = new List<int>(this.m_AnimationCurves.Length);
				for (int i = 0; i < this.m_DrawOrder.Count; i++)
				{
					int num = this.m_DrawOrder[i];
					for (int j = 0; j < this.m_AnimationCurves.Length; j++)
					{
						if (this.m_AnimationCurves[j].id == num)
						{
							list.Add(num);
							break;
						}
					}
				}
				this.m_DrawOrder = list;
				if (this.m_DrawOrder.Count != this.m_AnimationCurves.Length)
				{
					for (int k = 0; k < this.m_AnimationCurves.Length; k++)
					{
						int id = this.m_AnimationCurves[k].id;
						bool flag = false;
						for (int l = 0; l < this.m_DrawOrder.Count; l++)
						{
							if (this.m_DrawOrder[l] == id)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							this.m_DrawOrder.Add(id);
						}
					}
					if (this.m_DrawOrder.Count != this.m_AnimationCurves.Length)
					{
						this.m_DrawOrder = (from cw in this.m_AnimationCurves
						select cw.id).ToList<int>();
					}
				}
			}
		}

		private Matrix4x4 TimeOffsetMatrix(CurveWrapper cw)
		{
			return Matrix4x4.TRS(new Vector3(cw.timeOffset * this.m_Scale.x, 0f, 0f), Quaternion.identity, Vector3.one);
		}

		private Matrix4x4 DrawingToOffsetViewMatrix(CurveWrapper cw)
		{
			return this.TimeOffsetMatrix(cw) * base.drawingToViewMatrix;
		}

		private Vector2 DrawingToOffsetViewTransformPoint(CurveWrapper cw, Vector2 lhs)
		{
			return new Vector2(lhs.x * this.m_Scale.x + this.m_Translation.x + cw.timeOffset * this.m_Scale.x, lhs.y * this.m_Scale.y + this.m_Translation.y);
		}

		private Vector3 DrawingToOffsetViewTransformPoint(CurveWrapper cw, Vector3 lhs)
		{
			return new Vector3(lhs.x * this.m_Scale.x + this.m_Translation.x + cw.timeOffset * this.m_Scale.x, lhs.y * this.m_Scale.y + this.m_Translation.y, 0f);
		}

		private Vector2 OffsetViewToDrawingTransformPoint(CurveWrapper cw, Vector2 lhs)
		{
			return new Vector2((lhs.x - this.m_Translation.x - cw.timeOffset * this.m_Scale.x) / this.m_Scale.x, (lhs.y - this.m_Translation.y) / this.m_Scale.y);
		}

		private Vector3 OffsetViewToDrawingTransformPoint(CurveWrapper cw, Vector3 lhs)
		{
			return new Vector3((lhs.x - this.m_Translation.x - cw.timeOffset * this.m_Scale.x) / this.m_Scale.x, (lhs.y - this.m_Translation.y) / this.m_Scale.y, 0f);
		}

		private Vector2 OffsetMousePositionInDrawing(CurveWrapper cw)
		{
			return this.OffsetViewToDrawingTransformPoint(cw, Event.current.mousePosition);
		}

		protected void ApplySettings()
		{
			base.hRangeLocked = this.settings.hRangeLocked;
			base.vRangeLocked = this.settings.vRangeLocked;
			base.hRangeMin = this.settings.hRangeMin;
			base.hRangeMax = this.settings.hRangeMax;
			base.vRangeMin = this.settings.vRangeMin;
			base.vRangeMax = this.settings.vRangeMax;
			base.scaleWithWindow = this.settings.scaleWithWindow;
			base.hSlider = this.settings.hSlider;
			base.vSlider = this.settings.vSlider;
			this.RecalculateBounds();
		}

		internal void AddSelection(CurveSelection curveSelection)
		{
			this.selectedCurves.Add(curveSelection);
		}

		internal void RemoveSelection(CurveSelection curveSelection)
		{
			this.selectedCurves.Remove(curveSelection);
		}

		internal void ClearSelection()
		{
			this.selectedCurves.Clear();
		}

		internal CurveWrapper GetCurveWrapperFromID(int curveID)
		{
			CurveWrapper result;
			if (this.m_AnimationCurves == null)
			{
				result = null;
			}
			else
			{
				CurveWrapper[] animationCurves = this.m_AnimationCurves;
				for (int i = 0; i < animationCurves.Length; i++)
				{
					CurveWrapper curveWrapper = animationCurves[i];
					if (curveWrapper.id == curveID)
					{
						result = curveWrapper;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		internal CurveWrapper GetCurveWrapperFromSelection(CurveSelection curveSelection)
		{
			return this.GetCurveWrapperFromID(curveSelection.curveID);
		}

		internal AnimationCurve GetCurveFromSelection(CurveSelection curveSelection)
		{
			CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(curveSelection);
			return (curveWrapperFromSelection == null) ? null : curveWrapperFromSelection.curve;
		}

		internal Keyframe GetKeyframeFromSelection(CurveSelection curveSelection)
		{
			AnimationCurve curveFromSelection = this.GetCurveFromSelection(curveSelection);
			Keyframe result;
			if (curveFromSelection != null)
			{
				if (curveSelection.key >= 0 && curveSelection.key < curveFromSelection.length)
				{
					result = curveFromSelection[curveSelection.key];
					return result;
				}
			}
			result = default(Keyframe);
			return result;
		}

		public void OnEnable()
		{
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		public void OnDisable()
		{
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			if (this.m_PointRenderer != null)
			{
				this.m_PointRenderer.FlushCache();
			}
		}

		public void OnDestroy()
		{
			if (this.m_Selection != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Selection);
			}
		}

		private void UndoRedoPerformed()
		{
			if (!this.settings.undoRedoSelection)
			{
				this.SelectNone();
			}
		}

		private void ValidateCurveList()
		{
			for (int i = 0; i < this.m_AnimationCurves.Length; i++)
			{
				CurveWrapper curveWrapper = this.m_AnimationCurves[i];
				int regionId = curveWrapper.regionId;
				if (regionId >= 0)
				{
					if (i == this.m_AnimationCurves.Length - 1)
					{
						Debug.LogError("Region has only one curve last! Regions should be added as two curves after each other with same regionId");
					}
					else
					{
						CurveWrapper curveWrapper2 = this.m_AnimationCurves[++i];
						int regionId2 = curveWrapper2.regionId;
						if (regionId == regionId2)
						{
							goto IL_98;
						}
						Debug.LogError(string.Concat(new object[]
						{
							"Regions should be added as two curves after each other with same regionId: ",
							regionId,
							" != ",
							regionId2
						}));
					}
					return;
				}
				IL_98:;
			}
			if (this.m_DrawOrder.Count != this.m_AnimationCurves.Length)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"DrawOrder and AnimationCurves mismatch: DrawOrder ",
					this.m_DrawOrder.Count,
					", AnimationCurves: ",
					this.m_AnimationCurves.Length
				}));
				return;
			}
			int count = this.m_DrawOrder.Count;
			for (int j = 0; j < count; j++)
			{
				int curveID = this.m_DrawOrder[j];
				int regionId3 = this.GetCurveWrapperFromID(curveID).regionId;
				if (regionId3 >= 0)
				{
					if (j == count - 1)
					{
						Debug.LogError("Region has only one curve last! Regions should be added as two curves after each other with same regionId");
						break;
					}
					int curveID2 = this.m_DrawOrder[++j];
					int regionId4 = this.GetCurveWrapperFromID(curveID2).regionId;
					if (regionId3 != regionId4)
					{
						Debug.LogError(string.Concat(new object[]
						{
							"DrawOrder: Regions not added correctly after each other. RegionIds: ",
							regionId3,
							" , ",
							regionId4
						}));
						break;
					}
				}
			}
		}

		private void UpdateTangentsFromSelection()
		{
			foreach (CurveSelection current in this.selectedCurves)
			{
				AnimationCurve curveFromSelection = this.GetCurveFromSelection(current);
				if (curveFromSelection != null)
				{
					AnimationUtility.UpdateTangentsFromModeSurrounding(curveFromSelection, current.key);
				}
			}
		}

		private void SyncSelection()
		{
			this.Init();
			List<CurveSelection> list = new List<CurveSelection>(this.selectedCurves.Count);
			foreach (CurveSelection current in this.selectedCurves)
			{
				CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(current);
				if (curveWrapperFromSelection != null && (!curveWrapperFromSelection.hidden || curveWrapperFromSelection.groupId != -1))
				{
					curveWrapperFromSelection.selected = CurveWrapper.SelectionMode.Selected;
					list.Add(current);
				}
			}
			this.selectedCurves = list;
			this.InvalidateBounds();
		}

		public void InvalidateBounds()
		{
			this.m_BoundsAreDirty = true;
		}

		private void RecalculateBounds()
		{
			if (this.m_BoundsAreDirty)
			{
				this.m_DrawingBounds = this.m_DefaultBounds;
				this.m_CurveBounds = this.m_DefaultBounds;
				if (this.animationCurves != null)
				{
					bool flag = false;
					for (int i = 0; i < this.animationCurves.Length; i++)
					{
						CurveWrapper curveWrapper = this.animationCurves[i];
						if (!curveWrapper.hidden)
						{
							if (curveWrapper.curve.length != 0)
							{
								if (!flag)
								{
									this.m_CurveBounds = curveWrapper.renderer.GetBounds();
									flag = true;
								}
								else
								{
									this.m_CurveBounds.Encapsulate(curveWrapper.renderer.GetBounds());
								}
							}
						}
					}
				}
				float x = (base.hRangeMin == float.NegativeInfinity) ? this.m_CurveBounds.min.x : base.hRangeMin;
				float y = (base.vRangeMin == float.NegativeInfinity) ? this.m_CurveBounds.min.y : base.vRangeMin;
				float x2 = (base.hRangeMax == float.PositiveInfinity) ? this.m_CurveBounds.max.x : base.hRangeMax;
				float y2 = (base.vRangeMax == float.PositiveInfinity) ? this.m_CurveBounds.max.y : base.vRangeMax;
				this.m_DrawingBounds.SetMinMax(new Vector3(x, y, this.m_CurveBounds.min.z), new Vector3(x2, y2, this.m_CurveBounds.max.z));
				this.m_DrawingBounds.size = new Vector3(Mathf.Max(this.m_DrawingBounds.size.x, 0.1f), Mathf.Max(this.m_DrawingBounds.size.y, 0.1f), 0f);
				this.m_CurveBounds.size = new Vector3(Mathf.Max(this.m_CurveBounds.size.x, 0.1f), Mathf.Max(this.m_CurveBounds.size.y, 0.1f), 0f);
				this.m_BoundsAreDirty = false;
			}
		}

		private void RecalculateSelectionBounds()
		{
			if (this.hasSelection)
			{
				CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(this.selectedCurves[0]);
				float num = (curveWrapperFromSelection == null) ? 0f : curveWrapperFromSelection.timeOffset;
				Keyframe keyframeFromSelection = this.GetKeyframeFromSelection(this.selectedCurves[0]);
				this.m_SelectionBounds = new Bounds(new Vector2(keyframeFromSelection.time + num, keyframeFromSelection.value), Vector2.zero);
				for (int i = 1; i < this.selectedCurves.Count; i++)
				{
					Keyframe keyframeFromSelection2 = this.GetKeyframeFromSelection(this.selectedCurves[i]);
					this.m_SelectionBounds.Encapsulate(new Vector2(keyframeFromSelection2.time + num, keyframeFromSelection2.value));
				}
			}
			else
			{
				this.m_SelectionBounds = new Bounds(Vector3.zero, Vector3.zero);
			}
		}

		public void FrameClip(bool horizontally, bool vertically)
		{
			Bounds curveBounds = this.curveBounds;
			if (!(curveBounds.size == Vector3.zero))
			{
				if (horizontally)
				{
					base.SetShownHRangeInsideMargins(curveBounds.min.x, curveBounds.max.x);
				}
				if (vertically)
				{
					base.SetShownVRangeInsideMargins(curveBounds.min.y, curveBounds.max.y);
				}
			}
		}

		public void FrameSelected(bool horizontally, bool vertically)
		{
			if (!this.hasSelection)
			{
				this.FrameClip(horizontally, vertically);
			}
			else
			{
				Bounds selectionBounds = this.selectionBounds;
				selectionBounds.size = new Vector3(Mathf.Max(selectionBounds.size.x, 0.1f), Mathf.Max(selectionBounds.size.y, 0.1f), 0f);
				if (horizontally)
				{
					base.SetShownHRangeInsideMargins(selectionBounds.min.x, selectionBounds.max.x);
				}
				if (vertically)
				{
					base.SetShownVRangeInsideMargins(selectionBounds.min.y, selectionBounds.max.y);
				}
			}
		}

		public void UpdateCurves(List<int> curveIds, string undoText)
		{
			foreach (int current in curveIds)
			{
				CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(current);
				curveWrapperFromID.changed = true;
			}
			if (this.curvesUpdated != null)
			{
				this.curvesUpdated();
			}
		}

		public void UpdateCurves(List<ChangedCurve> changedCurves, string undoText)
		{
			this.UpdateCurves(new List<int>(from curve in changedCurves
			select curve.curveId), undoText);
		}

		public void StartLiveEdit()
		{
			this.MakeCurveBackups();
		}

		public void EndLiveEdit()
		{
			this.m_CurveBackups = null;
		}

		public bool InLiveEdit()
		{
			return this.m_CurveBackups != null;
		}

		internal CurveWrapper GetCurveFromID(int curveID)
		{
			CurveWrapper result;
			if (this.m_AnimationCurves == null)
			{
				result = null;
			}
			else
			{
				CurveWrapper[] animationCurves = this.m_AnimationCurves;
				for (int i = 0; i < animationCurves.Length; i++)
				{
					CurveWrapper curveWrapper = animationCurves[i];
					if (curveWrapper.id == curveID)
					{
						result = curveWrapper;
						return result;
					}
				}
				result = null;
			}
			return result;
		}

		private void Init()
		{
		}

		public void OnGUI()
		{
			base.BeginViewGUI();
			this.GridGUI();
			this.DrawWrapperPopups();
			this.CurveGUI();
			base.EndViewGUI();
		}

		public void CurveGUI()
		{
			if (this.m_PointRenderer == null)
			{
				this.m_PointRenderer = new CurveControlPointRenderer(this.styles);
			}
			if (this.m_RectangleTool == null)
			{
				this.m_RectangleTool = new CurveEditorRectangleTool();
				this.m_RectangleTool.Initialize(this);
			}
			GUI.BeginGroup(base.drawRect);
			this.Init();
			GUIUtility.GetControlID(CurveEditor.s_SelectKeyHash, FocusType.Passive);
			Color white = Color.white;
			GUI.backgroundColor = white;
			GUI.contentColor = white;
			Color color = GUI.color;
			Event current = Event.current;
			if (current.type != EventType.Repaint)
			{
				this.EditSelectedPoints();
			}
			EventType type = current.type;
			switch (type)
			{
			case EventType.ValidateCommand:
			case EventType.ExecuteCommand:
			{
				bool flag = current.type == EventType.ExecuteCommand;
				string commandName = current.commandName;
				if (commandName != null)
				{
					if (!(commandName == "Delete"))
					{
						if (!(commandName == "FrameSelected"))
						{
							if (commandName == "SelectAll")
							{
								if (flag)
								{
									this.SelectAll();
								}
								current.Use();
							}
						}
						else
						{
							if (flag)
							{
								this.FrameSelected(true, true);
							}
							current.Use();
						}
					}
					else if (this.hasSelection)
					{
						if (flag)
						{
							this.DeleteSelectedKeys();
						}
						current.Use();
					}
				}
				goto IL_3C0;
			}
			case EventType.DragExited:
				IL_B0:
				if (type != EventType.KeyDown)
				{
					goto IL_3C0;
				}
				if ((current.keyCode == KeyCode.Backspace || current.keyCode == KeyCode.Delete) && this.hasSelection)
				{
					this.DeleteSelectedKeys();
					current.Use();
				}
				if (current.keyCode == KeyCode.A)
				{
					this.FrameClip(true, true);
					current.Use();
				}
				goto IL_3C0;
			case EventType.ContextClick:
			{
				CurveSelection curveSelection = this.FindNearest();
				if (curveSelection != null)
				{
					List<KeyIdentifier> list = new List<KeyIdentifier>();
					bool flag2 = false;
					foreach (CurveSelection current2 in this.selectedCurves)
					{
						list.Add(new KeyIdentifier(this.GetCurveFromSelection(current2), current2.curveID, current2.key));
						if (current2.curveID == curveSelection.curveID && current2.key == curveSelection.key)
						{
							flag2 = true;
						}
					}
					if (!flag2)
					{
						list.Clear();
						list.Add(new KeyIdentifier(this.GetCurveFromSelection(curveSelection), curveSelection.curveID, curveSelection.key));
						this.ClearSelection();
						this.AddSelection(curveSelection);
					}
					bool flag3 = !this.selectedCurves.Exists((CurveSelection sel) => !this.GetCurveWrapperFromSelection(sel).animationIsEditable);
					this.m_MenuManager = new CurveMenuManager(this);
					GenericMenu genericMenu = new GenericMenu();
					string text = (list.Count <= 1) ? "Delete Key" : "Delete Keys";
					string text2 = (list.Count <= 1) ? "Edit Key..." : "Edit Keys...";
					if (flag3)
					{
						genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.DeleteKeys), list);
						genericMenu.AddItem(new GUIContent(text2), false, new GenericMenu.MenuFunction2(this.StartEditingSelectedPointsContext), this.OffsetMousePositionInDrawing(this.GetCurveWrapperFromSelection(curveSelection)));
					}
					else
					{
						genericMenu.AddDisabledItem(new GUIContent(text));
						genericMenu.AddDisabledItem(new GUIContent(text2));
					}
					if (flag3)
					{
						genericMenu.AddSeparator("");
						this.m_MenuManager.AddTangentMenuItems(genericMenu, list);
					}
					genericMenu.ShowAsContext();
					Event.current.Use();
				}
				goto IL_3C0;
			}
			}
			goto IL_B0;
			IL_3C0:
			EditorGUI.BeginChangeCheck();
			GUI.color = color;
			this.m_RectangleTool.HandleOverlayEvents();
			this.DragTangents();
			this.m_RectangleTool.HandleEvents();
			this.EditAxisLabels();
			this.SelectPoints();
			if (EditorGUI.EndChangeCheck())
			{
				this.RecalcSecondarySelection();
				this.RecalcCurveSelection();
			}
			EditorGUI.BeginChangeCheck();
			Vector2 moveCoord = this.MovePoints();
			if (EditorGUI.EndChangeCheck() && this.m_DraggingKey != null)
			{
				if (this.syncTimeDuringDrag)
				{
					this.activeTime = moveCoord.x + this.s_StartClickedTime + this.m_DraggingKey.timeOffset;
				}
				this.m_MoveCoord = moveCoord;
			}
			if (current.type == EventType.Repaint)
			{
				this.DrawCurves();
				this.m_RectangleTool.OnGUI();
				this.DrawCurvesTangents();
				this.DrawCurvesOverlay();
				this.m_RectangleTool.OverlayOnGUI();
				this.EditSelectedPoints();
			}
			GUI.color = color;
			GUI.EndGroup();
		}

		private void RecalcCurveSelection()
		{
			CurveWrapper[] animationCurves = this.m_AnimationCurves;
			for (int i = 0; i < animationCurves.Length; i++)
			{
				CurveWrapper curveWrapper = animationCurves[i];
				curveWrapper.selected = CurveWrapper.SelectionMode.None;
			}
			foreach (CurveSelection current in this.selectedCurves)
			{
				CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(current);
				if (curveWrapperFromSelection != null)
				{
					curveWrapperFromSelection.selected = ((!current.semiSelected) ? CurveWrapper.SelectionMode.Selected : CurveWrapper.SelectionMode.SemiSelected);
				}
			}
		}

		private void RecalcSecondarySelection()
		{
			List<CurveSelection> list = new List<CurveSelection>(this.selectedCurves.Count);
			foreach (CurveSelection current in this.selectedCurves)
			{
				CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(current);
				if (curveWrapperFromSelection != null)
				{
					int groupId = curveWrapperFromSelection.groupId;
					if (groupId != -1 && !current.semiSelected)
					{
						list.Add(current);
						CurveWrapper[] animationCurves = this.m_AnimationCurves;
						for (int i = 0; i < animationCurves.Length; i++)
						{
							CurveWrapper curveWrapper = animationCurves[i];
							if (curveWrapper.groupId == groupId && curveWrapper != curveWrapperFromSelection)
							{
								list.Add(new CurveSelection(curveWrapper.id, current.key)
								{
									semiSelected = true
								});
							}
						}
					}
					else
					{
						list.Add(current);
					}
				}
			}
			list.Sort();
			int j = 0;
			while (j < list.Count - 1)
			{
				CurveSelection curveSelection = list[j];
				CurveSelection curveSelection2 = list[j + 1];
				if (curveSelection.curveID == curveSelection2.curveID && curveSelection.key == curveSelection2.key)
				{
					if (!curveSelection.semiSelected || !curveSelection2.semiSelected)
					{
						curveSelection.semiSelected = false;
					}
					list.RemoveAt(j + 1);
				}
				else
				{
					j++;
				}
			}
			this.selectedCurves = list;
		}

		private void DragTangents()
		{
			Event current = Event.current;
			int controlID = GUIUtility.GetControlID(CurveEditor.s_TangentControlIDHash, FocusType.Passive);
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (current.button == 0 && !current.alt)
				{
					this.m_SelectedTangentPoint = null;
					float num = 100f;
					Vector2 mousePosition = Event.current.mousePosition;
					foreach (CurveSelection current2 in this.selectedCurves)
					{
						CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(current2);
						if (curveWrapperFromSelection != null)
						{
							if (this.IsLeftTangentEditable(current2))
							{
								CurveSelection curveSelection = new CurveSelection(current2.curveID, current2.key, CurveSelection.SelectionType.InTangent);
								float sqrMagnitude = (this.DrawingToOffsetViewTransformPoint(curveWrapperFromSelection, this.GetPosition(curveSelection)) - mousePosition).sqrMagnitude;
								if (sqrMagnitude <= num)
								{
									this.m_SelectedTangentPoint = curveSelection;
									num = sqrMagnitude;
								}
							}
							if (this.IsRightTangentEditable(current2))
							{
								CurveSelection curveSelection2 = new CurveSelection(current2.curveID, current2.key, CurveSelection.SelectionType.OutTangent);
								float sqrMagnitude2 = (this.DrawingToOffsetViewTransformPoint(curveWrapperFromSelection, this.GetPosition(curveSelection2)) - mousePosition).sqrMagnitude;
								if (sqrMagnitude2 <= num)
								{
									this.m_SelectedTangentPoint = curveSelection2;
									num = sqrMagnitude2;
								}
							}
						}
					}
					if (this.m_SelectedTangentPoint != null)
					{
						this.SaveKeySelection("Edit Curve");
						GUIUtility.hotControl = controlID;
						current.Use();
					}
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					CurveSelection selectedTangentPoint = this.m_SelectedTangentPoint;
					CurveWrapper curveWrapperFromSelection2 = this.GetCurveWrapperFromSelection(selectedTangentPoint);
					if (curveWrapperFromSelection2 != null && curveWrapperFromSelection2.animationIsEditable)
					{
						Vector2 a = this.OffsetMousePositionInDrawing(curveWrapperFromSelection2);
						Keyframe keyframeFromSelection = this.GetKeyframeFromSelection(selectedTangentPoint);
						if (selectedTangentPoint.type == CurveSelection.SelectionType.InTangent)
						{
							Vector2 vector = a - new Vector2(keyframeFromSelection.time, keyframeFromSelection.value);
							if (vector.x < -0.0001f)
							{
								keyframeFromSelection.inTangent = vector.y / vector.x;
							}
							else
							{
								keyframeFromSelection.inTangent = float.PositiveInfinity;
							}
							AnimationUtility.SetKeyLeftTangentMode(ref keyframeFromSelection, AnimationUtility.TangentMode.Free);
							if (!AnimationUtility.GetKeyBroken(keyframeFromSelection))
							{
								keyframeFromSelection.outTangent = keyframeFromSelection.inTangent;
								AnimationUtility.SetKeyRightTangentMode(ref keyframeFromSelection, AnimationUtility.TangentMode.Free);
							}
						}
						else if (selectedTangentPoint.type == CurveSelection.SelectionType.OutTangent)
						{
							Vector2 vector2 = a - new Vector2(keyframeFromSelection.time, keyframeFromSelection.value);
							if (vector2.x > 0.0001f)
							{
								keyframeFromSelection.outTangent = vector2.y / vector2.x;
							}
							else
							{
								keyframeFromSelection.outTangent = float.PositiveInfinity;
							}
							AnimationUtility.SetKeyRightTangentMode(ref keyframeFromSelection, AnimationUtility.TangentMode.Free);
							if (!AnimationUtility.GetKeyBroken(keyframeFromSelection))
							{
								keyframeFromSelection.inTangent = keyframeFromSelection.outTangent;
								AnimationUtility.SetKeyLeftTangentMode(ref keyframeFromSelection, AnimationUtility.TangentMode.Free);
							}
						}
						selectedTangentPoint.key = curveWrapperFromSelection2.curve.MoveKey(selectedTangentPoint.key, keyframeFromSelection);
						AnimationUtility.UpdateTangentsFromModeSurrounding(curveWrapperFromSelection2.curve, selectedTangentPoint.key);
						curveWrapperFromSelection2.changed = true;
						GUI.changed = true;
					}
					Event.current.Use();
				}
				break;
			case EventType.Repaint:
				if (GUIUtility.hotControl == controlID)
				{
					Rect position = new Rect(current.mousePosition.x - 10f, current.mousePosition.y - 10f, 20f, 20f);
					EditorGUIUtility.AddCursorRect(position, MouseCursor.MoveArrow);
				}
				break;
			}
		}

		internal void DeleteSelectedKeys()
		{
			string undoLabel;
			if (this.selectedCurves.Count > 1)
			{
				undoLabel = "Delete Keys";
			}
			else
			{
				undoLabel = "Delete Key";
			}
			this.SaveKeySelection(undoLabel);
			for (int i = this.selectedCurves.Count - 1; i >= 0; i--)
			{
				CurveSelection curveSelection = this.selectedCurves[i];
				CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(curveSelection);
				if (curveWrapperFromSelection != null)
				{
					if (curveWrapperFromSelection.animationIsEditable)
					{
						if (this.settings.allowDeleteLastKeyInCurve || curveWrapperFromSelection.curve.keys.Length != 1)
						{
							curveWrapperFromSelection.curve.RemoveKey(curveSelection.key);
							AnimationUtility.UpdateTangentsFromMode(curveWrapperFromSelection.curve);
							curveWrapperFromSelection.changed = true;
							GUI.changed = true;
						}
					}
				}
			}
			this.SelectNone();
		}

		private void DeleteKeys(object obj)
		{
			List<KeyIdentifier> list = (List<KeyIdentifier>)obj;
			string text;
			if (list.Count > 1)
			{
				text = "Delete Keys";
			}
			else
			{
				text = "Delete Key";
			}
			this.SaveKeySelection(text);
			List<int> list2 = new List<int>();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (this.settings.allowDeleteLastKeyInCurve || list[i].curve.keys.Length != 1)
				{
					if (this.GetCurveWrapperFromID(list[i].curveId).animationIsEditable)
					{
						list[i].curve.RemoveKey(list[i].key);
						AnimationUtility.UpdateTangentsFromMode(list[i].curve);
						list2.Add(list[i].curveId);
					}
				}
			}
			this.UpdateCurves(list2, text);
			this.SelectNone();
		}

		private float ClampVerticalValue(float value, int curveID)
		{
			value = Mathf.Clamp(value, base.vRangeMin, base.vRangeMax);
			CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(curveID);
			if (curveWrapperFromID != null)
			{
				value = Mathf.Clamp(value, curveWrapperFromID.vRangeMin, curveWrapperFromID.vRangeMax);
			}
			return value;
		}

		internal void TranslateSelectedKeys(Vector2 movement)
		{
			bool flag = this.InLiveEdit();
			if (!flag)
			{
				this.StartLiveEdit();
			}
			this.UpdateCurvesFromPoints(delegate(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve)
			{
				CurveEditor.SavedCurve.SavedKeyFrame result;
				if (keyframe.selected != CurveWrapper.SelectionMode.None)
				{
					CurveEditor.SavedCurve.SavedKeyFrame savedKeyFrame = keyframe.Clone();
					savedKeyFrame.key.time = Mathf.Clamp(savedKeyFrame.key.time + movement.x, this.hRangeMin, this.hRangeMax);
					if (savedKeyFrame.selected == CurveWrapper.SelectionMode.Selected)
					{
						savedKeyFrame.key.value = this.ClampVerticalValue(savedKeyFrame.key.value + movement.y, curve.curveId);
					}
					result = savedKeyFrame;
				}
				else
				{
					result = keyframe;
				}
				return result;
			});
			if (!flag)
			{
				this.EndLiveEdit();
			}
		}

		internal void SetSelectedKeyPositions(Vector2 newPosition, bool updateTime, bool updateValue)
		{
			bool flag = this.InLiveEdit();
			if (!flag)
			{
				this.StartLiveEdit();
			}
			this.UpdateCurvesFromPoints(delegate(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve)
			{
				CurveEditor.SavedCurve.SavedKeyFrame result;
				if (keyframe.selected != CurveWrapper.SelectionMode.None)
				{
					CurveEditor.SavedCurve.SavedKeyFrame savedKeyFrame = keyframe.Clone();
					if (updateTime)
					{
						savedKeyFrame.key.time = Mathf.Clamp(newPosition.x, this.hRangeMin, this.hRangeMax);
					}
					if (updateValue)
					{
						savedKeyFrame.key.value = this.ClampVerticalValue(newPosition.y, curve.curveId);
					}
					result = savedKeyFrame;
				}
				else
				{
					result = keyframe;
				}
				return result;
			});
			if (!flag)
			{
				this.EndLiveEdit();
			}
		}

		internal void TransformSelectedKeys(Matrix4x4 matrix, bool flipX, bool flipY)
		{
			bool flag = this.InLiveEdit();
			if (!flag)
			{
				this.StartLiveEdit();
			}
			this.UpdateCurvesFromPoints(delegate(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve)
			{
				CurveEditor.SavedCurve.SavedKeyFrame result;
				if (keyframe.selected != CurveWrapper.SelectionMode.None)
				{
					CurveEditor.SavedCurve.SavedKeyFrame savedKeyFrame = keyframe.Clone();
					Vector3 v = new Vector3(savedKeyFrame.key.time, savedKeyFrame.key.value, 0f);
					v = matrix.MultiplyPoint3x4(v);
					v.x = this.SnapTime(v.x);
					savedKeyFrame.key.time = Mathf.Clamp(v.x, this.hRangeMin, this.hRangeMax);
					if (flipX)
					{
						savedKeyFrame.key.inTangent = -keyframe.key.outTangent;
						savedKeyFrame.key.outTangent = -keyframe.key.inTangent;
					}
					if (savedKeyFrame.selected == CurveWrapper.SelectionMode.Selected)
					{
						savedKeyFrame.key.value = this.ClampVerticalValue(v.y, curve.curveId);
						if (flipY)
						{
							savedKeyFrame.key.inTangent = -savedKeyFrame.key.inTangent;
							savedKeyFrame.key.outTangent = -savedKeyFrame.key.outTangent;
						}
					}
					result = savedKeyFrame;
				}
				else
				{
					result = keyframe;
				}
				return result;
			});
			if (!flag)
			{
				this.EndLiveEdit();
			}
		}

		internal void TransformRippleKeys(Matrix4x4 matrix, float t1, float t2, bool flipX)
		{
			bool flag = this.InLiveEdit();
			if (!flag)
			{
				this.StartLiveEdit();
			}
			this.UpdateCurvesFromPoints(delegate(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve)
			{
				float num = keyframe.key.time;
				CurveEditor.SavedCurve.SavedKeyFrame result;
				if (keyframe.selected != CurveWrapper.SelectionMode.None)
				{
					Vector3 v = new Vector3(keyframe.key.time, 0f, 0f);
					v = matrix.MultiplyPoint3x4(v);
					num = v.x;
					CurveEditor.SavedCurve.SavedKeyFrame savedKeyFrame = keyframe.Clone();
					savedKeyFrame.key.time = this.SnapTime(Mathf.Clamp(num, this.hRangeMin, this.hRangeMax));
					if (flipX)
					{
						savedKeyFrame.key.inTangent = -keyframe.key.outTangent;
						savedKeyFrame.key.outTangent = -keyframe.key.inTangent;
					}
					result = savedKeyFrame;
				}
				else
				{
					if (keyframe.key.time > t2)
					{
						Vector3 v2 = new Vector3((!flipX) ? t2 : t1, 0f, 0f);
						v2 = matrix.MultiplyPoint3x4(v2);
						float num2 = v2.x - t2;
						if (num2 > 0f)
						{
							num = keyframe.key.time + num2;
						}
					}
					else if (keyframe.key.time < t1)
					{
						Vector3 v3 = new Vector3((!flipX) ? t1 : t2, 0f, 0f);
						v3 = matrix.MultiplyPoint3x4(v3);
						float num3 = v3.x - t1;
						if (num3 < 0f)
						{
							num = keyframe.key.time + num3;
						}
					}
					if (!Mathf.Approximately(num, keyframe.key.time))
					{
						CurveEditor.SavedCurve.SavedKeyFrame savedKeyFrame2 = keyframe.Clone();
						savedKeyFrame2.key.time = this.SnapTime(Mathf.Clamp(num, this.hRangeMin, this.hRangeMax));
						result = savedKeyFrame2;
					}
					else
					{
						result = keyframe;
					}
				}
				return result;
			});
			if (!flag)
			{
				this.EndLiveEdit();
			}
		}

		private void UpdateCurvesFromPoints(CurveEditor.SavedCurve.KeyFrameOperation action)
		{
			if (this.m_CurveBackups != null)
			{
				List<CurveSelection> list = new List<CurveSelection>();
				foreach (CurveEditor.SavedCurve current in this.m_CurveBackups)
				{
					CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(current.curveId);
					if (curveWrapperFromID.animationIsEditable)
					{
						List<CurveEditor.SavedCurve.SavedKeyFrame> list2 = new List<CurveEditor.SavedCurve.SavedKeyFrame>(current.keys.Count);
						foreach (CurveEditor.SavedCurve.SavedKeyFrame current2 in current.keys)
						{
							if (current2.selected == CurveWrapper.SelectionMode.None)
							{
								CurveEditor.SavedCurve.SavedKeyFrame newKeyframe = action(current2, current);
								list2.RemoveAll((CurveEditor.SavedCurve.SavedKeyFrame workingKeyframe) => Mathf.Abs(workingKeyframe.key.time - newKeyframe.key.time) < 1E-05f);
								list2.Add(newKeyframe);
							}
						}
						foreach (CurveEditor.SavedCurve.SavedKeyFrame current3 in current.keys)
						{
							if (current3.selected != CurveWrapper.SelectionMode.None)
							{
								CurveEditor.SavedCurve.SavedKeyFrame newKeyframe = action(current3, current);
								list2.RemoveAll((CurveEditor.SavedCurve.SavedKeyFrame workingKeyframe) => Mathf.Abs(workingKeyframe.key.time - newKeyframe.key.time) < 1E-05f);
								list2.Add(newKeyframe);
							}
						}
						list2.Sort();
						Keyframe[] array = new Keyframe[list2.Count];
						for (int i = 0; i < list2.Count; i++)
						{
							CurveEditor.SavedCurve.SavedKeyFrame savedKeyFrame = list2[i];
							array[i] = savedKeyFrame.key;
							if (savedKeyFrame.selected != CurveWrapper.SelectionMode.None)
							{
								CurveSelection curveSelection = new CurveSelection(current.curveId, i);
								if (savedKeyFrame.selected == CurveWrapper.SelectionMode.SemiSelected)
								{
									curveSelection.semiSelected = true;
								}
								list.Add(curveSelection);
							}
						}
						this.selectedCurves = list;
						curveWrapperFromID.curve.keys = array;
						curveWrapperFromID.changed = true;
					}
				}
				this.UpdateTangentsFromSelection();
			}
		}

		private float SnapTime(float t)
		{
			if (EditorGUI.actionKey)
			{
				int levelWithMinSeparation = base.hTicks.GetLevelWithMinSeparation(5f);
				float periodOfLevel = base.hTicks.GetPeriodOfLevel(levelWithMinSeparation);
				t = Mathf.Round(t / periodOfLevel) * periodOfLevel;
			}
			else if (this.invSnap != 0f)
			{
				t = Mathf.Round(t * this.invSnap) / this.invSnap;
			}
			return t;
		}

		private float SnapValue(float v)
		{
			if (EditorGUI.actionKey)
			{
				int levelWithMinSeparation = base.vTicks.GetLevelWithMinSeparation(5f);
				float periodOfLevel = base.vTicks.GetPeriodOfLevel(levelWithMinSeparation);
				v = Mathf.Round(v / periodOfLevel) * periodOfLevel;
			}
			return v;
		}

		private Vector2 GetGUIPoint(CurveWrapper cw, Vector3 point)
		{
			return HandleUtility.WorldToGUIPoint(this.DrawingToOffsetViewTransformPoint(cw, point));
		}

		private int OnlyOneEditableCurve()
		{
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < this.m_AnimationCurves.Length; i++)
			{
				CurveWrapper curveWrapper = this.m_AnimationCurves[i];
				if (!curveWrapper.hidden && !curveWrapper.readOnly)
				{
					num2++;
					num = i;
				}
			}
			int result;
			if (num2 == 1)
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		private int GetCurveAtPosition(Vector2 viewPos, out Vector2 closestPointOnCurve)
		{
			int num = (int)Mathf.Sqrt(100f);
			float num2 = 100f;
			int num3 = -1;
			closestPointOnCurve = Vector3.zero;
			for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
			{
				CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(this.m_DrawOrder[i]);
				if (!curveWrapperFromID.hidden && !curveWrapperFromID.readOnly)
				{
					Vector2 vector = this.OffsetViewToDrawingTransformPoint(curveWrapperFromID, viewPos);
					Vector2 vector2;
					vector2.x = vector.x - (float)num / base.scale.x;
					vector2.y = curveWrapperFromID.renderer.EvaluateCurveSlow(vector2.x);
					vector2 = this.DrawingToOffsetViewTransformPoint(curveWrapperFromID, vector2);
					for (int j = -num; j < num; j++)
					{
						Vector2 vector3;
						vector3.x = vector.x + (float)(j + 1) / base.scale.x;
						vector3.y = curveWrapperFromID.renderer.EvaluateCurveSlow(vector3.x);
						vector3 = this.DrawingToOffsetViewTransformPoint(curveWrapperFromID, vector3);
						float num4 = HandleUtility.DistancePointLine(viewPos, vector2, vector3);
						num4 *= num4;
						if (num4 < num2)
						{
							num2 = num4;
							num3 = curveWrapperFromID.listIndex;
							closestPointOnCurve = HandleUtility.ProjectPointLine(viewPos, vector2, vector3);
						}
						vector2 = vector3;
					}
				}
			}
			if (num3 >= 0)
			{
				closestPointOnCurve = this.OffsetViewToDrawingTransformPoint(this.m_AnimationCurves[num3], closestPointOnCurve);
			}
			return num3;
		}

		private void CreateKeyFromClick(object obj)
		{
			string text = "Add Key";
			this.SaveKeySelection(text);
			List<int> list = this.CreateKeyFromClick((Vector2)obj);
			if (list.Count > 0)
			{
				this.UpdateCurves(list, text);
			}
		}

		private List<int> CreateKeyFromClick(Vector2 viewPos)
		{
			List<int> list = new List<int>();
			int num = this.OnlyOneEditableCurve();
			List<int> result;
			if (num >= 0)
			{
				CurveWrapper curveWrapper = this.m_AnimationCurves[num];
				Vector2 localPos = this.OffsetViewToDrawingTransformPoint(curveWrapper, viewPos);
				float num2 = localPos.x - curveWrapper.timeOffset;
				if (curveWrapper.curve.keys.Length == 0 || num2 < curveWrapper.curve.keys[0].time || num2 > curveWrapper.curve.keys[curveWrapper.curve.keys.Length - 1].time)
				{
					if (this.CreateKeyFromClick(num, localPos))
					{
						list.Add(curveWrapper.id);
					}
					result = list;
					return result;
				}
			}
			Vector2 vector;
			int curveAtPosition = this.GetCurveAtPosition(viewPos, out vector);
			if (this.CreateKeyFromClick(curveAtPosition, vector.x))
			{
				if (curveAtPosition >= 0)
				{
					list.Add(this.m_AnimationCurves[curveAtPosition].id);
				}
			}
			result = list;
			return result;
		}

		private bool CreateKeyFromClick(int curveIndex, float time)
		{
			time = Mathf.Clamp(time, this.settings.hRangeMin, this.settings.hRangeMax);
			bool result;
			if (curveIndex >= 0)
			{
				CurveSelection curveSelection = null;
				CurveWrapper curveWrapper = this.m_AnimationCurves[curveIndex];
				if (curveWrapper.animationIsEditable)
				{
					if (curveWrapper.groupId == -1)
					{
						curveSelection = this.AddKeyAtTime(curveWrapper, time);
					}
					else
					{
						CurveWrapper[] animationCurves = this.m_AnimationCurves;
						for (int i = 0; i < animationCurves.Length; i++)
						{
							CurveWrapper curveWrapper2 = animationCurves[i];
							if (curveWrapper2.groupId == curveWrapper.groupId)
							{
								CurveSelection curveSelection2 = this.AddKeyAtTime(curveWrapper2, time);
								if (curveWrapper2.id == curveWrapper.id)
								{
									curveSelection = curveSelection2;
								}
							}
						}
					}
					if (curveSelection != null)
					{
						this.ClearSelection();
						this.AddSelection(curveSelection);
						this.RecalcSecondarySelection();
					}
					else
					{
						this.SelectNone();
					}
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private bool CreateKeyFromClick(int curveIndex, Vector2 localPos)
		{
			localPos.x = Mathf.Clamp(localPos.x, this.settings.hRangeMin, this.settings.hRangeMax);
			bool result;
			if (curveIndex >= 0)
			{
				CurveSelection curveSelection = null;
				CurveWrapper curveWrapper = this.m_AnimationCurves[curveIndex];
				if (curveWrapper.animationIsEditable)
				{
					if (curveWrapper.groupId == -1)
					{
						curveSelection = this.AddKeyAtPosition(curveWrapper, localPos);
					}
					else
					{
						CurveWrapper[] animationCurves = this.m_AnimationCurves;
						for (int i = 0; i < animationCurves.Length; i++)
						{
							CurveWrapper curveWrapper2 = animationCurves[i];
							if (curveWrapper2.groupId == curveWrapper.groupId)
							{
								if (curveWrapper2.id == curveWrapper.id)
								{
									curveSelection = this.AddKeyAtPosition(curveWrapper2, localPos);
								}
								else
								{
									this.AddKeyAtTime(curveWrapper2, localPos.x);
								}
							}
						}
					}
					if (curveSelection != null)
					{
						this.ClearSelection();
						this.AddSelection(curveSelection);
						this.RecalcSecondarySelection();
					}
					else
					{
						this.SelectNone();
					}
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public void AddKey(CurveWrapper cw, Keyframe key)
		{
			CurveSelection curveSelection = this.AddKeyframeAndSelect(key, cw);
			if (curveSelection != null)
			{
				this.ClearSelection();
				this.AddSelection(curveSelection);
				this.RecalcSecondarySelection();
			}
			else
			{
				this.SelectNone();
			}
		}

		private CurveSelection AddKeyAtTime(CurveWrapper cw, float time)
		{
			time = this.SnapTime(time);
			float num;
			if (this.invSnap != 0f)
			{
				num = 0.5f / this.invSnap;
			}
			else
			{
				num = 0.0001f;
			}
			CurveSelection result;
			if (CurveUtility.HaveKeysInRange(cw.curve, time - num, time + num))
			{
				result = null;
			}
			else
			{
				float num2 = cw.renderer.EvaluateCurveDeltaSlow(time);
				float value = this.ClampVerticalValue(this.SnapValue(cw.renderer.EvaluateCurveSlow(time)), cw.id);
				Keyframe key = new Keyframe(time, value, num2, num2);
				result = this.AddKeyframeAndSelect(key, cw);
			}
			return result;
		}

		private CurveSelection AddKeyAtPosition(CurveWrapper cw, Vector2 position)
		{
			position.x = this.SnapTime(position.x);
			float num;
			if (this.invSnap != 0f)
			{
				num = 0.5f / this.invSnap;
			}
			else
			{
				num = 0.0001f;
			}
			CurveSelection result;
			if (CurveUtility.HaveKeysInRange(cw.curve, position.x - num, position.x + num))
			{
				result = null;
			}
			else
			{
				float num2 = 0f;
				Keyframe key = new Keyframe(position.x, this.SnapValue(position.y), num2, num2);
				result = this.AddKeyframeAndSelect(key, cw);
			}
			return result;
		}

		private CurveSelection AddKeyframeAndSelect(Keyframe key, CurveWrapper cw)
		{
			CurveSelection result;
			if (!cw.animationIsEditable)
			{
				result = null;
			}
			else
			{
				int num = cw.curve.AddKey(key);
				CurveUtility.SetKeyModeFromContext(cw.curve, num);
				AnimationUtility.UpdateTangentsFromModeSurrounding(cw.curve, num);
				CurveSelection curveSelection = new CurveSelection(cw.id, num);
				cw.selected = CurveWrapper.SelectionMode.Selected;
				cw.changed = true;
				if (this.syncTimeDuringDrag)
				{
					this.activeTime = key.time + cw.timeOffset;
				}
				result = curveSelection;
			}
			return result;
		}

		private CurveSelection FindNearest()
		{
			Vector2 mousePosition = Event.current.mousePosition;
			bool flag = false;
			int num = -1;
			int key = -1;
			float num2 = 100f;
			CurveSelection result;
			for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
			{
				CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(this.m_DrawOrder[i]);
				if (!curveWrapperFromID.readOnly && !curveWrapperFromID.hidden)
				{
					for (int j = 0; j < curveWrapperFromID.curve.keys.Length; j++)
					{
						Keyframe keyframe = curveWrapperFromID.curve.keys[j];
						float sqrMagnitude = (this.GetGUIPoint(curveWrapperFromID, new Vector2(keyframe.time, keyframe.value)) - mousePosition).sqrMagnitude;
						if (sqrMagnitude <= 16f)
						{
							result = new CurveSelection(curveWrapperFromID.id, j);
							return result;
						}
						if (sqrMagnitude < num2)
						{
							flag = true;
							num = curveWrapperFromID.id;
							key = j;
							num2 = sqrMagnitude;
						}
					}
					if (i == this.m_DrawOrder.Count - 1 && num >= 0)
					{
						num2 = 16f;
					}
				}
			}
			if (flag)
			{
				result = new CurveSelection(num, key);
				return result;
			}
			result = null;
			return result;
		}

		public void SelectNone()
		{
			this.ClearSelection();
			CurveWrapper[] animationCurves = this.m_AnimationCurves;
			for (int i = 0; i < animationCurves.Length; i++)
			{
				CurveWrapper curveWrapper = animationCurves[i];
				curveWrapper.selected = CurveWrapper.SelectionMode.None;
			}
		}

		public void SelectAll()
		{
			int num = 0;
			CurveWrapper[] animationCurves = this.m_AnimationCurves;
			for (int i = 0; i < animationCurves.Length; i++)
			{
				CurveWrapper curveWrapper = animationCurves[i];
				if (!curveWrapper.hidden)
				{
					num += curveWrapper.curve.length;
				}
			}
			List<CurveSelection> list = new List<CurveSelection>(num);
			CurveWrapper[] animationCurves2 = this.m_AnimationCurves;
			for (int j = 0; j < animationCurves2.Length; j++)
			{
				CurveWrapper curveWrapper2 = animationCurves2[j];
				curveWrapper2.selected = CurveWrapper.SelectionMode.Selected;
				for (int k = 0; k < curveWrapper2.curve.length; k++)
				{
					list.Add(new CurveSelection(curveWrapper2.id, k));
				}
			}
			this.selectedCurves = list;
		}

		public bool IsDraggingKey()
		{
			return this.m_DraggingKey != null;
		}

		public bool IsDraggingCurveOrRegion()
		{
			return this.m_DraggingCurveOrRegion != null;
		}

		public bool IsDraggingCurve(CurveWrapper cw)
		{
			return this.m_DraggingCurveOrRegion != null && this.m_DraggingCurveOrRegion.Length == 1 && this.m_DraggingCurveOrRegion[0] == cw;
		}

		public bool IsDraggingRegion(CurveWrapper cw1, CurveWrapper cw2)
		{
			return this.m_DraggingCurveOrRegion != null && this.m_DraggingCurveOrRegion.Length == 2 && (this.m_DraggingCurveOrRegion[0] == cw1 || this.m_DraggingCurveOrRegion[0] == cw2);
		}

		private bool HandleCurveAndRegionMoveToFrontOnMouseDown(ref Vector2 timeValue, ref CurveWrapper[] curves)
		{
			Vector2 vector;
			int curveAtPosition = this.GetCurveAtPosition(Event.current.mousePosition, out vector);
			bool result;
			if (curveAtPosition >= 0)
			{
				this.MoveCurveToFront(this.m_AnimationCurves[curveAtPosition].id);
				timeValue = this.OffsetMousePositionInDrawing(this.m_AnimationCurves[curveAtPosition]);
				curves = new CurveWrapper[]
				{
					this.m_AnimationCurves[curveAtPosition]
				};
				result = true;
			}
			else
			{
				for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
				{
					CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(this.m_DrawOrder[i]);
					if (curveWrapperFromID != null)
					{
						if (!curveWrapperFromID.hidden)
						{
							if (curveWrapperFromID.curve.length != 0)
							{
								CurveWrapper curveWrapper = null;
								if (i > 0)
								{
									curveWrapper = this.GetCurveWrapperFromID(this.m_DrawOrder[i - 1]);
								}
								if (this.IsRegion(curveWrapperFromID, curveWrapper))
								{
									Vector2 vector2 = this.OffsetMousePositionInDrawing(curveWrapperFromID);
									Vector2 vector3 = this.OffsetMousePositionInDrawing(curveWrapper);
									float num = curveWrapperFromID.renderer.EvaluateCurveSlow(vector2.x);
									float num2 = curveWrapper.renderer.EvaluateCurveSlow(vector3.x);
									if (num > num2)
									{
										float num3 = num;
										num = num2;
										num2 = num3;
									}
									if (vector2.y >= num && vector2.y <= num2)
									{
										timeValue = vector2;
										curves = new CurveWrapper[]
										{
											curveWrapperFromID,
											curveWrapper
										};
										this.MoveCurveToFront(curveWrapperFromID.id);
										result = true;
										return result;
									}
									i--;
								}
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		private void SelectPoints()
		{
			int controlID = GUIUtility.GetControlID(897560, FocusType.Passive);
			Event current = Event.current;
			bool shift = current.shift;
			bool actionKey = EditorGUI.actionKey;
			EventType typeForControl = current.GetTypeForControl(controlID);
			switch (typeForControl)
			{
			case EventType.MouseDown:
				if (current.clickCount == 2 && current.button == 0)
				{
					CurveSelection selectedPoint = this.FindNearest();
					if (selectedPoint != null)
					{
						if (!shift)
						{
							this.ClearSelection();
						}
						AnimationCurve curveFromSelection = this.GetCurveFromSelection(selectedPoint);
						if (curveFromSelection != null)
						{
							int keyIndex;
							for (keyIndex = 0; keyIndex < curveFromSelection.keys.Length; keyIndex++)
							{
								if (!this.selectedCurves.Any((CurveSelection x) => x.curveID == selectedPoint.curveID && x.key == keyIndex))
								{
									CurveSelection curveSelection = new CurveSelection(selectedPoint.curveID, keyIndex);
									this.AddSelection(curveSelection);
								}
							}
						}
					}
					else
					{
						this.SaveKeySelection("Add Key");
						List<int> list = this.CreateKeyFromClick(Event.current.mousePosition);
						if (list.Count > 0)
						{
							foreach (int current2 in list)
							{
								CurveWrapper curveFromID = this.GetCurveFromID(current2);
								curveFromID.changed = true;
							}
							GUI.changed = true;
						}
					}
					current.Use();
				}
				else if (current.button == 0)
				{
					CurveSelection selectedPoint = this.FindNearest();
					if (selectedPoint == null || selectedPoint.semiSelected)
					{
						Vector2 zero = Vector2.zero;
						CurveWrapper[] array = null;
						bool flag = this.HandleCurveAndRegionMoveToFrontOnMouseDown(ref zero, ref array);
						if (!shift && !actionKey && !flag)
						{
							this.SelectNone();
						}
						GUIUtility.hotControl = controlID;
						this.s_EndMouseDragPosition = (this.s_StartMouseDragPosition = current.mousePosition);
						this.s_PickMode = CurveEditor.PickMode.Click;
					}
					else
					{
						this.MoveCurveToFront(selectedPoint.curveID);
						if (this.syncTimeDuringDrag)
						{
							CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(selectedPoint);
							if (curveWrapperFromSelection != null)
							{
								this.activeTime = this.GetKeyframeFromSelection(selectedPoint).time + curveWrapperFromSelection.timeOffset;
							}
						}
						Keyframe keyframeFromSelection = this.GetKeyframeFromSelection(selectedPoint);
						this.s_StartKeyDragPosition = new Vector2(keyframeFromSelection.time, keyframeFromSelection.value);
						if (shift)
						{
							bool flag2 = false;
							int num = selectedPoint.key;
							int num2 = selectedPoint.key;
							for (int i = 0; i < this.selectedCurves.Count; i++)
							{
								CurveSelection curveSelection2 = this.selectedCurves[i];
								if (curveSelection2.curveID == selectedPoint.curveID)
								{
									flag2 = true;
									num = Mathf.Min(num, curveSelection2.key);
									num2 = Mathf.Max(num2, curveSelection2.key);
								}
							}
							if (!flag2)
							{
								if (!this.selectedCurves.Contains(selectedPoint))
								{
									this.AddSelection(selectedPoint);
								}
							}
							else
							{
								int keyIndex;
								for (keyIndex = num; keyIndex <= num2; keyIndex++)
								{
									if (!this.selectedCurves.Any((CurveSelection x) => x.curveID == selectedPoint.curveID && x.key == keyIndex))
									{
										CurveSelection curveSelection3 = new CurveSelection(selectedPoint.curveID, keyIndex);
										this.AddSelection(curveSelection3);
									}
								}
							}
							Event.current.Use();
						}
						else if (actionKey)
						{
							if (!this.selectedCurves.Contains(selectedPoint))
							{
								this.AddSelection(selectedPoint);
							}
							else
							{
								this.RemoveSelection(selectedPoint);
							}
							Event.current.Use();
						}
						else if (!this.selectedCurves.Contains(selectedPoint))
						{
							this.ClearSelection();
							this.AddSelection(selectedPoint);
						}
					}
					GUI.changed = true;
					HandleUtility.Repaint();
				}
				goto IL_742;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					this.s_PickMode = CurveEditor.PickMode.None;
					Event.current.Use();
				}
				goto IL_742;
			case EventType.MouseMove:
			{
				IL_40:
				if (typeForControl == EventType.Layout)
				{
					HandleUtility.AddDefaultControl(controlID);
					goto IL_742;
				}
				if (typeForControl != EventType.ContextClick)
				{
					goto IL_742;
				}
				Rect drawRect = base.drawRect;
				float num3 = 0f;
				drawRect.y = num3;
				drawRect.x = num3;
				if (drawRect.Contains(Event.current.mousePosition))
				{
					Vector2 vector;
					int curveAtPosition = this.GetCurveAtPosition(Event.current.mousePosition, out vector);
					if (curveAtPosition >= 0)
					{
						GenericMenu genericMenu = new GenericMenu();
						if (this.m_AnimationCurves[curveAtPosition].animationIsEditable)
						{
							genericMenu.AddItem(new GUIContent("Add Key"), false, new GenericMenu.MenuFunction2(this.CreateKeyFromClick), Event.current.mousePosition);
						}
						else
						{
							genericMenu.AddDisabledItem(new GUIContent("Add Key"));
						}
						genericMenu.ShowAsContext();
						Event.current.Use();
					}
				}
				goto IL_742;
			}
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					this.s_EndMouseDragPosition = current.mousePosition;
					if (this.s_PickMode == CurveEditor.PickMode.Click)
					{
						this.s_PickMode = CurveEditor.PickMode.Marquee;
						if (shift || actionKey)
						{
							this.s_SelectionBackup = new List<CurveSelection>(this.selectedCurves);
						}
						else
						{
							this.s_SelectionBackup = new List<CurveSelection>();
						}
					}
					else
					{
						Rect rect = EditorGUIExt.FromToRect(this.s_StartMouseDragPosition, current.mousePosition);
						List<CurveSelection> list2 = new List<CurveSelection>(this.s_SelectionBackup);
						CurveWrapper[] animationCurves = this.m_AnimationCurves;
						for (int j = 0; j < animationCurves.Length; j++)
						{
							CurveWrapper curveWrapper = animationCurves[j];
							if (!curveWrapper.readOnly && !curveWrapper.hidden)
							{
								int num4 = 0;
								Keyframe[] keys = curveWrapper.curve.keys;
								for (int k = 0; k < keys.Length; k++)
								{
									Keyframe keyframe = keys[k];
									if (rect.Contains(this.GetGUIPoint(curveWrapper, new Vector2(keyframe.time, keyframe.value))))
									{
										list2.Add(new CurveSelection(curveWrapper.id, num4));
										this.MoveCurveToFront(curveWrapper.id);
									}
									num4++;
								}
							}
						}
						this.selectedCurves = list2;
						GUI.changed = true;
					}
					current.Use();
				}
				goto IL_742;
			}
			goto IL_40;
			IL_742:
			if (this.s_PickMode == CurveEditor.PickMode.Marquee)
			{
				GUI.Label(EditorGUIExt.FromToRect(this.s_StartMouseDragPosition, this.s_EndMouseDragPosition), GUIContent.none, this.styles.selectionRect);
			}
		}

		private void EditAxisLabels()
		{
			int controlID = GUIUtility.GetControlID(18975602, FocusType.Keyboard);
			List<CurveWrapper> list = new List<CurveWrapper>();
			Vector2 axisUiScalars = this.GetAxisUiScalars(list);
			if (axisUiScalars.y >= 0f && list.Count > 0 && list[0].setAxisUiScalarsCallback != null)
			{
				Rect rect = new Rect(0f, base.topmargin - 8f, base.leftmargin - 4f, 16f);
				Rect position = rect;
				position.y -= rect.height;
				Event current = Event.current;
				switch (current.GetTypeForControl(controlID))
				{
				case EventType.MouseDown:
					if (current.button == 0)
					{
						if (position.Contains(Event.current.mousePosition))
						{
							if (GUIUtility.hotControl == 0)
							{
								GUIUtility.keyboardControl = 0;
								GUIUtility.hotControl = controlID;
								GUI.changed = true;
								current.Use();
							}
						}
						if (!rect.Contains(Event.current.mousePosition))
						{
							GUIUtility.keyboardControl = 0;
						}
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlID)
					{
						GUIUtility.hotControl = 0;
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID)
					{
						float num = Mathf.Clamp01(Mathf.Max(axisUiScalars.y, Mathf.Pow(Mathf.Abs(axisUiScalars.y), 0.5f)) * 0.01f);
						axisUiScalars.y += HandleUtility.niceMouseDelta * num;
						if (axisUiScalars.y < 0.001f)
						{
							axisUiScalars.y = 0.001f;
						}
						this.SetAxisUiScalars(axisUiScalars, list);
						current.Use();
					}
					break;
				case EventType.Repaint:
					if (GUIUtility.hotControl == 0)
					{
						EditorGUIUtility.AddCursorRect(position, MouseCursor.SlideArrow);
					}
					break;
				}
				string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
				EditorGUI.kFloatFieldFormatString = this.m_AxisLabelFormat;
				float num2 = EditorGUI.FloatField(rect, axisUiScalars.y, this.styles.axisLabelNumberField);
				if (axisUiScalars.y != num2)
				{
					this.SetAxisUiScalars(new Vector2(axisUiScalars.x, num2), list);
				}
				EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
			}
		}

		public void BeginTimeRangeSelection(float time, bool addToSelection)
		{
			if (this.s_TimeRangeSelectionActive)
			{
				Debug.LogError("BeginTimeRangeSelection can only be called once");
			}
			else
			{
				this.s_TimeRangeSelectionActive = true;
				this.s_TimeRangeSelectionEnd = time;
				this.s_TimeRangeSelectionStart = time;
				if (addToSelection)
				{
					this.s_SelectionBackup = new List<CurveSelection>(this.selectedCurves);
				}
				else
				{
					this.s_SelectionBackup = new List<CurveSelection>();
				}
			}
		}

		public void TimeRangeSelectTo(float time)
		{
			if (!this.s_TimeRangeSelectionActive)
			{
				Debug.LogError("TimeRangeSelectTo can only be called after BeginTimeRangeSelection");
			}
			else
			{
				this.s_TimeRangeSelectionEnd = time;
				List<CurveSelection> list = new List<CurveSelection>(this.s_SelectionBackup);
				float num = Mathf.Min(this.s_TimeRangeSelectionStart, this.s_TimeRangeSelectionEnd);
				float num2 = Mathf.Max(this.s_TimeRangeSelectionStart, this.s_TimeRangeSelectionEnd);
				CurveWrapper[] animationCurves = this.m_AnimationCurves;
				for (int i = 0; i < animationCurves.Length; i++)
				{
					CurveWrapper curveWrapper = animationCurves[i];
					if (!curveWrapper.readOnly && !curveWrapper.hidden)
					{
						int num3 = 0;
						Keyframe[] keys = curveWrapper.curve.keys;
						for (int j = 0; j < keys.Length; j++)
						{
							Keyframe keyframe = keys[j];
							if (keyframe.time >= num && keyframe.time < num2)
							{
								list.Add(new CurveSelection(curveWrapper.id, num3));
							}
							num3++;
						}
					}
				}
				this.selectedCurves = list;
				this.RecalcSecondarySelection();
				this.RecalcCurveSelection();
			}
		}

		public void EndTimeRangeSelection()
		{
			if (!this.s_TimeRangeSelectionActive)
			{
				Debug.LogError("EndTimeRangeSelection can only be called after BeginTimeRangeSelection");
			}
			else
			{
				this.s_TimeRangeSelectionStart = (this.s_TimeRangeSelectionEnd = 0f);
				this.s_TimeRangeSelectionActive = false;
			}
		}

		public void CancelTimeRangeSelection()
		{
			if (!this.s_TimeRangeSelectionActive)
			{
				Debug.LogError("CancelTimeRangeSelection can only be called after BeginTimeRangeSelection");
			}
			else
			{
				this.selectedCurves = this.s_SelectionBackup;
				this.s_TimeRangeSelectionActive = false;
			}
		}

		private void StartEditingSelectedPointsContext(object fieldPosition)
		{
			this.StartEditingSelectedPoints((Vector2)fieldPosition);
		}

		private void StartEditingSelectedPoints()
		{
			float num = this.selectedCurves.Min((CurveSelection x) => this.GetKeyframeFromSelection(x).time);
			float num2 = this.selectedCurves.Max((CurveSelection x) => this.GetKeyframeFromSelection(x).time);
			float num3 = this.selectedCurves.Min((CurveSelection x) => this.GetKeyframeFromSelection(x).value);
			float num4 = this.selectedCurves.Max((CurveSelection x) => this.GetKeyframeFromSelection(x).value);
			Vector2 fieldPosition = new Vector2(num + num2, num3 + num4) * 0.5f;
			this.StartEditingSelectedPoints(fieldPosition);
		}

		private void StartEditingSelectedPoints(Vector2 fieldPosition)
		{
			this.pointEditingFieldPosition = fieldPosition;
			this.focusedPointField = "pointValueField";
			this.timeWasEdited = (this.valueWasEdited = false);
			this.editingPoints = true;
		}

		private void FinishEditingPoints()
		{
			this.editingPoints = false;
		}

		private void EditSelectedPoints()
		{
			Event current = Event.current;
			if (this.editingPoints && !this.hasSelection)
			{
				this.editingPoints = false;
			}
			bool flag = false;
			if (current.type == EventType.KeyDown)
			{
				if (current.keyCode == KeyCode.KeypadEnter || current.keyCode == KeyCode.Return)
				{
					if (this.hasSelection && !this.editingPoints)
					{
						this.StartEditingSelectedPoints();
						current.Use();
					}
					else if (this.editingPoints)
					{
						this.FinishEditingPoints();
						current.Use();
					}
				}
				else if (current.keyCode == KeyCode.Escape)
				{
					flag = true;
				}
			}
			if (this.editingPoints)
			{
				Vector2 vector = base.DrawingToViewTransformPoint(this.pointEditingFieldPosition);
				Rect rect = Rect.MinMaxRect(base.leftmargin, base.topmargin, base.rect.width - base.rightmargin, base.rect.height - base.bottommargin);
				vector.x = Mathf.Clamp(vector.x, rect.xMin, rect.xMax - 80f);
				vector.y = Mathf.Clamp(vector.y, rect.yMin, rect.yMax - 36f);
				EditorGUI.BeginChangeCheck();
				GUI.SetNextControlName("pointTimeField");
				float x2 = this.PointFieldForSelection(new Rect(vector.x, vector.y, 80f, 18f), 1, (CurveSelection x) => this.GetKeyframeFromSelection(x).time, (Rect r, int id, float time) => base.TimeField(r, id, time, this.invSnap, this.timeFormat), "time");
				if (EditorGUI.EndChangeCheck())
				{
					this.timeWasEdited = true;
				}
				EditorGUI.BeginChangeCheck();
				GUI.SetNextControlName("pointValueField");
				float y = this.PointFieldForSelection(new Rect(vector.x, vector.y + 18f, 80f, 18f), 2, (CurveSelection x) => this.GetKeyframeFromSelection(x).value, (Rect r, int id, float value) => base.ValueField(r, id, value), "value");
				if (EditorGUI.EndChangeCheck())
				{
					this.valueWasEdited = true;
				}
				if (this.timeWasEdited || this.valueWasEdited)
				{
					this.SetSelectedKeyPositions(new Vector2(x2, y), this.timeWasEdited, this.valueWasEdited);
				}
				if (flag)
				{
					this.FinishEditingPoints();
				}
				if (this.focusedPointField != null)
				{
					EditorGUI.FocusTextInControl(this.focusedPointField);
					if (current.type == EventType.Repaint)
					{
						this.focusedPointField = null;
					}
				}
				if (current.type == EventType.KeyDown && current.character == '\t')
				{
					this.focusedPointField = ((!(GUI.GetNameOfFocusedControl() == "pointValueField")) ? "pointValueField" : "pointTimeField");
					current.Use();
				}
				if (current.type == EventType.MouseDown)
				{
					this.FinishEditingPoints();
				}
			}
		}

		private float PointFieldForSelection(Rect rect, int customID, Func<CurveSelection, float> memberGetter, Func<Rect, int, float, float> memberSetter, string label)
		{
			float arg = 0f;
			if (this.selectedCurves.All((CurveSelection x) => memberGetter(x) == memberGetter(this.selectedCurves[0])))
			{
				arg = memberGetter(this.selectedCurves[0]);
			}
			else
			{
				EditorGUI.showMixedValue = true;
			}
			Rect position = rect;
			position.x -= position.width;
			GUIStyle label2 = GUI.skin.label;
			label2.alignment = TextAnchor.UpperRight;
			int controlID = GUIUtility.GetControlID(customID, FocusType.Keyboard, rect);
			Color color = GUI.color;
			GUI.color = Color.white;
			GUI.Label(position, label, label2);
			float result = memberSetter(rect, controlID, arg);
			GUI.color = color;
			EditorGUI.showMixedValue = false;
			return result;
		}

		private void SetupKeyOrCurveDragging(Vector2 timeValue, CurveWrapper cw, int id, Vector2 mousePos)
		{
			this.m_DraggedCoord = timeValue;
			this.m_DraggingKey = cw;
			GUIUtility.hotControl = id;
			if (this.syncTimeDuringDrag)
			{
				this.activeTime = timeValue.x + cw.timeOffset;
			}
			this.s_StartMouseDragPosition = mousePos;
			this.s_StartClickedTime = timeValue.x;
		}

		public Vector2 MovePoints()
		{
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			Vector2 result;
			if (!this.hasSelection && !this.settings.allowDraggingCurvesAndRegions)
			{
				result = Vector2.zero;
			}
			else
			{
				Event current = Event.current;
				switch (current.GetTypeForControl(controlID))
				{
				case EventType.MouseDown:
					if (current.button == 0)
					{
						foreach (CurveSelection current2 in this.selectedCurves)
						{
							CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(current2);
							if (curveWrapperFromSelection != null && !curveWrapperFromSelection.hidden)
							{
								if ((this.DrawingToOffsetViewTransformPoint(curveWrapperFromSelection, this.GetPosition(current2)) - current.mousePosition).sqrMagnitude <= 100f)
								{
									Keyframe keyframeFromSelection = this.GetKeyframeFromSelection(current2);
									this.SetupKeyOrCurveDragging(new Vector2(keyframeFromSelection.time, keyframeFromSelection.value), curveWrapperFromSelection, controlID, current.mousePosition);
									current.Use();
									break;
								}
							}
						}
						if (this.settings.allowDraggingCurvesAndRegions && this.m_DraggingKey == null)
						{
							Vector2 zero = Vector2.zero;
							CurveWrapper[] array = null;
							if (this.HandleCurveAndRegionMoveToFrontOnMouseDown(ref zero, ref array))
							{
								List<CurveSelection> list = new List<CurveSelection>();
								CurveWrapper[] array2 = array;
								for (int i = 0; i < array2.Length; i++)
								{
									CurveWrapper curveWrapper = array2[i];
									for (int j = 0; j < curveWrapper.curve.keys.Length; j++)
									{
										list.Add(new CurveSelection(curveWrapper.id, j));
									}
									this.MoveCurveToFront(curveWrapper.id);
								}
								this.preCurveDragSelection = this.selectedCurves;
								this.selectedCurves = list;
								this.SetupKeyOrCurveDragging(zero, array[0], controlID, current.mousePosition);
								this.m_DraggingCurveOrRegion = array;
								current.Use();
							}
						}
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlID)
					{
						if (this.InLiveEdit())
						{
							this.EndLiveEdit();
						}
						this.ResetDragging();
						GUI.changed = true;
						current.Use();
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID)
					{
						Vector2 lhs = current.mousePosition - this.s_StartMouseDragPosition;
						Vector3 v = Vector3.zero;
						if (current.shift && this.m_AxisLock == CurveEditor.AxisLock.None)
						{
							this.m_AxisLock = ((Mathf.Abs(lhs.x) <= Mathf.Abs(lhs.y)) ? CurveEditor.AxisLock.Y : CurveEditor.AxisLock.X);
						}
						if (this.m_DraggingCurveOrRegion != null)
						{
							lhs.x = 0f;
							v = base.ViewToDrawingTransformVector(lhs);
							v.y = this.SnapValue(v.y + this.s_StartKeyDragPosition.y) - this.s_StartKeyDragPosition.y;
						}
						else
						{
							CurveEditor.AxisLock axisLock = this.m_AxisLock;
							if (axisLock != CurveEditor.AxisLock.None)
							{
								if (axisLock != CurveEditor.AxisLock.X)
								{
									if (axisLock == CurveEditor.AxisLock.Y)
									{
										lhs.x = 0f;
										v = base.ViewToDrawingTransformVector(lhs);
										v.y = this.SnapValue(v.y + this.s_StartKeyDragPosition.y) - this.s_StartKeyDragPosition.y;
									}
								}
								else
								{
									lhs.y = 0f;
									v = base.ViewToDrawingTransformVector(lhs);
									v.x = this.SnapTime(v.x + this.s_StartKeyDragPosition.x) - this.s_StartKeyDragPosition.x;
								}
							}
							else
							{
								v = base.ViewToDrawingTransformVector(lhs);
								v.x = this.SnapTime(v.x + this.s_StartKeyDragPosition.x) - this.s_StartKeyDragPosition.x;
								v.y = this.SnapValue(v.y + this.s_StartKeyDragPosition.y) - this.s_StartKeyDragPosition.y;
							}
						}
						if (!this.InLiveEdit())
						{
							this.StartLiveEdit();
						}
						this.TranslateSelectedKeys(v);
						GUI.changed = true;
						current.Use();
						result = v;
						return result;
					}
					break;
				case EventType.KeyDown:
					if (GUIUtility.hotControl == controlID && current.keyCode == KeyCode.Escape)
					{
						this.TranslateSelectedKeys(Vector2.zero);
						this.ResetDragging();
						GUI.changed = true;
						current.Use();
					}
					break;
				case EventType.Repaint:
				{
					Rect position = new Rect(current.mousePosition.x - 10f, current.mousePosition.y - 10f, 20f, 20f);
					if (this.m_DraggingCurveOrRegion != null)
					{
						EditorGUIUtility.AddCursorRect(position, MouseCursor.ResizeVertical);
					}
					else if (this.m_DraggingKey != null)
					{
						EditorGUIUtility.AddCursorRect(position, MouseCursor.MoveArrow);
					}
					break;
				}
				}
				result = Vector2.zero;
			}
			return result;
		}

		private void ResetDragging()
		{
			if (this.m_DraggingCurveOrRegion != null)
			{
				this.selectedCurves = this.preCurveDragSelection;
				this.preCurveDragSelection = null;
			}
			GUIUtility.hotControl = 0;
			this.m_DraggingKey = null;
			this.m_DraggingCurveOrRegion = null;
			this.m_MoveCoord = Vector2.zero;
			this.m_AxisLock = CurveEditor.AxisLock.None;
		}

		private void MakeCurveBackups()
		{
			this.SaveKeySelection("Edit Curve");
			this.m_CurveBackups = new List<CurveEditor.SavedCurve>();
			int num = -1;
			CurveEditor.SavedCurve savedCurve = null;
			for (int i = 0; i < this.selectedCurves.Count; i++)
			{
				CurveSelection curveSelection = this.selectedCurves[i];
				if (curveSelection.curveID != num)
				{
					AnimationCurve curveFromSelection = this.GetCurveFromSelection(curveSelection);
					if (curveFromSelection != null)
					{
						savedCurve = new CurveEditor.SavedCurve();
						num = (savedCurve.curveId = curveSelection.curveID);
						Keyframe[] keys = curveFromSelection.keys;
						savedCurve.keys = new List<CurveEditor.SavedCurve.SavedKeyFrame>(keys.Length);
						Keyframe[] array = keys;
						for (int j = 0; j < array.Length; j++)
						{
							Keyframe key = array[j];
							savedCurve.keys.Add(new CurveEditor.SavedCurve.SavedKeyFrame(key, CurveWrapper.SelectionMode.None));
						}
						this.m_CurveBackups.Add(savedCurve);
					}
				}
				savedCurve.keys[curveSelection.key].selected = ((!curveSelection.semiSelected) ? CurveWrapper.SelectionMode.Selected : CurveWrapper.SelectionMode.SemiSelected);
			}
		}

		public void SaveKeySelection(string undoLabel)
		{
			if (this.settings.undoRedoSelection)
			{
				Undo.RegisterCompleteObjectUndo(this.selection, undoLabel);
			}
		}

		private Vector2 GetPosition(CurveSelection selection)
		{
			Keyframe keyframeFromSelection = this.GetKeyframeFromSelection(selection);
			Vector2 vector = new Vector2(keyframeFromSelection.time, keyframeFromSelection.value);
			float d = 50f;
			Vector2 result;
			if (selection.type == CurveSelection.SelectionType.InTangent)
			{
				Vector2 vector2 = new Vector2(1f, keyframeFromSelection.inTangent);
				if (keyframeFromSelection.inTangent == float.PositiveInfinity)
				{
					vector2 = new Vector2(0f, -1f);
				}
				vector2 = base.NormalizeInViewSpace(vector2);
				result = vector - vector2 * d;
			}
			else if (selection.type == CurveSelection.SelectionType.OutTangent)
			{
				Vector2 vector3 = new Vector2(1f, keyframeFromSelection.outTangent);
				if (keyframeFromSelection.outTangent == float.PositiveInfinity)
				{
					vector3 = new Vector2(0f, -1f);
				}
				vector3 = base.NormalizeInViewSpace(vector3);
				result = vector + vector3 * d;
			}
			else
			{
				result = vector;
			}
			return result;
		}

		private void MoveCurveToFront(int curveID)
		{
			int count = this.m_DrawOrder.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.m_DrawOrder[i] == curveID)
				{
					int regionId = this.GetCurveWrapperFromID(curveID).regionId;
					if (regionId >= 0)
					{
						int num = 0;
						int num2 = -1;
						if (i - 1 >= 0)
						{
							int num3 = this.m_DrawOrder[i - 1];
							if (regionId == this.GetCurveWrapperFromID(num3).regionId)
							{
								num2 = num3;
								num = -1;
							}
						}
						if (i + 1 < count && num2 < 0)
						{
							int num4 = this.m_DrawOrder[i + 1];
							if (regionId == this.GetCurveWrapperFromID(num4).regionId)
							{
								num2 = num4;
								num = 0;
							}
						}
						if (num2 >= 0)
						{
							this.m_DrawOrder.RemoveRange(i + num, 2);
							this.m_DrawOrder.Add(num2);
							this.m_DrawOrder.Add(curveID);
							this.ValidateCurveList();
							break;
						}
						Debug.LogError("Unhandled region");
					}
					else
					{
						if (i == count - 1)
						{
							break;
						}
						this.m_DrawOrder.RemoveAt(i);
						this.m_DrawOrder.Add(curveID);
						this.ValidateCurveList();
						break;
					}
				}
			}
		}

		private bool IsCurveSelected(CurveWrapper cw)
		{
			return cw != null && cw.selected != CurveWrapper.SelectionMode.None;
		}

		private bool IsRegionCurveSelected(CurveWrapper cw1, CurveWrapper cw2)
		{
			return this.IsCurveSelected(cw1) || this.IsCurveSelected(cw2);
		}

		private bool IsRegion(CurveWrapper cw1, CurveWrapper cw2)
		{
			return cw1 != null && cw2 != null && cw1.regionId >= 0 && cw1.regionId == cw2.regionId;
		}

		private bool IsLeftTangentEditable(CurveSelection selection)
		{
			Keyframe keyframeFromSelection = this.GetKeyframeFromSelection(selection);
			AnimationUtility.TangentMode keyLeftTangentMode = AnimationUtility.GetKeyLeftTangentMode(keyframeFromSelection);
			return keyLeftTangentMode == AnimationUtility.TangentMode.Free || (keyLeftTangentMode == AnimationUtility.TangentMode.ClampedAuto || keyLeftTangentMode == AnimationUtility.TangentMode.Auto);
		}

		private bool IsRightTangentEditable(CurveSelection selection)
		{
			Keyframe keyframeFromSelection = this.GetKeyframeFromSelection(selection);
			AnimationUtility.TangentMode keyRightTangentMode = AnimationUtility.GetKeyRightTangentMode(keyframeFromSelection);
			return keyRightTangentMode == AnimationUtility.TangentMode.Free || (keyRightTangentMode == AnimationUtility.TangentMode.ClampedAuto || keyRightTangentMode == AnimationUtility.TangentMode.Auto);
		}

		private void DrawCurvesAndRegion(CurveWrapper cw1, CurveWrapper cw2, List<CurveSelection> selection, bool hasFocus)
		{
			this.DrawRegion(cw1, cw2, hasFocus);
			this.DrawCurveAndPoints(cw1, (!this.IsCurveSelected(cw1)) ? null : selection, hasFocus);
			this.DrawCurveAndPoints(cw2, (!this.IsCurveSelected(cw2)) ? null : selection, hasFocus);
		}

		private void DrawCurveAndPoints(CurveWrapper cw, List<CurveSelection> selection, bool hasFocus)
		{
			this.DrawCurve(cw, hasFocus);
			this.DrawPointsOnCurve(cw, selection, hasFocus);
		}

		private bool ShouldCurveHaveFocus(int indexIntoDrawOrder, CurveWrapper cw1, CurveWrapper cw2)
		{
			bool result = false;
			if (indexIntoDrawOrder == this.m_DrawOrder.Count - 1)
			{
				result = true;
			}
			else if (this.hasSelection)
			{
				result = (this.IsCurveSelected(cw1) || this.IsCurveSelected(cw2));
			}
			return result;
		}

		private void DrawCurves()
		{
			if (Event.current.type == EventType.Repaint)
			{
				this.m_PointRenderer.Clear();
				for (int i = 0; i < this.m_DrawOrder.Count; i++)
				{
					CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(this.m_DrawOrder[i]);
					if (curveWrapperFromID != null)
					{
						if (!curveWrapperFromID.hidden)
						{
							if (curveWrapperFromID.curve.length != 0)
							{
								CurveWrapper cw = null;
								if (i < this.m_DrawOrder.Count - 1)
								{
									cw = this.GetCurveWrapperFromID(this.m_DrawOrder[i + 1]);
								}
								if (this.IsRegion(curveWrapperFromID, cw))
								{
									i++;
									bool hasFocus = this.ShouldCurveHaveFocus(i, curveWrapperFromID, cw);
									this.DrawCurvesAndRegion(curveWrapperFromID, cw, (!this.IsRegionCurveSelected(curveWrapperFromID, cw)) ? null : this.selectedCurves, hasFocus);
								}
								else
								{
									bool hasFocus2 = this.ShouldCurveHaveFocus(i, curveWrapperFromID, null);
									this.DrawCurveAndPoints(curveWrapperFromID, (!this.IsCurveSelected(curveWrapperFromID)) ? null : this.selectedCurves, hasFocus2);
								}
							}
						}
					}
				}
				this.m_PointRenderer.Render();
			}
		}

		private void DrawCurvesTangents()
		{
			if (this.m_DraggingCurveOrRegion == null)
			{
				HandleUtility.ApplyWireMaterial();
				GL.Begin(1);
				GL.Color(this.m_TangentColor * new Color(1f, 1f, 1f, 0.75f));
				for (int i = 0; i < this.selectedCurves.Count; i++)
				{
					CurveSelection curveSelection = this.selectedCurves[i];
					if (!curveSelection.semiSelected)
					{
						Vector2 position = this.GetPosition(curveSelection);
						CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(curveSelection);
						if (curveWrapperFromSelection != null)
						{
							AnimationCurve curve = curveWrapperFromSelection.curve;
							if (this.IsLeftTangentEditable(curveSelection) && this.GetKeyframeFromSelection(curveSelection).time != curve.keys[0].time)
							{
								Vector2 position2 = this.GetPosition(new CurveSelection(curveSelection.curveID, curveSelection.key, CurveSelection.SelectionType.InTangent));
								this.DrawCurveLine(curveWrapperFromSelection, position2, position);
							}
							if (this.IsRightTangentEditable(curveSelection) && this.GetKeyframeFromSelection(curveSelection).time != curve.keys[curve.keys.Length - 1].time)
							{
								Vector2 position3 = this.GetPosition(new CurveSelection(curveSelection.curveID, curveSelection.key, CurveSelection.SelectionType.OutTangent));
								this.DrawCurveLine(curveWrapperFromSelection, position, position3);
							}
						}
					}
				}
				GL.End();
				this.m_PointRenderer.Clear();
				GUI.color = this.m_TangentColor;
				for (int j = 0; j < this.selectedCurves.Count; j++)
				{
					CurveSelection curveSelection2 = this.selectedCurves[j];
					if (!curveSelection2.semiSelected)
					{
						CurveWrapper curveWrapperFromSelection2 = this.GetCurveWrapperFromSelection(curveSelection2);
						if (curveWrapperFromSelection2 != null)
						{
							AnimationCurve curve2 = curveWrapperFromSelection2.curve;
							if (this.IsLeftTangentEditable(curveSelection2) && this.GetKeyframeFromSelection(curveSelection2).time != curve2.keys[0].time)
							{
								Vector2 viewPos = this.DrawingToOffsetViewTransformPoint(curveWrapperFromSelection2, this.GetPosition(new CurveSelection(curveSelection2.curveID, curveSelection2.key, CurveSelection.SelectionType.InTangent)));
								this.DrawPoint(viewPos, CurveWrapper.SelectionMode.None);
							}
							if (this.IsRightTangentEditable(curveSelection2) && this.GetKeyframeFromSelection(curveSelection2).time != curve2.keys[curve2.keys.Length - 1].time)
							{
								Vector2 viewPos2 = this.DrawingToOffsetViewTransformPoint(curveWrapperFromSelection2, this.GetPosition(new CurveSelection(curveSelection2.curveID, curveSelection2.key, CurveSelection.SelectionType.OutTangent)));
								this.DrawPoint(viewPos2, CurveWrapper.SelectionMode.None);
							}
						}
					}
				}
				this.m_PointRenderer.Render();
			}
		}

		private void DrawCurvesOverlay()
		{
			if (this.m_DraggingCurveOrRegion == null)
			{
				if (this.m_DraggingKey != null && this.settings.rectangleToolFlags == CurveEditorSettings.RectangleToolFlags.NoRectangleTool)
				{
					GUI.color = Color.white;
					float num = base.vRangeMin;
					float num2 = base.vRangeMax;
					num = Mathf.Max(num, this.m_DraggingKey.vRangeMin);
					num2 = Mathf.Min(num2, this.m_DraggingKey.vRangeMax);
					Vector2 lhs = this.m_DraggedCoord + this.m_MoveCoord;
					lhs.x = Mathf.Clamp(lhs.x, base.hRangeMin, base.hRangeMax);
					lhs.y = Mathf.Clamp(lhs.y, num, num2);
					Vector2 vector = this.DrawingToOffsetViewTransformPoint(this.m_DraggingKey, lhs);
					Vector2 vector2 = (this.m_DraggingKey.getAxisUiScalarsCallback == null) ? Vector2.one : this.m_DraggingKey.getAxisUiScalarsCallback();
					if (vector2.x >= 0f)
					{
						lhs.x *= vector2.x;
					}
					if (vector2.y >= 0f)
					{
						lhs.y *= vector2.y;
					}
					GUIContent content = new GUIContent(string.Format("{0}, {1}", base.FormatTime(lhs.x, this.invSnap, this.timeFormat), base.FormatValue(lhs.y)));
					Vector2 vector3 = this.styles.dragLabel.CalcSize(content);
					EditorGUI.DoDropShadowLabel(new Rect(vector.x, vector.y - vector3.y, vector3.x, vector3.y), content, this.styles.dragLabel, 0.3f);
				}
			}
		}

		private List<Vector3> CreateRegion(CurveWrapper minCurve, CurveWrapper maxCurve, float deltaTime)
		{
			List<Vector3> list = new List<Vector3>();
			List<float> list2 = new List<float>();
			float num;
			for (num = deltaTime; num <= 1f; num += deltaTime)
			{
				list2.Add(num);
			}
			if (num != 1f)
			{
				list2.Add(1f);
			}
			Keyframe[] keys = maxCurve.curve.keys;
			for (int i = 0; i < keys.Length; i++)
			{
				Keyframe keyframe = keys[i];
				if (keyframe.time > 0f && keyframe.time < 1f)
				{
					list2.Add(keyframe.time - 0.0001f);
					list2.Add(keyframe.time);
					list2.Add(keyframe.time + 0.0001f);
				}
			}
			Keyframe[] keys2 = minCurve.curve.keys;
			for (int j = 0; j < keys2.Length; j++)
			{
				Keyframe keyframe2 = keys2[j];
				if (keyframe2.time > 0f && keyframe2.time < 1f)
				{
					list2.Add(keyframe2.time - 0.0001f);
					list2.Add(keyframe2.time);
					list2.Add(keyframe2.time + 0.0001f);
				}
			}
			list2.Sort();
			Vector3 v = new Vector3(0f, maxCurve.renderer.EvaluateCurveSlow(0f), 0f);
			Vector3 v2 = new Vector3(0f, minCurve.renderer.EvaluateCurveSlow(0f), 0f);
			Vector3 vector = this.DrawingToOffsetViewMatrix(maxCurve).MultiplyPoint(v);
			Vector3 vector2 = this.DrawingToOffsetViewMatrix(minCurve).MultiplyPoint(v2);
			for (int k = 0; k < list2.Count; k++)
			{
				float num2 = list2[k];
				Vector3 vector3 = new Vector3(num2, maxCurve.renderer.EvaluateCurveSlow(num2), 0f);
				Vector3 vector4 = new Vector3(num2, minCurve.renderer.EvaluateCurveSlow(num2), 0f);
				Vector3 vector5 = this.DrawingToOffsetViewMatrix(maxCurve).MultiplyPoint(vector3);
				Vector3 vector6 = this.DrawingToOffsetViewMatrix(minCurve).MultiplyPoint(vector4);
				if (v.y >= v2.y && vector3.y >= vector4.y)
				{
					list.Add(vector);
					list.Add(vector6);
					list.Add(vector2);
					list.Add(vector);
					list.Add(vector5);
					list.Add(vector6);
				}
				else if (v.y <= v2.y && vector3.y <= vector4.y)
				{
					list.Add(vector2);
					list.Add(vector5);
					list.Add(vector);
					list.Add(vector2);
					list.Add(vector6);
					list.Add(vector5);
				}
				else
				{
					Vector2 zero = Vector2.zero;
					if (Mathf.LineIntersection(vector, vector5, vector2, vector6, ref zero))
					{
						list.Add(vector);
						list.Add(zero);
						list.Add(vector2);
						list.Add(vector5);
						list.Add(zero);
						list.Add(vector6);
					}
					else
					{
						Debug.Log("Error: No intersection found! There should be one...");
					}
				}
				v = vector3;
				v2 = vector4;
				vector = vector5;
				vector2 = vector6;
			}
			return list;
		}

		public void DrawRegion(CurveWrapper curve1, CurveWrapper curve2, bool hasFocus)
		{
			if (Event.current.type == EventType.Repaint)
			{
				float deltaTime = 1f / (base.rect.width / 10f);
				List<Vector3> list = this.CreateRegion(curve1, curve2, deltaTime);
				Color color = curve1.color;
				if (this.IsDraggingRegion(curve1, curve2))
				{
					color = Color.Lerp(color, Color.black, 0.1f);
					color.a = 0.4f;
				}
				else if (this.settings.useFocusColors && !hasFocus)
				{
					color *= 0.4f;
					color.a = 0.1f;
				}
				else
				{
					color *= 1f;
					color.a = 0.4f;
				}
				Shader.SetGlobalColor("_HandleColor", color);
				HandleUtility.ApplyWireMaterial();
				GL.Begin(4);
				int num = list.Count / 3;
				for (int i = 0; i < num; i++)
				{
					GL.Color(color);
					GL.Vertex(list[i * 3]);
					GL.Vertex(list[i * 3 + 1]);
					GL.Vertex(list[i * 3 + 2]);
				}
				GL.End();
			}
		}

		private void DrawCurve(CurveWrapper cw, bool hasFocus)
		{
			CurveRenderer renderer = cw.renderer;
			Color color = cw.color;
			if (this.IsDraggingCurve(cw) || cw.selected == CurveWrapper.SelectionMode.Selected)
			{
				color = Color.Lerp(color, Color.white, 0.3f);
			}
			else if (this.settings.useFocusColors && !hasFocus)
			{
				color *= 0.5f;
				color.a = 0.8f;
			}
			Rect shownArea = base.shownArea;
			renderer.DrawCurve(shownArea.xMin - cw.timeOffset, shownArea.xMax, color, this.DrawingToOffsetViewMatrix(cw), this.settings.wrapColor * cw.wrapColorMultiplier);
		}

		private void DrawPointsOnCurve(CurveWrapper cw, List<CurveSelection> selected, bool hasFocus)
		{
			this.m_PreviousDrawPointCenter = new Vector2(-3.40282347E+38f, -3.40282347E+38f);
			if (selected == null)
			{
				Color color = cw.color;
				if (this.settings.useFocusColors && !hasFocus)
				{
					color *= 0.5f;
				}
				GUI.color = color;
				Keyframe[] keys = cw.curve.keys;
				for (int i = 0; i < keys.Length; i++)
				{
					Keyframe keyframe = keys[i];
					this.DrawPoint(this.DrawingToOffsetViewTransformPoint(cw, new Vector2(keyframe.time, keyframe.value)), CurveWrapper.SelectionMode.None);
				}
			}
			else
			{
				Color color2 = Color.Lerp(cw.color, Color.white, 0.2f);
				GUI.color = color2;
				int num = 0;
				while (num < selected.Count && selected[num].curveID != cw.id)
				{
					num++;
				}
				int num2 = 0;
				Keyframe[] keys2 = cw.curve.keys;
				for (int j = 0; j < keys2.Length; j++)
				{
					Keyframe keyframe2 = keys2[j];
					if (num < selected.Count && selected[num].key == num2 && selected[num].curveID == cw.id)
					{
						if (selected[num].semiSelected)
						{
							this.DrawPoint(this.DrawingToOffsetViewTransformPoint(cw, new Vector2(keyframe2.time, keyframe2.value)), CurveWrapper.SelectionMode.SemiSelected);
						}
						else
						{
							this.DrawPoint(this.DrawingToOffsetViewTransformPoint(cw, new Vector2(keyframe2.time, keyframe2.value)), CurveWrapper.SelectionMode.Selected, (this.settings.rectangleToolFlags != CurveEditorSettings.RectangleToolFlags.NoRectangleTool) ? MouseCursor.Arrow : MouseCursor.MoveArrow);
						}
						num++;
					}
					else
					{
						this.DrawPoint(this.DrawingToOffsetViewTransformPoint(cw, new Vector2(keyframe2.time, keyframe2.value)), CurveWrapper.SelectionMode.None);
					}
					num2++;
				}
				GUI.color = Color.white;
			}
		}

		private void DrawPoint(Vector2 viewPos, CurveWrapper.SelectionMode selected)
		{
			this.DrawPoint(viewPos, selected, MouseCursor.Arrow);
		}

		private void DrawPoint(Vector2 viewPos, CurveWrapper.SelectionMode selected, MouseCursor mouseCursor)
		{
			Rect rect = new Rect(Mathf.Floor(viewPos.x) - 4f, Mathf.Floor(viewPos.y) - 4f, (float)this.styles.pointIcon.width, (float)this.styles.pointIcon.height);
			Vector2 center = rect.center;
			if ((center - this.m_PreviousDrawPointCenter).magnitude > 8f)
			{
				if (selected == CurveWrapper.SelectionMode.None)
				{
					this.m_PointRenderer.AddPoint(rect, GUI.color);
				}
				else if (selected == CurveWrapper.SelectionMode.Selected)
				{
					this.m_PointRenderer.AddSelectedPoint(rect, GUI.color);
				}
				else
				{
					this.m_PointRenderer.AddSemiSelectedPoint(rect, GUI.color);
				}
				if (mouseCursor != MouseCursor.Arrow)
				{
					if (GUIUtility.hotControl == 0)
					{
						EditorGUIUtility.AddCursorRect(rect, mouseCursor);
					}
				}
				this.m_PreviousDrawPointCenter = center;
			}
		}

		private void DrawLine(Vector2 lhs, Vector2 rhs)
		{
			GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(lhs.x, lhs.y, 0f)));
			GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(rhs.x, rhs.y, 0f)));
		}

		private void DrawCurveLine(CurveWrapper cw, Vector2 lhs, Vector2 rhs)
		{
			GL.Vertex(this.DrawingToOffsetViewTransformPoint(cw, new Vector3(lhs.x, lhs.y, 0f)));
			GL.Vertex(this.DrawingToOffsetViewTransformPoint(cw, new Vector3(rhs.x, rhs.y, 0f)));
		}

		private void DrawWrapperPopups()
		{
			if (this.settings.showWrapperPopups)
			{
				int num;
				this.GetTopMostCurveID(out num);
				if (num != -1)
				{
					CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(num);
					AnimationCurve curve = curveWrapperFromID.curve;
					if (curve != null && curve.length >= 2 && curve.preWrapMode != WrapMode.Default)
					{
						Color contentColor = GUI.contentColor;
						Keyframe key = curve.keys[0];
						WrapMode wrapMode = (curve == null) ? WrapMode.Default : curve.preWrapMode;
						wrapMode = this.WrapModeIconPopup(key, wrapMode, -1.5f);
						if (curve != null && wrapMode != curve.preWrapMode)
						{
							curve.preWrapMode = wrapMode;
							curveWrapperFromID.changed = true;
						}
						Keyframe key2 = curve.keys[curve.length - 1];
						WrapMode wrapMode2 = (curve == null) ? WrapMode.Default : curve.postWrapMode;
						wrapMode2 = this.WrapModeIconPopup(key2, wrapMode2, 0.5f);
						if (curve != null && wrapMode2 != curve.postWrapMode)
						{
							curve.postWrapMode = wrapMode2;
							curveWrapperFromID.changed = true;
						}
						if (curveWrapperFromID.changed)
						{
							curveWrapperFromID.renderer.SetWrap(curve.preWrapMode, curve.postWrapMode);
							if (this.curvesUpdated != null)
							{
								this.curvesUpdated();
							}
						}
						GUI.contentColor = contentColor;
					}
				}
			}
		}

		private WrapMode WrapModeIconPopup(Keyframe key, WrapMode oldWrap, float hOffset)
		{
			float num = (float)this.styles.wrapModeMenuIcon.image.width;
			Vector3 lhs = new Vector3(key.time, key.value);
			lhs = base.DrawingToViewTransformPoint(lhs);
			Rect position = new Rect(lhs.x + num * hOffset, lhs.y + base.drawRect.y, num, num);
			Enum[] array = Enum.GetValues(typeof(WrapModeFixedCurve)).Cast<Enum>().ToArray<Enum>();
			string[] texts = (from x in Enum.GetNames(typeof(WrapModeFixedCurve))
			select ObjectNames.NicifyVariableName(x)).ToArray<string>();
			int selected = Array.IndexOf<Enum>(array, (WrapModeFixedCurve)oldWrap);
			int controlID = GUIUtility.GetControlID("WrapModeIconPopup".GetHashCode(), FocusType.Keyboard, position);
			int selectedValueForControl = EditorGUI.PopupCallbackInfo.GetSelectedValueForControl(controlID, selected);
			GUIContent[] options = EditorGUIUtility.TempContent(texts);
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.Repaint)
			{
				if (type != EventType.MouseDown)
				{
					if (type == EventType.KeyDown)
					{
						if (current.MainActionKeyForControl(controlID))
						{
							if (Application.platform == RuntimePlatform.OSXEditor)
							{
								position.y = position.y - (float)(selectedValueForControl * 16) - 19f;
							}
							EditorGUI.PopupCallbackInfo.instance = new EditorGUI.PopupCallbackInfo(controlID);
							EditorUtility.DisplayCustomMenu(position, options, selectedValueForControl, new EditorUtility.SelectMenuItemFunction(EditorGUI.PopupCallbackInfo.instance.SetEnumValueDelegate), null);
							current.Use();
						}
					}
				}
				else if (current.button == 0 && position.Contains(current.mousePosition))
				{
					if (Application.platform == RuntimePlatform.OSXEditor)
					{
						position.y = position.y - (float)(selectedValueForControl * 16) - 19f;
					}
					EditorGUI.PopupCallbackInfo.instance = new EditorGUI.PopupCallbackInfo(controlID);
					EditorUtility.DisplayCustomMenu(position, options, selectedValueForControl, new EditorUtility.SelectMenuItemFunction(EditorGUI.PopupCallbackInfo.instance.SetEnumValueDelegate), null);
					GUIUtility.keyboardControl = controlID;
					current.Use();
				}
			}
			else
			{
				GUIStyle.none.Draw(position, this.styles.wrapModeMenuIcon, controlID, false);
			}
			return (WrapMode)array[selectedValueForControl];
		}

		private Vector2 GetAxisUiScalars(List<CurveWrapper> curvesWithSameParameterSpace)
		{
			Vector2 result;
			if (this.selectedCurves.Count <= 1)
			{
				if (this.m_DrawOrder.Count > 0)
				{
					CurveWrapper curveWrapperFromID = this.GetCurveWrapperFromID(this.m_DrawOrder[this.m_DrawOrder.Count - 1]);
					if (curveWrapperFromID != null && curveWrapperFromID.getAxisUiScalarsCallback != null)
					{
						if (curvesWithSameParameterSpace != null)
						{
							curvesWithSameParameterSpace.Add(curveWrapperFromID);
						}
						result = curveWrapperFromID.getAxisUiScalarsCallback();
						return result;
					}
				}
				result = Vector2.one;
			}
			else
			{
				Vector2 vector = new Vector2(-1f, -1f);
				if (this.selectedCurves.Count > 1)
				{
					bool flag = true;
					bool flag2 = true;
					Vector2 vector2 = Vector2.one;
					for (int i = 0; i < this.selectedCurves.Count; i++)
					{
						CurveWrapper curveWrapperFromSelection = this.GetCurveWrapperFromSelection(this.selectedCurves[i]);
						if (curveWrapperFromSelection != null)
						{
							if (curveWrapperFromSelection.getAxisUiScalarsCallback != null)
							{
								Vector2 vector3 = curveWrapperFromSelection.getAxisUiScalarsCallback();
								if (i == 0)
								{
									vector2 = vector3;
								}
								else
								{
									if (Mathf.Abs(vector3.x - vector2.x) > 1E-05f)
									{
										flag = false;
									}
									if (Mathf.Abs(vector3.y - vector2.y) > 1E-05f)
									{
										flag2 = false;
									}
									vector2 = vector3;
								}
								if (curvesWithSameParameterSpace != null)
								{
									curvesWithSameParameterSpace.Add(curveWrapperFromSelection);
								}
							}
						}
					}
					if (flag)
					{
						vector.x = vector2.x;
					}
					if (flag2)
					{
						vector.y = vector2.y;
					}
				}
				result = vector;
			}
			return result;
		}

		private void SetAxisUiScalars(Vector2 newScalars, List<CurveWrapper> curvesInSameSpace)
		{
			foreach (CurveWrapper current in curvesInSameSpace)
			{
				Vector2 newAxisScalars = current.getAxisUiScalarsCallback();
				if (newScalars.x >= 0f)
				{
					newAxisScalars.x = newScalars.x;
				}
				if (newScalars.y >= 0f)
				{
					newAxisScalars.y = newScalars.y;
				}
				if (current.setAxisUiScalarsCallback != null)
				{
					current.setAxisUiScalarsCallback(newAxisScalars);
				}
			}
		}

		public void GridGUI()
		{
			if (Event.current.type == EventType.Repaint)
			{
				GUI.BeginClip(base.drawRect);
				Color color = GUI.color;
				Vector2 axisUiScalars = this.GetAxisUiScalars(null);
				Rect shownArea = base.shownArea;
				base.hTicks.SetRanges(shownArea.xMin * axisUiScalars.x, shownArea.xMax * axisUiScalars.x, base.drawRect.xMin, base.drawRect.xMax);
				base.vTicks.SetRanges(shownArea.yMin * axisUiScalars.y, shownArea.yMax * axisUiScalars.y, base.drawRect.yMin, base.drawRect.yMax);
				HandleUtility.ApplyWireMaterial();
				GL.Begin(1);
				base.hTicks.SetTickStrengths((float)this.settings.hTickStyle.distMin, (float)this.settings.hTickStyle.distFull, false);
				float num;
				float num2;
				if (this.settings.hTickStyle.stubs)
				{
					num = shownArea.yMin;
					num2 = shownArea.yMin - 40f / base.scale.y;
				}
				else
				{
					num = Mathf.Max(shownArea.yMin, base.vRangeMin);
					num2 = Mathf.Min(shownArea.yMax, base.vRangeMax);
				}
				for (int i = 0; i < base.hTicks.tickLevels; i++)
				{
					float strengthOfLevel = base.hTicks.GetStrengthOfLevel(i);
					if (strengthOfLevel > 0f)
					{
						GL.Color(this.settings.hTickStyle.tickColor * new Color(1f, 1f, 1f, strengthOfLevel) * new Color(1f, 1f, 1f, 0.75f));
						float[] ticksAtLevel = base.hTicks.GetTicksAtLevel(i, true);
						for (int j = 0; j < ticksAtLevel.Length; j++)
						{
							ticksAtLevel[j] /= axisUiScalars.x;
							if (ticksAtLevel[j] > base.hRangeMin && ticksAtLevel[j] < base.hRangeMax)
							{
								this.DrawLine(new Vector2(ticksAtLevel[j], num), new Vector2(ticksAtLevel[j], num2));
							}
						}
					}
				}
				GL.Color(this.settings.hTickStyle.tickColor * new Color(1f, 1f, 1f, 1f) * new Color(1f, 1f, 1f, 0.75f));
				if (base.hRangeMin != float.NegativeInfinity)
				{
					this.DrawLine(new Vector2(base.hRangeMin, num), new Vector2(base.hRangeMin, num2));
				}
				if (base.hRangeMax != float.PositiveInfinity)
				{
					this.DrawLine(new Vector2(base.hRangeMax, num), new Vector2(base.hRangeMax, num2));
				}
				base.vTicks.SetTickStrengths((float)this.settings.vTickStyle.distMin, (float)this.settings.vTickStyle.distFull, false);
				if (this.settings.vTickStyle.stubs)
				{
					num = shownArea.xMin;
					num2 = shownArea.xMin + 40f / base.scale.x;
				}
				else
				{
					num = Mathf.Max(shownArea.xMin, base.hRangeMin);
					num2 = Mathf.Min(shownArea.xMax, base.hRangeMax);
				}
				for (int k = 0; k < base.vTicks.tickLevels; k++)
				{
					float strengthOfLevel2 = base.vTicks.GetStrengthOfLevel(k);
					if (strengthOfLevel2 > 0f)
					{
						GL.Color(this.settings.vTickStyle.tickColor * new Color(1f, 1f, 1f, strengthOfLevel2) * new Color(1f, 1f, 1f, 0.75f));
						float[] ticksAtLevel2 = base.vTicks.GetTicksAtLevel(k, true);
						for (int l = 0; l < ticksAtLevel2.Length; l++)
						{
							ticksAtLevel2[l] /= axisUiScalars.y;
							if (ticksAtLevel2[l] > base.vRangeMin && ticksAtLevel2[l] < base.vRangeMax)
							{
								this.DrawLine(new Vector2(num, ticksAtLevel2[l]), new Vector2(num2, ticksAtLevel2[l]));
							}
						}
					}
				}
				GL.Color(this.settings.vTickStyle.tickColor * new Color(1f, 1f, 1f, 1f) * new Color(1f, 1f, 1f, 0.75f));
				if (base.vRangeMin != float.NegativeInfinity)
				{
					this.DrawLine(new Vector2(num, base.vRangeMin), new Vector2(num2, base.vRangeMin));
				}
				if (base.vRangeMax != float.PositiveInfinity)
				{
					this.DrawLine(new Vector2(num, base.vRangeMax), new Vector2(num2, base.vRangeMax));
				}
				GL.End();
				if (this.settings.showAxisLabels)
				{
					if (this.settings.hTickStyle.distLabel > 0 && axisUiScalars.x > 0f)
					{
						GUI.color = this.settings.hTickStyle.labelColor;
						int levelWithMinSeparation = base.hTicks.GetLevelWithMinSeparation((float)this.settings.hTickStyle.distLabel);
						int numberOfDecimalsForMinimumDifference = MathUtils.GetNumberOfDecimalsForMinimumDifference(base.hTicks.GetPeriodOfLevel(levelWithMinSeparation));
						float[] ticksAtLevel3 = base.hTicks.GetTicksAtLevel(levelWithMinSeparation, false);
						float[] array = (float[])ticksAtLevel3.Clone();
						float y = Mathf.Floor(base.drawRect.height);
						for (int m = 0; m < ticksAtLevel3.Length; m++)
						{
							array[m] /= axisUiScalars.x;
							if (array[m] >= base.hRangeMin && array[m] <= base.hRangeMax)
							{
								Vector2 vector = base.DrawingToViewTransformPoint(new Vector2(array[m], 0f));
								vector = new Vector2(Mathf.Floor(vector.x), y);
								float num3 = ticksAtLevel3[m];
								TextAnchor textAnchor;
								Rect position;
								if (this.settings.hTickStyle.centerLabel)
								{
									textAnchor = TextAnchor.UpperCenter;
									position = new Rect(vector.x, vector.y - 16f - this.settings.hTickLabelOffset, 1f, 16f);
								}
								else
								{
									textAnchor = TextAnchor.UpperLeft;
									position = new Rect(vector.x, vector.y - 16f - this.settings.hTickLabelOffset, 50f, 16f);
								}
								if (this.styles.labelTickMarksX.alignment != textAnchor)
								{
									this.styles.labelTickMarksX.alignment = textAnchor;
								}
								GUI.Label(position, num3.ToString("n" + numberOfDecimalsForMinimumDifference) + this.settings.hTickStyle.unit, this.styles.labelTickMarksX);
							}
						}
					}
					if (this.settings.vTickStyle.distLabel > 0 && axisUiScalars.y > 0f)
					{
						GUI.color = this.settings.vTickStyle.labelColor;
						int levelWithMinSeparation2 = base.vTicks.GetLevelWithMinSeparation((float)this.settings.vTickStyle.distLabel);
						float[] ticksAtLevel4 = base.vTicks.GetTicksAtLevel(levelWithMinSeparation2, false);
						float[] array2 = (float[])ticksAtLevel4.Clone();
						int numberOfDecimalsForMinimumDifference2 = MathUtils.GetNumberOfDecimalsForMinimumDifference(base.vTicks.GetPeriodOfLevel(levelWithMinSeparation2));
						string text = "n" + numberOfDecimalsForMinimumDifference2;
						this.m_AxisLabelFormat = text;
						float width = 35f;
						if (!this.settings.vTickStyle.stubs && ticksAtLevel4.Length > 1)
						{
							float num4 = ticksAtLevel4[1];
							float num5 = ticksAtLevel4[ticksAtLevel4.Length - 1];
							string text2 = num4.ToString(text) + this.settings.vTickStyle.unit;
							string text3 = num5.ToString(text) + this.settings.vTickStyle.unit;
							width = Mathf.Max(this.styles.labelTickMarksY.CalcSize(new GUIContent(text2)).x, this.styles.labelTickMarksY.CalcSize(new GUIContent(text3)).x) + 6f;
						}
						for (int n = 0; n < ticksAtLevel4.Length; n++)
						{
							array2[n] /= axisUiScalars.y;
							if (array2[n] >= base.vRangeMin && array2[n] <= base.vRangeMax)
							{
								Vector2 vector2 = base.DrawingToViewTransformPoint(new Vector2(0f, array2[n]));
								vector2 = new Vector2(vector2.x, Mathf.Floor(vector2.y));
								float num6 = ticksAtLevel4[n];
								Rect position2;
								if (this.settings.vTickStyle.centerLabel)
								{
									position2 = new Rect(0f, vector2.y - 8f, base.leftmargin - 4f, 16f);
								}
								else
								{
									position2 = new Rect(0f, vector2.y - 13f, width, 16f);
								}
								GUI.Label(position2, num6.ToString(text) + this.settings.vTickStyle.unit, this.styles.labelTickMarksY);
							}
						}
					}
				}
				GUI.color = color;
				GUI.EndClip();
			}
		}
	}
}
