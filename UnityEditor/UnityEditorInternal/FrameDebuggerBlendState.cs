using System;
using UnityEngine.Rendering;

namespace UnityEditorInternal
{
	internal struct FrameDebuggerBlendState
	{
		public uint writeMask;

		public BlendMode srcBlend;

		public BlendMode dstBlend;

		public BlendMode srcBlendAlpha;

		public BlendMode dstBlendAlpha;

		public BlendOp blendOp;

		public BlendOp blendOpAlpha;
	}
}
