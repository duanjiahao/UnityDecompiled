using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class SplitView : View, ICleanuppable, IDropArea
	{
		private class ExtraDropInfo
		{
			public Rect dropRect;

			public int idx;

			public ExtraDropInfo(Rect _dropRect, int _idx)
			{
				this.dropRect = _dropRect;
				this.idx = _idx;
			}
		}

		internal const float kGrabDist = 5f;

		public bool vertical;

		public int controlID;

		private SplitterState splitState;

		private static float[] s_StartDragPos;

		private static float[] s_DragPos;

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
			if (base.children.Length == 0)
			{
				return;
			}
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

		private DropInfo DoDropZone(int idx, Vector2 mousePos, Rect sourceRect, Rect previewRect)
		{
			if (!sourceRect.Contains(mousePos))
			{
				return null;
			}
			return new DropInfo(this)
			{
				type = DropInfo.Type.Pane,
				userData = idx,
				rect = previewRect
			};
		}

		private DropInfo CheckRootWindowDropZones(Vector2 mouseScreenPosition)
		{
			DropInfo dropInfo = null;
			if (!(base.parent is SplitView) && (base.children.Length != 1 || !(DockArea.s_IgnoreDockingForView == base.children[0])))
			{
				Rect screenPosition = base.screenPosition;
				if (base.parent is MainWindow)
				{
					dropInfo = this.DoDropZone(-1, mouseScreenPosition, new Rect(screenPosition.x, screenPosition.yMax, screenPosition.width, 100f), new Rect(screenPosition.x, screenPosition.yMax - 200f, screenPosition.width, 200f));
				}
				else
				{
					dropInfo = this.DoDropZone(-1, mouseScreenPosition, new Rect(screenPosition.x, screenPosition.yMax - 20f, screenPosition.width, 100f), new Rect(screenPosition.x, screenPosition.yMax - 50f, screenPosition.width, 200f));
				}
				if (dropInfo != null)
				{
					return dropInfo;
				}
				dropInfo = this.DoDropZone(-2, mouseScreenPosition, new Rect(screenPosition.x - 30f, screenPosition.y, 50f, screenPosition.height), new Rect(screenPosition.x - 50f, screenPosition.y, 100f, screenPosition.height));
				if (dropInfo != null)
				{
					return dropInfo;
				}
				dropInfo = this.DoDropZone(-3, mouseScreenPosition, new Rect(screenPosition.xMax - 20f, screenPosition.y, 50f, screenPosition.height), new Rect(screenPosition.xMax - 50f, screenPosition.y, 100f, screenPosition.height));
			}
			return dropInfo;
		}

		public DropInfo DragOver(EditorWindow w, Vector2 mouseScreenPosition)
		{
			DropInfo dropInfo = this.CheckRootWindowDropZones(mouseScreenPosition);
			if (dropInfo != null)
			{
				return dropInfo;
			}
			for (int i = 0; i < base.children.Length; i++)
			{
				View view = base.children[i];
				if (!(view == DockArea.s_IgnoreDockingForView))
				{
					if (!(view is SplitView))
					{
						Rect screenPosition = view.screenPosition;
						int num = 0;
						float num2 = Mathf.Round(Mathf.Min(screenPosition.width / 3f, 300f));
						float num3 = Mathf.Round(Mathf.Min(screenPosition.height / 3f, 300f));
						Rect rect = new Rect(screenPosition.x, screenPosition.y + 39f, num2, screenPosition.height - 39f);
						if (rect.Contains(mouseScreenPosition))
						{
							num |= 1;
						}
						Rect rect2 = new Rect(screenPosition.x, screenPosition.yMax - num3, screenPosition.width, num3);
						if (rect2.Contains(mouseScreenPosition))
						{
							num |= 2;
						}
						Rect rect3 = new Rect(screenPosition.xMax - num2, screenPosition.y + 39f, num2, screenPosition.height - 39f);
						if (rect3.Contains(mouseScreenPosition))
						{
							num |= 4;
						}
						if (num == 3)
						{
							Vector2 vector = new Vector2(screenPosition.x, screenPosition.yMax) - mouseScreenPosition;
							Vector2 vector2 = new Vector2(num2, -num3);
							if (vector.x * vector2.y - vector.y * vector2.x < 0f)
							{
								num = 1;
							}
							else
							{
								num = 2;
							}
						}
						else if (num == 6)
						{
							Vector2 vector3 = new Vector2(screenPosition.xMax, screenPosition.yMax) - mouseScreenPosition;
							Vector2 vector4 = new Vector2(-num2, -num3);
							if (vector3.x * vector4.y - vector3.y * vector4.x < 0f)
							{
								num = 2;
							}
							else
							{
								num = 4;
							}
						}
						float num4 = Mathf.Round(Mathf.Max(screenPosition.width / 3f, 100f));
						float num5 = Mathf.Round(Mathf.Max(screenPosition.height / 3f, 100f));
						if (this.vertical)
						{
							switch (num)
							{
							case 1:
								return new DropInfo(this)
								{
									userData = i + 1000,
									type = DropInfo.Type.Pane,
									rect = new Rect(screenPosition.x, screenPosition.y, num4, screenPosition.height)
								};
							case 2:
								return new DropInfo(this)
								{
									userData = i + 1,
									type = DropInfo.Type.Pane,
									rect = new Rect(screenPosition.x, screenPosition.yMax - num5, screenPosition.width, num5)
								};
							case 4:
								return new DropInfo(this)
								{
									userData = i + 2000,
									type = DropInfo.Type.Pane,
									rect = new Rect(screenPosition.xMax - num4, screenPosition.y, num4, screenPosition.height)
								};
							}
						}
						else
						{
							switch (num)
							{
							case 1:
								return new DropInfo(this)
								{
									userData = i,
									type = DropInfo.Type.Pane,
									rect = new Rect(screenPosition.x, screenPosition.y, num4, screenPosition.height)
								};
							case 2:
								return new DropInfo(this)
								{
									userData = i + 2000,
									type = DropInfo.Type.Pane,
									rect = new Rect(screenPosition.x, screenPosition.yMax - num5, screenPosition.width, num5)
								};
							case 4:
								return new DropInfo(this)
								{
									userData = i + 1,
									type = DropInfo.Type.Pane,
									rect = new Rect(screenPosition.xMax - num4, screenPosition.y, num4, screenPosition.height)
								};
							}
						}
					}
				}
			}
			if (base.screenPosition.Contains(mouseScreenPosition) && !(base.parent is SplitView))
			{
				return new DropInfo(null);
			}
			return null;
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

		public bool PerformDrop(EditorWindow w, DropInfo di, Vector2 screenPos)
		{
			int num = (int)di.userData;
			DockArea dockArea = ScriptableObject.CreateInstance<DockArea>();
			Rect rect = di.rect;
			if (num == -1 || num == -2 || num == -3)
			{
				bool flag = num == -2;
				bool flag2 = num == -1;
				this.splitState = null;
				if (this.vertical == flag2 || base.children.Length < 2)
				{
					this.vertical = flag2;
					rect.x -= base.screenPosition.x;
					rect.y -= base.screenPosition.y;
					this.MakeRoomForRect(rect);
					this.AddChild(dockArea, (!flag) ? base.children.Length : 0);
					dockArea.position = rect;
				}
				else
				{
					SplitView splitView = ScriptableObject.CreateInstance<SplitView>();
					Rect position = base.position;
					splitView.vertical = flag2;
					splitView.position = new Rect(position.x, position.y, position.width, position.height);
					if (base.window.mainView == this)
					{
						base.window.mainView = splitView;
					}
					else
					{
						base.parent.AddChild(splitView, base.parent.IndexOfChild(this));
					}
					splitView.AddChild(this);
					base.position = new Rect(0f, 0f, position.width, position.height);
					Rect rect2 = rect;
					rect2.x -= base.screenPosition.x;
					rect2.y -= base.screenPosition.y;
					splitView.MakeRoomForRect(rect2);
					dockArea.position = rect2;
					splitView.AddChild(dockArea, (!flag) ? 1 : 0);
				}
			}
			else if (num < 1000)
			{
				Rect rect3 = rect;
				rect3.x -= base.screenPosition.x;
				rect3.y -= base.screenPosition.y;
				this.MakeRoomForRect(rect3);
				this.AddChild(dockArea, num);
				dockArea.position = rect3;
			}
			else
			{
				int num2 = num % 1000;
				if (base.children.Length != 1)
				{
					SplitView splitView2 = ScriptableObject.CreateInstance<SplitView>();
					splitView2.vertical = !this.vertical;
					Rect position2 = base.children[num2].position;
					splitView2.AddChild(base.children[num2]);
					this.AddChild(splitView2, num2);
					splitView2.position = position2;
					float num3 = 0f;
					position2.y = num3;
					position2.x = num3;
					splitView2.children[0].position = position2;
					Rect rect4 = rect;
					rect4.x -= splitView2.screenPosition.x;
					rect4.y -= splitView2.screenPosition.y;
					splitView2.MakeRoomForRect(rect4);
					splitView2.AddChild(dockArea, (num >= 2000) ? 1 : 0);
					dockArea.position = rect4;
				}
				else
				{
					this.vertical = !this.vertical;
					Rect rect5 = rect;
					rect5.x -= base.screenPosition.x;
					rect5.y -= base.screenPosition.y;
					this.MakeRoomForRect(rect5);
					this.AddChild(dockArea, (num != 1000) ? 1 : 0);
					dockArea.position = rect5;
				}
			}
			DockArea.s_OriginalDragSource.RemoveTab(w);
			w.m_Parent = dockArea;
			dockArea.AddTab(w);
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
					string.Empty,
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
						int expr_20A_cp_1 = k;
						sources[expr_20A_cp_1].y = sources[expr_20A_cp_1].y + num7;
					}
					else
					{
						int expr_225_cp_1 = k;
						sources[expr_225_cp_1].x = sources[expr_225_cp_1].x + num7;
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
						int expr_402_cp_1 = m;
						sources[expr_402_cp_1].y = sources[expr_402_cp_1].y + num9;
					}
					else
					{
						int expr_41D_cp_1 = m;
						sources[expr_41D_cp_1].x = sources[expr_41D_cp_1].x + num9;
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
					base.window.mainView = view;
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
				if (splitView && splitView.vertical == this.vertical)
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
				return;
			}
			this.splitState = null;
			this.Reflow();
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
			switch (evt.GetTypeForControl(num2))
			{
			case EventType.MouseDown:
				if (base.children.Length != 1)
				{
					int num3 = (!this.vertical) ? ((int)base.children[0].position.x) : ((int)base.children[0].position.y);
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
						if (((!this.vertical) ? new Rect((float)(num3 + this.splitState.realSizes[i] - this.splitState.splitSize / 2), base.children[0].position.y, (float)this.splitState.splitSize, base.children[0].position.height) : new Rect(base.children[0].position.x, (float)(num3 + this.splitState.realSizes[i] - this.splitState.splitSize / 2), base.children[0].position.width, (float)this.splitState.splitSize)).Contains(evt.mousePosition))
						{
							this.splitState.splitterInitialOffset = (int)num;
							this.splitState.currentActiveSplitter = i;
							GUIUtility.hotControl = num2;
							evt.Use();
							break;
						}
						num3 += this.splitState.realSizes[i];
					}
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == num2)
				{
					GUIUtility.hotControl = 0;
				}
				break;
			case EventType.MouseDrag:
				if (base.children.Length > 1 && GUIUtility.hotControl == num2 && this.splitState.currentActiveSplitter >= 0)
				{
					int num4 = (int)num - this.splitState.splitterInitialOffset;
					if (num4 != 0)
					{
						this.splitState.splitterInitialOffset = (int)num;
						this.splitState.DoSplitter(this.splitState.currentActiveSplitter, this.splitState.currentActiveSplitter + 1, num4);
					}
					this.SetupRectsFromSplitter();
					evt.Use();
				}
				break;
			}
		}

		protected override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			this.Reflow();
		}
	}
}
