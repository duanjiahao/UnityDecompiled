using System;

namespace UnityEditor.VersionControl
{
	public enum SubmitResult
	{
		OK = 1,
		Error,
		ConflictingFiles = 4,
		UnaddedFiles = 8
	}
}
