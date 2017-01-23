using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class GL
	{
		public const int TRIANGLES = 4;

		public const int TRIANGLE_STRIP = 5;

		public const int QUADS = 7;

		public const int LINES = 1;

		public static Matrix4x4 modelview
		{
			get
			{
				Matrix4x4 result;
				GL.INTERNAL_get_modelview(out result);
				return result;
			}
			set
			{
				GL.INTERNAL_set_modelview(ref value);
			}
		}

		public static extern bool wireframe
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool sRGBWrite
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool invertCulling
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Vertex3(float x, float y, float z);

		public static void Vertex(Vector3 v)
		{
			GL.INTERNAL_CALL_Vertex(ref v);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Vertex(ref Vector3 v);

		public static void Color(Color c)
		{
			GL.INTERNAL_CALL_Color(ref c);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Color(ref Color c);

		public static void TexCoord(Vector3 v)
		{
			GL.INTERNAL_CALL_TexCoord(ref v);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_TexCoord(ref Vector3 v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void TexCoord2(float x, float y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void TexCoord3(float x, float y, float z);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MultiTexCoord2(int unit, float x, float y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MultiTexCoord3(int unit, float x, float y, float z);

		public static void MultiTexCoord(int unit, Vector3 v)
		{
			GL.INTERNAL_CALL_MultiTexCoord(unit, ref v);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MultiTexCoord(int unit, ref Vector3 v);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginInternal(int mode);

		public static void Begin(int mode)
		{
			GL.BeginInternal(mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void End();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadOrtho();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadPixelMatrix();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LoadPixelMatrixArgs(float left, float right, float bottom, float top);

		public static void LoadPixelMatrix(float left, float right, float bottom, float top)
		{
			GL.LoadPixelMatrixArgs(left, right, bottom, top);
		}

		public static void Viewport(Rect pixelRect)
		{
			GL.INTERNAL_CALL_Viewport(ref pixelRect);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Viewport(ref Rect pixelRect);

		public static void LoadProjectionMatrix(Matrix4x4 mat)
		{
			GL.INTERNAL_CALL_LoadProjectionMatrix(ref mat);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_LoadProjectionMatrix(ref Matrix4x4 mat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadIdentity();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_modelview(out Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_modelview(ref Matrix4x4 value);

		public static void MultMatrix(Matrix4x4 mat)
		{
			GL.INTERNAL_CALL_MultMatrix(ref mat);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MultMatrix(ref Matrix4x4 mat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PushMatrix();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PopMatrix();

		public static Matrix4x4 GetGPUProjectionMatrix(Matrix4x4 proj, bool renderIntoTexture)
		{
			Matrix4x4 result;
			GL.INTERNAL_CALL_GetGPUProjectionMatrix(ref proj, renderIntoTexture, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetGPUProjectionMatrix(ref Matrix4x4 proj, bool renderIntoTexture, out Matrix4x4 value);

		[Obsolete("Use invertCulling property")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetRevertBackfacing(bool revertBackFaces);

		[ExcludeFromDocs]
		public static void Clear(bool clearDepth, bool clearColor, Color backgroundColor)
		{
			float depth = 1f;
			GL.Clear(clearDepth, clearColor, backgroundColor, depth);
		}

		public static void Clear(bool clearDepth, bool clearColor, Color backgroundColor, [DefaultValue("1.0f")] float depth)
		{
			GL.Internal_Clear(clearDepth, clearColor, backgroundColor, depth);
		}

		private static void Internal_Clear(bool clearDepth, bool clearColor, Color backgroundColor, float depth)
		{
			GL.INTERNAL_CALL_Internal_Clear(clearDepth, clearColor, ref backgroundColor, depth);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_Clear(bool clearDepth, bool clearColor, ref Color backgroundColor, float depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearWithSkybox(bool clearDepth, Camera camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Flush();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void InvalidateState();

		[Obsolete("IssuePluginEvent(eventID) is deprecated. Use IssuePluginEvent(callback, eventID) instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void IssuePluginEvent(int eventID);

		public static void IssuePluginEvent(IntPtr callback, int eventID)
		{
			if (callback == IntPtr.Zero)
			{
				throw new ArgumentException("Null callback specified.");
			}
			GL.IssuePluginEventInternal(callback, eventID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IssuePluginEventInternal(IntPtr callback, int eventID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RenderTargetBarrier();
	}
}
