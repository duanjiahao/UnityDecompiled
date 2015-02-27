using System;
using UnityEngine;
namespace UnityEditor
{
	internal class PrefKey : IPrefType
	{
		private string m_name;
		private Event m_event;
		private string m_DefaultShortcut;
		public string Name
		{
			get
			{
				return this.m_name;
			}
		}
		public Event KeyboardEvent
		{
			get
			{
				return this.m_event;
			}
			set
			{
				this.m_event = value;
			}
		}
		public bool activated
		{
			get
			{
				return Event.current.Equals(this);
			}
		}
		public PrefKey()
		{
		}
		public PrefKey(string name, string shortcut)
		{
			this.m_name = name;
			this.m_event = Event.KeyboardEvent(shortcut);
			this.m_DefaultShortcut = shortcut;
			PrefKey prefKey = Settings.Get<PrefKey>(name, this);
			this.m_name = prefKey.Name;
			this.m_event = prefKey.KeyboardEvent;
		}
		public string ToUniqueString()
		{
			return string.Concat(new object[]
			{
				this.m_name,
				";",
				(!this.m_event.alt) ? string.Empty : "&",
				(!this.m_event.command) ? string.Empty : "%",
				(!this.m_event.shift) ? string.Empty : "#",
				(!this.m_event.control) ? string.Empty : "^",
				this.m_event.keyCode
			});
		}
		public void FromUniqueString(string s)
		{
			int num = s.IndexOf(";");
			if (num < 0)
			{
				Debug.LogError("Malformed string in Keyboard preferences");
				return;
			}
			this.m_name = s.Substring(0, num);
			this.m_event = Event.KeyboardEvent(s.Substring(num + 1));
		}
		internal void ResetToDefault()
		{
			this.m_event = Event.KeyboardEvent(this.m_DefaultShortcut);
		}
		public static implicit operator Event(PrefKey pkey)
		{
			return pkey.m_event;
		}
	}
}
