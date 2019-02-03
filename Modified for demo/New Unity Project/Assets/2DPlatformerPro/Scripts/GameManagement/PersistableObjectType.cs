using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Different ways we can impleemnt the persisted state in the game world.
	/// </summary>
	public enum PersistableObjectType
	{
		ACTIVATE_DEACTIVATE,
		DESTROY,
		SEND_MESSAGE,
		PLATFORM,
		DOOR
	}

	public static class PersistableObjectTypeExtensions
	{
		public static string GetDescription(this PersistableObjectType me)
		{
			switch(me)
			{
			case PersistableObjectType.ACTIVATE_DEACTIVATE: return "Activate or deactivate GameObject based on persistence state.";
			case PersistableObjectType.DESTROY: return "Destroy GameObject is persistence state is false.";
			case PersistableObjectType.SEND_MESSAGE: return "Send SetPersistenceState() message to the GameObject.";
			case PersistableObjectType.PLATFORM: return "Activate or deactivate platform based on persistence state (Platform componets only).";
			case PersistableObjectType.DOOR: return "Open or close door based on persistence state (Door componets only).";
			}
			return "No information available.";
		}
	}

}
