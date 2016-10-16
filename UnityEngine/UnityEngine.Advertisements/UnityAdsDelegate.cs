using System;

namespace UnityEngine.Advertisements
{
	public delegate void UnityAdsDelegate();
	public delegate void UnityAdsDelegate<T1, T2>(T1 p1, T2 p2);
}
