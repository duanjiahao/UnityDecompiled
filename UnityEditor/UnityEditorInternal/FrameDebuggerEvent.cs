using System;
namespace UnityEditorInternal
{
	internal struct FrameDebuggerEvent
	{
		public FrameEventType type;
		public int vertexCount;
		public int indexCount;
		public string rtName;
		public int rtWidth;
		public int rtHeight;
		public int rtFormat;
		public int rtDim;
		public int rtFace;
		public short rtCount;
		public short rtHasDepthTexture;
	}
}
