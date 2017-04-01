using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	public struct DrawRendererSettings
	{
		public DrawRendererSortSettings sorting;

		public ShaderPassName shaderPassName;

		public InputFilter inputFilter;

		public RendererConfiguration rendererConfiguration;

		public DrawRendererFlags flags;

		private IntPtr _cullResults;

		public CullResults cullResults
		{
			set
			{
				this._cullResults = value.cullResults;
			}
		}

		public DrawRendererSettings(CullResults cullResults, Camera camera, ShaderPassName shaderPassName)
		{
			this._cullResults = cullResults.cullResults;
			this.shaderPassName = shaderPassName;
			this.rendererConfiguration = RendererConfiguration.None;
			this.flags = DrawRendererFlags.EnableInstancing;
			this.inputFilter = InputFilter.Default();
			DrawRendererSettings.InitializeSortSettings(camera, out this.sorting);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitializeSortSettings(Camera camera, out DrawRendererSortSettings sortSettings);
	}
}
