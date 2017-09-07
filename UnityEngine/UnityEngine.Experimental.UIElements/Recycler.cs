using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class Recycler
	{
		public const int MaxInstancesPerType = 500;

		private Dictionary<Type, Stack<IRecyclable>> m_ReusableStacks = new Dictionary<Type, Stack<IRecyclable>>();

		public int Count
		{
			get
			{
				int num = 0;
				Dictionary<Type, Stack<IRecyclable>>.Enumerator enumerator = this.m_ReusableStacks.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<Type, Stack<IRecyclable>> current = enumerator.Current;
					num += current.Value.Count;
				}
				return num;
			}
		}

		public void Trash(IRecyclable recyclable)
		{
			if (recyclable.isTrashed)
			{
				throw new ArgumentException("Trying to add an element to the Recycler more than once");
			}
			Type type = recyclable.GetType();
			Stack<IRecyclable> stack;
			if (!this.m_ReusableStacks.TryGetValue(type, out stack))
			{
				stack = new Stack<IRecyclable>();
				this.m_ReusableStacks.Add(type, stack);
			}
			recyclable.isTrashed = true;
			recyclable.OnTrash();
			if (stack.Count < 500)
			{
				stack.Push(recyclable);
			}
		}

		public void Clear()
		{
			this.m_ReusableStacks.Clear();
		}

		public TType TryReuse<TType>() where TType : IRecyclable
		{
			Type typeFromHandle = typeof(TType);
			TType result = default(TType);
			Stack<IRecyclable> stack;
			if (this.m_ReusableStacks.TryGetValue(typeFromHandle, out stack))
			{
				if (stack.Count > 0)
				{
					result = (TType)((object)stack.Pop());
					result.isTrashed = false;
					result.OnReuse();
				}
			}
			return result;
		}
	}
}
