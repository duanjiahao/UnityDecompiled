using System;

namespace UnityEditor
{
	internal interface IBaseInspectView
	{
		void UpdateInstructions();

		void DrawInstructionList();

		void DrawSelectedInstructionDetails();

		void Unselect();

		void SelectRow(int index);

		void ShowOverlay();
	}
}
