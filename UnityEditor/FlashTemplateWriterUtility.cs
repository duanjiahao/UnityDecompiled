using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
internal class FlashTemplateWriterUtility
{
	private readonly string _outputFile;
	private readonly string _templateFile;
	private readonly Dictionary<string, string> _tagsToReplace;
	public FlashTemplateWriterUtility(string templateFile, string outputFile, Dictionary<string, string> tagsToReplace)
	{
		this._outputFile = outputFile;
		this._tagsToReplace = tagsToReplace;
		this._templateFile = templateFile;
	}
	public void WriteTemplate()
	{
		string seed = File.ReadAllText(this._templateFile);
		string contents = this._tagsToReplace.Aggregate(seed, (string current, KeyValuePair<string, string> kvp) => current.Replace("[%" + kvp.Key + "%]", kvp.Value));
		File.WriteAllText(this._outputFile, contents);
	}
	public static void WriteGendarmeXMLTemplate(FlashPostProcessSettings settings, FlashFileHelper fileHelper, GendarmeXMLTemplateTags gendarmeXMLTemplateTags, string output)
	{
		new FlashTemplateWriterUtility(fileHelper.PathForFileInGendarme("rules_template.xml"), output, gendarmeXMLTemplateTags.ToTagsDictionary()).WriteTemplate();
	}
	public static void WriteHTMLTemplate(FlashPostProcessSettings settings, FlashFileHelper fileHelper)
	{
		new FlashTemplateWriterUtility(fileHelper.PathForFileInBuildTools("index.html"), fileHelper.PathForHtmlInInstallPath(), FlashTemplateWriterUtility.GetTagsToReplaceForHTML(settings, fileHelper)).WriteTemplate();
	}
	private static Dictionary<string, string> GetTagsToReplaceForHTML(FlashPostProcessSettings settings, FlashFileHelper fileHelper)
	{
		int num = settings.Height / 2;
		return new Dictionary<string, string>
		{

			{
				"hdimy",
				num.ToString()
			},

			{
				"dimx",
				settings.Width.ToString()
			},

			{
				"dimy",
				settings.Height.ToString()
			},

			{
				"swfname",
				fileHelper.ProjectName() + ".swf"
			},

			{
				"flashplayer_version",
				settings.GetTargetPlayerForSubtarget()
			}
		};
	}
}
