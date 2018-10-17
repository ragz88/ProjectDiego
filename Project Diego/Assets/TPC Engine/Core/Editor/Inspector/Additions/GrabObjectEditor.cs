/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using UnityEditor;
using UnityEngine;

namespace TPCEngine.Editor
{
	[CustomEditor(typeof(GrabObject))]
	[CanEditMultipleObjects]
	public class GrabObjectEditor : TPCEditor
	{
		private GrabObject instance;
		private bool debugPivots;

		private float handleSize = 0.1f;
		private Color handleLeftColor = Color.red;
		private Color handleRightColor = Color.blue;

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			instance = (GrabObject) target;
		}

		/// <summary>
		/// Custom Scene GUI.
		/// </summary>
		protected virtual void OnSceneGUI()
		{
			if (!TPCEPreferences.EditorData.DebugMode || instance == null || (instance.LeftHandPivot == null || instance.RighthandPivot == null))
				return;

			Handles.color = handleLeftColor;
			Handles.CubeHandleCap(0, instance.LeftHandPivot.position, instance.LeftHandPivot.rotation, handleSize, EventType.Repaint);

			Handles.color = handleRightColor;
			Handles.CubeHandleCap(0, instance.RighthandPivot.position, instance.RighthandPivot.rotation, handleSize, EventType.Repaint);

			instance.LeftHandPivot.position = Handles.PositionHandle(instance.LeftHandPivot.position, Quaternion.identity);
			instance.RighthandPivot.position = Handles.PositionHandle(instance.RighthandPivot.position, Quaternion.identity);

			instance.LeftHandPivot.rotation = Handles.RotationHandle(instance.LeftHandPivot.rotation, instance.LeftHandPivot.position);
			instance.RighthandPivot.rotation = Handles.RotationHandle(instance.RighthandPivot.rotation, instance.RighthandPivot.position);
		}

		/// <summary>
		/// Custom Inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			BeginBackground();
			Title("Grab Object");
			BeginBox();
			base.OnInspectorGUI();
			if (TPCEPreferences.EditorData.DebugMode)
			{
				debugPivots = EditorGUILayout.Foldout(debugPivots, "Debug");
				if (debugPivots)
				{
					handleSize = EditorGUILayout.Slider("Handle Size", handleSize, 0.0f, 1.0f);
					handleLeftColor = EditorGUILayout.ColorField("Left Hand Handle", handleLeftColor);
					handleRightColor = EditorGUILayout.ColorField("Right Hand Handle", handleRightColor);
				}
			}

			EndBox();
			EndBackground();
			serializedObject.ApplyModifiedProperties();
		}
	}
}