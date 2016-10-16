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
			PropertyHandler result;
			if (this.m_PropertyHandlers.TryGetValue(propertyHash, out result))
			{
				return result;
			}
			return null;
		}

		internal void SetHandler(SerializedProperty property, PropertyHandler handler)
		{
			int propertyHash = PropertyHandlerCache.GetPropertyHash(property);
			this.m_PropertyHandlers[propertyHash] = handler;
		}

		private static int GetPropertyHash(SerializedProperty property)
		{
			if (property.serializedObject.targetObject == null)
			{
				return 0;
			}
			int num = property.serializedObject.targetObject.GetInstanceID() ^ property.hashCodeForPropertyPathWithoutArrayIndex;
			if (property.propertyType == SerializedPropertyType.ObjectReference)
			{
				num ^= property.objectReferenceInstanceIDValue;
			}
			return num;
		}

		public void Clear()
		{
			this.m_PropertyHandlers.Clear();
		}
	}
}
