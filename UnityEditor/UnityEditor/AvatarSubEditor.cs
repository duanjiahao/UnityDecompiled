using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class AvatarSubEditor : ScriptableObject
	{
		protected AvatarEditor m_Inspector;

		protected GameObject gameObject
		{
			get
			{
				return this.m_Inspector.m_GameObject;
			}
		}

		protected GameObject prefab
		{
			get
			{
				return this.m_Inspector.prefab;
			}
		}

		protected Dictionary<Transform, bool> modelBones
		{
			get
			{
				return this.m_Inspector.m_ModelBones;
			}
		}

		protected Transform root
		{
			get
			{
				return (!(this.gameObject == null)) ? this.gameObject.transform : null;
			}
		}

		protected SerializedObject serializedObject
		{
			get
			{
				return this.m_Inspector.serializedObject;
			}
		}

		protected Avatar avatarAsset
		{
			get
			{
				return this.m_Inspector.avatar;
			}
		}

		private static void DoWriteAllAssets()
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
			UnityEngine.Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				UnityEngine.Object @object = array2[i];
				if (AssetDatabase.Contains(@object))
				{
					EditorUtility.SetDirty(@object);
				}
			}
			AssetDatabase.SaveAssets();
		}

		public virtual void Enable(AvatarEditor inspector)
		{
			this.m_Inspector = inspector;
		}

		public virtual void Disable()
		{
		}

		public virtual void OnDestroy()
		{
			if (this.HasModified())
			{
				AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.avatarAsset));
				if (atPath)
				{
					if (EditorUtility.DisplayDialog("Unapplied import settings", "Unapplied import settings for '" + atPath.assetPath + "'", "Apply", "Revert"))
					{
						this.ApplyAndImport();
					}
					else
					{
						this.ResetValues();
					}
				}
			}
		}

		public virtual void OnInspectorGUI()
		{
		}

		public virtual void OnSceneGUI()
		{
		}

		protected bool HasModified()
		{
			return this.serializedObject.hasModifiedProperties;
		}

		protected virtual void ResetValues()
		{
			this.serializedObject.Update();
		}

		protected void Apply()
		{
			this.serializedObject.ApplyModifiedProperties();
		}

		public void ApplyAndImport()
		{
			this.Apply();
			string assetPath = AssetDatabase.GetAssetPath(this.avatarAsset);
			AssetDatabase.ImportAsset(assetPath);
			this.ResetValues();
		}

		protected void ApplyRevertGUI()
		{
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			using (new EditorGUI.DisabledScope(!this.HasModified()))
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Revert", new GUILayoutOption[0]))
				{
					this.ResetValues();
					if (this.HasModified())
					{
						Debug.LogError("Avatar tool reports modified values after reset.");
					}
				}
				if (GUILayout.Button("Apply", new GUILayoutOption[0]))
				{
					this.ApplyAndImport();
				}
			}
			if (GUILayout.Button("Done", new GUILayoutOption[0]))
			{
				this.m_Inspector.SwitchToAssetMode();
				GUIUtility.ExitGUI();
			}
			GUILayout.EndHorizontal();
		}
	}
}
