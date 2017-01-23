using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class UnifiedInspectView : BaseInspectView
	{
		private readonly List<IMGUIInstruction> m_Instructions = new List<IMGUIInstruction>();

		private Vector2 m_StacktraceScrollPos = default(Vector2);

		private BaseInspectView m_InstructionClipView;

		private BaseInspectView m_InstructionStyleView;

		private BaseInspectView m_InstructionLayoutView;

		public UnifiedInspectView(GUIViewDebuggerWindow guiViewDebuggerWindow) : base(guiViewDebuggerWindow)
		{
			this.m_InstructionClipView = new GUIClipInspectView(guiViewDebuggerWindow);
			this.m_InstructionStyleView = new StyleDrawInspectView(guiViewDebuggerWindow);
			this.m_InstructionLayoutView = new GUILayoutInspectView(guiViewDebuggerWindow);
		}

		protected BaseInspectView GetInspectViewForType(InstructionType type)
		{
			BaseInspectView result;
			switch (type)
			{
			case InstructionType.kStyleDraw:
				result = this.m_InstructionStyleView;
				break;
			case InstructionType.kClipPush:
			case InstructionType.kClipPop:
				result = this.m_InstructionClipView;
				break;
			case InstructionType.kLayoutBeginGroup:
			case InstructionType.kLayoutEndGroup:
			case InstructionType.kLayoutEntry:
				result = this.m_InstructionLayoutView;
				break;
			default:
				throw new NotImplementedException("Unhandled InstructionType");
			}
			return result;
		}

		public override void UpdateInstructions()
		{
			this.m_InstructionClipView.UpdateInstructions();
			this.m_InstructionStyleView.UpdateInstructions();
			this.m_InstructionLayoutView.UpdateInstructions();
			this.m_Instructions.Clear();
			GUIViewDebuggerHelper.GetUnifiedInstructions(this.m_Instructions);
		}

		protected override int GetInstructionCount()
		{
			return this.m_Instructions.Count;
		}

		protected override void DoDrawInstruction(ListViewElement el, int controlId)
		{
			IMGUIInstruction iMGUIInstruction = this.m_Instructions[el.row];
			string instructionListName = this.GetInstructionListName(el.row);
			GUIContent content = GUIContent.Temp(instructionListName);
			Rect position = el.position;
			position.xMin += (float)(iMGUIInstruction.level * 10);
			GUIViewDebuggerWindow.s_Styles.listItemBackground.Draw(position, false, false, this.m_ListViewState.row == el.row, false);
			GUIViewDebuggerWindow.s_Styles.listItem.Draw(position, content, controlId, this.m_ListViewState.row == el.row);
		}

		internal override string GetInstructionListName(int index)
		{
			IMGUIInstruction iMGUIInstruction = this.m_Instructions[index];
			BaseInspectView inspectViewForType = this.GetInspectViewForType(iMGUIInstruction.type);
			return inspectViewForType.GetInstructionListName(iMGUIInstruction.typeInstructionIndex);
		}

		internal override void OnDoubleClickInstruction(int index)
		{
			IMGUIInstruction iMGUIInstruction = this.m_Instructions[index];
			BaseInspectView inspectViewForType = this.GetInspectViewForType(iMGUIInstruction.type);
			inspectViewForType.OnDoubleClickInstruction(iMGUIInstruction.typeInstructionIndex);
		}

		protected override void DrawInspectedStacktrace()
		{
			IMGUIInstruction iMGUIInstruction = this.m_Instructions[this.m_ListViewState.row];
			this.m_StacktraceScrollPos = EditorGUILayout.BeginScrollView(this.m_StacktraceScrollPos, GUIViewDebuggerWindow.s_Styles.stacktraceBackground, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(false)
			});
			base.DrawStackFrameList(iMGUIInstruction.stack);
			EditorGUILayout.EndScrollView();
		}

		internal override void DoDrawSelectedInstructionDetails(int index)
		{
			IMGUIInstruction iMGUIInstruction = this.m_Instructions[index];
			BaseInspectView inspectViewForType = this.GetInspectViewForType(iMGUIInstruction.type);
			inspectViewForType.DoDrawSelectedInstructionDetails(iMGUIInstruction.typeInstructionIndex);
		}

		internal override void OnSelectedInstructionChanged(int index)
		{
			this.m_ListViewState.row = index;
			IMGUIInstruction iMGUIInstruction = this.m_Instructions[this.m_ListViewState.row];
			BaseInspectView inspectViewForType = this.GetInspectViewForType(iMGUIInstruction.type);
			inspectViewForType.OnSelectedInstructionChanged(iMGUIInstruction.typeInstructionIndex);
			this.ShowOverlay();
		}

		public override void ShowOverlay()
		{
			if (this.HasSelectedinstruction())
			{
				BaseInspectView inspectViewForType = this.GetInspectViewForType(this.m_Instructions[this.m_ListViewState.row].type);
				inspectViewForType.ShowOverlay();
			}
		}
	}
}
