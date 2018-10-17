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
	[CustomEditor(typeof(TPGrabSystem))]
	[CanEditMultipleObjects]
	public class TPGrabSystemEditor : TPCEditor
	{
		/// <summary>
		/// Custom Inspector GUI for TPGrabSystem
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			BeginBackground();
			Title("Grab System");
			BeginBox();
			base.OnInspectorGUI();
			EndBox();
			EndBackground();
			serializedObject.ApplyModifiedProperties();
		}
	}
}