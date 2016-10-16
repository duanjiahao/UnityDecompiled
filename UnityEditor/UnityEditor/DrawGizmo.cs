using System;

namespace UnityEditor
{
	public sealed class DrawGizmo : Attribute
	{
		public Type drawnType;

		public GizmoType drawOptions;

		public DrawGizmo(GizmoType gizmo)
		{
			this.drawOptions = gizmo;
		}

		public DrawGizmo(GizmoType gizmo, Type drawnGizmoType)
		{
			this.drawnType = drawnGizmoType;
			this.drawOptions = gizmo;
		}
	}
}
