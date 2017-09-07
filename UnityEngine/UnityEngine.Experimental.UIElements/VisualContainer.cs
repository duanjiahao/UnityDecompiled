using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.CSSLayout;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	public class VisualContainer : VisualElement, IEnumerable<VisualElement>, IEnumerable
	{
		public struct Enumerator : IEnumerator<VisualElement>, IDisposable, IEnumerator
		{
			private List<VisualElement>.Enumerator m_Enumerator;

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public VisualElement Current
			{
				get
				{
					return this.m_Enumerator.Current;
				}
			}

			public Enumerator(ref List<VisualElement> list)
			{
				this.m_Enumerator = list.GetEnumerator();
			}

			public void Dispose()
			{
				this.m_Enumerator.Dispose();
			}

			public void Reset()
			{
				((IEnumerator)this.m_Enumerator).Reset();
			}

			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}
		}

		private List<VisualElement> m_Children = new List<VisualElement>();

		internal List<StyleSheet> m_StyleSheets;

		private List<string> m_StyleSheetPaths;

		internal IEnumerable<StyleSheet> styleSheets
		{
			get
			{
				return this.m_StyleSheets;
			}
		}

		public bool clipChildren
		{
			get;
			set;
		}

		public int childrenCount
		{
			get
			{
				return this.m_Children.Count;
			}
		}

		public FlexDirection flexDirection
		{
			get
			{
				return (FlexDirection)this.m_Styles.flexDirection.value;
			}
			set
			{
				base.EnsureInlineStyles();
				this.m_Styles.flexDirection = Style<int>.Create((int)value);
				base.cssNode.FlexDirection = (CSSFlexDirection)value;
			}
		}

		public Align alignItems
		{
			get
			{
				return (Align)this.m_Styles.alignItems.value;
			}
			set
			{
				base.EnsureInlineStyles();
				this.m_Styles.alignItems = Style<int>.Create((int)value);
				base.cssNode.AlignItems = (CSSAlign)value;
			}
		}

		public Align alignContent
		{
			get
			{
				return (Align)this.m_Styles.alignContent.value;
			}
			set
			{
				base.EnsureInlineStyles();
				this.m_Styles.alignContent = Style<int>.Create((int)value);
				base.cssNode.AlignContent = (CSSAlign)value;
			}
		}

		public Justify justifyContent
		{
			get
			{
				return (Justify)this.m_Styles.justifyContent.value;
			}
			set
			{
				base.EnsureInlineStyles();
				this.m_Styles.justifyContent = Style<int>.Create((int)value);
				base.cssNode.JustifyContent = (CSSJustify)value;
			}
		}

		public Wrap flexWrap
		{
			get
			{
				return (Wrap)this.m_Styles.flexWrap.value;
			}
			set
			{
				base.EnsureInlineStyles();
				this.m_Styles.flexWrap = Style<int>.Create((int)value);
				base.cssNode.Wrap = (CSSWrap)value;
			}
		}

		public VisualContainer()
		{
			this.clipChildren = true;
			base.cssNode.SetMeasureFunction(null);
		}

		public VisualContainer.Enumerator GetEnumerator()
		{
			return new VisualContainer.Enumerator(ref this.m_Children);
		}

		IEnumerator<VisualElement> IEnumerable<VisualElement>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal override void ChangePanel(BaseVisualElementPanel p)
		{
			if (p != base.panel)
			{
				base.ChangePanel(p);
				this.LoadStyleSheetsFromPaths();
				for (int i = 0; i < this.m_Children.Count; i++)
				{
					this.m_Children[i].ChangePanel(p);
				}
			}
		}

		public void AddChild(VisualElement child)
		{
			if (child == null)
			{
				throw new ArgumentException("Cannot add null child");
			}
			if (child.parent != null)
			{
				child.parent.RemoveChild(child);
			}
			child.parent = this;
			this.m_Children.Add(child);
			base.cssNode.Insert(base.cssNode.Count, child.cssNode);
			child.Dirty(ChangeType.Styles);
			child.PropagateChangesToParents();
		}

		public void InsertChild(int index, VisualElement child)
		{
			if (child == null)
			{
				throw new ArgumentException("Cannot insert null child");
			}
			if (index > this.m_Children.Count)
			{
				throw new IndexOutOfRangeException("Index out of range: " + index);
			}
			if (child.parent != null)
			{
				child.parent.RemoveChild(child);
			}
			child.parent = this;
			this.m_Children.Insert(index, child);
			base.cssNode.Insert(index, child.cssNode);
			child.Dirty(ChangeType.Styles);
			child.PropagateChangesToParents();
		}

		public void RemoveChild(VisualElement child)
		{
			if (child == null)
			{
				throw new ArgumentException("Cannot add null child");
			}
			if (child.parent != this)
			{
				throw new ArgumentException("This visualElement is not my child");
			}
			child.parent = null;
			this.m_Children.Remove(child);
			base.cssNode.RemoveAt(base.cssNode.IndexOf(child.cssNode));
		}

		public void RemoveChildAt(int index)
		{
			if (index < 0 || index >= this.m_Children.Count)
			{
				throw new IndexOutOfRangeException("Index out of range: " + index);
			}
			VisualElement visualElement = this.m_Children[index];
			visualElement.parent = null;
			this.m_Children.RemoveAt(index);
			base.cssNode.RemoveAt(index);
		}

		public void ClearChildren()
		{
			for (int i = 0; i < this.m_Children.Count; i++)
			{
				this.m_Children[i].parent = null;
			}
			this.m_Children.Clear();
			base.cssNode.Clear();
		}

		public VisualElement GetChildAt(int index)
		{
			return this.m_Children[index];
		}

		public bool ContainsChild(VisualElement elem)
		{
			bool result;
			for (int i = 0; i < this.m_Children.Count; i++)
			{
				if (this.m_Children[i] == elem)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public void AddStyleSheetPath(string sheetPath)
		{
			if (this.m_StyleSheetPaths == null)
			{
				this.m_StyleSheetPaths = new List<string>();
			}
			this.m_StyleSheetPaths.Add(sheetPath);
			base.Dirty(ChangeType.Styles);
		}

		public void RemoveStyleSheetPath(string sheetPath)
		{
			if (this.m_StyleSheetPaths == null)
			{
				Debug.LogWarning("Attempting to remove from null style shee path list");
			}
			else
			{
				this.m_StyleSheetPaths.Remove(sheetPath);
				base.Dirty(ChangeType.Styles);
			}
		}

		public bool HasStyleSheetPath(string sheetPath)
		{
			return this.m_StyleSheetPaths != null && this.m_StyleSheetPaths.Contains(sheetPath);
		}

		internal void LoadStyleSheetsFromPaths()
		{
			if (this.m_StyleSheetPaths != null && base.elementPanel != null)
			{
				this.m_StyleSheets = new List<StyleSheet>();
				foreach (string current in this.m_StyleSheetPaths)
				{
					StyleSheet styleSheet = base.elementPanel.loadResourceFunc(current, typeof(StyleSheet)) as StyleSheet;
					if (styleSheet != null)
					{
						int i = 0;
						int num = styleSheet.complexSelectors.Length;
						while (i < num)
						{
							styleSheet.complexSelectors[i].CachePseudoStateMasks();
							i++;
						}
						this.m_StyleSheets.Add(styleSheet);
					}
					else
					{
						Debug.LogWarning(string.Format("Style sheet not found for path \"{0}\"", current));
					}
				}
			}
		}
	}
}
