using System;
using UnityEngine;

namespace UnityEditor
{
	internal interface IHierarchyProperty
	{
		int instanceID
		{
			get;
		}

		UnityEngine.Object pptrValue
		{
			get;
		}

		string name
		{
			get;
		}

		bool hasChildren
		{
			get;
		}

		int depth
		{
			get;
		}

		int row
		{
			get;
		}

		int colorCode
		{
			get;
		}

		string guid
		{
			get;
		}

		Texture2D icon
		{
			get;
		}

		bool isValid
		{
			get;
		}

		bool isMainRepresentation
		{
			get;
		}

		bool hasFullPreviewImage
		{
			get;
		}

		IconDrawStyle iconDrawStyle
		{
			get;
		}

		bool isFolder
		{
			get;
		}

		int[] ancestors
		{
			get;
		}

		void Reset();

		bool IsExpanded(int[] expanded);

		bool Next(int[] expanded);

		bool NextWithDepthCheck(int[] expanded, int minDepth);

		bool Previous(int[] expanded);

		bool Parent();

		bool Find(int instanceID, int[] expanded);

		int[] FindAllAncestors(int[] instanceIDs);

		bool Skip(int count, int[] expanded);

		int CountRemaining(int[] expanded);
	}
}
