using System;

namespace UnityEditor.U2D.Interface
{
	internal interface ITexturePlatformSettingsFormatHelper
	{
		void AcquireTextureFormatValuesAndStrings(BuildTarget buildTarget, out int[] displayValues, out string[] displayStrings);

		bool TextureFormatRequireCompressionQualityInput(TextureImporterFormat format);
	}
}
