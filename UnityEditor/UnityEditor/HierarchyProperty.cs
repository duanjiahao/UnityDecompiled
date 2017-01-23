using System;
using System.Runtime.CompilerServices;
using UnityEditor.Collaboration;
using UnityEditor.Connect;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
	public sealed class HierarchyProperty : IHierarchyProperty
	{
		private IntPtr m_Property;

		public extern int instanceID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern UnityEngine.Object pptrValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasChildren
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int depth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int[] ancestors
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int row
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int colorCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string guid
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool alphaSorted
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isValid
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isMainRepresentation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasFullPreviewImage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern IconDrawStyle iconDrawStyle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isFolder
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Texture2D icon
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern HierarchyProperty(HierarchyType hierarchytType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Reset();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Dispose();

		~HierarchyProperty()
		{
			this.Dispose();
		}

		public Scene GetScene()
		{
			Scene result;
			HierarchyProperty.INTERNAL_CALL_GetScene(this, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetScene(HierarchyProperty self, out Scene value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsExpanded(int[] expanded);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Next(int[] expanded);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool NextWithDepthCheck(int[] expanded, int minDepth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Previous(int[] expanded);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Parent();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Find(int instanceID, int[] expanded);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Skip(int count, int[] expanded);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int CountRemaining(int[] expanded);

		public void SetSearchFilter(string searchString, int mode)
		{
			SearchFilter searchFilter = SearchableEditorWindow.CreateFilter(searchString, (SearchableEditorWindow.SearchMode)mode);
			this.SetSearchFilter(searchFilter);
		}

		internal void SetSearchFilter(SearchFilter filter)
		{
			if (UnityConnect.instance.userInfo.whitelisted && Collab.instance.collabInfo.whitelisted)
			{
				this.SetSearchFilterINTERNAL(SearchFilter.Split(filter.nameFilter), filter.classNames, filter.assetLabels, filter.assetBundleNames, filter.versionControlStates, filter.referencingInstanceIDs, filter.scenePaths, filter.showAllHits);
			}
			else
			{
				this.SetSearchFilterINTERNAL(SearchFilter.Split(filter.nameFilter), filter.classNames, filter.assetLabels, filter.assetBundleNames, new string[0], filter.referencingInstanceIDs, filter.scenePaths, filter.showAllHits);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSearchFilterINTERNAL(string[] nameFilters, string[] classNames, string[] assetLabels, string[] assetBundleNames, string[] versionControlStates, int[] referencingInstanceIDs, string[] scenePaths, bool showAllHits);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int[] FindAllAncestors(int[] instanceIDs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearSceneObjectsFilter();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FilterSingleSceneObject(int instanceID, bool otherVisibilityState);
	}
}
