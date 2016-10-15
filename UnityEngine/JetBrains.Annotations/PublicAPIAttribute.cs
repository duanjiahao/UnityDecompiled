using System;

namespace JetBrains.Annotations
{
	[MeansImplicitUse]
	public sealed class PublicAPIAttribute : Attribute
	{
		[NotNull]
		public string Comment
		{
			get;
			private set;
		}

		public PublicAPIAttribute()
		{
		}

		public PublicAPIAttribute([NotNull] string comment)
		{
			this.Comment = comment;
		}
	}
}
