using System;
using UnityEngine;

namespace UnityEditor
{
	internal class CurveEditorRectangleTool : RectangleTool
	{
		private struct ToolLayout
		{
			public Rect selectionRect;

			public Rect hBarRect;

			public Rect hBarLeftRect;

			public Rect hBarRightRect;

			public bool displayHScale;

			public Rect vBarRect;

			public Rect vBarBottomRect;

			public Rect vBarTopRect;

			public bool displayVScale;

			public Rect selectionLeftRect;

			public Rect selectionTopRect;

			public Rect underlayTopRect;

			public Rect underlayLeftRect;

			public Rect scaleLeftRect;

			public Rect scaleRightRect;

			public Rect scaleTopRect;

			public Rect scaleBottomRect;

			public Vector2 leftLabelAnchor;

			public Vector2 rightLabelAnchor;

			public Vector2 bottomLabelAnchor;

			public Vector2 topLabelAnchor;
		}

		private enum DragMode
		{
			None,
			MoveHorizontal,
			MoveVertical,
			MoveBothAxis,
			ScaleHorizontal,
			ScaleVertical = 8,
			ScaleBothAxis = 12,
			MoveScaleHorizontal = 5,
			MoveScaleVertical = 10
		}

		private const int kHBarMinWidth = 14;

		private const int kHBarHeight = 13;

		private const int kHBarLeftWidth = 15;

		private const int kHBarLeftHeight = 13;

		private const int kHBarRightWidth = 15;

		private const int kHBarRightHeight = 13;

		private const int kHLabelMarginHorizontal = 3;

		private const int kHLabelMarginVertical = 1;

		private const int kVBarMinHeight = 15;

		private const int kVBarWidth = 13;

		private const int kVBarBottomWidth = 13;

		private const int kVBarBottomHeight = 15;

		private const int kVBarTopWidth = 13;

		private const int kVBarTopHeight = 15;

		private const int kVLabelMarginHorizontal = 1;

		private const int kVLabelMarginVertical = 2;

		private const int kScaleLeftWidth = 17;

		private const int kScaleLeftMarginHorizontal = 0;

		private const float kScaleLeftRatio = 0.8f;

		private const int kScaleRightWidth = 17;

		private const int kScaleRightMarginHorizontal = 0;

		private const float kScaleRightRatio = 0.8f;

		private const int kScaleBottomHeight = 17;

		private const int kScaleBottomMarginVertical = 0;

		private const float kScaleBottomRatio = 0.8f;

		private const int kScaleTopHeight = 17;

		private const int kScaleTopMarginVertical = 0;

		private const float kScaleTopRatio = 0.8f;

		private static Rect g_EmptyRect = new Rect(0f, 0f, 0f, 0f);

		private CurveEditor m_CurveEditor;

		private CurveEditorRectangleTool.ToolLayout m_Layout;

		private Vector2 m_Pivot;

		private Vector2 m_Previous;

		private Vector2 m_MouseOffset;

		private CurveEditorRectangleTool.DragMode m_DragMode;

		private bool m_RippleTime;

		private float m_RippleTimeStart;

		private float m_RippleTimeEnd;

		private AreaManipulator m_HBarLeft;

		private AreaManipulator m_HBarRight;

		private AreaManipulator m_HBar;

		private AreaManipulator m_VBarBottom;

		private AreaManipulator m_VBarTop;

		private AreaManipulator m_VBar;

		private AreaManipulator m_SelectionBox;

		private AreaManipulator m_SelectionScaleLeft;

		private AreaManipulator m_SelectionScaleRight;

		private AreaManipulator m_SelectionScaleBottom;

		private AreaManipulator m_SelectionScaleTop;

		private bool hasSelection
		{
			get
			{
				return this.m_CurveEditor.hasSelection && !this.m_CurveEditor.IsDraggingCurveOrRegion();
			}
		}

		private Bounds selectionBounds
		{
			get
			{
				return this.m_CurveEditor.selectionBounds;
			}
		}

		private float frameRate
		{
			get
			{
				return this.m_CurveEditor.invSnap;
			}
		}

		private CurveEditorRectangleTool.DragMode dragMode
		{
			get
			{
				CurveEditorRectangleTool.DragMode result;
				if (this.m_DragMode != CurveEditorRectangleTool.DragMode.None)
				{
					result = this.m_DragMode;
				}
				else if (this.m_CurveEditor.IsDraggingKey())
				{
					result = CurveEditorRectangleTool.DragMode.MoveBothAxis;
				}
				else
				{
					result = CurveEditorRectangleTool.DragMode.None;
				}
				return result;
			}
		}

		public override void Initialize(TimeArea timeArea)
		{
			base.Initialize(timeArea);
			this.m_CurveEditor = (timeArea as CurveEditor);
			if (this.m_HBarLeft == null)
			{
				this.m_HBarLeft = new AreaManipulator(base.styles.rectangleToolHBarLeft, MouseCursor.ResizeHorizontal);
				AreaManipulator expr_3D = this.m_HBarLeft;
				expr_3D.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_3D.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartScale(RectangleTool.ToolCoord.Right, RectangleTool.ToolCoord.Left, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), CurveEditorRectangleTool.DragMode.ScaleHorizontal, base.rippleTimeClutch);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_64 = this.m_HBarLeft;
				expr_64.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_64.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnScaleTime(base.PixelToTime(evt.mousePosition.x, this.frameRate));
					return true;
				}));
				AreaManipulator expr_8B = this.m_HBarLeft;
				expr_8B.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_8B.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndScale();
					return true;
				}));
			}
			if (this.m_HBarRight == null)
			{
				this.m_HBarRight = new AreaManipulator(base.styles.rectangleToolHBarRight, MouseCursor.ResizeHorizontal);
				AreaManipulator expr_D6 = this.m_HBarRight;
				expr_D6.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_D6.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartScale(RectangleTool.ToolCoord.Left, RectangleTool.ToolCoord.Right, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), CurveEditorRectangleTool.DragMode.ScaleHorizontal, base.rippleTimeClutch);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_FD = this.m_HBarRight;
				expr_FD.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_FD.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnScaleTime(base.PixelToTime(evt.mousePosition.x, this.frameRate));
					return true;
				}));
				AreaManipulator expr_124 = this.m_HBarRight;
				expr_124.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_124.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndScale();
					return true;
				}));
			}
			if (this.m_HBar == null)
			{
				this.m_HBar = new AreaManipulator(base.styles.rectangleToolHBar, MouseCursor.MoveArrow);
				AreaManipulator expr_16F = this.m_HBar;
				expr_16F.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_16F.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartMove(new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), CurveEditorRectangleTool.DragMode.MoveHorizontal, base.rippleTimeClutch);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_196 = this.m_HBar;
				expr_196.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_196.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnMove(new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f));
					return true;
				}));
				AreaManipulator expr_1BD = this.m_HBar;
				expr_1BD.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_1BD.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndMove();
					return true;
				}));
			}
			if (this.m_VBarBottom == null)
			{
				this.m_VBarBottom = new AreaManipulator(base.styles.rectangleToolVBarBottom, MouseCursor.ResizeVertical);
				AreaManipulator expr_208 = this.m_VBarBottom;
				expr_208.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_208.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartScale(RectangleTool.ToolCoord.Top, RectangleTool.ToolCoord.Bottom, new Vector2(0f, base.PixelToValue(evt.mousePosition.y)), CurveEditorRectangleTool.DragMode.ScaleVertical, false);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_22F = this.m_VBarBottom;
				expr_22F.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_22F.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnScaleValue(base.PixelToValue(evt.mousePosition.y));
					return true;
				}));
				AreaManipulator expr_256 = this.m_VBarBottom;
				expr_256.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_256.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndScale();
					return true;
				}));
			}
			if (this.m_VBarTop == null)
			{
				this.m_VBarTop = new AreaManipulator(base.styles.rectangleToolVBarTop, MouseCursor.ResizeVertical);
				AreaManipulator expr_2A1 = this.m_VBarTop;
				expr_2A1.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_2A1.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartScale(RectangleTool.ToolCoord.Bottom, RectangleTool.ToolCoord.Top, new Vector2(0f, base.PixelToValue(evt.mousePosition.y)), CurveEditorRectangleTool.DragMode.ScaleVertical, false);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_2C8 = this.m_VBarTop;
				expr_2C8.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_2C8.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnScaleValue(base.PixelToValue(evt.mousePosition.y));
					return true;
				}));
				AreaManipulator expr_2EF = this.m_VBarTop;
				expr_2EF.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_2EF.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndScale();
					return true;
				}));
			}
			if (this.m_VBar == null)
			{
				this.m_VBar = new AreaManipulator(base.styles.rectangleToolVBar, MouseCursor.MoveArrow);
				AreaManipulator expr_33A = this.m_VBar;
				expr_33A.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_33A.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartMove(new Vector2(0f, base.PixelToValue(evt.mousePosition.y)), CurveEditorRectangleTool.DragMode.MoveVertical, false);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_361 = this.m_VBar;
				expr_361.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_361.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnMove(new Vector2(0f, base.PixelToValue(evt.mousePosition.y)));
					return true;
				}));
				AreaManipulator expr_388 = this.m_VBar;
				expr_388.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_388.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndMove();
					return true;
				}));
			}
			if (this.m_SelectionBox == null)
			{
				this.m_SelectionBox = new AreaManipulator(base.styles.rectangleToolSelection, MouseCursor.MoveArrow);
				AreaManipulator expr_3D3 = this.m_SelectionBox;
				expr_3D3.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_3D3.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool flag = evt.shift || EditorGUI.actionKey;
					bool result;
					if ((!Mathf.Approximately(this.selectionBounds.size.x, 0f) || !Mathf.Approximately(this.selectionBounds.size.y, 0f)) && !flag && this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartMove(new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), base.PixelToValue(evt.mousePosition.y)), (!base.rippleTimeClutch) ? CurveEditorRectangleTool.DragMode.MoveBothAxis : CurveEditorRectangleTool.DragMode.MoveHorizontal, base.rippleTimeClutch);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_3FA = this.m_SelectionBox;
				expr_3FA.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_3FA.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnMove(new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), base.PixelToValue(evt.mousePosition.y)));
					return true;
				}));
				AreaManipulator expr_421 = this.m_SelectionBox;
				expr_421.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_421.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndMove();
					return true;
				}));
			}
			if (this.m_SelectionScaleLeft == null)
			{
				this.m_SelectionScaleLeft = new AreaManipulator(base.styles.rectangleToolScaleLeft, MouseCursor.ResizeHorizontal);
				AreaManipulator expr_46C = this.m_SelectionScaleLeft;
				expr_46C.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_46C.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartScale(RectangleTool.ToolCoord.Right, RectangleTool.ToolCoord.Left, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), CurveEditorRectangleTool.DragMode.ScaleHorizontal, base.rippleTimeClutch);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_493 = this.m_SelectionScaleLeft;
				expr_493.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_493.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnScaleTime(base.PixelToTime(evt.mousePosition.x, this.frameRate));
					return true;
				}));
				AreaManipulator expr_4BA = this.m_SelectionScaleLeft;
				expr_4BA.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_4BA.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndScale();
					return true;
				}));
			}
			if (this.m_SelectionScaleRight == null)
			{
				this.m_SelectionScaleRight = new AreaManipulator(base.styles.rectangleToolScaleRight, MouseCursor.ResizeHorizontal);
				AreaManipulator expr_505 = this.m_SelectionScaleRight;
				expr_505.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_505.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartScale(RectangleTool.ToolCoord.Left, RectangleTool.ToolCoord.Right, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), CurveEditorRectangleTool.DragMode.ScaleHorizontal, base.rippleTimeClutch);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_52C = this.m_SelectionScaleRight;
				expr_52C.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_52C.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnScaleTime(base.PixelToTime(evt.mousePosition.x, this.frameRate));
					return true;
				}));
				AreaManipulator expr_553 = this.m_SelectionScaleRight;
				expr_553.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_553.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndScale();
					return true;
				}));
			}
			if (this.m_SelectionScaleBottom == null)
			{
				this.m_SelectionScaleBottom = new AreaManipulator(base.styles.rectangleToolScaleBottom, MouseCursor.ResizeVertical);
				AreaManipulator expr_59E = this.m_SelectionScaleBottom;
				expr_59E.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_59E.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartScale(RectangleTool.ToolCoord.Top, RectangleTool.ToolCoord.Bottom, new Vector2(0f, base.PixelToValue(evt.mousePosition.y)), CurveEditorRectangleTool.DragMode.ScaleVertical, false);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_5C5 = this.m_SelectionScaleBottom;
				expr_5C5.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_5C5.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnScaleValue(base.PixelToValue(evt.mousePosition.y));
					return true;
				}));
				AreaManipulator expr_5EC = this.m_SelectionScaleBottom;
				expr_5EC.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_5EC.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndScale();
					return true;
				}));
			}
			if (this.m_SelectionScaleTop == null)
			{
				this.m_SelectionScaleTop = new AreaManipulator(base.styles.rectangleToolScaleTop, MouseCursor.ResizeVertical);
				AreaManipulator expr_637 = this.m_SelectionScaleTop;
				expr_637.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_637.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartScale(RectangleTool.ToolCoord.Bottom, RectangleTool.ToolCoord.Top, new Vector2(0f, base.PixelToValue(evt.mousePosition.y)), CurveEditorRectangleTool.DragMode.ScaleVertical, false);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_65E = this.m_SelectionScaleTop;
				expr_65E.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_65E.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnScaleValue(base.PixelToValue(evt.mousePosition.y));
					return true;
				}));
				AreaManipulator expr_685 = this.m_SelectionScaleTop;
				expr_685.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_685.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndScale();
					return true;
				}));
			}
		}

		public void OnGUI()
		{
			if (this.hasSelection)
			{
				if (Event.current.type == EventType.Repaint)
				{
					CurveEditorSettings.RectangleToolFlags rectangleToolFlags = this.m_CurveEditor.settings.rectangleToolFlags;
					if (rectangleToolFlags != CurveEditorSettings.RectangleToolFlags.NoRectangleTool)
					{
						Color color = GUI.color;
						GUI.color = Color.white;
						this.m_Layout = this.CalculateLayout();
						if (rectangleToolFlags == CurveEditorSettings.RectangleToolFlags.FullRectangleTool)
						{
							GUI.Label(this.m_Layout.selectionLeftRect, GUIContent.none, base.styles.rectangleToolHighlight);
							GUI.Label(this.m_Layout.selectionTopRect, GUIContent.none, base.styles.rectangleToolHighlight);
							GUI.Label(this.m_Layout.underlayLeftRect, GUIContent.none, base.styles.rectangleToolHighlight);
							GUI.Label(this.m_Layout.underlayTopRect, GUIContent.none, base.styles.rectangleToolHighlight);
						}
						this.m_SelectionBox.OnGUI(this.m_Layout.selectionRect);
						this.m_SelectionScaleTop.OnGUI(this.m_Layout.scaleTopRect);
						this.m_SelectionScaleBottom.OnGUI(this.m_Layout.scaleBottomRect);
						this.m_SelectionScaleLeft.OnGUI(this.m_Layout.scaleLeftRect);
						this.m_SelectionScaleRight.OnGUI(this.m_Layout.scaleRightRect);
						GUI.color = color;
					}
				}
			}
		}

		public void OverlayOnGUI()
		{
			if (this.hasSelection)
			{
				if (Event.current.type == EventType.Repaint)
				{
					Color color = GUI.color;
					CurveEditorSettings.RectangleToolFlags rectangleToolFlags = this.m_CurveEditor.settings.rectangleToolFlags;
					if (rectangleToolFlags == CurveEditorSettings.RectangleToolFlags.FullRectangleTool)
					{
						GUI.color = Color.white;
						this.m_HBar.OnGUI(this.m_Layout.hBarRect);
						this.m_HBarLeft.OnGUI(this.m_Layout.hBarLeftRect);
						this.m_HBarRight.OnGUI(this.m_Layout.hBarRightRect);
						this.m_VBar.OnGUI(this.m_Layout.vBarRect);
						this.m_VBarBottom.OnGUI(this.m_Layout.vBarBottomRect);
						this.m_VBarTop.OnGUI(this.m_Layout.vBarTopRect);
					}
					this.DrawLabels();
					GUI.color = color;
				}
			}
		}

		public void HandleEvents()
		{
			this.m_SelectionScaleTop.HandleEvents();
			this.m_SelectionScaleBottom.HandleEvents();
			this.m_SelectionScaleLeft.HandleEvents();
			this.m_SelectionScaleRight.HandleEvents();
			this.m_SelectionBox.HandleEvents();
		}

		public void HandleOverlayEvents()
		{
			base.HandleClutchKeys();
			CurveEditorSettings.RectangleToolFlags rectangleToolFlags = this.m_CurveEditor.settings.rectangleToolFlags;
			if (rectangleToolFlags == CurveEditorSettings.RectangleToolFlags.FullRectangleTool)
			{
				this.m_VBarBottom.HandleEvents();
				this.m_VBarTop.HandleEvents();
				this.m_VBar.HandleEvents();
				this.m_HBarLeft.HandleEvents();
				this.m_HBarRight.HandleEvents();
				this.m_HBar.HandleEvents();
			}
		}

		private CurveEditorRectangleTool.ToolLayout CalculateLayout()
		{
			CurveEditorRectangleTool.ToolLayout result = default(CurveEditorRectangleTool.ToolLayout);
			bool flag = !Mathf.Approximately(this.selectionBounds.size.x, 0f);
			bool flag2 = !Mathf.Approximately(this.selectionBounds.size.y, 0f);
			float num = base.TimeToPixel(this.selectionBounds.min.x);
			float num2 = base.TimeToPixel(this.selectionBounds.max.x);
			float num3 = base.ValueToPixel(this.selectionBounds.max.y);
			float num4 = base.ValueToPixel(this.selectionBounds.min.y);
			result.selectionRect = new Rect(num, num3, num2 - num, num4 - num3);
			result.displayHScale = true;
			float num5 = result.selectionRect.width - 15f - 15f;
			if (num5 < 14f)
			{
				result.displayHScale = false;
				num5 = result.selectionRect.width;
				if (num5 < 14f)
				{
					result.selectionRect.x = result.selectionRect.center.x - 7f;
					result.selectionRect.width = 14f;
					num5 = 14f;
				}
			}
			if (result.displayHScale)
			{
				result.hBarLeftRect = new Rect(result.selectionRect.xMin, base.contentRect.yMin, 15f, 13f);
				result.hBarRect = new Rect(result.hBarLeftRect.xMax, base.contentRect.yMin, num5, 13f);
				result.hBarRightRect = new Rect(result.hBarRect.xMax, base.contentRect.yMin, 15f, 13f);
			}
			else
			{
				result.hBarRect = new Rect(result.selectionRect.xMin, base.contentRect.yMin, num5, 13f);
				result.hBarLeftRect = new Rect(0f, 0f, 0f, 0f);
				result.hBarRightRect = new Rect(0f, 0f, 0f, 0f);
			}
			result.displayVScale = true;
			float num6 = result.selectionRect.height - 15f - 15f;
			if (num6 < 15f)
			{
				result.displayVScale = false;
				num6 = result.selectionRect.height;
				if (num6 < 15f)
				{
					result.selectionRect.y = result.selectionRect.center.y - 7.5f;
					result.selectionRect.height = 15f;
					num6 = 15f;
				}
			}
			if (result.displayVScale)
			{
				result.vBarTopRect = new Rect(base.contentRect.xMin, result.selectionRect.yMin, 13f, 15f);
				result.vBarRect = new Rect(base.contentRect.xMin, result.vBarTopRect.yMax, 13f, num6);
				result.vBarBottomRect = new Rect(base.contentRect.xMin, result.vBarRect.yMax, 13f, 15f);
			}
			else
			{
				result.vBarRect = new Rect(base.contentRect.xMin, result.selectionRect.yMin, 13f, num6);
				result.vBarTopRect = CurveEditorRectangleTool.g_EmptyRect;
				result.vBarBottomRect = CurveEditorRectangleTool.g_EmptyRect;
			}
			if (flag)
			{
				float num7 = 0.099999994f;
				float num8 = 0.099999994f;
				result.scaleLeftRect = new Rect(result.selectionRect.xMin - 17f, result.selectionRect.yMin + result.selectionRect.height * num7, 17f, result.selectionRect.height * 0.8f);
				result.scaleRightRect = new Rect(result.selectionRect.xMax, result.selectionRect.yMin + result.selectionRect.height * num8, 17f, result.selectionRect.height * 0.8f);
			}
			else
			{
				result.scaleLeftRect = CurveEditorRectangleTool.g_EmptyRect;
				result.scaleRightRect = CurveEditorRectangleTool.g_EmptyRect;
			}
			if (flag2)
			{
				float num9 = 0.099999994f;
				float num10 = 0.099999994f;
				result.scaleTopRect = new Rect(result.selectionRect.xMin + result.selectionRect.width * num10, result.selectionRect.yMin - 17f, result.selectionRect.width * 0.8f, 17f);
				result.scaleBottomRect = new Rect(result.selectionRect.xMin + result.selectionRect.width * num9, result.selectionRect.yMax, result.selectionRect.width * 0.8f, 17f);
			}
			else
			{
				result.scaleTopRect = CurveEditorRectangleTool.g_EmptyRect;
				result.scaleBottomRect = CurveEditorRectangleTool.g_EmptyRect;
			}
			if (flag)
			{
				result.leftLabelAnchor = new Vector2(result.selectionRect.xMin - 3f, base.contentRect.yMin + 1f);
				result.rightLabelAnchor = new Vector2(result.selectionRect.xMax + 3f, base.contentRect.yMin + 1f);
			}
			else
			{
				result.leftLabelAnchor = (result.rightLabelAnchor = new Vector2(result.selectionRect.xMax + 3f, base.contentRect.yMin + 1f));
			}
			if (flag2)
			{
				result.bottomLabelAnchor = new Vector2(base.contentRect.xMin + 1f, result.selectionRect.yMax + 2f);
				result.topLabelAnchor = new Vector2(base.contentRect.xMin + 1f, result.selectionRect.yMin - 2f);
			}
			else
			{
				result.bottomLabelAnchor = (result.topLabelAnchor = new Vector2(base.contentRect.xMin + 1f, result.selectionRect.yMin - 2f));
			}
			result.selectionLeftRect = new Rect(base.contentRect.xMin + 13f, result.selectionRect.yMin, result.selectionRect.xMin - 13f, result.selectionRect.height);
			result.selectionTopRect = new Rect(result.selectionRect.xMin, base.contentRect.yMin + 13f, result.selectionRect.width, result.selectionRect.yMin - 13f);
			result.underlayTopRect = new Rect(base.contentRect.xMin, base.contentRect.yMin, base.contentRect.width, 13f);
			result.underlayLeftRect = new Rect(base.contentRect.xMin, base.contentRect.yMin + 13f, 13f, base.contentRect.height - 13f);
			return result;
		}

		private void DrawLabels()
		{
			if (this.dragMode != CurveEditorRectangleTool.DragMode.None)
			{
				CurveEditorSettings.RectangleToolFlags rectangleToolFlags = this.m_CurveEditor.settings.rectangleToolFlags;
				bool flag = !Mathf.Approximately(this.selectionBounds.size.x, 0f);
				bool flag2 = !Mathf.Approximately(this.selectionBounds.size.y, 0f);
				if (rectangleToolFlags == CurveEditorSettings.RectangleToolFlags.FullRectangleTool)
				{
					if ((this.dragMode & CurveEditorRectangleTool.DragMode.MoveScaleHorizontal) != CurveEditorRectangleTool.DragMode.None)
					{
						if (flag)
						{
							GUIContent content = new GUIContent(string.Format("{0}", this.m_CurveEditor.FormatTime(this.selectionBounds.min.x, this.m_CurveEditor.invSnap, this.m_CurveEditor.timeFormat)));
							GUIContent content2 = new GUIContent(string.Format("{0}", this.m_CurveEditor.FormatTime(this.selectionBounds.max.x, this.m_CurveEditor.invSnap, this.m_CurveEditor.timeFormat)));
							Vector2 vector = base.styles.dragLabel.CalcSize(content);
							Vector2 vector2 = base.styles.dragLabel.CalcSize(content2);
							EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.leftLabelAnchor.x - vector.x, this.m_Layout.leftLabelAnchor.y, vector.x, vector.y), content, base.styles.dragLabel, 0.3f);
							EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.rightLabelAnchor.x, this.m_Layout.rightLabelAnchor.y, vector2.x, vector2.y), content2, base.styles.dragLabel, 0.3f);
						}
						else
						{
							GUIContent content3 = new GUIContent(string.Format("{0}", this.m_CurveEditor.FormatTime(this.selectionBounds.center.x, this.m_CurveEditor.invSnap, this.m_CurveEditor.timeFormat)));
							Vector2 vector3 = base.styles.dragLabel.CalcSize(content3);
							EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.leftLabelAnchor.x, this.m_Layout.leftLabelAnchor.y, vector3.x, vector3.y), content3, base.styles.dragLabel, 0.3f);
						}
					}
					if ((this.dragMode & CurveEditorRectangleTool.DragMode.MoveScaleVertical) != CurveEditorRectangleTool.DragMode.None)
					{
						if (flag2)
						{
							GUIContent content4 = new GUIContent(string.Format("{0}", this.m_CurveEditor.FormatValue(this.selectionBounds.min.y)));
							GUIContent content5 = new GUIContent(string.Format("{0}", this.m_CurveEditor.FormatValue(this.selectionBounds.max.y)));
							Vector2 vector4 = base.styles.dragLabel.CalcSize(content4);
							Vector2 vector5 = base.styles.dragLabel.CalcSize(content5);
							EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.bottomLabelAnchor.x, this.m_Layout.bottomLabelAnchor.y, vector4.x, vector4.y), content4, base.styles.dragLabel, 0.3f);
							EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.topLabelAnchor.x, this.m_Layout.topLabelAnchor.y - vector5.y, vector5.x, vector5.y), content5, base.styles.dragLabel, 0.3f);
						}
						else
						{
							GUIContent content6 = new GUIContent(string.Format("{0}", this.m_CurveEditor.FormatValue(this.selectionBounds.center.y)));
							Vector2 vector6 = base.styles.dragLabel.CalcSize(content6);
							EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.topLabelAnchor.x, this.m_Layout.topLabelAnchor.y - vector6.y, vector6.x, vector6.y), content6, base.styles.dragLabel, 0.3f);
						}
					}
				}
				else if (rectangleToolFlags == CurveEditorSettings.RectangleToolFlags.MiniRectangleTool)
				{
					if ((this.dragMode & CurveEditorRectangleTool.DragMode.MoveBothAxis) != CurveEditorRectangleTool.DragMode.None)
					{
						Vector2 vector7 = (!flag && !flag2) ? this.selectionBounds.center : new Vector2(base.PixelToTime(Event.current.mousePosition.x, this.frameRate), base.PixelToValue(Event.current.mousePosition.y));
						Vector2 vector8 = new Vector2(base.TimeToPixel(vector7.x), base.ValueToPixel(vector7.y));
						GUIContent content7 = new GUIContent(string.Format("{0}, {1}", this.m_CurveEditor.FormatTime(vector7.x, this.m_CurveEditor.invSnap, this.m_CurveEditor.timeFormat), this.m_CurveEditor.FormatValue(vector7.y)));
						Vector2 vector9 = base.styles.dragLabel.CalcSize(content7);
						EditorGUI.DoDropShadowLabel(new Rect(vector8.x, vector8.y - vector9.y, vector9.x, vector9.y), content7, base.styles.dragLabel, 0.3f);
					}
				}
			}
		}

		private void OnStartScale(RectangleTool.ToolCoord pivotCoord, RectangleTool.ToolCoord pickedCoord, Vector2 mousePos, CurveEditorRectangleTool.DragMode dragMode, bool rippleTime)
		{
			Bounds selectionBounds = this.selectionBounds;
			this.m_DragMode = dragMode;
			this.m_Pivot = base.ToolCoordToPosition(pivotCoord, selectionBounds);
			this.m_Previous = base.ToolCoordToPosition(pickedCoord, selectionBounds);
			this.m_MouseOffset = mousePos - this.m_Previous;
			this.m_RippleTime = rippleTime;
			this.m_RippleTimeStart = selectionBounds.min.x;
			this.m_RippleTimeEnd = selectionBounds.max.x;
			this.m_CurveEditor.StartLiveEdit();
		}

		private void OnScaleTime(float time)
		{
			Matrix4x4 matrix;
			bool flipX;
			if (base.CalculateScaleTimeMatrix(this.m_Previous.x, time, this.m_MouseOffset.x, this.m_Pivot.x, this.frameRate, out matrix, out flipX))
			{
				this.TransformKeys(matrix, flipX, false);
			}
		}

		private void OnScaleValue(float val)
		{
			Matrix4x4 matrix;
			bool flipY;
			if (base.CalculateScaleValueMatrix(this.m_Previous.y, val, this.m_MouseOffset.y, this.m_Pivot.y, out matrix, out flipY))
			{
				this.TransformKeys(matrix, false, flipY);
			}
		}

		private void OnEndScale()
		{
			this.m_CurveEditor.EndLiveEdit();
			this.m_DragMode = CurveEditorRectangleTool.DragMode.None;
			GUI.changed = true;
		}

		private void OnStartMove(Vector2 position, CurveEditorRectangleTool.DragMode dragMode, bool rippleTime)
		{
			Bounds selectionBounds = this.selectionBounds;
			this.m_DragMode = dragMode;
			this.m_Previous = position;
			this.m_RippleTime = rippleTime;
			this.m_RippleTimeStart = selectionBounds.min.x;
			this.m_RippleTimeEnd = selectionBounds.max.x;
			this.m_CurveEditor.StartLiveEdit();
		}

		private void OnMove(Vector2 position)
		{
			Vector2 vector = position - this.m_Previous;
			Matrix4x4 identity = Matrix4x4.identity;
			identity.SetTRS(new Vector3(vector.x, vector.y, 0f), Quaternion.identity, Vector3.one);
			this.TransformKeys(identity, false, false);
		}

		private void OnEndMove()
		{
			this.m_CurveEditor.EndLiveEdit();
			this.m_DragMode = CurveEditorRectangleTool.DragMode.None;
			GUI.changed = true;
		}

		private void TransformKeys(Matrix4x4 matrix, bool flipX, bool flipY)
		{
			if (this.m_RippleTime)
			{
				this.m_CurveEditor.TransformRippleKeys(matrix, this.m_RippleTimeStart, this.m_RippleTimeEnd, flipX);
				GUI.changed = true;
			}
			else
			{
				this.m_CurveEditor.TransformSelectedKeys(matrix, flipX, flipY);
				GUI.changed = true;
			}
		}
	}
}
