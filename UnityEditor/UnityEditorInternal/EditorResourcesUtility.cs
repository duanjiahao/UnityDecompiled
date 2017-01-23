using System;
using System.Runtime.CompilerServices;

namespace UnityEditorInternal
{
	internal sealed class EditorResourcesUtility
	{
		public static extern string lightSkinSourcePath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string darkSkinSourcePath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string fontsPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string brushesPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string iconsPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string generatedIconsPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string folderIconName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string emptyFolderIconName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
