using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Rendering;
namespace UnityEngine
{
	public sealed class Camera : Behaviour
	{
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float nearClipPlane
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float farClipPlane
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern RenderingPath renderingPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern RenderingPath actualRenderingPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern bool hdr
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float orthographicSize
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property isOrthoGraphic has been deprecated. Use orthographic (UnityUpgradable).", true)]
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
		public extern bool orthographic
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern TransparencySortMode transparencySortMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float depth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float aspect
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern int cullingMask
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		internal static extern int PreviewCullingLayer
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern int eventMask
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern int pixelWidth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern int pixelHeight
		{
			[WrapperlessIcall]
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool stereoEnabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern float stereoSeparation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float stereoConvergence
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern int targetDisplay
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public static extern Camera main
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern Camera current
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern Camera[] allCameras
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern int allCamerasCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property mainCamera has been deprecated. Use Camera.main instead (UnityUpgradable).", true)]
		public static Camera mainCamera
		{
			get
			{
				return null;
			}
		}
		public extern bool useOcclusionCulling
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float[] layerCullDistances
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool layerCullSpherical
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern DepthTextureMode depthTextureMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool clearStencilAfterLightingPass
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern int commandBufferCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_backgroundColor(out Color value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_backgroundColor(ref Color value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rect(out Rect value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_rect(ref Rect value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_pixelRect(out Rect value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_pixelRect(ref Rect value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTargetBuffersImpl(out RenderBuffer color, out RenderBuffer depth);
		[WrapperlessIcall]
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
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_cameraToWorldMatrix(out Matrix4x4 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_worldToCameraMatrix(out Matrix4x4 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_worldToCameraMatrix(ref Matrix4x4 value);
		public void ResetWorldToCameraMatrix()
		{
			Camera.INTERNAL_CALL_ResetWorldToCameraMatrix(this);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ResetWorldToCameraMatrix(Camera self);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_projectionMatrix(out Matrix4x4 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_projectionMatrix(ref Matrix4x4 value);
		public void ResetProjectionMatrix()
		{
			Camera.INTERNAL_CALL_ResetProjectionMatrix(this);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ResetProjectionMatrix(Camera self);
		public void ResetAspect()
		{
			Camera.INTERNAL_CALL_ResetAspect(this);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ResetAspect(Camera self);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_velocity(out Vector3 value);
		public Vector3 WorldToScreenPoint(Vector3 position)
		{
			return Camera.INTERNAL_CALL_WorldToScreenPoint(this, ref position);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector3 INTERNAL_CALL_WorldToScreenPoint(Camera self, ref Vector3 position);
		public Vector3 WorldToViewportPoint(Vector3 position)
		{
			return Camera.INTERNAL_CALL_WorldToViewportPoint(this, ref position);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector3 INTERNAL_CALL_WorldToViewportPoint(Camera self, ref Vector3 position);
		public Vector3 ViewportToWorldPoint(Vector3 position)
		{
			return Camera.INTERNAL_CALL_ViewportToWorldPoint(this, ref position);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector3 INTERNAL_CALL_ViewportToWorldPoint(Camera self, ref Vector3 position);
		public Vector3 ScreenToWorldPoint(Vector3 position)
		{
			return Camera.INTERNAL_CALL_ScreenToWorldPoint(this, ref position);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector3 INTERNAL_CALL_ScreenToWorldPoint(Camera self, ref Vector3 position);
		public Vector3 ScreenToViewportPoint(Vector3 position)
		{
			return Camera.INTERNAL_CALL_ScreenToViewportPoint(this, ref position);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector3 INTERNAL_CALL_ScreenToViewportPoint(Camera self, ref Vector3 position);
		public Vector3 ViewportToScreenPoint(Vector3 position)
		{
			return Camera.INTERNAL_CALL_ViewportToScreenPoint(this, ref position);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector3 INTERNAL_CALL_ViewportToScreenPoint(Camera self, ref Vector3 position);
		public Ray ViewportPointToRay(Vector3 position)
		{
			return Camera.INTERNAL_CALL_ViewportPointToRay(this, ref position);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Ray INTERNAL_CALL_ViewportPointToRay(Camera self, ref Vector3 position);
		public Ray ScreenPointToRay(Vector3 position)
		{
			return Camera.INTERNAL_CALL_ScreenPointToRay(this, ref position);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Ray INTERNAL_CALL_ScreenPointToRay(Camera self, ref Vector3 position);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetAllCameras(Camera[] cameras);
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property GetScreenWidth() has been deprecated. Use Screen.width instead (UnityUpgradable).", true), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetScreenWidth();
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property GetScreenHeight() has been deprecated. Use Screen.height instead (UnityUpgradable).", true), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetScreenHeight();
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Camera.DoClear has been deprecated (UnityUpgradable).", true), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DoClear();
		private static void FireOnPreCull(Camera cam)
		{
			if (Camera.onPreCull != null)
			{
				Camera.onPreCull(cam);
			}
		}
		private static void FireOnPreRender(Camera cam)
		{
			if (Camera.onPreRender != null)
			{
				Camera.onPreRender(cam);
			}
		}
		private static void FireOnPostRender(Camera cam)
		{
			if (Camera.onPostRender != null)
			{
				Camera.onPostRender(cam);
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Render();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RenderWithShader(Shader shader, string replacementTag);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetReplacementShader(Shader shader, string replacementTag);
		public void ResetReplacementShader()
		{
			Camera.INTERNAL_CALL_ResetReplacementShader(this);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ResetReplacementShader(Camera self);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RenderDontRestore();
		[WrapperlessIcall]
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
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_RenderToCubemapRT(RenderTexture cubemap, int faceMask);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_RenderToCubemapTexture(Cubemap cubemap, int faceMask);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CopyFrom(Camera other);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsFiltered(GameObject go);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddCommandBuffer(CameraEvent evt, CommandBuffer buffer);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveCommandBuffer(CameraEvent evt, CommandBuffer buffer);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveCommandBuffers(CameraEvent evt);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveAllCommandBuffers();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern CommandBuffer[] GetCommandBuffers(CameraEvent evt);
		internal GameObject RaycastTry(Ray ray, float distance, int layerMask)
		{
			return Camera.INTERNAL_CALL_RaycastTry(this, ref ray, distance, layerMask);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GameObject INTERNAL_CALL_RaycastTry(Camera self, ref Ray ray, float distance, int layerMask);
		internal GameObject RaycastTry2D(Ray ray, float distance, int layerMask)
		{
			return Camera.INTERNAL_CALL_RaycastTry2D(this, ref ray, distance, layerMask);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GameObject INTERNAL_CALL_RaycastTry2D(Camera self, ref Ray ray, float distance, int layerMask);
		public Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane)
		{
			return Camera.INTERNAL_CALL_CalculateObliqueMatrix(this, ref clipPlane);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Matrix4x4 INTERNAL_CALL_CalculateObliqueMatrix(Camera self, ref Vector4 clipPlane);
		internal void OnlyUsedForTesting1()
		{
		}
		internal void OnlyUsedForTesting2()
		{
		}
	}
}
