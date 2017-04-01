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
				return this.timeCaptured - this.startTime;
			}
			set
			{
				this.startTime = this.timeCaptured - value;
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

		public double timeCaptured
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

		public bool includeChildren
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

		public TreeViewAnimationInput()
		{
			double timeSinceStartup = EditorApplication.timeSinceStartup;
			this.timeCaptured = timeSinceStartup;
			this.startTime = timeSinceStartup;
		}

		public void CaptureTime()
		{
			this.timeCaptured = EditorApplication.timeSinceStartup;
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
