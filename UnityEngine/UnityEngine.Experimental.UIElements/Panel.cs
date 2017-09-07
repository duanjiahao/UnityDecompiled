using System;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	internal class Panel : BaseVisualElementPanel
	{
		private StyleContext m_StyleContext;

		private VisualContainer m_RootContainer;

		private TimerEventScheduler m_Scheduler;

		private const int kMaxValidateLayoutCount = 5;

		[CompilerGenerated]
		private static LoadResourceFunction <>f__mg$cache0;

		public override VisualContainer visualTree
		{
			get
			{
				return this.m_RootContainer;
			}
		}

		public VisualContainer defaultIMRoot
		{
			get;
			set;
		}

		public override IDispatcher dispatcher
		{
			get;
			protected set;
		}

		public override IDataWatchService dataWatch
		{
			get;
			protected set;
		}

		public TimerEventScheduler timerEventScheduler
		{
			get
			{
				TimerEventScheduler arg_1C_0;
				if ((arg_1C_0 = this.m_Scheduler) == null)
				{
					arg_1C_0 = (this.m_Scheduler = new TimerEventScheduler());
				}
				return arg_1C_0;
			}
		}

		public override IScheduler scheduler
		{
			get
			{
				return this.timerEventScheduler;
			}
		}

		internal StyleContext styleContext
		{
			get
			{
				return this.m_StyleContext;
			}
		}

		public override int instanceID
		{
			get;
			protected set;
		}

		public bool allowPixelCaching
		{
			get;
			set;
		}

		public override ContextType contextType
		{
			get;
			protected set;
		}

		public override EventInterests IMGUIEventInterests
		{
			get;
			set;
		}

		public override LoadResourceFunction loadResourceFunc
		{
			get;
			protected set;
		}

		public override int IMGUIContainersCount
		{
			get;
			set;
		}

		public Panel(int instanceID, ContextType contextType, LoadResourceFunction loadResourceDelegate = null, IDataWatchService dataWatch = null, IDispatcher dispatcher = null)
		{
			this.instanceID = instanceID;
			this.contextType = contextType;
			LoadResourceFunction arg_3B_1 = loadResourceDelegate;
			if (loadResourceDelegate == null)
			{
				if (Panel.<>f__mg$cache0 == null)
				{
					Panel.<>f__mg$cache0 = new LoadResourceFunction(Resources.Load);
				}
				arg_3B_1 = Panel.<>f__mg$cache0;
			}
			this.loadResourceFunc = arg_3B_1;
			this.dataWatch = dataWatch;
			this.dispatcher = dispatcher;
			this.stylePainter = new StylePainter();
			this.m_RootContainer = new VisualContainer();
			this.m_RootContainer.name = VisualElementUtils.GetUniqueName("PanelContainer");
			this.visualTree.ChangePanel(this);
			this.m_StyleContext = new StyleContext(this.m_RootContainer);
			this.defaultIMRoot = new IMContainer
			{
				name = "DefaultOnGUI",
				pickingMode = PickingMode.Ignore
			};
			this.defaultIMRoot.StretchToParentSize();
			this.visualTree.InsertChild(0, this.defaultIMRoot);
			this.allowPixelCaching = true;
		}

		private VisualElement Pick(VisualElement root, Vector2 point)
		{
			VisualElement result;
			if ((root.pseudoStates & PseudoStates.Invisible) == PseudoStates.Invisible)
			{
				result = null;
			}
			else
			{
				VisualContainer visualContainer = root as VisualContainer;
				Vector3 vector = root.transform.inverse.MultiplyPoint3x4(point);
				bool flag = root.ContainsPoint(vector);
				if (visualContainer != null)
				{
					if (!flag && visualContainer.clipChildren)
					{
						result = null;
						return result;
					}
					vector -= new Vector3(visualContainer.position.position.x, visualContainer.position.position.y, 0f);
					for (int i = visualContainer.childrenCount - 1; i >= 0; i--)
					{
						VisualElement childAt = visualContainer.GetChildAt(i);
						VisualElement visualElement = this.Pick(childAt, vector);
						if (visualElement != null)
						{
							result = visualElement;
							return result;
						}
					}
				}
				PickingMode pickingMode = root.pickingMode;
				if (pickingMode != PickingMode.Position)
				{
					if (pickingMode != PickingMode.Ignore)
					{
					}
				}
				else if (flag)
				{
					result = root;
					return result;
				}
				result = null;
			}
			return result;
		}

		public override VisualElement Pick(Vector2 point)
		{
			this.ValidateLayout();
			return this.Pick(this.visualTree, point);
		}

		private void ValidateStyling()
		{
			if (!Mathf.Approximately(this.m_StyleContext.currentPixelsPerPoint, GUIUtility.pixelsPerPoint))
			{
				this.m_RootContainer.Dirty(ChangeType.Styles);
				this.m_StyleContext.currentPixelsPerPoint = GUIUtility.pixelsPerPoint;
			}
			if (this.m_RootContainer.IsDirty(ChangeType.Styles | ChangeType.StylesPath))
			{
				this.m_StyleContext.ApplyStyles();
			}
		}

		public override void ValidateLayout()
		{
			this.ValidateStyling();
			int num = 0;
			while (this.visualTree.cssNode.IsDirty)
			{
				this.visualTree.cssNode.CalculateLayout();
				this.ValidateSubTree(this.visualTree);
				if (num++ >= 5)
				{
					Debug.LogError("ValidateLayout is struggling to process current layout (consider simplifying to avoid recursive layout): " + this.visualTree);
					break;
				}
			}
		}

		private bool ValidateSubTree(VisualElement root)
		{
			if (root.renderData.lastLayout != new Rect(root.cssNode.LayoutX, root.cssNode.LayoutY, root.cssNode.LayoutWidth, root.cssNode.LayoutHeight))
			{
				root.Dirty(ChangeType.Transform);
				root.renderData.lastLayout = new Rect(root.cssNode.LayoutX, root.cssNode.LayoutY, root.cssNode.LayoutWidth, root.cssNode.LayoutHeight);
			}
			bool flag = root.cssNode.HasNewLayout;
			if (flag)
			{
				VisualContainer visualContainer = root as VisualContainer;
				if (visualContainer != null)
				{
					foreach (VisualElement current in visualContainer)
					{
						flag |= this.ValidateSubTree(current);
					}
				}
			}
			root.OnPostLayout(flag);
			root.ClearDirty(ChangeType.Layout);
			root.cssNode.MarkLayoutSeen();
			return flag;
		}

		private Rect ComputeAAAlignedBound(Rect position, Matrix4x4 mat)
		{
			Rect rect = position;
			Vector3 vector = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y, 0f));
			Vector3 vector2 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y, 0f));
			Vector3 vector3 = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y + rect.height, 0f));
			Vector3 vector4 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y + rect.height, 0f));
			return Rect.MinMaxRect(Mathf.Min(vector.x, Mathf.Min(vector2.x, Mathf.Min(vector3.x, vector4.x))), Mathf.Min(vector.y, Mathf.Min(vector2.y, Mathf.Min(vector3.y, vector4.y))), Mathf.Max(vector.x, Mathf.Max(vector2.x, Mathf.Max(vector3.x, vector4.x))), Mathf.Max(vector.y, Mathf.Max(vector2.y, Mathf.Max(vector3.y, vector4.y))));
		}

		public void PaintSubTree(Event e, VisualElement root, Matrix4x4 offset, Rect currentGlobalClip)
		{
			if ((root.pseudoStates & PseudoStates.Invisible) != PseudoStates.Invisible)
			{
				VisualContainer visualContainer = root as VisualContainer;
				if (visualContainer != null)
				{
					if (visualContainer.clipChildren)
					{
						Rect rect = this.ComputeAAAlignedBound(root.position, offset * root.globalTransform);
						if (!rect.Overlaps(currentGlobalClip))
						{
							return;
						}
						float num = Mathf.Max(rect.x, currentGlobalClip.x);
						float num2 = Mathf.Min(rect.x + rect.width, currentGlobalClip.x + currentGlobalClip.width);
						float num3 = Mathf.Max(rect.y, currentGlobalClip.y);
						float num4 = Mathf.Min(rect.y + rect.height, currentGlobalClip.y + currentGlobalClip.height);
						currentGlobalClip = new Rect(num, num3, num2 - num, num4 - num3);
					}
				}
				else if (!this.ComputeAAAlignedBound(root.globalBound, offset).Overlaps(currentGlobalClip))
				{
					return;
				}
				if (root.usePixelCaching && this.allowPixelCaching && root.globalBound.size.magnitude > Mathf.Epsilon)
				{
					IStylePainter stylePainter = this.stylePainter;
					Rect globalBound = root.globalBound;
					int num5 = (int)globalBound.width;
					int num6 = (int)globalBound.height;
					int num7 = (int)(globalBound.width * GUIUtility.pixelsPerPoint);
					int num8 = (int)(globalBound.height * GUIUtility.pixelsPerPoint);
					RenderTexture renderTexture = root.renderData.pixelCache;
					if (renderTexture != null && renderTexture.width != num7 && renderTexture.height != num8)
					{
						UnityEngine.Object.DestroyImmediate(renderTexture);
						root.renderData.pixelCache = null;
					}
					float opacity = this.stylePainter.opacity;
					if (root.IsDirty(ChangeType.Repaint) || root.renderData.pixelCache == null)
					{
						if (renderTexture == null)
						{
							renderTexture = (root.renderData.pixelCache = new RenderTexture(num7, num8, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear));
						}
						RenderTexture active = RenderTexture.active;
						RenderTexture.active = renderTexture;
						GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
						offset = Matrix4x4.Translate(new Vector3(-globalBound.x, -globalBound.y, 0f));
						Rect rect2 = new Rect(0f, 0f, (float)num5, (float)num6);
						GUIClip.SetTransform(offset * root.globalTransform, rect2);
						stylePainter.currentWorldClip = currentGlobalClip;
						root.DoRepaint(stylePainter);
						root.ClearDirty(ChangeType.Repaint);
						if (visualContainer != null)
						{
							int childrenCount = visualContainer.childrenCount;
							for (int i = 0; i < childrenCount; i++)
							{
								VisualElement childAt = visualContainer.GetChildAt(i);
								this.PaintSubTree(e, childAt, offset, rect2);
								if (childrenCount != visualContainer.childrenCount)
								{
									throw new NotImplementedException("Visual tree is read-only during repaint");
								}
							}
						}
						RenderTexture.active = active;
					}
					stylePainter.currentWorldClip = currentGlobalClip;
					GUIClip.SetTransform(root.globalTransform, currentGlobalClip);
					stylePainter.DrawTexture(root.position, root.renderData.pixelCache, Color.white, ScaleMode.ScaleAndCrop, 0f, 0f, 0, 0, 0, 0);
				}
				else
				{
					GUIClip.SetTransform(offset * root.globalTransform, currentGlobalClip);
					this.stylePainter.currentWorldClip = currentGlobalClip;
					this.stylePainter.mousePosition = root.globalTransform.inverse.MultiplyPoint3x4(e.mousePosition);
					this.stylePainter.opacity = root.styles.opacity.GetSpecifiedValueOrDefault(1f);
					root.DoRepaint(this.stylePainter);
					this.stylePainter.opacity = 1f;
					root.ClearDirty(ChangeType.Repaint);
					if (visualContainer != null)
					{
						int childrenCount2 = visualContainer.childrenCount;
						for (int j = 0; j < childrenCount2; j++)
						{
							VisualElement childAt2 = visualContainer.GetChildAt(j);
							this.PaintSubTree(e, childAt2, offset, currentGlobalClip);
							if (childrenCount2 != visualContainer.childrenCount)
							{
								throw new NotImplementedException("Visual tree is read-only during repaint");
							}
						}
					}
				}
			}
		}

		public override void Repaint(Event e)
		{
			this.ValidateLayout();
			this.stylePainter.repaintEvent = e;
			this.PaintSubTree(e, this.visualTree, Matrix4x4.identity, this.visualTree.position);
			if (base.panelDebug != null)
			{
				GUIClip.Internal_Push(this.visualTree.position, Vector2.zero, Vector2.zero, true);
				if (base.panelDebug.EndRepaint())
				{
					this.visualTree.Dirty(ChangeType.Repaint);
				}
				GUIClip.Internal_Pop();
			}
		}
	}
}
