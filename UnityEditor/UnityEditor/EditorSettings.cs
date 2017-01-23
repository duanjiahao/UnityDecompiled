using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.VisualStudioIntegration;
using UnityEngine;

namespace UnityEditor
{
	public sealed class EditorSettings : UnityEngine.Object
	{
		public static extern string unityRemoteDevice
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string unityRemoteCompression
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string unityRemoteResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string unityRemoteJoystickSource
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string externalVersionControl
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern SerializationMode serializationMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("EditorSettings.webSecurityEmulationEnabled is no longer supported, since the Unity Web Player is no longer supported by Unity.")]
		public static extern bool webSecurityEmulationEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("EditorSettings.webSecurityEmulationHostUrl is no longer supported, since the Unity Web Player is no longer supported by Unity.")]
		public static extern string webSecurityEmulationHostUrl
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern EditorBehaviorMode defaultBehaviorMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern SpritePackerMode spritePackerMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int spritePackerPaddingPower
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static string[] projectGenerationUserExtensions
		{
			get
			{
				return (from s in EditorSettings.Internal_ProjectGenerationUserExtensions.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries)
				select s.TrimStart(new char[]
				{
					'.',
					'*'
				})).ToArray<string>();
			}
			set
			{
				EditorSettings.Internal_ProjectGenerationUserExtensions = string.Join(";", value);
			}
		}

		public static string[] projectGenerationBuiltinExtensions
		{
			get
			{
				return SolutionSynchronizer.BuiltinSupportedExtensions.Keys.ToArray<string>();
			}
		}

		internal static extern string Internal_ProjectGenerationUserExtensions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string projectGenerationRootNamespace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string Internal_UserGeneratedProjectSuffix
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
