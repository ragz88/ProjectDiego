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
	public static class TPCEPreferences
	{
		private static TPCEditorData editorData;

		[PreferenceItem("TPC Engine")]
		private static void Preference()
		{
			if (EditorData == null)
			{
				EditorGUILayout.HelpBox(
					"Third Person Character Engine editor preferences data not found.\n" +
					"They may have been deleted...\n" +
					"Path: TPC Engine/Core/Editor/Resources/TPCEditorData.asset.", MessageType.Info);
				return;
			}

			GUILayout.BeginVertical(EditorStyles.helpBox);
			GUILayout.Label("Path", EditorStyles.boldLabel);
			EditorData.SourcePath = EditorGUILayout.TextField("Source Path", EditorData.SourcePath);
			EditorData.PluginsPath = EditorGUILayout.DelayedTextField("Plugins Path", EditorData.PluginsPath);
			GUILayout.EndVertical();

			GUILayout.Space(5);

			GUILayout.BeginVertical(EditorStyles.helpBox);
			GUILayout.Label("Debug", EditorStyles.boldLabel);
			EditorData.DebugMode = EditorGUILayout.Toggle("Debug Mode", EditorData.DebugMode);
			GUILayout.EndVertical();

			GUILayout.Space(5);

			GUILayout.BeginVertical(EditorStyles.helpBox);
			GUILayout.Label("Editor Color", EditorStyles.boldLabel);
			EditorData.BackgroundColor = EditorGUILayout.ColorField("Background Color", EditorData.BackgroundColor);
			EditorData.BoxColor = EditorGUILayout.ColorField("Box Color", EditorData.BoxColor);
			GUILayout.EndVertical();

			GUILayout.Space(10);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Reset Paths"))
			{
				EditorData.SourcePath = "Assets/TPC Engine";
				EditorData.PluginsPath = "/Resources/Plugin";
			}
			if (GUILayout.Button("Reset Colors"))
			{
				EditorData.BackgroundColor = new Color32(97, 95, 93, 255);
				EditorData.BoxColor = Color.white;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			if(GUI.changed)
				EditorUtility.SetDirty(EditorData);
		}

		/// <summary>
		/// Third Person Character Engine editor preferences data
		/// </summary>
		/// <value></value>
		public static TPCEditorData EditorData
		{
			get
			{
				if (!editorData)
					editorData = Resources.Load("TPCEditorData") as TPCEditorData;
				return editorData;
			}
		}
	}
}