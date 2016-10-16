using System;
using UnityEngine;

namespace UnityEditor.TreeViewTests
{
	internal class TreeViewColumnHeader
	{
		public float[] columnWidths
		{
			get;
			set;
		}

		public float minColumnWidth
		{
			get;
			set;
		}

		public float dragWidth
		{
			get;
			set;
		}

		public Action<int, Rect> columnRenderer
		{
			get;
			set;
		}

		public TreeViewColumnHeader()
		{
			this.minColumnWidth = 10f;
			this.dragWidth = 6f;
		}

		public void OnGUI(Rect rect)
		{
			float num = rect.x;
			for (int i = 0; i < this.columnWidths.Length; i++)
			{
				Rect arg = new Rect(num, rect.y, this.columnWidths[i], rect.height);
				num += this.columnWidths[i];
				Rect position = new Rect(num - this.dragWidth / 2f, rect.y, 3f, rect.height);
				float x = EditorGUI.MouseDeltaReader(position, true).x;
				if (x != 0f)
				{
					this.columnWidths[i] += x;
					this.columnWidths[i] = Mathf.Max(this.columnWidths[i], this.minColumnWidth);
				}
				if (this.columnRenderer != null)
				{
					this.columnRenderer(i, arg);
				}
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUIUtility.AddCursorRect(position, MouseCursor.SplitResizeLeftRight);
				}
			}
		}
	}
}
