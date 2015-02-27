using System;
using System.Collections.Generic;
using System.Linq;
internal class GendarmeXMLTemplateTags
{
	public bool IncludePrimitiveTypes;
	public bool AutomaticallyWhitelistAttributes;
	public List<string> WhiteListFiles = new List<string>();
	public List<string> BlackListFiles = new List<string>();
	public List<string> WhiteListAssemblies = new List<string>();
	public List<string> BlackListAssemblies = new List<string>();
	public List<string> LimitToNamespaces = new List<string>();
	public List<string> WhiteListTypes = new List<string>();
	public GendarmeXMLTemplateTags()
	{
		this.IncludePrimitiveTypes = true;
		this.AutomaticallyWhitelistAttributes = true;
	}
	public Dictionary<string, string> ToTagsDictionary()
	{
		return new Dictionary<string, string>
		{

			{
				"IncludePrimitiveTypes",
				(!this.IncludePrimitiveTypes) ? "0" : "1"
			},

			{
				"AutomaticallyWhitelistAttributes",
				(!this.AutomaticallyWhitelistAttributes) ? "0" : "1"
			},

			{
				"LimitToNamespaces",
				this.AggregateIntoString(this.LimitToNamespaces)
			},

			{
				"WhiteListTypes",
				this.AggregateIntoString(this.WhiteListTypes)
			},

			{
				"WhiteListFiles",
				this.AggregateIntoString(this.WhiteListFiles)
			},

			{
				"BlackListFiles",
				this.AggregateIntoString(this.BlackListFiles)
			},

			{
				"WhiteListAssemblies",
				this.AggregateIntoString(this.WhiteListAssemblies)
			},

			{
				"BlackListAssemblies",
				this.AggregateIntoString(this.BlackListAssemblies)
			}
		};
	}
	private string AggregateIntoString(IEnumerable<string> list)
	{
		return list.DefaultIfEmpty<string>().Aggregate(new Func<string, string, string>(GendarmeXMLTemplateTags.ToCommaSeparatedString));
	}
	private static string ToCommaSeparatedString(string agg, string s)
	{
		return agg + "," + s;
	}
}
