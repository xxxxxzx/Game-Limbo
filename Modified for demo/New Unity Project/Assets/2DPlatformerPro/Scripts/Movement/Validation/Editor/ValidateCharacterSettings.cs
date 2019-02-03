using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Validation
{
	/// <summary>
	/// Validate layer settings
	/// </summary>
	public class ValidateCharacterSettings : IValidator
	{
		protected const float MINIMUM_GROUND_LOOKAHEAD = 0.05f;

		protected const float MINIMUM_FEET_LOOKAHEAD_FOR_SLOPES = 0.2f;

		protected const float EXPECTED_STICK_GROUND_LOOKAHEAD_RATIO = 1.0f;

		/// <summary>
		/// Apply this validation to the scene.
		/// </summary>
		public List<ValidationResult> Validate()
		{
			List<ValidationResult> result = new List<ValidationResult> ();

			// Get all characters
			Character[] allCharacters = GameObject.FindObjectsOfType<Character> ();
			foreach (Character c in allCharacters)
			{
				result.AddRange (CheckLookAheads(c));
				result.AddRange (CheckSlopes(c));
				result.AddRange (CheckMovements(c));
			}
			return result;
		}

		/// <summary>
		/// Checks that character look aheads use reasonable values
		/// </summary>
		protected List<ValidationResult> CheckLookAheads(Character c)
		{
			List<ValidationResult> result = new List<ValidationResult> ();
		
			// Look aheads
			if (c.groundedLookAhead > c.feetLookAhead)
			{
				result.Add(new ValidationResult(string.Format("Your grounded look ahead is larger than your feet look ahead. It shouldn't be.", MINIMUM_GROUND_LOOKAHEAD), MessageType.Info));
			}
			if (c.groundedLookAhead < MINIMUM_GROUND_LOOKAHEAD)
			{
				result.Add(new ValidationResult(string.Format("Your grounded look ahead is very low which may affect some movements. You may want to increase it to at least {0}.", MINIMUM_GROUND_LOOKAHEAD), MessageType.Info));
			}
			if (c.groundedLookAhead < MINIMUM_FEET_LOOKAHEAD_FOR_SLOPES && c.rotateToSlopes)
			{
				result.Add(new ValidationResult(string.Format("You are using slopes and your feet look ahead is quite low. This will result in jerky slopes. You may want to increase it to at least {0}.", MINIMUM_FEET_LOOKAHEAD_FOR_SLOPES), MessageType.Warning));
			}
			else if (c.feetLookAhead < MINIMUM_GROUND_LOOKAHEAD)
			{
				result.Add(new ValidationResult(string.Format("Your feet look ahead is quite low. You may want to increase it to at least {0}.", MINIMUM_GROUND_LOOKAHEAD), MessageType.Warning));
			}
			return result;
		}

		/// <summary>
		/// Checks that slopes settings seem reasonable
		/// </summary>
		protected List<ValidationResult> CheckSlopes(Character c)
		{
			List<ValidationResult> result = new List<ValidationResult> ();
			
			return result;
		}

		/// <summary>
		/// Checks that movements have sane settings in relation to the Character
		/// </summary>
		protected List<ValidationResult> CheckMovements(Character c)
		{
			List<ValidationResult> result = new List<ValidationResult> ();
			foreach (Movement m in c.GetComponentsInChildren<Movement>())
			{
				result.AddRange(m.ValidateMovement (c));
			}
			return result;
		}
	}
}