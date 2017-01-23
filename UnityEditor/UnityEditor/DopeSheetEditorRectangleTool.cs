using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class DopeSheetEditorRectangleTool : RectangleTool
	{
		private struct ToolLayout
		{
			public Rect summaryRect;

			public Rect selectionRect;

			public Rect hBarRect;

			public Rect hBarOverlayRect;

			public Rect hBarLeftRect;

			public Rect hBarRightRect;

			public bool displayHScale;

			public Rect vBarRect;

			public Rect vBarOverlayRect;

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
		}

		private const float kDefaultFrameRate = 60f;

		private const int kScaleLeftWidth = 17;

		private const int kScaleLeftMarginHorizontal = 0;

		private const float kScaleLeftMarginVertical = 4f;

		private const int kScaleRightWidth = 17;

		private const int kScaleRightMarginHorizontal = 0;

		private const float kScaleRightMarginVertical = 4f;

		private const int kHLabelMarginHorizontal = 8;

		private const int kHLabelMarginVertical = 1;

		private static Rect g_EmptyRect = new Rect(0f, 0f, 0f, 0f);

		private DopeSheetEditor m_DopeSheetEditor;

		private AnimationWindowState m_State;

		private DopeSheetEditorRectangleTool.ToolLayout m_Layout;

		private Vector2 m_Pivot;

		private Vector2 m_Previous;

		private Vector2 m_MouseOffset;

		private bool m_IsDragging;

		private bool m_RippleTime;

		private float m_RippleTimeStart;

		private float m_RippleTimeEnd;

		private bool m_IncrementalUpdate;

		private AreaManipulator[] m_SelectionBoxes;

		private AreaManipulator m_SelectionScaleLeft;

		private AreaManipulator m_SelectionScaleRight;

		private bool hasSelection
		{
			get
			{
				return this.m_State.selectedKeys.Count > 0;
			}
		}

		private Bounds selectionBounds
		{
			get
			{
				return this.m_DopeSheetEditor.selectionBounds;
			}
		}

		private float frameRate
		{
			get
			{
				return this.m_State.frameRate;
			}
		}

		private bool isDragging
		{
			get
			{
				return this.m_IsDragging || this.m_DopeSheetEditor.isDragging;
			}
		}

		public override void Initialize(TimeArea timeArea)
		{
			base.Initialize(timeArea);
			this.m_DopeSheetEditor = (timeArea as DopeSheetEditor);
			this.m_State = this.m_DopeSheetEditor.state;
			if (this.m_SelectionBoxes == null)
			{
				this.m_SelectionBoxes = new AreaManipulator[2];
				for (int i = 0; i < 2; i++)
				{
					this.m_SelectionBoxes[i] = new AreaManipulator(base.styles.rectangleToolSelection, MouseCursor.MoveArrow);
					AreaManipulator expr_66 = this.m_SelectionBoxes[i];
					expr_66.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_66.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
					{
						bool result;
						if (!evt.shift && !EditorGUI.actionKey && this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
						{
							this.OnStartMove(new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), base.rippleTimeClutch);
							result = true;
						}
						else
						{
							result = false;
						}
						return result;
					}));
					AreaManipulator expr_8F = this.m_SelectionBoxes[i];
					expr_8F.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_8F.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
					{
						this.OnMove(new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f));
						return true;
					}));
					AreaManipulator expr_B8 = this.m_SelectionBoxes[i];
					expr_B8.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_B8.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
					{
						this.OnEndMove();
						return true;
					}));
				}
			}
			if (this.m_SelectionScaleLeft == null)
			{
				this.m_SelectionScaleLeft = new AreaManipulator(base.styles.dopesheetScaleLeft, MouseCursor.ResizeHorizontal);
				AreaManipulator expr_10F = this.m_SelectionScaleLeft;
				expr_10F.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_10F.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartScale(RectangleTool.ToolCoord.Right, RectangleTool.ToolCoord.Left, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), base.rippleTimeClutch);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_136 = this.m_SelectionScaleLeft;
				expr_136.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_136.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnScaleTime(base.PixelToTime(evt.mousePosition.x, this.frameRate));
					return true;
				}));
				AreaManipulator expr_15D = this.m_SelectionScaleLeft;
				expr_15D.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_15D.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnEndScale();
					return true;
				}));
			}
			if (this.m_SelectionScaleRight == null)
			{
				this.m_SelectionScaleRight = new AreaManipulator(base.styles.dopesheetScaleRight, MouseCursor.ResizeHorizontal);
				AreaManipulator expr_1A8 = this.m_SelectionScaleRight;
				expr_1A8.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_1A8.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					bool result;
					if (this.hasSelection && manipulator.rect.Contains(evt.mousePosition))
					{
						this.OnStartScale(RectangleTool.ToolCoord.Left, RectangleTool.ToolCoord.Right, new Vector2(base.PixelToTime(evt.mousePosition.x, this.frameRate), 0f), base.rippleTimeClutch);
						result = true;
					}
					else
					{
						result = false;
					}
					return result;
				}));
				AreaManipulator expr_1CF = this.m_SelectionScaleRight;
				expr_1CF.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_1CF.onDrag, new AnimationWindowManipulator.OnDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
				{
					this.OnScaleTime(base.PixelToTime(evt.mousePosition.x, this.frameRate));
					return true;
				}));
				AreaManipulator expr_1F6 = this.m_SelectionScaleRight;
				expr_1F6.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_1F6.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate(delegate(AnimationWindowManipulator manipulator, Event evt)
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
					this.m_Layout = this.CalculateLayout();
					this.m_SelectionBoxes[0].OnGUI(this.m_Layout.summaryRect);
					this.m_SelectionBoxes[1].OnGUI(this.m_Layout.selectionRect);
					this.m_SelectionScaleLeft.OnGUI(this.m_Layout.scaleLeftRect);
					this.m_SelectionScaleRight.OnGUI(this.m_Layout.scaleRightRect);
					this.DrawLabels();
				}
			}
		}

		public void HandleEvents()
		{
			base.HandleClutchKeys();
			this.m_SelectionScaleLeft.HandleEvents();
			this.m_SelectionScaleRight.HandleEvents();
			this.m_SelectionBoxes[0].HandleEvents();
			this.m_SelectionBoxes[1].HandleEvents();
		}

		private DopeSheetEditorRectangleTool.ToolLayout CalculateLayout()
		{
			DopeSheetEditorRectangleTool.ToolLayout result = default(DopeSheetEditorRectangleTool.ToolLayout);
			Bounds selectionBounds = this.selectionBounds;
			bool flag = !Mathf.Approximately(selectionBounds.size.x, 0f);
			float num = base.TimeToPixel(selectionBounds.min.x);
			float num2 = base.TimeToPixel(selectionBounds.max.x);
			float num3 = 0f;
			float num4 = 0f;
			bool flag2 = true;
			float num5 = 0f;
			List<DopeLine> dopelines = this.m_State.dopelines;
			for (int i = 0; i < dopelines.Count; i++)
			{
				DopeLine dopeLine = dopelines[i];
				float num6 = (!dopeLine.tallMode) ? 16f : 32f;
				if (!dopeLine.isMasterDopeline)
				{
					int count = dopeLine.keys.Count;
					for (int j = 0; j < count; j++)
					{
						AnimationWindowKeyframe keyframe = dopeLine.keys[j];
						if (this.m_State.KeyIsSelected(keyframe))
						{
							if (flag2)
							{
								num3 = num5;
								flag2 = false;
							}
							num4 = num5 + num6;
							break;
						}
					}
				}
				num5 += num6;
			}
			result.summaryRect = new Rect(num, 0f, num2 - num, 16f);
			result.selectionRect = new Rect(num, num3, num2 - num, num4 - num3);
			if (flag)
			{
				result.scaleLeftRect = new Rect(result.selectionRect.xMin - 17f, result.selectionRect.yMin + 4f, 17f, result.selectionRect.height - 8f);
				result.scaleRightRect = new Rect(result.selectionRect.xMax, result.selectionRect.yMin + 4f, 17f, result.selectionRect.height - 8f);
			}
			else
			{
				result.scaleLeftRect = DopeSheetEditorRectangleTool.g_EmptyRect;
				result.scaleRightRect = DopeSheetEditorRectangleTool.g_EmptyRect;
			}
			if (flag)
			{
				result.leftLabelAnchor = new Vector2(result.summaryRect.xMin - 8f, base.contentRect.yMin + 1f);
				result.rightLabelAnchor = new Vector2(result.summaryRect.xMax + 8f, base.contentRect.yMin + 1f);
			}
			else
			{
				result.leftLabelAnchor = (result.rightLabelAnchor = new Vector2(result.summaryRect.center.x + 8f, base.contentRect.yMin + 1f));
			}
			return result;
		}

		private void DrawLabels()
		{
			if (this.isDragging)
			{
				bool flag = !Mathf.Approximately(this.selectionBounds.size.x, 0f);
				if (flag)
				{
					GUIContent content = new GUIContent(string.Format("{0}", this.m_DopeSheetEditor.FormatTime(this.selectionBounds.min.x, this.m_State.frameRate, this.m_State.timeFormat)));
					GUIContent content2 = new GUIContent(string.Format("{0}", this.m_DopeSheetEditor.FormatTime(this.selectionBounds.max.x, this.m_State.frameRate, this.m_State.timeFormat)));
					Vector2 vector = base.styles.dragLabel.CalcSize(content);
					Vector2 vector2 = base.styles.dragLabel.CalcSize(content2);
					EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.leftLabelAnchor.x - vector.x, this.m_Layout.leftLabelAnchor.y, vector.x, vector.y), content, base.styles.dragLabel, 0.3f);
					EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.rightLabelAnchor.x, this.m_Layout.rightLabelAnchor.y, vector2.x, vector2.y), content2, base.styles.dragLabel, 0.3f);
				}
				else
				{
					GUIContent content3 = new GUIContent(string.Format("{0}", this.m_DopeSheetEditor.FormatTime(this.selectionBounds.center.x, this.m_State.frameRate, this.m_State.timeFormat)));
					Vector2 vector3 = base.styles.dragLabel.CalcSize(content3);
					EditorGUI.DoDropShadowLabel(new Rect(this.m_Layout.leftLabelAnchor.x, this.m_Layout.leftLabelAnchor.y, vector3.x, vector3.y), content3, base.styles.dragLabel, 0.3f);
				}
			}
		}

		private void OnStartScale(RectangleTool.ToolCoord pivotCoord, RectangleTool.ToolCoord pickedCoord, Vector2 mousePos, bool rippleTime)
		{
			Bounds selectionBounds = this.selectionBounds;
			this.m_IsDragging = true;
			this.m_Pivot = base.ToolCoordToPosition(pivotCoord, selectionBounds);
			this.m_Previous = base.ToolCoordToPosition(pickedCoord, selectionBounds);
			this.m_MouseOffset = mousePos - this.m_Previous;
			this.m_RippleTime = rippleTime;
			this.m_RippleTimeStart = selectionBounds.min.x;
			this.m_RippleTimeEnd = selectionBounds.max.x;
			this.m_State.StartLiveEdit();
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
			this.m_State.EndLiveEdit();
			this.m_IsDragging = false;
		}

		private void OnStartMove(Vector2 position, bool rippleTime)
		{
			Bounds selectionBounds = this.selectionBounds;
			this.m_IsDragging = true;
			this.m_Previous = position;
			this.m_RippleTime = rippleTime;
			this.m_RippleTimeStart = selectionBounds.min.x;
			this.m_RippleTimeEnd = selectionBounds.max.x;
			this.m_State.StartLiveEdit();
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
			this.m_State.EndLiveEdit();
			this.m_IsDragging = false;
		}

		private void TransformKeys(Matrix4x4 matrix, bool flipX, bool flipY)
		{
			if (this.m_RippleTime)
			{
				this.m_State.TransformRippleKeys(matrix, this.m_RippleTimeStart, this.m_RippleTimeEnd, flipX, true);
			}
			else
			{
				this.m_State.TransformSelectedKeys(matrix, flipX, flipY, true);
			}
		}
	}
}
