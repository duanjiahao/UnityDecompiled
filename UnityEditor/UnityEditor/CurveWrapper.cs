using System;
using UnityEngine;

namespace UnityEditor
{
	internal class CurveWrapper
	{
		internal enum SelectionMode
		{
			None,
			Selected,
			SemiSelected
		}

		public delegate Vector2 GetAxisScalarsCallback();

		public delegate void SetAxisScalarsCallback(Vector2 newAxisScalars);

		private CurveRenderer m_Renderer;

		private ISelectionBinding m_SelectionBinding;

		public int id;

		public EditorCurveBinding binding;

		public int groupId;

		public int regionId;

		public Color color;

		public bool readOnly;

		public bool hidden;

		public CurveWrapper.GetAxisScalarsCallback getAxisUiScalarsCallback;

		public CurveWrapper.SetAxisScalarsCallback setAxisUiScalarsCallback;

		public bool changed;

		public CurveWrapper.SelectionMode selected;

		public int listIndex;

		public float vRangeMin = float.NegativeInfinity;

		public float vRangeMax = float.PositiveInfinity;

		public CurveRenderer renderer
		{
			get
			{
				return this.m_Renderer;
			}
			set
			{
				this.m_Renderer = value;
			}
		}

		public AnimationCurve curve
		{
			get
			{
				return this.renderer.GetCurve();
			}
		}

		public float timeOffset
		{
			get
			{
				return (this.m_SelectionBinding == null) ? 0f : this.m_SelectionBinding.timeOffset;
			}
		}

		public bool clipIsEditable
		{
			get
			{
				return this.m_SelectionBinding == null || this.m_SelectionBinding.clipIsEditable;
			}
		}

		public bool animationIsEditable
		{
			get
			{
				return this.m_SelectionBinding == null || this.m_SelectionBinding.animationIsEditable;
			}
		}

		public int selectionID
		{
			get
			{
				return (this.m_SelectionBinding == null) ? 0 : this.m_SelectionBinding.id;
			}
		}

		public ISelectionBinding selectionBindingInterface
		{
			get
			{
				return this.m_SelectionBinding;
			}
			set
			{
				this.m_SelectionBinding = value;
			}
		}

		public CurveWrapper()
		{
			this.id = 0;
			this.groupId = -1;
			this.regionId = -1;
			this.hidden = false;
			this.readOnly = false;
			this.listIndex = -1;
			this.getAxisUiScalarsCallback = null;
			this.setAxisUiScalarsCallback = null;
		}
	}
}
