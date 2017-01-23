using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public sealed class Camera : Behaviour
	{
		public enum StereoscopicEye
		{
			Left,
			Right
		}

		public enum MonoOrStereoscopicEye
		{
			Left,
			Right,
			Mono
		}

		public delegate void CameraCallback(Camera cam);

		public static Camera.CameraCallback onPreCull;

		public static Camera.CameraCallback onPreRender;

		public static Camera.CameraCallback onPostRender;

		[Obsolete("use Camera.fieldOfView instead.")]
		public float fov
		{
			get
			{
				return this.fieldOfView;
			}
			set
			{
				this.fieldOfView = value;
			}
		}

		[Obsolete("use Camera.nearClipPlane instead.")]
		public float near
		{
			get
			{
				return this.nearClipPlane;
			}
			set
			{
				this.nearClipPlane = value;
			}
		}

		[Obsolete("use Camera.farClipPlane instead.")]
		public float far
		{
			get
			{
				return this.farClipPlane;
			}
			set
			{
				this.farClipPlane = value;
			}
		}

		public extern float fieldOfView
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float nearClipPlane
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float farClipPlane
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern RenderingPath renderingPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern RenderingPath actualRenderingPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hdr
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float orthographicSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool orthographic
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern OpaqueSortMode opaqueSortMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TransparencySortMode transparencySortMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float depth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float aspect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int cullingMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern int PreviewCullingLayer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int eventMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Color backgroundColor
		{
			get
			{
				Color result;
				this.INTERNAL_get_backgroundColor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_backgroundColor(ref value);
			}
		}

		public Rect rect
		{
			get
			{
				Rect result;
				this.INTERNAL_get_rect(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_rect(ref value);
			}
		}

		public Rect pixelRect
		{
			get
			{
				Rect result;
				this.INTERNAL_get_pixelRect(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_pixelRect(ref value);
			}
		}

		public extern RenderTexture targetTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int pixelWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int pixelHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Matrix4x4 cameraToWorldMatrix
		{
			get
			{
				Matrix4x4 result;
				this.INTERNAL_get_cameraToWorldMatrix(out result);
				return result;
			}
		}

		public Matrix4x4 worldToCameraMatrix
		{
			get
			{
				Matrix4x4 result;
				this.INTERNAL_get_worldToCameraMatrix(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_worldToCameraMatrix(ref value);
			}
		}

		public Matrix4x4 projectionMatrix
		{
			get
			{
				Matrix4x4 result;
				this.INTERNAL_get_projectionMatrix(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_projectionMatrix(ref value);
			}
		}

		public Matrix4x4 nonJitteredProjectionMatrix
		{
			get
			{
				Matrix4x4 result;
				this.INTERNAL_get_nonJitteredProjectionMatrix(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_nonJitteredProjectionMatrix(ref value);
			}
		}

		public extern bool useJitteredProjectionMatrixForTransparentRendering
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 velocity
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_velocity(out result);
				return result;
			}
		}

		public extern CameraClearFlags clearFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool stereoEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float stereoSeparation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float stereoConvergence
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CameraType cameraType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool stereoMirrorMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern StereoTargetEyeMask stereoTargetEye
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Camera.MonoOrStereoscopicEye stereoActiveEye
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int targetDisplay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Camera main
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern Camera current
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern Camera[] allCameras
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int allCamerasCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool useOcclusionCulling
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Matrix4x4 cullingMatrix
		{
			get
			{
				Matrix4x4 result;
				this.INTERNAL_get_cullingMatrix(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_cullingMatrix(ref value);
			}
		}

		public extern float[] layerCullDistances
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool layerCullSpherical
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern DepthTextureMode depthTextureMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool clearStencilAfterLightingPass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int commandBufferCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property isOrthoGraphic has been deprecated. Use orthographic (UnityUpgradable) -> orthographic", true)]
		public bool isOrthoGraphic
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property mainCamera has been deprecated. Use Camera.main instead (UnityUpgradable) -> main", true)]
		public static Camera mainCamera
		{
			get
			{
				return null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string[] GetHDRWarnings();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_backgroundColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_backgroundColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rect(out Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_rect(ref Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_pixelRect(out Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_pixelRect(ref Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTargetBuffersImpl(out RenderBuffer color, out RenderBuffer depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTargetBuffersMRTImpl(RenderBuffer[] color, out RenderBuffer depth);

		public void SetTargetBuffers(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
		{
			this.SetTargetBuffersImpl(out colorBuffer, out depthBuffer);
		}

		public void SetTargetBuffers(RenderBuffer[] colorBuffer, RenderBuffer depthBuffer)
		{
			this.SetTargetBuffersMRTImpl(colorBuffer, out depthBuffer);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_cameraToWorldMatrix(out Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_worldToCameraMatrix(out Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_worldToCameraMatrix(ref Matrix4x4 value);

		public void ResetWorldToCameraMatrix()
		{
			Camera.INTERNAL_CALL_ResetWorldToCameraMatrix(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ResetWorldToCameraMatrix(Camera self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_projectionMatrix(out Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_projectionMatrix(ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_nonJitteredProjectionMatrix(out Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_nonJitteredProjectionMatrix(ref Matrix4x4 value);

		public void ResetProjectionMatrix()
		{
			Camera.INTERNAL_CALL_ResetProjectionMatrix(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ResetProjectionMatrix(Camera self);

		public void ResetAspect()
		{
			Camera.INTERNAL_CALL_ResetAspect(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ResetAspect(Camera self);

		public void ResetFieldOfView()
		{
			Camera.INTERNAL_CALL_ResetFieldOfView(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ResetFieldOfView(Camera self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_velocity(out Vector3 value);

		[Obsolete("GetStereoViewMatrices is deprecated. Use GetStereoViewMatrix(StereoscopicEye eye) instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Matrix4x4[] GetStereoViewMatrices();

		public Matrix4x4 GetStereoViewMatrix(Camera.StereoscopicEye eye)
		{
			Matrix4x4 result;
			Camera.INTERNAL_CALL_GetStereoViewMatrix(this, eye, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetStereoViewMatrix(Camera self, Camera.StereoscopicEye eye, out Matrix4x4 value);

		[Obsolete("SetStereoViewMatrices is deprecated. Use SetStereoViewMatrix(StereoscopicEye eye) instead.")]
		public void SetStereoViewMatrices(Matrix4x4 leftMatrix, Matrix4x4 rightMatrix)
		{
			Camera.INTERNAL_CALL_SetStereoViewMatrices(this, ref leftMatrix, ref rightMatrix);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetStereoViewMatrices(Camera self, ref Matrix4x4 leftMatrix, ref Matrix4x4 rightMatrix);

		public void SetStereoViewMatrix(Camera.StereoscopicEye eye, Matrix4x4 matrix)
		{
			Camera.INTERNAL_CALL_SetStereoViewMatrix(this, eye, ref matrix);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetStereoViewMatrix(Camera self, Camera.StereoscopicEye eye, ref Matrix4x4 matrix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetStereoViewMatrices();

		[Obsolete("GetStereoProjectionMatrices is deprecated. Use GetStereoProjectionMatrix(StereoscopicEye eye) instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Matrix4x4[] GetStereoProjectionMatrices();

		public Matrix4x4 GetStereoProjectionMatrix(Camera.StereoscopicEye eye)
		{
			Matrix4x4 result;
			Camera.INTERNAL_CALL_GetStereoProjectionMatrix(this, eye, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetStereoProjectionMatrix(Camera self, Camera.StereoscopicEye eye, out Matrix4x4 value);

		public void SetStereoProjectionMatrix(Camera.StereoscopicEye eye, Matrix4x4 matrix)
		{
			Camera.INTERNAL_CALL_SetStereoProjectionMatrix(this, eye, ref matrix);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetStereoProjectionMatrix(Camera self, Camera.StereoscopicEye eye, ref Matrix4x4 matrix);

		[Obsolete("SetStereoProjectionMatrices is deprecated. Use SetStereoProjectionMatrix(StereoscopicEye eye) instead.")]
		public void SetStereoProjectionMatrices(Matrix4x4 leftMatrix, Matrix4x4 rightMatrix)
		{
			Camera.INTERNAL_CALL_SetStereoProjectionMatrices(this, ref leftMatrix, ref rightMatrix);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetStereoProjectionMatrices(Camera self, ref Matrix4x4 leftMatrix, ref Matrix4x4 rightMatrix);

		public void CalculateFrustumCorners(Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, Vector3[] outCorners)
		{
			if (outCorners == null)
			{
				throw new ArgumentNullException("outCorners");
			}
			if (outCorners.Length < 4)
			{
				throw new ArgumentException("outCorners minimum size is 4", "outCorners");
			}
			this.CalculateFrustumCornersInternal(viewport, z, eye, outCorners);
		}

		private void CalculateFrustumCornersInternal(Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, Vector3[] outCorners)
		{
			Camera.INTERNAL_CALL_CalculateFrustumCornersInternal(this, ref viewport, z, eye, outCorners);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CalculateFrustumCornersInternal(Camera self, ref Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, Vector3[] outCorners);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetStereoProjectionMatrices();

		public Vector3 WorldToScreenPoint(Vector3 position)
		{
			Vector3 result;
			Camera.INTERNAL_CALL_WorldToScreenPoint(this, ref position, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_WorldToScreenPoint(Camera self, ref Vector3 position, out Vector3 value);

		public Vector3 WorldToViewportPoint(Vector3 position)
		{
			Vector3 result;
			Camera.INTERNAL_CALL_WorldToViewportPoint(this, ref position, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_WorldToViewportPoint(Camera self, ref Vector3 position, out Vector3 value);

		public Vector3 ViewportToWorldPoint(Vector3 position)
		{
			Vector3 result;
			Camera.INTERNAL_CALL_ViewportToWorldPoint(this, ref position, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ViewportToWorldPoint(Camera self, ref Vector3 position, out Vector3 value);

		public Vector3 ScreenToWorldPoint(Vector3 position)
		{
			Vector3 result;
			Camera.INTERNAL_CALL_ScreenToWorldPoint(this, ref position, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ScreenToWorldPoint(Camera self, ref Vector3 position, out Vector3 value);

		public Vector3 ScreenToViewportPoint(Vector3 position)
		{
			Vector3 result;
			Camera.INTERNAL_CALL_ScreenToViewportPoint(this, ref position, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ScreenToViewportPoint(Camera self, ref Vector3 position, out Vector3 value);

		public Vector3 ViewportToScreenPoint(Vector3 position)
		{
			Vector3 result;
			Camera.INTERNAL_CALL_ViewportToScreenPoint(this, ref position, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ViewportToScreenPoint(Camera self, ref Vector3 position, out Vector3 value);

		public Ray ViewportPointToRay(Vector3 position)
		{
			Ray result;
			Camera.INTERNAL_CALL_ViewportPointToRay(this, ref position, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ViewportPointToRay(Camera self, ref Vector3 position, out Ray value);

		public Ray ScreenPointToRay(Vector3 position)
		{
			Ray result;
			Camera.INTERNAL_CALL_ScreenPointToRay(this, ref position, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ScreenPointToRay(Camera self, ref Vector3 position, out Ray value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetAllCameras(Camera[] cameras);

		[RequiredByNativeCode]
		private static void FireOnPreCull(Camera cam)
		{
			if (Camera.onPreCull != null)
			{
				Camera.onPreCull(cam);
			}
		}

		[RequiredByNativeCode]
		private static void FireOnPreRender(Camera cam)
		{
			if (Camera.onPreRender != null)
			{
				Camera.onPreRender(cam);
			}
		}

		[RequiredByNativeCode]
		private static void FireOnPostRender(Camera cam)
		{
			if (Camera.onPostRender != null)
			{
				Camera.onPostRender(cam);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Render();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RenderWithShader(Shader shader, string replacementTag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetReplacementShader(Shader shader, string replacementTag);

		public void ResetReplacementShader()
		{
			Camera.INTERNAL_CALL_ResetReplacementShader(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ResetReplacementShader(Camera self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_cullingMatrix(out Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_cullingMatrix(ref Matrix4x4 value);

		public void ResetCullingMatrix()
		{
			Camera.INTERNAL_CALL_ResetCullingMatrix(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ResetCullingMatrix(Camera self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RenderDontRestore();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetupCurrent(Camera cur);

		[ExcludeFromDocs]
		public bool RenderToCubemap(Cubemap cubemap)
		{
			int faceMask = 63;
			return this.RenderToCubemap(cubemap, faceMask);
		}

		public bool RenderToCubemap(Cubemap cubemap, [UnityEngine.Internal.DefaultValue("63")] int faceMask)
		{
			return this.Internal_RenderToCubemapTexture(cubemap, faceMask);
		}

		[ExcludeFromDocs]
		public bool RenderToCubemap(RenderTexture cubemap)
		{
			int faceMask = 63;
			return this.RenderToCubemap(cubemap, faceMask);
		}

		public bool RenderToCubemap(RenderTexture cubemap, [UnityEngine.Internal.DefaultValue("63")] int faceMask)
		{
			return this.Internal_RenderToCubemapRT(cubemap, faceMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_RenderToCubemapRT(RenderTexture cubemap, int faceMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_RenderToCubemapTexture(Cubemap cubemap, int faceMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CopyFrom(Camera other);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsFiltered(GameObject go);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddCommandBuffer(CameraEvent evt, CommandBuffer buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveCommandBuffer(CameraEvent evt, CommandBuffer buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveCommandBuffers(CameraEvent evt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveAllCommandBuffers();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern CommandBuffer[] GetCommandBuffers(CameraEvent evt);

		internal GameObject RaycastTry(Ray ray, float distance, int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			return Camera.INTERNAL_CALL_RaycastTry(this, ref ray, distance, layerMask, queryTriggerInteraction);
		}

		[ExcludeFromDocs]
		internal GameObject RaycastTry(Ray ray, float distance, int layerMask)
		{
			QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
			return Camera.INTERNAL_CALL_RaycastTry(this, ref ray, distance, layerMask, queryTriggerInteraction);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GameObject INTERNAL_CALL_RaycastTry(Camera self, ref Ray ray, float distance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

		internal GameObject RaycastTry2D(Ray ray, float distance, int layerMask)
		{
			return Camera.INTERNAL_CALL_RaycastTry2D(this, ref ray, distance, layerMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GameObject INTERNAL_CALL_RaycastTry2D(Camera self, ref Ray ray, float distance, int layerMask);

		public Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane)
		{
			Matrix4x4 result;
			Camera.INTERNAL_CALL_CalculateObliqueMatrix(this, ref clipPlane, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CalculateObliqueMatrix(Camera self, ref Vector4 clipPlane, out Matrix4x4 value);

		internal void OnlyUsedForTesting1()
		{
		}

		internal void OnlyUsedForTesting2()
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property GetScreenWidth() has been deprecated. Use Screen.width instead (UnityUpgradable) -> Screen.width", true)]
		public float GetScreenWidth()
		{
			return 0f;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property GetScreenHeight() has been deprecated. Use Screen.height instead (UnityUpgradable) -> Screen.height", true)]
		public float GetScreenHeight()
		{
			return 0f;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Camera.DoClear has been deprecated (UnityUpgradable).", true)]
		public void DoClear()
		{
		}
	}
}
