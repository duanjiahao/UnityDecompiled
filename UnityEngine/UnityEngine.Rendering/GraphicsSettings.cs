using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	public sealed class GraphicsSettings : UnityEngine.Object
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShaderMode(BuiltinShaderType type, BuiltinShaderMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern BuiltinShaderMode GetShaderMode(BuiltinShaderType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCustomShader(BuiltinShaderType type, Shader shader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Shader GetCustomShader(BuiltinShaderType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern UnityEngine.Object GetGraphicsSettings();
	}
}
