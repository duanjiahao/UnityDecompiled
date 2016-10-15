using System;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[Obsolete("PostProcessAttribute has been renamed to CallbackOrderAttribute."), RequiredByNativeCode]
	public abstract class PostProcessAttribute : CallbackOrderAttribute
	{
		[Obsolete("PostProcessAttribute has been renamed. Use m_CallbackOrder of CallbackOrderAttribute.")]
		protected int m_PostprocessOrder;

		[Obsolete("PostProcessAttribute has been renamed. Use callbackOrder of CallbackOrderAttribute.")]
		internal int GetPostprocessOrder
		{
			get
			{
				return this.m_PostprocessOrder;
			}
		}
	}
}
