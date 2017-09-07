using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMImage : IMElement
	{
		public Texture image
		{
			get;
			set;
		}

		public ScaleMode scaleMode
		{
			get;
			set;
		}

		public float imageAspect
		{
			get;
			set;
		}

		public bool alphaBlend
		{
			get;
			set;
		}

		public IMImage()
		{
			this.scaleMode = ScaleMode.ScaleAndCrop;
		}

		protected override int DoGenerateControlID()
		{
			return 0;
		}

		internal override void DoRepaint(IStylePainter args)
		{
			if (this.image == null)
			{
				Debug.LogWarning("null texture passed to GUI.DrawTexture");
			}
			else
			{
				if (this.imageAspect == 0f)
				{
					this.imageAspect = (float)this.image.width / (float)this.image.height;
				}
				Material mat = (!this.alphaBlend) ? GUI.blitMaterial : GUI.blendMaterial;
				float num = base.position.width / base.position.height;
				Internal_DrawTextureArguments internal_DrawTextureArguments = new Internal_DrawTextureArguments
				{
					leftBorder = 0,
					rightBorder = 0,
					topBorder = 0,
					bottomBorder = 0,
					color = GUI.color,
					texture = this.image,
					mat = mat
				};
				ScaleMode scaleMode = this.scaleMode;
				if (scaleMode != ScaleMode.StretchToFill)
				{
					if (scaleMode != ScaleMode.ScaleAndCrop)
					{
						if (scaleMode == ScaleMode.ScaleToFit)
						{
							if (num > this.imageAspect)
							{
								float num2 = this.imageAspect / num;
								internal_DrawTextureArguments.screenRect = new Rect(base.position.xMin + base.position.width * (1f - num2) * 0.5f, base.position.yMin, num2 * base.position.width, base.position.height);
								internal_DrawTextureArguments.sourceRect = new Rect(0f, 0f, 1f, 1f);
								Graphics.Internal_DrawTexture(ref internal_DrawTextureArguments);
							}
							else
							{
								float num3 = num / this.imageAspect;
								internal_DrawTextureArguments.screenRect = new Rect(base.position.xMin, base.position.yMin + base.position.height * (1f - num3) * 0.5f, base.position.width, num3 * base.position.height);
								internal_DrawTextureArguments.sourceRect = new Rect(0f, 0f, 1f, 1f);
								Graphics.Internal_DrawTexture(ref internal_DrawTextureArguments);
							}
						}
					}
					else if (num > this.imageAspect)
					{
						float num4 = this.imageAspect / num;
						internal_DrawTextureArguments.screenRect = base.position;
						internal_DrawTextureArguments.sourceRect = new Rect(0f, (1f - num4) * 0.5f, 1f, num4);
						Graphics.Internal_DrawTexture(ref internal_DrawTextureArguments);
					}
					else
					{
						float num5 = num / this.imageAspect;
						internal_DrawTextureArguments.screenRect = base.position;
						internal_DrawTextureArguments.sourceRect = new Rect(0.5f - num5 * 0.5f, 0f, num5, 1f);
						Graphics.Internal_DrawTexture(ref internal_DrawTextureArguments);
					}
				}
				else
				{
					internal_DrawTextureArguments.screenRect = base.position;
					internal_DrawTextureArguments.sourceRect = new Rect(0f, 0f, 1f, 1f);
					Graphics.Internal_DrawTexture(ref internal_DrawTextureArguments);
				}
			}
		}
	}
}
