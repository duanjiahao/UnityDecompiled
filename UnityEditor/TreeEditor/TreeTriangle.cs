using System;
namespace TreeEditor
{
	public class TreeTriangle
	{
		public bool tileV;
		public bool isBillboard;
		public bool isCutout = true;
		public int materialIndex = -1;
		public int[] v = new int[3];
		public TreeTriangle(int material, int v0, int v1, int v2)
		{
			this.materialIndex = material;
			this.v[0] = v0;
			this.v[1] = v1;
			this.v[2] = v2;
		}
		public TreeTriangle(int material, int v0, int v1, int v2, bool isBillboard)
		{
			this.isBillboard = isBillboard;
			this.materialIndex = material;
			this.v[0] = v0;
			this.v[1] = v1;
			this.v[2] = v2;
		}
		public TreeTriangle(int material, int v0, int v1, int v2, bool isBillboard, bool tileV, bool isCutout)
		{
			this.tileV = tileV;
			this.isBillboard = isBillboard;
			this.isCutout = isCutout;
			this.materialIndex = material;
			this.v[0] = v0;
			this.v[1] = v1;
			this.v[2] = v2;
		}
		public void flip()
		{
			int num = this.v[0];
			this.v[0] = this.v[1];
			this.v[1] = num;
		}
	}
}
