using System;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(Physics2DSettings))]
	internal class Physics2DSettingsInspector : Editor
	{
		private Vector2 scrollPos;
		private bool show = true;
		private bool GetValue(int layerA, int layerB)
		{
			return !Physics2D.GetIgnoreLayerCollision(layerA, layerB);
		}
		private void SetValue(int layerA, int layerB, bool val)
		{
			Physics2D.IgnoreLayerCollision(layerA, layerB, !val);
		}
		public override void OnInspectorGUI()
		{
			base.DrawDefaultInspector();
			LayerMatrixGUI.DoGUI("Layer Collision Matrix", ref this.show, ref this.scrollPos, new LayerMatrixGUI.GetValueFunc(this.GetValue), new LayerMatrixGUI.SetValueFunc(this.SetValue));
		}
	}
}
