using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	public sealed class GraphicsSettings : UnityEngine.Object
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShaderMode(BuiltinShaderType type, BuiltinShaderMode mode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern BuiltinShaderMode GetShaderMode(BuiltinShaderType type);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCustomShader(BuiltinShaderType type, Shader shader);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Shader GetCustomShader(BuiltinShaderType type);
	}
}
