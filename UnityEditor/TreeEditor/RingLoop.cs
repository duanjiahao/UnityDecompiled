using System;
using System.Collections.Generic;
using UnityEngine;
namespace TreeEditor
{
	public class RingLoop
	{
		public float radius;
		private Matrix4x4 matrix;
		private int segments;
		public float baseOffset;
		private Vector4 animParams;
		private float spreadTop;
		private float spreadBot;
		private float noiseScale;
		private float noiseScaleU = 1f;
		private float noiseScaleV = 1f;
		private float flareRadius;
		private float flareNoise;
		private float surfAngleCos;
		private float surfAngleSin;
		private int vertOffset;
		private static Perlin perlin = new Perlin();
		private static int noiseSeed = -1;
		public static void SetNoiseSeed(int seed)
		{
			if (RingLoop.noiseSeed != seed)
			{
				RingLoop.perlin.SetSeed(seed);
			}
		}
		public RingLoop Clone()
		{
			return new RingLoop
			{
				radius = this.radius,
				matrix = this.matrix,
				segments = this.segments,
				baseOffset = this.baseOffset,
				animParams = this.animParams,
				spreadTop = this.spreadTop,
				spreadBot = this.spreadBot,
				noiseScale = this.noiseScale,
				noiseScaleU = this.noiseScaleU,
				noiseScaleV = this.noiseScaleV,
				flareRadius = this.flareRadius,
				flareNoise = this.flareNoise,
				surfAngleCos = this.surfAngleCos,
				surfAngleSin = this.surfAngleSin
			};
		}
		public void Reset(float r, Matrix4x4 m, float bOffset, int segs)
		{
			this.radius = r;
			this.matrix = m;
			this.baseOffset = bOffset;
			this.segments = segs;
			this.vertOffset = 0;
		}
		public void SetSurfaceAngle(float angleDeg)
		{
			this.surfAngleCos = Mathf.Cos(angleDeg * 0.0174532924f);
			this.surfAngleSin = -Mathf.Sin(angleDeg * 0.0174532924f);
		}
		public void SetAnimationProperties(float primaryFactor, float secondaryFactor, float edgeFactor, float phase)
		{
			this.animParams = new Vector4(primaryFactor, secondaryFactor, edgeFactor, phase);
		}
		public void SetSpread(float top, float bottom)
		{
			this.spreadTop = top;
			this.spreadBot = bottom;
		}
		public void SetNoise(float scale, float scaleU, float scaleV)
		{
			this.noiseScale = scale;
			this.noiseScaleU = scaleU;
			this.noiseScaleV = scaleV;
		}
		public void SetFlares(float radius, float noise)
		{
			this.flareRadius = radius;
			this.flareNoise = noise;
		}
		public void BuildVertices(List<TreeVertex> verts)
		{
			this.vertOffset = verts.Count;
			for (int i = 0; i <= this.segments; i++)
			{
				float f = (float)i * 3.14159274f * 2f / (float)this.segments;
				TreeVertex treeVertex = new TreeVertex();
				float num = this.radius;
				float num2 = 1f - (float)i / (float)this.segments;
				float num3 = this.baseOffset;
				float num4 = num2;
				float num5 = num3;
				if (i == this.segments)
				{
					num4 = 1f;
				}
				float num6 = Mathf.Cos(f);
				float num7;
				if (num6 > 0f)
				{
					num7 = Mathf.Pow(num6, 3f) * this.radius * this.spreadBot;
				}
				else
				{
					num7 = Mathf.Pow(Mathf.Abs(num6), 3f) * this.radius * this.spreadTop;
				}
				float x = num4 * this.noiseScaleU;
				float y = num5 * this.noiseScaleV;
				num += this.radius * (RingLoop.perlin.Noise(x, y) * this.noiseScale);
				x = num4 * this.flareNoise;
				num += this.flareRadius * Mathf.Abs(RingLoop.perlin.Noise(x, 0.12932f));
				treeVertex.pos = this.matrix.MultiplyPoint(new Vector3(Mathf.Sin(f) * (num + num7 * 0.25f), 0f, Mathf.Cos(f) * (num + num7)));
				treeVertex.uv0 = new Vector2(num2, num3);
				treeVertex.SetAnimationProperties(this.animParams.x, this.animParams.y, this.animParams.z, this.animParams.w);
				verts.Add(treeVertex);
			}
			if (this.radius == 0f)
			{
				for (int j = 0; j <= this.segments; j++)
				{
					TreeVertex treeVertex2 = verts[j + this.vertOffset];
					float num8 = (float)j * 3.14159274f * 2f / (float)this.segments;
					float f2 = num8 - 1.57079637f;
					Vector3 zero = Vector3.zero;
					zero.x = Mathf.Sin(num8) * this.surfAngleCos;
					zero.y = this.surfAngleSin;
					zero.z = Mathf.Cos(num8) * this.surfAngleCos;
					treeVertex2.nor = Vector3.Normalize(this.matrix.MultiplyVector(zero));
					Vector3 v = Vector3.zero;
					v.x = Mathf.Sin(f2);
					v.y = 0f;
					v.z = Mathf.Cos(f2);
					v = Vector3.Normalize(this.matrix.MultiplyVector(v));
					treeVertex2.tangent = new Vector4(v.x, v.y, v.z, -1f);
				}
				return;
			}
			Matrix4x4 inverse = this.matrix.inverse;
			for (int k = 0; k <= this.segments; k++)
			{
				int num9 = k - 1;
				if (num9 < 0)
				{
					num9 = this.segments - 1;
				}
				int num10 = k + 1;
				if (num10 > this.segments)
				{
					num10 = 1;
				}
				TreeVertex treeVertex3 = verts[num9 + this.vertOffset];
				TreeVertex treeVertex4 = verts[num10 + this.vertOffset];
				TreeVertex treeVertex5 = verts[k + this.vertOffset];
				Vector3 vector = Vector3.Normalize(treeVertex3.pos - treeVertex4.pos);
				Vector3 v2 = inverse.MultiplyVector(treeVertex3.pos - treeVertex4.pos);
				v2.y = v2.x;
				v2.x = v2.z;
				v2.z = -v2.y;
				v2.y = 0f;
				v2.Normalize();
				v2.x = this.surfAngleCos * v2.x;
				v2.y = this.surfAngleSin;
				v2.z = this.surfAngleCos * v2.z;
				treeVertex5.nor = Vector3.Normalize(this.matrix.MultiplyVector(v2));
				treeVertex5.tangent.x = vector.x;
				treeVertex5.tangent.y = vector.y;
				treeVertex5.tangent.z = vector.z;
				treeVertex5.tangent.w = -1f;
			}
		}
		public void Cap(float sphereFactor, float noise, int mappingMode, float mappingScale, List<TreeVertex> verts, List<TreeTriangle> tris, int materialIndex)
		{
			int num = Mathf.Max(1, (int)((float)(this.segments / 2) * Mathf.Clamp01(sphereFactor)));
			int num2 = this.segments;
			int num3 = num;
			if (mappingMode == 1)
			{
				num2++;
				num3++;
				mappingScale /= Mathf.Max(1f, sphereFactor);
			}
			int count = verts.Count;
			Vector3 vector = Vector3.Normalize(this.matrix.MultiplyVector(Vector3.up));
			Vector3 a = this.matrix.MultiplyPoint(Vector3.zero);
			TreeVertex treeVertex = new TreeVertex();
			treeVertex.nor = vector;
			treeVertex.pos = a + treeVertex.nor * sphereFactor * this.radius;
			Vector3 vector2 = Vector3.Normalize(this.matrix.MultiplyVector(Vector3.right));
			treeVertex.tangent = new Vector4(vector2.x, vector2.y, vector2.z, -1f);
			treeVertex.SetAnimationProperties(this.animParams.x, this.animParams.y, this.animParams.z, this.animParams.w);
			if (mappingMode == 0)
			{
				treeVertex.uv0 = new Vector2(0.5f, 0.5f);
			}
			else
			{
				treeVertex.uv0 = new Vector2(0.5f, this.baseOffset + sphereFactor * mappingScale);
			}
			verts.Add(treeVertex);
			int count2 = verts.Count;
			Matrix4x4 inverse = this.matrix.inverse;
			for (int i = 0; i < num3; i++)
			{
				float f = (1f - (float)i / (float)num) * 3.14159274f * 0.5f;
				float num4 = Mathf.Sin(f);
				float num5 = num4;
				float num6 = num4 * Mathf.Clamp01(sphereFactor) + num4 * 0.5f * Mathf.Clamp01(1f - sphereFactor);
				float num7 = Mathf.Cos(f);
				for (int j = 0; j < num2; j++)
				{
					TreeVertex treeVertex2 = verts[this.vertOffset + j];
					Vector3 vector3 = inverse.MultiplyPoint(treeVertex2.pos).normalized * 0.5f * num4;
					TreeVertex treeVertex3 = new TreeVertex();
					treeVertex3.pos = treeVertex2.pos * num5 + a * (1f - num5) + vector * num7 * sphereFactor * this.radius;
					treeVertex3.nor = (treeVertex2.nor * num6 + vector * (1f - num6)).normalized;
					treeVertex3.SetAnimationProperties(this.animParams.x, this.animParams.y, this.animParams.z, this.animParams.w);
					if (mappingMode == 0)
					{
						treeVertex3.tangent = treeVertex.tangent;
						treeVertex3.uv0 = new Vector2(0.5f + vector3.x, 0.5f + vector3.z);
					}
					else
					{
						treeVertex3.tangent = treeVertex2.tangent;
						treeVertex3.uv0 = new Vector2((float)j / (float)this.segments, this.baseOffset + sphereFactor * num7 * mappingScale);
					}
					verts.Add(treeVertex3);
				}
			}
			float num8 = 3f;
			for (int k = this.vertOffset; k < verts.Count; k++)
			{
				float x = verts[k].pos.x * num8;
				float y = verts[k].pos.z * num8;
				float d = this.radius * (RingLoop.perlin.Noise(x, y) * noise);
				verts[k].pos += treeVertex.nor * d;
			}
			for (int l = 0; l < num; l++)
			{
				for (int m = 0; m < num2; m++)
				{
					if (l == num3 - 1)
					{
						int num9 = m + count2 + num2 * l;
						int v = num9 + 1;
						if (m == num2 - 1)
						{
							v = count2 + num2 * l;
						}
						tris.Add(new TreeTriangle(materialIndex, count, num9, v, false, false, false));
					}
					else
					{
						int num10 = m + count2 + num2 * l;
						int v2 = num10 + 1;
						int num11 = m + count2 + num2 * (l + 1);
						int num12 = num11 + 1;
						if (m == num2 - 1)
						{
							v2 = count2 + num2 * l;
							num12 = count2 + num2 * (l + 1);
						}
						tris.Add(new TreeTriangle(materialIndex, num10, v2, num12, false, false, false));
						tris.Add(new TreeTriangle(materialIndex, num10, num12, num11, false, false, false));
					}
				}
			}
		}
		public void Connect(RingLoop other, List<TreeTriangle> tris, int materialIndex, bool flipTris, bool lowres)
		{
			if (other.segments > this.segments)
			{
				other.Connect(this, tris, materialIndex, true, lowres);
				return;
			}
			if (lowres)
			{
				for (int i = 0; i < this.segments / 2; i++)
				{
					int num = 0 + i + other.vertOffset;
					int num2 = other.segments / 2 + i + other.vertOffset;
					int num3 = 0 + i + this.vertOffset;
					int num4 = this.segments / 2 + i + this.vertOffset;
					if (flipTris)
					{
						int num5 = num;
						num = num2;
						num2 = num5;
						num5 = num3;
						num3 = num4;
						num4 = num5;
					}
					tris.Add(new TreeTriangle(materialIndex, num, num2, num3, false, true, false));
					tris.Add(new TreeTriangle(materialIndex, num2, num4, num3, false, true, false));
				}
			}
			else
			{
				for (int j = 0; j < this.segments; j++)
				{
					int num6 = Mathf.Min((int)Mathf.Round((float)j / (float)this.segments * (float)other.segments), other.segments);
					int num7 = Mathf.Min((int)Mathf.Round((float)(j + 1) / (float)this.segments * (float)other.segments), other.segments);
					int num8 = Mathf.Min(j + 1, this.segments);
					int num9 = num6 + other.vertOffset;
					int num10 = j + this.vertOffset;
					int v = num7 + other.vertOffset;
					int num11 = j + this.vertOffset;
					int num12 = num8 + this.vertOffset;
					int v2 = num7 + other.vertOffset;
					if (flipTris)
					{
						int num13 = num10;
						num10 = num9;
						num9 = num13;
						num13 = num12;
						num12 = num11;
						num11 = num13;
					}
					tris.Add(new TreeTriangle(materialIndex, num9, num10, v, false, true, false));
					tris.Add(new TreeTriangle(materialIndex, num11, num12, v2, false, true, false));
				}
			}
		}
	}
}
