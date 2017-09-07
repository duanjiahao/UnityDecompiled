using System;

namespace UnityEditor.Collaboration
{
	[Flags]
	internal enum CollabOperation : ulong
	{
		Noop = 0uL,
		Publish = 1uL,
		Update = 2uL,
		Revert = 4uL,
		GoBack = 8uL,
		Restore = 16uL,
		Diff = 32uL,
		ConflictDiff = 64uL,
		Exclude = 128uL,
		Include = 256uL,
		ChooseMine = 512uL,
		ChooseTheirs = 1024uL,
		ExternalMerge = 2048uL
	}
}
