using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{

	/// <summary>
	/// Enemy AI which runs a default movement until damage at which point it hides. 
	/// Optionally if hit while hiding it can enter a charge state.
	/// </summary>
	public class EnemyAI_HideOnDamage : EnemyAI
	{

		/// <summary>
		/// How long after being damaged does the enemy hide for.
		/// </summary>
		[Tooltip ("How long after being damaged the enemy hides for.")]
		public float hideStateTime = 5.0f;

		/// <summary>
		/// After hiding finished this amount of health will be added to the character.
		/// </summary>
		public int addHealthAfterHide = 1;

		/// <summary>
		/// If true then the enemy will go into a charge state after being hit when hiding.
		/// </summary>
		[Tooltip ("If true then the enemy will go into a charge state after being hit when hiding.")]
		public bool useChargeState;

		/// <summary>
		/// Counts down from hideStateTime when the enemy hides.
		/// </summary>
		protected float hideStateTimer;

		/// <summary>
		/// Are we currently charging?
		/// </summary>
		protected bool isCharging;

		/// <summary>
		/// Cached reference to a charge movement which we need to be able to set the direction on.
		/// </summary>
		protected EnemyMovement chargeMovement;

		/// <summary>
		/// Optional cached copy of a cahracter hurt box enabling the enemy to collect coins and cause damage to other enemies.
		/// </summary>
		protected CharacterHitBox characterHitBox;

		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update()
		{
			if (hideStateTimer > 0.0f) hideStateTimer -= TimeManager.FrameTime;
		}

		/// <summary>
		/// Unity disable hook.
		/// </summary>
		void OnDisable()
		{
			enemy.Damaged -= EnemyDamaged;
			enemy.Collided -= EnemyDamaged;
		}

		/// <summary>
		/// Init this enemy AI.
		/// </summary>
		override public void Init(Enemy enemy)
		{
			base.Init (enemy);
			// Listen to damage events and if we get them hide
			enemy.Damaged += EnemyDamaged;
			enemy.Collided += EnemyDamaged;

			// If we need it try and find a charge movement
			if (useChargeState) {
				EnemyMovement_Distributor distributor = enemy.GetComponentInChildren<EnemyMovement_Distributor>();
				if (distributor != null) 
				{
					foreach (EnemyStateToMovement estm in distributor.statesToMovements)
					{
						if (estm.state == EnemyState.CHARGING) 
						{
							chargeMovement = estm.movement;
							break;
						}
					}
				}
				else
				{
					chargeMovement = enemy.GetComponentInChildren<EnemyMovement_Charge>();
				}
			}
			// Try to find a cahracter hurt box so we can collect coins and damage other enemies
			characterHitBox = GetComponentInChildren<CharacterHitBox>();
			if (characterHitBox != null)
			{
				// Don't hit ourselves
				EnemyHurtBox myHurtBox = enemy.GetComponentInChildren<EnemyHurtBox>();
				if (myHurtBox != null) Physics2D.IgnoreCollision(characterHitBox.GetComponent<Collider2D>(), myHurtBox.GetComponent<Collider2D>());
          		// Init hit box
				characterHitBox.Init (new DamageInfo(1, DamageType.PHYSICAL, Vector2.zero));
			}
		}

		/// <summary>
		/// The sense routine used to detect when something changes. In this
		/// case cast a ray from the transform in the facing direction to look for the player.
		/// </summary>
		override public bool Sense()
		{
			if (isCharging && enemy.State != EnemyState.CHARGING)
			{
				return true;
			}
			if (hideStateTimer > 0.0f && enemy.State != EnemyState.HIDING)
			{
				return true;
			}
			if (hideStateTimer <= 0.0f && enemy.State == EnemyState.HIDING)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Decide the next move.
		/// </summary>
		override public EnemyState Decide()
		{
			if (isCharging )  return EnemyState.CHARGING;
			if (hideStateTimer > 0.0f) 
			{
				if (enemy.State != EnemyState.HIDING && addHealthAfterHide > 0) enemy.Heal(addHealthAfterHide);
				return EnemyState.HIDING;
			}
			return EnemyState.DEFAULT;
		}

		/// <summary>
		/// Used to inform the AI we were damaged so an action like HIDE may be triggered.
		/// </summary>
		override public void Damaged()
		{
			hideStateTimer = hideStateTime;
		}

		/// <summary>
		/// When enemy is damaged keep hiding!
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		virtual protected void EnemyDamaged( object sender, DamageInfoEventArgs args)
		{
			if (!isCharging && useChargeState && hideStateTimer > 0)
			{
				if (chargeMovement != null) chargeMovement.SetDirection(args.DamageInfo.Direction);
				isCharging = true;
				hideStateTimer = -1.0f;
				// Turn on hurtbox
				if (characterHitBox != null && args.DamageInfo.DamageCauser is Character) 
				{
					characterHitBox.Character = (Character) args.DamageInfo.DamageCauser;
					characterHitBox.GetComponent<Collider2D>().enabled = true;
				}
			}
			else
			{
				hideStateTimer = hideStateTime;
				isCharging = false;
				// Turn off hurt box
				if (characterHitBox != null) 
				{
					characterHitBox.GetComponent<Collider2D>().enabled = false;
				}
			}
		}

#if UNITY_EDITOR

		/// <summary>
		/// Static info used by the editor.
		/// </summary>
		override public EnemyState[] Info
		{
			get
			{
				return new EnemyState[]{EnemyState.DEFAULT, EnemyState.HIDING, EnemyState.CHARGING};
			}
		}

#endif

	}
}

