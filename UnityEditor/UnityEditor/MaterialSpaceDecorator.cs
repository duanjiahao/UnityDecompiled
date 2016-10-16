using System;
using UnityEngine;

namespace UnityEditor
{
	internal class MaterialSpaceDecorator : MaterialPropertyDrawer
	{
		private readonly float height;

		public MaterialSpaceDecorator()
		{
			this.height = 6f;
		}

		public MaterialSpaceDecorator(float height)
		{
			this.height = height;
		}

		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			return this.height;
		}

		public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
		{
		}
	}
}
