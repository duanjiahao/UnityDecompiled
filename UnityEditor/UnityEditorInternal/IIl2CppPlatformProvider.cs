using System;
using UnityEditor;
namespace UnityEditorInternal
{
	internal interface IIl2CppPlatformProvider
	{
		BuildTarget target
		{
			get;
		}
		bool emitNullChecks
		{
			get;
		}
		bool enableStackTraces
		{
			get;
		}
		bool enableArrayBoundsCheck
		{
			get;
		}
		bool compactMode
		{
			get;
		}
		bool loadSymbols
		{
			get;
		}
		string nativeLibraryFileName
		{
			get;
		}
		string il2CppFolder
		{
			get;
		}
		bool usePrecompiledHeader
		{
			get;
		}
		string[] includePaths
		{
			get;
		}
		string[] libraryPaths
		{
			get;
		}
		INativeCompiler CreateNativeCompiler();
	}
}
