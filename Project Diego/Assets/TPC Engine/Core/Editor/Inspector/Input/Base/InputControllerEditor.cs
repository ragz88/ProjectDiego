/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using System.Text;
using UnityEditor;
using UnityEngine;
using TPCEngine.Utility;

namespace TPCEngine.Editor
{
	[CustomEditor(typeof(InputController), true)]
	[CanEditMultipleObjects]
	public class InputControllerEditor : TPCEditor
	{
		private string title;

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			InputController instance = target as InputController;
			title = instance.GetType().Name.AddSpaces();
		}

		/// <summary>
		/// Custom Inspector GUI
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			BeginBackground();
			Title(title);
			BeginBox();
			EditorGUILayout.HelpBox("Handling TPC Engine Input By " + title, MessageType.Info);
			EndBox();
			EndBackground();
			serializedObject.ApplyModifiedProperties();
		}
	}
}