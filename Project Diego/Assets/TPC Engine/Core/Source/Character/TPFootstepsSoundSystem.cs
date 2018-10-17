/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using System.Collections;
using TPCEngine.Utility;
using UnityEngine;

namespace TPCEngine
{
	/// <summary>
	/// Volume relative to speed
	/// </summary>
	[System.Serializable]
	public struct FootstepsVolume
	{
		public float volume;
		public float minSpeed;
		public float maxSpeed;
	}

	/// <summary>
	/// The sound of steps relative to the material of the object
	/// </summary>
	[System.Serializable]
	public struct FootstepsMaterial
	{
		public string materialName;
		public AudioClip[] footstepsClips;
		public AudioClip[] landingClips;
	}

	/// <summary>
	/// Third person character footsteps sound system
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class TPFootstepsSoundSystem : MonoBehaviour
	{
		#region [Variables are editable in the inspector]
		[SerializeField] private Transform leftFoot;
		[SerializeField] private Transform rightFoot;
		[SerializeField] private float defaultVolume;
		[SerializeField] private FootstepsVolume[] footstepsVolumes;
		[SerializeField] private FootstepsMaterial[] footstepsMaterials;
		[SerializeField] private float rayRange;
		#endregion

		#region [Required variables]
		private AudioSource audioSource;
		private TPCMotor characterMotor;
		private bool leftFootIsPlayed;
		private bool rightFootIsPlayed;
		private bool isLanding;
		#endregion

		#region [Functions]
		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		protected virtual void Start()
		{
			audioSource = GetComponent<AudioSource>();
			characterMotor = GetComponent<TPCharacter>().GetCharacteMotor();
		}

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		protected virtual void Update()
		{
			ProcessLanding();
			if (characterMotor.MoveAmount > 0)
			{
				VolumeHandler();
				ProcessingSteps();
			}
			else
			{
				audioSource.volume = defaultVolume;
			}
		}

		/// <summary>
		/// Handling volume relative to the character speed
		/// </summary>
		protected virtual void VolumeHandler()
		{
			for (int i = 0; i < footstepsVolumes.Length; i++)
			{
				if (characterMotor.MoveAmount >= footstepsVolumes[i].minSpeed && characterMotor.MoveAmount < footstepsVolumes[i].maxSpeed)
				{
					audioSource.volume = footstepsVolumes[i].volume;
					break;
				}
			}
		}

		/// <summary>
		/// Handling character steps 
		/// </summary>
		protected virtual void ProcessingSteps()
		{
			RaycastHit leftFootRayHit;

			if (Physics.Raycast(leftFoot.position, -leftFoot.up, out leftFootRayHit, rayRange))
			{
				if (!leftFootIsPlayed)
					PlayStepSound((leftFootRayHit.collider.sharedMaterial != null) ? leftFootRayHit.collider.sharedMaterial.name : "Default");
				leftFootIsPlayed = true;
			}
			else
			{
				leftFootIsPlayed = false;
			}

			RaycastHit rigthFootRayHit;
			if (Physics.Raycast(rightFoot.position, -rightFoot.up, out rigthFootRayHit, rayRange))
			{
				if (!rightFootIsPlayed)
					PlayStepSound((rigthFootRayHit.collider.sharedMaterial != null) ? rigthFootRayHit.collider.sharedMaterial.name : "Default");
				rightFootIsPlayed = true;
			}
			else
			{
				rightFootIsPlayed = false;
			}
		}

		/// <summary>
		/// Processing character landing
		/// </summary>
		protected virtual void ProcessLanding()
		{
			if (!characterMotor.IsGrounded)
				isLanding = true;

			if (isLanding && characterMotor.IsGrounded)
			{
				StartCoroutine(PlayLandSound());
				isLanding = false;
			}
		}

		/// <summary>
		/// Play step sound
		/// </summary>
		/// <param name="material"></param>
		public virtual void PlayStepSound(string material = "Default")
		{
			for (int i = 0; i < footstepsMaterials.Length; i++)
			{
				if (footstepsMaterials[i].materialName == material)
				{
					int randomClip = Random.Range(0, footstepsMaterials[i].footstepsClips.Length);
					audioSource.PlayOneShot(footstepsMaterials[i].footstepsClips[randomClip]);
					break;
				}
			}
		}

		/// <summary>
		/// Play step sound
		/// </summary>
		/// <param name="material"></param>
		public virtual IEnumerator PlayLandSound()
		{
			yield return null;
			RaycastHit landHit;
			if (Physics.Raycast(characterMotor.CharacterTransform.position, -Vector3.up, out landHit, 100.0f))
			{
				string material = (landHit.collider.sharedMaterial != null) ? landHit.collider.sharedMaterial.name : "Default";
				for (int i = 0; i < footstepsMaterials.Length; i++)
				{
					if (footstepsMaterials[i].materialName == material)
					{
						int randomClip = Random.Range(0, footstepsMaterials[i].landingClips.Length);
						audioSource.PlayOneShot(footstepsMaterials[i].landingClips[randomClip]);
					}
				}
			}
			yield break;
		}

		#endregion

		#region [Properties]
		/// <summary>
		/// Character Left Foot
		/// </summary>
		/// <value></value>
		public Transform LeftFoot { get { return leftFoot; } set { leftFoot = value; } }

		/// <summary>
		/// Character Right Foot
		/// </summary>
		/// <value></value>
		public Transform RightFoot { get { return rightFoot; } set { rightFoot = value; } }

		/// <summary>
		/// Volume relative to the character speed
		/// </summary>
		/// <value></value>
		public FootstepsVolume[] FootstepsVolumes { get { return footstepsVolumes; } set { footstepsVolumes = value; } }

		/// <summary>
		/// The sound of steps relative to the material of the object
		/// </summary>
		/// <value></value>
		public FootstepsMaterial[] FootstepsMaterials { get { return footstepsMaterials; } set { footstepsMaterials = value; } }

		public float RayRange { get { return rayRange; } set { rayRange = value; } }
		#endregion
	}
}