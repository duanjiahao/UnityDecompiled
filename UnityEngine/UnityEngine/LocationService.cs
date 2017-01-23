using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class LocationService
	{
		public extern bool isEnabledByUser
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern LocationServiceStatus status
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern LocationInfo lastData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Start([DefaultValue("10f")] float desiredAccuracyInMeters, [DefaultValue("10f")] float updateDistanceInMeters);

		[ExcludeFromDocs]
		public void Start(float desiredAccuracyInMeters)
		{
			float updateDistanceInMeters = 10f;
			this.Start(desiredAccuracyInMeters, updateDistanceInMeters);
		}

		[ExcludeFromDocs]
		public void Start()
		{
			float updateDistanceInMeters = 10f;
			float desiredAccuracyInMeters = 10f;
			this.Start(desiredAccuracyInMeters, updateDistanceInMeters);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop();
	}
}
