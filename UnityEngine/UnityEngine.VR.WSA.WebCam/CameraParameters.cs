using System;
using System.Linq;

namespace UnityEngine.VR.WSA.WebCam
{
	public struct CameraParameters
	{
		private float m_HologramOpacity;

		private float m_FrameRate;

		private int m_CameraResolutionWidth;

		private int m_CameraResolutionHeight;

		private CapturePixelFormat m_PixelFormat;

		public float hologramOpacity
		{
			get
			{
				return this.m_HologramOpacity;
			}
			set
			{
				this.m_HologramOpacity = value;
			}
		}

		public float frameRate
		{
			get
			{
				return this.m_FrameRate;
			}
			set
			{
				this.m_FrameRate = value;
			}
		}

		public int cameraResolutionWidth
		{
			get
			{
				return this.m_CameraResolutionWidth;
			}
			set
			{
				this.m_CameraResolutionWidth = value;
			}
		}

		public int cameraResolutionHeight
		{
			get
			{
				return this.m_CameraResolutionHeight;
			}
			set
			{
				this.m_CameraResolutionHeight = value;
			}
		}

		public CapturePixelFormat pixelFormat
		{
			get
			{
				return this.m_PixelFormat;
			}
			set
			{
				this.m_PixelFormat = value;
			}
		}

		public CameraParameters(WebCamMode webCamMode)
		{
			this.m_HologramOpacity = 1f;
			this.m_PixelFormat = CapturePixelFormat.BGRA32;
			this.m_FrameRate = 0f;
			this.m_CameraResolutionWidth = 0;
			this.m_CameraResolutionHeight = 0;
			if (webCamMode == WebCamMode.PhotoMode)
			{
				Resolution resolution = (from res in PhotoCapture.SupportedResolutions
				orderby res.width * res.height descending
				select res).First<Resolution>();
				this.m_CameraResolutionWidth = resolution.width;
				this.m_CameraResolutionHeight = resolution.height;
			}
			else if (webCamMode == WebCamMode.VideoMode)
			{
				Resolution resolution2 = (from res in VideoCapture.SupportedResolutions
				orderby res.width * res.height descending
				select res).First<Resolution>();
				float frameRate = (from fps in VideoCapture.GetSupportedFrameRatesForResolution(resolution2)
				orderby fps descending
				select fps).First<float>();
				this.m_CameraResolutionWidth = resolution2.width;
				this.m_CameraResolutionHeight = resolution2.height;
				this.m_FrameRate = frameRate;
			}
		}
	}
}
