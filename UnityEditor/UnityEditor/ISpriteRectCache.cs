using System;
using UnityEditor.U2D.Interface;

namespace UnityEditor
{
	internal interface ISpriteRectCache : IUndoableObject
	{
		int Count
		{
			get;
		}

		SpriteRect RectAt(int i);

		void AddRect(SpriteRect r);

		void RemoveRect(SpriteRect r);

		void ClearAll();

		int GetIndex(SpriteRect spriteRect);

		bool Contains(SpriteRect spriteRect);
	}
}
