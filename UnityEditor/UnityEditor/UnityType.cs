using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal sealed class UnityType
	{
		[UsedByNativeCode]
		private struct UnityTypeTransport
		{
			public uint runtimeTypeIndex;

			public uint descendantCount;

			public uint baseClassIndex;

			public string className;

			public string classNamespace;

			public int persistentTypeID;

			public uint flags;
		}

		private uint runtimeTypeIndex;

		private uint descendantCount;

		private static UnityType[] ms_types;

		private static ReadOnlyCollection<UnityType> ms_typesReadOnly;

		private static Dictionary<int, UnityType> ms_idToTypeInfo;

		private static Dictionary<string, UnityType> ms_nameToTypeInfo;

		public string name
		{
			get;
			private set;
		}

		public string nativeNamespace
		{
			get;
			private set;
		}

		public int persistentTypeID
		{
			get;
			private set;
		}

		public UnityType baseClass
		{
			get;
			private set;
		}

		public UnityTypeFlags flags
		{
			get;
			private set;
		}

		public bool isAbstract
		{
			get
			{
				return (this.flags & UnityTypeFlags.Abstract) != (UnityTypeFlags)0;
			}
		}

		public bool isSealed
		{
			get
			{
				return (this.flags & UnityTypeFlags.Sealed) != (UnityTypeFlags)0;
			}
		}

		public bool isEditorOnly
		{
			get
			{
				return (this.flags & UnityTypeFlags.EditorOnly) != (UnityTypeFlags)0;
			}
		}

		public bool isDeprecated
		{
			get
			{
				return (this.flags & UnityTypeFlags.Deprecated) != (UnityTypeFlags)0;
			}
		}

		public string qualifiedName
		{
			get
			{
				return (!this.hasNativeNamespace) ? this.name : (this.nativeNamespace + "::" + this.name);
			}
		}

		public bool hasNativeNamespace
		{
			get
			{
				return this.nativeNamespace.Length > 0;
			}
		}

		static UnityType()
		{
			UnityType.UnityTypeTransport[] array = UnityType.Internal_GetAllTypes();
			UnityType.ms_types = new UnityType[array.Length];
			UnityType.ms_idToTypeInfo = new Dictionary<int, UnityType>();
			UnityType.ms_nameToTypeInfo = new Dictionary<string, UnityType>();
			for (int i = 0; i < array.Length; i++)
			{
				UnityType baseClass = null;
				if ((ulong)array[i].baseClassIndex < (ulong)((long)array.Length))
				{
					baseClass = UnityType.ms_types[(int)((UIntPtr)array[i].baseClassIndex)];
				}
				UnityType unityType = new UnityType
				{
					runtimeTypeIndex = array[i].runtimeTypeIndex,
					descendantCount = array[i].descendantCount,
					name = array[i].className,
					nativeNamespace = array[i].classNamespace,
					persistentTypeID = array[i].persistentTypeID,
					baseClass = baseClass,
					flags = (UnityTypeFlags)array[i].flags
				};
				UnityType.ms_types[i] = unityType;
				UnityType.ms_typesReadOnly = new ReadOnlyCollection<UnityType>(UnityType.ms_types);
				UnityType.ms_idToTypeInfo[unityType.persistentTypeID] = unityType;
				UnityType.ms_nameToTypeInfo[unityType.name] = unityType;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityType.UnityTypeTransport[] Internal_GetAllTypes();

		public bool IsDerivedFrom(UnityType baseClass)
		{
			return this.runtimeTypeIndex - baseClass.runtimeTypeIndex < baseClass.descendantCount;
		}

		public static UnityType FindTypeByPersistentTypeID(int id)
		{
			UnityType result = null;
			UnityType.ms_idToTypeInfo.TryGetValue(id, out result);
			return result;
		}

		public static UnityType FindTypeByName(string name)
		{
			UnityType result = null;
			UnityType.ms_nameToTypeInfo.TryGetValue(name, out result);
			return result;
		}

		public static UnityType FindTypeByNameCaseInsensitive(string name)
		{
			return UnityType.ms_types.FirstOrDefault((UnityType t) => string.Equals(name, t.name, StringComparison.OrdinalIgnoreCase));
		}

		public static ReadOnlyCollection<UnityType> GetTypes()
		{
			return UnityType.ms_typesReadOnly;
		}
	}
}
