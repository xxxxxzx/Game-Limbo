using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Activates a character object.
	/// </summary>
	public class CharacterLoader : MonoBehaviour
	{
		/// <summary>
		///  Character to activate.
		/// </summary>
		[Tooltip ("Character to activate (note the GameObject this component is attached to will be activated.")]
		public Character character;

		/// <summary>
		/// How long to wait before loading the character.
		/// </summary>
		[Tooltip ("How long to wait before loading the character.")]
		public float delay;

		/// <summary>
		/// List of character ids which should be loaded on Start()
		/// </summary>
		protected static List<string> charactersToLoad;

		/// <summary>
		/// Corresponding id for the player. If null or empty the character will always be loaded.
		/// </summary>
		[Tooltip ("Corresponding id for the player. If null or empty the character will always be loaded.")]
		public string characterId;

		/// <summary>
		/// Event fired when the cahracter loading finishes.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> CharacterLoaded;

		/// <summary>
		/// Raises the character loaded event.
		/// </summary>
		virtual protected void OnCharacterLoaded()
		{
			if (CharacterLoaded != null)
			{
				CharacterLoaded(this, new CharacterEventArgs(character));
			}
		}

		/// <summary>
		/// Raises the character loaded event.
		/// </summary>
		/// <param name="character">Character that was loaded.</param>
		virtual protected void OnCharacterLoaded(Character character)
		{
			if (CharacterLoaded != null)
			{
				CharacterLoaded(this, new CharacterEventArgs(character));
			}
		}

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake ()
		{
			Register (this);
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start () 
		{
			if (characterId == null || characterId == "" || (charactersToLoad != null && charactersToLoad.Contains(characterId))) LoadCharacter ();
		}
		
		/// <summary>
		/// Unity OnDestroy() hook.
		/// </summary>
		void OnDestroy()
		{
			Deregister (this);
		}

		/// <summary>
		/// Loads the character.
		/// </summary>
		virtual protected void LoadCharacter()
		{
			StartCoroutine (DoLoad ());
		}

		/// <summary>
		/// Does the load after a delay.
		/// </summary>
		virtual protected IEnumerator DoLoad()
		{
			if (LevelManager.Instance != null)
			{
				LevelManager.Instance.Respawn(character);
			}
			yield return new WaitForSeconds(delay);
			character.gameObject.SetActive(true);
			OnCharacterLoaded ();
		}

		#region static behaviour

		protected static List<CharacterLoader> characterLoaders;

		/// <summary>
		/// Gets a character loader (generally for single character games only).
		/// </summary>
		/// <returns>A character loader or null if none found.</returns>
		public static CharacterLoader GetCharacterLoader()
		{
			if (characterLoaders != null && characterLoaders.Count > 0)
			{
				return characterLoaders[0];
			}
			return null;
		}

		/// <summary>
		/// Gets a character loader (generally for single character games only).
		/// </summary>
		/// <returns>A character loader or null if none found.</returns>
		public static List<CharacterLoader> GetCharacterLoaders()
		{
			if (characterLoaders != null) return new List<CharacterLoader> (characterLoaders);
			return new List<CharacterLoader> ();
		}

		/// <summary>
		/// Gets the character loader for the given character or null if no loader matches
		/// </summary>
		/// <returns>The character loader for character.</returns>
		/// <param name="character">Character.</param>
		public static CharacterLoader GetCharacterLoaderForCharacter(Character character)
		{
			if (characterLoaders != null)
			{
				foreach (CharacterLoader loader in characterLoaders)
				{
					if (loader.character == character) return loader;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the character loader for the given character id or null if no loader matches.
		/// </summary>
		/// <returns>The character loader for character id.</returns>
		/// <param name="characterId">Character id string.</param>
		public static CharacterLoader GetCharacterLoaderForCharacters(string characterId)
		{
			if (characterLoaders != null)
			{
				foreach (CharacterLoader loader in characterLoaders)
				{
					if (loader.characterId == characterId) return loader;
				}
			}
			return null;
		}

		/// <summary>
		/// Call this to specify a list of characters to load.
		/// </summary>
		/// <param name="ids">List of character id.</param>
		public static void ShouldLoadCharacters(List<string> ids)
		{
			charactersToLoad = ids;
		}

		/// <summary>
		/// Call this to add a single character to the list of characters to load.
		/// </summary>
		/// <param name="id">Character id.</param>
		public static void ShouldLoadCharacter(string id)
		{
			if (charactersToLoad == null) charactersToLoad = new List<string>();
			if (!(charactersToLoad.Contains (id)))charactersToLoad.Add (id);
		}

		/// <summary>
		/// Call this to remove a single character tofromthe list of characters to load.
		/// </summary>
		/// <param name="id">Character id.</param>
		public static void ShouldNotLoadCharacter(string id)
		{
			if (charactersToLoad == null) return;
			if (charactersToLoad.Contains (id))charactersToLoad.Remove (id);
		}

		/// <summary>
		/// Register the specified loader.
		/// </summary>
		/// <param name="loader">Loader.</param>
		protected static void Register(CharacterLoader loader)
		{
			if (characterLoaders == null) characterLoaders = new List<CharacterLoader>();
			characterLoaders.Add (loader);
		}
		
		/// <summary>
		/// Deregister the specified loader.
		/// </summary>
		/// <param name="loader">Loader.</param>
		protected static void Deregister(CharacterLoader loader)
		{
			if (characterLoaders != null && characterLoaders.Contains (loader)) characterLoaders.Remove (loader);
		}

		#endregion
	}
}