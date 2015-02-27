using System;
namespace UnityEditorInternal
{
	internal interface IProfilerWindowController
	{
		void SetSelectedPropertyPath(string path);
		void ClearSelectedPropertyPath();
		ProfilerProperty CreateProperty(bool details);
		void Repaint();
	}
}
