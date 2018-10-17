/*	-----------------------------------------------------------
	|	Author:		Tamerlan Shakirov
	|	Publisher:	Infinite Dawn
	|	Project:	TPC Engine
	|	Copyright © Tamerlan Shakirov 2018 All rights reserved.
	----------------------------------------------------------- */

using TPCEngine.Utility;
using UnityEditor;
using UnityEngine;

namespace TPCEngine.Editor
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(TPCharacter))]
    [CanEditMultipleObjects]
    public class TPCharacterEditor : TPCEditor
    {
        #region [Serialized Properties]
        private SerializedProperty e_CharacterCamera;
        private SerializedProperty e_CharacterMotor;
        private SerializedProperty e_InverseKinematics;
        #endregion

        #region [Required variables]
        private bool f_CharacterCamera;
        private bool f_CharacterMotor;
        private bool f_InverseKinematics;
        private bool f_BaseSpeed;
        private bool f_CrouchSpeed;

        private bool debugFootIK;
        private bool debugUpperBodyIK;
        #endregion

        #region [Functions]
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            e_CharacterCamera = serializedObject.FindProperty("characterCamera");
            e_CharacterMotor = serializedObject.FindProperty("characterMotor");
            e_InverseKinematics = serializedObject.FindProperty("inverseKinematics");
        }

        /// <summary>
        /// Custom On Scene GUI
        /// </summary>
        protected virtual void OnSceneGUI()
        {
            TPCharacter _TPCharacter = (TPCharacter)target;
            TPCInverseKinematics inverseKinematics = _TPCharacter.GetInverseKinematics();

            if (debugFootIK)
            {
                if (inverseKinematics.LeftFootPivot == null && inverseKinematics.RightFootPivot == null)
                    return;

                Handles.color = Color.black;
                Handles.DrawLine(inverseKinematics.LeftFootPivot.transform.position, inverseKinematics.LeftFootPivot.transform.position - Vector3.up * inverseKinematics.RayRange);
                Handles.DrawWireArc(inverseKinematics.LeftFootPivot.transform.position - Vector3.up * inverseKinematics.RayRange, Vector3.up, Vector3.forward, 360, 0.03f);
                Handles.color = new Color(0.25f, 0.25f, 0.25f, 0.1f);
                Handles.DrawSolidArc(inverseKinematics.LeftFootPivot.transform.position - Vector3.up * inverseKinematics.RayRange, Vector3.up, Vector3.forward, 360, 0.03f);

                Handles.color = Color.black;
                Handles.DrawLine(inverseKinematics.RightFootPivot.transform.position, inverseKinematics.RightFootPivot.transform.position - Vector3.up * inverseKinematics.RayRange);
                Handles.DrawWireArc(inverseKinematics.RightFootPivot.transform.position - Vector3.up * inverseKinematics.RayRange, Vector3.up, Vector3.forward, 360, 0.03f);
                Handles.color = new Color(0.25f, 0.25f, 0.25f, 0.1f);
                Handles.DrawSolidArc(inverseKinematics.RightFootPivot.transform.position - Vector3.up * inverseKinematics.RayRange, Vector3.up, Vector3.forward, 360, 0.03f);
            }

            if (debugUpperBodyIK)
            {
                Animator animator = _TPCharacter.GetComponent<Animator>();
                if (animator == null)
                    return;

                Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
                Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);

                Vector3 startPos = Extensions.CenterOfVectors(head.position, spine.position);

                if (inverseKinematics.LookTarget != null)
                {
                    Handles.color = Color.black;
                    Handles.DrawLine(startPos, inverseKinematics.LookTarget.position);
                }
            }
        }

        /// <summary>
        /// Custom Inspector GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            BeginBackground();
            Title("Third Person Character");

            BeginBox();
            f_CharacterCamera = EditorGUILayout.Foldout(f_CharacterCamera, "TPCamera", true);
            if (f_CharacterCamera)
                EditorGUILayout.PropertyField(e_CharacterCamera);
            GUILayout.Label("TPCamera Instance");
            EndBox();

            BeginBox();
            f_CharacterMotor = EditorGUILayout.Foldout(f_CharacterMotor, "Character Motor", true);
            if (f_CharacterMotor)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);
                GUILayout.Label("Locomotion", MiniTitleText);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("locomotionType"), new GUIContent("Locomotion Type", "Character Locomotion Type"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("freeRotationSpeed"), new GUIContent("Free Rotation Speed", "Speed of the rotation on free directional movement"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("strafeRotationSpeed"), new GUIContent("Locomotion Type", "Speed of the rotation while strafe movement"));
                GUILayout.Space(5);
                GUILayout.EndVertical();

                GUILayout.Space(7);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);
                GUILayout.Label("Air", MiniTitleText);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("jumpAirControl"), new GUIContent("Air Control", "Check to control the character while jumping"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("jumpTimer"), new GUIContent("Jump Timer", "How much time the character will be jumping"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("jumpForward"), new GUIContent("Jump Forward", "Add Extra jump speed, based on your speed input the character will move forward"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("jumpHeight"), new GUIContent("Jump Height", "Add Extra jump height, if you want to jump only with Root Motion leave the value with 0"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("jumpClip"), new GUIContent("Jump Sound"));
                GUILayout.Space(5);
                GUILayout.EndVertical();

                GUILayout.Space(7);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);
                GUILayout.Label("Movement", MiniTitleText);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("useRootMotion"), new GUIContent("Root Motion", "Check to drive the character using RootMotion of the animation"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("keepDirection"), new GUIContent("Keep Direction", "[TODO: Keep Direction Info...]"));
                f_BaseSpeed = EditorGUILayout.Foldout(f_BaseSpeed, "Base Speed", true);
                if (f_BaseSpeed)
                {
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("freeWalkSpeed"), new GUIContent("Free Walk Speed", "Add extra (free walk) speed for the locomotion movement"));
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("freeRunningSpeed"), new GUIContent("Free Running Speed", "Add extra (free running) speed for the locomotion movement"));
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("freeSprintSpeed"), new GUIContent("Free Sprint Speed", "Add extra (free sprint) speed for the locomotion movement"));
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("strafeWalkSpeed"), new GUIContent("Strafe Walk Speed", "Add extra (strafe Walk) speed for the locomotion movement"));
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("strafeRunningSpeed"), new GUIContent("Strafe Running Speed", "Add extra (strafe running) speed for the locomotion movement"));
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("strafeSprintSpeed"), new GUIContent("Strafe Sprint Speed", "Add extra (strafe sprint) speed for the locomotion movement"));
                }

                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("crouchHeight"), new GUIContent("Crouch Height"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("crouchSmooth"), new GUIContent("Crouch Smooth"));
                f_CrouchSpeed = EditorGUILayout.Foldout(f_CrouchSpeed, "Crouch Speed", true);
                if (f_CrouchSpeed)
                {
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("freeCrouchWalkSpeed"), new GUIContent("Free Crouch Walk Speed", "Add extra (free crouch walk) speed for the locomotion movement"));
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("freeCrouchRunningSpeed"), new GUIContent("Free Crouch Running Speed", "Add extra (free crouch running) speed for the locomotion movement"));
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("freeCrouchSprintSpeed"), new GUIContent("Free Crouch Sprint Speed", "Add extra (free crouch sprint) speed for the locomotion movement"));
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("strafeCrouchWalkSpeed"), new GUIContent("Strafe Crouch Walk Speed", "Add extra (strafe crouch Walk) speed for the locomotion movement"));
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("strafeCrouchRunningSpeed"), new GUIContent("Strafe Crouch Running Speed", "Add extra (strafe crouch running) speed for the locomotion movement"));
                    EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("strafeCrouchSprintSpeed"), new GUIContent("Strafe Crouch Sprint Speed", "Add extra (strafe crouch sprint) speed for the locomotion movement"));
                }
                GUILayout.Space(5);
                GUILayout.EndVertical();

                GUILayout.Space(3);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);
                GUILayout.Label("Ground", MiniTitleText);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("stepOffsetEnd"), new GUIContent("Step Offset End", "Offset height limit for sters"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("stepOffsetStart"), new GUIContent("Step Offset Start", "Offset height origin for sters, make sure to keep slight above the floor"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("stepSmooth"), new GUIContent("Step Smooth", "Higher value will result jittering on ramps, lower values will have difficulty on steps"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("slopeLimit"), new GUIContent("Slope Limit", "Max angle to walk"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("extraGravity"), new GUIContent("Extra Gravity", "Apply extra gravity when the character is not grounded"));
                GUILayout.Space(5);
                GUILayout.EndVertical();

                GUILayout.Space(7);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);
                GUILayout.Label("Layer", MiniTitleText);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("groundLayer"), new GUIContent("Ground Layer", "Layers that the character can walk on"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("groundMinDistance"), new GUIContent("Ground Min Distance", "Distance to became not grounded"));
                EditorGUILayout.PropertyField(e_CharacterMotor.FindPropertyRelative("groundMaxDistance"), new GUIContent("Ground Max Distance", "Distance to became not grounded"));
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label("Character Motor Settings");
            }
            EndBox();

            BeginBox();
            f_InverseKinematics = EditorGUILayout.Foldout(f_InverseKinematics, "Inverse Kinematic", true);
            if (f_InverseKinematics)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);
                GUILayout.Label("Foot IK", MiniTitleText);
                GUILayout.Space(3);
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("leftFootPivot"), new GUIContent("Left Foot Pivot", ""));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("rightFootPivot"), new GUIContent("Right Foot Pivot", ""));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("rayRange"), new GUIContent("Pivot Distance", "Distance between pivots and ground"));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("offsetY"), new GUIContent("Height Offset", "Height Offset between legs and ground"));
                if (TPCEPreferences.EditorData.DebugMode)
                    debugFootIK = EditorGUILayout.Toggle("Debug Foot IK", debugFootIK);
                GUILayout.Space(5);
                GUILayout.EndVertical();

                GUILayout.Space(3);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);
                GUILayout.Label("Upper body IK", MiniTitleText);
                GUILayout.Space(3);
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("lookTarget"), new GUIContent("Look Target", ""));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("upperBodyIKWeight"), new GUIContent("Upper Body IK Weight", "Global Upper Body IK Weight"));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("bodyIKWeight"), new GUIContent("Body IK Weight", ""));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("headIKWeight"), new GUIContent("Head IK Weight", ""));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("clampWeight"), new GUIContent("Clamp Weight", ""));
                if (TPCEPreferences.EditorData.DebugMode)
                    debugUpperBodyIK = EditorGUILayout.Toggle("Debug Upper Body IK", debugUpperBodyIK);
                GUILayout.Space(5);
                GUILayout.EndVertical();

                GUILayout.Space(3);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);
                GUILayout.Label("Hands IK", MiniTitleText);
                GUILayout.Space(3);
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("leftHand"));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("rightHand"));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("handIKWeight"));
                GUILayout.Space(5);
                GUILayout.EndVertical();

                GUILayout.Space(3);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);
                GUILayout.Label("IK States", MiniTitleText);
                GUILayout.Space(3);
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("active"), new GUIContent("IK Active", "Sets Global IK system active state \n True: IK system is active, however, subsystems can be disabled. \n False: Completely disables the IK system and all its subsystems."));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("footIKActive"), new GUIContent("Foot IK Active"));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("headIKActive"), new GUIContent("Upper Body IK Active"));
                EditorGUILayout.PropertyField(e_InverseKinematics.FindPropertyRelative("handIKActive"), new GUIContent("Hand IK Active"));
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label("Character IK Settings");
            }
            EndBox();
            EndBackground();
            serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }
}