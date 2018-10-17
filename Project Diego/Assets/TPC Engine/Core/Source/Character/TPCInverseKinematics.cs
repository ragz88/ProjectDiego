/* ==================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================== */

using System.Collections;
using UnityEngine;

namespace TPCEngine
{
	[System.Serializable]
	public class TPCInverseKinematics
	{
		#region [Variables are editable in the inspector]
		// Foot IK
		[SerializeField] private Transform leftFootPivot;
		[SerializeField] private Transform rightFootPivot;
		[SerializeField] private float rayRange;
		[SerializeField] private float offsetY;

		//Upper Body IK
		[SerializeField] private Transform lookTarget;
		[SerializeField] private float upperBodyIKWeight;
		[SerializeField] private float bodyIKWeight;
		[SerializeField] private float headIKWeight;
		[SerializeField] private float eyesIKWeight;
		[SerializeField] private float clampWeight;

		//Hand IK
		[SerializeField] private Transform leftHand;
		[SerializeField] private Transform rightHand;
		[SerializeField] private float handIKWeight;

		//States
		[SerializeField] private bool active;
		[SerializeField] private bool footIKActive;
		[SerializeField] private bool headIKActive;
		[SerializeField] private bool handIKActive;
        #endregion

        #region [Required variables]
		private Animator animator;
		private float leftFootWeight;
		private float rightFootWeight;
		private Transform leftFoot;
		private Transform rightFoot;
		#endregion

		#region [Functions]
		/// <summary>
		/// Init is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		/// <param name="animator">Interface to control the Mecanim animation system.</param>
		public void Init(Animator animator)
		{
			this.animator = animator;
			leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
			rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
		}

		/// <summary>
		/// Foot IK system
		/// </summary>
		public virtual void FootIK()
		{
			//If the IK is not active, set the position and rotation of the hand and head back to the original position.
			if (!active || !footIKActive)
				return;

			leftFootWeight = animator.GetFloat("IK Left Foot");
			rightFootWeight = animator.GetFloat("IK Right Foot");

			//Activate IK, set the rotation directly to the goal.
			animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
			animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
			animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
			animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);

			RaycastHit leftHit;
			if (Physics.Raycast(leftFootPivot.position, -Vector3.up, out leftHit, rayRange))
			{
				Quaternion ikRotation = Quaternion.FromToRotation(leftFoot.up, leftHit.normal) * leftFoot.rotation;
				ikRotation = new Quaternion(ikRotation.x, leftFoot.rotation.y, ikRotation.z, ikRotation.w);
				Vector3 ikPosition = new Vector3(leftFoot.position.x, leftHit.point.y, leftFoot.position.z);
				animator.SetIKPosition(AvatarIKGoal.LeftFoot, ikPosition + (Vector3.up * offsetY));
				animator.SetIKRotation(AvatarIKGoal.LeftFoot, ikRotation);
			}

			RaycastHit rightHit;
			if (Physics.Raycast(rightFootPivot.position, -Vector3.up, out rightHit, rayRange))
			{
				Quaternion ikRotation = Quaternion.FromToRotation(rightFoot.up, rightHit.normal) * rightFoot.rotation;
				ikRotation = new Quaternion(ikRotation.x, rightFoot.rotation.y, ikRotation.z, ikRotation.w);
				Vector3 ikPosition = new Vector3(rightFoot.position.x, rightHit.point.y, rightFoot.position.z);
				animator.SetIKPosition(AvatarIKGoal.RightFoot, ikPosition + (Vector3.up * offsetY));
				animator.SetIKRotation(AvatarIKGoal.RightFoot, ikRotation);
			}
		}

		/// <summary>
		/// Head IK system
		/// </summary>
		public virtual void HeadIK()
		{
			if (!active || !headIKActive)
				return;
			animator.SetLookAtWeight(upperBodyIKWeight, bodyIKWeight, headIKWeight, eyesIKWeight, clampWeight);
			animator.SetLookAtPosition(lookTarget.position);
		}

		public virtual void HandIK()
		{
			if (!active || !handIKActive)
				return;

			if (leftHand != null)
			{
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKWeight);
				animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
				animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
			}
			else
			{
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
			}

			if (rightHand != null)
			{
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKWeight);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKWeight);
				animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
				animator.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
			}
			else
			{
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
			}

		}

		/// <summary>
		/// Sets Global IK system active state
		/// 	True: IK system is active, however, subsystems can be disabled.
		/// 	False: Completely disables the IK system and all its subsystems.
		/// </summary>
		/// <param name="active">bool value</param>
		public void SetActive(bool active)
		{
			this.active = active;
		}

		/// <summary>
		/// Set foot IK system
		/// </summary>
		/// <param name="active">bool value</param>
		public void SetFootIKActive(bool active)
		{
			this.footIKActive = active;
		}

		/// <summary>
		/// Set head IK system
		/// </summary>
		/// <param name="active">bool value</param>
		public void SetHeadIKActive(bool active)
		{
			this.headIKActive = active;
		}

		/// <summary>
		/// Set Hand IK Weight
		/// </summary>
		/// <param name="state"></param>
		public void SetHandIKWeight(float weight)
		{
			handIKWeight = weight;
		}

		public void UpdateHandsTarget(Transform leftHand, Transform rightHand)
		{
			this.leftHand = (leftHand != null) ? leftHand : this.leftHand;
			this.rightHand = (rightHand != null) ? rightHand : this.rightHand;
		}
		#endregion

		#region [Properties]
		public Animator _Animator { get { return animator; } }

		/// <summary>
		/// Left Foot Pivot
		/// </summary>
		/// <value></value>
		public Transform LeftFootPivot { get { return leftFootPivot; } set { leftFootPivot = value; } }

		/// <summary>
		/// Right Foot Pivot
		/// </summary>
		/// <value></value>
		public Transform RightFootPivot { get { return rightFootPivot; } set { rightFootPivot = value; } }

		/// <summary>
		/// Left Hand Transform
		/// </summary>
		/// <value></value>
		public Transform LeftHand { get { return leftHand; } set { leftHand = value; } }

		/// <summary>
		/// Right Hand Transform
		/// </summary>
		/// <value></value>
		public Transform RightHand { get { return rightHand; } set { rightHand = value; } }

		/// <summary>
		/// Upper body look target
		/// </summary>
		/// <value></value>
		public Transform LookTarget { get { return lookTarget; } set { lookTarget = value; } }

		/// <summary>
		/// Global IK system active state
		/// </summary>
		/// <returns></returns>
		public bool Active { get { return active; } }

		/// <summary>
		/// Foot IK system active state
		/// </summary>
		/// <returns></returns>
		public bool FootIKActive { get { return footIKActive; } }

		/// <summary>
		/// Head IK system active state
		/// </summary>
		/// <returns></returns>
		public bool HeadIKActive { get { return headIKActive; } }

		/// <summary>
		/// Distance between pivots and ground
		/// </summary>
		/// <value></value>
		public float RayRange { get { return rayRange; } set { rayRange = value; } }
		
		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float HandIKWeight { get { return handIKWeight; } set { handIKWeight = value; } }
		#endregion
	}
}