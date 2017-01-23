using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Raw Image", 12)]
	public class RawImage : MaskableGraphic
	{
		[FormerlySerializedAs("m_Tex"), SerializeField]
		private Texture m_Texture;

		[SerializeField]
		private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

		public override Texture mainTexture
		{
			get
			{
				Texture result;
				if (this.m_Texture == null)
				{
					if (this.material != null && this.material.mainTexture != null)
					{
						result = this.material.mainTexture;
					}
					else
					{
						result = Graphic.s_WhiteTexture;
					}
				}
				else
				{
					result = this.m_Texture;
				}
				return result;
			}
		}

		public Texture texture
		{
			get
			{
				return this.m_Texture;
			}
			set
			{
				if (!(this.m_Texture == value))
				{
					this.m_Texture = value;
					this.SetVerticesDirty();
					this.SetMaterialDirty();
				}
			}
		}

		public Rect uvRect
		{
			get
			{
				return this.m_UVRect;
			}
			set
			{
				if (!(this.m_UVRect == value))
				{
					this.m_UVRect = value;
					this.SetVerticesDirty();
				}
			}
		}

		protected RawImage()
		{
			base.useLegacyMeshGeneration = false;
		}

		public override void SetNativeSize()
		{
			Texture mainTexture = this.mainTexture;
			if (mainTexture != null)
			{
				int num = Mathf.RoundToInt((float)mainTexture.width * this.uvRect.width);
				int num2 = Mathf.RoundToInt((float)mainTexture.height * this.uvRect.height);
				base.rectTransform.anchorMax = base.rectTransform.anchorMin;
				base.rectTransform.sizeDelta = new Vector2((float)num, (float)num2);
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Texture mainTexture = this.mainTexture;
			vh.Clear();
			if (mainTexture != null)
			{
				Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
				Vector4 vector = new Vector4(pixelAdjustedRect.x, pixelAdjustedRect.y, pixelAdjustedRect.x + pixelAdjustedRect.width, pixelAdjustedRect.y + pixelAdjustedRect.height);
				float num = (float)mainTexture.width * mainTexture.texelSize.x;
				float num2 = (float)mainTexture.height * mainTexture.texelSize.y;
				Color color = this.color;
				vh.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(this.m_UVRect.xMin * num, this.m_UVRect.yMin * num2));
				vh.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(this.m_UVRect.xMin * num, this.m_UVRect.yMax * num2));
				vh.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(this.m_UVRect.xMax * num, this.m_UVRect.yMax * num2));
				vh.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(this.m_UVRect.xMax * num, this.m_UVRect.yMin * num2));
				vh.AddTriangle(0, 1, 2);
				vh.AddTriangle(2, 3, 0);
			}
		}
	}
}
