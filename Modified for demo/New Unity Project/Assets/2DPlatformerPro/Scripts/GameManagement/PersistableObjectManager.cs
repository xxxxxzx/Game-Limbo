using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace PlatformerPro
{
	/// <summary>
	/// Static class for managing persistable objects.
	/// </summary>
	public class PersistableObjectManager : MonoBehaviour
	{
		/// <summary>
		/// Shouldwe save data on change.
		/// </summary>
		[Tooltip ("Should we save on every change. Usually this should be false.")]
		public bool saveOnChange;

	
		/// <summary>
		/// Tracks if we have run initialisation.
		/// </summary>
		protected bool initialised;

		/// <summary>
		/// The persistence data.
		/// </summary>
		protected Dictionary<string, PersistableObjectData> objectData;

		/// <summary>
		/// Cached character reference.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Cached loader reference.
		/// </summary>
		protected CharacterLoader characterLoader;

		/// <summary>
		/// Cached health reference.
		/// </summary>
		protected CharacterHealth characterHealth;

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake()
		{
			if (instance != null) Debug.LogError ("More than one PersistableObjectManager found in the scene.");
			Instance = this;
			Init ();
		}

		/// <summary>
		/// Unity Destory hook.
		/// </summary>
		void OnDestroy()
		{
			if (characterLoader != null) characterLoader.CharacterLoaded -= HandleCharacterLoaded;
			if (character != null) character.WillExitScene -= HandleExitScene;
			if (characterHealth != null)
			{
				characterHealth.GameOver -= HandleGameOver;
				characterHealth.Died -= HandleDied;
			}
		}

		/// <summary>
		/// Gets the state for the object with the given guid. This is not a copy!
		/// </summary>
		/// <param name="guid">GUID.</param>
		/// <param name="defaultStateIsDisabled">If true the object starts disabled.</param>
		public PersistableObjectData GetState(string guid, bool defaultStateIsDisabled)
		{
			if (objectData.ContainsKey (guid))
			{
				return objectData [guid];
			}
			else
			{
				PersistableObjectData data = new PersistableObjectData ();
				data.guid = guid;
				data.state = !defaultStateIsDisabled;
				objectData.Add (guid, data);
				return data;
			}
		}

		/// <summary>
		/// Sets the state for the object with the given guid.
		/// </summary>
		/// <param name="guid">GUID.</param>
		/// <param name="state">State to set.</param>
		/// <param name="extraInfo">Extra info.</param>
		public void SetState(string guid, bool state, string extraInfo)
		{
			if (objectData.ContainsKey (guid))
			{
				objectData [guid].state = state;
				objectData [guid].extraStateInfo = extraInfo;
			}
			else
			{
				PersistableObjectData data = new PersistableObjectData ();
				data.guid = guid;
				data.state = state;
				data.extraStateInfo = extraInfo;
				objectData.Add (guid, data);
			}
			if (saveOnChange) Save ();
		}

		/// <summary>
		/// Updates persistable object state.
		/// </summary>
		public void Save()
		{
			using(StringWriter writer = new StringWriter())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(List<PersistableObjectData>));
				serializer.Serialize(writer, GetSaveData());
				PlayerPrefs.SetString(UniqueDataIdentifier, writer.ToString());
			}
		}

		/// <summary>
		/// Reset persistable object state.
		/// </summary>
		public void Reset()
		{
			PlayerPrefs.SetString(UniqueDataIdentifier, "");
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected void Init()
		{
			if (initialised) return;
			objectData = new Dictionary<string, PersistableObjectData> ();
			Load ();
			InitEvents ();
			initialised = true;
		}

		/// <summary>
		/// Load the saved data from prefs.
		/// </summary>
		protected void Load()
		{
			
			string data = PlayerPrefs.GetString(UniqueDataIdentifier, "");
			if (data.Length > 0)
			{
				List<PersistableObjectData> saveData;
				using (StringReader reader = new StringReader(data)){
					XmlSerializer serializer = new XmlSerializer(typeof(List<PersistableObjectData>));
					saveData = (List<PersistableObjectData>) serializer.Deserialize(reader);
					foreach (PersistableObjectData p in saveData)
					{
						objectData.Add (p.guid, p);
					}
				}
			}
		}

		/// <summary>
		/// Find references and initialise all the event listeners..
		/// </summary>
		protected void InitEvents()
		{
			character = FindObjectOfType <Character>();
			if (character == null)
			{
				characterLoader = CharacterLoader.GetCharacterLoader ();
				if (characterLoader != null)
				{
					characterLoader.CharacterLoaded += HandleCharacterLoaded;
				} 
				else
				{
					Debug.LogWarning ("PersistableObjectManager couldn't find a Character or CharacterLoader.");
				}
			}
			else
			{
				RegisterCharacterEvents ();
			}
		}

		/// <summary>
		/// Registers the character events.
		/// </summary>
		protected void RegisterCharacterEvents()
		{
			if (character == null)
			{
				Debug.LogWarning("Tried to register character events with a null character");
				return;
			}
			character.WillExitScene += HandleExitScene;
			characterHealth = character.GetComponentInChildren<CharacterHealth> ();
			if (characterHealth != null)
			{
				characterHealth.GameOver += HandleGameOver;
				characterHealth.Died += HandleDied;
			}
		}

		/// <summary>
		/// Handles the cahracter dying.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event data.</param>
		void HandleDied (object sender, DamageInfoEventArgs e)
		{
			Save ();
		}

		/// <summary>
		/// Handles the character being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event data.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			character = e.Character;
			RegisterCharacterEvents ();
		}

		/// <summary>
		/// Handles the game ending.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event data.</param>
		virtual public void HandleGameOver (object sender, DamageInfoEventArgs e)
		{
			Reset ();
		}

		/// <summary>
		/// Handles the character exiting the scene.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event data.</param>
		virtual protected void HandleExitScene (object sender, SceneEventArgs e)
		{
			Save ();
		}

		/// <summary>
		/// Convert dictionary into savable list.
		/// </summary>
		/// <returns>The save data.</returns>
		protected List<PersistableObjectData> GetSaveData()
		{
			if (!initialised) Init ();
			return objectData.Values.ToList ();
		}

		#region static methods

		/// <summary>
		/// The static instance.
		/// </summary>
		protected static PersistableObjectManager instance;

		/// <summary>
		/// The player preference identifier.
		/// </summary>
		public const string UniqueDataIdentifier = "PersistableObjectManagerData";

		/// <summary>
		/// Get the static manager instance.
		/// </summary>
		public static PersistableObjectManager Instance
		{
			get
			{
				if (instance == null) CreateNewPersistableObjectManager();
				return instance;
			}
			protected set
			{
				instance = value;
			}
		}

		/// <summary>
		/// Creates a new time manager.
		/// </summary>
		protected static void CreateNewPersistableObjectManager()
		{
			GameObject go = new GameObject ();
			go.name = "PersistableObjectManager";
			instance = go.AddComponent<PersistableObjectManager> ();
			instance.Init ();
		}

		#endregion

	}
}
