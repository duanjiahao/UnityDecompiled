using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.CSSLayout;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	public class VisualElement : IEventHandler
	{
		public enum MeasureMode
		{
			Undefined,
			Exactly,
			AtMost
		}

		private static uint s_NextId;

		private string m_Name;

		private HashSet<string> m_ClassList;

		private string m_TypeName;

		private string m_FullTypeName;

		private RenderData m_RenderData;

		internal Matrix4x4 m_Transform = Matrix4x4.identity;

		private Rect m_Position;

		private PseudoStates m_PseudoStates;

		private VisualContainer m_Parent;

		internal VisualElementStyles m_Styles = VisualElementStyles.none;

		private List<IManipulator> m_Manipulators = new List<IManipulator>();

		internal readonly uint controlid;

		private ChangeType changesNeeded;

		[SerializeField]
		private string m_Text;

		[SerializeField]
		private string m_Tooltip;

		protected const int DefaultAlignContent = 1;

		protected const int DefaultAlignItems = 4;

		public event Action onEnter
		{
			add
			{
				Action action = this.onEnter;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref this.onEnter, (Action)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action action = this.onEnter;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref this.onEnter, (Action)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public event Action onLeave
		{
			add
			{
				Action action = this.onLeave;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref this.onLeave, (Action)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action action = this.onLeave;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action>(ref this.onLeave, (Action)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		internal event OnStylesResolved onStylesResolved
		{
			add
			{
				OnStylesResolved onStylesResolved = this.onStylesResolved;
				OnStylesResolved onStylesResolved2;
				do
				{
					onStylesResolved2 = onStylesResolved;
					onStylesResolved = Interlocked.CompareExchange<OnStylesResolved>(ref this.onStylesResolved, (OnStylesResolved)Delegate.Combine(onStylesResolved2, value), onStylesResolved);
				}
				while (onStylesResolved != onStylesResolved2);
			}
			remove
			{
				OnStylesResolved onStylesResolved = this.onStylesResolved;
				OnStylesResolved onStylesResolved2;
				do
				{
					onStylesResolved2 = onStylesResolved;
					onStylesResolved = Interlocked.CompareExchange<OnStylesResolved>(ref this.onStylesResolved, (OnStylesResolved)Delegate.Remove(onStylesResolved2, value), onStylesResolved);
				}
				while (onStylesResolved != onStylesResolved2);
			}
		}

		public bool usePixelCaching
		{
			get;
			set;
		}

		internal RenderData renderData
		{
			get
			{
				RenderData arg_1C_0;
				if ((arg_1C_0 = this.m_RenderData) == null)
				{
					arg_1C_0 = (this.m_RenderData = new RenderData());
				}
				return arg_1C_0;
			}
		}

		public Matrix4x4 transform
		{
			get
			{
				return this.m_Transform;
			}
			set
			{
				if (!(this.m_Transform == value))
				{
					this.m_Transform = value;
					this.Dirty(ChangeType.Transform);
				}
			}
		}

		public Rect position
		{
			get
			{
				Rect position = this.m_Position;
				if (this.cssNode != null && this.positionType != PositionType.Manual)
				{
					position.x = this.cssNode.LayoutX;
					position.y = this.cssNode.LayoutY;
					position.width = this.cssNode.LayoutWidth;
					position.height = this.cssNode.LayoutHeight;
				}
				return position;
			}
			set
			{
				if (this.cssNode == null)
				{
					this.cssNode = new CSSNode();
				}
				if (this.positionType != PositionType.Manual || !(this.m_Position == value))
				{
					this.m_Position = value;
					this.cssNode.SetPosition(CSSEdge.Left, value.x);
					this.cssNode.SetPosition(CSSEdge.Top, value.y);
					this.cssNode.Width = value.width;
					this.cssNode.Height = value.height;
					this.EnsureInlineStyles();
					this.m_Styles.positionType = Style<int>.Create(2);
					this.m_Styles.marginLeft = Style<float>.Create(0f);
					this.m_Styles.marginRight = Style<float>.Create(0f);
					this.m_Styles.marginBottom = Style<float>.Create(0f);
					this.m_Styles.marginTop = Style<float>.Create(0f);
					this.m_Styles.positionLeft = Style<float>.Create(value.x);
					this.m_Styles.positionTop = Style<float>.Create(value.y);
					this.m_Styles.positionRight = Style<float>.Create(float.NaN);
					this.m_Styles.positionBottom = Style<float>.Create(float.NaN);
					this.m_Styles.width = Style<float>.Create(value.width);
					this.m_Styles.height = Style<float>.Create(value.height);
					this.Dirty(ChangeType.Layout);
				}
			}
		}

		public Rect contentRect
		{
			get
			{
				Spacing a = new Spacing(this.paddingLeft, this.paddingTop, this.paddingRight, this.paddingBottom);
				return this.paddingRect - a;
			}
		}

		protected Rect paddingRect
		{
			get
			{
				Spacing a = new Spacing(this.borderWidth, this.borderWidth, this.borderWidth, this.borderWidth);
				return this.position - a;
			}
		}

		public Rect globalBound
		{
			get
			{
				Matrix4x4 globalTransform = this.globalTransform;
				Vector3 vector = globalTransform.MultiplyPoint3x4(this.position.min);
				Vector3 vector2 = globalTransform.MultiplyPoint3x4(this.position.max);
				return Rect.MinMaxRect(Math.Min(vector.x, vector2.x), Math.Min(vector.y, vector2.y), Math.Max(vector.x, vector2.x), Math.Max(vector.y, vector2.y));
			}
		}

		public Rect localBound
		{
			get
			{
				Matrix4x4 transform = this.transform;
				Vector3 vector = transform.MultiplyPoint3x4(this.position.min);
				Vector3 vector2 = transform.MultiplyPoint3x4(this.position.max);
				return Rect.MinMaxRect(Math.Min(vector.x, vector2.x), Math.Min(vector.y, vector2.y), Math.Max(vector.x, vector2.x), Math.Max(vector.y, vector2.y));
			}
		}

		public Matrix4x4 globalTransform
		{
			get
			{
				if (this.IsDirty(ChangeType.Transform))
				{
					if (this.parent != null)
					{
						this.renderData.worldTransForm = this.parent.globalTransform * Matrix4x4.Translate(new Vector3(this.parent.position.x, this.parent.position.y, 0f)) * this.transform;
					}
					else
					{
						this.renderData.worldTransForm = this.transform;
					}
					this.ClearDirty(ChangeType.Transform);
				}
				return this.renderData.worldTransForm;
			}
		}

		internal PseudoStates pseudoStates
		{
			get
			{
				return this.m_PseudoStates;
			}
			set
			{
				if (this.m_PseudoStates != value)
				{
					this.m_PseudoStates = value;
					this.Dirty(ChangeType.Styles);
				}
			}
		}

		public VisualContainer parent
		{
			get
			{
				return this.m_Parent;
			}
			set
			{
				this.m_Parent = value;
				if (value != null)
				{
					this.ChangePanel(this.m_Parent.elementPanel);
				}
				else
				{
					this.ChangePanel(null);
				}
			}
		}

		internal BaseVisualElementPanel elementPanel
		{
			get;
			private set;
		}

		public IPanel panel
		{
			get
			{
				return this.elementPanel;
			}
		}

		public EventPhase phaseInterest
		{
			get;
			set;
		}

		public PickingMode pickingMode
		{
			get;
			set;
		}

		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				if (!(this.m_Name == value))
				{
					this.m_Name = value;
					this.Dirty(ChangeType.Styles);
				}
			}
		}

		internal string fullTypeName
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_FullTypeName))
				{
					this.m_FullTypeName = base.GetType().FullName;
				}
				return this.m_FullTypeName;
			}
		}

		internal string typeName
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_TypeName))
				{
					this.m_TypeName = base.GetType().Name;
				}
				return this.m_TypeName;
			}
		}

		internal CSSNode cssNode
		{
			get;
			private set;
		}

		internal VisualElementStyles styles
		{
			get
			{
				return this.m_Styles;
			}
		}

		public float width
		{
			get
			{
				return this.m_Styles.width;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.width = Style<float>.Create(value);
				this.cssNode.Width = value;
				this.Dirty(ChangeType.Layout);
			}
		}

		public float height
		{
			get
			{
				return this.m_Styles.height;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.height = Style<float>.Create(value);
				this.cssNode.Height = value;
				this.Dirty(ChangeType.Layout);
			}
		}

		public float maxWidth
		{
			get
			{
				return this.m_Styles.maxWidth;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.maxWidth = Style<float>.Create(value);
				this.cssNode.MaxWidth = value;
				this.Dirty(ChangeType.Layout);
			}
		}

		public float maxHeight
		{
			get
			{
				return this.m_Styles.maxHeight;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.maxHeight = Style<float>.Create(value);
				this.cssNode.MaxHeight = value;
				this.Dirty(ChangeType.Layout);
			}
		}

		public float minWidth
		{
			get
			{
				return this.m_Styles.minWidth;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.minWidth = Style<float>.Create(value);
				this.cssNode.MinWidth = value;
				this.Dirty(ChangeType.Layout);
			}
		}

		public float minHeight
		{
			get
			{
				return this.m_Styles.minHeight;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.minHeight = Style<float>.Create(value);
				this.cssNode.MinHeight = value;
				this.Dirty(ChangeType.Layout);
			}
		}

		public float flex
		{
			get
			{
				return this.m_Styles.flex;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.flex = Style<float>.Create(value);
				this.cssNode.Flex = value;
				this.Dirty(ChangeType.Layout);
			}
		}

		public float positionLeft
		{
			get
			{
				return this.m_Styles.positionLeft;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.positionLeft = Style<float>.Create(value);
				this.cssNode.SetPosition(CSSEdge.Left, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float positionTop
		{
			get
			{
				return this.m_Styles.positionTop;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.positionTop = Style<float>.Create(value);
				this.cssNode.SetPosition(CSSEdge.Top, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float positionRight
		{
			get
			{
				return this.m_Styles.positionRight;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.positionRight = Style<float>.Create(value);
				this.cssNode.SetPosition(CSSEdge.Right, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float positionBottom
		{
			get
			{
				return this.m_Styles.positionBottom;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.positionBottom = Style<float>.Create(value);
				this.cssNode.SetPosition(CSSEdge.Bottom, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float marginLeft
		{
			get
			{
				return this.m_Styles.marginLeft;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.marginLeft = Style<float>.Create(value);
				this.cssNode.SetMargin(CSSEdge.Left, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float marginTop
		{
			get
			{
				return this.m_Styles.marginTop;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.marginTop = Style<float>.Create(value);
				this.cssNode.SetMargin(CSSEdge.Top, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float marginRight
		{
			get
			{
				return this.m_Styles.marginRight;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.marginRight = Style<float>.Create(value);
				this.cssNode.SetMargin(CSSEdge.Right, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float marginBottom
		{
			get
			{
				return this.m_Styles.marginBottom;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.marginBottom = Style<float>.Create(value);
				this.cssNode.SetMargin(CSSEdge.Bottom, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float borderLeft
		{
			get
			{
				return this.m_Styles.borderLeft;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.borderLeft = Style<float>.Create(value);
				this.cssNode.SetBorder(CSSEdge.Left, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float borderTop
		{
			get
			{
				return this.m_Styles.borderTop;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.borderTop = Style<float>.Create(value);
				this.cssNode.SetBorder(CSSEdge.Top, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float borderRight
		{
			get
			{
				return this.m_Styles.borderRight;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.borderRight = Style<float>.Create(value);
				this.cssNode.SetBorder(CSSEdge.Right, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float borderBottom
		{
			get
			{
				return this.m_Styles.borderBottom;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.borderBottom = Style<float>.Create(value);
				this.cssNode.SetBorder(CSSEdge.Bottom, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float paddingLeft
		{
			get
			{
				return this.m_Styles.paddingLeft;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.paddingLeft = Style<float>.Create(value);
				this.cssNode.SetPadding(CSSEdge.Left, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float paddingTop
		{
			get
			{
				return this.m_Styles.paddingTop;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.paddingTop = Style<float>.Create(value);
				this.cssNode.SetPadding(CSSEdge.Top, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float paddingRight
		{
			get
			{
				return this.m_Styles.paddingRight;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.paddingRight = Style<float>.Create(value);
				this.cssNode.SetPadding(CSSEdge.Right, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public float paddingBottom
		{
			get
			{
				return this.m_Styles.paddingBottom;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.paddingBottom = Style<float>.Create(value);
				this.cssNode.SetPadding(CSSEdge.Bottom, value);
				this.Dirty(ChangeType.Layout);
			}
		}

		public PositionType positionType
		{
			get
			{
				return (PositionType)this.m_Styles.positionType.value;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.positionType = Style<int>.Create((int)value);
				this.cssNode.PositionType = (CSSPositionType)value;
				this.Dirty(ChangeType.Layout);
			}
		}

		public ImageScaleMode backgroundSize
		{
			get
			{
				return (ImageScaleMode)this.m_Styles.backgroundSize.value;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.backgroundSize = Style<int>.Create((int)value);
			}
		}

		public Align alignSelf
		{
			get
			{
				return (Align)this.m_Styles.alignSelf.value;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.alignSelf = Style<int>.Create((int)value);
				this.cssNode.AlignSelf = (CSSAlign)value;
				this.Dirty(ChangeType.Layout);
			}
		}

		public TextAnchor textAlignment
		{
			get
			{
				return (TextAnchor)this.m_Styles.textAlignment.value;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.textAlignment = Style<int>.Create((int)value);
			}
		}

		public FontStyle fontStyle
		{
			get
			{
				return (FontStyle)this.m_Styles.fontStyle.value;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.fontStyle = Style<int>.Create((int)value);
			}
		}

		public TextClipping textClipping
		{
			get
			{
				return (TextClipping)this.m_Styles.textClipping.value;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.textClipping = Style<int>.Create((int)value);
			}
		}

		public Font font
		{
			get
			{
				return this.m_Styles.font;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.font = Style<Font>.Create(value);
			}
		}

		public int fontSize
		{
			get
			{
				return this.m_Styles.fontSize;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.fontSize = Style<int>.Create(value);
			}
		}

		public bool wordWrap
		{
			get
			{
				return this.m_Styles.wordWrap;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.wordWrap = Style<bool>.Create(value);
			}
		}

		public Texture2D backgroundImage
		{
			get
			{
				return this.m_Styles.backgroundImage;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.backgroundImage = Style<Texture2D>.Create(value);
			}
		}

		public Color textColor
		{
			get
			{
				return this.m_Styles.textColor.GetSpecifiedValueOrDefault(Color.black);
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.textColor = Style<Color>.Create(value);
			}
		}

		public Color backgroundColor
		{
			get
			{
				return this.m_Styles.backgroundColor;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.backgroundColor = Style<Color>.Create(value);
			}
		}

		public Color borderColor
		{
			get
			{
				return this.m_Styles.borderColor;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.borderColor = Style<Color>.Create(value);
			}
		}

		public float borderWidth
		{
			get
			{
				return this.m_Styles.borderWidth;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.borderWidth = Style<float>.Create(value);
			}
		}

		public float borderRadius
		{
			get
			{
				return this.m_Styles.borderRadius;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.borderRadius = Style<float>.Create(value);
			}
		}

		public Overflow overflow
		{
			get
			{
				return (Overflow)this.m_Styles.overflow.value;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.overflow = Style<int>.Create((int)value);
				this.cssNode.Overflow = (CSSOverflow)value;
				this.Dirty(ChangeType.Layout);
			}
		}

		internal float opacity
		{
			get
			{
				return this.m_Styles.opacity.value;
			}
			set
			{
				this.EnsureInlineStyles();
				this.m_Styles.opacity = Style<float>.Create(value);
				this.Dirty(ChangeType.Repaint);
			}
		}

		public string text
		{
			get
			{
				return this.m_Text ?? string.Empty;
			}
			set
			{
				if (this.m_Text != value)
				{
					this.m_Text = value;
					this.Dirty(ChangeType.Layout);
				}
			}
		}

		public string tooltip
		{
			get
			{
				return this.m_Tooltip ?? string.Empty;
			}
			set
			{
				if (this.m_Tooltip != value)
				{
					this.m_Tooltip = value;
					this.Dirty(ChangeType.Layout);
				}
			}
		}

		public virtual bool enabled
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Disabled) != PseudoStates.Disabled;
			}
			set
			{
				if (value)
				{
					this.pseudoStates &= ~PseudoStates.Disabled;
				}
				else
				{
					this.pseudoStates |= PseudoStates.Disabled;
				}
			}
		}

		public bool visible
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Invisible) != PseudoStates.Invisible;
			}
			set
			{
				if (value)
				{
					this.pseudoStates &= (PseudoStates)2147483647;
				}
				else
				{
					this.pseudoStates |= PseudoStates.Invisible;
				}
			}
		}

		public VisualElement()
		{
			this.controlid = (VisualElement.s_NextId += 1u);
			this.m_ClassList = new HashSet<string>();
			this.m_FullTypeName = string.Empty;
			this.m_TypeName = string.Empty;
			this.enabled = true;
			this.visible = true;
			this.phaseInterest = EventPhase.BubbleUp;
			this.name = string.Empty;
			this.cssNode = new CSSNode();
			this.cssNode.SetMeasureFunction(new MeasureFunction(this.Measure));
			this.changesNeeded = (ChangeType)0;
		}

		public virtual void OnStylesResolved(ICustomStyles styles)
		{
			this.FinalizeLayout();
		}

		internal List<IManipulator>.Enumerator GetManipulatorsInternal()
		{
			List<IManipulator>.Enumerator result;
			if (this.m_Manipulators != null)
			{
				result = this.m_Manipulators.GetEnumerator();
			}
			else
			{
				result = default(List<IManipulator>.Enumerator);
			}
			return result;
		}

		public void InsertManipulator(int index, IManipulator manipulator)
		{
			if (this.m_Manipulators == null)
			{
				this.m_Manipulators = new List<IManipulator>();
			}
			manipulator.target = this;
			if (!this.m_Manipulators.Contains(manipulator))
			{
				this.m_Manipulators.Insert(index, manipulator);
			}
		}

		public void AddManipulator(IManipulator manipulator)
		{
			if (this.m_Manipulators == null)
			{
				this.m_Manipulators = new List<IManipulator>();
			}
			manipulator.target = this;
			if (!this.m_Manipulators.Contains(manipulator))
			{
				this.m_Manipulators.Add(manipulator);
			}
		}

		public void RemoveManipulator(IManipulator manipulator)
		{
			manipulator.target = null;
			if (this.m_Manipulators != null)
			{
				this.m_Manipulators.Remove(manipulator);
			}
		}

		internal virtual void ChangePanel(BaseVisualElementPanel p)
		{
			if (this.panel != p)
			{
				if (this.panel != null && this.onLeave != null)
				{
					this.onLeave();
				}
				this.elementPanel = p;
				if (this.panel != null && this.onEnter != null)
				{
					this.onEnter();
				}
			}
		}

		private void PropagateToChildren(ChangeType type)
		{
			if ((type & this.changesNeeded) != type)
			{
				this.changesNeeded |= type;
				type &= (ChangeType.Styles | ChangeType.Transform);
				if (type != (ChangeType)0)
				{
					VisualContainer visualContainer = this as VisualContainer;
					if (visualContainer != null)
					{
						foreach (VisualElement current in visualContainer)
						{
							current.PropagateToChildren(type);
						}
					}
				}
			}
		}

		protected internal void PropagateChangesToParents()
		{
			ChangeType changeType = (ChangeType)0;
			if ((this.changesNeeded & ChangeType.Styles) > (ChangeType)0)
			{
				changeType |= ChangeType.StylesPath;
				changeType |= ChangeType.Repaint;
			}
			if ((this.changesNeeded & ChangeType.Transform) > (ChangeType)0)
			{
				changeType |= ChangeType.Repaint;
			}
			if ((this.changesNeeded & ChangeType.Layout) > (ChangeType)0)
			{
				changeType |= ChangeType.Repaint;
			}
			if ((this.changesNeeded & ChangeType.Repaint) > (ChangeType)0)
			{
				changeType |= ChangeType.Repaint;
			}
			for (VisualContainer parent = this.parent; parent != null; parent = parent.parent)
			{
				if ((parent.changesNeeded & changeType) == changeType)
				{
					break;
				}
				parent.changesNeeded |= changeType;
			}
		}

		public void Dirty(ChangeType type)
		{
			if ((type & this.changesNeeded) != type)
			{
				if ((type & ChangeType.Layout) > (ChangeType)0 && this.cssNode != null && this.cssNode.IsMeasureDefined)
				{
					this.cssNode.MarkDirty();
				}
				this.PropagateToChildren(type);
				this.PropagateChangesToParents();
			}
		}

		public bool IsDirty(ChangeType type)
		{
			return (this.changesNeeded & type) > (ChangeType)0;
		}

		public void ClearDirty(ChangeType type)
		{
			this.changesNeeded &= ~type;
		}

		protected internal virtual void OnPostLayout(bool hasNewLayout)
		{
		}

		public virtual void DoRepaint()
		{
			ScaleMode backgroundSize = (ScaleMode)this.backgroundSize;
			IStylePainter stylePainter = this.elementPanel.stylePainter;
			if (this.backgroundImage != null)
			{
				stylePainter.DrawTexture(this.position, this.backgroundImage, Color.white, backgroundSize, 0f, this.borderRadius, this.m_Styles.sliceLeft, this.m_Styles.sliceTop, this.m_Styles.sliceRight, this.m_Styles.sliceBottom);
			}
			else if (this.backgroundColor != Color.clear)
			{
				stylePainter.DrawRect(this.position, this.backgroundColor, 0f, this.borderRadius);
			}
			if (this.borderColor != Color.clear && this.borderWidth > 0f)
			{
				stylePainter.DrawRect(this.position, this.borderColor, this.borderWidth, this.borderRadius);
			}
			if (!string.IsNullOrEmpty(this.text) && this.contentRect.width > 0f && this.contentRect.height > 0f)
			{
				stylePainter.DrawText(this.contentRect, this.text, this.font, this.fontSize, this.fontStyle, this.textColor, this.textAlignment, this.wordWrap, this.contentRect.width, false, this.textClipping);
			}
		}

		internal virtual void DoRepaint(IStylePainter painter)
		{
			if ((this.pseudoStates & PseudoStates.Invisible) != PseudoStates.Invisible)
			{
				this.DoRepaint();
			}
		}

		public virtual bool ContainsPoint(Vector2 localPoint)
		{
			return this.position.Contains(localPoint);
		}

		public virtual bool ContainsPointToLocal(Vector2 point)
		{
			return this.ContainsPoint(this.ChangeCoordinatesTo(this.parent, point));
		}

		public virtual bool Overlaps(Rect rectangle)
		{
			return this.position.Overlaps(rectangle, true);
		}

		public virtual EventPropagation HandleEvent(Event evt, VisualElement finalTarget)
		{
			return EventPropagation.Continue;
		}

		public virtual void OnLostCapture()
		{
		}

		public virtual void OnLostKeyboardFocus()
		{
		}

		protected internal virtual Vector2 DoMeasure(float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			float num = float.NaN;
			float num2 = float.NaN;
			Vector2 result;
			if (string.IsNullOrEmpty(this.text) || this.font == null)
			{
				result = new Vector2(num, num2);
			}
			else
			{
				if (widthMode == VisualElement.MeasureMode.Exactly)
				{
					num = width;
				}
				else
				{
					num = this.elementPanel.stylePainter.ComputeTextWidth(this.text, this.font, this.fontSize, this.fontStyle, this.textAlignment, true);
					if (widthMode == VisualElement.MeasureMode.AtMost)
					{
						num = Mathf.Min(num, width);
					}
				}
				if (heightMode == VisualElement.MeasureMode.Exactly)
				{
					num2 = height;
				}
				else
				{
					num2 = this.elementPanel.stylePainter.ComputeTextHeight(this.text, num, this.wordWrap, this.font, this.fontSize, this.fontStyle, this.textAlignment, true);
					if (heightMode == VisualElement.MeasureMode.AtMost)
					{
						num2 = Mathf.Min(num2, height);
					}
				}
				result = new Vector2(num, num2);
			}
			return result;
		}

		internal long Measure(CSSNode node, float width, CSSMeasureMode widthMode, float height, CSSMeasureMode heightMode)
		{
			Debug.Assert(node == this.cssNode, "CSSNode instance mismatch");
			Vector2 vector = this.DoMeasure(width, (VisualElement.MeasureMode)widthMode, height, (VisualElement.MeasureMode)heightMode);
			return MeasureOutput.Make(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
		}

		public void SetSize(Vector2 size)
		{
			Rect position = this.position;
			position.width = size.x;
			position.height = size.y;
			this.position = position;
		}

		internal void FinalizeLayout()
		{
			this.cssNode.Flex = this.styles.flex.GetSpecifiedValueOrDefault(float.NaN);
			this.cssNode.SetPosition(CSSEdge.Left, this.styles.positionLeft.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetPosition(CSSEdge.Top, this.styles.positionTop.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetPosition(CSSEdge.Right, this.styles.positionRight.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetPosition(CSSEdge.Bottom, this.styles.positionBottom.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetMargin(CSSEdge.Left, this.styles.marginLeft.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetMargin(CSSEdge.Top, this.styles.marginTop.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetMargin(CSSEdge.Right, this.styles.marginRight.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetMargin(CSSEdge.Bottom, this.styles.marginBottom.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetPadding(CSSEdge.Left, this.styles.paddingLeft.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetPadding(CSSEdge.Top, this.styles.paddingTop.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetPadding(CSSEdge.Right, this.styles.paddingRight.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetPadding(CSSEdge.Bottom, this.styles.paddingBottom.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetBorder(CSSEdge.Left, this.styles.borderLeft.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetBorder(CSSEdge.Top, this.styles.borderTop.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetBorder(CSSEdge.Right, this.styles.borderRight.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.SetBorder(CSSEdge.Bottom, this.styles.borderBottom.GetSpecifiedValueOrDefault(float.NaN));
			this.cssNode.Width = this.styles.width.GetSpecifiedValueOrDefault(float.NaN);
			this.cssNode.Height = this.styles.height.GetSpecifiedValueOrDefault(float.NaN);
			PositionType value = (PositionType)this.styles.positionType.value;
			if (value != PositionType.Absolute && value != PositionType.Manual)
			{
				if (value == PositionType.Relative)
				{
					this.cssNode.PositionType = CSSPositionType.Relative;
				}
			}
			else
			{
				this.cssNode.PositionType = CSSPositionType.Absolute;
			}
			this.cssNode.Overflow = (CSSOverflow)this.styles.overflow.value;
			this.cssNode.AlignSelf = (CSSAlign)this.styles.alignSelf.value;
			this.cssNode.MaxWidth = this.styles.maxWidth.GetSpecifiedValueOrDefault(float.NaN);
			this.cssNode.MaxHeight = this.styles.maxHeight.GetSpecifiedValueOrDefault(float.NaN);
			this.cssNode.MinWidth = this.styles.minWidth.GetSpecifiedValueOrDefault(float.NaN);
			this.cssNode.MinHeight = this.styles.minHeight.GetSpecifiedValueOrDefault(float.NaN);
			this.cssNode.FlexDirection = (CSSFlexDirection)this.styles.flexDirection.value;
			this.cssNode.AlignContent = (CSSAlign)this.styles.alignContent.GetSpecifiedValueOrDefault(1);
			this.cssNode.AlignItems = (CSSAlign)this.styles.alignItems.GetSpecifiedValueOrDefault(4);
			this.cssNode.JustifyContent = (CSSJustify)this.styles.justifyContent.value;
			this.cssNode.Wrap = (CSSWrap)this.styles.flexWrap.value;
			this.Dirty(ChangeType.Layout);
		}

		internal void SetSharedStyles(VisualElementStyles styles)
		{
			Debug.Assert(styles.isShared);
			this.ClearDirty(ChangeType.Styles | ChangeType.StylesPath);
			if (styles != this.m_Styles)
			{
				if (!this.m_Styles.isShared)
				{
					this.m_Styles.Apply(styles, StylePropertyApplyMode.CopyIfNotInline);
				}
				else
				{
					this.m_Styles = styles;
				}
				if (this.onStylesResolved != null)
				{
					this.onStylesResolved(this.m_Styles);
				}
				this.OnStylesResolved(this.m_Styles);
				this.Dirty(ChangeType.Repaint);
			}
		}

		internal void EnsureInlineStyles()
		{
			if (this.m_Styles.isShared)
			{
				this.m_Styles = new VisualElementStyles(this.m_Styles, false);
			}
		}

		public void ResetPositionProperties()
		{
			if (this.m_Styles != null && !this.m_Styles.isShared)
			{
				this.m_Styles.positionType = Style<int>.nil;
				this.m_Styles.marginLeft = Style<float>.nil;
				this.m_Styles.marginRight = Style<float>.nil;
				this.m_Styles.marginBottom = Style<float>.nil;
				this.m_Styles.marginTop = Style<float>.nil;
				this.m_Styles.positionLeft = Style<float>.nil;
				this.m_Styles.positionTop = Style<float>.nil;
				this.m_Styles.positionRight = Style<float>.nil;
				this.m_Styles.positionBottom = Style<float>.nil;
				this.m_Styles.width = Style<float>.nil;
				this.m_Styles.height = Style<float>.nil;
				this.Dirty(ChangeType.Styles);
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.name,
				" ",
				this.position,
				" global rect: ",
				this.globalBound
			});
		}

		internal IEnumerable<string> GetClasses()
		{
			return this.m_ClassList;
		}

		public void ClearClassList()
		{
			if (this.m_ClassList != null && this.m_ClassList.Count > 0)
			{
				this.m_ClassList.Clear();
				this.Dirty(ChangeType.Styles);
			}
		}

		public void AddToClassList(string className)
		{
			if (this.m_ClassList == null)
			{
				this.m_ClassList = new HashSet<string>();
			}
			if (this.m_ClassList.Add(className))
			{
				this.Dirty(ChangeType.Styles);
			}
		}

		public void RemoveFromClassList(string className)
		{
			if (this.m_ClassList != null && this.m_ClassList.Remove(className))
			{
				this.Dirty(ChangeType.Styles);
			}
		}

		public bool ClassListContains(string cls)
		{
			return this.m_ClassList != null && this.m_ClassList.Contains(cls);
		}
	}
}
