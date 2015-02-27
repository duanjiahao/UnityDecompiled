using System;
using UnityEngine;
namespace TreeEditor
{
	public class TreeVertex
	{
		public Vector3 pos;
		public Vector3 nor;
		public Vector4 tangent = new Vector4(1f, 0f, 0f, 1f);
		public Vector2 uv0;
		public Vector2 uv1 = new Vector2(0f, 0f);
		public Color color = new Color(0f, 0f, 0f, 1f);
		public bool flag;
		public void SetAnimationProperties(float primaryFactor, float secondaryFactor, float edgeFactor, float phase)
		{
			this.color.r = phase;
			this.color.g = edgeFactor;
			this.uv1.x = primaryFactor;
			this.uv1.y = secondaryFactor;
		}
		public void SetAmbientOcclusion(float ao)
		{
			this.color.a = ao;
		}
		public void Lerp4(TreeVertex[] tv, Vector2 factor)
		{
			this.pos = Vector3.Lerp(Vector3.Lerp(tv[1].pos, tv[2].pos, factor.x), Vector3.Lerp(tv[0].pos, tv[3].pos, factor.x), factor.y);
			this.nor = Vector3.Lerp(Vector3.Lerp(tv[1].nor, tv[2].nor, factor.x), Vector3.Lerp(tv[0].nor, tv[3].nor, factor.x), factor.y).normalized;
			this.tangent = Vector4.Lerp(Vector4.Lerp(tv[1].tangent, tv[2].tangent, factor.x), Vector4.Lerp(tv[0].tangent, tv[3].tangent, factor.x), factor.y);
			Vector3 vector = new Vector3(this.tangent.x, this.tangent.y, this.tangent.z);
			vector.Normalize();
			this.tangent.x = vector.x;
			this.tangent.y = vector.y;
			this.tangent.z = vector.z;
			this.color = Color.Lerp(Color.Lerp(tv[1].color, tv[2].color, factor.x), Color.Lerp(tv[0].color, tv[3].color, factor.x), factor.y);
		}
	}
}
