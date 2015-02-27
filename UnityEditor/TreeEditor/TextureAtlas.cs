using System;
using System.Collections.Generic;
using UnityEngine;
namespace TreeEditor
{
	public class TextureAtlas
	{
		public class TextureNode
		{
			public string name;
			public Texture2D diffuseTexture;
			public Color diffuseColor;
			public Texture2D normalTexture;
			public Texture2D glossTexture;
			public Texture2D translucencyTexture;
			public Texture2D shadowOffsetTexture;
			public float shininess;
			public Vector2 scale = new Vector2(1f, 1f);
			public bool tileV;
			public Vector2 uvTiling;
			public Rect sourceRect = new Rect(0f, 0f, 0f, 0f);
			public Rect packedRect = new Rect(0f, 0f, 0f, 0f);
			public Rect uvRect = new Rect(0f, 0f, 0f, 0f);
			public static bool Overlap(TextureAtlas.TextureNode a, TextureAtlas.TextureNode b)
			{
				if (a.tileV || b.tileV)
				{
					return a.packedRect.x <= b.packedRect.x + b.packedRect.width && a.packedRect.x + a.packedRect.width >= b.packedRect.x;
				}
				return a.packedRect.x <= b.packedRect.x + b.packedRect.width && a.packedRect.x + a.packedRect.width >= b.packedRect.x && a.packedRect.y <= b.packedRect.y + b.packedRect.height && a.packedRect.y + a.packedRect.height >= b.packedRect.y;
			}
			public int CompareTo(TextureAtlas.TextureNode b)
			{
				if (this.tileV && b.tileV)
				{
					return -this.packedRect.width.CompareTo(b.packedRect.width);
				}
				if (this.tileV)
				{
					return -1;
				}
				if (b.tileV)
				{
					return 1;
				}
				return -this.packedRect.height.CompareTo(b.packedRect.height);
			}
		}
		public int atlasWidth;
		public int atlasHeight;
		public int atlasPadding;
		public List<TextureAtlas.TextureNode> nodes = new List<TextureAtlas.TextureNode>();
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.nodes.Count; i++)
			{
				num ^= this.nodes[i].uvRect.GetHashCode();
			}
			return num;
		}
		public void AddTexture(string name, Texture2D diffuse, Color diffuseColor, Texture2D normal, Texture2D gloss, Texture2D transtex, Texture2D shadowOffsetTex, float shininess, Vector2 scale, bool tileV, Vector2 uvTiling)
		{
			TextureAtlas.TextureNode textureNode = new TextureAtlas.TextureNode();
			textureNode.name = name;
			textureNode.diffuseTexture = diffuse;
			textureNode.diffuseColor = diffuseColor;
			textureNode.normalTexture = normal;
			textureNode.glossTexture = gloss;
			textureNode.translucencyTexture = transtex;
			textureNode.shadowOffsetTexture = shadowOffsetTex;
			textureNode.shininess = shininess;
			textureNode.scale = scale;
			textureNode.tileV = tileV;
			textureNode.uvTiling = uvTiling;
			if (diffuse)
			{
				textureNode.sourceRect.width = (float)diffuse.width;
				textureNode.sourceRect.height = (float)diffuse.height;
			}
			else
			{
				textureNode.sourceRect.width = 64f;
				textureNode.sourceRect.height = 64f;
				textureNode.scale = new Vector2(1f, 1f);
			}
			this.nodes.Add(textureNode);
		}
		public Vector2 GetTexTiling(string name)
		{
			for (int i = 0; i < this.nodes.Count; i++)
			{
				if (this.nodes[i].name == name)
				{
					return this.nodes[i].uvTiling;
				}
			}
			return new Vector2(1f, 1f);
		}
		public Rect GetUVRect(string name)
		{
			for (int i = 0; i < this.nodes.Count; i++)
			{
				if (this.nodes[i].name == name)
				{
					return this.nodes[i].uvRect;
				}
			}
			return new Rect(0f, 0f, 0f, 0f);
		}
		public void Pack(ref int targetWidth, int targetHeight, int padding, bool correctPow2)
		{
			if (padding % 2 != 0)
			{
				Debug.LogWarning("Padding not an even number");
				padding++;
			}
			for (int i = 0; i < this.nodes.Count; i++)
			{
				TextureAtlas.TextureNode textureNode = this.nodes[i];
				textureNode.packedRect.x = 0f;
				textureNode.packedRect.y = 0f;
				textureNode.packedRect.width = Mathf.Round(textureNode.sourceRect.width * textureNode.scale.x);
				textureNode.packedRect.height = Mathf.Min((float)targetHeight, Mathf.Round(textureNode.sourceRect.height * textureNode.scale.y));
				if (textureNode.tileV)
				{
					textureNode.packedRect.height = (float)targetHeight;
				}
				if (correctPow2)
				{
					textureNode.packedRect.width = (float)Mathf.ClosestPowerOfTwo((int)textureNode.packedRect.width);
					textureNode.packedRect.height = (float)Mathf.ClosestPowerOfTwo((int)textureNode.packedRect.height);
				}
			}
			this.nodes.Sort((TextureAtlas.TextureNode a, TextureAtlas.TextureNode b) => a.CompareTo(b));
			int num = 0;
			int num2 = 0;
			for (int j = 0; j < this.nodes.Count; j++)
			{
				TextureAtlas.TextureNode textureNode2 = this.nodes[j];
				bool flag = false;
				for (int k = 0; k < num; k++)
				{
					textureNode2.packedRect.x = (float)k;
					textureNode2.packedRect.y = 0f;
					flag = true;
					for (int l = 0; l <= num2; l++)
					{
						flag = true;
						textureNode2.packedRect.y = (float)l;
						for (int m = 0; m < j; m++)
						{
							TextureAtlas.TextureNode textureNode3 = this.nodes[m];
							if (TextureAtlas.TextureNode.Overlap(textureNode2, textureNode3))
							{
								flag = false;
								if (textureNode3.tileV)
								{
									l = num2;
								}
								else
								{
									l = (int)(textureNode3.packedRect.y + textureNode3.packedRect.height);
								}
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					textureNode2.packedRect.x = (float)num;
					textureNode2.packedRect.y = 0f;
				}
				num = Mathf.Max(num, (int)(textureNode2.packedRect.x + textureNode2.packedRect.width));
				num2 = Mathf.Max(num2, (int)(textureNode2.packedRect.y + textureNode2.packedRect.height));
			}
			int min = Mathf.Max(Mathf.ClosestPowerOfTwo(padding * 2), 64);
			int num3 = Mathf.Clamp(Mathf.ClosestPowerOfTwo(num), min, targetWidth);
			targetWidth = num3;
			this.atlasWidth = targetWidth;
			this.atlasHeight = targetHeight;
			this.atlasPadding = padding;
			float num4 = (float)targetWidth / (float)num;
			float num5 = (float)targetHeight / (float)num2;
			for (int n = 0; n < this.nodes.Count; n++)
			{
				TextureAtlas.TextureNode textureNode4 = this.nodes[n];
				TextureAtlas.TextureNode expr_32A_cp_0 = textureNode4;
				expr_32A_cp_0.packedRect.x = expr_32A_cp_0.packedRect.x * num4;
				TextureAtlas.TextureNode expr_33F_cp_0 = textureNode4;
				expr_33F_cp_0.packedRect.y = expr_33F_cp_0.packedRect.y * num5;
				TextureAtlas.TextureNode expr_354_cp_0 = textureNode4;
				expr_354_cp_0.packedRect.width = expr_354_cp_0.packedRect.width * num4;
				TextureAtlas.TextureNode expr_369_cp_0 = textureNode4;
				expr_369_cp_0.packedRect.height = expr_369_cp_0.packedRect.height * num5;
				if (textureNode4.tileV)
				{
					textureNode4.packedRect.y = 0f;
					textureNode4.packedRect.height = (float)targetHeight;
					TextureAtlas.TextureNode expr_3A9_cp_0 = textureNode4;
					expr_3A9_cp_0.packedRect.x = expr_3A9_cp_0.packedRect.x + (float)(padding / 2);
					TextureAtlas.TextureNode expr_3C0_cp_0 = textureNode4;
					expr_3C0_cp_0.packedRect.width = expr_3C0_cp_0.packedRect.width - (float)padding;
				}
				else
				{
					TextureAtlas.TextureNode expr_3DA_cp_0 = textureNode4;
					expr_3DA_cp_0.packedRect.x = expr_3DA_cp_0.packedRect.x + (float)(padding / 2);
					TextureAtlas.TextureNode expr_3F1_cp_0 = textureNode4;
					expr_3F1_cp_0.packedRect.y = expr_3F1_cp_0.packedRect.y + (float)(padding / 2);
					TextureAtlas.TextureNode expr_408_cp_0 = textureNode4;
					expr_408_cp_0.packedRect.width = expr_408_cp_0.packedRect.width - (float)padding;
					TextureAtlas.TextureNode expr_41D_cp_0 = textureNode4;
					expr_41D_cp_0.packedRect.height = expr_41D_cp_0.packedRect.height - (float)padding;
				}
				if (textureNode4.packedRect.width < 1f)
				{
					textureNode4.packedRect.width = 1f;
				}
				if (textureNode4.packedRect.height < 1f)
				{
					textureNode4.packedRect.height = 1f;
				}
				textureNode4.packedRect.x = Mathf.Round(textureNode4.packedRect.x);
				textureNode4.packedRect.y = Mathf.Round(textureNode4.packedRect.y);
				textureNode4.packedRect.width = Mathf.Round(textureNode4.packedRect.width);
				textureNode4.packedRect.height = Mathf.Round(textureNode4.packedRect.height);
				textureNode4.uvRect.x = textureNode4.packedRect.x / (float)targetWidth;
				textureNode4.uvRect.y = textureNode4.packedRect.y / (float)targetHeight;
				textureNode4.uvRect.width = textureNode4.packedRect.width / (float)targetWidth;
				textureNode4.uvRect.height = textureNode4.packedRect.height / (float)targetHeight;
			}
		}
	}
}
