using System;
using UnityEngine;

namespace UnityEditor
{
	internal class MemoryTreeListClickable : MemoryTreeList
	{
		public MemoryTreeListClickable(EditorWindow editorWindow, MemoryTreeList detailview) : base(editorWindow, detailview)
		{
		}

		protected override void SetupSplitter()
		{
			float[] array = new float[3];
			int[] array2 = new int[3];
			array[0] = 300f;
			array2[0] = 100;
			array[1] = 70f;
			array2[1] = 50;
			array[2] = 70f;
			array2[2] = 50;
			this.m_Splitter = new SplitterState(array, array2, null);
		}

		protected override void DrawHeader()
		{
			GUILayout.Label("Name", MemoryTreeList.styles.header, new GUILayoutOption[0]);
			GUILayout.Label("Memory", MemoryTreeList.styles.header, new GUILayoutOption[0]);
			GUILayout.Label("Ref count", MemoryTreeList.styles.header, new GUILayoutOption[0]);
		}

		protected override void DrawData(Rect rect, MemoryElement memoryElement, int indent, int row, bool selected)
		{
			if (Event.current.type == EventType.Repaint)
			{
				string text = memoryElement.name;
				if (memoryElement.ChildCount() > 0 && indent < 3)
				{
					text = text + " (" + memoryElement.AccumulatedChildCount().ToString() + ")";
				}
				int num = 0;
				rect.xMax = (float)this.m_Splitter.realSizes[num];
				MemoryTreeList.styles.numberLabel.Draw(rect, text, false, false, false, selected);
				rect.x = rect.xMax;
				rect.width = (float)this.m_Splitter.realSizes[++num] - 4f;
				MemoryTreeList.styles.numberLabel.Draw(rect, EditorUtility.FormatBytes(memoryElement.totalMemory), false, false, false, selected);
				rect.x += (float)this.m_Splitter.realSizes[num++];
				rect.width = (float)this.m_Splitter.realSizes[num] - 4f;
				if (memoryElement.ReferenceCount() > 0)
				{
					MemoryTreeList.styles.numberLabel.Draw(rect, memoryElement.ReferenceCount().ToString(), false, false, false, selected);
				}
				else if (selected)
				{
					MemoryTreeList.styles.numberLabel.Draw(rect, "", false, false, false, selected);
				}
			}
		}
	}
}
