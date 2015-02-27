using System;
using UnityEngine;
namespace UnityEditor
{
	public class AlphabeticalSort : BaseHierarchySort
	{
		private const string kDefaultSorting = "AlphabeticalSorting";
		private GUIContent m_Content = new GUIContent(EditorGUIUtility.FindTexture("AlphabeticalSorting"), "Alphabetical Order");
		public override GUIContent content
		{
			get
			{
				return this.m_Content;
			}
		}
		public override int Compare(GameObject lhs, GameObject rhs)
		{
			if (lhs == rhs)
			{
				return 0;
			}
			if (lhs == null)
			{
				return -1;
			}
			if (rhs == null)
			{
				return 1;
			}
			return EditorUtility.NaturalCompareObjectNames(lhs, rhs);
		}
	}
}
