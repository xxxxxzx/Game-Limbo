using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An extension to a platform that represents a door.
	/// </summary>
	public class Door : Platform
	{
		/// <summary>
		/// Item type for the key that unlocks door, or empty string if no key is required.
		/// </summary>
		[Tooltip ("Item type for the key that unlocks this door. leave emptyfor a door that does not require a key.")]
		public string keyType;

		/// <summary>
		/// Item type for the key that unlocks door, or empty string if no key is required.
		/// </summary>
		[Tooltip ("Should this door start in the open state")]
		public bool startOpen;

		/// <summary>
		/// Is door currently open, closed, opening or closing.
		/// </summary>
		protected DoorState state;

		#region events


		/// <summary>
		/// Event for door opened.
		/// </summary>
		public event System.EventHandler <DoorEventArgs> Opened;

		/// <summary>
		/// Event for door closed.
		/// </summary>
		public event System.EventHandler <DoorEventArgs> Closed;

		/// <summary>
		/// Raises the door opened event.
		/// </summary>
		virtual protected void OnOpened(Character character)
		{
			if (Opened != null)
			{
				Opened(this, new DoorEventArgs(this, character));
			}
		}

		/// <summary>
		/// Raises the door opened event.
		/// </summary>
		virtual protected void OnClosed(Character character)
		{
			if (Closed != null)
			{
				Closed(this, new DoorEventArgs(this, character));
			}
		}

		#endregion

		/// <summary>
		/// Unity start hook.
		/// </summary>
	 	void Start()
	 	{
			Init ();
		}

		/// <summary>
		/// Init this door.
		/// </summary>
		override protected void Init() 
		{
			PersistableObject po = GetComponent<PersistableObject> ();
			// No persistable lets set a default state
			if (po == null)
			{
				if (startOpen) state = DoorState.OPEN;
				else state = DoorState.CLOSED;
			}
			conditions = GetComponents<AdditionalCondition> ();
	 	}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PlatformerPro.Platform"/> is activated.
		/// </summary>
		override public bool Activated
		{
			get
			{
				// Generally doors are always active.
				return true;
			}
			protected set
			{
				Debug.LogWarning ("You should not call Activate on a door. Instead use Open() or Close().");
			}
		}

		/// <summary>
		/// Open the door.
		/// </summary>
		virtual public void Open(Character character) 
		{
			// Check additional conditions
			if (conditions != null)
			{
				foreach (AdditionalCondition condition in conditions)
				{
					if (!condition.CheckCondition (character, this))
						return;
				}
			}

			if (keyType == null || keyType == "")
			{
				DoOpen (character);
			} 
			else
			{
				ItemManager itemManager = character.GetComponentInChildren<ItemManager> ();
				if (itemManager != null)
				{
					if (itemManager.ItemCount(keyType) > 0) DoOpen (character);
				}
				else
				{
					Debug.LogError("Door requires a key but there is no item manager in the scene.");
				}
		    }
		}

		/// <summary>
		/// Forces the door open.
		/// </summary>
		virtual public void ForceOpen() 
		{
			DoOpen (null);
		}

		/// <summary>
		/// Forces the door closed.
		/// </summary>
		virtual public void ForceClosed() 
		{
			DoClose (null);
		}


		/// <summary>
		/// Close the door.
		/// </summary>
		virtual public void Close(Character character) 
		{
			// Check additional conditions
			if (conditions != null)
			{
				foreach (AdditionalCondition condition in conditions)
				{
					if (!condition.CheckCondition (character, this))
						return;
				}
			}

			DoClose(character);
		}

		/// <summary>
		/// Show or otherwise handle the door opening.
		/// </summary>
		virtual protected void DoOpen(Character character)
		{
			state = DoorState.OPEN;
			OnOpened (character);
			// If we have a persistable object attached set the state.
			PersistableObject po = GetComponent<PersistableObject> ();
			if (po != null) po.SetState (true);
		}

		/// <summary>
		/// Show or otherwise handle the door closing.
		/// </summary>
		virtual protected void DoClose(Character character)
	 	{
			state = DoorState.CLOSED;
			OnClosed (character);
			// If we have a persistable object attached set the state.
			PersistableObject po = GetComponent<PersistableObject> ();
			if (po != null) po.SetState (false);
	 	}


		/// <summary>
		/// Called to determine if collision should be ignored. Use for one way platforms or z-ordered platforms
		/// like those found in loops.
		/// </summary>
		/// <returns><c>true</c>, if Collision should be ignored, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="collider">Collider.</param>
		override public bool IgnoreCollision(Character character, BasicRaycast collider)
		{
			// Override, don't use additional conditions here, on a door additional conditions apply to openeing the door.
			return false;
		}

	}

	public enum DoorState
	{
		OPEN, 
		CLOSED,
		OPENING,
		CLOSING
	}

}