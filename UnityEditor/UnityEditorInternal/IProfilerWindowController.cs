using System;

namespace UnityEditorInternal
{
	internal interface IProfilerWindowController
	{
		void SetSelectedPropertyPath(string path);

		void ClearSelectedPropertyPath();

		ProfilerProperty GetRootProfilerProperty(ProfilerColumn sortType);

		int GetActiveVisibleFrameIndex();

		void SetSearch(string searchString);

		string GetSearch();

		bool IsSearching();

		void Repaint();
	}
}
