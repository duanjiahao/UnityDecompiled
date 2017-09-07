using System;
using System.Runtime.CompilerServices;

namespace UnityEditor.Experimental.Build.AssetBundle
{
	public class BuildInterface
	{
		public static BuildInput GenerateBuildInput()
		{
			BuildInput result;
			BuildInterface.GenerateBuildInput_Injected(out result);
			return result;
		}

		public static ObjectIdentifier[] GetPlayerObjectIdentifiersInAsset(GUID asset, BuildTarget target)
		{
			return BuildInterface.GetPlayerObjectIdentifiersInAsset_Injected(ref asset, target);
		}

		public static ObjectIdentifier[] GetPlayerDependenciesForObject(ObjectIdentifier objectID, BuildTarget target)
		{
			return BuildInterface.GetPlayerDependenciesForObject_Injected(ref objectID, target);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ObjectIdentifier[] GetPlayerDependenciesForObjects(ObjectIdentifier[] objectIDs, BuildTarget target);

		public static Type GetTypeForObject(ObjectIdentifier objectID)
		{
			return BuildInterface.GetTypeForObject_Injected(ref objectID);
		}

		public static BuildOutput WriteResourceFiles(BuildCommandSet commands, BuildSettings settings)
		{
			BuildOutput result;
			BuildInterface.WriteResourceFiles_Injected(ref commands, ref settings, out result);
			return result;
		}

		public static uint ArchiveAndCompress(BuildOutput.ResourceFile[] resourceFiles, string outputBundlePath, BuildCompression compression)
		{
			return BuildInterface.ArchiveAndCompress_Injected(resourceFiles, outputBundlePath, ref compression);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateBuildInput_Injected(out BuildInput ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ObjectIdentifier[] GetPlayerObjectIdentifiersInAsset_Injected(ref GUID asset, BuildTarget target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ObjectIdentifier[] GetPlayerDependenciesForObject_Injected(ref ObjectIdentifier objectID, BuildTarget target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type GetTypeForObject_Injected(ref ObjectIdentifier objectID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WriteResourceFiles_Injected(ref BuildCommandSet commands, ref BuildSettings settings, out BuildOutput ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint ArchiveAndCompress_Injected(BuildOutput.ResourceFile[] resourceFiles, string outputBundlePath, ref BuildCompression compression);
	}
}
