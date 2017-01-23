using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class SplitView : View, ICleanuppable, IDropArea
	{
		[Flags]
		internal enum ViewEdge
		{
			None = 0,
			Left = 1,
			Bottom = 2,
			Top = 4,
			Right = 8,
			BottomLeft = 3,
			BottomRight = 10,
			TopLeft = 5,
			TopRight = 12,
			FitsVertical = 6,
			FitsHorizontal = 9,
			Before = 5,
			After = 10
		}

		internal class ExtraDropInfo
		{
			public bool rootWindow;

			public SplitView.ViewEdge edge;

			public int index;

			public ExtraDropInfo(bool rootWindow, SplitView.ViewEdge edge, int index)
			{
				this.rootWindow = rootWindow;
				this.edge = edge;
				this.index = index;
			}
		}

		private const float kRootDropZoneThickness = 70f;

		private const float kRootDropZoneOffset = 50f;

		private const float kRootDropDestinationThickness = 200f;

		private const float kMaxViewDropZoneThickness = 300f;

		private const float kMinViewDropDestinationThickness = 100f;

		public bool vertical = false;

		public int controlID = 0;

		private SplitterState splitState = null;

		private static float[] s_StartDragPos;

		private static float[] s_DragPos;

		internal const float kGrabDist = 5f;

		private Rect RectFromEdge(Rect rect, SplitView.ViewEdge edge, float thickness, float offset)
		{
			switch (edge)
			{
			case SplitView.ViewEdge.Left:
			{
				Rect result = new Rect(rect.x - offset, rect.y, thickness, rect.height);
				return result;
			}
			case SplitView.ViewEdge.Bottom:
			{
				Rect result = new Rect(rect.x, rect.yMax - thickness + offset, rect.width, thickness);
				return result;
			}
			case SplitView.ViewEdge.Top:
			{
				Rect result = new Rect(rect.x, rect.y - offset, rect.width, thickness);
				return result;
			}
			case SplitView.ViewEdge.Right:
			{
				Rect result = new Rect(rect.xMax - thickness + offset, rect.y, thickness, rect.height);
				return result;
			}
			}
			throw new ArgumentException("Specify exactly one edge");
		}

		private void SetupSplitter()
		{
			int[] array = new int[base.children.Length];
			int[] array2 = new int[base.children.Length];
			for (int i = 0; i < base.children.Length; i++)
			{
				View view = base.children[i];
				array[i] = ((!this.vertical) ? ((int)view.position.width) : ((int)view.position.height));
				array2[i] = (int)((!this.vertical) ? view.minSize.x : view.minSize.y);
			}
			this.splitState = new SplitterState(array, array2, null);
			this.splitState.splitSize = 10;
		}

		private void SetupRectsFromSplitter()
		{
			if (base.children.Length != 0)
			{
				int num = 0;
				int num2 = 0;
				int[] realSizes = this.splitState.realSizes;
				for (int i = 0; i < realSizes.Length; i++)
				{
					int num3 = realSizes[i];
					num2 += num3;
				}
				float num4 = 1f;
				if ((float)num2 > ((!this.vertical) ? base.position.width : base.position.height))
				{
					num4 = ((!this.vertical) ? base.position.width : base.position.height) / (float)num2;
				}
				SavedGUIState savedGUIState = SavedGUIState.Create();
				for (int j = 0; j < base.children.Length; j++)
				{
					int num5 = (int)Mathf.Round((float)this.splitState.realSizes[j] * num4);
					if (this.vertical)
					{
						base.children[j].position = new Rect(0f, (float)num, base.position.width, (float)num5);
					}
					else
					{
						base.children[j].position = new Rect((float)num, 0f, (float)num5, base.position.height);
					}
					num += num5;
				}
				savedGUIState.ApplyAndForget();
			}
		}

		private static void RecalcMinMaxAndReflowAll(SplitView start)
		{
			SplitView splitView = start;
			SplitView splitView2;
			do
			{
				splitView2 = splitView;
				splitView = (splitView2.parent as SplitView);
			}
			while (splitView);
			SplitView.RecalcMinMaxRecurse(splitView2);
			SplitView.ReflowRecurse(splitView2);
		}

		private static void RecalcMinMaxRecurse(SplitView node)
		{
			View[] children = node.children;
			for (int i = 0; i < children.Length; i++)
			{
				View view = children[i];
				SplitView splitView = view as SplitView;
				if (splitView)
				{
					SplitView.RecalcMinMaxRecurse(splitView);
				}
			}
			node.ChildrenMinMaxChanged();
		}

		private static void ReflowRecurse(SplitView node)
		{
			node.Reflow();
			View[] children = node.children;
			for (int i = 0; i < children.Length; i++)
			{
				View view = children[i];
				SplitView splitView = view as SplitView;
				if (splitView)
				{
					SplitView.RecalcMinMaxRecurse(splitView);
				}
			}
		}

		internal override void Reflow()
		{
			this.SetupSplitter();
			for (int i = 0; i < base.children.Length - 1; i++)
			{
				this.splitState.DoSplitter(i, i + 1, 0);
			}
			this.splitState.RelativeToRealSizes((!this.vertical) ? ((int)base.position.width) : ((int)base.position.height));
			this.SetupRectsFromSplitter();
		}

		private void PlaceView(int i, float pos, float size)
		{
			float num = Mathf.Round(pos);
			if (this.vertical)
			{
				base.children[i].position = new Rect(0f, num, base.position.width, Mathf.Round(pos + size) - num);
			}
			else
			{
				base.children[i].position = new Rect(num, 0f, Mathf.Round(pos + size) - num, base.position.height);
			}
		}

		public override void AddChild(View child, int idx)
		{
			base.AddChild(child, idx);
			this.ChildrenMinMaxChanged();
			this.splitState = null;
		}

		public void RemoveChildNice(View child)
		{
			if (base.children.Length != 1)
			{
				int num = base.IndexOfChild(child);
				float num2;
				if (num == 0)
				{
					num2 = 0f;
				}
				else if (num == base.children.Length - 1)
				{
					num2 = 1f;
				}
				else
				{
					num2 = 0.5f;
				}
				num2 = ((!this.vertical) ? Mathf.Lerp(child.position.xMin, child.position.xMax, num2) : Mathf.Lerp(child.position.yMin, child.position.yMax, num2));
				if (num > 0)
				{
					View view = base.children[num - 1];
					Rect position = view.position;
					if (this.vertical)
					{
						position.yMax = num2;
					}
					else
					{
						position.xMax = num2;
					}
					view.position = position;
					if (view is SplitView)
					{
						((SplitView)view).Reflow();
					}
				}
				if (num < base.children.Length - 1)
				{
					View view2 = base.children[num + 1];
					Rect position2 = view2.position;
					if (this.vertical)
					{
						view2.position = new Rect(position2.x, num2, position2.width, position2.yMax - num2);
					}
					else
					{
						view2.position = new Rect(num2, position2.y, position2.xMax - num2, position2.height);
					}
					if (view2 is SplitView)
					{
						((SplitView)view2).Reflow();
					}
				}
			}
			this.RemoveChild(child);
		}

		public override void RemoveChild(View child)
		{
			this.splitState = null;
			base.RemoveChild(child);
		}

		private DropInfo RootViewDropZone(SplitView.ViewEdge edge, Vector2 mousePos, Rect screenRect)
		{
			float offset = ((edge & SplitView.ViewEdge.FitsVertical) == SplitView.ViewEdge.None) ? 50f : 70f;
			DropInfo result;
			if (!this.RectFromEdge(screenRect, edge, 70f, offset).Contains(mousePos))
			{
				result = null;
			}
			else
			{
				result = new DropInfo(this)
				{
					type = DropInfo.Type.Pane,
					userData = new SplitView.ExtraDropInfo(true, edge, 0),
					rect = this.RectFromEdge(screenRect, edge, 200f, 0f)
				};
			}
			return result;
		}

		public DropInfo DragOverRootView(Vector2 mouseScreenPosition)
		{
			DropInfo result;
			if (base.children.Length == 1 && DockArea.s_IgnoreDockingForView == base.children[0])
			{
				result = null;
			}
			else
			{
				DropInfo arg_7B_0;
				if ((arg_7B_0 = this.RootViewDropZone(SplitView.ViewEdge.Bottom, mouseScreenPosition, base.screenPosition)) == null && (arg_7B_0 = this.RootViewDropZone(SplitView.ViewEdge.Top, mouseScreenPosition, base.screenPosition)) == null)
				{
					arg_7B_0 = (this.RootViewDropZone(SplitView.ViewEdge.Left, mouseScreenPosition, base.screenPosition) ?? this.RootViewDropZone(SplitView.ViewEdge.Right, mouseScreenPosition, base.screenPosition));
				}
				result = arg_7B_0;
			}
			return result;
		}

		public DropInfo DragOver(EditorWindow w, Vector2 mouseScreenPosition)
		{
			DropInfo result;
			for (int i = 0; i < base.children.Length; i++)
			{
				View view = base.children[i];
				if (!(view == DockArea.s_IgnoreDockingForView))
				{
					if (!(view is SplitView))
					{
						SplitView.ViewEdge viewEdge = SplitView.ViewEdge.None;
						Rect screenPosition = view.screenPosition;
						Rect rect = this.RectFromEdge(screenPosition, SplitView.ViewEdge.Bottom, screenPosition.height - 39f, 0f);
						float num = Mathf.Min(Mathf.Round(rect.width / 3f), 300f);
						float num2 = Mathf.Min(Mathf.Round(rect.height / 3f), 300f);
						Rect rect2 = this.RectFromEdge(rect, SplitView.ViewEdge.Left, num, 0f);
						Rect rect3 = this.RectFromEdge(rect, SplitView.ViewEdge.Right, num, 0f);
						Rect rect4 = this.RectFromEdge(rect, SplitView.ViewEdge.Bottom, num2, 0f);
						Rect rect5 = this.RectFromEdge(rect, SplitView.ViewEdge.Top, num2, 0f);
						if (rect2.Contains(mouseScreenPosition))
						{
							viewEdge |= SplitView.ViewEdge.Left;
						}
						if (rect3.Contains(mouseScreenPosition))
						{
							viewEdge |= SplitView.ViewEdge.Right;
						}
						if (rect4.Contains(mouseScreenPosition))
						{
							viewEdge |= SplitView.ViewEdge.Bottom;
						}
						if (rect5.Contains(mouseScreenPosition))
						{
							viewEdge |= SplitView.ViewEdge.Top;
						}
						Vector2 vector = Vector2.zero;
						Vector2 zero = Vector2.zero;
						SplitView.ViewEdge viewEdge2 = viewEdge;
						SplitView.ViewEdge viewEdge3 = viewEdge;
						switch (viewEdge)
						{
						case SplitView.ViewEdge.BottomLeft:
							viewEdge2 = SplitView.ViewEdge.Bottom;
							viewEdge3 = SplitView.ViewEdge.Left;
							vector = new Vector2(rect.x, rect.yMax) - mouseScreenPosition;
							zero = new Vector2(-num, num2);
							goto IL_22E;
						case SplitView.ViewEdge.Top:
							IL_14C:
							switch (viewEdge)
							{
							case SplitView.ViewEdge.BottomRight:
								viewEdge2 = SplitView.ViewEdge.Right;
								viewEdge3 = SplitView.ViewEdge.Bottom;
								vector = new Vector2(rect.xMax, rect.yMax) - mouseScreenPosition;
								zero = new Vector2(num, num2);
								goto IL_22E;
							case SplitView.ViewEdge.Left | SplitView.ViewEdge.Bottom | SplitView.ViewEdge.Right:
								goto IL_22E;
							case SplitView.ViewEdge.TopRight:
								viewEdge2 = SplitView.ViewEdge.Top;
								viewEdge3 = SplitView.ViewEdge.Right;
								vector = new Vector2(rect.xMax, rect.y) - mouseScreenPosition;
								zero = new Vector2(num, -num2);
								goto IL_22E;
							default:
								goto IL_22E;
							}
							break;
						case SplitView.ViewEdge.TopLeft:
							viewEdge2 = SplitView.ViewEdge.Left;
							viewEdge3 = SplitView.ViewEdge.Top;
							vector = new Vector2(rect.x, rect.y) - mouseScreenPosition;
							zero = new Vector2(-num, -num2);
							goto IL_22E;
						}
						goto IL_14C;
						IL_22E:
						viewEdge = ((vector.x * zero.y - vector.y * zero.x >= 0f) ? viewEdge3 : viewEdge2);
						if (viewEdge != SplitView.ViewEdge.None)
						{
							float num3 = Mathf.Round((((viewEdge & SplitView.ViewEdge.FitsHorizontal) == SplitView.ViewEdge.None) ? screenPosition.height : screenPosition.width) / 3f);
							num3 = Mathf.Max(num3, 100f);
							result = new DropInfo(this)
							{
								userData = new SplitView.ExtraDropInfo(false, viewEdge, i),
								type = DropInfo.Type.Pane,
								rect = this.RectFromEdge(screenPosition, viewEdge, num3, 0f)
							};
							return result;
						}
					}
				}
			}
			if (base.screenPosition.Contains(mouseScreenPosition) && !(base.parent is SplitView))
			{
				result = new DropInfo(null);
				return result;
			}
			result = null;
			return result;
		}

		protected override void ChildrenMinMaxChanged()
		{
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			if (this.vertical)
			{
				View[] children = base.children;
				for (int i = 0; i < children.Length; i++)
				{
					View view = children[i];
					zero.x = Mathf.Max(view.minSize.x, zero.x);
					zero2.x = Mathf.Max(view.maxSize.x, zero2.x);
					zero.y += view.minSize.y;
					zero2.y += view.maxSize.y;
				}
			}
			else
			{
				View[] children2 = base.children;
				for (int j = 0; j < children2.Length; j++)
				{
					View view2 = children2[j];
					zero.x += view2.minSize.x;
					zero2.x += view2.maxSize.x;
					zero.y = Mathf.Max(view2.minSize.y, zero.y);
					zero2.y = Mathf.Max(view2.maxSize.y, zero2.y);
				}
			}
			this.splitState = null;
			base.SetMinMaxSizes(zero, zero2);
		}

		public override string ToString()
		{
			return (!this.vertical) ? "SplitView (horiz)" : "SplitView (vert)";
		}

		public bool PerformDrop(EditorWindow dropWindow, DropInfo dropInfo, Vector2 screenPos)
		{
			SplitView.ExtraDropInfo extraDropInfo = dropInfo.userData as SplitView.ExtraDropInfo;
			bool rootWindow = extraDropInfo.rootWindow;
			SplitView.ViewEdge edge = extraDropInfo.edge;
			int num = extraDropInfo.index;
			Rect rect = dropInfo.rect;
			bool flag = (edge & SplitView.ViewEdge.TopLeft) != SplitView.ViewEdge.None;
			bool flag2 = (edge & SplitView.ViewEdge.FitsVertical) != SplitView.ViewEdge.None;
			SplitView splitView;
			if (this.vertical == flag2 || base.children.Length < 2)
			{
				if (!flag)
				{
					if (rootWindow)
					{
						num = base.children.Length;
					}
					else
					{
						num++;
					}
				}
				splitView = this;
			}
			else if (rootWindow)
			{
				SplitView splitView2 = ScriptableObject.CreateInstance<SplitView>();
				splitView2.position = base.position;
				if (base.window.rootView == this)
				{
					base.window.rootView = splitView2;
				}
				else
				{
					base.parent.AddChild(splitView2, base.parent.IndexOfChild(this));
				}
				splitView2.AddChild(this);
				base.position = new Rect(Vector2.zero, base.position.size);
				num = ((!flag) ? 1 : 0);
				splitView = splitView2;
			}
			else
			{
				SplitView splitView3 = ScriptableObject.CreateInstance<SplitView>();
				splitView3.AddChild(base.children[num]);
				this.AddChild(splitView3, num);
				splitView3.position = splitView3.children[0].position;
				splitView3.children[0].position = new Rect(Vector2.zero, splitView3.position.size);
				num = ((!flag) ? 1 : 0);
				splitView = splitView3;
			}
			rect.position -= base.screenPosition.position;
			DockArea dockArea = ScriptableObject.CreateInstance<DockArea>();
			splitView.vertical = flag2;
			splitView.MakeRoomForRect(rect);
			splitView.AddChild(dockArea, num);
			dockArea.position = rect;
			DockArea.s_OriginalDragSource.RemoveTab(dropWindow);
			dropWindow.m_Parent = dockArea;
			dockArea.AddTab(dropWindow);
			this.Reflow();
			SplitView.RecalcMinMaxAndReflowAll(this);
			dockArea.MakeVistaDWMHappyDance();
			return true;
		}

		private static string PosVals(float[] posVals)
		{
			string text = "[";
			for (int i = 0; i < posVals.Length; i++)
			{
				float num = posVals[i];
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"",
					num,
					", "
				});
			}
			return text + "]";
		}

		private void MakeRoomForRect(Rect r)
		{
			Rect[] array = new Rect[base.children.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = base.children[i].position;
			}
			this.CalcRoomForRect(array, r);
			for (int j = 0; j < array.Length; j++)
			{
				base.children[j].position = array[j];
			}
		}

		private void CalcRoomForRect(Rect[] sources, Rect r)
		{
			float num = (!this.vertical) ? r.x : r.y;
			float num2 = num + ((!this.vertical) ? r.width : r.height);
			float num3 = (num + num2) * 0.5f;
			int i;
			for (i = 0; i < sources.Length; i++)
			{
				float num4 = (!this.vertical) ? (sources[i].x + sources[i].width * 0.5f) : (sources[i].y + sources[i].height * 0.5f);
				if (num4 > num3)
				{
					break;
				}
			}
			float num5 = num;
			for (int j = i - 1; j >= 0; j--)
			{
				if (this.vertical)
				{
					sources[j].yMax = num5;
					if (sources[j].height >= base.children[j].minSize.y)
					{
						break;
					}
					float num6 = sources[j].yMax - base.children[j].minSize.y;
					sources[j].yMin = num6;
					num5 = num6;
				}
				else
				{
					sources[j].xMax = num5;
					if (sources[j].width >= base.children[j].minSize.x)
					{
						break;
					}
					float num6 = sources[j].xMax - base.children[j].minSize.x;
					sources[j].xMin = num6;
					num5 = num6;
				}
			}
			if (num5 < 0f)
			{
				float num7 = -num5;
				for (int k = 0; k < i - 1; k++)
				{
					if (this.vertical)
					{
						int expr_215_cp_1 = k;
						sources[expr_215_cp_1].y = sources[expr_215_cp_1].y + num7;
					}
					else
					{
						int expr_230_cp_1 = k;
						sources[expr_230_cp_1].x = sources[expr_230_cp_1].x + num7;
					}
				}
				num2 += num7;
			}
			num5 = num2;
			for (int l = i; l < sources.Length; l++)
			{
				if (this.vertical)
				{
					float yMax = sources[l].yMax;
					sources[l].yMin = num5;
					sources[l].yMax = yMax;
					if (sources[l].height >= base.children[l].minSize.y)
					{
						break;
					}
					float num6 = sources[l].yMin + base.children[l].minSize.y;
					sources[l].yMax = num6;
					num5 = num6;
				}
				else
				{
					float xMax = sources[l].xMax;
					sources[l].xMin = num5;
					sources[l].xMax = xMax;
					if (sources[l].width >= base.children[l].minSize.x)
					{
						break;
					}
					float num6 = sources[l].xMin + base.children[l].minSize.x;
					sources[l].xMax = num6;
					num5 = num6;
				}
			}
			float num8 = (!this.vertical) ? base.position.width : base.position.height;
			if (num5 > num8)
			{
				float num9 = num8 - num5;
				for (int m = 0; m < i - 1; m++)
				{
					if (this.vertical)
					{
						int expr_417_cp_1 = m;
						sources[expr_417_cp_1].y = sources[expr_417_cp_1].y + num9;
					}
					else
					{
						int expr_432_cp_1 = m;
						sources[expr_432_cp_1].x = sources[expr_432_cp_1].x + num9;
					}
				}
				num2 += num9;
			}
		}

		public void Cleanup()
		{
			SplitView splitView = base.parent as SplitView;
			if (base.children.Length == 1 && splitView != null)
			{
				View view = base.children[0];
				view.position = base.position;
				if (base.parent != null)
				{
					base.parent.AddChild(view, base.parent.IndexOfChild(this));
					base.parent.RemoveChild(this);
					if (splitView)
					{
						splitView.Cleanup();
					}
					view.position = base.position;
					if (!Unsupported.IsDestroyScriptableObject(this))
					{
						UnityEngine.Object.DestroyImmediate(this);
					}
					return;
				}
				if (view is SplitView)
				{
					this.RemoveChild(view);
					base.window.rootView = view;
					view.position = new Rect(0f, 0f, view.window.position.width, base.window.position.height);
					view.Reflow();
					if (!Unsupported.IsDestroyScriptableObject(this))
					{
						UnityEngine.Object.DestroyImmediate(this);
					}
					return;
				}
			}
			if (splitView)
			{
				splitView.Cleanup();
				splitView = (base.parent as SplitView);
				if (splitView)
				{
					if (splitView.vertical == this.vertical)
					{
						int num = new List<View>(base.parent.children).IndexOf(this);
						View[] children = base.children;
						for (int i = 0; i < children.Length; i++)
						{
							View view2 = children[i];
							splitView.AddChild(view2, num++);
							view2.position = new Rect(base.position.x + view2.position.x, base.position.y + view2.position.y, view2.position.width, view2.position.height);
						}
					}
				}
			}
			if (base.children.Length == 0)
			{
				if (base.parent == null && base.window != null)
				{
					base.window.Close();
				}
				else
				{
					ICleanuppable cleanuppable = base.parent as ICleanuppable;
					if (base.parent is SplitView)
					{
						((SplitView)base.parent).RemoveChildNice(this);
						if (!Unsupported.IsDestroyScriptableObject(this))
						{
							UnityEngine.Object.DestroyImmediate(this, true);
						}
					}
					cleanuppable.Cleanup();
				}
			}
			else
			{
				this.splitState = null;
				this.Reflow();
			}
		}

		public void SplitGUI(Event evt)
		{
			if (this.splitState == null)
			{
				this.SetupSplitter();
			}
			SplitView splitView = base.parent as SplitView;
			if (splitView)
			{
				Event @event = new Event(evt);
				@event.mousePosition += new Vector2(base.position.x, base.position.y);
				splitView.SplitGUI(@event);
				if (@event.type == EventType.Used)
				{
					evt.Use();
				}
			}
			float num = (!this.vertical) ? evt.mousePosition.x : evt.mousePosition.y;
			int num2 = GUIUtility.GetControlID(546739, FocusType.Passive);
			this.controlID = num2;
			EventType typeForControl = evt.GetTypeForControl(num2);
			if (typeForControl != EventType.MouseDown)
			{
				if (typeForControl != EventType.MouseDrag)
				{
					if (typeForControl == EventType.MouseUp)
					{
						if (GUIUtility.hotControl == num2)
						{
							GUIUtility.hotControl = 0;
						}
					}
				}
				else if (base.children.Length > 1 && GUIUtility.hotControl == num2 && this.splitState.currentActiveSplitter >= 0)
				{
					int num3 = (int)num - this.splitState.splitterInitialOffset;
					if (num3 != 0)
					{
						this.splitState.splitterInitialOffset = (int)num;
						this.splitState.DoSplitter(this.splitState.currentActiveSplitter, this.splitState.currentActiveSplitter + 1, num3);
					}
					this.SetupRectsFromSplitter();
					evt.Use();
				}
			}
			else if (base.children.Length != 1)
			{
				int num4 = (!this.vertical) ? ((int)base.children[0].position.x) : ((int)base.children[0].position.y);
				for (int i = 0; i < base.children.Length - 1; i++)
				{
					if (i >= this.splitState.realSizes.Length)
					{
						DockArea dockArea = GUIView.current as DockArea;
						string text = "Non-dock area " + GUIView.current.GetType();
						if (dockArea && dockArea.m_Selected < dockArea.m_Panes.Count && dockArea.m_Panes[dockArea.m_Selected])
						{
							text = dockArea.m_Panes[dockArea.m_Selected].GetType().ToString();
						}
						if (Unsupported.IsDeveloperBuild())
						{
							Debug.LogError(string.Concat(new object[]
							{
								"Real sizes out of bounds for: ",
								text,
								" index: ",
								i,
								" RealSizes: ",
								this.splitState.realSizes.Length
							}));
						}
						this.SetupSplitter();
					}
					if (((!this.vertical) ? new Rect((float)(num4 + this.splitState.realSizes[i] - this.splitState.splitSize / 2), base.children[0].position.y, (float)this.splitState.splitSize, base.children[0].position.height) : new Rect(base.children[0].position.x, (float)(num4 + this.splitState.realSizes[i] - this.splitState.splitSize / 2), base.children[0].position.width, (float)this.splitState.splitSize)).Contains(evt.mousePosition))
					{
						this.splitState.splitterInitialOffset = (int)num;
						this.splitState.currentActiveSplitter = i;
						GUIUtility.hotControl = num2;
						evt.Use();
						break;
					}
					num4 += this.splitState.realSizes[i];
				}
			}
		}

		protected override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			this.Reflow();
		}
	}
}
