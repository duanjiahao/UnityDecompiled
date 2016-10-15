using System;

namespace UnityEditor
{
	internal class MaterialToggleOffDrawer : MaterialToggleDrawer
	{
		public MaterialToggleOffDrawer()
		{
		}

		public MaterialToggleOffDrawer(string keyword) : base(keyword)
		{
		}

		protected override void SetKeyword(MaterialProperty prop, bool on)
		{
			base.SetKeywordInternal(prop, !on, "_OFF");
		}
	}
}
