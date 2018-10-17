/*	-----------------------------------------------------------
	|	Author:		Tamerlan Shakirov
	|	Publisher:	Infinite Dawn
	|	Project:	TPC Engine
	|	Copyright © Tamerlan Shakirov 2018 All rights reserved.
	----------------------------------------------------------- */

using UnityEngine;
using UnityEditor;

namespace TPCEngine.Editor
{
	/// <summary>
	/// 
	/// </summary>
	[CustomEditor(typeof(TPCamera))]
	[CanEditMultipleObjects]
	public class TPCameraEditor : TPCEditor
	{
		/// <summary>
		/// Custom Inspector GUI
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			BeginBackground();
			Title("Third Person Camera");
			BeginBox();
			GUILayout.Space(5);
			base.OnInspectorGUI();
			GUILayout.Space(5);
			EndBox();
			EndBackground();
			serializedObject.ApplyModifiedProperties();
		}
	}
}