using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomPropertyDrawer(typeof(SpaceAttribute))]
	internal sealed class SpaceDrawer : DecoratorDrawer
	{
		public override float GetHeight()
		{
			return (base.attribute as SpaceAttribute).height;
		}
	}
}
