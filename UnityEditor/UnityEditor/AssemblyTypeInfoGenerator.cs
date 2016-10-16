using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.SerializationLogic;
using UnityEngine;

namespace UnityEditor
{
	internal class AssemblyTypeInfoGenerator
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

		private class AssemblyResolver : BaseAssemblyResolver
		{
			private readonly IDictionary m_Assemblies;

			private AssemblyResolver() : this(new Hashtable())
			{
			}

			private AssemblyResolver(IDictionary assemblyCache)
			{
				this.m_Assemblies = assemblyCache;
			}

			public static IAssemblyResolver WithSearchDirs(params string[] searchDirs)
			{
				AssemblyTypeInfoGenerator.AssemblyResolver assemblyResolver = new AssemblyTypeInfoGenerator.AssemblyResolver();
				for (int i = 0; i < searchDirs.Length; i++)
				{
					string directory = searchDirs[i];
					assemblyResolver.AddSearchDirectory(directory);
				}
				return assemblyResolver;
			}

			public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
			{
				AssemblyDefinition assemblyDefinition = (AssemblyDefinition)this.m_Assemblies[name.Name];
				if (assemblyDefinition != null)
				{
					return assemblyDefinition;
				}
				assemblyDefinition = base.Resolve(name, parameters);
				this.m_Assemblies[name.Name] = assemblyDefinition;
				return assemblyDefinition;
			}
		}

		private AssemblyDefinition assembly_;

		private List<AssemblyTypeInfoGenerator.ClassInfo> classes_ = new List<AssemblyTypeInfoGenerator.ClassInfo>();

		private TypeResolver typeResolver = new TypeResolver(null);

		public AssemblyTypeInfoGenerator.ClassInfo[] ClassInfoArray
		{
			get
			{
				return this.classes_.ToArray();
			}
		}

		public AssemblyTypeInfoGenerator(string assembly, string[] searchDirs)
		{
			this.assembly_ = AssemblyDefinition.ReadAssembly(assembly, new ReaderParameters
			{
				AssemblyResolver = AssemblyTypeInfoGenerator.AssemblyResolver.WithSearchDirs(searchDirs)
			});
		}

		public AssemblyTypeInfoGenerator(string assembly, IAssemblyResolver resolver)
		{
			this.assembly_ = AssemblyDefinition.ReadAssembly(assembly, new ReaderParameters
			{
				AssemblyResolver = resolver
			});
		}

		private string GetMonoEmbeddedFullTypeNameFor(TypeReference type)
		{
			TypeSpecification typeSpecification = type as TypeSpecification;
			string fullName;
			if (typeSpecification != null && typeSpecification.IsRequiredModifier)
			{
				fullName = typeSpecification.ElementType.FullName;
			}
			else if (type.IsRequiredModifier)
			{
				fullName = type.GetElementType().FullName;
			}
			else
			{
				fullName = type.FullName;
			}
			return fullName.Replace('/', '+').Replace('<', '[').Replace('>', ']');
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
			if (this.classes_.Any((AssemblyTypeInfoGenerator.ClassInfo x) => x.name == this.GetMonoEmbeddedFullTypeNameFor(typeRef)))
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
			catch (NotSupportedException)
			{
				return;
			}
			if (typeDefinition == null)
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
				this.typeResolver.Add((GenericInstanceType)typeRef);
			}
			bool flag = false;
			try
			{
				flag = UnitySerializationLogic.ShouldImplementIDeserializable(typeDefinition);
			}
			catch
			{
			}
			if (!flag)
			{
				this.AddNestedTypes(typeDefinition, genericInstanceTypeMap);
			}
			else
			{
				AssemblyTypeInfoGenerator.ClassInfo item = default(AssemblyTypeInfoGenerator.ClassInfo);
				item.name = this.GetMonoEmbeddedFullTypeNameFor(typeRef);
				item.fields = this.GetFields(typeDefinition, typeRef.IsGenericInstance, genericInstanceTypeMap);
				this.classes_.Add(item);
				this.AddNestedTypes(typeDefinition, genericInstanceTypeMap);
				this.AddBaseType(typeRef, genericInstanceTypeMap);
			}
			if (typeRef.IsGenericInstance)
			{
				this.typeResolver.Remove((GenericInstanceType)typeRef);
			}
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
			foreach (TypeReference current in from x in arguments
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
			if (!this.WillSerialize(field))
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
			value.type = this.GetMonoEmbeddedFullTypeNameFor(type2);
			return new AssemblyTypeInfoGenerator.FieldInfo?(value);
		}

		private bool WillSerialize(FieldDefinition field)
		{
			bool result;
			try
			{
				result = UnitySerializationLogic.WillUnitySerialize(field, this.typeResolver);
			}
			catch (Exception ex)
			{
				Debug.LogFormat("Field '{0}' from '{1}', exception {2}", new object[]
				{
					field.FullName,
					field.Module.FullyQualifiedName,
					ex.Message
				});
				result = false;
			}
			return result;
		}

		public AssemblyTypeInfoGenerator.ClassInfo[] GatherClassInfo()
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
			return this.classes_.ToArray();
		}
	}
}
