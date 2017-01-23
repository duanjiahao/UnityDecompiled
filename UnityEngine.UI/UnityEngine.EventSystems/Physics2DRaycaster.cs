using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.EventSystems
{
	[AddComponentMenu("Event/Physics 2D Raycaster"), RequireComponent(typeof(Camera))]
	public class Physics2DRaycaster : PhysicsRaycaster
	{
		protected Physics2DRaycaster()
		{
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (!(this.eventCamera == null))
			{
				Ray r = this.eventCamera.ScreenPointToRay(eventData.position);
				float f = this.eventCamera.farClipPlane - this.eventCamera.nearClipPlane;
				if (ReflectionMethodsCache.Singleton.getRayIntersectionAll != null)
				{
					RaycastHit2D[] array = ReflectionMethodsCache.Singleton.getRayIntersectionAll(r, f, base.finalEventMask);
					if (array.Length != 0)
					{
						int i = 0;
						int num = array.Length;
						while (i < num)
						{
							SpriteRenderer component = array[i].collider.gameObject.GetComponent<SpriteRenderer>();
							RaycastResult item = new RaycastResult
							{
								gameObject = array[i].collider.gameObject,
								module = this,
								distance = Vector3.Distance(this.eventCamera.transform.position, array[i].point),
								worldPosition = array[i].point,
								worldNormal = array[i].normal,
								screenPosition = eventData.position,
								index = (float)resultAppendList.Count,
								sortingLayer = ((!(component != null)) ? 0 : component.sortingLayerID),
								sortingOrder = ((!(component != null)) ? 0 : component.sortingOrder)
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
