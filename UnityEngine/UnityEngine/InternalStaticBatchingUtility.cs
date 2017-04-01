using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine
{
	internal class InternalStaticBatchingUtility
	{
		internal class SortGO : IComparer
		{
			int IComparer.Compare(object a, object b)
			{
				int result;
				if (a == b)
				{
					result = 0;
				}
				else
				{
					Renderer renderer = InternalStaticBatchingUtility.SortGO.GetRenderer(a as GameObject);
					Renderer renderer2 = InternalStaticBatchingUtility.SortGO.GetRenderer(b as GameObject);
					int num = InternalStaticBatchingUtility.SortGO.GetMaterialId(renderer).CompareTo(InternalStaticBatchingUtility.SortGO.GetMaterialId(renderer2));
					if (num == 0)
					{
						num = InternalStaticBatchingUtility.SortGO.GetLightmapIndex(renderer).CompareTo(InternalStaticBatchingUtility.SortGO.GetLightmapIndex(renderer2));
					}
					result = num;
				}
				return result;
			}

			private static int GetMaterialId(Renderer renderer)
			{
				int result;
				if (renderer == null || renderer.sharedMaterial == null)
				{
					result = 0;
				}
				else
				{
					result = renderer.sharedMaterial.GetInstanceID();
				}
				return result;
			}

			private static int GetLightmapIndex(Renderer renderer)
			{
				int result;
				if (renderer == null)
				{
					result = -1;
				}
				else
				{
					result = renderer.lightmapIndex;
				}
				return result;
			}

			private static Renderer GetRenderer(GameObject go)
			{
				Renderer result;
				if (go == null)
				{
					result = null;
				}
				else
				{
					MeshFilter meshFilter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
					if (meshFilter == null)
					{
						result = null;
					}
					else
					{
						result = meshFilter.GetComponent<Renderer>();
					}
				}
				return result;
			}
		}

		private const int MaxVerticesInBatch = 64000;

		private const string CombinedMeshPrefix = "Combined Mesh";

		public static void CombineRoot(GameObject staticBatchRoot)
		{
			InternalStaticBatchingUtility.Combine(staticBatchRoot, false, false);
		}

		public static void Combine(GameObject staticBatchRoot, bool combineOnlyStatic, bool isEditorPostprocessScene)
		{
			GameObject[] array = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
			List<GameObject> list = new List<GameObject>();
			GameObject[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				GameObject gameObject = array2[i];
				if (!(staticBatchRoot != null) || gameObject.transform.IsChildOf(staticBatchRoot.transform))
				{
					if (!combineOnlyStatic || gameObject.isStaticBatchable)
					{
						list.Add(gameObject);
					}
				}
			}
			array = list.ToArray();
			InternalStaticBatchingUtility.CombineGameObjects(array, staticBatchRoot, isEditorPostprocessScene);
		}

		public static void CombineGameObjects(GameObject[] gos, GameObject staticBatchRoot, bool isEditorPostprocessScene)
		{
			Matrix4x4 lhs = Matrix4x4.identity;
			Transform staticBatchRootTransform = null;
			if (staticBatchRoot)
			{
				lhs = staticBatchRoot.transform.worldToLocalMatrix;
				staticBatchRootTransform = staticBatchRoot.transform;
			}
			int batchIndex = 0;
			int num = 0;
			List<MeshSubsetCombineUtility.MeshContainer> list = new List<MeshSubsetCombineUtility.MeshContainer>();
			Array.Sort(gos, new InternalStaticBatchingUtility.SortGO());
			for (int i = 0; i < gos.Length; i++)
			{
				GameObject gameObject = gos[i];
				MeshFilter meshFilter = gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
				if (!(meshFilter == null))
				{
					Mesh sharedMesh = meshFilter.sharedMesh;
					if (!(sharedMesh == null) && (isEditorPostprocessScene || sharedMesh.canAccess))
					{
						Renderer component = meshFilter.GetComponent<Renderer>();
						if (!(component == null) && component.enabled)
						{
							if (component.staticBatchIndex == 0)
							{
								Material[] array = component.sharedMaterials;
								if (!array.Any((Material m) => m != null && m.shader != null && m.shader.disableBatching != DisableBatchingType.False))
								{
									int vertexCount = sharedMesh.vertexCount;
									if (vertexCount != 0)
									{
										MeshRenderer meshRenderer = component as MeshRenderer;
										if (meshRenderer != null && meshRenderer.additionalVertexStreams != null)
										{
											if (vertexCount != meshRenderer.additionalVertexStreams.vertexCount)
											{
												goto IL_387;
											}
										}
										if (num + vertexCount > 64000)
										{
											InternalStaticBatchingUtility.MakeBatch(list, staticBatchRootTransform, batchIndex++);
											list.Clear();
											num = 0;
										}
										MeshSubsetCombineUtility.MeshInstance instance = default(MeshSubsetCombineUtility.MeshInstance);
										instance.meshInstanceID = sharedMesh.GetInstanceID();
										instance.rendererInstanceID = component.GetInstanceID();
										if (meshRenderer != null && meshRenderer.additionalVertexStreams != null)
										{
											instance.additionalVertexStreamsMeshInstanceID = meshRenderer.additionalVertexStreams.GetInstanceID();
										}
										instance.transform = lhs * meshFilter.transform.localToWorldMatrix;
										instance.lightmapScaleOffset = component.lightmapScaleOffset;
										instance.realtimeLightmapScaleOffset = component.realtimeLightmapScaleOffset;
										MeshSubsetCombineUtility.MeshContainer item = default(MeshSubsetCombineUtility.MeshContainer);
										item.gameObject = gameObject;
										item.instance = instance;
										item.subMeshInstances = new List<MeshSubsetCombineUtility.SubMeshInstance>();
										list.Add(item);
										if (array.Length > sharedMesh.subMeshCount)
										{
											Debug.LogWarning(string.Concat(new object[]
											{
												"Mesh '",
												sharedMesh.name,
												"' has more materials (",
												array.Length,
												") than subsets (",
												sharedMesh.subMeshCount,
												")"
											}), component);
											Material[] array2 = new Material[sharedMesh.subMeshCount];
											for (int j = 0; j < sharedMesh.subMeshCount; j++)
											{
												array2[j] = component.sharedMaterials[j];
											}
											component.sharedMaterials = array2;
											array = array2;
										}
										for (int k = 0; k < Math.Min(array.Length, sharedMesh.subMeshCount); k++)
										{
											MeshSubsetCombineUtility.SubMeshInstance item2 = default(MeshSubsetCombineUtility.SubMeshInstance);
											item2.meshInstanceID = meshFilter.sharedMesh.GetInstanceID();
											item2.vertexOffset = num;
											item2.subMeshIndex = k;
											item2.gameObjectInstanceID = gameObject.GetInstanceID();
											item2.transform = instance.transform;
											item.subMeshInstances.Add(item2);
										}
										num += sharedMesh.vertexCount;
									}
								}
							}
						}
					}
				}
				IL_387:;
			}
			InternalStaticBatchingUtility.MakeBatch(list, staticBatchRootTransform, batchIndex);
		}

		private static void MakeBatch(List<MeshSubsetCombineUtility.MeshContainer> meshes, Transform staticBatchRootTransform, int batchIndex)
		{
			if (meshes.Count >= 2)
			{
				List<MeshSubsetCombineUtility.MeshInstance> list = new List<MeshSubsetCombineUtility.MeshInstance>();
				List<MeshSubsetCombineUtility.SubMeshInstance> list2 = new List<MeshSubsetCombineUtility.SubMeshInstance>();
				foreach (MeshSubsetCombineUtility.MeshContainer current in meshes)
				{
					list.Add(current.instance);
					list2.AddRange(current.subMeshInstances);
				}
				string text = "Combined Mesh";
				text = text + " (root: " + ((!(staticBatchRootTransform != null)) ? "scene" : staticBatchRootTransform.name) + ")";
				if (batchIndex > 0)
				{
					text = text + " " + (batchIndex + 1);
				}
				Mesh mesh = StaticBatchingHelper.InternalCombineVertices(list.ToArray(), text);
				StaticBatchingHelper.InternalCombineIndices(list2.ToArray(), mesh);
				int num = 0;
				foreach (MeshSubsetCombineUtility.MeshContainer current2 in meshes)
				{
					MeshFilter meshFilter = (MeshFilter)current2.gameObject.GetComponent(typeof(MeshFilter));
					meshFilter.sharedMesh = mesh;
					int num2 = current2.subMeshInstances.Count<MeshSubsetCombineUtility.SubMeshInstance>();
					Renderer component = current2.gameObject.GetComponent<Renderer>();
					component.SetStaticBatchInfo(num, num2);
					component.staticBatchRootTransform = staticBatchRootTransform;
					component.enabled = false;
					component.enabled = true;
					MeshRenderer meshRenderer = component as MeshRenderer;
					if (meshRenderer != null)
					{
						meshRenderer.additionalVertexStreams = null;
					}
					num += num2;
				}
			}
		}
	}
}
