using System;
using UnityEngine;

namespace UnityEditorInternal
{
	[Serializable]
	internal class SpriteEditorMenuSetting : ScriptableObject
	{
		public enum SlicingType
		{
			Automatic,
			GridByCellSize,
			GridByCellCount
		}

		[SerializeField]
		public Vector2 gridCellCount = new Vector2(1f, 1f);

		[SerializeField]
		public Vector2 gridSpriteSize = new Vector2(64f, 64f);

		[SerializeField]
		public Vector2 gridSpriteOffset = new Vector2(0f, 0f);

		[SerializeField]
		public Vector2 gridSpritePadding = new Vector2(0f, 0f);

		[SerializeField]
		public Vector2 pivot = Vector2.zero;

		[SerializeField]
		public int autoSlicingMethod;

		[SerializeField]
		public int spriteAlignment;

		[SerializeField]
		public SpriteEditorMenuSetting.SlicingType slicingType;
	}
}
