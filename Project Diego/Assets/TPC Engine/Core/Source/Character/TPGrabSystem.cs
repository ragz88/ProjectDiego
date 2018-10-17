/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using System.Collections;
using UnityEngine;

namespace TPCEngine
{
	/// <summary>
	/// Third person grab system
	/// </summary>
	public class TPGrabSystem : MonoBehaviour
	{
        #region [Variables are editable in the inspector]
        [SerializeField] private float grabRange;
		[SerializeField] private float spring = 50.0f;
		[SerializeField] private float damper = 5.0f;
		[SerializeField] private float drag = 10.0f;
		[SerializeField] private float angularDrag = 5.0f;
		[SerializeField] private float distance = 0.2f;
		#endregion

		#region [Required variables]
		private GrabObject grabObject;
		private TPCharacter character;
        private Transform _camera;
        private TPCInverseKinematics inverseKinematics;
		private CharacterHealth characterHealth;
		private LayerMask layerMask;
		private bool isGrabbing;

		private SpringJoint springJoint;
		#endregion

		#region [Functions]
		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		protected virtual void Start()
		{
			//Init required components
			character = GetComponent<TPCharacter>();
            _camera = character.GetCamera().transform;
			inverseKinematics = character.GetInverseKinematics();
			characterHealth = GetComponent<CharacterHealth>();

			//Ignore player layer for grabbing raycast
			layerMask = ~(1 << LayerMask.NameToLayer("Player"));

			//Add and configuring Spring Joint
			if (!springJoint)
			{
				GameObject go = new GameObject("Rigidbody Dragger");
				gameObject.transform.parent = transform;
				Rigidbody body = go.AddComponent<Rigidbody>();
				springJoint = go.AddComponent<SpringJoint>();
				body.isKinematic = true;
			}

		}

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		/// <summary>
		protected virtual void Update()
		{
			if (TPCInput.GetButtonDown("Grab") && !isGrabbing)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(_camera.position, _camera.forward, out raycastHit, grabRange, layerMask))
				{
					grabObject = raycastHit.transform.GetComponent<GrabObject>();
					if (raycastHit.rigidbody != null && !raycastHit.rigidbody.isKinematic && grabObject != null)
					{
						grabObject.transform.rotation = Quaternion.LookRotation(character.transform.forward);
						springJoint.transform.position = raycastHit.point;
						springJoint.anchor = Vector3.zero;
						springJoint.spring = spring;
						springJoint.damper = damper;
						springJoint.maxDistance = distance;
						springJoint.connectedBody = raycastHit.rigidbody;
						isGrabbing = true;
						StartCoroutine(DragObject(grabObject.Range));
					}
				}
			}
			else if ((TPCInput.GetButtonDown("Grab") && isGrabbing) || !characterHealth.IsAlive)
			{
				inverseKinematics.SetHandIKWeight(0);
				grabObject = null;
				isGrabbing = false;
			}
		}

		private IEnumerator DragObject(float distance)
		{
			inverseKinematics.SetHandIKWeight(1);
			var oldDrag = springJoint.connectedBody.drag;
			var oldAngularDrag = springJoint.connectedBody.angularDrag;
			springJoint.connectedBody.drag = drag;
			springJoint.connectedBody.angularDrag = angularDrag;
			while (isGrabbing)
			{
				springJoint.transform.rotation = Quaternion.LookRotation(character.transform.forward);
				springJoint.transform.position = character.transform.position + (character.transform.forward * distance) + (Vector3.up * grabObject.HeightOffset);
				inverseKinematics.LeftHand = grabObject.LeftHandPivot;
				inverseKinematics.RightHand = grabObject.RighthandPivot;
				yield return null;
			}
			if (springJoint.connectedBody)
			{
				springJoint.connectedBody.drag = oldDrag;
				springJoint.connectedBody.angularDrag = oldAngularDrag;
				springJoint.connectedBody = null;
			}
		}
		#endregion

		#region [Properties]
		/// <summary>
		/// Distance between grab object and character in which character can grab objects
		/// </summary>
		/// <value>float</value>
		public float GrabRange { get { return grabRange; } set { grabRange = value; } }

		/// <summary>
		/// Character transform
		/// </summary>
		/// <value>Transform</value>
		public TPCharacter Character { get { return character; } }

		/// <summary>
		/// Third person camera
		/// </summary>
		/// <value>Transform</value>
		public Transform Camera { get { return _camera; } set { _camera = value; } }

		/// <summary>
		/// Character Inverse Kinematics instance
		/// </summary>
		/// <value>TPCInverseKinematics</value>
		public TPCInverseKinematics InverseKinematics { get { return inverseKinematics; } }
		#endregion
	}
}