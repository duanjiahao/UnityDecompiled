using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class GameViewSizeGroup
	{
		[NonSerialized]
		private List<GameViewSize> m_Builtin = new List<GameViewSize>();

		[SerializeField]
		private List<GameViewSize> m_Custom = new List<GameViewSize>();

		public GameViewSize GetGameViewSize(int index)
		{
			if (index < this.m_Builtin.Count)
			{
				return this.m_Builtin[index];
			}
			index -= this.m_Builtin.Count;
			if (index >= 0 && index < this.m_Custom.Count)
			{
				return this.m_Custom[index];
			}
			Debug.LogError(string.Concat(new object[]
			{
				"Invalid index ",
				index + this.m_Builtin.Count,
				" ",
				this.m_Builtin.Count,
				" ",
				this.m_Custom.Count
			}));
			return new GameViewSize(GameViewSizeType.AspectRatio, 0, 0, string.Empty);
		}

		public string[] GetDisplayTexts()
		{
			List<string> list = new List<string>();
			foreach (GameViewSize current in this.m_Builtin)
			{
				list.Add(current.displayText);
			}
			foreach (GameViewSize current2 in this.m_Custom)
			{
				list.Add(current2.displayText);
			}
			return list.ToArray();
		}

		public int GetTotalCount()
		{
			return this.m_Builtin.Count + this.m_Custom.Count;
		}

		public int GetBuiltinCount()
		{
			return this.m_Builtin.Count;
		}

		public int GetCustomCount()
		{
			return this.m_Custom.Count;
		}

		public void AddBuiltinSizes(params GameViewSize[] sizes)
		{
			for (int i = 0; i < sizes.Length; i++)
			{
				this.AddBuiltinSize(sizes[i]);
			}
		}

		public void AddBuiltinSize(GameViewSize size)
		{
			this.m_Builtin.Add(size);
			ScriptableSingleton<GameViewSizes>.instance.Changed();
		}

		public void AddCustomSizes(params GameViewSize[] sizes)
		{
			for (int i = 0; i < sizes.Length; i++)
			{
				this.AddCustomSize(sizes[i]);
			}
		}

		public void AddCustomSize(GameViewSize size)
		{
			this.m_Custom.Add(size);
			ScriptableSingleton<GameViewSizes>.instance.Changed();
		}

		public void RemoveCustomSize(int index)
		{
			int num = this.TotalIndexToCustomIndex(index);
			if (num >= 0 && num < this.m_Custom.Count)
			{
				this.m_Custom.RemoveAt(num);
				ScriptableSingleton<GameViewSizes>.instance.Changed();
			}
			else
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Invalid index ",
					index,
					" ",
					this.m_Builtin.Count,
					" ",
					this.m_Custom.Count
				}));
			}
		}

		public bool IsCustomSize(int index)
		{
			return index >= this.m_Builtin.Count;
		}

		public int TotalIndexToCustomIndex(int index)
		{
			return index - this.m_Builtin.Count;
		}

		public int IndexOf(GameViewSize view)
		{
			int num = this.m_Builtin.IndexOf(view);
			if (num >= 0)
			{
				return num;
			}
			return this.m_Custom.IndexOf(view);
		}
	}
}
