using System;
using UnityEngine;

namespace UnityEditor
{
	internal class DropInfo
	{
		internal enum Type
		{
			Tab,
			Pane,
			Window
		}

		public IDropArea dropArea;

		public object userData = null;

		public DropInfo.Type type = DropInfo.Type.Window;

		public Rect rect;

		public DropInfo(IDropArea source)
		{
			this.dropArea = source;
		}
	}
}
