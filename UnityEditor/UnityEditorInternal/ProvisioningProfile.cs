using System;
using System.IO;
using System.Text.RegularExpressions;

namespace UnityEditorInternal
{
	internal class ProvisioningProfile
	{
		private string m_UUID = string.Empty;

		private static readonly string s_FirstLinePattern = "<key>UUID<\\/key>";

		private static readonly string s_SecondLinePattern = "<string>((\\w*\\-?){5})";

		public string UUID
		{
			get
			{
				return this.m_UUID;
			}
			set
			{
				this.m_UUID = value;
			}
		}

		internal ProvisioningProfile()
		{
		}

		internal ProvisioningProfile(string UUID)
		{
			this.m_UUID = UUID;
		}

		internal static ProvisioningProfile ParseProvisioningProfileAtPath(string pathToFile)
		{
			ProvisioningProfile provisioningProfile = new ProvisioningProfile();
			ProvisioningProfile.parseFile(pathToFile, provisioningProfile);
			return provisioningProfile;
		}

		private static void parseFile(string filePath, ProvisioningProfile profile)
		{
			StreamReader streamReader = new StreamReader(filePath);
			string input;
			while ((input = streamReader.ReadLine()) != null)
			{
				Match match = Regex.Match(input, ProvisioningProfile.s_FirstLinePattern);
				if (match.Success)
				{
					if ((input = streamReader.ReadLine()) != null)
					{
						Match match2 = Regex.Match(input, ProvisioningProfile.s_SecondLinePattern);
						if (match2.Success)
						{
							profile.UUID = match2.Groups[1].Value;
							break;
						}
					}
				}
				if (!string.IsNullOrEmpty(profile.UUID))
				{
					break;
				}
			}
			streamReader.Close();
		}
	}
}
