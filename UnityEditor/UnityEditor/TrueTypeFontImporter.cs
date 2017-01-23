using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class TrueTypeFontImporter : AssetImporter
	{
		public extern int fontSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern FontTextureCase fontTextureCase
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("FontRenderModes are no longer supported.", true)]
		private int fontRenderMode
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public extern bool includeFontData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("use2xBehaviour is deprecated. Use ascentCalculationMode instead")]
		private bool use2xBehaviour
		{
			get
			{
				return this.ascentCalculationMode == AscentCalculationMode.Legacy2x;
			}
			set
			{
				if (value)
				{
					this.ascentCalculationMode = AscentCalculationMode.Legacy2x;
				}
				else if (this.ascentCalculationMode == AscentCalculationMode.Legacy2x)
				{
					this.ascentCalculationMode = AscentCalculationMode.FaceAscender;
				}
			}
		}

		public extern AscentCalculationMode ascentCalculationMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string[] fontNames
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Font[] fontReferences
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string customCharacters
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string fontTTFName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Per-Font styles are no longer supported. Set the style in the rendering component, or import a styled version of the font.", true)]
		private FontStyle style
		{
			get
			{
				return FontStyle.Normal;
			}
			set
			{
			}
		}

		public extern int characterSpacing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int characterPadding
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern FontRenderingMode fontRenderingMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Font[] LookupFallbackFontReferences(string[] _names);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsFormatSupported();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Font GenerateEditableFont(string path);
	}
}
