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
			int result;
			if (gameViewSize == null)
			{
				result = -1;
			}
			else
			{
				this.m_GameViewSizeGroup.AddCustomSize(gameViewSize);
				ScriptableSingleton<GameViewSizes>.instance.SaveToHDD();
				result = this.Count() - 1;
			}
			return result;
		}

		public void Replace(int index, object obj)
		{
			GameViewSize gameViewSize = GameViewSizesMenuItemProvider.CastToGameViewSize(obj);
			if (gameViewSize != null)
			{
				if (index < this.m_GameViewSizeGroup.GetBuiltinCount())
				{
					Debug.LogError("Only custom game view sizes can be changed");
				}
				else
				{
					GameViewSize gameViewSize2 = this.m_GameViewSizeGroup.GetGameViewSize(index);
					if (gameViewSize2 != null)
					{
						gameViewSize2.Set(gameViewSize);
						ScriptableSingleton<GameViewSizes>.instance.SaveToHDD();
					}
				}
			}
		}

		public void Remove(int index)
		{
			if (index < this.m_GameViewSizeGroup.GetBuiltinCount())
			{
				Debug.LogError("Only custom game view sizes can be changed");
			}
			else
			{
				this.m_GameViewSizeGroup.RemoveCustomSize(index);
				ScriptableSingleton<GameViewSizes>.instance.SaveToHDD();
			}
		}

		public object Create()
		{
			return new GameViewSize(GameViewSizeType.FixedResolution, 0, 0, "");
		}

		public void Move(int index, int destIndex, bool insertAfterDestIndex)
		{
			Debug.LogError("Missing impl");
		}

		public string GetName(int index)
		{
			GameViewSize gameViewSize = this.m_GameViewSizeGroup.GetGameViewSize(index);
			string result;
			if (gameViewSize != null)
			{
				result = gameViewSize.displayText;
			}
			else
			{
				result = "";
			}
			return result;
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
			GameViewSize gameViewSize = obj as GameViewSize;
			GameViewSize result;
			if (obj == null)
			{
				Debug.LogError("Incorrect input");
				result = null;
			}
			else
			{
				result = gameViewSize;
			}
			return result;
		}
	}
}
