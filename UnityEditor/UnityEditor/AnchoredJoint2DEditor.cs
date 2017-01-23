using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AnchoredJoint2D), true)]
	internal class AnchoredJoint2DEditor : Joint2DEditor
	{
		private const float k_SnapDistance = 0.13f;

		private AnchoredJoint2D anchorJoint2D;

		public void OnSceneGUI()
		{
			this.anchorJoint2D = (AnchoredJoint2D)base.target;
			if (this.anchorJoint2D.enabled)
			{
				Vector3 vector = Joint2DEditor.TransformPoint(this.anchorJoint2D.transform, this.anchorJoint2D.anchor);
				Vector3 vector2 = this.anchorJoint2D.connectedAnchor;
				if (this.anchorJoint2D.connectedBody)
				{
					vector2 = Joint2DEditor.TransformPoint(this.anchorJoint2D.connectedBody.transform, vector2);
				}
				Vector3 vector3 = vector + (vector2 - vector).normalized * HandleUtility.GetHandleSize(vector) * 0.1f;
				Handles.color = Color.green;
				Handles.DrawAAPolyLine(new Vector3[]
				{
					vector3,
					vector2
				});
				if (base.HandleAnchor(ref vector2, true))
				{
					vector2 = this.SnapToSprites(vector2);
					vector2 = Joint2DEditor.SnapToPoint(vector2, vector, 0.13f);
					if (this.anchorJoint2D.connectedBody)
					{
						vector2 = Joint2DEditor.InverseTransformPoint(this.anchorJoint2D.connectedBody.transform, vector2);
					}
					Undo.RecordObject(this.anchorJoint2D, "Move Connected Anchor");
					this.anchorJoint2D.connectedAnchor = vector2;
				}
				if (base.HandleAnchor(ref vector, false))
				{
					vector = this.SnapToSprites(vector);
					vector = Joint2DEditor.SnapToPoint(vector, vector2, 0.13f);
					Undo.RecordObject(this.anchorJoint2D, "Move Anchor");
					this.anchorJoint2D.anchor = Joint2DEditor.InverseTransformPoint(this.anchorJoint2D.transform, vector);
				}
			}
		}

		private Vector3 SnapToSprites(Vector3 position)
		{
			SpriteRenderer component = this.anchorJoint2D.GetComponent<SpriteRenderer>();
			position = Joint2DEditor.SnapToSprite(component, position, 0.13f);
			if (this.anchorJoint2D.connectedBody)
			{
				component = this.anchorJoint2D.connectedBody.GetComponent<SpriteRenderer>();
				position = Joint2DEditor.SnapToSprite(component, position, 0.13f);
			}
			return position;
		}
	}
}
