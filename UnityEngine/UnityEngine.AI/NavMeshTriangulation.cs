using System;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	[MovedFrom("UnityEngine"), UsedByNativeCode]
	public struct NavMeshTriangulation
	{
		public Vector3[] vertices;

		public int[] indices;

		public int[] areas;

		[Obsolete("Use areas instead.")]
		public int[] layers
		{
			get
			{
				return this.areas;
			}
		}
	}
}
