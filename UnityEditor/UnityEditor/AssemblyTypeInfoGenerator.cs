using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
namespace UnityEditor
{
	public class AssemblyTypeInfoGenerator
	{
		public struct FieldInfo
		{
			public string name;
			public string type;
		}
		public struct ClassInfo
		{
			public string name;
			public AssemblyTypeInfoGenerator.FieldInfo[] fields;
		}
		private AssemblyDefinition assembly_;
		private List<AssemblyTypeInfoGenerator.ClassInfo> classes_ = new List<AssemblyTypeInfoGenerator.ClassInfo>();
		public AssemblyTypeInfoGenerator.ClassInfo[] ClassInfoArray
		{
			get
			{
				return this.classes_.ToArray();
			}
		}
		public AssemblyTypeInfoGenerator(string assembly)
		{
			this.assembly_ = AssemblyDefinition.ReadAssembly(assembly);
		}
		private string GetFullTypeName(TypeReference type)
		{
			return type.FullName.Replace('/', '+').Replace('<', '[').Replace('>', ']');
		}
		private TypeReference ResolveGenericInstanceType(TypeReference typeToResolve, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			ArrayType arrayType = typeToResolve as ArrayType;
			if (arrayType != null)
			{
				typeToResolve = new ArrayType(this.ResolveGenericInstanceType(arrayType.ElementType, genericInstanceTypeMap), arrayType.Rank);
			}
			while (genericInstanceTypeMap.ContainsKey(typeToResolve))
			{
				typeToResolve = genericInstanceTypeMap[typeToResolve];
			}
			if (typeToResolve.IsGenericInstance)
			{
				GenericInstanceType genericInstanceType = (GenericInstanceType)typeToResolve;
				typeToResolve = this.MakeGenericInstance(genericInstanceType.ElementType, genericInstanceType.GenericArguments, genericInstanceTypeMap);
			}
			return typeToResolve;
		}
		private void AddType(TypeReference typeRef, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			if (this.classes_.Any((AssemblyTypeInfoGenerator.ClassInfo x) => x.name == this.GetFullTypeName(typeRef)))
			{
				return;
			}
			TypeDefinition typeDefinition;
			try
			{
				typeDefinition = typeRef.Resolve();
			}
			catch (AssemblyResolutionException)
			{
				return;
			}
			if (typeRef.IsGenericInstance)
			{
				Collection<TypeReference> genericArguments = ((GenericInstanceType)typeRef).GenericArguments;
				Collection<GenericParameter> genericParameters = typeDefinition.GenericParameters;
				for (int i = 0; i < genericArguments.Count; i++)
				{
					if (genericParameters[i] != genericArguments[i])
					{
						genericInstanceTypeMap[genericParameters[i]] = genericArguments[i];
					}
				}
			}
			AssemblyTypeInfoGenerator.ClassInfo item = default(AssemblyTypeInfoGenerator.ClassInfo);
			item.name = this.GetFullTypeName(typeRef);
			item.fields = this.GetFields(typeDefinition, typeRef.IsGenericInstance, genericInstanceTypeMap);
			this.classes_.Add(item);
			this.AddNestedTypes(typeDefinition, genericInstanceTypeMap);
			this.AddBaseType(typeRef, genericInstanceTypeMap);
		}
		private void AddNestedTypes(TypeDefinition type, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			foreach (TypeDefinition current in type.NestedTypes)
			{
				this.AddType(current, genericInstanceTypeMap);
			}
		}
		private void AddBaseType(TypeReference typeRef, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			TypeReference typeReference = typeRef.Resolve().BaseType;
			if (typeReference != null)
			{
				if (typeRef.IsGenericInstance && typeReference.IsGenericInstance)
				{
					GenericInstanceType genericInstanceType = (GenericInstanceType)typeReference;
					typeReference = this.MakeGenericInstance(genericInstanceType.ElementType, genericInstanceType.GenericArguments, genericInstanceTypeMap);
				}
				this.AddType(typeReference, genericInstanceTypeMap);
			}
		}
		private TypeReference MakeGenericInstance(TypeReference genericClass, IEnumerable<TypeReference> arguments, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			GenericInstanceType genericInstanceType = new GenericInstanceType(genericClass);
			foreach (TypeReference current in 
				from x in arguments
				select this.ResolveGenericInstanceType(x, genericInstanceTypeMap))
			{
				genericInstanceType.GenericArguments.Add(current);
			}
			return genericInstanceType;
		}
		private AssemblyTypeInfoGenerator.FieldInfo[] GetFields(TypeDefinition type, bool isGenericInstance, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			List<AssemblyTypeInfoGenerator.FieldInfo> list = new List<AssemblyTypeInfoGenerator.FieldInfo>();
			foreach (FieldDefinition current in type.Fields)
			{
				AssemblyTypeInfoGenerator.FieldInfo? fieldInfo = this.GetFieldInfo(type, current, isGenericInstance, genericInstanceTypeMap);
				if (fieldInfo.HasValue)
				{
					list.Add(fieldInfo.Value);
				}
			}
			return list.ToArray();
		}
		private AssemblyTypeInfoGenerator.FieldInfo? GetFieldInfo(TypeDefinition type, FieldDefinition field, bool isDeclaringTypeGenericInstance, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			if (field.IsStatic)
			{
				return null;
			}
			if (field.IsInitOnly)
			{
				return null;
			}
			if ((ushort)(field.Attributes & FieldAttributes.NotSerialized) != 0)
			{
				return null;
			}
			bool flag = field.CustomAttributes.Any((CustomAttribute ca) => ca.AttributeType.Name == "SerializeField");
			if (!field.IsPublic && !flag)
			{
				return null;
			}
			AssemblyTypeInfoGenerator.FieldInfo value = default(AssemblyTypeInfoGenerator.FieldInfo);
			value.name = field.Name;
			TypeReference type2;
			if (isDeclaringTypeGenericInstance)
			{
				type2 = this.ResolveGenericInstanceType(field.FieldType, genericInstanceTypeMap);
			}
			else
			{
				type2 = field.FieldType;
			}
			value.type = this.GetFullTypeName(type2);
			return new AssemblyTypeInfoGenerator.FieldInfo?(value);
		}
		public void GatherClassInfo()
		{
			foreach (ModuleDefinition current in this.assembly_.Modules)
			{
				foreach (TypeDefinition current2 in current.Types)
				{
					if (!(current2.Name == "<Module>"))
					{
						this.AddType(current2, new Dictionary<TypeReference, TypeReference>());
					}
				}
			}
		}
	}
}
