using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SerializedModule
	{
		protected string m_ModuleName;

		private SerializedObject m_Object;

		internal SerializedObject serializedObject
		{
			get
			{
				return this.m_Object;
			}
		}

		public SerializedModule(SerializedObject o, string name)
		{
			this.m_Object = o;
			this.m_ModuleName = name;
		}

		public SerializedProperty GetProperty0(string name)
		{
			SerializedProperty serializedProperty = this.m_Object.FindProperty(name);
			if (serializedProperty == null)
			{
				Debug.LogError("GetProperty0: not found: " + name);
			}
			return serializedProperty;
		}

		public SerializedProperty GetProperty(string name)
		{
			SerializedProperty serializedProperty = this.m_Object.FindProperty(SerializedModule.Concat(this.m_ModuleName, name));
			if (serializedProperty == null)
			{
				Debug.LogError("GetProperty: not found: " + SerializedModule.Concat(this.m_ModuleName, name));
			}
			return serializedProperty;
		}

		public SerializedProperty GetProperty0(string structName, string propName)
		{
			SerializedProperty serializedProperty = this.m_Object.FindProperty(SerializedModule.Concat(structName, propName));
			if (serializedProperty == null)
			{
				Debug.LogError("GetProperty: not found: " + SerializedModule.Concat(structName, propName));
			}
			return serializedProperty;
		}

		public SerializedProperty GetProperty(string structName, string propName)
		{
			SerializedProperty serializedProperty = this.m_Object.FindProperty(SerializedModule.Concat(SerializedModule.Concat(this.m_ModuleName, structName), propName));
			if (serializedProperty == null)
			{
				Debug.LogError("GetProperty: not found: " + SerializedModule.Concat(SerializedModule.Concat(this.m_ModuleName, structName), propName));
			}
			return serializedProperty;
		}

		public static string Concat(string a, string b)
		{
			return a + "." + b;
		}

		public string GetUniqueModuleName()
		{
			return SerializedModule.Concat(string.Empty + this.m_Object.targetObject.GetInstanceID(), this.m_ModuleName);
		}
	}
}
