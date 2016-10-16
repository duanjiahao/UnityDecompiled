using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class MaterialProperty
	{
		public enum PropType
		{
			Color,
			Vector,
			Float,
			Range,
			Texture
		}

		[Obsolete("Use UnityEngine.Rendering.TextureDimension instead", false)]
		public enum TexDim
		{
			Unknown = -1,
			None,
			Tex2D = 2,
			Tex3D,
			Cube,
			Any = 6
		}

		[Flags]
		public enum PropFlags
		{
			None = 0,
			HideInInspector = 1,
			PerRendererData = 2,
			NoScaleOffset = 4,
			Normal = 8,
			HDR = 16
		}

		public delegate bool ApplyPropertyCallback(MaterialProperty prop, int changeMask, object previousValue);

		private UnityEngine.Object[] m_Targets;

		private MaterialProperty.ApplyPropertyCallback m_ApplyPropertyCallback;

		private string m_Name;

		private string m_DisplayName;

		private object m_Value;

		private Vector4 m_TextureScaleAndOffset;

		private Vector2 m_RangeLimits;

		private MaterialProperty.PropType m_Type;

		private MaterialProperty.PropFlags m_Flags;

		private TextureDimension m_TextureDimension;

		private int m_MixedValueMask;

		public UnityEngine.Object[] targets
		{
			get
			{
				return this.m_Targets;
			}
		}

		public MaterialProperty.PropType type
		{
			get
			{
				return this.m_Type;
			}
		}

		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public string displayName
		{
			get
			{
				return this.m_DisplayName;
			}
		}

		public MaterialProperty.PropFlags flags
		{
			get
			{
				return this.m_Flags;
			}
		}

		public TextureDimension textureDimension
		{
			get
			{
				return this.m_TextureDimension;
			}
		}

		public Vector2 rangeLimits
		{
			get
			{
				return this.m_RangeLimits;
			}
		}

		public bool hasMixedValue
		{
			get
			{
				return (this.m_MixedValueMask & 1) != 0;
			}
		}

		public MaterialProperty.ApplyPropertyCallback applyPropertyCallback
		{
			get
			{
				return this.m_ApplyPropertyCallback;
			}
			set
			{
				this.m_ApplyPropertyCallback = value;
			}
		}

		internal int mixedValueMask
		{
			get
			{
				return this.m_MixedValueMask;
			}
		}

		public Color colorValue
		{
			get
			{
				if (this.m_Type == MaterialProperty.PropType.Color)
				{
					return (Color)this.m_Value;
				}
				return Color.black;
			}
			set
			{
				if (this.m_Type != MaterialProperty.PropType.Color)
				{
					return;
				}
				if (!this.hasMixedValue && value == (Color)this.m_Value)
				{
					return;
				}
				this.ApplyProperty(value);
			}
		}

		public Vector4 vectorValue
		{
			get
			{
				if (this.m_Type == MaterialProperty.PropType.Vector)
				{
					return (Vector4)this.m_Value;
				}
				return Vector4.zero;
			}
			set
			{
				if (this.m_Type != MaterialProperty.PropType.Vector)
				{
					return;
				}
				if (!this.hasMixedValue && value == (Vector4)this.m_Value)
				{
					return;
				}
				this.ApplyProperty(value);
			}
		}

		public float floatValue
		{
			get
			{
				if (this.m_Type == MaterialProperty.PropType.Float || this.m_Type == MaterialProperty.PropType.Range)
				{
					return (float)this.m_Value;
				}
				return 0f;
			}
			set
			{
				if (this.m_Type != MaterialProperty.PropType.Float && this.m_Type != MaterialProperty.PropType.Range)
				{
					return;
				}
				if (!this.hasMixedValue && value == (float)this.m_Value)
				{
					return;
				}
				this.ApplyProperty(value);
			}
		}

		public Texture textureValue
		{
			get
			{
				if (this.m_Type == MaterialProperty.PropType.Texture)
				{
					return (Texture)this.m_Value;
				}
				return null;
			}
			set
			{
				if (this.m_Type != MaterialProperty.PropType.Texture)
				{
					return;
				}
				if (!this.hasMixedValue && value == (Texture)this.m_Value)
				{
					return;
				}
				this.m_MixedValueMask &= -2;
				object value2 = this.m_Value;
				this.m_Value = value;
				this.ApplyProperty(value2, 1);
			}
		}

		public Vector4 textureScaleAndOffset
		{
			get
			{
				if (this.m_Type == MaterialProperty.PropType.Texture)
				{
					return this.m_TextureScaleAndOffset;
				}
				return Vector4.zero;
			}
			set
			{
				if (this.m_Type != MaterialProperty.PropType.Texture)
				{
					return;
				}
				if (!this.hasMixedValue && value == this.m_TextureScaleAndOffset)
				{
					return;
				}
				this.m_MixedValueMask &= 1;
				int num = 0;
				for (int i = 1; i < 5; i++)
				{
					num |= 1 << i;
				}
				object previousValue = this.m_TextureScaleAndOffset;
				this.m_TextureScaleAndOffset = value;
				this.ApplyProperty(previousValue, num);
			}
		}

		public void ReadFromMaterialPropertyBlock(MaterialPropertyBlock block)
		{
			ShaderUtil.ApplyMaterialPropertyBlockToMaterialProperty(block, this);
		}

		public void WriteToMaterialPropertyBlock(MaterialPropertyBlock materialblock, int changedPropertyMask)
		{
			ShaderUtil.ApplyMaterialPropertyToMaterialPropertyBlock(this, changedPropertyMask, materialblock);
		}

		internal static bool IsTextureOffsetAndScaleChangedMask(int changedMask)
		{
			changedMask >>= 1;
			return changedMask != 0;
		}

		private void ApplyProperty(object newValue)
		{
			this.m_MixedValueMask = 0;
			object value = this.m_Value;
			this.m_Value = newValue;
			this.ApplyProperty(value, 1);
		}

		private void ApplyProperty(object previousValue, int changedPropertyMask)
		{
			if (this.targets == null || this.targets.Length == 0)
			{
				throw new ArgumentException("No material targets provided");
			}
			UnityEngine.Object[] targets = this.targets;
			string str;
			if (targets.Length == 1)
			{
				str = targets[0].name;
			}
			else
			{
				str = string.Concat(new object[]
				{
					targets.Length,
					" ",
					ObjectNames.NicifyVariableName(ObjectNames.GetClassName(targets[0])),
					"s"
				});
			}
			bool flag = false;
			if (this.m_ApplyPropertyCallback != null)
			{
				flag = this.m_ApplyPropertyCallback(this, changedPropertyMask, previousValue);
			}
			if (!flag)
			{
				ShaderUtil.ApplyProperty(this, changedPropertyMask, "Modify " + this.displayName + " of " + str);
			}
		}
	}
}
