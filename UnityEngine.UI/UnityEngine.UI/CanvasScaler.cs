using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("Layout/Canvas Scaler", 101), ExecuteInEditMode, RequireComponent(typeof(Canvas))]
	public class CanvasScaler : UIBehaviour
	{
		public enum ScaleMode
		{
			ConstantPixelSize,
			ScaleWithScreenSize,
			ConstantPhysicalSize
		}

		public enum ScreenMatchMode
		{
			MatchWidthOrHeight,
			Expand,
			Shrink
		}

		public enum Unit
		{
			Centimeters,
			Millimeters,
			Inches,
			Points,
			Picas
		}

		[SerializeField, Tooltip("Determines how UI elements in the Canvas are scaled.")]
		private CanvasScaler.ScaleMode m_UiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

		[SerializeField, Tooltip("If a sprite has this 'Pixels Per Unit' setting, then one pixel in the sprite will cover one unit in the UI.")]
		protected float m_ReferencePixelsPerUnit = 100f;

		[SerializeField, Tooltip("Scales all UI elements in the Canvas by this factor.")]
		protected float m_ScaleFactor = 1f;

		[SerializeField, Tooltip("The resolution the UI layout is designed for. If the screen resolution is larger, the UI will be scaled up, and if it's smaller, the UI will be scaled down. This is done in accordance with the Screen Match Mode.")]
		protected Vector2 m_ReferenceResolution = new Vector2(800f, 600f);

		[SerializeField, Tooltip("A mode used to scale the canvas area if the aspect ratio of the current resolution doesn't fit the reference resolution.")]
		protected CanvasScaler.ScreenMatchMode m_ScreenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

		[Range(0f, 1f), SerializeField, Tooltip("Determines if the scaling is using the width or height as reference, or a mix in between.")]
		protected float m_MatchWidthOrHeight = 0f;

		private const float kLogBase = 2f;

		[SerializeField, Tooltip("The physical unit to specify positions and sizes in.")]
		protected CanvasScaler.Unit m_PhysicalUnit = CanvasScaler.Unit.Points;

		[SerializeField, Tooltip("The DPI to assume if the screen DPI is not known.")]
		protected float m_FallbackScreenDPI = 96f;

		[SerializeField, Tooltip("The pixels per inch to use for sprites that have a 'Pixels Per Unit' setting that matches the 'Reference Pixels Per Unit' setting.")]
		protected float m_DefaultSpriteDPI = 96f;

		[SerializeField, Tooltip("The amount of pixels per unit to use for dynamically created bitmaps in the UI, such as Text.")]
		protected float m_DynamicPixelsPerUnit = 1f;

		private Canvas m_Canvas;

		[NonSerialized]
		private float m_PrevScaleFactor = 1f;

		[NonSerialized]
		private float m_PrevReferencePixelsPerUnit = 100f;

		public CanvasScaler.ScaleMode uiScaleMode
		{
			get
			{
				return this.m_UiScaleMode;
			}
			set
			{
				this.m_UiScaleMode = value;
			}
		}

		public float referencePixelsPerUnit
		{
			get
			{
				return this.m_ReferencePixelsPerUnit;
			}
			set
			{
				this.m_ReferencePixelsPerUnit = value;
			}
		}

		public float scaleFactor
		{
			get
			{
				return this.m_ScaleFactor;
			}
			set
			{
				this.m_ScaleFactor = Mathf.Max(0.01f, value);
			}
		}

		public Vector2 referenceResolution
		{
			get
			{
				return this.m_ReferenceResolution;
			}
			set
			{
				this.m_ReferenceResolution = value;
				if (this.m_ReferenceResolution.x > -1E-05f && this.m_ReferenceResolution.x < 1E-05f)
				{
					this.m_ReferenceResolution.x = 1E-05f * Mathf.Sign(this.m_ReferenceResolution.x);
				}
				if (this.m_ReferenceResolution.y > -1E-05f && this.m_ReferenceResolution.y < 1E-05f)
				{
					this.m_ReferenceResolution.y = 1E-05f * Mathf.Sign(this.m_ReferenceResolution.y);
				}
			}
		}

		public CanvasScaler.ScreenMatchMode screenMatchMode
		{
			get
			{
				return this.m_ScreenMatchMode;
			}
			set
			{
				this.m_ScreenMatchMode = value;
			}
		}

		public float matchWidthOrHeight
		{
			get
			{
				return this.m_MatchWidthOrHeight;
			}
			set
			{
				this.m_MatchWidthOrHeight = value;
			}
		}

		public CanvasScaler.Unit physicalUnit
		{
			get
			{
				return this.m_PhysicalUnit;
			}
			set
			{
				this.m_PhysicalUnit = value;
			}
		}

		public float fallbackScreenDPI
		{
			get
			{
				return this.m_FallbackScreenDPI;
			}
			set
			{
				this.m_FallbackScreenDPI = value;
			}
		}

		public float defaultSpriteDPI
		{
			get
			{
				return this.m_DefaultSpriteDPI;
			}
			set
			{
				this.m_DefaultSpriteDPI = Mathf.Max(1f, value);
			}
		}

		public float dynamicPixelsPerUnit
		{
			get
			{
				return this.m_DynamicPixelsPerUnit;
			}
			set
			{
				this.m_DynamicPixelsPerUnit = value;
			}
		}

		protected CanvasScaler()
		{
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_Canvas = base.GetComponent<Canvas>();
			this.Handle();
		}

		protected override void OnDisable()
		{
			this.SetScaleFactor(1f);
			this.SetReferencePixelsPerUnit(100f);
			base.OnDisable();
		}

		protected virtual void Update()
		{
			this.Handle();
		}

		protected virtual void Handle()
		{
			if (!(this.m_Canvas == null) && this.m_Canvas.isRootCanvas)
			{
				if (this.m_Canvas.renderMode == RenderMode.WorldSpace)
				{
					this.HandleWorldCanvas();
				}
				else
				{
					CanvasScaler.ScaleMode uiScaleMode = this.m_UiScaleMode;
					if (uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize)
					{
						if (uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
						{
							if (uiScaleMode == CanvasScaler.ScaleMode.ConstantPhysicalSize)
							{
								this.HandleConstantPhysicalSize();
							}
						}
						else
						{
							this.HandleScaleWithScreenSize();
						}
					}
					else
					{
						this.HandleConstantPixelSize();
					}
				}
			}
		}

		protected virtual void HandleWorldCanvas()
		{
			this.SetScaleFactor(this.m_DynamicPixelsPerUnit);
			this.SetReferencePixelsPerUnit(this.m_ReferencePixelsPerUnit);
		}

		protected virtual void HandleConstantPixelSize()
		{
			this.SetScaleFactor(this.m_ScaleFactor);
			this.SetReferencePixelsPerUnit(this.m_ReferencePixelsPerUnit);
		}

		protected virtual void HandleScaleWithScreenSize()
		{
			Vector2 vector = new Vector2((float)Screen.width, (float)Screen.height);
			int targetDisplay = this.m_Canvas.targetDisplay;
			if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
			{
				Display display = Display.displays[targetDisplay];
				vector = new Vector2((float)display.renderingWidth, (float)display.renderingHeight);
			}
			float scaleFactor = 0f;
			CanvasScaler.ScreenMatchMode screenMatchMode = this.m_ScreenMatchMode;
			if (screenMatchMode != CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
			{
				if (screenMatchMode != CanvasScaler.ScreenMatchMode.Expand)
				{
					if (screenMatchMode == CanvasScaler.ScreenMatchMode.Shrink)
					{
						scaleFactor = Mathf.Max(vector.x / this.m_ReferenceResolution.x, vector.y / this.m_ReferenceResolution.y);
					}
				}
				else
				{
					scaleFactor = Mathf.Min(vector.x / this.m_ReferenceResolution.x, vector.y / this.m_ReferenceResolution.y);
				}
			}
			else
			{
				float a = Mathf.Log(vector.x / this.m_ReferenceResolution.x, 2f);
				float b = Mathf.Log(vector.y / this.m_ReferenceResolution.y, 2f);
				float p = Mathf.Lerp(a, b, this.m_MatchWidthOrHeight);
				scaleFactor = Mathf.Pow(2f, p);
			}
			this.SetScaleFactor(scaleFactor);
			this.SetReferencePixelsPerUnit(this.m_ReferencePixelsPerUnit);
		}

		protected virtual void HandleConstantPhysicalSize()
		{
			float dpi = Screen.dpi;
			float num = (dpi != 0f) ? dpi : this.m_FallbackScreenDPI;
			float num2 = 1f;
			switch (this.m_PhysicalUnit)
			{
			case CanvasScaler.Unit.Centimeters:
				num2 = 2.54f;
				break;
			case CanvasScaler.Unit.Millimeters:
				num2 = 25.4f;
				break;
			case CanvasScaler.Unit.Inches:
				num2 = 1f;
				break;
			case CanvasScaler.Unit.Points:
				num2 = 72f;
				break;
			case CanvasScaler.Unit.Picas:
				num2 = 6f;
				break;
			}
			this.SetScaleFactor(num / num2);
			this.SetReferencePixelsPerUnit(this.m_ReferencePixelsPerUnit * num2 / this.m_DefaultSpriteDPI);
		}

		protected void SetScaleFactor(float scaleFactor)
		{
			if (scaleFactor != this.m_PrevScaleFactor)
			{
				this.m_Canvas.scaleFactor = scaleFactor;
				this.m_PrevScaleFactor = scaleFactor;
			}
		}

		protected void SetReferencePixelsPerUnit(float referencePixelsPerUnit)
		{
			if (referencePixelsPerUnit != this.m_PrevReferencePixelsPerUnit)
			{
				this.m_Canvas.referencePixelsPerUnit = referencePixelsPerUnit;
				this.m_PrevReferencePixelsPerUnit = referencePixelsPerUnit;
			}
		}

		protected override void OnValidate()
		{
			this.m_ScaleFactor = Mathf.Max(0.01f, this.m_ScaleFactor);
			this.m_DefaultSpriteDPI = Mathf.Max(1f, this.m_DefaultSpriteDPI);
		}
	}
}
