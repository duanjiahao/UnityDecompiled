using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[Serializable]
	public class AnimationTriggers
	{
		private const string kDefaultNormalAnimName = "Normal";

		private const string kDefaultSelectedAnimName = "Highlighted";

		private const string kDefaultPressedAnimName = "Pressed";

		private const string kDefaultDisabledAnimName = "Disabled";

		[FormerlySerializedAs("normalTrigger"), SerializeField]
		private string m_NormalTrigger = "Normal";

		[FormerlySerializedAs("highlightedTrigger"), FormerlySerializedAs("m_SelectedTrigger"), SerializeField]
		private string m_HighlightedTrigger = "Highlighted";

		[FormerlySerializedAs("pressedTrigger"), SerializeField]
		private string m_PressedTrigger = "Pressed";

		[FormerlySerializedAs("disabledTrigger"), SerializeField]
		private string m_DisabledTrigger = "Disabled";

		public string normalTrigger
		{
			get
			{
				return this.m_NormalTrigger;
			}
			set
			{
				this.m_NormalTrigger = value;
			}
		}

		public string highlightedTrigger
		{
			get
			{
				return this.m_HighlightedTrigger;
			}
			set
			{
				this.m_HighlightedTrigger = value;
			}
		}

		public string pressedTrigger
		{
			get
			{
				return this.m_PressedTrigger;
			}
			set
			{
				this.m_PressedTrigger = value;
			}
		}

		public string disabledTrigger
		{
			get
			{
				return this.m_DisabledTrigger;
			}
			set
			{
				this.m_DisabledTrigger = value;
			}
		}
	}
}
