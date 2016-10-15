using System;

namespace UnityEditor
{
	[CustomEditor(typeof(DefaultAsset), isFallback = true)]
	internal class DefaultAssetInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			DefaultAsset defaultAsset = (DefaultAsset)this.target;
			if (defaultAsset.message.Length > 0)
			{
				EditorGUILayout.HelpBox(defaultAsset.message, (!defaultAsset.isWarning) ? MessageType.Info : MessageType.Warning);
			}
		}
	}
}
