using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class AssemblyValidationRule : Attribute
{
	public int Priority;

	private readonly RuntimePlatform _platform;

	public RuntimePlatform Platform
	{
		get
		{
			return this._platform;
		}
	}

	public AssemblyValidationRule(RuntimePlatform platform)
	{
		this._platform = platform;
		this.Priority = 0;
	}
}
