using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
namespace UnityEngine
{
	public sealed class Debug
	{
		public static extern bool developerConsoleVisible
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern bool isDebugBuild
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
		{
			Debug.INTERNAL_CALL_DrawLine(ref start, ref end, ref color, duration, depthTest);
		}
		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
		{
			bool depthTest = true;
			Debug.INTERNAL_CALL_DrawLine(ref start, ref end, ref color, duration, depthTest);
		}
		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end, Color color)
		{
			bool depthTest = true;
			float duration = 0f;
			Debug.INTERNAL_CALL_DrawLine(ref start, ref end, ref color, duration, depthTest);
		}
		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end)
		{
			bool depthTest = true;
			float duration = 0f;
			Color white = Color.white;
			Debug.INTERNAL_CALL_DrawLine(ref start, ref end, ref white, duration, depthTest);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DrawLine(ref Vector3 start, ref Vector3 end, ref Color color, float duration, bool depthTest);
		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
		{
			bool depthTest = true;
			Debug.DrawRay(start, dir, color, duration, depthTest);
		}
		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color)
		{
			bool depthTest = true;
			float duration = 0f;
			Debug.DrawRay(start, dir, color, duration, depthTest);
		}
		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir)
		{
			bool depthTest = true;
			float duration = 0f;
			Color white = Color.white;
			Debug.DrawRay(start, dir, white, duration, depthTest);
		}
		public static void DrawRay(Vector3 start, Vector3 dir, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
		{
			Debug.DrawLine(start, start + dir, color, duration, depthTest);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Break();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DebugBreak();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Log(int level, string msg, [Writable] Object obj);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_LogException(Exception exception, [Writable] Object obj);
		public static void Log(object message)
		{
			Debug.Internal_Log(0, (message == null) ? "Null" : message.ToString(), null);
		}
		public static void Log(object message, Object context)
		{
			Debug.Internal_Log(0, (message == null) ? "Null" : message.ToString(), context);
		}
		public static void LogFormat(string format, params object[] args)
		{
			Debug.Log(string.Format(format, args));
		}
		public static void LogFormat(Object context, string format, params object[] args)
		{
			Debug.Log(string.Format(format, args), context);
		}
		public static void LogError(object message)
		{
			Debug.Internal_Log(2, (message == null) ? "Null" : message.ToString(), null);
		}
		public static void LogError(object message, Object context)
		{
			Debug.Internal_Log(2, message.ToString(), context);
		}
		public static void LogErrorFormat(string format, params object[] args)
		{
			Debug.LogError(string.Format(format, args));
		}
		public static void LogErrorFormat(Object context, string format, params object[] args)
		{
			Debug.LogError(string.Format(format, args), context);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearDeveloperConsole();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void WriteLineToLogFile(string message);
		public static void LogException(Exception exception)
		{
			Debug.Internal_LogException(exception, null);
		}
		public static void LogException(Exception exception, Object context)
		{
			Debug.Internal_LogException(exception, context);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void LogPlayerBuildError(string message, string file, int line, int column);
		public static void LogWarning(object message)
		{
			Debug.Internal_Log(1, message.ToString(), null);
		}
		public static void LogWarning(object message, Object context)
		{
			Debug.Internal_Log(1, message.ToString(), context);
		}
		public static void LogWarningFormat(string format, params object[] args)
		{
			Debug.LogWarning(string.Format(format, args));
		}
		public static void LogWarningFormat(Object context, string format, params object[] args)
		{
			Debug.LogWarning(string.Format(format, args), context);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenConsoleFile();
	}
}
