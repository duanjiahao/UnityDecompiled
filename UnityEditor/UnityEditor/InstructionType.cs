using System;

namespace UnityEditor
{
	internal enum InstructionType
	{
		kStyleDraw = 1,
		kClipPush,
		kClipPop,
		kLayoutBeginGroup,
		kLayoutEndGroup,
		kLayoutEntry
	}
}
