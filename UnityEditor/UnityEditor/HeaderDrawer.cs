using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomPropertyDrawer(typeof(HeaderAttribute))]
	internal sealed class HeaderDrawer : DecoratorDrawer
	{
		public override void OnGUI(Rect position)
		{
			position.y += 8f;
			position = EditorGUI.IndentedRect(position);
			GUI.Label(position, (base.attribute as HeaderAttribute).header, EditorStyles.boldLabel);
		}

		public override float GetHeight()
		{
			return 24f;
		}
	}
}
