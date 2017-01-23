using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class GUILayoutInspectView : BaseInspectView
	{
		private Vector2 m_StacktraceScrollPos = default(Vector2);

		private readonly List<IMGUILayoutInstruction> m_LayoutInstructions = new List<IMGUILayoutInstruction>();

		private GUIStyle m_FakeMargingStyleForOverlay = new GUIStyle();

		public GUILayoutInspectView(GUIViewDebuggerWindow guiViewDebuggerWindow) : base(guiViewDebuggerWindow)
		{
		}

		public override void UpdateInstructions()
		{
			this.m_LayoutInstructions.Clear();
			GUIViewDebuggerHelper.GetLayoutInstructions(this.m_LayoutInstructions);
		}

		protected override int GetInstructionCount()
		{
			return this.m_LayoutInstructions.Count;
		}

		protected override void DoDrawInstruction(ListViewElement el, int id)
		{
			IMGUILayoutInstruction iMGUILayoutInstruction = this.m_LayoutInstructions[el.row];
			GUIContent content = GUIContent.Temp(this.GetInstructionListName(el.row));
			Rect position = el.position;
			position.xMin += (float)(iMGUILayoutInstruction.level * 10);
			GUIViewDebuggerWindow.s_Styles.listItemBackground.Draw(position, false, false, this.m_ListViewState.row == el.row, false);
			GUIViewDebuggerWindow.s_Styles.listItem.Draw(position, content, id, this.m_ListViewState.row == el.row);
		}

		internal override string GetInstructionListName(int index)
		{
			StackFrame[] stack = this.m_LayoutInstructions[index].stack;
			int num = this.GetInterestingFrameIndex(stack);
			if (num > 0)
			{
				num--;
			}
			StackFrame stackFrame = stack[num];
			return stackFrame.methodName;
		}

		private int GetInterestingFrameIndex(StackFrame[] stacktrace)
		{
			string dataPath = Application.dataPath;
			int num = -1;
			int result;
			for (int i = 0; i < stacktrace.Length; i++)
			{
				StackFrame stackFrame = stacktrace[i];
				if (!string.IsNullOrEmpty(stackFrame.sourceFile))
				{
					if (!stackFrame.signature.StartsWith("UnityEngine.GUIDebugger"))
					{
						if (!stackFrame.signature.StartsWith("UnityEngine.GUILayoutUtility"))
						{
							if (num == -1)
							{
								num = i;
							}
							if (stackFrame.sourceFile.StartsWith(dataPath))
							{
								result = i;
								return result;
							}
						}
					}
				}
			}
			if (num != -1)
			{
				result = num;
				return result;
			}
			result = stacktrace.Length - 1;
			return result;
		}

		internal override void OnDoubleClickInstruction(int index)
		{
			throw new NotImplementedException();
		}

		protected override void DrawInspectedStacktrace()
		{
			IMGUILayoutInstruction iMGUILayoutInstruction = this.m_LayoutInstructions[this.m_ListViewState.row];
			this.m_StacktraceScrollPos = EditorGUILayout.BeginScrollView(this.m_StacktraceScrollPos, GUIViewDebuggerWindow.s_Styles.stacktraceBackground, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(false)
			});
			base.DrawStackFrameList(iMGUILayoutInstruction.stack);
			EditorGUILayout.EndScrollView();
		}

		internal override void DoDrawSelectedInstructionDetails(int index)
		{
			IMGUILayoutInstruction iMGUILayoutInstruction = this.m_LayoutInstructions[index];
			using (new EditorGUI.DisabledScope(true))
			{
				base.DrawInspectedRect(iMGUILayoutInstruction.unclippedRect);
				EditorGUILayout.IntField("margin.left", iMGUILayoutInstruction.marginLeft, new GUILayoutOption[0]);
				EditorGUILayout.IntField("margin.top", iMGUILayoutInstruction.marginTop, new GUILayoutOption[0]);
				EditorGUILayout.IntField("margin.right", iMGUILayoutInstruction.marginRight, new GUILayoutOption[0]);
				EditorGUILayout.IntField("margin.bottom", iMGUILayoutInstruction.marginBottom, new GUILayoutOption[0]);
				if (iMGUILayoutInstruction.style != null)
				{
					EditorGUILayout.LabelField("Style Name", iMGUILayoutInstruction.style.name, new GUILayoutOption[0]);
				}
				if (iMGUILayoutInstruction.isGroup != 1)
				{
					EditorGUILayout.Toggle("IsVertical", iMGUILayoutInstruction.isVertical == 1, new GUILayoutOption[0]);
				}
			}
		}

		internal override void OnSelectedInstructionChanged(int index)
		{
			this.m_ListViewState.row = index;
			this.ShowOverlay();
		}

		public override void ShowOverlay()
		{
			if (this.HasSelectedinstruction())
			{
				IMGUILayoutInstruction iMGUILayoutInstruction = this.m_LayoutInstructions[this.m_ListViewState.row];
				RectOffset rectOffset = new RectOffset();
				rectOffset.left = iMGUILayoutInstruction.marginLeft;
				rectOffset.right = iMGUILayoutInstruction.marginRight;
				rectOffset.top = iMGUILayoutInstruction.marginTop;
				rectOffset.bottom = iMGUILayoutInstruction.marginBottom;
				this.m_FakeMargingStyleForOverlay.padding = rectOffset;
				Rect rect = iMGUILayoutInstruction.unclippedRect;
				rect = rectOffset.Add(rect);
				this.m_GuiViewDebuggerWindow.HighlightInstruction(this.m_GuiViewDebuggerWindow.m_Inspected, rect, this.m_FakeMargingStyleForOverlay);
			}
		}
	}
}
