using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMBox : IMElement
	{
		protected override int DoGenerateControlID()
		{
			return GUIUtility.GetControlID("IMBox".GetHashCode(), FocusType.Passive);
		}

		internal override void DoRepaint(IStylePainter args)
		{
			base.style.Draw(base.position, GUIContent.Temp(base.text), base.id);
		}
	}
}
