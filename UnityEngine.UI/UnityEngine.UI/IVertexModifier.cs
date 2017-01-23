using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	[Obsolete("Use IMeshModifier instead", true)]
	public interface IVertexModifier
	{
		[Obsolete("use IMeshModifier.ModifyMesh (VertexHelper verts)  instead", true)]
		void ModifyVertices(List<UIVertex> verts);
	}
}
