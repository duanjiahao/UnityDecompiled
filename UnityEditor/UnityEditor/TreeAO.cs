using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TreeAO
	{
		private const int kWorkLayer = 29;

		private const float occlusion = 0.5f;

		private static bool kDebug;

		private static Vector3[] directions;

		private static int PermuteCuboid(Vector3[] dirs, int offset, float x, float y, float z)
		{
			dirs[offset] = new Vector3(x, y, z);
			dirs[offset + 1] = new Vector3(x, y, -z);
			dirs[offset + 2] = new Vector3(x, -y, z);
			dirs[offset + 3] = new Vector3(x, -y, -z);
			dirs[offset + 4] = new Vector3(-x, y, z);
			dirs[offset + 5] = new Vector3(-x, y, -z);
			dirs[offset + 6] = new Vector3(-x, -y, z);
			dirs[offset + 7] = new Vector3(-x, -y, -z);
			return offset + 8;
		}

		public static void InitializeDirections()
		{
			float num = (1f + Mathf.Sqrt(5f)) / 2f;
			TreeAO.directions = new Vector3[60];
			TreeAO.directions[0] = new Vector3(0f, 1f, 3f * num);
			TreeAO.directions[1] = new Vector3(0f, 1f, -3f * num);
			TreeAO.directions[2] = new Vector3(0f, -1f, 3f * num);
			TreeAO.directions[3] = new Vector3(0f, -1f, -3f * num);
			TreeAO.directions[4] = new Vector3(1f, 3f * num, 0f);
			TreeAO.directions[5] = new Vector3(1f, -3f * num, 0f);
			TreeAO.directions[6] = new Vector3(-1f, 3f * num, 0f);
			TreeAO.directions[7] = new Vector3(-1f, -3f * num, 0f);
			TreeAO.directions[8] = new Vector3(3f * num, 0f, 1f);
			TreeAO.directions[9] = new Vector3(3f * num, 0f, -1f);
			TreeAO.directions[10] = new Vector3(-3f * num, 0f, 1f);
			TreeAO.directions[11] = new Vector3(-3f * num, 0f, -1f);
			int offset = 12;
			offset = TreeAO.PermuteCuboid(TreeAO.directions, offset, 2f, 1f + 2f * num, num);
			offset = TreeAO.PermuteCuboid(TreeAO.directions, offset, 1f + 2f * num, num, 2f);
			offset = TreeAO.PermuteCuboid(TreeAO.directions, offset, num, 2f, 1f + 2f * num);
			offset = TreeAO.PermuteCuboid(TreeAO.directions, offset, 1f, 2f + num, 2f * num);
			offset = TreeAO.PermuteCuboid(TreeAO.directions, offset, 2f + num, 2f * num, 1f);
			offset = TreeAO.PermuteCuboid(TreeAO.directions, offset, 2f * num, 1f, 2f + num);
			for (int i = 0; i < TreeAO.directions.Length; i++)
			{
				TreeAO.directions[i] = TreeAO.directions[i].normalized;
			}
		}

		public static void CalcSoftOcclusion(Mesh mesh)
		{
			GameObject gameObject = new GameObject("Test");
			gameObject.layer = 29;
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			gameObject.AddComponent<MeshCollider>();
			if (TreeAO.directions == null)
			{
				TreeAO.InitializeDirections();
			}
			Vector4[] array = new Vector4[TreeAO.directions.Length];
			for (int i = 0; i < TreeAO.directions.Length; i++)
			{
				array[i] = new Vector4(TreeAO.GetWeight(1, TreeAO.directions[i]), TreeAO.GetWeight(2, TreeAO.directions[i]), TreeAO.GetWeight(3, TreeAO.directions[i]), TreeAO.GetWeight(0, TreeAO.directions[i]));
			}
			Vector3[] vertices = mesh.vertices;
			Vector4[] array2 = new Vector4[vertices.Length];
			float num = 0f;
			for (int j = 0; j < vertices.Length; j++)
			{
				Vector4 vector = Vector4.zero;
				Vector3 v = gameObject.transform.TransformPoint(vertices[j]);
				for (int k = 0; k < TreeAO.directions.Length; k++)
				{
					float num2 = (float)TreeAO.CountIntersections(v, gameObject.transform.TransformDirection(TreeAO.directions[k]), 3f);
					num2 = Mathf.Pow(0.5f, num2);
					vector += array[k] * num2;
				}
				vector /= (float)TreeAO.directions.Length;
				num += vector.w;
				array2[j] = vector;
			}
			num /= (float)vertices.Length;
			for (int l = 0; l < vertices.Length; l++)
			{
				Vector4[] expr_1D4_cp_0 = array2;
				int expr_1D4_cp_1 = l;
				expr_1D4_cp_0[expr_1D4_cp_1].w = expr_1D4_cp_0[expr_1D4_cp_1].w - num;
			}
			mesh.tangents = array2;
			UnityEngine.Object.DestroyImmediate(gameObject);
		}

		private static int CountIntersections(Vector3 v, Vector3 dist, float length)
		{
			v += dist * 0.01f;
			if (!TreeAO.kDebug)
			{
				return Physics.RaycastAll(v, dist, length, 536870912).Length + Physics.RaycastAll(v + dist * length, -dist, length, 536870912).Length;
			}
			RaycastHit[] array = Physics.RaycastAll(v, dist, length, 536870912);
			int num = array.Length;
			float num2 = 0f;
			if (num > 0)
			{
				num2 = array[array.Length - 1].distance;
			}
			array = Physics.RaycastAll(v + dist * length, -dist, length, 536870912);
			if (array.Length > 0)
			{
				float num3 = length - array[0].distance;
				if (num3 > num2)
				{
				}
			}
			return num + array.Length;
		}

		private static float GetWeight(int coeff, Vector3 dir)
		{
			switch (coeff)
			{
			case 0:
				return 0.5f;
			case 1:
				return 0.5f * dir.x;
			case 2:
				return 0.5f * dir.y;
			case 3:
				return 0.5f * dir.z;
			default:
				Debug.Log("Only defined up to 3");
				return 0f;
			}
		}
	}
}
