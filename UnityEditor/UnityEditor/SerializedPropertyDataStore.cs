using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityEditor
{
	internal class SerializedPropertyDataStore
	{
		internal class Data
		{
			private UnityEngine.Object m_Object;

			private SerializedObject m_SerializedObject;

			private SerializedProperty[] m_Properties;

			public string name
			{
				get
				{
					return (!(this.m_Object != null)) ? string.Empty : this.m_Object.name;
				}
			}

			public SerializedObject serializedObject
			{
				get
				{
					return this.m_SerializedObject;
				}
			}

			public SerializedProperty[] properties
			{
				get
				{
					return this.m_Properties;
				}
			}

			public int objectId
			{
				get
				{
					int result;
					if (!this.m_Object)
					{
						result = 0;
					}
					else
					{
						Component component = this.m_Object as Component;
						result = ((!(component != null)) ? this.m_Object.GetInstanceID() : component.gameObject.GetInstanceID());
					}
					return result;
				}
			}

			public Data(UnityEngine.Object obj, string[] props)
			{
				this.m_Object = obj;
				this.m_SerializedObject = new SerializedObject(obj);
				this.m_Properties = new SerializedProperty[props.Length];
				for (int i = 0; i < props.Length; i++)
				{
					this.m_Properties[i] = this.m_SerializedObject.FindProperty(props[i]);
				}
			}

			public void Dispose()
			{
				SerializedProperty[] properties = this.m_Properties;
				for (int i = 0; i < properties.Length; i++)
				{
					SerializedProperty serializedProperty = properties[i];
					if (serializedProperty != null)
					{
						serializedProperty.Dispose();
					}
				}
				this.m_SerializedObject.Dispose();
				this.m_Object = null;
				this.m_SerializedObject = null;
				this.m_Properties = null;
			}

			public bool Update()
			{
				return this.m_Object != null && this.m_SerializedObject.UpdateIfRequiredOrScript();
			}

			public void Store()
			{
				if (this.m_Object != null)
				{
					this.m_SerializedObject.ApplyModifiedProperties();
				}
			}
		}

		internal delegate UnityEngine.Object[] GatherDelegate();

		private UnityEngine.Object[] m_Objects;

		private SerializedPropertyDataStore.Data[] m_Elements;

		private string[] m_PropNames;

		private SerializedPropertyDataStore.GatherDelegate m_GatherDel;

		public SerializedPropertyDataStore(string[] propNames, SerializedPropertyDataStore.GatherDelegate gatherDel)
		{
			this.m_PropNames = propNames;
			this.m_GatherDel = gatherDel;
			this.Repopulate();
		}

		public SerializedPropertyDataStore.Data[] GetElements()
		{
			return this.m_Elements;
		}

		~SerializedPropertyDataStore()
		{
			this.Clear();
		}

		public bool Repopulate()
		{
			Profiler.BeginSample("SerializedPropertyDataStore.Repopulate.GatherDelegate");
			UnityEngine.Object[] array = this.m_GatherDel();
			Profiler.EndSample();
			bool result;
			if (this.m_Objects != null)
			{
				if (array.Length == this.m_Objects.Length && ArrayUtility.ArrayReferenceEquals<UnityEngine.Object>(array, this.m_Objects))
				{
					result = false;
					return result;
				}
				this.Clear();
			}
			this.m_Objects = array;
			this.m_Elements = new SerializedPropertyDataStore.Data[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				this.m_Elements[i] = new SerializedPropertyDataStore.Data(array[i], this.m_PropNames);
			}
			result = true;
			return result;
		}

		private void Clear()
		{
			for (int i = 0; i < this.m_Elements.Length; i++)
			{
				this.m_Elements[i].Dispose();
			}
			this.m_Objects = null;
			this.m_Elements = null;
		}
	}
}
