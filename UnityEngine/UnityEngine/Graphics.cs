using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public sealed class Graphics
	{
		public static RenderBuffer activeColorBuffer
		{
			get
			{
				RenderBuffer result;
				Graphics.GetActiveColorBuffer(out result);
				return result;
			}
		}

		public static RenderBuffer activeDepthBuffer
		{
			get
			{
				RenderBuffer result;
				Graphics.GetActiveDepthBuffer(out result);
				return result;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property deviceName has been deprecated. Use SystemInfo.graphicsDeviceName instead (UnityUpgradable) -> UnityEngine.SystemInfo.graphicsDeviceName", true)]
		public static string deviceName
		{
			get
			{
				return SystemInfo.graphicsDeviceName;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property deviceVendor has been deprecated. Use SystemInfo.graphicsDeviceVendor instead (UnityUpgradable) -> UnityEngine.SystemInfo.graphicsDeviceVendor", true)]
		public static string deviceVendor
		{
			get
			{
				return SystemInfo.graphicsDeviceVendor;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property deviceVersion has been deprecated. Use SystemInfo.graphicsDeviceVersion instead (UnityUpgradable) -> UnityEngine.SystemInfo.graphicsDeviceVersion", true)]
		public static string deviceVersion
		{
			get
			{
				return SystemInfo.graphicsDeviceVersion;
			}
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
		{
			bool useLightProbes = true;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
		{
			bool useLightProbes = true;
			bool receiveShadows = true;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
		{
			bool useLightProbes = true;
			bool receiveShadows = true;
			bool castShadows = true;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex)
		{
			bool useLightProbes = true;
			bool receiveShadows = true;
			bool castShadows = true;
			MaterialPropertyBlock properties = null;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera)
		{
			bool useLightProbes = true;
			bool receiveShadows = true;
			bool castShadows = true;
			MaterialPropertyBlock properties = null;
			int submeshIndex = 0;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer)
		{
			bool useLightProbes = true;
			bool receiveShadows = true;
			bool castShadows = true;
			MaterialPropertyBlock properties = null;
			int submeshIndex = 0;
			Camera camera = null;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, [UnityEngine.Internal.DefaultValue("null")] Camera camera, [UnityEngine.Internal.DefaultValue("0")] int submeshIndex, [UnityEngine.Internal.DefaultValue("null")] MaterialPropertyBlock properties, [UnityEngine.Internal.DefaultValue("true")] bool castShadows, [UnityEngine.Internal.DefaultValue("true")] bool receiveShadows, [UnityEngine.Internal.DefaultValue("true")] bool useLightProbes)
		{
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows, null, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
		{
			bool useLightProbes = true;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			bool useLightProbes = true;
			Transform probeAnchor = null;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			bool useLightProbes = true;
			Transform probeAnchor = null;
			bool receiveShadows = true;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
		}

		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [UnityEngine.Internal.DefaultValue("true")] bool receiveShadows, [UnityEngine.Internal.DefaultValue("null")] Transform probeAnchor, [UnityEngine.Internal.DefaultValue("true")] bool useLightProbes)
		{
			Internal_DrawMeshTRArguments internal_DrawMeshTRArguments = default(Internal_DrawMeshTRArguments);
			internal_DrawMeshTRArguments.position = position;
			internal_DrawMeshTRArguments.rotation = rotation;
			internal_DrawMeshTRArguments.layer = layer;
			internal_DrawMeshTRArguments.submeshIndex = submeshIndex;
			internal_DrawMeshTRArguments.castShadows = (int)castShadows;
			internal_DrawMeshTRArguments.receiveShadows = ((!receiveShadows) ? 0 : 1);
			internal_DrawMeshTRArguments.reflectionProbeAnchorInstanceID = ((!(probeAnchor != null)) ? 0 : probeAnchor.GetInstanceID());
			internal_DrawMeshTRArguments.useLightProbes = useLightProbes;
			Graphics.Internal_DrawMeshTR(ref internal_DrawMeshTRArguments, properties, material, mesh, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
		{
			bool useLightProbes = true;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
		{
			bool useLightProbes = true;
			bool receiveShadows = true;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
		{
			bool useLightProbes = true;
			bool receiveShadows = true;
			bool castShadows = true;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex)
		{
			bool useLightProbes = true;
			bool receiveShadows = true;
			bool castShadows = true;
			MaterialPropertyBlock properties = null;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera)
		{
			bool useLightProbes = true;
			bool receiveShadows = true;
			bool castShadows = true;
			MaterialPropertyBlock properties = null;
			int submeshIndex = 0;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer)
		{
			bool useLightProbes = true;
			bool receiveShadows = true;
			bool castShadows = true;
			MaterialPropertyBlock properties = null;
			int submeshIndex = 0;
			Camera camera = null;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
		}

		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, [UnityEngine.Internal.DefaultValue("null")] Camera camera, [UnityEngine.Internal.DefaultValue("0")] int submeshIndex, [UnityEngine.Internal.DefaultValue("null")] MaterialPropertyBlock properties, [UnityEngine.Internal.DefaultValue("true")] bool castShadows, [UnityEngine.Internal.DefaultValue("true")] bool receiveShadows, [UnityEngine.Internal.DefaultValue("true")] bool useLightProbes)
		{
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows, null, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
		{
			bool useLightProbes = true;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			bool useLightProbes = true;
			Transform probeAnchor = null;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			bool useLightProbes = true;
			Transform probeAnchor = null;
			bool receiveShadows = true;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
		}

		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [UnityEngine.Internal.DefaultValue("true")] bool receiveShadows, [UnityEngine.Internal.DefaultValue("null")] Transform probeAnchor, [UnityEngine.Internal.DefaultValue("true")] bool useLightProbes)
		{
			Internal_DrawMeshMatrixArguments internal_DrawMeshMatrixArguments = default(Internal_DrawMeshMatrixArguments);
			internal_DrawMeshMatrixArguments.matrix = matrix;
			internal_DrawMeshMatrixArguments.layer = layer;
			internal_DrawMeshMatrixArguments.submeshIndex = submeshIndex;
			internal_DrawMeshMatrixArguments.castShadows = (int)castShadows;
			internal_DrawMeshMatrixArguments.receiveShadows = ((!receiveShadows) ? 0 : 1);
			internal_DrawMeshMatrixArguments.reflectionProbeAnchorInstanceID = ((!(probeAnchor != null)) ? 0 : probeAnchor.GetInstanceID());
			internal_DrawMeshMatrixArguments.useLightProbes = useLightProbes;
			Graphics.Internal_DrawMeshMatrix(ref internal_DrawMeshMatrixArguments, properties, material, mesh, camera);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshTR(ref Internal_DrawMeshTRArguments arguments, MaterialPropertyBlock properties, Material material, Mesh mesh, Camera camera);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshMatrix(ref Internal_DrawMeshMatrixArguments arguments, MaterialPropertyBlock properties, Material material, Mesh mesh, Camera camera);

		public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation)
		{
			Graphics.Internal_DrawMeshNow1(mesh, position, rotation, -1);
		}

		public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
		{
			Graphics.Internal_DrawMeshNow1(mesh, position, rotation, materialIndex);
		}

		public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix)
		{
			Graphics.Internal_DrawMeshNow2(mesh, matrix, -1);
		}

		public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix, int materialIndex)
		{
			Graphics.Internal_DrawMeshNow2(mesh, matrix, materialIndex);
		}

		private static void Internal_DrawMeshNow1(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
		{
			Graphics.INTERNAL_CALL_Internal_DrawMeshNow1(mesh, ref position, ref rotation, materialIndex);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DrawMeshNow1(Mesh mesh, ref Vector3 position, ref Quaternion rotation, int materialIndex);

		private static void Internal_DrawMeshNow2(Mesh mesh, Matrix4x4 matrix, int materialIndex)
		{
			Graphics.INTERNAL_CALL_Internal_DrawMeshNow2(mesh, ref matrix, materialIndex);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DrawMeshNow2(Mesh mesh, ref Matrix4x4 matrix, int materialIndex);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DrawProcedural(MeshTopology topology, int vertexCount, [UnityEngine.Internal.DefaultValue("1")] int instanceCount);

		[ExcludeFromDocs]
		public static void DrawProcedural(MeshTopology topology, int vertexCount)
		{
			int instanceCount = 1;
			Graphics.DrawProcedural(topology, vertexCount, instanceCount);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs, [UnityEngine.Internal.DefaultValue("0")] int argsOffset);

		[ExcludeFromDocs]
		public static void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs)
		{
			int argsOffset = 0;
			Graphics.DrawProceduralIndirect(topology, bufferWithArgs, argsOffset);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture)
		{
			Material mat = null;
			Graphics.DrawTexture(screenRect, texture, mat);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, [UnityEngine.Internal.DefaultValue("null")] Material mat)
		{
			Graphics.DrawTexture(screenRect, texture, 0, 0, 0, 0, mat);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
		{
			Material mat = null;
			Graphics.DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [UnityEngine.Internal.DefaultValue("null")] Material mat)
		{
			Graphics.DrawTexture(screenRect, texture, new Rect(0f, 0f, 1f, 1f), leftBorder, rightBorder, topBorder, bottomBorder, mat);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
		{
			Material mat = null;
			Graphics.DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, mat);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [UnityEngine.Internal.DefaultValue("null")] Material mat)
		{
			InternalDrawTextureArguments internalDrawTextureArguments = default(InternalDrawTextureArguments);
			internalDrawTextureArguments.screenRect = screenRect;
			internalDrawTextureArguments.texture = texture;
			internalDrawTextureArguments.sourceRect = sourceRect;
			internalDrawTextureArguments.leftBorder = leftBorder;
			internalDrawTextureArguments.rightBorder = rightBorder;
			internalDrawTextureArguments.topBorder = topBorder;
			internalDrawTextureArguments.bottomBorder = bottomBorder;
			Color32 color = default(Color32);
			color.r = (color.g = (color.b = (color.a = 128)));
			internalDrawTextureArguments.color = color;
			internalDrawTextureArguments.mat = mat;
			Graphics.DrawTexture(ref internalDrawTextureArguments);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color)
		{
			Material mat = null;
			Graphics.DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, [UnityEngine.Internal.DefaultValue("null")] Material mat)
		{
			InternalDrawTextureArguments internalDrawTextureArguments = default(InternalDrawTextureArguments);
			internalDrawTextureArguments.screenRect = screenRect;
			internalDrawTextureArguments.texture = texture;
			internalDrawTextureArguments.sourceRect = sourceRect;
			internalDrawTextureArguments.leftBorder = leftBorder;
			internalDrawTextureArguments.rightBorder = rightBorder;
			internalDrawTextureArguments.topBorder = topBorder;
			internalDrawTextureArguments.bottomBorder = bottomBorder;
			internalDrawTextureArguments.color = color;
			internalDrawTextureArguments.mat = mat;
			Graphics.DrawTexture(ref internalDrawTextureArguments);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DrawTexture(ref InternalDrawTextureArguments arguments);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ExecuteCommandBuffer(CommandBuffer buffer);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Blit(Texture source, RenderTexture dest);

		[ExcludeFromDocs]
		public static void Blit(Texture source, RenderTexture dest, Material mat)
		{
			int pass = -1;
			Graphics.Blit(source, dest, mat, pass);
		}

		public static void Blit(Texture source, RenderTexture dest, Material mat, [UnityEngine.Internal.DefaultValue("-1")] int pass)
		{
			Graphics.Internal_BlitMaterial(source, dest, mat, pass, true);
		}

		[ExcludeFromDocs]
		public static void Blit(Texture source, Material mat)
		{
			int pass = -1;
			Graphics.Blit(source, mat, pass);
		}

		public static void Blit(Texture source, Material mat, [UnityEngine.Internal.DefaultValue("-1")] int pass)
		{
			Graphics.Internal_BlitMaterial(source, null, mat, pass, false);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BlitMaterial(Texture source, RenderTexture dest, Material mat, int pass, bool setRT);

		public static void BlitMultiTap(Texture source, RenderTexture dest, Material mat, params Vector2[] offsets)
		{
			Graphics.Internal_BlitMultiTap(source, dest, mat, offsets);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BlitMultiTap(Texture source, RenderTexture dest, Material mat, Vector2[] offsets);

		public static void CopyTexture(Texture src, Texture dst)
		{
			Graphics.CopyTexture_Full(src, dst);
		}

		public static void CopyTexture(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip)
		{
			Graphics.CopyTexture_Slice(src, srcElement, srcMip, dst, dstElement, dstMip);
		}

		public static void CopyTexture(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY)
		{
			Graphics.CopyTexture_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dst, dstElement, dstMip, dstX, dstY);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Full(Texture src, Texture dst);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Slice(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetNullRT();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRTSimple(out RenderBuffer color, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetMRTFullSetup(RenderBuffer[] colorSA, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice, RenderBufferLoadAction[] colorLoadSA, RenderBufferStoreAction[] colorStoreSA, RenderBufferLoadAction depthLoad, RenderBufferStoreAction depthStore);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetMRTSimple(RenderBuffer[] colorSA, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveColorBuffer(out RenderBuffer res);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveDepthBuffer(out RenderBuffer res);

		public static void SetRandomWriteTarget(int index, RenderTexture uav)
		{
			Graphics.Internal_SetRandomWriteTargetRT(index, uav);
		}

		[ExcludeFromDocs]
		public static void SetRandomWriteTarget(int index, ComputeBuffer uav)
		{
			bool preserveCounterValue = false;
			Graphics.SetRandomWriteTarget(index, uav, preserveCounterValue);
		}

		public static void SetRandomWriteTarget(int index, ComputeBuffer uav, [UnityEngine.Internal.DefaultValue("false")] bool preserveCounterValue)
		{
			if (uav == null)
			{
				throw new ArgumentNullException("uav");
			}
			if (uav.m_Ptr == IntPtr.Zero)
			{
				throw new ObjectDisposedException("uav");
			}
			Graphics.Internal_SetRandomWriteTargetBuffer(index, uav, preserveCounterValue);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearRandomWriteTargets();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRandomWriteTargetRT(int index, RenderTexture uav);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRandomWriteTargetBuffer(int index, ComputeBuffer uav, bool preserveCounterValue);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetupVertexLights(Light[] lights);

		internal static void CheckLoadActionValid(RenderBufferLoadAction load, string bufferType)
		{
			if (load != RenderBufferLoadAction.Load && load != RenderBufferLoadAction.DontCare)
			{
				throw new ArgumentException(UnityString.Format("Bad {0} LoadAction provided.", new object[]
				{
					bufferType
				}));
			}
		}

		internal static void CheckStoreActionValid(RenderBufferStoreAction store, string bufferType)
		{
			if (store != RenderBufferStoreAction.Store && store != RenderBufferStoreAction.DontCare)
			{
				throw new ArgumentException(UnityString.Format("Bad {0} StoreAction provided.", new object[]
				{
					bufferType
				}));
			}
		}

		internal static void SetRenderTargetImpl(RenderTargetSetup setup)
		{
			if (setup.color.Length == 0)
			{
				throw new ArgumentException("Invalid color buffer count for SetRenderTarget");
			}
			if (setup.color.Length != setup.colorLoad.Length)
			{
				throw new ArgumentException("Color LoadAction and Buffer arrays have different sizes");
			}
			if (setup.color.Length != setup.colorStore.Length)
			{
				throw new ArgumentException("Color StoreAction and Buffer arrays have different sizes");
			}
			RenderBufferLoadAction[] colorLoad = setup.colorLoad;
			for (int i = 0; i < colorLoad.Length; i++)
			{
				RenderBufferLoadAction load = colorLoad[i];
				Graphics.CheckLoadActionValid(load, "Color");
			}
			RenderBufferStoreAction[] colorStore = setup.colorStore;
			for (int j = 0; j < colorStore.Length; j++)
			{
				RenderBufferStoreAction store = colorStore[j];
				Graphics.CheckStoreActionValid(store, "Color");
			}
			Graphics.CheckLoadActionValid(setup.depthLoad, "Depth");
			Graphics.CheckStoreActionValid(setup.depthStore, "Depth");
			if (setup.cubemapFace < CubemapFace.Unknown || setup.cubemapFace > CubemapFace.NegativeZ)
			{
				throw new ArgumentException("Bad CubemapFace provided");
			}
			Graphics.Internal_SetMRTFullSetup(setup.color, out setup.depth, setup.mipLevel, setup.cubemapFace, setup.depthSlice, setup.colorLoad, setup.colorStore, setup.depthLoad, setup.depthStore);
		}

		internal static void SetRenderTargetImpl(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
		{
			RenderBuffer renderBuffer = colorBuffer;
			RenderBuffer renderBuffer2 = depthBuffer;
			Graphics.Internal_SetRTSimple(out renderBuffer, out renderBuffer2, mipLevel, face, depthSlice);
		}

		internal static void SetRenderTargetImpl(RenderTexture rt, int mipLevel, CubemapFace face, int depthSlice)
		{
			if (rt)
			{
				Graphics.SetRenderTargetImpl(rt.colorBuffer, rt.depthBuffer, mipLevel, face, depthSlice);
			}
			else
			{
				Graphics.Internal_SetNullRT();
			}
		}

		internal static void SetRenderTargetImpl(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
		{
			RenderBuffer renderBuffer = depthBuffer;
			Graphics.Internal_SetMRTSimple(colorBuffers, out renderBuffer, mipLevel, face, depthSlice);
		}

		public static void SetRenderTarget(RenderTexture rt)
		{
			Graphics.SetRenderTargetImpl(rt, 0, CubemapFace.Unknown, 0);
		}

		public static void SetRenderTarget(RenderTexture rt, int mipLevel)
		{
			Graphics.SetRenderTargetImpl(rt, mipLevel, CubemapFace.Unknown, 0);
		}

		public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face)
		{
			Graphics.SetRenderTargetImpl(rt, mipLevel, face, 0);
		}

		public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face, int depthSlice)
		{
			Graphics.SetRenderTargetImpl(rt, mipLevel, face, depthSlice);
		}

		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
		{
			Graphics.SetRenderTargetImpl(colorBuffer, depthBuffer, 0, CubemapFace.Unknown, 0);
		}

		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel)
		{
			Graphics.SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, CubemapFace.Unknown, 0);
		}

		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face)
		{
			Graphics.SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, face, 0);
		}

		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
		{
			Graphics.SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, face, depthSlice);
		}

		public static void SetRenderTarget(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer)
		{
			Graphics.SetRenderTargetImpl(colorBuffers, depthBuffer, 0, CubemapFace.Unknown, 0);
		}

		public static void SetRenderTarget(RenderTargetSetup setup)
		{
			Graphics.SetRenderTargetImpl(setup);
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, int materialIndex)
		{
		}
	}
}
