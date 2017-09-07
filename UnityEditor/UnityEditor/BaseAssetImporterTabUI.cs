using System;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace UnityEditor
{
	internal abstract class BaseAssetImporterTabUI
	{
		private AssetImporterEditor m_PanelContainer = null;

		protected Func<UnityEngine.Object, UnityEngine.Object> Instantiate;

		protected Action<UnityEngine.Object> DestroyImmediate;

		public SerializedObject serializedObject
		{
			get
			{
				return this.m_PanelContainer.serializedObject;
			}
		}

		public UnityEngine.Object[] targets
		{
			get
			{
				return this.m_PanelContainer.targets;
			}
		}

		public UnityEngine.Object target
		{
			get
			{
				return this.m_PanelContainer.target;
			}
		}

		public int referenceTargetIndex
		{
			get
			{
				return this.m_PanelContainer.referenceTargetIndex;
			}
			set
			{
				this.m_PanelContainer.referenceTargetIndex = value;
			}
		}

		internal BaseAssetImporterTabUI(AssetImporterEditor panelContainer)
		{
			this.m_PanelContainer = panelContainer;
			this.Instantiate = ((UnityEngine.Object obj) => UnityEngine.Object.Instantiate(obj));
			this.DestroyImmediate = delegate(UnityEngine.Object obj)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			};
		}

		internal abstract void OnEnable();

		internal virtual void OnDisable()
		{
		}

		internal virtual void PreApply()
		{
		}

		internal virtual void PostApply()
		{
		}

		internal virtual void ResetValues()
		{
		}

		public abstract void OnInspectorGUI();

		internal virtual bool HasModified()
		{
			return this.serializedObject.hasModifiedProperties;
		}

		public virtual void OnPreviewSettings()
		{
		}

		public virtual void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			this.m_PanelContainer.OnPreviewGUI(r, background);
		}

		public virtual bool HasPreviewGUI()
		{
			return true;
		}

		internal virtual void OnDestroy()
		{
		}
	}
}
