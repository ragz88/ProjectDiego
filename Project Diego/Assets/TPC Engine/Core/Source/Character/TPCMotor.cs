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
	/// Character locomotion type	
	/// 	Description:	
	/// 		Strafe - The character moves forward, backward, left, right with a specific animation.
	/// 		Free - The character moves in all directions, with the direction of the body in the direction of movement.
	/// </summary>
	public enum LocomotionType { Strafe, Free }

	/// <summary>
	/// TPCMototr class handle movement system
	/// </summary>
	[System.Serializable]
	public class TPCMotor
	{
		#region [Variables are editable in the inspector]
		[SerializeField] private LocomotionType locomotionType = LocomotionType.Strafe;
		[SerializeField] private bool lockMovement = false;
		[SerializeField] private float freeRotationSpeed = 10f;
		[SerializeField] private float strafeRotationSpeed = 10f;

		[SerializeField] private bool jumpAirControl = true;
		[SerializeField] private float jumpTimer = 0.3f;
		[SerializeField] private float jumpForward = 3f;
		[SerializeField] private float jumpHeight = 4f;
		[SerializeField] private AudioClip jumpClip;

		[SerializeField] private bool useRootMotion = false;
		[SerializeField] private bool keepDirection;
		[SerializeField] private float freeWalkSpeed = 2.5f;
		[SerializeField] private float freeRunningSpeed = 3f;
		[SerializeField] private float freeSprintSpeed = 4f;
		[SerializeField] private float strafeWalkSpeed = 2.5f;
		[SerializeField] private float strafeRunningSpeed = 3f;
		[SerializeField] private float strafeSprintSpeed = 4f;
		[SerializeField] private float freeCrouchWalkSpeed = 2.5f;
		[SerializeField] private float freeCrouchRunningSpeed = 3f;
		[SerializeField] private float freeCrouchSprintSpeed = 4f;
		[SerializeField] private float strafeCrouchWalkSpeed = 2.5f;
		[SerializeField] private float strafeCrouchRunningSpeed = 3f;
		[SerializeField] private float strafeCrouchSprintSpeed = 4f;
		[SerializeField] private float crouchHeight;
		[SerializeField] private float crouchSmooth;

		[SerializeField] private float stepOffsetEnd = 0.45f;
		[SerializeField] private float stepOffsetStart = 0.05f;
		[SerializeField] private float stepSmooth = 4f;
		[SerializeField] private float slopeLimit = 45f;
		[SerializeField] private float extraGravity = -10f;

		[SerializeField] private LayerMask groundLayer = 1 << 0;
		[SerializeField] private float groundMinDistance = 0.2f;
		[SerializeField] private float groundMaxDistance = 0.5f;
		#endregion

		#region [Action variables]
		private bool isGrounded;
		private bool isJumping;
		private bool isSprinting;
		private bool isCrouching;
		private bool isStrafing;
		private bool isSliding;
		#endregion

		#region [Required variables]
		private Transform transform;
		private RaycastHit groundHit;
		private Vector3 targetDirection;
		private Quaternion targetRotation;
		private Quaternion freeRotation;
		private float groundDistance;
		private float jumpCounter;
		private Animator animator;
		private Rigidbody rigidbody;
		private PhysicMaterial maxFrictionPhysics;
		private PhysicMaterial frictionPhysics;
		private PhysicMaterial slippyPhysics;
		private CapsuleCollider capsuleCollider;
		private float colliderHeight;
		private float speed;
		private float direction;
		private float verticalVelocity;
		private float velocity;
		private float wasColliderVelocityCenter;
		#endregion

		#region [Functions]
		/// <summary>
		/// Init is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		public void Init(Transform transform, Animator animator, Rigidbody rigidbody, CapsuleCollider capsuleCollider)
		{
			this.transform = transform;
			this.animator = animator;
			this.rigidbody = rigidbody;
			this.capsuleCollider = capsuleCollider;
			colliderHeight = capsuleCollider.height;
			wasColliderVelocityCenter = capsuleCollider.center.y;
			InitPhysicMaterial();
		}

		/// <summary>
		/// UpdateMotor is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		public virtual void UpdateMotor()
		{
			CheckGround();
			if (lockMovement)
				return;
			ControlJumpBehaviour();
			ControlLocomotion();
			JumpHandler();
			SprintHandler();
			CrouchHandler();
		}

		private void ControlLocomotion()
		{
			if (FreeLocomotionConditions)
				FreeMovement();
			else
				StrafeMovement();
		}

		public virtual void StrafeMovement()
		{
			speed = Mathf.Clamp(TPCInput.GetAxis("Vertical"), -1, 1);
			direction = Mathf.Clamp(TPCInput.GetAxis("Horizontal"), -1, 1);
		}

		public virtual void FreeMovement()
		{
			// Set speed to both vertical and horizontal inputs
			speed = MoveAmount;

			if (MoveAmount != 0 && targetDirection.magnitude > 0.1f)
			{
				Vector3 lookDirection = targetDirection.normalized;
				freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
				float diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
				float eulerY = transform.eulerAngles.y;

				// Apply free directional rotation while not turning180 animations
				if (isGrounded || (!isGrounded && jumpAirControl))
				{
					if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
					Vector3 euler = new Vector3(transform.eulerAngles.x, eulerY, transform.eulerAngles.z);
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(euler), freeRotationSpeed * Time.deltaTime);
				}
			}
		}

		public virtual void ControlSpeed(float velocity)
		{
			if (Time.deltaTime == 0)
				return;

			this.velocity = velocity;

			if (useRootMotion)
			{
				Vector3 v = (animator.deltaPosition * (velocity > 0 ? velocity : 1f)) / Time.deltaTime;
				v.y = rigidbody.velocity.y;
				rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, v, 20f * Time.deltaTime);
			}
			else
			{
                Vector3 velY = transform.forward * velocity * Mathf.Clamp(speed, -1, 1);
				velY.y = rigidbody.velocity.y;
				Vector3 velX = transform.right * velocity * Mathf.Clamp(direction, -1, 1);
				velX.x = rigidbody.velocity.x;

				if (isStrafing)
				{
					Vector3 v = (transform.TransformDirection(new Vector3(TPCInput.GetAxis("Horizontal"), 0, TPCInput.GetAxis("Vertical"))) * (velocity > 0 ? velocity : 1f));
					v.y = rigidbody.velocity.y;
					rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, v, 20f * Time.deltaTime);
				}
				else
				{
					rigidbody.velocity = velY;
					rigidbody.AddForce(transform.forward * (velocity * Mathf.Clamp(speed, -1, 1)) * Time.deltaTime, ForceMode.VelocityChange);
				}
			}
		}

		public virtual void SpeedObserver()
		{
			if (isStrafing)
			{
				float strafeSpeed = (isSprinting ? 1.5f : 1f) * Mathf.Clamp(MoveAmount, 0f, 1f);
				if (!isCrouching)
				{
					if (strafeSpeed <= 0.5f)
						ControlSpeed(strafeWalkSpeed);
					else if (strafeSpeed > 0.5f && strafeSpeed <= 1f)
						ControlSpeed(strafeRunningSpeed);
					else
						ControlSpeed(strafeSprintSpeed);
				}
				else if (isCrouching)
				{
					if (strafeSpeed <= 0.5f)
						ControlSpeed(strafeCrouchWalkSpeed);
					else if (strafeSpeed > 0.5f && strafeSpeed <= 1f)
						ControlSpeed(strafeCrouchRunningSpeed);
					else
						ControlSpeed(strafeCrouchSprintSpeed);
				}
			}
			else if (!isStrafing)
			{
				if (!isCrouching)
				{
					if (speed <= 0.5f)
						ControlSpeed(freeWalkSpeed);
					else if (Speed > 0.5 && Speed <= 1f)
						ControlSpeed(freeRunningSpeed);
					else
						ControlSpeed(freeSprintSpeed);
				}
				else if (isCrouching)
				{
					if (speed <= 0.5f)
						ControlSpeed(freeCrouchWalkSpeed);
					else if (Speed > 0.5 && Speed <= 1f)
						ControlSpeed(freeCrouchRunningSpeed);
					else
						ControlSpeed(freeCrouchSprintSpeed);
				}
			}
		}

		private void SprintHandler()
		{
			isSprinting = TPCInput.GetButton("Sprint");

			if (MoveAmount > 0 && isSprinting)
            {
                if (speed > 0)
                    speed = 2;
                else if (speed < 0)
                    speed = -2;

                if (direction > 0)
                    direction = 2;
                else if (direction < 0)
                    direction = -2;
            }
		}

		public void ControlJumpBehaviour()
		{
			if (!isJumping)
				return;

			jumpCounter -= Time.deltaTime;
			if (jumpCounter <= 0)
			{
				jumpCounter = 0;
				isJumping = false;
			}
			// apply extra force to the jump height   
			Vector3 vel = rigidbody.velocity;
			vel.y = jumpHeight;
			rigidbody.velocity = vel;
		}

		public virtual void AirControl()
		{
			if (isGrounded) return;
			if (!JumpFwdCondition) return;

			Vector3 velY = transform.forward * jumpForward * Mathf.Clamp(speed, -1, 1);
			velY.y = rigidbody.velocity.y;
			Vector3 velX = transform.right * jumpForward * Mathf.Clamp(direction, -1, 1);
			velX.x = rigidbody.velocity.x;

			if (jumpAirControl)
			{
				if (isStrafing)
				{
					rigidbody.velocity = new Vector3(velX.x, velY.y, rigidbody.velocity.z);
					Vector3 vel = transform.forward * (jumpForward * Mathf.Clamp(speed, -1, 1)) + transform.right * (jumpForward * Mathf.Clamp(direction, -1, 1));
					rigidbody.velocity = new Vector3(vel.x, rigidbody.velocity.y, vel.z);
				}
				else
				{
					Vector3 vel = transform.forward * (jumpForward * Mathf.Clamp(speed, -1, 1));
					rigidbody.velocity = new Vector3(vel.x, rigidbody.velocity.y, vel.z);
				}
			}
			else
			{
				Vector3 vel = transform.forward * (jumpForward);
				rigidbody.velocity = new Vector3(vel.x, rigidbody.velocity.y, vel.z);
			}
		}

		public virtual void Jump()
		{
			// conditions to do this action
			bool jumpConditions = isGrounded && !isJumping;
			// return if jumpCondigions is false
			if (!jumpConditions) return;
			// trigger jump behaviour
			jumpCounter = jumpTimer;
			isJumping = true;
			// trigger jump animations            
			if (rigidbody.velocity.magnitude < 1)
				animator.CrossFadeInFixedTime("Jump", 0.1f);
			else
				animator.CrossFadeInFixedTime("JumpMove", 0.2f);
		}

		private void JumpHandler()
		{
			if (TPCInput.GetButtonDown("Jump"))
				Jump();
		}

		public bool JumpFwdCondition
		{
			get
			{
				Vector3 p1 = transform.position + capsuleCollider.center + Vector3.up * -capsuleCollider.height * 0.5F;
				Vector3 p2 = p1 + Vector3.up * capsuleCollider.height;
				return Physics.CapsuleCastAll(p1, p2, capsuleCollider.radius * 0.5f, transform.forward, 0.6f, groundLayer).Length == 0;
			}
		}

		protected virtual void CrouchHandler()
		{
			float capsuleHeight = colliderHeight;
			float capsuleVelociy = wasColliderVelocityCenter;

			if (TPCInput.GetButtonDown("Crouch") && !isCrouching)
			{
				isCrouching = !isCrouching;
			}
			else if(TPCInput.GetButtonDown("Crouch") && isCrouching)
			{
				if (!Physics.Raycast(transform.position, Vector3.up, colliderHeight))
				{
					isCrouching = !isCrouching;
				}
			}

			if (isCrouching)
			{
				capsuleHeight = colliderHeight * crouchHeight;
				capsuleVelociy = wasColliderVelocityCenter * crouchHeight;
			}

			float lastCapsuleHeight = capsuleCollider.height;
			capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, capsuleHeight, crouchSmooth * Time.deltaTime);
			float fixedVelocity = Mathf.Lerp(capsuleCollider.center.y, capsuleVelociy, crouchSmooth * Time.deltaTime);
			capsuleCollider.center = new Vector3(0, fixedVelocity, 0);
			float fixedVerticalPosition = Mathf.Lerp(transform.position.y, transform.position.y + (capsuleCollider.height - lastCapsuleHeight) / 2.0f, crouchSmooth * Time.deltaTime);
			transform.position = new Vector3(transform.position.x, fixedVerticalPosition, transform.position.z);
		}

		private void CheckGround()
		{
			CheckGroundDistance();

			// change the physics material to very slip when not grounded or maxFriction when is
			if (isGrounded && MoveAmount == 0)
				capsuleCollider.material = maxFrictionPhysics;
			else if (isGrounded && MoveAmount != 0)
				capsuleCollider.material = frictionPhysics;
			else
				capsuleCollider.material = slippyPhysics;

			float magVel = (float) System.Math.Round(new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z).magnitude, 2);
			magVel = Mathf.Clamp(magVel, 0, 1);

			float groundCheckDistance = groundMinDistance;
			if (magVel > 0.25f) groundCheckDistance = groundMaxDistance;

			// clear the checkground to free the character to attack on air                
			bool onStep = StepOffset();

			if (groundDistance <= 0.05f)
			{
				isGrounded = true;
				Sliding();
			}
			else
			{
				if (groundDistance >= groundCheckDistance)
				{
					isGrounded = false;
					// check vertical velocity
					verticalVelocity = rigidbody.velocity.y;
					// apply extra gravity when falling
					if (!onStep && !isJumping)
						rigidbody.AddForce(transform.up * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
				}
				else if (!onStep && !isJumping)
				{
					rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
				}
			}
		}

		private void CheckGroundDistance()
		{
			if (capsuleCollider != null)
			{
				// radius of the SphereCast
				float radius = capsuleCollider.radius * 0.9f;
				float dist = 10f;
				// position of the SphereCast origin starting at the base of the capsule
				Vector3 pos = transform.position + Vector3.up * (capsuleCollider.radius);
				// ray for RayCast
				Ray ray1 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
				// ray for SphereCast
				Ray ray2 = new Ray(pos, -Vector3.up);
				// raycast for check the ground distance
				if (Physics.Raycast(ray1, out groundHit, colliderHeight / 2 + 2f, groundLayer))
					dist = transform.position.y - groundHit.point.y;
				// sphere cast around the base of the capsule to check the ground distance
				if (Physics.SphereCast(ray2, radius, out groundHit, capsuleCollider.radius + 2f, groundLayer))
				{
					// check if sphereCast distance is small than the ray cast distance
					if (dist > (groundHit.distance - capsuleCollider.radius * 0.1f))
						dist = (groundHit.distance - capsuleCollider.radius * 0.1f);
				}
				groundDistance = (float) System.Math.Round(dist, 2);
			}
		}

		private float GroundAngle()
		{
			float groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
			return groundAngle;
		}

		private void Sliding()
		{
			bool onStep = StepOffset();
			float groundAngleTwo = 0f;
			RaycastHit hitinfo;
			Ray ray = new Ray(transform.position, -transform.up);

			if (Physics.Raycast(ray, out hitinfo, 1f, groundLayer))
			{
				groundAngleTwo = Vector3.Angle(Vector3.up, hitinfo.normal);
			}

			if (GroundAngle() > slopeLimit + 1f && GroundAngle() <= 85 &&
				groundAngleTwo > slopeLimit + 1f && groundAngleTwo <= 85 &&
				groundDistance <= 0.05f && !onStep)
			{
				isSliding = true;
				isGrounded = false;
				var slideVelocity = (GroundAngle() - slopeLimit) * 2f;
				slideVelocity = Mathf.Clamp(slideVelocity, 0, 10);
				rigidbody.velocity = new Vector3(rigidbody.velocity.x, -slideVelocity, rigidbody.velocity.z);
			}
			else
			{
				isSliding = false;
				isGrounded = true;
			}
		}

		private bool StepOffset()
		{
			if (Mathf.Sqrt(MoveAmount) < 0.1 || !isGrounded)
                return false;

			RaycastHit _hit = new RaycastHit();
			Vector3 _movementDirection = isStrafing && MoveAmount > 0 ? (transform.right * TPCInput.GetAxis("Horizontal") + transform.forward * TPCInput.GetAxis("Vertical")).normalized : transform.forward;
			Ray rayStep = new Ray((transform.position + new Vector3(0, stepOffsetEnd, 0) + _movementDirection * ((capsuleCollider).radius + 0.05f)), Vector3.down);

			if (Physics.Raycast(rayStep, out _hit, stepOffsetEnd - stepOffsetStart, groundLayer) && !_hit.collider.isTrigger)
			{
				if (_hit.point.y >= (transform.position.y) && _hit.point.y <= (transform.position.y + stepOffsetEnd))
				{
					float _speed = isStrafing ? Mathf.Clamp(MoveAmount, 0, 1) : Mathf.Clamp(speed, -1, 1);
					Vector3 velocityDirection = isStrafing ? (_hit.point - transform.position) : (_hit.point - transform.position).normalized;
					rigidbody.velocity = velocityDirection * stepSmooth * (_speed * (velocity > 1 ? velocity : 1));
					return true;
				}
			}
			return false;
		}

		public virtual void RotateToTarget(Transform target)
		{
			Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
			Vector3 newPos = new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
			targetRotation = Quaternion.Euler(newPos);
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, strafeRotationSpeed * Time.deltaTime);
		}

		/// <summary>
		/// Update the targetDirection variable using referenceTransform or just input.Rotate by word  the referenceDirection
		/// </summary>
		/// <param name="referenceTransform"></param>
		public virtual void UpdateTargetDirection(Transform referenceTransform = null)
		{
			if (referenceTransform)
			{
				Vector3 forward = keepDirection ? referenceTransform.forward : referenceTransform.TransformDirection(Vector3.forward);
				forward.y = 0;

				forward = keepDirection ? forward : referenceTransform.TransformDirection(Vector3.forward);
				forward.y = 0; //set to 0 because of referenceTransform rotation on the X axis

				//get the right-facing direction of the referenceTransform
				Vector3 right = keepDirection ? referenceTransform.right : referenceTransform.TransformDirection(Vector3.right);

				// determine the direction the player will face based on input and the referenceTransform's right and forward directions
				targetDirection = TPCInput.GetAxis("Horizontal") * right + TPCInput.GetAxis("Vertical") * forward;
			}
			else
			{
				targetDirection = keepDirection ? targetDirection : new Vector3(TPCInput.GetAxis("Horizontal"), 0, TPCInput.GetAxis("Vertical"));
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="referenceTransform"></param>
		public virtual void RotateWithAnotherTransform(Transform referenceTransform)
		{
			Vector3 newRotation = new Vector3(transform.eulerAngles.x, referenceTransform.eulerAngles.y, transform.eulerAngles.z);
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation), strafeRotationSpeed * Time.fixedDeltaTime);
			targetRotation = transform.rotation;
		}

		public bool FreeLocomotionConditions
		{
			get
			{
				if (locomotionType.Equals(LocomotionType.Strafe))
					isStrafing = true;
				return !isStrafing && !locomotionType.Equals(LocomotionType.Strafe) || locomotionType.Equals(LocomotionType.Free);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void InitPhysicMaterial()
		{
			// slides the character through walls and edges
			frictionPhysics = new PhysicMaterial();
			frictionPhysics.name = "frictionPhysics";
			frictionPhysics.staticFriction = .25f;
			frictionPhysics.dynamicFriction = .25f;
			frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

			// prevents the collider from slipping on ramps
			maxFrictionPhysics = new PhysicMaterial();
			maxFrictionPhysics.name = "maxFrictionPhysics";
			maxFrictionPhysics.staticFriction = 1f;
			maxFrictionPhysics.dynamicFriction = 1f;
			maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

			// air physics 
			slippyPhysics = new PhysicMaterial();
			slippyPhysics.name = "slippyPhysics";
			slippyPhysics.staticFriction = 0f;
			slippyPhysics.dynamicFriction = 0f;
			slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float MoveAmount
		{
			get
			{
				return Mathf.Clamp01(Mathf.Abs(TPCInput.GetAxis("Vertical")) + Mathf.Abs(TPCInput.GetAxis("Horizontal")));
			}
		}
		#endregion

		#region [Properties]

		public Transform CharacterTransform { get { return transform; } }

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float Speed { get { return speed; } }

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float Direction { get { return direction; } }

		public bool LockMovement { get { return lockMovement; } set { lockMovement = value; } }

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float FreeWalkSpeed
		{
			get { return freeWalkSpeed; } set { freeWalkSpeed = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float FreeRunningSpeed
		{
			get { return freeRunningSpeed; } set { freeRunningSpeed = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float FreeSprintSpeed
		{
			get
			{
				return freeSprintSpeed;
			}
			set
			{
				freeSprintSpeed = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float StrafeWalkSpeed
		{
			get
			{
				return strafeWalkSpeed;
			}
			set
			{
				strafeWalkSpeed = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float StrafeRunningSpeed
		{
			get
			{
				return strafeRunningSpeed;
			}
			set
			{
				strafeRunningSpeed = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float StrafeSprintSpeed
		{
			get
			{
				return strafeSprintSpeed;
			}
			set
			{
				strafeSprintSpeed = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float FreeCrouchWalkSpeed
		{
			get
			{
				return freeCrouchWalkSpeed;
			}
			set
			{
				freeCrouchWalkSpeed = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float FreeCrouchRunningSpeed
		{
			get
			{
				return freeCrouchRunningSpeed;
			}
			set
			{
				freeCrouchRunningSpeed = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float FreeCrouchSprintSpeed
		{
			get
			{
				return freeCrouchSprintSpeed;
			}
			set
			{
				freeCrouchSprintSpeed = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float StrafeCrouchWalkSpeed
		{
			get
			{
				return strafeCrouchWalkSpeed;
			}
			set
			{
				strafeCrouchWalkSpeed = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float StrafeCrouchRunningSpeed
		{
			get
			{
				return strafeCrouchRunningSpeed;
			}
			set
			{
				strafeCrouchRunningSpeed = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float StrafeCrouchSprintSpeed
		{
			get
			{
				return strafeCrouchSprintSpeed;
			}
			set
			{
				strafeCrouchSprintSpeed = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float VerticalVelocity
		{
			get
			{
				return verticalVelocity;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public float GroundDistance
		{
			get
			{
				return groundDistance;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public bool IsGrounded
		{
			get
			{
				return isGrounded;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public bool IsCrouching
		{
			get
			{
				return isCrouching;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public bool IsJumping
		{
			get
			{
				return isJumping;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public bool IsSliding
		{
			get
			{
				return isSliding;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public bool IsSprinting
		{
			get
			{
				return isSprinting;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public bool IsStrafing
		{
			get
			{
				return isStrafing;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public bool KeepDirection
		{
			get
			{
				return keepDirection;
			}
		}

		public AudioClip JumpClip { get { return jumpClip; } set { jumpClip = value; } }
		#endregion
	}
}