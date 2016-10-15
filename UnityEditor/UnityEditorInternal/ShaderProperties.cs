using System;

namespace UnityEditorInternal
{
	internal struct ShaderProperties
	{
		public ShaderFloatInfo[] floats;

		public ShaderVectorInfo[] vectors;

		public ShaderMatrixInfo[] matrices;

		public ShaderTextureInfo[] textures;
	}
}
