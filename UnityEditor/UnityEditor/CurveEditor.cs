using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace UnityEditor
{
	[Serializable]
	internal class CurveEditor : TimeArea, CurveUpdater
	{
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
			}
			public int curveId;
			public List<CurveEditor.SavedCurve.SavedKeyFrame> keys;
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
		public delegate void CallbackFunction();
		private const float kMaxPickDistSqr = 64f;
		private const float kExactPickDistSqr = 16f;
		private const float kCurveTimeEpsilon = 1E-05f;
		[NonSerialized]
		private CurveWrapper[] m_AnimationCurves;
		private static int s_SelectKeyHash = "SelectKeys".GetHashCode();
		public CurveEditor.CallbackFunction curvesUpdated;
		private List<int> m_DrawOrder = new List<int>();
		internal TimeUpdater m_TimeUpdater;
		internal Bounds m_DefaultBounds = new Bounds(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 1f, 0f));
		private Color m_TangentColor = new Color(1f, 1f, 1f, 0.5f);
		public float invSnap;
		private CurveMenuManager m_MenuManager;
		private static int s_TangentControlIDHash = "s_TangentControlIDHash".GetHashCode();
		private List<CurveSelection> m_Selection = new List<CurveSelection>();
		[NonSerialized]
		private List<CurveSelection> m_DisplayedSelection;
		private CurveSelection m_SelectedTangentPoint;
		private List<CurveSelection> s_SelectionBackup;
		private float s_TimeRangeSelectionStart;
		private float s_TimeRangeSelectionEnd;
		private bool s_TimeRangeSelectionActive;
		private Bounds m_Bounds = new Bounds(Vector3.zero, Vector3.zero);
		private List<CurveEditor.SavedCurve> m_CurveBackups;
		private CurveWrapper m_DraggingKey;
		private Vector2 m_DraggedCoord;
		private Vector2 m_MoveCoord;
		private Vector2 m_PreviousDrawPointCenter;
		internal CurveEditor.Styles ms_Styles;
		private Vector2 s_StartMouseDragPosition;
		private Vector2 s_EndMouseDragPosition;
		private Vector2 s_StartKeyDragPosition;
		private float s_StartClickedTime;
		private CurveEditor.PickMode s_PickMode;
		private string m_AxisLabelFormat = "n1";
		private CurveWrapper[] m_DraggingCurveOrRegion;
		public bool hasSelection
		{
			get
			{
				return this.m_Selection.Count != 0;
			}
		}
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
		public float activeTime
		{
			set
			{
				if (this.m_TimeUpdater != null)
				{
					this.m_TimeUpdater.UpdateTime(value);
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
		internal List<CurveSelection> selectedCurves
		{
			get
			{
				return this.m_Selection;
			}
		}
		public override Bounds drawingBounds
		{
			get
			{
				return this.m_Bounds;
			}
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
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}
		public bool GetTopMostCurveID(out int curveID)
		{
			if (this.m_DrawOrder.Count > 0)
			{
				curveID = this.m_DrawOrder[this.m_DrawOrder.Count - 1];
				return true;
			}
			curveID = -1;
			return false;
		}
		private void SyncDrawOrder()
		{
			if (this.m_DrawOrder.Count == 0)
			{
				this.m_DrawOrder = (
					from cw in this.m_AnimationCurves
					select cw.id).ToList<int>();
				return;
			}
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
			if (this.m_DrawOrder.Count == this.m_AnimationCurves.Length)
			{
				return;
			}
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
				this.m_DrawOrder = (
					from cw in this.m_AnimationCurves
					select cw.id).ToList<int>();
			}
		}
		public CurveWrapper getCurveWrapperById(int id)
		{
			CurveWrapper[] animationCurves = this.m_AnimationCurves;
			for (int i = 0; i < animationCurves.Length; i++)
			{
				CurveWrapper curveWrapper = animationCurves[i];
				if (curveWrapper.id == id)
				{
					return curveWrapper;
				}
			}
			return null;
		}
		protected override void ApplySettings()
		{
			base.ApplySettings();
			this.RecalculateBounds();
		}
		internal void ClearSelection()
		{
			this.m_Selection.Clear();
		}
		internal void ClearDisplayedSelection()
		{
			this.m_DisplayedSelection = null;
		}
		public void OnDisable()
		{
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}
		private void UndoRedoPerformed()
		{
			this.SelectNone();
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
						return;
					}
					CurveWrapper curveWrapper2 = this.m_AnimationCurves[++i];
					int regionId2 = curveWrapper2.regionId;
					if (regionId != regionId2)
					{
						Debug.LogError(string.Concat(new object[]
						{
							"Regions should be added as two curves after each other with same regionId: ",
							regionId,
							" != ",
							regionId2
						}));
						return;
					}
				}
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
				int id = this.m_DrawOrder[j];
				int regionId3 = this.getCurveWrapperById(id).regionId;
				if (regionId3 >= 0)
				{
					if (j == count - 1)
					{
						Debug.LogError("Region has only one curve last! Regions should be added as two curves after each other with same regionId");
						return;
					}
					int id2 = this.m_DrawOrder[++j];
					int regionId4 = this.getCurveWrapperById(id2).regionId;
					if (regionId3 != regionId4)
					{
						Debug.LogError(string.Concat(new object[]
						{
							"DrawOrder: Regions not added correctly after each other. RegionIds: ",
							regionId3,
							" , ",
							regionId4
						}));
						return;
					}
				}
			}
		}
		private void UpdateTangentsFromSelection()
		{
			foreach (CurveSelection current in this.m_Selection)
			{
				CurveUtility.UpdateTangentsFromModeSurrounding(current.curveWrapper.curve, current.key);
			}
		}
		private void SyncSelection()
		{
			this.Init();
			List<CurveSelection> list = new List<CurveSelection>(this.m_Selection.Count);
			foreach (CurveSelection current in this.m_Selection)
			{
				CurveWrapper curveWrapper = current.curveWrapper;
				if (curveWrapper != null && (!curveWrapper.hidden || curveWrapper.groupId != -1))
				{
					curveWrapper.selected = CurveWrapper.SelectionMode.Selected;
					list.Add(current);
				}
			}
			this.m_Selection = list;
			this.RecalculateBounds();
		}
		public void RecalculateBounds()
		{
			this.m_Bounds = this.m_DefaultBounds;
			if (this.animationCurves != null && (base.hRangeMin == float.NegativeInfinity || base.hRangeMax == float.PositiveInfinity || base.vRangeMin == float.NegativeInfinity || base.vRangeMax == float.PositiveInfinity))
			{
				bool flag = false;
				CurveWrapper[] animationCurves = this.animationCurves;
				for (int i = 0; i < animationCurves.Length; i++)
				{
					CurveWrapper curveWrapper = animationCurves[i];
					if (!curveWrapper.hidden)
					{
						if (curveWrapper.curve.length != 0)
						{
							if (!flag)
							{
								this.m_Bounds = curveWrapper.renderer.GetBounds();
								flag = true;
							}
							else
							{
								this.m_Bounds.Encapsulate(curveWrapper.renderer.GetBounds());
							}
						}
					}
				}
			}
			if (base.hRangeMin != float.NegativeInfinity)
			{
				this.m_Bounds.min = new Vector3(base.hRangeMin, this.m_Bounds.min.y, this.m_Bounds.min.z);
			}
			if (base.hRangeMax != float.PositiveInfinity)
			{
				this.m_Bounds.max = new Vector3(base.hRangeMax, this.m_Bounds.max.y, this.m_Bounds.max.z);
			}
			if (base.vRangeMin != float.NegativeInfinity)
			{
				this.m_Bounds.min = new Vector3(this.m_Bounds.min.x, base.vRangeMin, this.m_Bounds.min.z);
			}
			if (base.vRangeMax != float.PositiveInfinity)
			{
				this.m_Bounds.max = new Vector3(this.m_Bounds.max.y, base.vRangeMax, this.m_Bounds.max.z);
			}
			this.m_Bounds.size = new Vector3(Mathf.Max(this.m_Bounds.size.x, 0.1f), Mathf.Max(this.m_Bounds.size.y, 0.1f), 0f);
		}
		public void FrameSelected(bool horizontally, bool vertically)
		{
			Bounds bounds = default(Bounds);
			if (!this.hasSelection)
			{
				bounds = this.drawingBounds;
				if (bounds.size == Vector3.zero)
				{
					return;
				}
			}
			else
			{
				bounds = new Bounds(new Vector2(this.m_Selection[0].keyframe.time, this.m_Selection[0].keyframe.value), Vector2.zero);
				foreach (CurveSelection current in this.m_Selection)
				{
					bounds.Encapsulate(new Vector2(current.curve[current.key].time, current.curve[current.key].value));
					if (current.key - 1 >= 0)
					{
						bounds.Encapsulate(new Vector2(current.curve[current.key - 1].time, current.curve[current.key - 1].value));
					}
					if (current.key + 1 < current.curve.length)
					{
						bounds.Encapsulate(new Vector2(current.curve[current.key + 1].time, current.curve[current.key + 1].value));
					}
				}
				bounds.size = new Vector3(Mathf.Max(bounds.size.x, 0.1f), Mathf.Max(bounds.size.y, 0.1f), 0f);
			}
			if (horizontally)
			{
				base.SetShownHRangeInsideMargins(bounds.min.x, bounds.max.x);
			}
			if (vertically)
			{
				base.SetShownVRangeInsideMargins(bounds.min.y, bounds.max.y);
			}
		}
		public void UpdateCurves(List<int> curveIds, string undoText)
		{
			foreach (int current in curveIds)
			{
				CurveWrapper curveFromID = this.GetCurveFromID(current);
				curveFromID.changed = true;
			}
			if (this.curvesUpdated != null)
			{
				this.curvesUpdated();
			}
		}
		internal CurveWrapper GetCurveFromID(int curveID)
		{
			if (this.m_AnimationCurves == null)
			{
				return null;
			}
			CurveWrapper[] animationCurves = this.m_AnimationCurves;
			for (int i = 0; i < animationCurves.Length; i++)
			{
				CurveWrapper curveWrapper = animationCurves[i];
				if (curveWrapper.id == curveID)
				{
					return curveWrapper;
				}
			}
			return null;
		}
		private void Init()
		{
			if (this.m_Selection != null && this.hasSelection && this.m_Selection[0].m_Host == null)
			{
				foreach (CurveSelection current in this.m_Selection)
				{
					current.m_Host = this;
				}
			}
		}
		internal void InitStyles()
		{
			if (this.ms_Styles == null)
			{
				this.ms_Styles = new CurveEditor.Styles();
			}
		}
		public void OnGUI()
		{
			base.BeginViewGUI();
			this.GridGUI();
			this.CurveGUI();
			base.EndViewGUI();
		}
		public void CurveGUI()
		{
			this.InitStyles();
			GUI.BeginGroup(base.drawRect);
			this.Init();
			GUIUtility.GetControlID(CurveEditor.s_SelectKeyHash, FocusType.Passive);
			Color white = Color.white;
			GUI.backgroundColor = white;
			GUI.contentColor = white;
			Color color = GUI.color;
			Event current = Event.current;
			EventType type = current.type;
			switch (type)
			{
			case EventType.KeyDown:
				if ((current.keyCode == KeyCode.Backspace || current.keyCode == KeyCode.Delete) && this.hasSelection)
				{
					this.DeleteSelectedPoints();
					current.Use();
				}
				goto IL_30C;
			case EventType.KeyUp:
			case EventType.ScrollWheel:
				IL_65:
				switch (type)
				{
				case EventType.ValidateCommand:
				case EventType.ExecuteCommand:
				{
					bool flag = current.type == EventType.ExecuteCommand;
					string commandName = current.commandName;
					switch (commandName)
					{
					case "Delete":
						if (this.hasSelection)
						{
							if (flag)
							{
								this.DeleteSelectedPoints();
							}
							current.Use();
						}
						break;
					case "FrameSelected":
						if (flag)
						{
							this.FrameSelected(true, true);
						}
						current.Use();
						break;
					case "SelectAll":
						if (flag)
						{
							this.SelectAll();
						}
						current.Use();
						break;
					}
					goto IL_30C;
				}
				case EventType.DragExited:
					goto IL_30C;
				case EventType.ContextClick:
				{
					CurveSelection curveSelection = this.FindNearest();
					if (curveSelection != null)
					{
						List<KeyIdentifier> list = new List<KeyIdentifier>();
						bool flag2 = false;
						foreach (CurveSelection current2 in this.m_Selection)
						{
							list.Add(new KeyIdentifier(current2.curveWrapper.renderer, current2.curveID, current2.key));
							if (current2.curveID == curveSelection.curveID && current2.key == curveSelection.key)
							{
								flag2 = true;
							}
						}
						if (!flag2)
						{
							list.Clear();
							list.Add(new KeyIdentifier(curveSelection.curveWrapper.renderer, curveSelection.curveID, curveSelection.key));
							this.m_Selection.Clear();
							this.m_Selection.Add(curveSelection);
						}
						this.m_MenuManager = new CurveMenuManager(this);
						GenericMenu genericMenu = new GenericMenu();
						string text;
						if (list.Count > 1)
						{
							text = "Delete Keys";
						}
						else
						{
							text = "Delete Key";
						}
						genericMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.DeleteKeys), list);
						genericMenu.AddSeparator(string.Empty);
						this.m_MenuManager.AddTangentMenuItems(genericMenu, list);
						genericMenu.ShowAsContext();
						Event.current.Use();
					}
					goto IL_30C;
				}
				default:
					goto IL_30C;
				}
				break;
			case EventType.Repaint:
				this.DrawCurves(this.animationCurves);
				goto IL_30C;
			}
			goto IL_65;
			IL_30C:
			bool changed = GUI.changed;
			GUI.changed = false;
			GUI.color = color;
			this.DragTangents();
			this.EditAxisLabels();
			this.SelectPoints();
			if (GUI.changed)
			{
				this.RecalcSecondarySelection();
				this.RecalcCurveSelection();
			}
			GUI.changed = false;
			Vector2 moveCoord = this.MovePoints();
			if (GUI.changed && this.m_DraggingKey != null)
			{
				this.activeTime = moveCoord.x + this.s_StartClickedTime;
				this.m_MoveCoord = moveCoord;
			}
			GUI.changed = changed;
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
			foreach (CurveSelection current in this.m_Selection)
			{
				current.curveWrapper.selected = ((!current.semiSelected) ? CurveWrapper.SelectionMode.Selected : CurveWrapper.SelectionMode.SemiSelected);
			}
		}
		private void RecalcSecondarySelection()
		{
			List<CurveSelection> list = new List<CurveSelection>();
			foreach (CurveSelection current in this.m_Selection)
			{
				CurveWrapper curveWrapper = current.curveWrapper;
				int groupId = current.curveWrapper.groupId;
				if (groupId != -1 && !current.semiSelected)
				{
					list.Add(current);
					CurveWrapper[] animationCurves = this.m_AnimationCurves;
					for (int i = 0; i < animationCurves.Length; i++)
					{
						CurveWrapper curveWrapper2 = animationCurves[i];
						if (curveWrapper2.groupId == groupId && curveWrapper2 != curveWrapper)
						{
							list.Add(new CurveSelection(curveWrapper2.id, this, current.key)
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
			this.m_Selection = list;
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
					float num = 64f;
					Vector2 mousePosition = Event.current.mousePosition;
					foreach (CurveSelection current2 in this.m_Selection)
					{
						Keyframe keyframe = current2.keyframe;
						if (CurveUtility.GetKeyTangentMode(keyframe, 0) == TangentMode.Editable)
						{
							CurveSelection curveSelection = new CurveSelection(current2.curveID, this, current2.key, CurveSelection.SelectionType.InTangent);
							float sqrMagnitude = (base.DrawingToViewTransformPoint(this.GetPosition(curveSelection)) - mousePosition).sqrMagnitude;
							if (sqrMagnitude <= num)
							{
								this.m_SelectedTangentPoint = curveSelection;
								num = sqrMagnitude;
							}
						}
						if (CurveUtility.GetKeyTangentMode(keyframe, 1) == TangentMode.Editable)
						{
							CurveSelection curveSelection2 = new CurveSelection(current2.curveID, this, current2.key, CurveSelection.SelectionType.OutTangent);
							float sqrMagnitude2 = (base.DrawingToViewTransformPoint(this.GetPosition(curveSelection2)) - mousePosition).sqrMagnitude;
							if (sqrMagnitude2 <= num)
							{
								this.m_SelectedTangentPoint = curveSelection2;
								num = sqrMagnitude2;
							}
						}
					}
					if (this.m_SelectedTangentPoint != null)
					{
						GUIUtility.hotControl = controlID;
						this.activeTime = this.m_SelectedTangentPoint.keyframe.time;
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
					Vector2 mousePositionInDrawing = base.mousePositionInDrawing;
					CurveSelection selectedTangentPoint = this.m_SelectedTangentPoint;
					Keyframe keyframe2 = selectedTangentPoint.keyframe;
					if (selectedTangentPoint.type == CurveSelection.SelectionType.InTangent)
					{
						Vector2 vector = mousePositionInDrawing - new Vector2(keyframe2.time, keyframe2.value);
						if (vector.x < -0.0001f)
						{
							keyframe2.inTangent = vector.y / vector.x;
						}
						else
						{
							keyframe2.inTangent = float.PositiveInfinity;
						}
						CurveUtility.SetKeyTangentMode(ref keyframe2, 0, TangentMode.Editable);
						if (!CurveUtility.GetKeyBroken(keyframe2))
						{
							keyframe2.outTangent = keyframe2.inTangent;
							CurveUtility.SetKeyTangentMode(ref keyframe2, 1, TangentMode.Editable);
						}
					}
					else
					{
						if (selectedTangentPoint.type == CurveSelection.SelectionType.OutTangent)
						{
							Vector2 vector2 = mousePositionInDrawing - new Vector2(keyframe2.time, keyframe2.value);
							if (vector2.x > 0.0001f)
							{
								keyframe2.outTangent = vector2.y / vector2.x;
							}
							else
							{
								keyframe2.outTangent = float.PositiveInfinity;
							}
							CurveUtility.SetKeyTangentMode(ref keyframe2, 1, TangentMode.Editable);
							if (!CurveUtility.GetKeyBroken(keyframe2))
							{
								keyframe2.inTangent = keyframe2.outTangent;
								CurveUtility.SetKeyTangentMode(ref keyframe2, 0, TangentMode.Editable);
							}
						}
					}
					selectedTangentPoint.key = selectedTangentPoint.curve.MoveKey(selectedTangentPoint.key, keyframe2);
					CurveUtility.UpdateTangentsFromModeSurrounding(selectedTangentPoint.curveWrapper.curve, selectedTangentPoint.key);
					selectedTangentPoint.curveWrapper.changed = true;
					Event.current.Use();
				}
				break;
			}
		}
		private void DeleteSelectedPoints()
		{
			for (int i = this.m_Selection.Count - 1; i >= 0; i--)
			{
				CurveSelection curveSelection = this.m_Selection[i];
				CurveWrapper curveWrapper = curveSelection.curveWrapper;
				if (base.settings.allowDeleteLastKeyInCurve || curveWrapper.curve.keys.Length != 1)
				{
					curveWrapper.curve.RemoveKey(curveSelection.key);
					CurveUtility.UpdateTangentsFromMode(curveWrapper.curve);
					curveWrapper.changed = true;
				}
			}
			this.SelectNone();
		}
		private void DeleteKeys(object obj)
		{
			List<KeyIdentifier> list = (List<KeyIdentifier>)obj;
			List<int> list2 = new List<int>();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (base.settings.allowDeleteLastKeyInCurve || list[i].curve.keys.Length != 1)
				{
					list[i].curve.RemoveKey(list[i].key);
					CurveUtility.UpdateTangentsFromMode(list[i].curve);
					list2.Add(list[i].curveId);
				}
			}
			string undoText;
			if (list.Count > 1)
			{
				undoText = "Delete Keys";
			}
			else
			{
				undoText = "Delete Key";
			}
			this.UpdateCurves(list2, undoText);
			this.SelectNone();
		}
		private float ClampVerticalValue(float value, int curveID)
		{
			value = Mathf.Clamp(value, base.vRangeMin, base.vRangeMax);
			CurveWrapper curveFromID = this.GetCurveFromID(curveID);
			if (curveFromID != null)
			{
				value = Mathf.Clamp(value, curveFromID.vRangeMin, curveFromID.vRangeMax);
			}
			return value;
		}
		public void UpdateCurvesFromPoints(Vector2 movement)
		{
			this.m_DisplayedSelection = new List<CurveSelection>();
			foreach (CurveEditor.SavedCurve current in this.m_CurveBackups)
			{
				List<CurveEditor.SavedCurve.SavedKeyFrame> list = new List<CurveEditor.SavedCurve.SavedKeyFrame>(current.keys.Count);
				int num;
				int num2;
				int num3;
				if (movement.x <= 0f)
				{
					num = 0;
					num2 = current.keys.Count;
					num3 = 1;
				}
				else
				{
					num = current.keys.Count - 1;
					num2 = -1;
					num3 = -1;
				}
				for (int num4 = num; num4 != num2; num4 += num3)
				{
					CurveEditor.SavedCurve.SavedKeyFrame savedKeyFrame = current.keys[num4];
					if (savedKeyFrame.selected != CurveWrapper.SelectionMode.None)
					{
						savedKeyFrame = new CurveEditor.SavedCurve.SavedKeyFrame(savedKeyFrame.key, savedKeyFrame.selected);
						savedKeyFrame.key.time = Mathf.Clamp(savedKeyFrame.key.time + movement.x, base.hRangeMin, base.hRangeMax);
						if (savedKeyFrame.selected == CurveWrapper.SelectionMode.Selected)
						{
							savedKeyFrame.key.value = this.ClampVerticalValue(savedKeyFrame.key.value + movement.y, current.curveId);
						}
						for (int i = list.Count - 1; i >= 0; i--)
						{
							if (Mathf.Abs(list[i].key.time - savedKeyFrame.key.time) < 1E-05f)
							{
								list.RemoveAt(i);
							}
						}
					}
					list.Add(new CurveEditor.SavedCurve.SavedKeyFrame(savedKeyFrame.key, savedKeyFrame.selected));
				}
				list.Sort();
				Keyframe[] array = new Keyframe[list.Count];
				for (int j = 0; j < list.Count; j++)
				{
					CurveEditor.SavedCurve.SavedKeyFrame savedKeyFrame2 = list[j];
					array[j] = savedKeyFrame2.key;
					if (savedKeyFrame2.selected != CurveWrapper.SelectionMode.None)
					{
						CurveSelection curveSelection = new CurveSelection(current.curveId, this, j);
						if (savedKeyFrame2.selected == CurveWrapper.SelectionMode.SemiSelected)
						{
							curveSelection.semiSelected = true;
						}
						this.m_DisplayedSelection.Add(curveSelection);
					}
				}
				CurveWrapper curveFromID = this.GetCurveFromID(current.curveId);
				curveFromID.curve.keys = array;
				curveFromID.changed = true;
			}
			this.UpdateTangentsFromSelection();
		}
		private float SnapTime(float t)
		{
			if (EditorGUI.actionKey)
			{
				int levelWithMinSeparation = base.hTicks.GetLevelWithMinSeparation(5f);
				float periodOfLevel = base.hTicks.GetPeriodOfLevel(levelWithMinSeparation);
				t = Mathf.Round(t / periodOfLevel) * periodOfLevel;
			}
			else
			{
				if (this.invSnap != 0f)
				{
					t = Mathf.Round(t * this.invSnap) / this.invSnap;
				}
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
		private Vector2 GetGUIPoint(Vector3 point)
		{
			return HandleUtility.WorldToGUIPoint(base.DrawingToViewTransformPoint(point));
		}
		private int OnlyOneEditableCurve()
		{
			int result = -1;
			int num = 0;
			for (int i = 0; i < this.m_AnimationCurves.Length; i++)
			{
				CurveWrapper curveWrapper = this.m_AnimationCurves[i];
				if (!curveWrapper.hidden && !curveWrapper.readOnly)
				{
					num++;
					result = i;
				}
			}
			if (num == 1)
			{
				return result;
			}
			return -1;
		}
		private int GetCurveAtPosition(Vector2 position, out Vector2 closestPointOnCurve)
		{
			Vector2 v = base.DrawingToViewTransformPoint(position);
			int num = (int)Mathf.Sqrt(64f);
			float num2 = 64f;
			int result = -1;
			closestPointOnCurve = Vector3.zero;
			for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
			{
				CurveWrapper curveWrapperById = this.getCurveWrapperById(this.m_DrawOrder[i]);
				if (!curveWrapperById.hidden && !curveWrapperById.readOnly)
				{
					Vector2 vector;
					vector.x = position.x - (float)num / base.scale.x;
					vector.y = curveWrapperById.renderer.EvaluateCurveSlow(vector.x);
					vector = base.DrawingToViewTransformPoint(vector);
					for (int j = -num; j < num; j++)
					{
						Vector2 vector2;
						vector2.x = position.x + (float)(j + 1) / base.scale.x;
						vector2.y = curveWrapperById.renderer.EvaluateCurveSlow(vector2.x);
						vector2 = base.DrawingToViewTransformPoint(vector2);
						float num3 = HandleUtility.DistancePointLine(v, vector, vector2);
						num3 *= num3;
						if (num3 < num2)
						{
							num2 = num3;
							result = curveWrapperById.listIndex;
							closestPointOnCurve = HandleUtility.ProjectPointLine(v, vector, vector2);
						}
						vector = vector2;
					}
				}
			}
			closestPointOnCurve = base.ViewToDrawingTransformPoint(closestPointOnCurve);
			return result;
		}
		private void CreateKeyFromClick(object obj)
		{
			List<int> curveIds = this.CreateKeyFromClick((Vector2)obj);
			this.UpdateCurves(curveIds, "Add Key");
		}
		private List<int> CreateKeyFromClick(Vector2 position)
		{
			List<int> list = new List<int>();
			int num = this.OnlyOneEditableCurve();
			if (num >= 0)
			{
				float x = position.x;
				CurveWrapper curveWrapper = this.m_AnimationCurves[num];
				if (curveWrapper.curve.keys.Length == 0 || x < curveWrapper.curve.keys[0].time || x > curveWrapper.curve.keys[curveWrapper.curve.keys.Length - 1].time)
				{
					this.CreateKeyFromClick(num, position);
					list.Add(curveWrapper.id);
					return list;
				}
			}
			Vector2 vector;
			int curveAtPosition = this.GetCurveAtPosition(position, out vector);
			this.CreateKeyFromClick(curveAtPosition, vector.x);
			if (curveAtPosition >= 0)
			{
				list.Add(this.m_AnimationCurves[curveAtPosition].id);
			}
			return list;
		}
		private void CreateKeyFromClick(int curveIndex, float time)
		{
			time = Mathf.Clamp(time, base.settings.hRangeMin, base.settings.hRangeMax);
			if (curveIndex >= 0)
			{
				CurveSelection curveSelection = null;
				CurveWrapper curveWrapper = this.m_AnimationCurves[curveIndex];
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
					this.m_Selection = new List<CurveSelection>(1);
					this.m_Selection.Add(curveSelection);
					this.RecalcSecondarySelection();
				}
				else
				{
					this.SelectNone();
				}
			}
		}
		private void CreateKeyFromClick(int curveIndex, Vector2 position)
		{
			position.x = Mathf.Clamp(position.x, base.settings.hRangeMin, base.settings.hRangeMax);
			if (curveIndex >= 0)
			{
				CurveSelection curveSelection = null;
				CurveWrapper curveWrapper = this.m_AnimationCurves[curveIndex];
				if (curveWrapper.groupId == -1)
				{
					curveSelection = this.AddKeyAtPosition(curveWrapper, position);
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
								curveSelection = this.AddKeyAtPosition(curveWrapper2, position);
							}
							else
							{
								this.AddKeyAtTime(curveWrapper2, position.x);
							}
						}
					}
				}
				if (curveSelection != null)
				{
					this.m_Selection = new List<CurveSelection>(1);
					this.m_Selection.Add(curveSelection);
					this.RecalcSecondarySelection();
				}
				else
				{
					this.SelectNone();
				}
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
			if (CurveUtility.HaveKeysInRange(cw.curve, time - num, time + num))
			{
				return null;
			}
			float num2 = cw.renderer.EvaluateCurveDeltaSlow(time);
			float value = this.ClampVerticalValue(this.SnapValue(cw.renderer.EvaluateCurveSlow(time)), cw.id);
			Keyframe key = new Keyframe(time, value, num2, num2);
			return this.AddKeyframeAndSelect(key, cw);
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
			if (CurveUtility.HaveKeysInRange(cw.curve, position.x - num, position.x + num))
			{
				return null;
			}
			float num2 = 0f;
			Keyframe key = new Keyframe(position.x, this.SnapValue(position.y), num2, num2);
			return this.AddKeyframeAndSelect(key, cw);
		}
		private CurveSelection AddKeyframeAndSelect(Keyframe key, CurveWrapper cw)
		{
			int num = cw.curve.AddKey(key);
			CurveUtility.SetKeyModeFromContext(cw.curve, num);
			CurveUtility.UpdateTangentsFromModeSurrounding(cw.curve, num);
			CurveSelection result = new CurveSelection(cw.id, this, num);
			cw.selected = CurveWrapper.SelectionMode.Selected;
			cw.changed = true;
			this.activeTime = key.time;
			return result;
		}
		private CurveSelection FindNearest()
		{
			Vector2 mousePosition = Event.current.mousePosition;
			int num = -1;
			int keyIndex = -1;
			float num2 = 64f;
			for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
			{
				CurveWrapper curveWrapperById = this.getCurveWrapperById(this.m_DrawOrder[i]);
				if (!curveWrapperById.readOnly && !curveWrapperById.hidden)
				{
					for (int j = 0; j < curveWrapperById.curve.keys.Length; j++)
					{
						Keyframe keyframe = curveWrapperById.curve.keys[j];
						float sqrMagnitude = (this.GetGUIPoint(new Vector2(keyframe.time, keyframe.value)) - mousePosition).sqrMagnitude;
						if (sqrMagnitude <= 16f)
						{
							return new CurveSelection(curveWrapperById.id, this, j);
						}
						if (sqrMagnitude < num2)
						{
							num = curveWrapperById.id;
							keyIndex = j;
							num2 = sqrMagnitude;
						}
					}
					if (i == this.m_DrawOrder.Count - 1 && num >= 0)
					{
						num2 = 16f;
					}
				}
			}
			if (num >= 0)
			{
				return new CurveSelection(num, this, keyIndex);
			}
			return null;
		}
		public void SelectNone()
		{
			this.m_Selection = new List<CurveSelection>();
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
			this.m_Selection = new List<CurveSelection>(num);
			CurveWrapper[] animationCurves2 = this.m_AnimationCurves;
			for (int j = 0; j < animationCurves2.Length; j++)
			{
				CurveWrapper curveWrapper2 = animationCurves2[j];
				curveWrapper2.selected = CurveWrapper.SelectionMode.Selected;
				for (int k = 0; k < curveWrapper2.curve.length; k++)
				{
					this.m_Selection.Add(new CurveSelection(curveWrapper2.id, this, k));
				}
			}
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
			int curveAtPosition = this.GetCurveAtPosition(base.mousePositionInDrawing, out vector);
			if (curveAtPosition >= 0)
			{
				this.MoveCurveToFront(this.m_AnimationCurves[curveAtPosition].id);
				timeValue = base.mousePositionInDrawing;
				curves = new CurveWrapper[]
				{
					this.m_AnimationCurves[curveAtPosition]
				};
				return true;
			}
			for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
			{
				CurveWrapper curveWrapperById = this.getCurveWrapperById(this.m_DrawOrder[i]);
				if (curveWrapperById != null)
				{
					if (!curveWrapperById.hidden)
					{
						if (curveWrapperById.curve.length != 0)
						{
							CurveWrapper curveWrapper = null;
							if (i > 0)
							{
								curveWrapper = this.getCurveWrapperById(this.m_DrawOrder[i - 1]);
							}
							if (this.IsRegion(curveWrapperById, curveWrapper))
							{
								float y = base.mousePositionInDrawing.y;
								float x = base.mousePositionInDrawing.x;
								float num = curveWrapperById.renderer.EvaluateCurveSlow(x);
								float num2 = curveWrapper.renderer.EvaluateCurveSlow(x);
								if (num > num2)
								{
									float num3 = num;
									num = num2;
									num2 = num3;
								}
								if (y >= num && y <= num2)
								{
									timeValue = base.mousePositionInDrawing;
									curves = new CurveWrapper[]
									{
										curveWrapperById,
										curveWrapper
									};
									this.MoveCurveToFront(curveWrapperById.id);
									return true;
								}
								i--;
							}
						}
					}
				}
			}
			return false;
		}
		private void SelectPoints()
		{
			int controlID = GUIUtility.GetControlID(897560, FocusType.Passive);
			Event current = Event.current;
			bool shift = current.shift;
			EventType typeForControl = current.GetTypeForControl(controlID);
			switch (typeForControl)
			{
			case EventType.MouseDown:
				if (current.clickCount == 2 && current.button == 0)
				{
					this.CreateKeyFromClick(base.mousePositionInDrawing);
				}
				else
				{
					if (current.button == 0)
					{
						CurveSelection curveSelection = this.FindNearest();
						if (curveSelection == null || curveSelection.semiSelected)
						{
							if (!shift)
							{
								this.SelectNone();
							}
							Vector2 zero = Vector2.zero;
							CurveWrapper[] array = null;
							this.HandleCurveAndRegionMoveToFrontOnMouseDown(ref zero, ref array);
							GUIUtility.hotControl = controlID;
							this.s_EndMouseDragPosition = (this.s_StartMouseDragPosition = current.mousePosition);
							this.s_PickMode = CurveEditor.PickMode.Click;
						}
						else
						{
							this.MoveCurveToFront(curveSelection.curveID);
							this.activeTime = curveSelection.keyframe.time;
							this.s_StartKeyDragPosition = new Vector2(curveSelection.keyframe.time, curveSelection.keyframe.value);
							if (shift)
							{
								List<CurveSelection> list = new List<CurveSelection>(this.m_Selection);
								if (this.m_Selection.IndexOf(curveSelection) == -1)
								{
									list.Add(curveSelection);
									list.Sort();
								}
								this.m_Selection = list;
							}
							else
							{
								if (this.m_Selection.IndexOf(curveSelection) == -1)
								{
									this.m_Selection = new List<CurveSelection>(1);
									this.m_Selection.Add(curveSelection);
								}
							}
						}
						GUI.changed = true;
						HandleUtility.Repaint();
					}
				}
				goto IL_3E3;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					this.s_PickMode = CurveEditor.PickMode.None;
					Event.current.Use();
				}
				goto IL_3E3;
			case EventType.MouseMove:
			{
				IL_39:
				if (typeForControl == EventType.Layout)
				{
					HandleUtility.AddDefaultControl(controlID);
					goto IL_3E3;
				}
				if (typeForControl != EventType.ContextClick)
				{
					goto IL_3E3;
				}
				Rect drawRect = base.drawRect;
				float num = 0f;
				drawRect.y = num;
				drawRect.x = num;
				if (drawRect.Contains(Event.current.mousePosition))
				{
					Vector2 vector;
					int curveAtPosition = this.GetCurveAtPosition(base.mousePositionInDrawing, out vector);
					if (curveAtPosition >= 0)
					{
						GenericMenu genericMenu = new GenericMenu();
						genericMenu.AddItem(new GUIContent("Add Key"), false, new GenericMenu.MenuFunction2(this.CreateKeyFromClick), base.mousePositionInDrawing);
						genericMenu.ShowAsContext();
						Event.current.Use();
					}
				}
				goto IL_3E3;
			}
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					this.s_EndMouseDragPosition = current.mousePosition;
					if (this.s_PickMode == CurveEditor.PickMode.Click)
					{
						this.s_PickMode = CurveEditor.PickMode.Marquee;
						if (shift)
						{
							this.s_SelectionBackup = new List<CurveSelection>(this.m_Selection);
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
						for (int i = 0; i < animationCurves.Length; i++)
						{
							CurveWrapper curveWrapper = animationCurves[i];
							if (!curveWrapper.readOnly && !curveWrapper.hidden)
							{
								int num2 = 0;
								Keyframe[] keys = curveWrapper.curve.keys;
								for (int j = 0; j < keys.Length; j++)
								{
									Keyframe keyframe = keys[j];
									if (rect.Contains(this.GetGUIPoint(new Vector2(keyframe.time, keyframe.value))))
									{
										list2.Add(new CurveSelection(curveWrapper.id, this, num2));
										this.MoveCurveToFront(curveWrapper.id);
									}
									num2++;
								}
							}
						}
						list2.Sort();
						this.m_Selection = list2;
						GUI.changed = true;
					}
					current.Use();
				}
				goto IL_3E3;
			}
			goto IL_39;
			IL_3E3:
			if (this.s_PickMode == CurveEditor.PickMode.Marquee)
			{
				GUI.Label(EditorGUIExt.FromToRect(this.s_StartMouseDragPosition, this.s_EndMouseDragPosition), GUIContent.none, this.ms_Styles.selectionRect);
			}
		}
		private void EditAxisLabels()
		{
			int controlID = GUIUtility.GetControlID(18975602, FocusType.Keyboard);
			List<CurveWrapper> list = new List<CurveWrapper>();
			Vector2 axisUiScalars = this.GetAxisUiScalars(list);
			if (axisUiScalars.y < 0f || list.Count <= 0 || list[0].setAxisUiScalarsCallback == null)
			{
				return;
			}
			Rect rect = new Rect(0f, base.topmargin - 8f, base.leftmargin - 4f, 16f);
			Rect position = rect;
			position.y -= rect.height;
			Event current = Event.current;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (current.button == 0)
				{
					if (position.Contains(Event.current.mousePosition) && GUIUtility.hotControl == 0)
					{
						GUIUtility.keyboardControl = 0;
						GUIUtility.hotControl = controlID;
						GUI.changed = true;
						current.Use();
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
			float num2 = EditorGUI.FloatField(rect, axisUiScalars.y, this.ms_Styles.axisLabelNumberField);
			if (axisUiScalars.y != num2)
			{
				this.SetAxisUiScalars(new Vector2(axisUiScalars.x, num2), list);
			}
			EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
		}
		public void BeginTimeRangeSelection(float time, bool addToSelection)
		{
			if (this.s_TimeRangeSelectionActive)
			{
				Debug.LogError("BeginTimeRangeSelection can only be called once");
				return;
			}
			this.s_TimeRangeSelectionActive = true;
			this.s_TimeRangeSelectionEnd = time;
			this.s_TimeRangeSelectionStart = time;
			if (addToSelection)
			{
				this.s_SelectionBackup = new List<CurveSelection>(this.m_Selection);
			}
			else
			{
				this.s_SelectionBackup = new List<CurveSelection>();
			}
		}
		public void TimeRangeSelectTo(float time)
		{
			if (!this.s_TimeRangeSelectionActive)
			{
				Debug.LogError("TimeRangeSelectTo can only be called after BeginTimeRangeSelection");
				return;
			}
			this.s_TimeRangeSelectionEnd = time;
			this.m_Selection = new List<CurveSelection>(this.s_SelectionBackup);
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
							this.m_Selection.Add(new CurveSelection(curveWrapper.id, this, num3));
						}
						num3++;
					}
				}
			}
			this.m_Selection.Sort();
			this.RecalcSecondarySelection();
			this.RecalcCurveSelection();
		}
		public void EndTimeRangeSelection()
		{
			if (!this.s_TimeRangeSelectionActive)
			{
				Debug.LogError("EndTimeRangeSelection can only be called after BeginTimeRangeSelection");
				return;
			}
			this.s_TimeRangeSelectionStart = (this.s_TimeRangeSelectionEnd = 0f);
			this.s_TimeRangeSelectionActive = false;
		}
		public void CancelTimeRangeSelection()
		{
			if (!this.s_TimeRangeSelectionActive)
			{
				Debug.LogError("CancelTimeRangeSelection can only be called after BeginTimeRangeSelection");
				return;
			}
			this.m_Selection = this.s_SelectionBackup;
			this.s_TimeRangeSelectionActive = false;
		}
		public void DrawTimeRange()
		{
			if (this.s_TimeRangeSelectionActive && Event.current.type == EventType.Repaint)
			{
				float x = Mathf.Min(this.s_TimeRangeSelectionStart, this.s_TimeRangeSelectionEnd);
				float x2 = Mathf.Max(this.s_TimeRangeSelectionStart, this.s_TimeRangeSelectionEnd);
				float x3 = this.GetGUIPoint(new Vector3(x, 0f, 0f)).x;
				float x4 = this.GetGUIPoint(new Vector3(x2, 0f, 0f)).x;
				GUI.Label(new Rect(x3, -10000f, x4 - x3, 20000f), GUIContent.none, this.ms_Styles.selectionRect);
			}
		}
		private void SetupKeyOrCurveDragging(Vector2 timeValue, CurveWrapper cw, int id, Vector2 mousePos)
		{
			this.m_DraggedCoord = timeValue;
			this.m_DraggingKey = cw;
			GUIUtility.hotControl = id;
			this.MakeCurveBackups();
			this.s_StartMouseDragPosition = mousePos;
			this.activeTime = timeValue.x;
			this.s_StartClickedTime = timeValue.x;
		}
		public Vector2 MovePoints()
		{
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			if (!this.hasSelection && !base.settings.allowDraggingCurvesAndRegions)
			{
				return Vector2.zero;
			}
			Event current = Event.current;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (current.button == 0)
				{
					foreach (CurveSelection current2 in this.m_Selection)
					{
						if (!current2.curveWrapper.hidden)
						{
							if ((base.DrawingToViewTransformPoint(this.GetPosition(current2)) - current.mousePosition).sqrMagnitude <= 64f)
							{
								this.SetupKeyOrCurveDragging(new Vector2(current2.keyframe.time, current2.keyframe.value), current2.curveWrapper, controlID, current.mousePosition);
								break;
							}
						}
					}
					if (base.settings.allowDraggingCurvesAndRegions && this.m_DraggingKey == null)
					{
						Vector2 zero = Vector2.zero;
						CurveWrapper[] array = null;
						if (this.HandleCurveAndRegionMoveToFrontOnMouseDown(ref zero, ref array))
						{
							CurveWrapper[] array2 = array;
							for (int i = 0; i < array2.Length; i++)
							{
								CurveWrapper curveWrapper = array2[i];
								for (int j = 0; j < curveWrapper.curve.keys.Length; j++)
								{
									this.m_Selection.Add(new CurveSelection(curveWrapper.id, this, j));
								}
								this.MoveCurveToFront(curveWrapper.id);
							}
							this.SetupKeyOrCurveDragging(zero, array[0], controlID, current.mousePosition);
							this.m_DraggingCurveOrRegion = array;
						}
					}
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					this.ResetDragging();
					GUI.changed = true;
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					Vector2 lhs = current.mousePosition - this.s_StartMouseDragPosition;
					Vector3 v;
					if (this.m_DraggingCurveOrRegion != null)
					{
						lhs.x = 0f;
						v = base.ViewToDrawingTransformVector(lhs);
						v.y = this.SnapValue(v.y + this.s_StartKeyDragPosition.y) - this.s_StartKeyDragPosition.y;
					}
					else
					{
						if (current.shift)
						{
							if (Mathf.Abs(lhs.x) > Mathf.Abs(lhs.y))
							{
								lhs.y = 0f;
								v = base.ViewToDrawingTransformVector(lhs);
								v.x = this.SnapTime(v.x + this.s_StartKeyDragPosition.x) - this.s_StartKeyDragPosition.x;
							}
							else
							{
								lhs.x = 0f;
								v = base.ViewToDrawingTransformVector(lhs);
								v.y = this.SnapValue(v.y + this.s_StartKeyDragPosition.y) - this.s_StartKeyDragPosition.y;
							}
						}
						else
						{
							v = base.ViewToDrawingTransformVector(lhs);
							v.x = this.SnapTime(v.x + this.s_StartKeyDragPosition.x) - this.s_StartKeyDragPosition.x;
							v.y = this.SnapValue(v.y + this.s_StartKeyDragPosition.y) - this.s_StartKeyDragPosition.y;
						}
					}
					this.UpdateCurvesFromPoints(v);
					GUI.changed = true;
					current.Use();
					return v;
				}
				break;
			case EventType.KeyDown:
				if (GUIUtility.hotControl == controlID && current.keyCode == KeyCode.Escape)
				{
					this.UpdateCurvesFromPoints(Vector2.zero);
					this.ResetDragging();
					GUI.changed = true;
					current.Use();
				}
				break;
			case EventType.Repaint:
				if (this.m_DraggingCurveOrRegion != null)
				{
					EditorGUIUtility.AddCursorRect(new Rect(current.mousePosition.x - 10f, current.mousePosition.y - 10f, 20f, 20f), MouseCursor.ResizeVertical);
				}
				break;
			}
			return Vector2.zero;
		}
		private void ResetDragging()
		{
			if (this.m_DraggingCurveOrRegion != null)
			{
				this.SelectNone();
			}
			GUIUtility.hotControl = 0;
			this.m_DraggingKey = null;
			this.m_DraggingCurveOrRegion = null;
			this.m_DisplayedSelection = null;
			this.m_MoveCoord = Vector2.zero;
		}
		internal void MakeCurveBackups()
		{
			this.m_CurveBackups = new List<CurveEditor.SavedCurve>();
			int num = -1;
			CurveEditor.SavedCurve savedCurve = null;
			for (int i = 0; i < this.m_Selection.Count; i++)
			{
				CurveSelection curveSelection = this.m_Selection[i];
				if (curveSelection.curveID != num)
				{
					savedCurve = new CurveEditor.SavedCurve();
					num = (savedCurve.curveId = curveSelection.curveID);
					Keyframe[] keys = curveSelection.curve.keys;
					savedCurve.keys = new List<CurveEditor.SavedCurve.SavedKeyFrame>(keys.Length);
					Keyframe[] array = keys;
					for (int j = 0; j < array.Length; j++)
					{
						Keyframe key = array[j];
						savedCurve.keys.Add(new CurveEditor.SavedCurve.SavedKeyFrame(key, CurveWrapper.SelectionMode.None));
					}
					this.m_CurveBackups.Add(savedCurve);
				}
				savedCurve.keys[curveSelection.key].selected = ((!curveSelection.semiSelected) ? CurveWrapper.SelectionMode.Selected : CurveWrapper.SelectionMode.SemiSelected);
			}
		}
		private Vector2 GetPosition(CurveSelection selection)
		{
			Keyframe keyframe = selection.keyframe;
			Vector2 vector = new Vector2(keyframe.time, keyframe.value);
			float d = 50f;
			if (selection.type == CurveSelection.SelectionType.InTangent)
			{
				Vector2 vector2 = new Vector2(1f, keyframe.inTangent);
				if (keyframe.inTangent == float.PositiveInfinity)
				{
					vector2 = new Vector2(0f, -1f);
				}
				vector2 = base.NormalizeInViewSpace(vector2);
				return vector - vector2 * d;
			}
			if (selection.type == CurveSelection.SelectionType.OutTangent)
			{
				Vector2 vector3 = new Vector2(1f, keyframe.outTangent);
				if (keyframe.outTangent == float.PositiveInfinity)
				{
					vector3 = new Vector2(0f, -1f);
				}
				vector3 = base.NormalizeInViewSpace(vector3);
				return vector + vector3 * d;
			}
			return vector;
		}
		private void MoveCurveToFront(int curveID)
		{
			int count = this.m_DrawOrder.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.m_DrawOrder[i] == curveID)
				{
					int regionId = this.getCurveWrapperById(curveID).regionId;
					if (regionId >= 0)
					{
						int num = 0;
						int num2 = -1;
						if (i - 1 >= 0)
						{
							int num3 = this.m_DrawOrder[i - 1];
							if (regionId == this.getCurveWrapperById(num3).regionId)
							{
								num2 = num3;
								num = -1;
							}
						}
						if (i + 1 < count && num2 < 0)
						{
							int num4 = this.m_DrawOrder[i + 1];
							if (regionId == this.getCurveWrapperById(num4).regionId)
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
							return;
						}
						Debug.LogError("Unhandled region");
					}
					else
					{
						if (i == count - 1)
						{
							return;
						}
						this.m_DrawOrder.RemoveAt(i);
						this.m_DrawOrder.Add(curveID);
						this.ValidateCurveList();
						return;
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
			else
			{
				if (this.hasSelection)
				{
					result = (this.IsCurveSelected(cw1) || this.IsCurveSelected(cw2));
				}
			}
			return result;
		}
		private void DrawCurves(CurveWrapper[] curves)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			List<CurveSelection> list = (this.m_DisplayedSelection == null) ? this.m_Selection : this.m_DisplayedSelection;
			for (int i = 0; i < this.m_DrawOrder.Count; i++)
			{
				CurveWrapper curveWrapperById = this.getCurveWrapperById(this.m_DrawOrder[i]);
				if (curveWrapperById != null)
				{
					if (!curveWrapperById.hidden)
					{
						if (curveWrapperById.curve.length != 0)
						{
							CurveWrapper cw = null;
							if (i < this.m_DrawOrder.Count - 1)
							{
								cw = this.getCurveWrapperById(this.m_DrawOrder[i + 1]);
							}
							if (this.IsRegion(curveWrapperById, cw))
							{
								i++;
								bool hasFocus = this.ShouldCurveHaveFocus(i, curveWrapperById, cw);
								this.DrawCurvesAndRegion(curveWrapperById, cw, (!this.IsRegionCurveSelected(curveWrapperById, cw)) ? null : list, hasFocus);
							}
							else
							{
								bool hasFocus2 = this.ShouldCurveHaveFocus(i, curveWrapperById, null);
								this.DrawCurveAndPoints(curveWrapperById, (!this.IsCurveSelected(curveWrapperById)) ? null : list, hasFocus2);
							}
						}
					}
				}
			}
			if (this.m_DraggingCurveOrRegion != null)
			{
				return;
			}
			HandleUtility.ApplyWireMaterial();
			GL.Begin(1);
			GL.Color(this.m_TangentColor * new Color(1f, 1f, 1f, 0.75f));
			foreach (CurveSelection current in list)
			{
				if (!current.semiSelected)
				{
					Vector2 position = this.GetPosition(current);
					if (CurveUtility.GetKeyTangentMode(current.keyframe, 0) == TangentMode.Editable && current.keyframe.time != current.curve.keys[0].time)
					{
						Vector2 position2 = this.GetPosition(new CurveSelection(current.curveID, this, current.key, CurveSelection.SelectionType.InTangent));
						this.DrawLine(position2, position);
					}
					if (CurveUtility.GetKeyTangentMode(current.keyframe, 1) == TangentMode.Editable && current.keyframe.time != current.curve.keys[current.curve.keys.Length - 1].time)
					{
						Vector2 position3 = this.GetPosition(new CurveSelection(current.curveID, this, current.key, CurveSelection.SelectionType.OutTangent));
						this.DrawLine(position, position3);
					}
				}
			}
			GL.End();
			GUI.color = this.m_TangentColor;
			foreach (CurveSelection current2 in list)
			{
				if (!current2.semiSelected)
				{
					if (CurveUtility.GetKeyTangentMode(current2.keyframe, 0) == TangentMode.Editable && current2.keyframe.time != current2.curve.keys[0].time)
					{
						Vector2 position4 = this.GetPosition(new CurveSelection(current2.curveID, this, current2.key, CurveSelection.SelectionType.InTangent));
						this.DrawPoint(position4.x, position4.y, this.ms_Styles.pointIcon);
					}
					if (CurveUtility.GetKeyTangentMode(current2.keyframe, 1) == TangentMode.Editable && current2.keyframe.time != current2.curve.keys[current2.curve.keys.Length - 1].time)
					{
						Vector2 position5 = this.GetPosition(new CurveSelection(current2.curveID, this, current2.key, CurveSelection.SelectionType.OutTangent));
						this.DrawPoint(position5.x, position5.y, this.ms_Styles.pointIcon);
					}
				}
			}
			if (this.m_DraggingKey != null)
			{
				float num = base.vRangeMin;
				float num2 = base.vRangeMax;
				num = Mathf.Max(num, this.m_DraggingKey.vRangeMin);
				num2 = Mathf.Min(num2, this.m_DraggingKey.vRangeMax);
				Vector2 lhs = this.m_DraggedCoord + this.m_MoveCoord;
				lhs.x = Mathf.Clamp(lhs.x, base.hRangeMin, base.hRangeMax);
				lhs.y = Mathf.Clamp(lhs.y, num, num2);
				Vector2 vector = base.DrawingToViewTransformPoint(lhs);
				int numberOfDecimalsForMinimumDifference;
				if (this.invSnap != 0f)
				{
					numberOfDecimalsForMinimumDifference = MathUtils.GetNumberOfDecimalsForMinimumDifference(1f / this.invSnap);
				}
				else
				{
					numberOfDecimalsForMinimumDifference = MathUtils.GetNumberOfDecimalsForMinimumDifference(base.shownArea.width / base.drawRect.width);
				}
				int numberOfDecimalsForMinimumDifference2 = MathUtils.GetNumberOfDecimalsForMinimumDifference(base.shownArea.height / base.drawRect.height);
				Vector2 vector2 = (this.m_DraggingKey.getAxisUiScalarsCallback == null) ? Vector2.one : this.m_DraggingKey.getAxisUiScalarsCallback();
				if (vector2.x >= 0f)
				{
					lhs.x *= vector2.x;
				}
				if (vector2.y >= 0f)
				{
					lhs.y *= vector2.y;
				}
				GUIContent content = new GUIContent(string.Format("{0}, {1}", lhs.x.ToString("N" + numberOfDecimalsForMinimumDifference), lhs.y.ToString("N" + numberOfDecimalsForMinimumDifference2)));
				Vector2 vector3 = this.ms_Styles.dragLabel.CalcSize(content);
				EditorGUI.DoDropShadowLabel(new Rect(vector.x, vector.y - vector3.y, vector3.x, vector3.y), content, this.ms_Styles.dragLabel, 0.3f);
			}
		}
		private static List<Vector3> CreateRegion(CurveWrapper minCurve, CurveWrapper maxCurve, float deltaTime)
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
			Vector3 vector = new Vector3(0f, maxCurve.renderer.EvaluateCurveSlow(0f), 0f);
			Vector3 vector2 = new Vector3(0f, minCurve.renderer.EvaluateCurveSlow(0f), 0f);
			for (int k = 0; k < list2.Count; k++)
			{
				float num2 = list2[k];
				Vector3 vector3 = new Vector3(num2, maxCurve.renderer.EvaluateCurveSlow(num2), 0f);
				Vector3 vector4 = new Vector3(num2, minCurve.renderer.EvaluateCurveSlow(num2), 0f);
				if (vector.y >= vector2.y && vector3.y >= vector4.y)
				{
					list.Add(vector);
					list.Add(vector4);
					list.Add(vector2);
					list.Add(vector);
					list.Add(vector3);
					list.Add(vector4);
				}
				else
				{
					if (vector.y <= vector2.y && vector3.y <= vector4.y)
					{
						list.Add(vector2);
						list.Add(vector3);
						list.Add(vector);
						list.Add(vector2);
						list.Add(vector4);
						list.Add(vector3);
					}
					else
					{
						Vector2 zero = Vector2.zero;
						if (Mathf.LineIntersection(vector, vector3, vector2, vector4, ref zero))
						{
							list.Add(vector);
							list.Add(zero);
							list.Add(vector2);
							list.Add(vector3);
							list.Add(zero);
							list.Add(vector4);
						}
						else
						{
							Debug.Log("Error: No intersection found! There should be one...");
						}
					}
				}
				vector = vector3;
				vector2 = vector4;
			}
			return list;
		}
		public void DrawRegion(CurveWrapper curve1, CurveWrapper curve2, bool hasFocus)
		{
			float deltaTime = 1f / (base.rect.width / 10f);
			List<Vector3> list = CurveEditor.CreateRegion(curve1, curve2, deltaTime);
			Color color = curve1.color;
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = base.drawingToViewMatrix.MultiplyPoint(list[i]);
			}
			if (this.IsDraggingRegion(curve1, curve2))
			{
				color = Color.Lerp(color, Color.black, 0.1f);
				color.a = 0.4f;
			}
			else
			{
				if (base.settings.useFocusColors && !hasFocus)
				{
					color *= 0.4f;
					color.a = 0.1f;
				}
				else
				{
					color *= 1f;
					color.a = 0.4f;
				}
			}
			Shader.SetGlobalColor("_HandleColor", color);
			HandleUtility.ApplyWireMaterial();
			GL.Begin(4);
			int num = list.Count / 3;
			for (int j = 0; j < num; j++)
			{
				GL.Color(color);
				GL.Vertex(list[j * 3]);
				GL.Vertex(list[j * 3 + 1]);
				GL.Vertex(list[j * 3 + 2]);
			}
			GL.End();
		}
		private void DrawCurve(CurveWrapper cw, bool hasFocus)
		{
			CurveRenderer renderer = cw.renderer;
			Color color = cw.color;
			if (this.IsDraggingCurve(cw) || cw.selected == CurveWrapper.SelectionMode.Selected)
			{
				color = Color.Lerp(color, Color.white, 0.3f);
			}
			else
			{
				if (base.settings.useFocusColors && !hasFocus)
				{
					color *= 0.5f;
					color.a = 0.8f;
				}
			}
			Rect shownArea = base.shownArea;
			renderer.DrawCurve(shownArea.xMin, shownArea.xMax, color, base.drawingToViewMatrix, base.settings.wrapColor);
		}
		private void DrawPointsOnCurve(CurveWrapper cw, List<CurveSelection> selected, bool hasFocus)
		{
			this.m_PreviousDrawPointCenter = new Vector2(-3.40282347E+38f, -3.40282347E+38f);
			if (selected == null)
			{
				Color color = cw.color;
				if (base.settings.useFocusColors && !hasFocus)
				{
					color *= 0.5f;
				}
				GUI.color = color;
				Keyframe[] keys = cw.curve.keys;
				for (int i = 0; i < keys.Length; i++)
				{
					Keyframe keyframe = keys[i];
					this.DrawPoint(keyframe.time, keyframe.value, this.ms_Styles.pointIcon);
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
						Vector3 vector = base.DrawingToViewTransformPoint(new Vector3(keyframe2.time, keyframe2.value, 0f));
						vector = new Vector3(Mathf.Floor(vector.x), Mathf.Floor(vector.y), 0f);
						Rect position = new Rect(vector.x - 4f, vector.y - 4f, (float)this.ms_Styles.pointIcon.width, (float)this.ms_Styles.pointIcon.height);
						GUI.Label(position, this.ms_Styles.pointIconSelected, this.ms_Styles.none);
						GUI.color = Color.white;
						if (!selected[num].semiSelected)
						{
							GUI.Label(position, this.ms_Styles.pointIconSelectedOverlay, this.ms_Styles.none);
						}
						else
						{
							GUI.Label(position, this.ms_Styles.pointIconSemiSelectedOverlay, this.ms_Styles.none);
						}
						GUI.color = color2;
						num++;
					}
					else
					{
						this.DrawPoint(keyframe2.time, keyframe2.value, this.ms_Styles.pointIcon);
					}
					num2++;
				}
				GUI.color = Color.white;
			}
		}
		private void DrawPoint(float x, float y, Texture2D icon)
		{
			Vector3 vector = base.DrawingToViewTransformPoint(new Vector3(x, y, 0f));
			vector = new Vector3(Mathf.Floor(vector.x), Mathf.Floor(vector.y), 0f);
			Rect position = new Rect(vector.x - 4f, vector.y - 4f, (float)this.ms_Styles.pointIcon.width, (float)this.ms_Styles.pointIcon.height);
			Vector2 center = position.center;
			if ((center - this.m_PreviousDrawPointCenter).magnitude > 8f)
			{
				GUI.Label(position, icon, GUIStyleX.none);
				this.m_PreviousDrawPointCenter = center;
			}
		}
		private void DrawLine(Vector2 lhs, Vector2 rhs)
		{
			GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(lhs.x, lhs.y, 0f)));
			GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(rhs.x, rhs.y, 0f)));
		}
		private Vector2 GetAxisUiScalars(List<CurveWrapper> curvesWithSameParameterSpace)
		{
			if (this.m_Selection.Count <= 1)
			{
				if (this.m_DrawOrder.Count > 0)
				{
					CurveWrapper curveWrapperById = this.getCurveWrapperById(this.m_DrawOrder[this.m_DrawOrder.Count - 1]);
					if (curveWrapperById.getAxisUiScalarsCallback != null)
					{
						if (curvesWithSameParameterSpace != null)
						{
							curvesWithSameParameterSpace.Add(curveWrapperById);
						}
						return curveWrapperById.getAxisUiScalarsCallback();
					}
				}
				return Vector2.one;
			}
			Vector2 result = new Vector2(-1f, -1f);
			if (this.m_Selection.Count > 1)
			{
				bool flag = true;
				bool flag2 = true;
				Vector2 vector = Vector2.one;
				for (int i = 0; i < this.m_Selection.Count; i++)
				{
					CurveWrapper curveWrapper = this.m_Selection[i].curveWrapper;
					if (curveWrapper.getAxisUiScalarsCallback != null)
					{
						Vector2 vector2 = curveWrapper.getAxisUiScalarsCallback();
						if (i == 0)
						{
							vector = vector2;
						}
						else
						{
							if (Mathf.Abs(vector2.x - vector.x) > 1E-05f)
							{
								flag = false;
							}
							if (Mathf.Abs(vector2.y - vector.y) > 1E-05f)
							{
								flag2 = false;
							}
							vector = vector2;
						}
						if (curvesWithSameParameterSpace != null)
						{
							curvesWithSameParameterSpace.Add(curveWrapper);
						}
					}
				}
				if (flag)
				{
					result.x = vector.x;
				}
				if (flag2)
				{
					result.y = vector.y;
				}
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
			GUI.BeginGroup(base.drawRect);
			if (Event.current.type != EventType.Repaint)
			{
				GUI.EndGroup();
				return;
			}
			this.InitStyles();
			Color color = GUI.color;
			Vector2 axisUiScalars = this.GetAxisUiScalars(null);
			Rect shownArea = base.shownArea;
			base.hTicks.SetRanges(shownArea.xMin * axisUiScalars.x, shownArea.xMax * axisUiScalars.x, base.drawRect.xMin, base.drawRect.xMax);
			base.vTicks.SetRanges(shownArea.yMin * axisUiScalars.y, shownArea.yMax * axisUiScalars.y, base.drawRect.yMin, base.drawRect.yMax);
			HandleUtility.ApplyWireMaterial();
			GL.Begin(1);
			base.hTicks.SetTickStrengths((float)base.settings.hTickStyle.distMin, (float)base.settings.hTickStyle.distFull, false);
			float num;
			float num2;
			if (base.settings.hTickStyle.stubs)
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
					GL.Color(base.settings.hTickStyle.color * new Color(1f, 1f, 1f, strengthOfLevel) * new Color(1f, 1f, 1f, 0.75f));
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
			GL.Color(base.settings.hTickStyle.color * new Color(1f, 1f, 1f, 1f) * new Color(1f, 1f, 1f, 0.75f));
			if (base.hRangeMin != float.NegativeInfinity)
			{
				this.DrawLine(new Vector2(base.hRangeMin, num), new Vector2(base.hRangeMin, num2));
			}
			if (base.hRangeMax != float.PositiveInfinity)
			{
				this.DrawLine(new Vector2(base.hRangeMax, num), new Vector2(base.hRangeMax, num2));
			}
			base.vTicks.SetTickStrengths((float)base.settings.vTickStyle.distMin, (float)base.settings.vTickStyle.distFull, false);
			if (base.settings.vTickStyle.stubs)
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
					GL.Color(base.settings.vTickStyle.color * new Color(1f, 1f, 1f, strengthOfLevel2) * new Color(1f, 1f, 1f, 0.75f));
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
			GL.Color(base.settings.vTickStyle.color * new Color(1f, 1f, 1f, 1f) * new Color(1f, 1f, 1f, 0.75f));
			if (base.vRangeMin != float.NegativeInfinity)
			{
				this.DrawLine(new Vector2(num, base.vRangeMin), new Vector2(num2, base.vRangeMin));
			}
			if (base.vRangeMax != float.PositiveInfinity)
			{
				this.DrawLine(new Vector2(num, base.vRangeMax), new Vector2(num2, base.vRangeMax));
			}
			GL.End();
			if (base.settings.showAxisLabels)
			{
				if (base.settings.hTickStyle.distLabel > 0 && axisUiScalars.x > 0f)
				{
					GUI.color = base.settings.hTickStyle.labelColor;
					int levelWithMinSeparation = base.hTicks.GetLevelWithMinSeparation((float)base.settings.hTickStyle.distLabel);
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
							if (base.settings.hTickStyle.centerLabel)
							{
								textAnchor = TextAnchor.UpperCenter;
								position = new Rect(vector.x, vector.y - 16f - base.settings.hTickLabelOffset, 1f, 16f);
							}
							else
							{
								textAnchor = TextAnchor.UpperLeft;
								position = new Rect(vector.x, vector.y - 16f - base.settings.hTickLabelOffset, 50f, 16f);
							}
							if (this.ms_Styles.labelTickMarksX.alignment != textAnchor)
							{
								this.ms_Styles.labelTickMarksX.alignment = textAnchor;
							}
							GUI.Label(position, num3.ToString("n" + numberOfDecimalsForMinimumDifference) + base.settings.hTickStyle.unit, this.ms_Styles.labelTickMarksX);
						}
					}
				}
				if (base.settings.vTickStyle.distLabel > 0 && axisUiScalars.y > 0f)
				{
					GUI.color = base.settings.vTickStyle.labelColor;
					int levelWithMinSeparation2 = base.vTicks.GetLevelWithMinSeparation((float)base.settings.vTickStyle.distLabel);
					float[] ticksAtLevel4 = base.vTicks.GetTicksAtLevel(levelWithMinSeparation2, false);
					float[] array2 = (float[])ticksAtLevel4.Clone();
					int numberOfDecimalsForMinimumDifference2 = MathUtils.GetNumberOfDecimalsForMinimumDifference(base.vTicks.GetPeriodOfLevel(levelWithMinSeparation2));
					string text = "n" + numberOfDecimalsForMinimumDifference2;
					this.m_AxisLabelFormat = text;
					float width = 35f;
					if (!base.settings.vTickStyle.stubs && ticksAtLevel4.Length > 1)
					{
						float num4 = ticksAtLevel4[1];
						float num5 = ticksAtLevel4[ticksAtLevel4.Length - 1];
						string text2 = num4.ToString(text) + base.settings.vTickStyle.unit;
						string text3 = num5.ToString(text) + base.settings.vTickStyle.unit;
						width = Mathf.Max(this.ms_Styles.labelTickMarksY.CalcSize(new GUIContent(text2)).x, this.ms_Styles.labelTickMarksY.CalcSize(new GUIContent(text3)).x) + 6f;
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
							if (base.settings.vTickStyle.centerLabel)
							{
								position2 = new Rect(0f, vector2.y - 8f, base.leftmargin - 4f, 16f);
							}
							else
							{
								position2 = new Rect(0f, vector2.y - 13f, width, 16f);
							}
							GUI.Label(position2, num6.ToString(text) + base.settings.vTickStyle.unit, this.ms_Styles.labelTickMarksY);
						}
					}
				}
			}
			GUI.color = color;
			GUI.EndGroup();
		}
	}
}
