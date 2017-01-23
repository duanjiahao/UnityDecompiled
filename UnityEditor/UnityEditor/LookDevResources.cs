using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	internal class LookDevResources
	{
		public static SphericalHarmonicsL2 m_ZeroAmbientProbe;

		public static Material m_SkyboxMaterial = null;

		public static Material m_GBufferPatchMaterial = null;

		public static Material m_DrawBallsMaterial = null;

		public static Mesh m_ScreenQuadMesh = null;

		public static Material m_LookDevCompositing = null;

		public static Material m_DeferredOverlayMaterial = null;

		public static Cubemap m_DefaultHDRI = null;

		public static Material m_LookDevCubeToLatlong = null;

		public static RenderTexture m_SelectionTexture = null;

		public static RenderTexture m_BrightestPointRT = null;

		public static Texture2D m_BrightestPointTexture = null;

		public static void Initialize()
		{
			LookDevResources.m_ZeroAmbientProbe.Clear();
			if (LookDevResources.m_SkyboxMaterial == null)
			{
				LookDevResources.m_SkyboxMaterial = new Material(Shader.Find("Skybox/Cubemap"));
			}
			if (LookDevResources.m_ScreenQuadMesh == null)
			{
				LookDevResources.m_ScreenQuadMesh = new Mesh();
				LookDevResources.m_ScreenQuadMesh.vertices = new Vector3[]
				{
					new Vector3(-1f, -1f, 0f),
					new Vector3(1f, 1f, 0f),
					new Vector3(1f, -1f, 0f),
					new Vector3(-1f, 1f, 0f)
				};
				LookDevResources.m_ScreenQuadMesh.triangles = new int[]
				{
					0,
					1,
					2,
					1,
					0,
					3
				};
			}
			if (LookDevResources.m_GBufferPatchMaterial == null)
			{
				LookDevResources.m_GBufferPatchMaterial = new Material(EditorGUIUtility.LoadRequired("LookDevView/GBufferWhitePatch.shader") as Shader);
				LookDevResources.m_DrawBallsMaterial = new Material(EditorGUIUtility.LoadRequired("LookDevView/GBufferBalls.shader") as Shader);
			}
			if (LookDevResources.m_LookDevCompositing == null)
			{
				LookDevResources.m_LookDevCompositing = new Material(EditorGUIUtility.LoadRequired("LookDevView/LookDevCompositing.shader") as Shader);
			}
			if (LookDevResources.m_DeferredOverlayMaterial == null)
			{
				LookDevResources.m_DeferredOverlayMaterial = (EditorGUIUtility.LoadRequired("SceneView/SceneViewDeferredMaterial.mat") as Material);
			}
			if (LookDevResources.m_DefaultHDRI == null)
			{
				LookDevResources.m_DefaultHDRI = (EditorGUIUtility.Load("LookDevView/DefaultHDRI.exr") as Cubemap);
				if (LookDevResources.m_DefaultHDRI == null)
				{
					LookDevResources.m_DefaultHDRI = (EditorGUIUtility.Load("LookDevView/DefaultHDRI.asset") as Cubemap);
				}
			}
			if (LookDevResources.m_LookDevCubeToLatlong == null)
			{
				LookDevResources.m_LookDevCubeToLatlong = new Material(EditorGUIUtility.LoadRequired("LookDevView/LookDevCubeToLatlong.shader") as Shader);
			}
			if (LookDevResources.m_SelectionTexture == null)
			{
				LookDevResources.m_SelectionTexture = new RenderTexture(250, 125, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
			}
			if (LookDevResources.m_BrightestPointRT == null)
			{
				LookDevResources.m_BrightestPointRT = new RenderTexture(250, 125, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default);
			}
			if (LookDevResources.m_BrightestPointTexture == null)
			{
				LookDevResources.m_BrightestPointTexture = new Texture2D(250, 125, TextureFormat.RGBAHalf, false);
			}
		}

		public static void Cleanup()
		{
			LookDevResources.m_SkyboxMaterial = null;
			if (LookDevResources.m_LookDevCompositing)
			{
				UnityEngine.Object.DestroyImmediate(LookDevResources.m_LookDevCompositing);
				LookDevResources.m_LookDevCompositing = null;
			}
		}

		public static void UpdateShadowInfoWithBrightestSpot(CubemapInfo cubemapInfo)
		{
			LookDevResources.m_LookDevCubeToLatlong.SetTexture("_MainTex", cubemapInfo.cubemap);
			LookDevResources.m_LookDevCubeToLatlong.SetVector("_WindowParams", new Vector4(10000f, -1000f, 2f, 0f));
			LookDevResources.m_LookDevCubeToLatlong.SetVector("_CubeToLatLongParams", new Vector4(0.0174532924f * cubemapInfo.angleOffset, 0.5f, 1f, 3f));
			LookDevResources.m_LookDevCubeToLatlong.SetPass(0);
			int num = 250;
			int num2 = 125;
			Graphics.Blit(cubemapInfo.cubemap, LookDevResources.m_BrightestPointRT, LookDevResources.m_LookDevCubeToLatlong);
			LookDevResources.m_BrightestPointTexture.ReadPixels(new Rect(0f, 0f, (float)num, (float)num2), 0, 0, false);
			LookDevResources.m_BrightestPointTexture.Apply();
			Color[] pixels = LookDevResources.m_BrightestPointTexture.GetPixels();
			float num3 = 0f;
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					Vector3 vector = new Vector3(pixels[i * num + j].r, pixels[i * num + j].g, pixels[i * num + j].b);
					float num4 = vector.x * 0.2126729f + vector.y * 0.7151522f + vector.z * 0.072175f;
					if (num3 < num4)
					{
						Vector2 vector2 = LookDevEnvironmentWindow.PositionToLatLong(new Vector2((float)j / (float)(num - 1) * 2f - 1f, (float)i / (float)(num2 - 1) * 2f - 1f));
						cubemapInfo.shadowInfo.latitude = vector2.x;
						cubemapInfo.shadowInfo.longitude = vector2.y - cubemapInfo.angleOffset;
						num3 = num4;
					}
				}
			}
		}
	}
}
