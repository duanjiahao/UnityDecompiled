using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.CSSLayout
{
	internal class CSSNode : IEnumerable<CSSNode>, IEnumerable
	{
		private IntPtr _cssNode;

		private WeakReference _parent;

		private List<CSSNode> _children;

		private MeasureFunction _measureFunction;

		private CSSMeasureFunc _cssMeasureFunc;

		private object _data;

		public bool IsDirty
		{
			get
			{
				return Native.CSSNodeIsDirty(this._cssNode);
			}
		}

		public bool HasNewLayout
		{
			get
			{
				return Native.CSSNodeGetHasNewLayout(this._cssNode);
			}
		}

		public CSSNode Parent
		{
			get
			{
				return (this._parent == null) ? null : (this._parent.Target as CSSNode);
			}
		}

		public bool IsMeasureDefined
		{
			get
			{
				return this._measureFunction != null;
			}
		}

		public CSSDirection StyleDirection
		{
			get
			{
				return Native.CSSNodeStyleGetDirection(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetDirection(this._cssNode, value);
			}
		}

		public CSSFlexDirection FlexDirection
		{
			get
			{
				return Native.CSSNodeStyleGetFlexDirection(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetFlexDirection(this._cssNode, value);
			}
		}

		public CSSJustify JustifyContent
		{
			get
			{
				return Native.CSSNodeStyleGetJustifyContent(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetJustifyContent(this._cssNode, value);
			}
		}

		public CSSAlign AlignItems
		{
			get
			{
				return Native.CSSNodeStyleGetAlignItems(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetAlignItems(this._cssNode, value);
			}
		}

		public CSSAlign AlignSelf
		{
			get
			{
				return Native.CSSNodeStyleGetAlignSelf(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetAlignSelf(this._cssNode, value);
			}
		}

		public CSSAlign AlignContent
		{
			get
			{
				return Native.CSSNodeStyleGetAlignContent(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetAlignContent(this._cssNode, value);
			}
		}

		public CSSPositionType PositionType
		{
			get
			{
				return Native.CSSNodeStyleGetPositionType(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetPositionType(this._cssNode, value);
			}
		}

		public CSSWrap Wrap
		{
			get
			{
				return Native.CSSNodeStyleGetFlexWrap(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetFlexWrap(this._cssNode, value);
			}
		}

		public float Flex
		{
			set
			{
				Native.CSSNodeStyleSetFlex(this._cssNode, value);
			}
		}

		public float FlexGrow
		{
			get
			{
				return Native.CSSNodeStyleGetFlexGrow(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetFlexGrow(this._cssNode, value);
			}
		}

		public float FlexShrink
		{
			get
			{
				return Native.CSSNodeStyleGetFlexShrink(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetFlexShrink(this._cssNode, value);
			}
		}

		public float FlexBasis
		{
			get
			{
				return Native.CSSNodeStyleGetFlexBasis(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetFlexBasis(this._cssNode, value);
			}
		}

		public float Width
		{
			get
			{
				return Native.CSSNodeStyleGetWidth(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetWidth(this._cssNode, value);
			}
		}

		public float Height
		{
			get
			{
				return Native.CSSNodeStyleGetHeight(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetHeight(this._cssNode, value);
			}
		}

		public float MaxWidth
		{
			get
			{
				return Native.CSSNodeStyleGetMaxWidth(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetMaxWidth(this._cssNode, value);
			}
		}

		public float MaxHeight
		{
			get
			{
				return Native.CSSNodeStyleGetMaxHeight(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetMaxHeight(this._cssNode, value);
			}
		}

		public float MinWidth
		{
			get
			{
				return Native.CSSNodeStyleGetMinWidth(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetMinWidth(this._cssNode, value);
			}
		}

		public float MinHeight
		{
			get
			{
				return Native.CSSNodeStyleGetMinHeight(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetMinHeight(this._cssNode, value);
			}
		}

		public float AspectRatio
		{
			get
			{
				return Native.CSSNodeStyleGetAspectRatio(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetAspectRatio(this._cssNode, value);
			}
		}

		public float LayoutX
		{
			get
			{
				return Native.CSSNodeLayoutGetLeft(this._cssNode);
			}
		}

		public float LayoutY
		{
			get
			{
				return Native.CSSNodeLayoutGetTop(this._cssNode);
			}
		}

		public float LayoutWidth
		{
			get
			{
				return Native.CSSNodeLayoutGetWidth(this._cssNode);
			}
		}

		public float LayoutHeight
		{
			get
			{
				return Native.CSSNodeLayoutGetHeight(this._cssNode);
			}
		}

		public CSSDirection LayoutDirection
		{
			get
			{
				return Native.CSSNodeLayoutGetDirection(this._cssNode);
			}
		}

		public CSSOverflow Overflow
		{
			get
			{
				return Native.CSSNodeStyleGetOverflow(this._cssNode);
			}
			set
			{
				Native.CSSNodeStyleSetOverflow(this._cssNode, value);
			}
		}

		public object Data
		{
			get
			{
				return this._data;
			}
			set
			{
				this._data = value;
			}
		}

		public CSSNode this[int index]
		{
			get
			{
				return this._children[index];
			}
		}

		public int Count
		{
			get
			{
				return (this._children == null) ? 0 : this._children.Count;
			}
		}

		public CSSNode()
		{
			CSSLogger.Initialize();
			this._cssNode = Native.CSSNodeNew();
			if (this._cssNode == IntPtr.Zero)
			{
				throw new InvalidOperationException("Failed to allocate native memory");
			}
		}

		~CSSNode()
		{
			Native.CSSNodeFree(this._cssNode);
		}

		public void Reset()
		{
			this._measureFunction = null;
			this._data = null;
			Native.CSSNodeReset(this._cssNode);
		}

		public virtual void MarkDirty()
		{
			Native.CSSNodeMarkDirty(this._cssNode);
		}

		public void MarkHasNewLayout()
		{
			Native.CSSNodeSetHasNewLayout(this._cssNode, true);
		}

		public void CopyStyle(CSSNode srcNode)
		{
			Native.CSSNodeCopyStyle(this._cssNode, srcNode._cssNode);
		}

		public float GetMargin(CSSEdge edge)
		{
			return Native.CSSNodeStyleGetMargin(this._cssNode, edge);
		}

		public void SetMargin(CSSEdge edge, float value)
		{
			Native.CSSNodeStyleSetMargin(this._cssNode, edge, value);
		}

		public float GetPadding(CSSEdge edge)
		{
			return Native.CSSNodeStyleGetPadding(this._cssNode, edge);
		}

		public void SetPadding(CSSEdge edge, float padding)
		{
			Native.CSSNodeStyleSetPadding(this._cssNode, edge, padding);
		}

		public float GetBorder(CSSEdge edge)
		{
			return Native.CSSNodeStyleGetBorder(this._cssNode, edge);
		}

		public void SetBorder(CSSEdge edge, float border)
		{
			Native.CSSNodeStyleSetBorder(this._cssNode, edge, border);
		}

		public float GetPosition(CSSEdge edge)
		{
			return Native.CSSNodeStyleGetPosition(this._cssNode, edge);
		}

		public void SetPosition(CSSEdge edge, float position)
		{
			Native.CSSNodeStyleSetPosition(this._cssNode, edge, position);
		}

		public void MarkLayoutSeen()
		{
			Native.CSSNodeSetHasNewLayout(this._cssNode, false);
		}

		public bool ValuesEqual(float f1, float f2)
		{
			bool result;
			if (float.IsNaN(f1) || float.IsNaN(f2))
			{
				result = (float.IsNaN(f1) && float.IsNaN(f2));
			}
			else
			{
				result = (Math.Abs(f2 - f1) < 1.401298E-45f);
			}
			return result;
		}

		public void Insert(int index, CSSNode node)
		{
			if (this._children == null)
			{
				this._children = new List<CSSNode>(4);
			}
			this._children.Insert(index, node);
			node._parent = new WeakReference(this);
			Native.CSSNodeInsertChild(this._cssNode, node._cssNode, (uint)index);
		}

		public void RemoveAt(int index)
		{
			CSSNode cSSNode = this._children[index];
			cSSNode._parent = null;
			this._children.RemoveAt(index);
			Native.CSSNodeRemoveChild(this._cssNode, cSSNode._cssNode);
		}

		public void Clear()
		{
			if (this._children != null)
			{
				while (this._children.Count > 0)
				{
					this.RemoveAt(this._children.Count - 1);
				}
			}
		}

		public int IndexOf(CSSNode node)
		{
			return (this._children == null) ? -1 : this._children.IndexOf(node);
		}

		public void SetMeasureFunction(MeasureFunction measureFunction)
		{
			this._measureFunction = measureFunction;
			this._cssMeasureFunc = ((measureFunction == null) ? null : new CSSMeasureFunc(this.MeasureInternal));
			Native.CSSNodeSetMeasureFunc(this._cssNode, this._cssMeasureFunc);
		}

		public void CalculateLayout()
		{
			Native.CSSNodeCalculateLayout(this._cssNode, float.NaN, float.NaN, Native.CSSNodeStyleGetDirection(this._cssNode));
		}

		private CSSSize MeasureInternal(IntPtr node, float width, CSSMeasureMode widthMode, float height, CSSMeasureMode heightMode)
		{
			if (this._measureFunction == null)
			{
				throw new InvalidOperationException("Measure function is not defined.");
			}
			long measureOutput = this._measureFunction(this, width, widthMode, height, heightMode);
			return new CSSSize
			{
				width = (float)MeasureOutput.GetWidth(measureOutput),
				height = (float)MeasureOutput.GetHeight(measureOutput)
			};
		}

		public string Print()
		{
			return this.Print((CSSPrintOptions)7);
		}

		public string Print(CSSPrintOptions options)
		{
			StringBuilder sb = new StringBuilder();
			CSSLogger.Func logger = CSSLogger.Logger;
			CSSLogger.Logger = delegate(CSSLogLevel level, string message)
			{
				sb.Append(message);
			};
			Native.CSSNodePrint(this._cssNode, options);
			CSSLogger.Logger = logger;
			return sb.ToString();
		}

		public IEnumerator<CSSNode> GetEnumerator()
		{
			return (this._children == null) ? Enumerable.Empty<CSSNode>().GetEnumerator() : ((IEnumerable<CSSNode>)this._children).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this._children == null) ? Enumerable.Empty<CSSNode>().GetEnumerator() : ((IEnumerable<CSSNode>)this._children).GetEnumerator();
		}

		public static int GetInstanceCount()
		{
			return Native.CSSNodeGetInstanceCount();
		}

		public static void SetExperimentalFeatureEnabled(CSSExperimentalFeature feature, bool enabled)
		{
			Native.CSSLayoutSetExperimentalFeatureEnabled(feature, enabled);
		}

		public static bool IsExperimentalFeatureEnabled(CSSExperimentalFeature feature)
		{
			return Native.CSSLayoutIsExperimentalFeatureEnabled(feature);
		}
	}
}
