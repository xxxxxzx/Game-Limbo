using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Component to attach to an object if you wish its state to be saved.
	/// </summary>
	public class PersistableObject : MonoBehaviour 
	{
		/// <summary>
		/// Unique ID for this object.
		/// </summary>
		[Tooltip ("Unique ID for this object.")]
		public string guid = System.Guid.NewGuid().ToString();

		/// <summary>
		/// GameObject to apply persitence settings to, defaults to self.
		/// </summary>
		[Tooltip ("GameObject to apply persitence settings to, defaults to self.")]
		public GameObject target;

		/// <summary>
		/// How do we implement the persistence settings.
		/// </summary>
		[Tooltip ("How do we implement the persistence settings.")]
		public PersistableObjectType implementation;

		/// <summary>
		/// If true then the obejct starts disabled.
		/// </summary>
		public bool defaultStateIsDisabled;

		/// <summary>
		/// Unity awake hook.
		/// </summary>
		void Awake()
		{
			if (target == null) target = gameObject;
			ProcessState ();
		}

		/// <summary>
		/// Sets the persistence state.
		/// </summary>
		/// <param name="state">State to set.</param>
		public void SetState(bool state)
		{
			SetState(state, "");
		}

		/// <summary>
		/// Sets the persistence state.
		/// </summary>
		/// <param name="state">State to set.</param>
		public void SetState(bool state, string extraInfo)
		{
			PersistableObjectManager.Instance.SetState (guid, state, extraInfo);
		}

		/// <summary>
		/// Processes the persisted state.
		/// </summary>
		virtual protected void ProcessState()
		{
			PersistableObjectData data = PersistableObjectManager.Instance.GetState (guid, defaultStateIsDisabled);
			if (data.state)
			{
				switch (implementation)
				{
				case PersistableObjectType.ACTIVATE_DEACTIVATE:
					target.SetActive (true);
					break;
				// Skip destroy there is no corresponding 'undestroy'
				case PersistableObjectType.SEND_MESSAGE:
					target.SendMessage ("SetPersistenceState", true, SendMessageOptions.RequireReceiver);
					break;
				case PersistableObjectType.PLATFORM:
					Platform platform = target.GetComponent<Platform> ();
					if (platform == null)
					{
						Debug.LogWarning ("Persistence type set to PLATFORM but no Platform component was found");
					} else
					{ 
						platform.Activate (null);
					}
					break;
				case PersistableObjectType.DOOR:
					Door door = target.GetComponent<Door> ();
					if (door == null)
					{
						Debug.LogWarning ("Persistence type set to DOOR but no Door component was found");
					} else
					{ 
						door.ForceOpen ();
					}
					break;
				}
			} 
			else
			{
				switch (implementation)
				{
				case PersistableObjectType.ACTIVATE_DEACTIVATE:
					target.SetActive (false);
					break;
				case PersistableObjectType.DESTROY:
					Destroy (target);
					break;
				case PersistableObjectType.SEND_MESSAGE:
					target.SendMessage ("SetPersistenceState", false, SendMessageOptions.RequireReceiver);
					break;
				case PersistableObjectType.PLATFORM:
					Platform platform = target.GetComponent<Platform> ();
					if (platform == null)
					{
						Debug.LogWarning ("Persistence type set to PLATFORM but no Platform component was found");
					} 
					else
					{ 
						platform.Deactivate (null);
					}
					break;
				case PersistableObjectType.DOOR:
					Door door = target.GetComponent<Door> ();
					if (door == null)
					{
						Debug.LogWarning ("Persistence type set to DOOR but no Door component was found");
					} else
					{ 
						door.ForceClosed ();
					}
					break;
				}
			}
		}
	}
}
