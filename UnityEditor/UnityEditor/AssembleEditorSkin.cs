using System;

namespace UnityEditor
{
	internal class AssembleEditorSkin : EditorWindow
	{
		public static void DoIt()
		{
			GenerateIconsWithMipLevels.DeleteGeneratedFolder();
			EditorApplication.ExecuteMenuItem("Tools/Regenerate All Gizmos Icons with Mips");
			EditorApplication.ExecuteMenuItem("Tools/Regenerate Editor Skins Now");
			AssembleEditorSkin.RegenerateAllIconsWithMipLevels();
		}

		private static void RegenerateAllIconsWithMipLevels()
		{
			GenerateIconsWithMipLevels.GenerateAllIconsWithMipLevels();
		}

		private static void RegenerateSelectedIconsWithMipLevels()
		{
			GenerateIconsWithMipLevels.GenerateSelectedIconsWithMips();
		}
	}
}
