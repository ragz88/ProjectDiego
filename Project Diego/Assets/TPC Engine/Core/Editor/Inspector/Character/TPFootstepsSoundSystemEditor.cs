/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TPCEngine.Editor
{
	[CustomEditor(typeof(TPFootstepsSoundSystem))]
	[CanEditMultipleObjects]
	public class TPFootstepsSoundSystemEditor : TPCEditor
	{
		private SerializedProperty e_LeftFoot;
		private SerializedProperty e_RightFoot;
		private SerializedProperty e_DefaultVolume;
		private ReorderableList e_FootstepsVolumes;
		private SerializedProperty e_FootstepsMaterials;
		private SerializedProperty e_RayRange;

		private TPFootstepsSoundSystem footstepsSoundSystem;
		private bool footstepsSoundSystem_Foldout;
		private bool footstepsVolumes_Foldout;
		private bool debugFootSteps;

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			footstepsSoundSystem = (TPFootstepsSoundSystem) target;

			e_LeftFoot = serializedObject.FindProperty("leftFoot");
			e_RightFoot = serializedObject.FindProperty("rightFoot");
			e_DefaultVolume = serializedObject.FindProperty("defaultVolume");

			e_FootstepsMaterials = serializedObject.FindProperty("footstepsMaterials");
			e_RayRange = serializedObject.FindProperty("rayRange");

			e_FootstepsVolumes = new ReorderableList(serializedObject, serializedObject.FindProperty("footstepsVolumes"), true, true, true, true)
			{
				drawHeaderCallback = (rect) =>
					{
						EditorGUI.LabelField(rect, "Volume relative to speed");
					},

					drawElementCallback = (rect, index, active, focused) =>
					{
						SerializedProperty property = serializedObject.FindProperty("footstepsVolumes").GetArrayElementAtIndex(index);

						EditorGUI.LabelField(new Rect(rect.x + 5, rect.y + 1, 50, EditorGUIUtility.singleLineHeight), "Volume");
						property.FindPropertyRelative("volume").floatValue = EditorGUI.FloatField(new Rect(rect.x + 55, rect.y + 1, 50, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("volume").floatValue);
						if (property.FindPropertyRelative("volume").floatValue > 1)
							property.FindPropertyRelative("volume").floatValue /= 100;

						float min = property.FindPropertyRelative("minSpeed").floatValue;
						float max = property.FindPropertyRelative("maxSpeed").floatValue;
						EditorGUI.LabelField(new Rect(rect.x + 115, rect.y + 1, 35, EditorGUIUtility.singleLineHeight), "Speed");
						EditorGUI.LabelField(new Rect(rect.x + 160, rect.y + 1, 50, EditorGUIUtility.singleLineHeight), min.ToString("0.00"));
						EditorGUI.MinMaxSlider(new Rect(rect.x + 200, rect.y + 1, 150, EditorGUIUtility.singleLineHeight), ref min, ref max, 0, 2);
						EditorGUI.LabelField(new Rect(rect.x + 360, rect.y + 1, 50, EditorGUIUtility.singleLineHeight), max.ToString("0.00"));
						property.FindPropertyRelative("minSpeed").floatValue = min;
						property.FindPropertyRelative("maxSpeed").floatValue = max;
					}
			};
		}

		/// <summary>
		/// Custom On Scene GUI
		/// </summary>
		protected virtual void OnSceneGUI()
		{
			if (debugFootSteps)
			{
				if (e_LeftFoot.objectReferenceValue == null && e_RightFoot.objectReferenceValue == null)
					return;

				Handles.color = Color.black;
				Handles.DrawLine(footstepsSoundSystem.LeftFoot.transform.position, footstepsSoundSystem.LeftFoot.transform.position - Vector3.up * footstepsSoundSystem.RayRange);
				Handles.DrawWireArc(footstepsSoundSystem.LeftFoot.transform.position - Vector3.up * footstepsSoundSystem.RayRange, Vector3.up, Vector3.forward, 360, 0.03f);
				Handles.color = new Color(0.25f, 0.25f, 0.25f, 0.1f);
				Handles.DrawSolidArc(footstepsSoundSystem.LeftFoot.transform.position - Vector3.up * footstepsSoundSystem.RayRange, Vector3.up, Vector3.forward, 360, 0.03f);

				Handles.color = Color.black;
				Handles.DrawLine(footstepsSoundSystem.RightFoot.transform.position, footstepsSoundSystem.RightFoot.transform.position - Vector3.up * footstepsSoundSystem.RayRange);
				Handles.DrawWireArc(footstepsSoundSystem.RightFoot.transform.position - Vector3.up * footstepsSoundSystem.RayRange, Vector3.up, Vector3.forward, 360, 0.03f);
				Handles.color = new Color(0.25f, 0.25f, 0.25f, 0.1f);
				Handles.DrawSolidArc(footstepsSoundSystem.RightFoot.transform.position - Vector3.up * footstepsSoundSystem.RayRange, Vector3.up, Vector3.forward, 360, 0.03f);
			}
		}

		/// <summary>
		/// Custom Inspector GUI
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			BeginBackground();
			Title("Footsteps Sound System");
			BeginBox();
			footstepsSoundSystem_Foldout = EditorGUILayout.Foldout(footstepsSoundSystem_Foldout, "Footsteps Sound", true);
			if (footstepsSoundSystem_Foldout)
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				GUILayout.Space(5);
				GUILayout.Label("Foots", MiniTitleText);
				GUILayout.Space(5);
				EditorGUILayout.PropertyField(e_LeftFoot);
				EditorGUILayout.PropertyField(e_RightFoot);
				if (e_LeftFoot.objectReferenceValue == null || e_RightFoot.objectReferenceValue == null)
				{
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Try to fill foots auto..."))
					{
						FillFoot(footstepsSoundSystem.transform);
					}
					GUILayout.EndHorizontal();
					EditorGUILayout.HelpBox("Fill in all foots fields!", MessageType.Error);
				}
				e_RayRange.floatValue = EditorGUILayout.Slider("Distance", e_RayRange.floatValue, 0.0f, 10.0f);
				if (TPCEPreferences.EditorData.DebugMode)
					debugFootSteps = EditorGUILayout.Toggle("Debug Footsteps", debugFootSteps);
				GUILayout.Space(5);
				GUILayout.EndVertical();

				GUILayout.BeginVertical(EditorStyles.helpBox);
				GUILayout.Space(5);
				GUILayout.Label("Volume Settings", MiniTitleText);
				GUILayout.Space(5);
				e_DefaultVolume.floatValue = EditorGUILayout.Slider("Default Volume", e_DefaultVolume.floatValue, 0.0f, 1.0f);
				footstepsVolumes_Foldout = EditorGUILayout.Foldout(footstepsVolumes_Foldout, "Footsteps Volumes", true);
				if (footstepsVolumes_Foldout)
					e_FootstepsVolumes.DoLayoutList();
				GUILayout.Space(5);
				GUILayout.EndVertical();

				GUILayout.BeginVertical(EditorStyles.helpBox);
				GUILayout.Space(5);
				GUILayout.Label("Material Settings", MiniTitleText);
				GUILayout.Space(5);
				EditorGUILayout.PropertyField(e_FootstepsMaterials, true);
				GUILayout.Space(5);
				GUILayout.EndVertical();
			}
			if (!footstepsSoundSystem_Foldout) GUILayout.Label("Edit Footsteps Sound System");
			EndBox();
			EndBackground();
			serializedObject.ApplyModifiedProperties();
		}

		public virtual void FillFoot(Transform transform)
		{
			Animator animator = transform.GetComponent<Animator>();

			if (animator == null)
			{
				Debug.Log("Animator not found.");
				return;
			}

			Transform leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
			Transform rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);

			if (leftFoot != null)
				e_LeftFoot.objectReferenceValue = leftFoot;
			else
				Debug.Log("Body Bones - \"Left Foot\" not found.");

			if (rightFoot != null)
				e_RightFoot.objectReferenceValue = rightFoot;
			else
				Debug.Log("Body Bones - \" Right Foot\" not found.");
		}
	}
}