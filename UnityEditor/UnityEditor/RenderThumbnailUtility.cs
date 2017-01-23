using System;
using UnityEngine;

namespace UnityEditor
{
	internal class RenderThumbnailUtility
	{
		public static Bounds CalculateVisibleBounds(GameObject prefab)
		{
			return prefab.GetComponent<Renderer>().bounds;
		}

		public static Texture2D Render(GameObject prefab)
		{
			Texture2D result;
			if (prefab == null)
			{
				result = null;
			}
			else if (prefab.GetComponent<Renderer>() == null)
			{
				result = null;
			}
			else
			{
				Texture2D texture2D = new Texture2D(64, 64);
				texture2D.hideFlags = HideFlags.HideAndDontSave;
				texture2D.name = "Preview Texture";
				RenderTexture temporary = RenderTexture.GetTemporary(texture2D.width, texture2D.height);
				GameObject gameObject = new GameObject("Preview");
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				Camera camera = gameObject.AddComponent(typeof(Camera)) as Camera;
				camera.cameraType = CameraType.Preview;
				camera.clearFlags = CameraClearFlags.Color;
				camera.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0f);
				camera.cullingMask = 0;
				camera.enabled = false;
				camera.targetTexture = temporary;
				Light light = gameObject.AddComponent(typeof(Light)) as Light;
				light.type = LightType.Directional;
				Bounds bounds = RenderThumbnailUtility.CalculateVisibleBounds(prefab);
				Vector3 vector = new Vector3(0.7f, 0.3f, 0.7f);
				float num = bounds.extents.magnitude * 1.6f;
				gameObject.transform.position = bounds.center + vector.normalized * num;
				gameObject.transform.LookAt(bounds.center);
				camera.nearClipPlane = num * 0.1f;
				camera.farClipPlane = num * 2.2f;
				Camera current = Camera.current;
				camera.RenderDontRestore();
				Light[] lights = new Light[]
				{
					light
				};
				Graphics.SetupVertexLights(lights);
				Component[] componentsInChildren = prefab.GetComponentsInChildren(typeof(Renderer));
				Component[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					Renderer renderer = (Renderer)array[i];
					if (renderer.enabled)
					{
						Material[] sharedMaterials = renderer.sharedMaterials;
						for (int j = 0; j < sharedMaterials.Length; j++)
						{
							if (!(sharedMaterials[j] == null))
							{
								Material material = sharedMaterials[j];
								string dependency = ShaderUtil.GetDependency(material.shader, "BillboardShader");
								if (dependency != null && dependency != "")
								{
									material = UnityEngine.Object.Instantiate<Material>(material);
									material.shader = Shader.Find(dependency);
									material.hideFlags = HideFlags.HideAndDontSave;
								}
								for (int k = 0; k < material.passCount; k++)
								{
									if (material.SetPass(k))
									{
										renderer.RenderNow(j);
									}
								}
								if (material != sharedMaterials[j])
								{
									UnityEngine.Object.DestroyImmediate(material);
								}
							}
						}
					}
				}
				texture2D.ReadPixels(new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), 0, 0);
				RenderTexture.ReleaseTemporary(temporary);
				UnityEngine.Object.DestroyImmediate(gameObject);
				Camera.SetupCurrent(current);
				result = texture2D;
			}
			return result;
		}
	}
}
