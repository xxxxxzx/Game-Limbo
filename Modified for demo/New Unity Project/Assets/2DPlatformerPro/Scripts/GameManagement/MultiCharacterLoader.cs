using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// Activates a character object. Extends the basic loader to enable multiple characters to be picked from.
	/// </summary>
	public class MultiCharacterLoader : CharacterLoader
	{
		/// <summary>
		///  Character to activate.
		/// </summary>
		[Tooltip ("Characters to activate (maps string id to Character")]
		public List<IdToCharacter> characters;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start () 
		{
			if (charactersToLoad == null || charactersToLoad.Count < 1)
			{
				Debug.LogWarning ("To use MultiCharacterLoader you must set the characters to load via ShouldLoadCharacter/ShouldLoadCharacters");
				return;
			}
			if (character != null || characterId != "") Debug.LogWarning ("MultiCharacterLoader ignores the Character/CharacterId setting, instead use the 'characters' map.");
			LoadCharacter ();
		}

		/// <summary>
		/// Does the load of all characters.
		/// </summary>
		override protected IEnumerator DoLoad()
		{
			int numberLoaded = 0;
			List<Character> filteredCharacters = characters.Where (itc => charactersToLoad.Contains (itc.id)).Select (itc => itc.character).ToList ();
			if (LevelManager.Instance != null)
			{
				foreach (Character c in filteredCharacters)
				{
					LevelManager.Instance.Respawn (c);
				}
			}
			yield return new WaitForSeconds (delay);
			foreach (Character c in filteredCharacters)
			{
				numberLoaded++;
				c.gameObject.SetActive (true);
				OnCharacterLoaded (c);
			}
			if (numberLoaded == 0) Debug.LogWarning("No characters were loaded. Check character ids!");
		}
	}

	/// <summary>
	/// Maps string id to character.
	/// </summary>
	[System.Serializable]
	public class IdToCharacter
	{
		/// <summary>
		/// The identifier.
		/// </summary>
		public string id;

		/// <summary>
		/// The character.
		/// </summary>
		public Character character;
	}
}