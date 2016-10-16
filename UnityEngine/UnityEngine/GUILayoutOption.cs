using System;

namespace UnityEngine
{
	public sealed class GUILayoutOption
	{
		internal enum Type
		{
			fixedWidth,
			fixedHeight,
			minWidth,
			maxWidth,
			minHeight,
			maxHeight,
			stretchWidth,
			stretchHeight,
			alignStart,
			alignMiddle,
			alignEnd,
			alignJustify,
			equalSize,
			spacing
		}

		internal GUILayoutOption.Type type;

		internal object value;

		internal GUILayoutOption(GUILayoutOption.Type type, object value)
		{
			this.type = type;
			this.value = value;
		}
	}
}
