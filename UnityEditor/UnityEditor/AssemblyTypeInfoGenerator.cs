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
					string text = searchDirs[i];
					assemblyResolver.AddSearchDirectory(text);
				}
				assemblyResolver.RemoveSearchDirectory(".");
				assemblyResolver.RemoveSearchDirectory("bin");
				return assemblyResolver;
			}

			public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
			{
				AssemblyDefinition assemblyDefinition = (AssemblyDefinition)this.m_Assemblies[name.get_Name()];
				AssemblyDefinition result;
				if (assemblyDefinition != null)
				{
					result = assemblyDefinition;
				}
				else
				{
					assemblyDefinition = base.Resolve(name, parameters);
					this.m_Assemblies[name.get_Name()] = assemblyDefinition;
					result = assemblyDefinition;
				}
				return result;
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
			ReaderParameters readerParameters = new ReaderParameters();
			readerParameters.set_AssemblyResolver(AssemblyTypeInfoGenerator.AssemblyResolver.WithSearchDirs(searchDirs));
			this.assembly_ = AssemblyDefinition.ReadAssembly(assembly, readerParameters);
		}

		public AssemblyTypeInfoGenerator(string assembly, IAssemblyResolver resolver)
		{
			ReaderParameters readerParameters = new ReaderParameters();
			readerParameters.set_AssemblyResolver(resolver);
			this.assembly_ = AssemblyDefinition.ReadAssembly(assembly, readerParameters);
		}

		private string GetMonoEmbeddedFullTypeNameFor(TypeReference type)
		{
			TypeSpecification typeSpecification = type as TypeSpecification;
			string fullName;
			if (typeSpecification != null && typeSpecification.get_IsRequiredModifier())
			{
				fullName = typeSpecification.get_ElementType().get_FullName();
			}
			else if (type.get_IsRequiredModifier())
			{
				fullName = type.GetElementType().get_FullName();
			}
			else
			{
				fullName = type.get_FullName();
			}
			return fullName.Replace('/', '+').Replace('<', '[').Replace('>', ']');
		}

		private TypeReference ResolveGenericInstanceType(TypeReference typeToResolve, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			ArrayType arrayType = typeToResolve as ArrayType;
			if (arrayType != null)
			{
				typeToResolve = new ArrayType(this.ResolveGenericInstanceType(arrayType.get_ElementType(), genericInstanceTypeMap), arrayType.get_Rank());
			}
			while (genericInstanceTypeMap.ContainsKey(typeToResolve))
			{
				typeToResolve = genericInstanceTypeMap[typeToResolve];
			}
			if (typeToResolve.get_IsGenericInstance())
			{
				GenericInstanceType genericInstanceType = (GenericInstanceType)typeToResolve;
				typeToResolve = this.MakeGenericInstance(genericInstanceType.get_ElementType(), genericInstanceType.get_GenericArguments(), genericInstanceTypeMap);
			}
			return typeToResolve;
		}

		private void AddType(TypeReference typeRef, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			if (!this.classes_.Any((AssemblyTypeInfoGenerator.ClassInfo x) => x.name == this.GetMonoEmbeddedFullTypeNameFor(typeRef)))
			{
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
				if (typeDefinition != null)
				{
					if (typeRef.get_IsGenericInstance())
					{
						Collection<TypeReference> genericArguments = ((GenericInstanceType)typeRef).get_GenericArguments();
						Collection<GenericParameter> genericParameters = typeDefinition.get_GenericParameters();
						for (int i = 0; i < genericArguments.get_Count(); i++)
						{
							if (genericParameters.get_Item(i) != genericArguments.get_Item(i))
							{
								genericInstanceTypeMap[genericParameters.get_Item(i)] = genericArguments.get_Item(i);
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
						item.fields = this.GetFields(typeDefinition, typeRef.get_IsGenericInstance(), genericInstanceTypeMap);
						this.classes_.Add(item);
						this.AddNestedTypes(typeDefinition, genericInstanceTypeMap);
						this.AddBaseType(typeRef, genericInstanceTypeMap);
					}
					if (typeRef.get_IsGenericInstance())
					{
						this.typeResolver.Remove((GenericInstanceType)typeRef);
					}
				}
			}
		}

		private void AddNestedTypes(TypeDefinition type, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			using (Collection<TypeDefinition>.Enumerator enumerator = type.get_NestedTypes().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TypeDefinition current = enumerator.get_Current();
					this.AddType(current, genericInstanceTypeMap);
				}
			}
		}

		private void AddBaseType(TypeReference typeRef, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			TypeReference typeReference = typeRef.Resolve().get_BaseType();
			if (typeReference != null)
			{
				if (typeRef.get_IsGenericInstance() && typeReference.get_IsGenericInstance())
				{
					GenericInstanceType genericInstanceType = (GenericInstanceType)typeReference;
					typeReference = this.MakeGenericInstance(genericInstanceType.get_ElementType(), genericInstanceType.get_GenericArguments(), genericInstanceTypeMap);
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
				genericInstanceType.get_GenericArguments().Add(current);
			}
			return genericInstanceType;
		}

		private AssemblyTypeInfoGenerator.FieldInfo[] GetFields(TypeDefinition type, bool isGenericInstance, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			List<AssemblyTypeInfoGenerator.FieldInfo> list = new List<AssemblyTypeInfoGenerator.FieldInfo>();
			using (Collection<FieldDefinition>.Enumerator enumerator = type.get_Fields().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FieldDefinition current = enumerator.get_Current();
					AssemblyTypeInfoGenerator.FieldInfo? fieldInfo = this.GetFieldInfo(type, current, isGenericInstance, genericInstanceTypeMap);
					if (fieldInfo.HasValue)
					{
						list.Add(fieldInfo.Value);
					}
				}
			}
			return list.ToArray();
		}

		private AssemblyTypeInfoGenerator.FieldInfo? GetFieldInfo(TypeDefinition type, FieldDefinition field, bool isDeclaringTypeGenericInstance, Dictionary<TypeReference, TypeReference> genericInstanceTypeMap)
		{
			AssemblyTypeInfoGenerator.FieldInfo? result;
			if (!this.WillSerialize(field))
			{
				result = null;
			}
			else
			{
				AssemblyTypeInfoGenerator.FieldInfo value = default(AssemblyTypeInfoGenerator.FieldInfo);
				value.name = field.get_Name();
				TypeReference type2;
				if (isDeclaringTypeGenericInstance)
				{
					type2 = this.ResolveGenericInstanceType(field.get_FieldType(), genericInstanceTypeMap);
				}
				else
				{
					type2 = field.get_FieldType();
				}
				value.type = this.GetMonoEmbeddedFullTypeNameFor(type2);
				result = new AssemblyTypeInfoGenerator.FieldInfo?(value);
			}
			return result;
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
					field.get_FullName(),
					field.get_Module().get_FullyQualifiedName(),
					ex.Message
				});
				result = false;
			}
			return result;
		}

		public AssemblyTypeInfoGenerator.ClassInfo[] GatherClassInfo()
		{
			using (Collection<ModuleDefinition>.Enumerator enumerator = this.assembly_.get_Modules().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ModuleDefinition current = enumerator.get_Current();
					using (Collection<TypeDefinition>.Enumerator enumerator2 = current.get_Types().GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							TypeDefinition current2 = enumerator2.get_Current();
							if (!(current2.get_Name() == "<Module>"))
							{
								this.AddType(current2, new Dictionary<TypeReference, TypeReference>());
							}
						}
					}
				}
			}
			return this.classes_.ToArray();
		}
	}
}
