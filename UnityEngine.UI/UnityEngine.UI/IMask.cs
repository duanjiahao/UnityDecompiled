using System;

namespace UnityEngine.UI
{
	[Obsolete("Not supported anymore.", true)]
	public interface IMask
	{
		RectTransform rectTransform
		{
			get;
		}

		bool Enabled();
	}
}
