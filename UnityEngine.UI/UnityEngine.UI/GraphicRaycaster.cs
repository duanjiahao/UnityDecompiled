using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("Event/Graphic Raycaster"), RequireComponent(typeof(Canvas))]
	public class GraphicRaycaster : BaseRaycaster
	{
		public enum BlockingObjects
		{
			None,
			TwoD,
			ThreeD,
			All
		}

		protected const int kNoEventMaskSet = -1;

		[FormerlySerializedAs("ignoreReversedGraphics"), SerializeField]
		private bool m_IgnoreReversedGraphics = true;

		[FormerlySerializedAs("blockingObjects"), SerializeField]
		private GraphicRaycaster.BlockingObjects m_BlockingObjects = GraphicRaycaster.BlockingObjects.None;

		[SerializeField]
		protected LayerMask m_BlockingMask = -1;

		private Canvas m_Canvas;

		[NonSerialized]
		private List<Graphic> m_RaycastResults = new List<Graphic>();

		[NonSerialized]
		private static readonly List<Graphic> s_SortedGraphics = new List<Graphic>();

		public override int sortOrderPriority
		{
			get
			{
				int result;
				if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					result = this.canvas.sortingOrder;
				}
				else
				{
					result = base.sortOrderPriority;
				}
				return result;
			}
		}

		public override int renderOrderPriority
		{
			get
			{
				int result;
				if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					result = this.canvas.renderOrder;
				}
				else
				{
					result = base.renderOrderPriority;
				}
				return result;
			}
		}

		public bool ignoreReversedGraphics
		{
			get
			{
				return this.m_IgnoreReversedGraphics;
			}
			set
			{
				this.m_IgnoreReversedGraphics = value;
			}
		}

		public GraphicRaycaster.BlockingObjects blockingObjects
		{
			get
			{
				return this.m_BlockingObjects;
			}
			set
			{
				this.m_BlockingObjects = value;
			}
		}

		private Canvas canvas
		{
			get
			{
				Canvas canvas;
				if (this.m_Canvas != null)
				{
					canvas = this.m_Canvas;
				}
				else
				{
					this.m_Canvas = base.GetComponent<Canvas>();
					canvas = this.m_Canvas;
				}
				return canvas;
			}
		}

		public override Camera eventCamera
		{
			get
			{
				Camera result;
				if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay || (this.canvas.renderMode == RenderMode.ScreenSpaceCamera && this.canvas.worldCamera == null))
				{
					result = null;
				}
				else
				{
					result = ((!(this.canvas.worldCamera != null)) ? Camera.main : this.canvas.worldCamera);
				}
				return result;
			}
		}

		protected GraphicRaycaster()
		{
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (!(this.canvas == null))
			{
				Vector3 vector = Display.RelativeMouseAt(eventData.position);
				int targetDisplay = this.canvas.targetDisplay;
				if (vector.z == (float)targetDisplay)
				{
					if (vector.z == 0f)
					{
						vector = eventData.position;
					}
					Vector2 vector2;
					if (this.eventCamera == null)
					{
						float num = (float)Screen.width;
						float num2 = (float)Screen.height;
						if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
						{
							num = (float)Display.displays[targetDisplay].systemWidth;
							num2 = (float)Display.displays[targetDisplay].systemHeight;
						}
						vector2 = new Vector2(vector.x / num, vector.y / num2);
					}
					else
					{
						vector2 = this.eventCamera.ScreenToViewportPoint(vector);
					}
					if (vector2.x >= 0f && vector2.x <= 1f && vector2.y >= 0f && vector2.y <= 1f)
					{
						float num3 = 3.40282347E+38f;
						Ray r = default(Ray);
						if (this.eventCamera != null)
						{
							r = this.eventCamera.ScreenPointToRay(vector);
						}
						if (this.canvas.renderMode != RenderMode.ScreenSpaceOverlay && this.blockingObjects != GraphicRaycaster.BlockingObjects.None)
						{
							float num4 = 100f;
							if (this.eventCamera != null)
							{
								num4 = this.eventCamera.farClipPlane - this.eventCamera.nearClipPlane;
							}
							if (this.blockingObjects == GraphicRaycaster.BlockingObjects.ThreeD || this.blockingObjects == GraphicRaycaster.BlockingObjects.All)
							{
								if (ReflectionMethodsCache.Singleton.raycast3D != null)
								{
									RaycastHit raycastHit;
									if (ReflectionMethodsCache.Singleton.raycast3D(r, out raycastHit, num4, this.m_BlockingMask))
									{
										num3 = raycastHit.distance;
									}
								}
							}
							if (this.blockingObjects == GraphicRaycaster.BlockingObjects.TwoD || this.blockingObjects == GraphicRaycaster.BlockingObjects.All)
							{
								if (ReflectionMethodsCache.Singleton.raycast2D != null)
								{
									RaycastHit2D raycastHit2D = ReflectionMethodsCache.Singleton.raycast2D(r.origin, r.direction, num4, this.m_BlockingMask);
									if (raycastHit2D.collider)
									{
										num3 = raycastHit2D.fraction * num4;
									}
								}
							}
						}
						this.m_RaycastResults.Clear();
						GraphicRaycaster.Raycast(this.canvas, this.eventCamera, vector, this.m_RaycastResults);
						int i = 0;
						while (i < this.m_RaycastResults.Count)
						{
							GameObject gameObject = this.m_RaycastResults[i].gameObject;
							bool flag = true;
							if (this.ignoreReversedGraphics)
							{
								if (this.eventCamera == null)
								{
									Vector3 rhs = gameObject.transform.rotation * Vector3.forward;
									flag = (Vector3.Dot(Vector3.forward, rhs) > 0f);
								}
								else
								{
									Vector3 lhs = this.eventCamera.transform.rotation * Vector3.forward;
									Vector3 rhs2 = gameObject.transform.rotation * Vector3.forward;
									flag = (Vector3.Dot(lhs, rhs2) > 0f);
								}
							}
							if (flag)
							{
								float num5;
								if (this.eventCamera == null || this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
								{
									num5 = 0f;
								}
								else
								{
									Transform transform = gameObject.transform;
									Vector3 forward = transform.forward;
									num5 = Vector3.Dot(forward, transform.position - r.origin) / Vector3.Dot(forward, r.direction);
									if (num5 < 0f)
									{
										goto IL_48A;
									}
								}
								if (num5 < num3)
								{
									RaycastResult item = new RaycastResult
									{
										gameObject = gameObject,
										module = this,
										distance = num5,
										screenPosition = vector,
										index = (float)resultAppendList.Count,
										depth = this.m_RaycastResults[i].depth,
										sortingLayer = this.canvas.sortingLayerID,
										sortingOrder = this.canvas.sortingOrder
									};
									resultAppendList.Add(item);
								}
							}
							IL_48A:
							i++;
							continue;
							goto IL_48A;
						}
					}
				}
			}
		}

		private static void Raycast(Canvas canvas, Camera eventCamera, Vector2 pointerPosition, List<Graphic> results)
		{
			IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
			for (int i = 0; i < graphicsForCanvas.Count; i++)
			{
				Graphic graphic = graphicsForCanvas[i];
				if (graphic.depth != -1 && graphic.raycastTarget)
				{
					if (RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera))
					{
						if (graphic.Raycast(pointerPosition, eventCamera))
						{
							GraphicRaycaster.s_SortedGraphics.Add(graphic);
						}
					}
				}
			}
			GraphicRaycaster.s_SortedGraphics.Sort((Graphic g1, Graphic g2) => g2.depth.CompareTo(g1.depth));
			for (int j = 0; j < GraphicRaycaster.s_SortedGraphics.Count; j++)
			{
				results.Add(GraphicRaycaster.s_SortedGraphics[j]);
			}
			GraphicRaycaster.s_SortedGraphics.Clear();
		}
	}
}
