using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Scales an image bar based on the percentage of breath (current/max).
	/// </summary>
	[RequireComponent (typeof(Image))]
	public class UIBreath_PercentageBar : MonoBehaviour
	{

		/// <summary>
		/// The item manager.
		/// </summary>
		public Breath breath;

		/// <summary>
		/// Should we use image fill?
		/// </summary>
		public bool useFill = true;

		/// <summary>
		/// The bar image.
		/// </summary>
		protected Image barImage;
		
		/// <summary>
		/// Reference to the character loader.
		/// </summary>
		protected CharacterLoader characterLoader;

		void Start()
		{
			Init ();
		}

		void Update()
		{
			if (breath != null) UpdateImage ();
		}

		/// <summary>
		/// Do the destroy actions.
		/// </summary>
		void OnDestroy()
		{
			if (characterLoader != null)
			{
				characterLoader.CharacterLoaded -= HandleCharacterLoaded;
			}
		}

		virtual protected void Init()
		{
			barImage = GetComponent<Image> ();
			if (breath == null) 
			{
				// No health assigned try to find one
				if (breath == null) characterLoader = CharacterLoader.GetCharacterLoader();
				if (characterLoader != null)
				{
					characterLoader.CharacterLoaded += HandleCharacterLoaded;
				}
				else 
				{
					breath = GameObject.FindObjectOfType<Breath> ();
					if (breath == null) Debug.LogWarning ("Couldn't find a Breath!");
				}
			}
		}

		/// <summary>
		/// Handles the character being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			breath = e.Character.GetComponentInChildren<Breath>();
			if (breath == null) Debug.LogWarning ("The loaded character doesn't have a Breath component.");
		}

		virtual protected void UpdateImage()
		{
			if (useFill)
				barImage.fillAmount = breath.CurrentBreathAsPercentage;
			else
				barImage.rectTransform.sizeDelta = new Vector2(100.0f * breath.CurrentBreathAsPercentage, barImage.rectTransform.sizeDelta.y);
		}
	}
}