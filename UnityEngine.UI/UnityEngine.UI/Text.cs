using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Text", 10)]
	public class Text : MaskableGraphic, ILayoutElement
	{
		[SerializeField]
		private FontData m_FontData = FontData.defaultFontData;

		private Font m_LastTrackedFont;

		[SerializeField, TextArea(3, 10)]
		protected string m_Text = string.Empty;

		private TextGenerator m_TextCache;

		private TextGenerator m_TextCacheForLayout;

		protected static Material s_DefaultText = null;

		[NonSerialized]
		protected bool m_DisableFontTextureRebuiltCallback = false;

		private readonly UIVertex[] m_TempVerts = new UIVertex[4];

		public TextGenerator cachedTextGenerator
		{
			get
			{
				TextGenerator arg_41_0;
				if ((arg_41_0 = this.m_TextCache) == null)
				{
					arg_41_0 = (this.m_TextCache = ((this.m_Text.Length == 0) ? new TextGenerator() : new TextGenerator(this.m_Text.Length)));
				}
				return arg_41_0;
			}
		}

		public TextGenerator cachedTextGeneratorForLayout
		{
			get
			{
				TextGenerator arg_1C_0;
				if ((arg_1C_0 = this.m_TextCacheForLayout) == null)
				{
					arg_1C_0 = (this.m_TextCacheForLayout = new TextGenerator());
				}
				return arg_1C_0;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				Texture mainTexture;
				if (this.font != null && this.font.material != null && this.font.material.mainTexture != null)
				{
					mainTexture = this.font.material.mainTexture;
				}
				else if (this.m_Material != null)
				{
					mainTexture = this.m_Material.mainTexture;
				}
				else
				{
					mainTexture = base.mainTexture;
				}
				return mainTexture;
			}
		}

		public Font font
		{
			get
			{
				return this.m_FontData.font;
			}
			set
			{
				if (!(this.m_FontData.font == value))
				{
					FontUpdateTracker.UntrackText(this);
					this.m_FontData.font = value;
					FontUpdateTracker.TrackText(this);
					this.m_LastTrackedFont = value;
					this.SetAllDirty();
				}
			}
		}

		public virtual string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					if (!string.IsNullOrEmpty(this.m_Text))
					{
						this.m_Text = "";
						this.SetVerticesDirty();
					}
				}
				else if (this.m_Text != value)
				{
					this.m_Text = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public bool supportRichText
		{
			get
			{
				return this.m_FontData.richText;
			}
			set
			{
				if (this.m_FontData.richText != value)
				{
					this.m_FontData.richText = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public bool resizeTextForBestFit
		{
			get
			{
				return this.m_FontData.bestFit;
			}
			set
			{
				if (this.m_FontData.bestFit != value)
				{
					this.m_FontData.bestFit = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public int resizeTextMinSize
		{
			get
			{
				return this.m_FontData.minSize;
			}
			set
			{
				if (this.m_FontData.minSize != value)
				{
					this.m_FontData.minSize = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public int resizeTextMaxSize
		{
			get
			{
				return this.m_FontData.maxSize;
			}
			set
			{
				if (this.m_FontData.maxSize != value)
				{
					this.m_FontData.maxSize = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public TextAnchor alignment
		{
			get
			{
				return this.m_FontData.alignment;
			}
			set
			{
				if (this.m_FontData.alignment != value)
				{
					this.m_FontData.alignment = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public bool alignByGeometry
		{
			get
			{
				return this.m_FontData.alignByGeometry;
			}
			set
			{
				if (this.m_FontData.alignByGeometry != value)
				{
					this.m_FontData.alignByGeometry = value;
					this.SetVerticesDirty();
				}
			}
		}

		public int fontSize
		{
			get
			{
				return this.m_FontData.fontSize;
			}
			set
			{
				if (this.m_FontData.fontSize != value)
				{
					this.m_FontData.fontSize = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public HorizontalWrapMode horizontalOverflow
		{
			get
			{
				return this.m_FontData.horizontalOverflow;
			}
			set
			{
				if (this.m_FontData.horizontalOverflow != value)
				{
					this.m_FontData.horizontalOverflow = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public VerticalWrapMode verticalOverflow
		{
			get
			{
				return this.m_FontData.verticalOverflow;
			}
			set
			{
				if (this.m_FontData.verticalOverflow != value)
				{
					this.m_FontData.verticalOverflow = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public float lineSpacing
		{
			get
			{
				return this.m_FontData.lineSpacing;
			}
			set
			{
				if (this.m_FontData.lineSpacing != value)
				{
					this.m_FontData.lineSpacing = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public FontStyle fontStyle
		{
			get
			{
				return this.m_FontData.fontStyle;
			}
			set
			{
				if (this.m_FontData.fontStyle != value)
				{
					this.m_FontData.fontStyle = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public float pixelsPerUnit
		{
			get
			{
				Canvas canvas = base.canvas;
				float result;
				if (!canvas)
				{
					result = 1f;
				}
				else if (!this.font || this.font.dynamic)
				{
					result = canvas.scaleFactor;
				}
				else if (this.m_FontData.fontSize <= 0 || this.font.fontSize <= 0)
				{
					result = 1f;
				}
				else
				{
					result = (float)this.font.fontSize / (float)this.m_FontData.fontSize;
				}
				return result;
			}
		}

		public virtual float minWidth
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				TextGenerationSettings generationSettings = this.GetGenerationSettings(Vector2.zero);
				return this.cachedTextGeneratorForLayout.GetPreferredWidth(this.m_Text, generationSettings) / this.pixelsPerUnit;
			}
		}

		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		public virtual float minHeight
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				TextGenerationSettings generationSettings = this.GetGenerationSettings(new Vector2(base.GetPixelAdjustedRect().size.x, 0f));
				return this.cachedTextGeneratorForLayout.GetPreferredHeight(this.m_Text, generationSettings) / this.pixelsPerUnit;
			}
		}

		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		public virtual int layoutPriority
		{
			get
			{
				return 0;
			}
		}

		protected Text()
		{
			base.useLegacyMeshGeneration = false;
		}

		public void FontTextureChanged()
		{
			if (this)
			{
				if (!this.m_DisableFontTextureRebuiltCallback)
				{
					this.cachedTextGenerator.Invalidate();
					if (this.IsActive())
					{
						if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
						{
							this.UpdateGeometry();
						}
						else
						{
							this.SetAllDirty();
						}
					}
				}
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.cachedTextGenerator.Invalidate();
			FontUpdateTracker.TrackText(this);
		}

		protected override void OnDisable()
		{
			FontUpdateTracker.UntrackText(this);
			base.OnDisable();
		}

		protected override void UpdateGeometry()
		{
			if (this.font != null)
			{
				base.UpdateGeometry();
			}
		}

		protected override void Reset()
		{
			this.AssignDefaultFont();
		}

		internal void AssignDefaultFont()
		{
			this.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		}

		public TextGenerationSettings GetGenerationSettings(Vector2 extents)
		{
			TextGenerationSettings result = default(TextGenerationSettings);
			result.generationExtents = extents;
			if (this.font != null && this.font.dynamic)
			{
				result.fontSize = this.m_FontData.fontSize;
				result.resizeTextMinSize = this.m_FontData.minSize;
				result.resizeTextMaxSize = this.m_FontData.maxSize;
			}
			result.textAnchor = this.m_FontData.alignment;
			result.alignByGeometry = this.m_FontData.alignByGeometry;
			result.scaleFactor = this.pixelsPerUnit;
			result.color = this.color;
			result.font = this.font;
			result.pivot = base.rectTransform.pivot;
			result.richText = this.m_FontData.richText;
			result.lineSpacing = this.m_FontData.lineSpacing;
			result.fontStyle = this.m_FontData.fontStyle;
			result.resizeTextForBestFit = this.m_FontData.bestFit;
			result.updateBounds = false;
			result.horizontalOverflow = this.m_FontData.horizontalOverflow;
			result.verticalOverflow = this.m_FontData.verticalOverflow;
			return result;
		}

		public static Vector2 GetTextAnchorPivot(TextAnchor anchor)
		{
			Vector2 result;
			switch (anchor)
			{
			case TextAnchor.UpperLeft:
				result = new Vector2(0f, 1f);
				break;
			case TextAnchor.UpperCenter:
				result = new Vector2(0.5f, 1f);
				break;
			case TextAnchor.UpperRight:
				result = new Vector2(1f, 1f);
				break;
			case TextAnchor.MiddleLeft:
				result = new Vector2(0f, 0.5f);
				break;
			case TextAnchor.MiddleCenter:
				result = new Vector2(0.5f, 0.5f);
				break;
			case TextAnchor.MiddleRight:
				result = new Vector2(1f, 0.5f);
				break;
			case TextAnchor.LowerLeft:
				result = new Vector2(0f, 0f);
				break;
			case TextAnchor.LowerCenter:
				result = new Vector2(0.5f, 0f);
				break;
			case TextAnchor.LowerRight:
				result = new Vector2(1f, 0f);
				break;
			default:
				result = Vector2.zero;
				break;
			}
			return result;
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (!(this.font == null))
			{
				this.m_DisableFontTextureRebuiltCallback = true;
				Vector2 size = base.rectTransform.rect.size;
				TextGenerationSettings generationSettings = this.GetGenerationSettings(size);
				this.cachedTextGenerator.PopulateWithErrors(this.text, generationSettings, base.gameObject);
				IList<UIVertex> verts = this.cachedTextGenerator.verts;
				float d = 1f / this.pixelsPerUnit;
				int num = verts.Count - 4;
				Vector2 vector = new Vector2(verts[0].position.x, verts[0].position.y) * d;
				vector = base.PixelAdjustPoint(vector) - vector;
				toFill.Clear();
				if (vector != Vector2.zero)
				{
					for (int i = 0; i < num; i++)
					{
						int num2 = i & 3;
						this.m_TempVerts[num2] = verts[i];
						UIVertex[] expr_10E_cp_0 = this.m_TempVerts;
						int expr_10E_cp_1 = num2;
						expr_10E_cp_0[expr_10E_cp_1].position = expr_10E_cp_0[expr_10E_cp_1].position * d;
						UIVertex[] expr_132_cp_0_cp_0 = this.m_TempVerts;
						int expr_132_cp_0_cp_1 = num2;
						expr_132_cp_0_cp_0[expr_132_cp_0_cp_1].position.x = expr_132_cp_0_cp_0[expr_132_cp_0_cp_1].position.x + vector.x;
						UIVertex[] expr_157_cp_0_cp_0 = this.m_TempVerts;
						int expr_157_cp_0_cp_1 = num2;
						expr_157_cp_0_cp_0[expr_157_cp_0_cp_1].position.y = expr_157_cp_0_cp_0[expr_157_cp_0_cp_1].position.y + vector.y;
						if (num2 == 3)
						{
							toFill.AddUIVertexQuad(this.m_TempVerts);
						}
					}
				}
				else
				{
					for (int j = 0; j < num; j++)
					{
						int num3 = j & 3;
						this.m_TempVerts[num3] = verts[j];
						UIVertex[] expr_1CB_cp_0 = this.m_TempVerts;
						int expr_1CB_cp_1 = num3;
						expr_1CB_cp_0[expr_1CB_cp_1].position = expr_1CB_cp_0[expr_1CB_cp_1].position * d;
						if (num3 == 3)
						{
							toFill.AddUIVertexQuad(this.m_TempVerts);
						}
					}
				}
				this.m_DisableFontTextureRebuiltCallback = false;
			}
		}

		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		public virtual void CalculateLayoutInputVertical()
		{
		}

		public override void OnRebuildRequested()
		{
			FontUpdateTracker.UntrackText(this);
			FontUpdateTracker.TrackText(this);
			this.cachedTextGenerator.Invalidate();
			base.OnRebuildRequested();
		}

		protected override void OnValidate()
		{
			if (!this.IsActive())
			{
				base.OnValidate();
			}
			else
			{
				if (this.m_FontData.font != this.m_LastTrackedFont)
				{
					Font font = this.m_FontData.font;
					this.m_FontData.font = this.m_LastTrackedFont;
					FontUpdateTracker.UntrackText(this);
					this.m_FontData.font = font;
					FontUpdateTracker.TrackText(this);
					this.m_LastTrackedFont = font;
				}
				base.OnValidate();
			}
		}
	}
}
