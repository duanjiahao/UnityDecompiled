using System;
using System.Collections.Generic;

namespace UnityEditor
{
	internal class PropertyHandlerCache
	{
		protected Dictionary<int, PropertyHandler> m_PropertyHandlers = new Dictionary<int, PropertyHandler>();

		internal PropertyHandler GetHandler(SerializedProperty property)
		{
			int propertyHash = PropertyHandlerCache.GetPropertyHash(property);
			PropertyHandler propertyHandler;
			PropertyHandler result;
			if (this.m_PropertyHandlers.TryGetValue(propertyHash, out propertyHandler))
			{
				result = propertyHandler;
			}
			else
			{
				result = null;
			}
			return result;
		}

		internal void SetHandler(SerializedProperty property, PropertyHandler handler)
		{
			int propertyHash = PropertyHandlerCache.GetPropertyHash(property);
			this.m_PropertyHandlers[propertyHash] = handler;
		}

		private static int GetPropertyHash(SerializedProperty property)
		{
			int result;
			if (property.serializedObject.targetObject == null)
			{
				result = 0;
			}
			else
			{
				int num = property.serializedObject.targetObject.GetInstanceID() ^ property.hashCodeForPropertyPathWithoutArrayIndex;
				if (property.propertyType == SerializedPropertyType.ObjectReference)
				{
					num ^= property.objectReferenceInstanceIDValue;
				}
				result = num;
			}
			return result;
		}

		public void Clear()
		{
			this.m_PropertyHandlers.Clear();
		}
	}
}
