using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor
{
	public abstract class BaseHierarchySort : IComparer<GameObject>
	{
		public virtual GUIContent content
		{
			get
			{
				return null;
			}
		}
		public virtual int Compare(GameObject lhs, GameObject rhs)
		{
			return 0;
		}
	}
}
