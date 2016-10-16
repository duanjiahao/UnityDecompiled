using System;
using UnityEngine;

namespace UnityEditor
{
	internal class GameViewSizesMenuItemProvider : IFlexibleMenuItemProvider
	{
		private readonly GameViewSizeGroup m_GameViewSizeGroup;

		public GameViewSizesMenuItemProvider(GameViewSizeGroupType gameViewSizeGroupType)
		{
			this.m_GameViewSizeGroup = ScriptableSingleton<GameViewSizes>.instance.GetGroup(gameViewSizeGroupType);
		}

		public int Count()
		{
			return this.m_GameViewSizeGroup.GetTotalCount();
		}

		public object GetItem(int index)
		{
			return this.m_GameViewSizeGroup.GetGameViewSize(index);
		}

		public int Add(object obj)
		{
			GameViewSize gameViewSize = GameViewSizesMenuItemProvider.CastToGameViewSize(obj);
			if (gameViewSize == null)
			{
				return -1;
			}
			this.m_GameViewSizeGroup.AddCustomSize(gameViewSize);
			ScriptableSingleton<GameViewSizes>.instance.SaveToHDD();
			return this.Count() - 1;
		}

		public void Replace(int index, object obj)
		{
			GameViewSize gameViewSize = GameViewSizesMenuItemProvider.CastToGameViewSize(obj);
			if (gameViewSize == null)
			{
				return;
			}
			if (index < this.m_GameViewSizeGroup.GetBuiltinCount())
			{
				Debug.LogError("Only custom game view sizes can be changed");
				return;
			}
			GameViewSize gameViewSize2 = this.m_GameViewSizeGroup.GetGameViewSize(index);
			if (gameViewSize2 != null)
			{
				gameViewSize2.Set(gameViewSize);
				ScriptableSingleton<GameViewSizes>.instance.SaveToHDD();
			}
		}

		public void Remove(int index)
		{
			if (index < this.m_GameViewSizeGroup.GetBuiltinCount())
			{
				Debug.LogError("Only custom game view sizes can be changed");
				return;
			}
			this.m_GameViewSizeGroup.RemoveCustomSize(index);
			ScriptableSingleton<GameViewSizes>.instance.SaveToHDD();
		}

		public object Create()
		{
			return new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, string.Empty);
		}

		public void Move(int index, int destIndex, bool insertAfterDestIndex)
		{
			Debug.LogError("Missing impl");
		}

		public string GetName(int index)
		{
			GameViewSize gameViewSize = this.m_GameViewSizeGroup.GetGameViewSize(index);
			if (gameViewSize != null)
			{
				return gameViewSize.displayText;
			}
			return string.Empty;
		}

		public bool IsModificationAllowed(int index)
		{
			return this.m_GameViewSizeGroup.IsCustomSize(index);
		}

		public int[] GetSeperatorIndices()
		{
			return new int[]
			{
				this.m_GameViewSizeGroup.GetBuiltinCount() - 1
			};
		}

		private static GameViewSize CastToGameViewSize(object obj)
		{
			GameViewSize result = obj as GameViewSize;
			if (obj == null)
			{
				Debug.LogError("Incorrect input");
				return null;
			}
			return result;
		}
	}
}
