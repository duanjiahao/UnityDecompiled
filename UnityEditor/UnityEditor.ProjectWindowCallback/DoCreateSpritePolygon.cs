using System;
using UnityEditor.Sprites;

namespace UnityEditor.ProjectWindowCallback
{
	internal class DoCreateSpritePolygon : EndNameEditAction
	{
		public int sides;

		public override void Action(int instanceId, string pathName, string resourceFile)
		{
			bool flag = false;
			if (this.sides < 0)
			{
				this.sides = 5;
				flag = true;
			}
			UnityEditor.Sprites.SpriteUtility.CreateSpritePolygonAssetAtPath(pathName, this.sides);
			if (flag)
			{
				Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(pathName);
				SpriteEditorWindow.GetWindow();
			}
		}
	}
}
