using System;
using UnityEngine;
using UnityEngine.U2D.Interface;

namespace UnityEditor
{
	internal class ShapeEditorFactory : IShapeEditorFactory
	{
		public ShapeEditor CreateShapeEditor()
		{
			return new ShapeEditor(new GUIUtilitySystem(), new EventSystem());
		}
	}
}
