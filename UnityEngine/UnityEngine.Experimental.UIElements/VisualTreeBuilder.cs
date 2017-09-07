using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class VisualTreeBuilder
	{
		private struct ViewState
		{
			public readonly int childIndex;

			public ViewState(int childIndex)
			{
				this.childIndex = childIndex;
			}
		}

		private Recycler m_ElementPool;

		private VisualContainer m_CurrentContainer;

		private VisualTreeBuilder.ViewState m_CurrentViewState;

		private Stack<VisualTreeBuilder.ViewState> m_ViewStates;

		public VisualContainer topLevelVisualContainer
		{
			get;
			private set;
		}

		public bool verbose
		{
			get;
			set;
		}

		public IMScrollView currentScrollView
		{
			get;
			private set;
		}

		public VisualTreeBuilder(Recycler r)
		{
			this.m_ElementPool = r;
			this.m_ViewStates = new Stack<VisualTreeBuilder.ViewState>();
			this.m_CurrentContainer = null;
			this.m_CurrentViewState = new VisualTreeBuilder.ViewState(0);
			this.topLevelVisualContainer = null;
			this.verbose = false;
			this.currentScrollView = null;
		}

		public void NextView<TType>(out TType view) where TType : IMContainer, new()
		{
			this.NextElement<TType>(out view);
			this.m_CurrentContainer = view;
			this.m_ViewStates.Push(this.m_CurrentViewState);
			this.m_CurrentViewState = new VisualTreeBuilder.ViewState(0);
			IMScrollView iMScrollView = this.m_CurrentContainer as IMScrollView;
			if (iMScrollView != null)
			{
				this.currentScrollView = iMScrollView;
			}
		}

		public void EndView()
		{
			if (this.m_ViewStates.Count < 1)
			{
				Debug.Assert(this.m_CurrentContainer == this.topLevelVisualContainer);
				if (this.verbose)
				{
					Debug.LogError("Unexpected call to EndView()");
				}
			}
			else
			{
				if (this.m_CurrentContainer.childrenCount > this.m_CurrentViewState.childIndex)
				{
					this.RecycleDescendants(this.m_CurrentContainer, this.m_CurrentViewState.childIndex);
				}
				Debug.Assert(this.m_CurrentContainer.parent != null);
				if (this.m_CurrentContainer is IMScrollView)
				{
					VisualContainer parent = this.m_CurrentContainer.parent;
					IMScrollView iMScrollView = parent as IMScrollView;
					while (parent != null && iMScrollView == null)
					{
						parent = parent.parent;
						iMScrollView = (parent as IMScrollView);
					}
					this.currentScrollView = iMScrollView;
				}
				this.m_CurrentViewState = this.m_ViewStates.Pop();
				this.m_CurrentContainer = this.m_CurrentContainer.parent;
			}
		}

		public void BeginGUI(VisualContainer container)
		{
			this.topLevelVisualContainer = container;
			this.m_CurrentContainer = this.topLevelVisualContainer;
			this.m_CurrentViewState = new VisualTreeBuilder.ViewState(0);
			this.m_ViewStates.Clear();
		}

		public void EndGUI()
		{
			if (this.topLevelVisualContainer == null)
			{
				Debug.Log("topLevelVisualContainer == null");
			}
			else
			{
				if (this.m_CurrentContainer != this.topLevelVisualContainer)
				{
					if (this.verbose)
					{
						Debug.LogWarning("Non-symmetrical VisualContainer calls to GUI");
					}
					while (this.m_CurrentContainer != this.topLevelVisualContainer)
					{
						this.EndView();
					}
				}
				if (this.topLevelVisualContainer.childrenCount > this.m_CurrentViewState.childIndex)
				{
					this.RecycleDescendants(this.topLevelVisualContainer, this.m_CurrentViewState.childIndex);
				}
				this.topLevelVisualContainer = null;
				this.m_CurrentContainer = null;
			}
		}

		public void NextElement<TType>(out TType widget) where TType : VisualElement, IOnGUIHandler, new()
		{
			Type typeFromHandle = typeof(TType);
			widget = (TType)((object)null);
			if (this.m_CurrentViewState.childIndex >= this.m_CurrentContainer.childrenCount)
			{
				TType arg_51_1;
				if ((arg_51_1 = this.m_ElementPool.TryReuse<TType>()) == null)
				{
					arg_51_1 = Activator.CreateInstance<TType>();
				}
				widget = arg_51_1;
				this.m_CurrentContainer.AddChild(widget);
			}
			else
			{
				VisualElement childAt = this.m_CurrentContainer.GetChildAt(this.m_CurrentViewState.childIndex);
				if (childAt.GetType() == typeFromHandle)
				{
					widget = (childAt as TType);
				}
				else
				{
					IOnGUIHandler onGUIHandler = childAt as IOnGUIHandler;
					if (onGUIHandler != null && onGUIHandler.id == 0 && !(onGUIHandler is VisualContainer))
					{
						this.m_CurrentContainer.RemoveChildAt(this.m_CurrentViewState.childIndex);
						this.m_ElementPool.Trash(onGUIHandler);
						this.NextElement<TType>(out widget);
						return;
					}
					TType arg_11E_1;
					if ((arg_11E_1 = this.m_ElementPool.TryReuse<TType>()) == null)
					{
						arg_11E_1 = Activator.CreateInstance<TType>();
					}
					widget = arg_11E_1;
					this.m_CurrentContainer.InsertChild(this.m_CurrentViewState.childIndex, widget);
				}
			}
			Debug.Assert(widget != null);
			this.m_CurrentViewState = new VisualTreeBuilder.ViewState(this.m_CurrentViewState.childIndex + 1);
		}

		private void RecycleDescendants(VisualContainer parent, int startAtIndex)
		{
			while (parent.childrenCount > startAtIndex)
			{
				VisualElement childAt = parent.GetChildAt(startAtIndex);
				VisualContainer visualContainer = childAt as VisualContainer;
				if (visualContainer != null)
				{
					this.RecycleDescendants(visualContainer, 0);
				}
				IOnGUIHandler onGUIHandler = childAt as IOnGUIHandler;
				if (onGUIHandler != null)
				{
					this.m_ElementPool.Trash(onGUIHandler);
					parent.RemoveChild(childAt);
				}
				else
				{
					startAtIndex++;
				}
			}
		}
	}
}
