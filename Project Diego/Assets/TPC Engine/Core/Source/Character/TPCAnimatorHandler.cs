/* ==================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================== */

using UnityEngine;

namespace TPCEngine
{
	/// <summary>
	/// 
	/// </summary>
	[System.Serializable]
	public class TPCAnimatorHandler
	{
		#region [Required Variables]
		private Animator animator;
		private Transform transform;
		private TPCMotor characterMotor;
		#endregion

		#region [Functions]
		/// <summary>
		/// Init is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		public void Init(Animator animator, Transform transform, TPCMotor characterMotor)
		{
			this.animator = animator;
			this.transform = transform;
			this.characterMotor = characterMotor;
		}

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		public virtual void UpdateAnimator()
		{
			if (animator == null || !animator.enabled)
				return;
			animator.SetBool("IsGrounded", characterMotor.IsGrounded);
			animator.SetFloat("GroundDistance", characterMotor.GroundDistance);
			animator.SetBool("IsCrouching", characterMotor.IsCrouching);
			animator.SetInteger("MoveAmount", (int) characterMotor.MoveAmount);
			if (!characterMotor.IsGrounded)
				animator.SetFloat("VerticalVelocity", characterMotor.VerticalVelocity);
			if (characterMotor.IsStrafing)
				animator.SetFloat("Direction", characterMotor.Direction, 0.1f, Time.deltaTime);
			animator.SetFloat("Speed", characterMotor.Speed, 0.1f, Time.deltaTime);
		}

		/// <summary>
		/// Callback for processing animation movements for modifying root motion. (Call in OnAnimatorMove())
		/// </summary>
		public virtual void AnimatorMove()
		{
			if (!characterMotor.IsGrounded)
				return;
			transform.rotation = animator.rootRotation;
			characterMotor.SpeedObserver();
		}
		#endregion

		public Animator _Animator { get { return animator; } }
	}
}