using System;

namespace Unity.BindingsGenerator.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class NativeTypeAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public string Header
		{
			get;
			set;
		}

		public ScriptingObjectType ObjectType
		{
			get;
			set;
		}

		public NativeTypeAttribute()
		{
			this.ObjectType = ScriptingObjectType.UnityEngineObject;
		}
	}
}
