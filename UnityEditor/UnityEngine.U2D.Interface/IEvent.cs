using System;

namespace UnityEngine.U2D.Interface
{
	internal interface IEvent
	{
		EventType type
		{
			get;
		}

		string commandName
		{
			get;
		}

		bool control
		{
			get;
		}

		bool alt
		{
			get;
		}

		bool shift
		{
			get;
		}

		KeyCode keyCode
		{
			get;
		}

		Vector2 mousePosition
		{
			get;
		}

		int button
		{
			get;
		}

		EventModifiers modifiers
		{
			get;
		}

		EventType GetTypeForControl(int id);

		void Use();
	}
}
