using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[Obsolete("BaseHierarchySort is no longer supported because of performance reasons")]
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
