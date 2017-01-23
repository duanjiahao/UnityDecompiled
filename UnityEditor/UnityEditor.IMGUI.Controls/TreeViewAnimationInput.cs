using System;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
	internal class TreeViewAnimationInput
	{
		public Action<TreeViewAnimationInput> animationEnded;

		public float elapsedTimeNormalized
		{
			get
			{
				return Mathf.Clamp01((float)this.elapsedTime / (float)this.animationDuration);
			}
		}

		public double elapsedTime
		{
			get
			{
				return EditorApplication.timeSinceStartup - this.startTime;
			}
			set
			{
				this.startTime = EditorApplication.timeSinceStartup - value;
			}
		}

		public int startRow
		{
			get;
			set;
		}

		public int endRow
		{
			get;
			set;
		}

		public Rect rowsRect
		{
			get;
			set;
		}

		public Rect startRowRect
		{
			get;
			set;
		}

		public double startTime
		{
			get;
			set;
		}

		public double animationDuration
		{
			get;
			set;
		}

		public bool expanding
		{
			get;
			set;
		}

		public TreeViewItem item
		{
			get;
			set;
		}

		public TreeViewController treeView
		{
			get;
			set;
		}

		public void FireAnimationEndedEvent()
		{
			if (this.animationEnded != null)
			{
				this.animationEnded(this);
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Input: startRow ",
				this.startRow,
				" endRow ",
				this.endRow,
				" rowsRect ",
				this.rowsRect,
				" startTime ",
				this.startTime,
				" anitmationDuration",
				this.animationDuration,
				" ",
				this.expanding,
				" ",
				this.item.displayName
			});
		}
	}
}
