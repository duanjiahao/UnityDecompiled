using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class AudioClip : Object
	{
		public delegate void PCMReaderCallback(float[] data);

		public delegate void PCMSetPositionCallback(int position);

		private event AudioClip.PCMReaderCallback m_PCMReaderCallback
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.m_PCMReaderCallback = (AudioClip.PCMReaderCallback)Delegate.Combine(this.m_PCMReaderCallback, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.m_PCMReaderCallback = (AudioClip.PCMReaderCallback)Delegate.Remove(this.m_PCMReaderCallback, value);
			}
		}

		private event AudioClip.PCMSetPositionCallback m_PCMSetPositionCallback
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.m_PCMSetPositionCallback = (AudioClip.PCMSetPositionCallback)Delegate.Combine(this.m_PCMSetPositionCallback, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.m_PCMSetPositionCallback = (AudioClip.PCMSetPositionCallback)Delegate.Remove(this.m_PCMSetPositionCallback, value);
			}
		}

		public extern float length
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int samples
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int channels
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int frequency
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Use AudioClip.loadState instead to get more detailed information about the loading process.")]
		public extern bool isReadyToPlay
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern AudioClipLoadType loadType
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool preloadAudioData
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern AudioDataLoadState loadState
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool loadInBackground
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool LoadAudioData();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool UnloadAudioData();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetData(float[] data, int offsetSamples);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetData(float[] data, int offsetSamples);

		[Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream)
		{
			return AudioClip.Create(name, lengthSamples, channels, frequency, stream);
		}

		[Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream, AudioClip.PCMReaderCallback pcmreadercallback)
		{
			return AudioClip.Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, null);
		}

		[Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream, AudioClip.PCMReaderCallback pcmreadercallback, AudioClip.PCMSetPositionCallback pcmsetpositioncallback)
		{
			return AudioClip.Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, pcmsetpositioncallback);
		}

		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream)
		{
			return AudioClip.Create(name, lengthSamples, channels, frequency, stream, null, null);
		}

		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream, AudioClip.PCMReaderCallback pcmreadercallback)
		{
			return AudioClip.Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, null);
		}

		public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream, AudioClip.PCMReaderCallback pcmreadercallback, AudioClip.PCMSetPositionCallback pcmsetpositioncallback)
		{
			if (name == null)
			{
				throw new NullReferenceException();
			}
			if (lengthSamples <= 0)
			{
				throw new ArgumentException("Length of created clip must be larger than 0");
			}
			if (channels <= 0)
			{
				throw new ArgumentException("Number of channels in created clip must be greater than 0");
			}
			if (frequency <= 0)
			{
				throw new ArgumentException("Frequency in created clip must be greater than 0");
			}
			AudioClip audioClip = AudioClip.Construct_Internal();
			if (pcmreadercallback != null)
			{
				AudioClip expr_50 = audioClip;
				expr_50.m_PCMReaderCallback = (AudioClip.PCMReaderCallback)Delegate.Combine(expr_50.m_PCMReaderCallback, pcmreadercallback);
			}
			if (pcmsetpositioncallback != null)
			{
				AudioClip expr_6F = audioClip;
				expr_6F.m_PCMSetPositionCallback = (AudioClip.PCMSetPositionCallback)Delegate.Combine(expr_6F.m_PCMSetPositionCallback, pcmsetpositioncallback);
			}
			audioClip.Init_Internal(name, lengthSamples, channels, frequency, stream);
			return audioClip;
		}

		[RequiredByNativeCode]
		private void InvokePCMReaderCallback_Internal(float[] data)
		{
			if (this.m_PCMReaderCallback != null)
			{
				this.m_PCMReaderCallback(data);
			}
		}

		[RequiredByNativeCode]
		private void InvokePCMSetPositionCallback_Internal(int position)
		{
			if (this.m_PCMSetPositionCallback != null)
			{
				this.m_PCMSetPositionCallback(position);
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AudioClip Construct_Internal();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Init_Internal(string name, int lengthSamples, int channels, int frequency, bool stream);
	}
}
