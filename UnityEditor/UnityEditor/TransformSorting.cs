using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TransformSorting : HierarchySorting
	{
		private readonly GUIContent m_Content = new GUIContent(EditorGUIUtility.FindTexture("DefaultSorting"), "Transform Child Order");

		public override GUIContent content
		{
			get
			{
				return this.m_Content;
			}
		}
	}
}
