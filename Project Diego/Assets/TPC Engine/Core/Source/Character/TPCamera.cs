/* ==================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================== */

using TPCEngine.Utility;
using UnityEngine;

namespace TPCEngine
{
    public class TPCamera : MonoBehaviour
    {
        #region [Variables are editable in the inspector]
        [SerializeField] private Transform target;
        [SerializeField] private float smoothCameraRotation = 12f;
        [SerializeField] private LayerMask cullingLayer = 1 << 0;
        [SerializeField] private bool lockCamera;
        [SerializeField] private float rightOffset = 0f;
        [SerializeField] private float defaultDistance = 2.5f;
        [SerializeField] private float minDistance = 0.7f;
        [SerializeField] private float maxDistance = 3.5f;
        [SerializeField] private float height = 1.4f;
        [SerializeField] private float smoothFollow = 10f;
        [SerializeField] private float xMouseSensitivity = 3f;
        [SerializeField] private float yMouseSensitivity = 3f;
        [SerializeField] private float yMinLimit = -40f;
        [SerializeField] private float yMaxLimit = 80f;
        [SerializeField] private bool scrollCameraDistance = true;
        [SerializeField] private float scrollSensitivity = 70;
        #endregion

        #region [Required variables]
        private TPCMotor characterMotor;
        private int indexList, indexLookPoint;
        private string currentStateName;
        private Transform currentTarget;
        private Vector2 movementSpeed;
        private Transform targetLookAt;
        private Vector3 currentTargetPos;
        private Vector3 lookPoint;
        private Vector3 current_cPos;
        private Vector3 desired_cPos;
        private Camera _camera;
        private float distance = 5f;
        private float mouseY = 0f;
        private float mouseX = 0f;
        private float currentHeight;
        private float cullingDistance;
        private float checkHeightRadius = 0.4f;
        private float clipPlaneMargin = 0f;
        private float forward = -1f;
        private float xMinLimit = -360f;
        private float xMaxLimit = 360f;
        private float cullingHeight = 0.2f;
        private float cullingMinDist = 0.1f;
        #endregion

        #region [Functions]
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            if (target == null)
                return;

            characterMotor = target.root.GetComponent<TPCharacter>().GetCharacteMotor();
            _camera = GetComponent<Camera>();
            currentTarget = target;
            currentTargetPos = new Vector3(currentTarget.position.x, currentTarget.position.y, currentTarget.position.z);

            targetLookAt = new GameObject("targetLookAt").transform;
            targetLookAt.position = currentTarget.position;
            targetLookAt.hideFlags = HideFlags.HideInHierarchy;
            targetLookAt.rotation = currentTarget.rotation;

            mouseY = currentTarget.eulerAngles.x;
            mouseX = currentTarget.eulerAngles.y;

            distance = defaultDistance;
            currentHeight = height;
        }

        /// <summary>
        /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void FixedUpdate()
        {
            if (target == null || targetLookAt == null) return;

            CameraMovement();
            CameraHandle();
            ScrollingCameraDistance();
            CompensateForWalls(transform.position, ref currentTargetPos);
        }

        protected virtual void CameraHandle()
        {
            float vertical = TPCInput.GetAxis("Mouse X");
            float horizontal = TPCInput.GetAxis("Mouse Y");

            RotateCamera(vertical, horizontal);

            if (characterMotor.LockMovement)
                return;
            // tranform Character direction from camera if not KeepDirection
            if (!characterMotor.KeepDirection)
                characterMotor.UpdateTargetDirection(transform);
            // rotate the character with the camera while strafing        
            if (characterMotor.IsStrafing && characterMotor.MoveAmount > 0)
                characterMotor.RotateWithAnotherTransform(transform);
        }

        /// <summary>
        /// Set the target for the camera
        /// </summary>
        /// <param name="New cursorObject"></param>
        public void SetTarget(Transform newTarget)
        {
            currentTarget = newTarget ? newTarget : target;
        }

        public void SetMainTarget(Transform newTarget)
        {
            target = newTarget;
            currentTarget = newTarget;
            mouseY = currentTarget.rotation.eulerAngles.x;
            mouseX = currentTarget.rotation.eulerAngles.y;
            Start();
        }

        /// <summary>    
        /// Convert a point in the screen in a Ray for the world
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        public Ray ScreenPointToRay(Vector3 Point)
        {
            return _camera.ScreenPointToRay(Point);
        }

        /// <summary>
        /// Camera Rotation behaviour
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void RotateCamera(float x, float y)
        {
            // free rotation 
            mouseX += x * xMouseSensitivity;
            mouseY -= y * yMouseSensitivity;

            movementSpeed.x = x;
            movementSpeed.y = -y;
            if (!lockCamera)
            {
                mouseY = Extensions.ClampAngle(mouseY, yMinLimit, yMaxLimit);
                mouseX = Extensions.ClampAngle(mouseX, xMinLimit, xMaxLimit);
            }
            else
            {
                mouseY = currentTarget.root.localEulerAngles.x;
                mouseX = currentTarget.root.localEulerAngles.y;
            }
        }

        /// <summary>
        /// Camera behaviour
        /// </summary>    
        private void CameraMovement()
        {
            if (currentTarget == null)
                return;

            distance = Mathf.Lerp(distance, defaultDistance, smoothFollow * Time.deltaTime);
            //_camera.fieldOfView = fov;
            cullingDistance = Mathf.Lerp(cullingDistance, distance, Time.deltaTime);
            var camDir = (forward * targetLookAt.forward) + (rightOffset * targetLookAt.right);

            camDir = camDir.normalized;

            var targetPos = new Vector3(currentTarget.position.x, currentTarget.position.y, currentTarget.position.z);
            currentTargetPos = targetPos;
            desired_cPos = targetPos + new Vector3(0, height, 0);
            current_cPos = currentTargetPos + new Vector3(0, currentHeight, 0);
            RaycastHit hitInfo;

            ClipPlanePoints planePoints = _camera.NearClipPlanePoints(current_cPos + (camDir * (distance)), clipPlaneMargin);
            ClipPlanePoints oldPoints = _camera.NearClipPlanePoints(desired_cPos + (camDir * distance), clipPlaneMargin);

            //Check if Height is not blocked 
            if (Physics.SphereCast(targetPos, checkHeightRadius, Vector3.up, out hitInfo, cullingHeight + 0.2f, cullingLayer))
            {
                var t = hitInfo.distance - 0.2f;
                t -= height;
                t /= (cullingHeight - height);
                cullingHeight = Mathf.Lerp(height, cullingHeight, Mathf.Clamp(t, 0.0f, 1.0f));
            }

            //Check if desired target position is not blocked       
            if (CullingRayCast(desired_cPos, oldPoints, out hitInfo, distance + 0.2f, cullingLayer, Color.blue))
            {
                distance = hitInfo.distance - 0.2f;
                if (distance < defaultDistance)
                {
                    var t = hitInfo.distance;
                    t -= cullingMinDist;
                    t /= cullingMinDist;
                    currentHeight = Mathf.Lerp(cullingHeight, height, Mathf.Clamp(t, 0.0f, 1.0f));
                    current_cPos = currentTargetPos + new Vector3(0, currentHeight, 0);
                }
            }
            else
            {
                currentHeight = height;
            }
            //Check if target position with culling height applied is not blocked
            if (CullingRayCast(current_cPos, planePoints, out hitInfo, distance, cullingLayer, Color.cyan)) distance = Mathf.Clamp(cullingDistance, 0.0f, defaultDistance);
            var lookPoint = current_cPos + targetLookAt.forward * 2f;
            lookPoint += (targetLookAt.right * Vector3.Dot(camDir * (distance), targetLookAt.right));
            targetLookAt.position = current_cPos;

            Quaternion newRot = Quaternion.Euler(mouseY, mouseX, 0);
            targetLookAt.rotation = Quaternion.Slerp(targetLookAt.rotation, newRot, smoothCameraRotation * Time.deltaTime);
            transform.position = current_cPos + (camDir * (distance));
            var rotation = Quaternion.LookRotation((lookPoint) - transform.position);

            //lookTargetOffSet = Vector3.Lerp(lookTargetOffSet, Vector3.zero, 1 * Time.fixedDeltaTime);

            //rotation.eulerAngles += rotationOffSet + lookTargetOffSet;
            transform.rotation = rotation;
            movementSpeed = Vector2.zero;
        }

        /// <summary>
        /// Custom Raycast using NearClipPlanesPoints
        /// </summary>
        /// <param name="_to"></param>
        /// <param name="from"></param>
        /// <param name="hitInfo"></param>
        /// <param name="distance"></param>
        /// <param name="cullingLayer"></param>
        /// <returns></returns>
        protected bool CullingRayCast(Vector3 from, ClipPlanePoints _to, out RaycastHit hitInfo, float distance, LayerMask cullingLayer, Color color)
        {
            bool value = false;

            if (Physics.Raycast(from, _to.LowerLeft - from, out hitInfo, distance, cullingLayer))
            {
                value = true;
                cullingDistance = hitInfo.distance;
            }

            if (Physics.Raycast(from, _to.LowerRight - from, out hitInfo, distance, cullingLayer))
            {
                value = true;
                if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
            }

            if (Physics.Raycast(from, _to.UpperLeft - from, out hitInfo, distance, cullingLayer))
            {
                value = true;
                if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
            }

            if (Physics.Raycast(from, _to.UpperRight - from, out hitInfo, distance, cullingLayer))
            {
                value = true;
                if (cullingDistance > hitInfo.distance) cullingDistance = hitInfo.distance;
            }

            return value;
        }

        /// <summary>
        /// Change distance between camera and character by scrolling mouse wheel
        /// </summary>
        protected virtual void ScrollingCameraDistance()
        {
            if (!scrollCameraDistance)
                return;

            float mouseWheelAxis = TPCInput.GetAxis("Mouse Wheel") * scrollSensitivity;
            float scrolledDistance = defaultDistance + (mouseWheelAxis * Time.deltaTime);
            defaultDistance = Mathf.Clamp(scrolledDistance, minDistance, maxDistance);
        }

        /// <summary>
        /// Camera direction relative to the target
        /// </summary>
        /// <value>float</value>
        public float DirectionRelativeTarget
        {
            get
            {
                return Vector3.Angle(transform.forward, target.forward);
            }
        }

        public Transform Target
        {
            get
            {
                return target;
            }

            set
            {
                target = value;
            }
        }

        public float SmoothCameraRotation
        {
            get
            {
                return smoothCameraRotation;
            }

            set
            {
                smoothCameraRotation = value;
            }
        }

        public LayerMask CullingLayer
        {
            get
            {
                return cullingLayer;
            }

            set
            {
                cullingLayer = value;
            }
        }

        public bool LockCamera
        {
            get
            {
                return lockCamera;
            }

            set
            {
                lockCamera = value;
            }
        }

        public float RightOffset
        {
            get
            {
                return rightOffset;
            }

            set
            {
                rightOffset = value;
            }
        }

        public float DefaultDistance
        {
            get
            {
                return defaultDistance;
            }

            set
            {
                defaultDistance = value;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }

            set
            {
                height = value;
            }
        }

        public float SmoothFollow
        {
            get
            {
                return smoothFollow;
            }

            set
            {
                smoothFollow = value;
            }
        }

        public float XMouseSensitivity
        {
            get
            {
                return xMouseSensitivity;
            }

            set
            {
                xMouseSensitivity = value;
            }
        }

        public float YMouseSensitivity
        {
            get
            {
                return yMouseSensitivity;
            }

            set
            {
                yMouseSensitivity = value;
            }
        }

        public float YMinLimit
        {
            get
            {
                return yMinLimit;
            }

            set
            {
                yMinLimit = value;
            }
        }

        public float YMaxLimit
        {
            get
            {
                return yMaxLimit;
            }

            set
            {
                yMaxLimit = value;
            }
        }

        public float CheckHeightRadius
        {
            get
            {
                return checkHeightRadius;
            }

            set
            {
                checkHeightRadius = value;
            }
        }

        public float ClipPlaneMargin
        {
            get
            {
                return clipPlaneMargin;
            }

            set
            {
                clipPlaneMargin = value;
            }
        }

        public float Forward
        {
            get
            {
                return forward;
            }

            set
            {
                forward = value;
            }
        }

        public float XMinLimit
        {
            get
            {
                return xMinLimit;
            }

            set
            {
                xMinLimit = value;
            }
        }

        public float XMaxLimit
        {
            get
            {
                return xMaxLimit;
            }

            set
            {
                xMaxLimit = value;
            }
        }

        public float CullingHeight
        {
            get
            {
                return cullingHeight;
            }

            set
            {
                cullingHeight = value;
            }
        }

        public float CullingMinDist
        {
            get
            {
                return cullingMinDist;
            }

            set
            {
                cullingMinDist = value;
            }
        }

        public bool ScrollDistance
        {
            get
            {
                return scrollCameraDistance;
            }

            set
            {
                scrollCameraDistance = value;
            }
        }

        public float ScrollSensitivity
        {
            get
            {
                return scrollSensitivity;
            }

            set
            {
                scrollSensitivity = value;
            }
        }

        public float MinDistance
        {
            get
            {
                return minDistance;
            }

            set
            {
                minDistance = value;
            }
        }

        public float MaxDistance
        {
            get
            {
                return maxDistance;
            }

            set
            {
                maxDistance = value;
            }
        }


        void CompensateForWalls(Vector3 fromObject, ref Vector3 toTarget)
        {
            Debug.DrawLine(fromObject, toTarget, Color.cyan);
            RaycastHit wallHit = new RaycastHit();
            if (Physics.Linecast(fromObject, toTarget, out wallHit))
            {
                Debug.DrawRay(wallHit.point, Vector3.left, Color.red);
                toTarget = new Vector3(wallHit.point.x, toTarget.y, wallHit.point.z);

            }
        }
        #endregion
    }
}