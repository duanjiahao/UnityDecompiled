using System;

namespace UnityEngine.Experimental.UIElements
{
	public class Image : VisualElement
	{
		public Texture image
		{
			get;
			set;
		}

		public ScaleMode scaleMode
		{
			get;
			set;
		}

		public Image()
		{
			this.scaleMode = ScaleMode.ScaleAndCrop;
		}

		internal override void DoRepaint(IStylePainter painter)
		{
			if (this.image == null)
			{
				Debug.LogWarning("null texture passed to GUI.DrawTexture");
			}
			else
			{
				painter.DrawTexture(base.contentRect, this.image, GUI.color, this.scaleMode, 0f, 0f, 0, 0, 0, 0);
			}
		}
	}
}
