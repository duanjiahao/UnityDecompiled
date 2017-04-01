using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Utils;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class MonoAOTRegistration
	{
		private static void ExtractNativeMethodsFromTypes(ICollection<TypeDefinition> types, ArrayList res)
		{
			foreach (TypeDefinition current in types)
			{
				foreach (MethodDefinition current2 in current.Methods)
				{
					if (current2.IsStatic && current2.IsPInvokeImpl && current2.PInvokeInfo.Module.Name.Equals("__Internal"))
					{
						if (res.Contains(current2.Name))
						{
							throw new SystemException("Duplicate native method found : " + current2.Name + ". Please check your source carefully.");
						}
						res.Add(current2.Name);
					}
				}
				if (current.HasNestedTypes)
				{
					MonoAOTRegistration.ExtractNativeMethodsFromTypes(current.NestedTypes, res);
				}
			}
		}

		private static ArrayList BuildNativeMethodList(AssemblyDefinition[] assemblies)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < assemblies.Length; i++)
			{
				AssemblyDefinition assemblyDefinition = assemblies[i];
				if (!"System".Equals(assemblyDefinition.Name.Name))
				{
					MonoAOTRegistration.ExtractNativeMethodsFromTypes(assemblyDefinition.MainModule.Types, arrayList);
				}
			}
			return arrayList;
		}

		public static HashSet<string> BuildReferencedTypeList(AssemblyDefinition[] assemblies)
		{
			HashSet<string> hashSet = new HashSet<string>();
			for (int i = 0; i < assemblies.Length; i++)
			{
				AssemblyDefinition assemblyDefinition = assemblies[i];
				if (!assemblyDefinition.Name.Name.StartsWith("System") && !assemblyDefinition.Name.Name.Equals("UnityEngine"))
				{
					foreach (TypeReference current in assemblyDefinition.MainModule.GetTypeReferences())
					{
						hashSet.Add(current.FullName);
					}
				}
			}
			return hashSet;
		}

		public static void WriteCPlusPlusFileForStaticAOTModuleRegistration(BuildTarget buildTarget, string file, CrossCompileOptions crossCompileOptions, bool advancedLic, string targetDevice, bool stripping, RuntimeClassRegistry usedClassRegistry, AssemblyReferenceChecker checker, string stagingAreaDataManaged)
		{
			MonoAOTRegistration.WriteCPlusPlusFileForStaticAOTModuleRegistration(buildTarget, file, crossCompileOptions, advancedLic, targetDevice, stripping, usedClassRegistry, checker, stagingAreaDataManaged, null);
		}

		public static void WriteCPlusPlusFileForStaticAOTModuleRegistration(BuildTarget buildTarget, string file, CrossCompileOptions crossCompileOptions, bool advancedLic, string targetDevice, bool stripping, RuntimeClassRegistry usedClassRegistry, AssemblyReferenceChecker checker, string stagingAreaDataManaged, IIl2CppPlatformProvider platformProvider)
		{
			string text = Path.Combine(stagingAreaDataManaged, "ICallSummary.txt");
			string exe = Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/InternalCallRegistrationWriter/InternalCallRegistrationWriter.exe");
			string args = string.Format("-assembly=\"{0}\" -summary=\"{1}\"", Path.Combine(stagingAreaDataManaged, "UnityEngine.dll"), text);
			Runner.RunManagedProgram(exe, args);
			HashSet<UnityType> hashSet;
			HashSet<string> nativeModules;
			CodeStrippingUtils.GenerateDependencies(Path.GetDirectoryName(stagingAreaDataManaged), text, usedClassRegistry, stripping, out hashSet, out nativeModules, platformProvider);
			using (TextWriter textWriter = new StreamWriter(file))
			{
				string[] assemblyFileNames = checker.GetAssemblyFileNames();
				AssemblyDefinition[] assemblyDefinitions = checker.GetAssemblyDefinitions();
				bool flag = (crossCompileOptions & CrossCompileOptions.FastICall) != CrossCompileOptions.Dynamic;
				ArrayList arrayList = MonoAOTRegistration.BuildNativeMethodList(assemblyDefinitions);
				if (buildTarget == BuildTarget.iOS)
				{
					textWriter.WriteLine("#include \"RegisterMonoModules.h\"");
					textWriter.WriteLine("#include <stdio.h>");
				}
				textWriter.WriteLine("");
				textWriter.WriteLine("#if defined(TARGET_IPHONE_SIMULATOR) && TARGET_IPHONE_SIMULATOR");
				textWriter.WriteLine("    #define DECL_USER_FUNC(f) void f() __attribute__((weak_import))");
				textWriter.WriteLine("    #define REGISTER_USER_FUNC(f)\\");
				textWriter.WriteLine("        do {\\");
				textWriter.WriteLine("        if(f != NULL)\\");
				textWriter.WriteLine("            mono_dl_register_symbol(#f, (void*)f);\\");
				textWriter.WriteLine("        else\\");
				textWriter.WriteLine("            ::printf_console(\"Symbol '%s' not found. Maybe missing implementation for Simulator?\\n\", #f);\\");
				textWriter.WriteLine("        }while(0)");
				textWriter.WriteLine("#else");
				textWriter.WriteLine("    #define DECL_USER_FUNC(f) void f() ");
				textWriter.WriteLine("    #if !defined(__arm64__)");
				textWriter.WriteLine("    #define REGISTER_USER_FUNC(f) mono_dl_register_symbol(#f, (void*)&f)");
				textWriter.WriteLine("    #else");
				textWriter.WriteLine("        #define REGISTER_USER_FUNC(f)");
				textWriter.WriteLine("    #endif");
				textWriter.WriteLine("#endif");
				textWriter.WriteLine("extern \"C\"\n{");
				textWriter.WriteLine("    typedef void* gpointer;");
				textWriter.WriteLine("    typedef int gboolean;");
				if (buildTarget == BuildTarget.iOS)
				{
					textWriter.WriteLine("    const char*         UnityIPhoneRuntimeVersion = \"{0}\";", Application.unityVersion);
					textWriter.WriteLine("    void                mono_dl_register_symbol (const char* name, void *addr);");
					textWriter.WriteLine("#if !defined(__arm64__)");
					textWriter.WriteLine("    extern int          mono_ficall_flag;");
					textWriter.WriteLine("#endif");
				}
				textWriter.WriteLine("    void                mono_aot_register_module(gpointer *aot_info);");
				textWriter.WriteLine("#if __ORBIS__ || SN_TARGET_PSP2");
				textWriter.WriteLine("#define DLL_EXPORT __declspec(dllexport)");
				textWriter.WriteLine("#else");
				textWriter.WriteLine("#define DLL_EXPORT");
				textWriter.WriteLine("#endif");
				textWriter.WriteLine("#if !(TARGET_IPHONE_SIMULATOR)");
				textWriter.WriteLine("    extern gboolean     mono_aot_only;");
				for (int i = 0; i < assemblyFileNames.Length; i++)
				{
					string arg = assemblyFileNames[i];
					string text2 = assemblyDefinitions[i].Name.Name;
					text2 = text2.Replace(".", "_");
					text2 = text2.Replace("-", "_");
					text2 = text2.Replace(" ", "_");
					textWriter.WriteLine("    extern gpointer*    mono_aot_module_{0}_info; // {1}", text2, arg);
				}
				textWriter.WriteLine("#endif // !(TARGET_IPHONE_SIMULATOR)");
				IEnumerator enumerator = arrayList.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string arg2 = (string)enumerator.Current;
						textWriter.WriteLine("    DECL_USER_FUNC({0});", arg2);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				textWriter.WriteLine("}");
				textWriter.WriteLine("DLL_EXPORT void RegisterMonoModules()");
				textWriter.WriteLine("{");
				textWriter.WriteLine("#if !(TARGET_IPHONE_SIMULATOR) && !defined(__arm64__)");
				textWriter.WriteLine("    mono_aot_only = true;");
				if (buildTarget == BuildTarget.iOS)
				{
					textWriter.WriteLine("    mono_ficall_flag = {0};", (!flag) ? "false" : "true");
				}
				AssemblyDefinition[] array = assemblyDefinitions;
				for (int j = 0; j < array.Length; j++)
				{
					AssemblyDefinition assemblyDefinition = array[j];
					string text3 = assemblyDefinition.Name.Name;
					text3 = text3.Replace(".", "_");
					text3 = text3.Replace("-", "_");
					text3 = text3.Replace(" ", "_");
					textWriter.WriteLine("    mono_aot_register_module(mono_aot_module_{0}_info);", text3);
				}
				textWriter.WriteLine("#endif // !(TARGET_IPHONE_SIMULATOR) && !defined(__arm64__)");
				textWriter.WriteLine("");
				if (buildTarget == BuildTarget.iOS)
				{
					IEnumerator enumerator2 = arrayList.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							string arg3 = (string)enumerator2.Current;
							textWriter.WriteLine("    REGISTER_USER_FUNC({0});", arg3);
						}
					}
					finally
					{
						IDisposable disposable2;
						if ((disposable2 = (enumerator2 as IDisposable)) != null)
						{
							disposable2.Dispose();
						}
					}
				}
				textWriter.WriteLine("}");
				textWriter.WriteLine("");
				AssemblyDefinition assemblyDefinition2 = null;
				for (int k = 0; k < assemblyFileNames.Length; k++)
				{
					if (assemblyFileNames[k] == "UnityEngine.dll")
					{
						assemblyDefinition2 = assemblyDefinitions[k];
					}
				}
				if (buildTarget == BuildTarget.iOS)
				{
					AssemblyDefinition[] assemblies = new AssemblyDefinition[]
					{
						assemblyDefinition2
					};
					MonoAOTRegistration.GenerateRegisterInternalCalls(assemblies, textWriter);
					MonoAOTRegistration.ResolveDefinedNativeClassesFromMono(assemblies, usedClassRegistry);
					MonoAOTRegistration.ResolveReferencedUnityEngineClassesFromMono(assemblyDefinitions, assemblyDefinition2, usedClassRegistry);
					MonoAOTRegistration.GenerateRegisterModules(hashSet, nativeModules, textWriter, stripping);
					if (stripping && usedClassRegistry != null)
					{
						MonoAOTRegistration.GenerateRegisterClassesForStripping(hashSet, textWriter);
					}
					else
					{
						MonoAOTRegistration.GenerateRegisterClasses(hashSet, textWriter);
					}
				}
				textWriter.Close();
			}
		}

		public static void ResolveReferencedUnityEngineClassesFromMono(AssemblyDefinition[] assemblies, AssemblyDefinition unityEngine, RuntimeClassRegistry res)
		{
			if (res != null)
			{
				for (int i = 0; i < assemblies.Length; i++)
				{
					AssemblyDefinition assemblyDefinition = assemblies[i];
					if (assemblyDefinition != unityEngine)
					{
						foreach (TypeReference current in assemblyDefinition.MainModule.GetTypeReferences())
						{
							if (current.Namespace.StartsWith("UnityEngine"))
							{
								string name = current.Name;
								res.AddMonoClass(name);
							}
						}
					}
				}
			}
		}

		public static void ResolveDefinedNativeClassesFromMono(AssemblyDefinition[] assemblies, RuntimeClassRegistry res)
		{
			if (res != null)
			{
				for (int i = 0; i < assemblies.Length; i++)
				{
					AssemblyDefinition assemblyDefinition = assemblies[i];
					foreach (TypeDefinition current in assemblyDefinition.MainModule.Types)
					{
						if (current.Fields.Count > 0 || current.Methods.Count > 0 || current.Properties.Count > 0)
						{
							string name = current.Name;
							res.AddMonoClass(name);
						}
					}
				}
			}
		}

		public static void GenerateRegisterModules(HashSet<UnityType> nativeClasses, HashSet<string> nativeModules, TextWriter output, bool strippingEnabled)
		{
			output.WriteLine("void InvokeRegisterStaticallyLinkedModuleClasses()");
			output.WriteLine("{");
			if (nativeClasses == null)
			{
				output.WriteLine("\tvoid RegisterStaticallyLinkedModuleClasses();");
				output.WriteLine("\tRegisterStaticallyLinkedModuleClasses();");
			}
			else
			{
				output.WriteLine("\t// Do nothing (we're in stripping mode)");
			}
			output.WriteLine("}");
			output.WriteLine();
			output.WriteLine("void RegisterStaticallyLinkedModulesGranular()");
			output.WriteLine("{");
			foreach (string current in nativeModules)
			{
				output.WriteLine("\tvoid RegisterModule_" + current + "();");
				output.WriteLine("\tRegisterModule_" + current + "();");
				output.WriteLine();
			}
			output.WriteLine("}\n");
		}

		public static void GenerateRegisterClassesForStripping(HashSet<UnityType> nativeClassesAndBaseClasses, TextWriter output)
		{
			output.WriteLine("template <typename T> void RegisterClass();");
			output.WriteLine("template <typename T> void RegisterStrippedTypeInfo(int, const char*, const char*);");
			output.WriteLine();
			foreach (UnityType current in UnityType.GetTypes())
			{
				if (current.baseClass != null && !current.isEditorOnly)
				{
					if (!current.hasNativeNamespace)
					{
						output.WriteLine("class {0};", current.name);
					}
					else
					{
						output.WriteLine("namespace {0} {{ class {1}; }}", current.nativeNamespace, current.name);
					}
					output.WriteLine();
				}
			}
			output.Write("void RegisterAllClasses() \n{\n");
			output.WriteLine("\tvoid RegisterBuiltinTypes();");
			output.WriteLine("\tRegisterBuiltinTypes();");
			output.WriteLine("\t// {0} Non stripped classes\n", nativeClassesAndBaseClasses.Count);
			int num = 1;
			foreach (UnityType current2 in UnityType.GetTypes())
			{
				if (current2.baseClass != null && !current2.isEditorOnly && nativeClassesAndBaseClasses.Contains(current2))
				{
					output.WriteLine("\t// {0}. {1}", num++, current2.qualifiedName);
					output.WriteLine("\tRegisterClass<{0}>();\n", current2.qualifiedName);
				}
			}
			output.WriteLine();
			output.Write("\n}\n");
		}

		public static void GenerateRegisterClasses(HashSet<UnityType> allClasses, TextWriter output)
		{
			output.WriteLine("void RegisterAllClasses() \n{");
			output.WriteLine("\tvoid RegisterAllClassesGranular();");
			output.WriteLine("\tRegisterAllClassesGranular();");
			output.WriteLine("}");
		}

		public static void GenerateRegisterInternalCalls(AssemblyDefinition[] assemblies, TextWriter output)
		{
			output.Write("void RegisterAllStrippedInternalCalls ()\n{\n");
			for (int i = 0; i < assemblies.Length; i++)
			{
				AssemblyDefinition assemblyDefinition = assemblies[i];
				MonoAOTRegistration.GenerateRegisterInternalCallsForTypes(assemblyDefinition.MainModule.Types, output);
			}
			output.Write("}\n\n");
		}

		private static void GenerateRegisterInternalCallsForTypes(IEnumerable<TypeDefinition> types, TextWriter output)
		{
			foreach (TypeDefinition current in types)
			{
				foreach (MethodDefinition current2 in current.Methods)
				{
					MonoAOTRegistration.GenerateInternalCallMethod(current, current2, output);
				}
				MonoAOTRegistration.GenerateRegisterInternalCallsForTypes(current.NestedTypes, output);
			}
		}

		private static void GenerateInternalCallMethod(TypeDefinition typeDefinition, MethodDefinition method, TextWriter output)
		{
			if (method.IsInternalCall)
			{
				string text = typeDefinition.FullName + "_" + method.Name;
				text = text.Replace('/', '_');
				text = text.Replace('.', '_');
				if (!text.Contains("UnityEngine_Serialization"))
				{
					output.WriteLine("\tvoid Register_{0} ();", text);
					output.WriteLine("\tRegister_{0} ();", text);
				}
			}
		}
	}
}
