using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition which requires a specific item to be held.
	/// </summary>
	public class MustHaveItemCondition : AdditionalCondition
	{
		/// <summary>
		/// The item manager.
		/// </summary>
		protected ItemManager itemManager;

		/// <summary>
		/// If this is not empty require the character to have an item with the matching type before triggering.
		/// </summary>
		[Tooltip ("If this is not empty require the character to have an item with the matching type to meet this condition.")]
		public string requiredItemType;

		/// <summary>
		/// The number required.
		/// </summary>
		[Tooltip ("The number of item required to activate the condition.")]
		public int requiredItemCount = 1;

		/// <summary>
		/// The optional number of the item to consume when the effect is activated.
		/// </summary>
		[Tooltip ("The optional number of the item to consume when the effect is activated.")]
		public int numberConsumed = 0;

		/// <summary>
		/// If only one character will ever use this, save a reference to the item manager.
		/// </summary>
		[Tooltip ("If only one character will ever use this condition, save a reference to the item manager.")]
		public bool cacheItemManager = true;

		/// <summary>
		/// Checks the condition. For example a check when entering a trigger.
		/// </summary>
		/// <returns><c>true</c>, if enter trigger was shoulded, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (requiredItemType != null && requiredItemType != "")
			{
				if (itemManager == null || !cacheItemManager)
				{
					itemManager = character.GetComponentInChildren<ItemManager> ();
				}
				if (itemManager == null) 
				{
					Debug.LogWarning("Conditions requires an item but the character has no item manager.");
					return false;
				}
				if (itemManager.ItemCount(requiredItemType) >= requiredItemCount) return true;
				return false;
			}
			Debug.LogWarning("MustHaveItemCondition has no item configured.");
			return false;
		}

		/// <summary>
		/// Applies any activation effects.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="other">Other object supporting the condition.</param>
		override public void Activated(Character character, object other)
		{
			// ItemManager should have already been set in the call to CheckCondition
			if (numberConsumed > 0) itemManager.ConsumeItem (requiredItemType, numberConsumed);
		}

	}

}
