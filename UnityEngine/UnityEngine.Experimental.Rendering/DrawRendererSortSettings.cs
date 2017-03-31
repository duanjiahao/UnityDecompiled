using System;

namespace UnityEngine.Experimental.Rendering
{
	public struct DrawRendererSortSettings
	{
		public Matrix4x4 worldToCameraMatrix;

		public Vector3 cameraPosition;

		public SortFlags flags;

		private int _sortOrthographic;

		public bool sortOrthographic
		{
			get
			{
				return this._sortOrthographic != 0;
			}
			set
			{
				this._sortOrthographic = ((!value) ? 0 : 1);
			}
		}
	}
}
