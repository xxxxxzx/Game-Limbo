using UnityEngine;
using System.Collections;

namespace PlatformerPro

{
	/// <summary>
	/// Projectile used for grappling hook.
	/// </summary>
	public class GrapplingHookProjectile : Projectile
	{
		/// <summary>
		/// The visual component of the hook that is disabled when not visible and enabled when visible.
		/// </summary>
		public GameObject visualComponent;

		/// <summary>
		/// Delay before making projectile visible.
		/// </summary>
		public float projectileDelay = 0.05f;

		/// <summary>
		/// If true grapple will parent to the thing it collides with. Required for grappling on to moving platforms.
		/// </summary>
		[Tooltip ("If true grapple will parent to the thing it collides with. Required for grappling on to moving platforms.")]
		public bool shouldParent = true;

		/// <summary>
		/// The movement that launched us.
		/// </summary>
		protected SpecialMovement_GrapplingHook movement;

		/// <summary>
		/// Can we see the projectile?
		/// </summary>
		public bool IsVisible
		{
			get
			{
				return visualComponent.activeInHierarchy;
			}
		}

		/// <summary>
		/// Unity late update hook.
		/// </summary>
		void LateUpdate()
		{
			if (fired)
			{
				// Check that we haven't got too far form character
				if (Vector2.Distance (movement.transform.position, transform.position) > movement.maxDistance)
				{
					movement.CancelGrapple ();
				}
			}
		}

		/// <summary>
		/// Call to start the projectile moving.
		/// </summary>
		/// <param name="damageAmount">Damage amount.</param>
		/// <param name="damageType">Damage type.</param>
		override public void Fire(int damageAmount, DamageType damageType, Vector2 direction, IMob character) 
		{
			this.transform.parent = null;
			base.Fire (damageAmount, damageType, direction, character);
			actualSpeed = speed;
			StartCoroutine (DelayVisualComponent ());
			projectileHitBox.gameObject.SetActive (true);
			if (movement == null && character is Character) movement = ((Character)character).GetComponentInChildren<SpecialMovement_GrapplingHook> ();
			if (movement == null) Debug.LogWarning ("Unable to find grappling hook movement");
		}

		/// <summary>
		/// Delays making componet visible until delay is reached.
		/// </summary>
		/// <returns>The visual component.</returns>
		virtual protected IEnumerator DelayVisualComponent()
		{
			yield return new WaitForSeconds(projectileDelay);
			visualComponent.SetActive (true);
		}

		/// <summary>
		/// Force the grapple from view.
		/// </summary>
		virtual public void Hide()
		{
			StopAllCoroutines ();
			fired = false;
			projectileHitBox.gameObject.SetActive (false);
			visualComponent.SetActive (false);
		}

		/// <summary>
		/// Destroy projectile or latch if we hit scenerey.
		/// </summary>
		override public void DestroyProjectile(bool isEnemyHit)
		{
			fired = false;
			projectileHitBox.gameObject.SetActive (false);
			if (isEnemyHit)
			{
				OnProjectileDestroyed (isEnemyHit ? damageInfo : null);
				StartCoroutine (DoDestroy (isEnemyHit));
			}
			else
			{
				movement.Latch();
			}
		}
	}
}
