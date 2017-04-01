using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.EventSystems
{
	[AddComponentMenu("Event/Physics Raycaster"), RequireComponent(typeof(Camera))]
	public class PhysicsRaycaster : BaseRaycaster
	{
		protected const int kNoEventMaskSet = -1;

		protected Camera m_EventCamera;

		[SerializeField]
		protected LayerMask m_EventMask = -1;

		public override Camera eventCamera
		{
			get
			{
				if (this.m_EventCamera == null)
				{
					this.m_EventCamera = base.GetComponent<Camera>();
				}
				return this.m_EventCamera ?? Camera.main;
			}
		}

		public virtual int depth
		{
			get
			{
				return (!(this.eventCamera != null)) ? 16777215 : ((int)this.eventCamera.depth);
			}
		}

		public int finalEventMask
		{
			get
			{
				return (!(this.eventCamera != null)) ? -1 : (this.eventCamera.cullingMask & this.m_EventMask);
			}
		}

		public LayerMask eventMask
		{
			get
			{
				return this.m_EventMask;
			}
			set
			{
				this.m_EventMask = value;
			}
		}

		protected PhysicsRaycaster()
		{
		}

		protected void ComputeRayAndDistance(PointerEventData eventData, out Ray ray, out float distanceToClipPlane)
		{
			ray = this.eventCamera.ScreenPointToRay(eventData.position);
			float z = ray.direction.z;
			distanceToClipPlane = ((!Mathf.Approximately(0f, z)) ? Mathf.Abs((this.eventCamera.farClipPlane - this.eventCamera.nearClipPlane) / z) : float.PositiveInfinity);
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (!(this.eventCamera == null))
			{
				Ray r;
				float f;
				this.ComputeRayAndDistance(eventData, out r, out f);
				if (ReflectionMethodsCache.Singleton.raycast3DAll != null)
				{
					RaycastHit[] array = ReflectionMethodsCache.Singleton.raycast3DAll(r, f, this.finalEventMask);
					if (array.Length > 1)
					{
						Array.Sort<RaycastHit>(array, (RaycastHit r1, RaycastHit r2) => r1.distance.CompareTo(r2.distance));
					}
					if (array.Length != 0)
					{
						int i = 0;
						int num = array.Length;
						while (i < num)
						{
							RaycastResult item = new RaycastResult
							{
								gameObject = array[i].collider.gameObject,
								module = this,
								distance = array[i].distance,
								worldPosition = array[i].point,
								worldNormal = array[i].normal,
								screenPosition = eventData.position,
								index = (float)resultAppendList.Count,
								sortingLayer = 0,
								sortingOrder = 0
							};
							resultAppendList.Add(item);
							i++;
						}
					}
				}
			}
		}
	}
}
