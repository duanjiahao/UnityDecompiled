using System;

namespace UnityEditor.Sprites
{
	public interface IPackerPolicy
	{
		void OnGroupAtlases(BuildTarget target, PackerJob job, int[] textureImporterInstanceIDs);

		int GetVersion();
	}
}
