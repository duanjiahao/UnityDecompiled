using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditor
{
	public sealed class AudioImporter : AssetImporter
	{
		public extern AudioImporterFormat format
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("Setting and getting import channels is not used anymore (use forceToMono instead)", true)]
		public AudioImporterChannels channels
		{
			get
			{
				return AudioImporterChannels.Automatic;
			}
			set
			{
			}
		}
		public extern int compressionBitrate
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("Setting/Getting decompressOnLoad is deprecated. Use AudioImporter.loadType instead.", true)]
		private bool decompressOnLoad
		{
			get
			{
				return this.loadType == AudioImporterLoadType.DecompressOnLoad;
			}
			set
			{
				this.loadType = ((!value) ? AudioImporterLoadType.CompressedInMemory : AudioImporterLoadType.DecompressOnLoad);
			}
		}
		public extern bool threeD
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool forceToMono
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool hardware
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public AudioImporterLoadType loadType
		{
			get
			{
				return (AudioImporterLoadType)this.Internal_GetLoadType();
			}
			set
			{
				this.Internal_SetLoadType((int)value);
			}
		}
		public extern bool loopable
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		internal extern int durationMS
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern int frequency
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern int origChannelCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern bool origIsCompressible
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern bool origIsMonoForcable
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern int defaultBitrate
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal AudioType origType
		{
			get
			{
				return (AudioType)this.Internal_GetType();
			}
		}
		internal extern int origFileSize
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetLoadType(int flag);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetLoadType();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void updateOrigData();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetMinBitrate(int type);
		internal int minBitrate(AudioType type)
		{
			return this.Internal_GetMinBitrate((int)type);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetMaxBitrate(int type);
		internal int maxBitrate(AudioType type)
		{
			return this.Internal_GetMaxBitrate((int)type);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetType();
	}
}
