using System;
using UnityEditor;
using UnityEngine;
namespace TreeEditor
{
	[Serializable]
	public class TreeGroupRoot : TreeGroup
	{
		public float adaptiveLODQuality = 0.8f;
		public int shadowTextureQuality = 3;
		public bool enableWelding = true;
		public bool enableAmbientOcclusion = true;
		public bool enableMaterialOptimize = true;
		public float aoDensity = 1f;
		public float rootSpread = 5f;
		public float groundOffset;
		public Matrix4x4 rootMatrix = Matrix4x4.identity;
		public void SetRootMatrix(Matrix4x4 m)
		{
			this.rootMatrix = m;
			this.rootMatrix.m03 = 0f;
			this.rootMatrix.m13 = 0f;
			this.rootMatrix.m23 = 0f;
			this.rootMatrix = MathUtils.OrthogonalizeMatrix(this.rootMatrix);
			this.nodes[0].matrix = this.rootMatrix;
		}
		public override bool CanHaveSubGroups()
		{
			return true;
		}
		public override void UpdateParameters()
		{
			Profiler.BeginSample("UpdateParameters");
			this.nodes[0].size = this.rootSpread;
			this.nodes[0].matrix = this.rootMatrix;
			base.UpdateParameters();
			Profiler.EndSample();
		}
	}
}
