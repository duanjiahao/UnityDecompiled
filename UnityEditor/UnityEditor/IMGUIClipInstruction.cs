using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[RequiredByNativeCode]
	internal struct IMGUIClipInstruction
	{
		public Rect screenRect;

		public Rect unclippedScreenRect;

		public Vector2 scrollOffset;

		public Vector2 renderOffset;

		public bool resetOffset;

		public int level;

		public StackFrame[] pushStacktrace;

		public StackFrame[] popStacktrace;
	}
}
