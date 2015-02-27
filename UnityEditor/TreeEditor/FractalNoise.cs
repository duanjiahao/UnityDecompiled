using System;
using UnityEngine;
namespace TreeEditor
{
	public class FractalNoise
	{
		private Perlin m_Noise;
		private float[] m_Exponent;
		private int m_IntOctaves;
		private float m_Octaves;
		private float m_Lacunarity;
		public FractalNoise(float inH, float inLacunarity, float inOctaves) : this(inH, inLacunarity, inOctaves, null)
		{
		}
		public FractalNoise(float inH, float inLacunarity, float inOctaves, Perlin noise)
		{
			this.m_Lacunarity = inLacunarity;
			this.m_Octaves = inOctaves;
			this.m_IntOctaves = (int)inOctaves;
			this.m_Exponent = new float[this.m_IntOctaves + 1];
			float num = 1f;
			for (int i = 0; i < this.m_IntOctaves + 1; i++)
			{
				this.m_Exponent[i] = (float)Math.Pow((double)this.m_Lacunarity, (double)(-(double)inH));
				num *= this.m_Lacunarity;
			}
			if (noise == null)
			{
				this.m_Noise = new Perlin();
			}
			else
			{
				this.m_Noise = noise;
			}
		}
		public float HybridMultifractal(float x, float y, float offset)
		{
			float num = (this.m_Noise.Noise(x, y) + offset) * this.m_Exponent[0];
			float num2 = num;
			x *= this.m_Lacunarity;
			y *= this.m_Lacunarity;
			int i;
			for (i = 1; i < this.m_IntOctaves; i++)
			{
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				float num3 = (this.m_Noise.Noise(x, y) + offset) * this.m_Exponent[i];
				num += num2 * num3;
				num2 *= num3;
				x *= this.m_Lacunarity;
				y *= this.m_Lacunarity;
			}
			float num4 = this.m_Octaves - (float)this.m_IntOctaves;
			return num + num4 * this.m_Noise.Noise(x, y) * this.m_Exponent[i];
		}
		public float RidgedMultifractal(float x, float y, float offset, float gain)
		{
			float num = Mathf.Abs(this.m_Noise.Noise(x, y));
			num = offset - num;
			num *= num;
			float num2 = num;
			for (int i = 1; i < this.m_IntOctaves; i++)
			{
				x *= this.m_Lacunarity;
				y *= this.m_Lacunarity;
				float num3 = num * gain;
				num3 = Mathf.Clamp01(num3);
				num = Mathf.Abs(this.m_Noise.Noise(x, y));
				num = offset - num;
				num *= num;
				num *= num3;
				num2 += num * this.m_Exponent[i];
			}
			return num2;
		}
		public float BrownianMotion(float x, float y)
		{
			float num = 0f;
			long num2;
			for (num2 = 0L; num2 < (long)this.m_IntOctaves; num2 += 1L)
			{
				num = this.m_Noise.Noise(x, y) * this.m_Exponent[(int)checked((IntPtr)num2)];
				x *= this.m_Lacunarity;
				y *= this.m_Lacunarity;
			}
			float num3 = this.m_Octaves - (float)this.m_IntOctaves;
			return num + num3 * this.m_Noise.Noise(x, y) * this.m_Exponent[(int)checked((IntPtr)num2)];
		}
	}
}
