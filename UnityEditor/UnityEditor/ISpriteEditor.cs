using System;
using UnityEngine;
using UnityEngine.U2D.Interface;

namespace UnityEditor
{
	internal interface ISpriteEditor
	{
		ISpriteRectCache spriteRects
		{
			get;
		}

		SpriteRect selectedSpriteRect
		{
			get;
			set;
		}

		bool enableMouseMoveEvent
		{
			set;
		}

		bool editingDisabled
		{
			get;
		}

		Rect windowDimension
		{
			get;
		}

		ITexture2D selectedTexture
		{
			get;
		}

		ITexture2D previewTexture
		{
			get;
		}

		void HandleSpriteSelection();

		void RequestRepaint();

		void SetDataModified();

		void DisplayProgressBar(string title, string content, float progress);

		void ClearProgressBar();

		ITexture2D GetReadableTexture2D();
	}
}
