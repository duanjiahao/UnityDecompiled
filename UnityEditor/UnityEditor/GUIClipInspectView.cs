using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class GUIClipInspectView : BaseInspectView
	{
		private List<IMGUIClipInstruction> m_ClipList = new List<IMGUIClipInstruction>();

		private IMGUIClipInstruction m_Instruction;

		private Vector2 m_StacktraceScrollPos = default(Vector2);

		public GUIClipInspectView(GUIViewDebuggerWindow guiViewDebuggerWindow) : base(guiViewDebuggerWindow)
		{
		}

		public override void UpdateInstructions()
		{
			this.m_ClipList.Clear();
			GUIViewDebuggerHelper.GetClipInstructions(this.m_ClipList);
		}

		protected override int GetInstructionCount()
		{
			return this.m_ClipList.Count;
		}

		protected override void DoDrawInstruction(ListViewElement el, int id)
		{
			IMGUIClipInstruction iMGUIClipInstruction = this.m_ClipList[el.row];
			string instructionListName = this.GetInstructionListName(el.row);
			GUIContent content = GUIContent.Temp(instructionListName);
			Rect position = el.position;
			position.xMin += (float)(iMGUIClipInstruction.level * 12);
			GUIViewDebuggerWindow.s_Styles.listItemBackground.Draw(el.position, false, false, this.m_ListViewState.row == el.row, false);
			GUIViewDebuggerWindow.s_Styles.listItem.Draw(position, content, id, this.m_ListViewState.row == el.row);
		}

		internal override void OnDoubleClickInstruction(int index)
		{
			throw new NotImplementedException();
		}

		protected override void DrawInspectedStacktrace()
		{
			IMGUIClipInstruction iMGUIClipInstruction = this.m_ClipList[this.m_ListViewState.row];
			this.m_StacktraceScrollPos = EditorGUILayout.BeginScrollView(this.m_StacktraceScrollPos, GUIViewDebuggerWindow.s_Styles.stacktraceBackground, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(false)
			});
			base.DrawStackFrameList(iMGUIClipInstruction.pushStacktrace);
			EditorGUILayout.EndScrollView();
		}

		internal override void DoDrawSelectedInstructionDetails(int index)
		{
			IMGUIClipInstruction iMGUIClipInstruction = this.m_ClipList[index];
			GUILayout.Label("RenderOffset:", new GUILayoutOption[0]);
			GUILayout.Label(iMGUIClipInstruction.renderOffset.ToString(), new GUILayoutOption[0]);
			GUILayout.Label("ResetOffset:", new GUILayoutOption[0]);
			GUILayout.Label(iMGUIClipInstruction.resetOffset.ToString(), new GUILayoutOption[0]);
			GUILayout.Label("screenRect:", new GUILayoutOption[0]);
			GUILayout.Label(iMGUIClipInstruction.screenRect.ToString(), new GUILayoutOption[0]);
			GUILayout.Label("scrollOffset:", new GUILayoutOption[0]);
			GUILayout.Label(iMGUIClipInstruction.scrollOffset.ToString(), new GUILayoutOption[0]);
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
				IMGUIClipInstruction iMGUIClipInstruction = this.m_ClipList[this.m_ListViewState.row];
				this.m_GuiViewDebuggerWindow.HighlightInstruction(this.m_GuiViewDebuggerWindow.m_Inspected, iMGUIClipInstruction.unclippedScreenRect, GUIStyle.none);
			}
		}

		internal override string GetInstructionListName(int index)
		{
			StackFrame[] pushStacktrace = this.m_ClipList[index].pushStacktrace;
			string result;
			if (pushStacktrace.Length == 0)
			{
				result = "Empty";
			}
			else
			{
				int interestingFrameIndex = this.GetInterestingFrameIndex(pushStacktrace);
				StackFrame stackFrame = pushStacktrace[interestingFrameIndex];
				string methodName = stackFrame.methodName;
				result = methodName;
			}
			return result;
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
					if (!stackFrame.signature.StartsWith("UnityEngine.GUIClip"))
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
			if (num != -1)
			{
				result = num;
				return result;
			}
			result = stacktrace.Length - 1;
			return result;
		}
	}
}
