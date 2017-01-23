using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class EditorCache : IDisposable
	{
		private Dictionary<UnityEngine.Object, EditorWrapper> m_EditorCache;

		private Dictionary<UnityEngine.Object, bool> m_UsedEditors;

		private EditorFeatures m_Requirements;

		public EditorWrapper this[UnityEngine.Object o]
		{
			get
			{
				this.m_UsedEditors[o] = true;
				EditorWrapper result;
				if (this.m_EditorCache.ContainsKey(o))
				{
					result = this.m_EditorCache[o];
				}
				else
				{
					EditorWrapper editorWrapper = EditorWrapper.Make(o, this.m_Requirements);
					EditorWrapper editorWrapper2 = editorWrapper;
					this.m_EditorCache[o] = editorWrapper2;
					result = editorWrapper2;
				}
				return result;
			}
		}

		public EditorCache() : this(EditorFeatures.None)
		{
		}

		public EditorCache(EditorFeatures requirements)
		{
			this.m_Requirements = requirements;
			this.m_EditorCache = new Dictionary<UnityEngine.Object, EditorWrapper>();
			this.m_UsedEditors = new Dictionary<UnityEngine.Object, bool>();
		}

		public void CleanupUntouchedEditors()
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			foreach (UnityEngine.Object current in this.m_EditorCache.Keys)
			{
				if (!this.m_UsedEditors.ContainsKey(current))
				{
					list.Add(current);
				}
			}
			if (this.m_EditorCache != null)
			{
				foreach (UnityEngine.Object current2 in list)
				{
					EditorWrapper editorWrapper = this.m_EditorCache[current2];
					this.m_EditorCache.Remove(current2);
					if (editorWrapper != null)
					{
						editorWrapper.Dispose();
					}
				}
			}
			this.m_UsedEditors.Clear();
		}

		public void CleanupAllEditors()
		{
			this.m_UsedEditors.Clear();
			this.CleanupUntouchedEditors();
		}

		public void Dispose()
		{
			this.CleanupAllEditors();
			GC.SuppressFinalize(this);
		}

		~EditorCache()
		{
			Debug.LogError("Failed to dispose EditorCache.");
		}
	}
}
