using System;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor.Experimental.UIElements
{
	public static class UIElementsEntryPoint
	{
		public static VisualContainer GetRootVisualContainer(this EditorWindow window)
		{
			return window.rootVisualContainer;
		}
	}
}
