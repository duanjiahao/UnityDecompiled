using System;

namespace UnityEditorInternal
{
	public enum MemoryInfoGCReason
	{
		SceneObject,
		BuiltinResource,
		MarkedDontSave,
		AssetMarkedDirtyInEditor,
		SceneAssetReferencedByNativeCodeOnly = 5,
		SceneAssetReferenced,
		AssetReferencedByNativeCodeOnly = 8,
		AssetReferenced,
		NotApplicable
	}
}
