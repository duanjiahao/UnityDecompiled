using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditorInternal
{
	[Serializable]
	internal class AnimationWindowSelection
	{
		[NonSerialized]
		public Action onSelectionChanged;

		[SerializeField]
		private bool m_Locked;

		[SerializeField]
		private List<AnimationWindowSelectionItem> m_Selection = new List<AnimationWindowSelectionItem>();

		private bool m_BatchOperations;

		private bool m_SelectionChanged;

		private List<AnimationWindowCurve> m_CurvesCache;

		public int count
		{
			get
			{
				return this.m_Selection.Count;
			}
		}

		public List<AnimationWindowCurve> curves
		{
			get
			{
				if (this.m_CurvesCache == null)
				{
					this.m_CurvesCache = new List<AnimationWindowCurve>();
					foreach (AnimationWindowSelectionItem current in this.m_Selection)
					{
						this.m_CurvesCache.AddRange(current.curves);
					}
				}
				return this.m_CurvesCache;
			}
		}

		public bool locked
		{
			get
			{
				return this.m_Locked;
			}
			set
			{
				if (!this.disabled)
				{
					this.m_Locked = value;
				}
			}
		}

		public bool disabled
		{
			get
			{
				if (this.m_Selection.Count > 0)
				{
					foreach (AnimationWindowSelectionItem current in this.m_Selection)
					{
						if (current.animationClip != null)
						{
							return false;
						}
					}
					return true;
				}
				return true;
			}
		}

		public AnimationWindowSelection()
		{
			this.onSelectionChanged = (Action)Delegate.Combine(this.onSelectionChanged, new Action(delegate
			{
			}));
		}

		public void BeginOperations()
		{
			if (this.m_BatchOperations)
			{
				Debug.LogWarning("AnimationWindowSelection: Already inside a BeginOperations/EndOperations block");
				return;
			}
			this.m_BatchOperations = true;
			this.m_SelectionChanged = false;
		}

		public void EndOperations()
		{
			if (this.m_BatchOperations)
			{
				if (this.m_SelectionChanged)
				{
					this.onSelectionChanged();
				}
				this.m_SelectionChanged = false;
				this.m_BatchOperations = false;
			}
		}

		public void Notify()
		{
			if (this.m_BatchOperations)
			{
				this.m_SelectionChanged = true;
			}
			else
			{
				this.onSelectionChanged();
			}
		}

		public void Set(AnimationWindowSelectionItem newItem)
		{
			if (this.locked)
			{
				return;
			}
			this.BeginOperations();
			this.Clear();
			this.Add(newItem);
			this.EndOperations();
		}

		public void Add(AnimationWindowSelectionItem newItem)
		{
			if (this.locked)
			{
				return;
			}
			if (!this.m_Selection.Contains(newItem))
			{
				this.m_Selection.Add(newItem);
				this.Notify();
			}
		}

		public void RangeAdd(AnimationWindowSelectionItem[] newItemArray)
		{
			if (this.locked)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < newItemArray.Length; i++)
			{
				AnimationWindowSelectionItem item = newItemArray[i];
				if (!this.m_Selection.Contains(item))
				{
					this.m_Selection.Add(item);
					flag = true;
				}
			}
			if (flag)
			{
				this.Notify();
			}
		}

		public void UpdateClip(AnimationWindowSelectionItem itemToUpdate, AnimationClip newClip)
		{
			if (this.m_Selection.Contains(itemToUpdate))
			{
				itemToUpdate.animationClip = newClip;
				this.Notify();
			}
		}

		public void UpdateTimeOffset(AnimationWindowSelectionItem itemToUpdate, float timeOffset)
		{
			if (this.m_Selection.Contains(itemToUpdate))
			{
				itemToUpdate.timeOffset = timeOffset;
			}
		}

		public bool Exists(AnimationWindowSelectionItem itemToFind)
		{
			return this.m_Selection.Contains(itemToFind);
		}

		public bool Exists(Predicate<AnimationWindowSelectionItem> predicate)
		{
			return this.m_Selection.Exists(predicate);
		}

		public AnimationWindowSelectionItem Find(Predicate<AnimationWindowSelectionItem> predicate)
		{
			return this.m_Selection.Find(predicate);
		}

		public AnimationWindowSelectionItem First()
		{
			return this.m_Selection.First<AnimationWindowSelectionItem>();
		}

		public int GetRefreshHash()
		{
			int num = 0;
			foreach (AnimationWindowSelectionItem current in this.m_Selection)
			{
				num ^= current.GetRefreshHash();
			}
			return num;
		}

		public void Refresh()
		{
			this.ClearCache();
			foreach (AnimationWindowSelectionItem current in this.m_Selection)
			{
				current.ClearCache();
			}
		}

		public AnimationWindowSelectionItem[] ToArray()
		{
			return this.m_Selection.ToArray();
		}

		public void Clear()
		{
			if (this.locked)
			{
				return;
			}
			if (this.m_Selection.Count > 0)
			{
				foreach (AnimationWindowSelectionItem current in this.m_Selection)
				{
					UnityEngine.Object.DestroyImmediate(current);
				}
				this.m_Selection.Clear();
				this.Notify();
			}
		}

		public void ClearCache()
		{
			this.m_CurvesCache = null;
		}
	}
}
