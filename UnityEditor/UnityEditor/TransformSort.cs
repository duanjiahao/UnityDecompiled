using System;
using UnityEngine;
namespace UnityEditor
{
	public class TransformSort : BaseHierarchySort
	{
		private const string kDefaultSorting = "DefaultSorting";
		private GUIContent m_Content = new GUIContent(EditorGUIUtility.FindTexture("DefaultSorting"), "Transform child order");
		public override GUIContent content
		{
			get
			{
				return this.m_Content;
			}
		}
	}
}
