/* ====================================================================
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
    /// <summary>
    /// Custom Editor for Spawn Manager
    /// </summary>
    [CustomEditor(typeof(SpawnArea), true)]
    public class SpawnAreaEditor : TPCEditor
    {
        SpawnArea spawnManager;
        private SerializedProperty e_Shape;
        private SerializedProperty e_Player;
        private SerializedProperty e_Rotation;
        private SerializedProperty e_Radius;
        private SerializedProperty e_Lenght;
        private SerializedProperty e_Weight;
        private SerializedProperty e_Time;
        private SerializedProperty e_AutoSpawn;
        private SerializedProperty e_SpawnKey;
        private SerializedProperty e_Sound;
        private SerializedProperty e_Panel;
        private SerializedProperty e_Message;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (spawnManager == null)
                spawnManager = (SpawnArea) target;
            e_Shape = serializedObject.FindProperty("shape");
            e_Player = serializedObject.FindProperty("player");
            e_Rotation = serializedObject.FindProperty("rotation");
            e_Radius = serializedObject.FindProperty("radius");
            e_Lenght = serializedObject.FindProperty("lenght");
            e_Weight = serializedObject.FindProperty("weight");
            e_Time = serializedObject.FindProperty("time");
            e_AutoSpawn = serializedObject.FindProperty("autoSpawn");
            e_SpawnKey = serializedObject.FindProperty("spawnKey");
            e_Sound = serializedObject.FindProperty("sound");
            e_Panel = serializedObject.FindProperty("panel");
            e_Message = serializedObject.FindProperty("message");
        }

        protected virtual void OnSceneGUI()
        {
            if (spawnManager == null)
                spawnManager = (SpawnArea) target;

            Vector3 pos = spawnManager.transform.position;
            switch (spawnManager.Shape)
            {
                case SpawnShape.Rectangle:

                    Vector3[] verts = new Vector3[]
                    {
                        new Vector3(pos.x + spawnManager.Weight / 2, pos.y, pos.z - spawnManager.Lenght / 2),
                        new Vector3(pos.x + spawnManager.Weight / 2, pos.y, pos.z + spawnManager.Lenght / 2),
                        new Vector3(pos.x - spawnManager.Weight / 2, pos.y, pos.z + spawnManager.Lenght / 2),
                        new Vector3(pos.x - spawnManager.Weight / 2, pos.y, pos.z - spawnManager.Lenght / 2)
                    };

                    Handles.DrawSolidRectangleWithOutline(verts, new Color(0.25f, 0.25f, 0.25f, 0.1f), new Color(0, 0, 0, 1));

                    foreach (Vector3 posCube in verts)
                    {
                        Handles.color = new Color32(167, 1, 42, 255);
                        Handles.CubeHandleCap(0, posCube, Quaternion.identity, 0.25f, EventType.Repaint);
                    }
                    Handles.color = Color.white;
                    spawnManager.Lenght = Handles.ScaleSlider(spawnManager.Lenght, new Vector3(pos.x, pos.y, pos.z - spawnManager.Lenght / 2), -spawnManager.transform.forward, spawnManager.transform.rotation, 2.0f, 0.5f);
                    spawnManager.Weight = Handles.ScaleSlider(spawnManager.Weight, new Vector3(pos.x + spawnManager.Weight / 2, pos.y, pos.z), spawnManager.transform.right, spawnManager.transform.rotation, 2.0f, 0.5f);
                    spawnManager.Lenght = Handles.ScaleSlider(spawnManager.Lenght, new Vector3(pos.x, pos.y, pos.z + spawnManager.Lenght / 2), spawnManager.transform.forward, spawnManager.transform.rotation, 2.0f, 0.5f);
                    spawnManager.Weight = Handles.ScaleSlider(spawnManager.Weight, new Vector3(pos.x - spawnManager.Weight / 2, pos.y, pos.z), -spawnManager.transform.right, spawnManager.transform.rotation, 2.0f, 0.5f);
                    break;

                case SpawnShape.Circle:

                    Vector3[] cverts = new Vector3[]
                    {
                        new Vector3(pos.x - spawnManager.Radius, pos.y, pos.z),
                        new Vector3(pos.x, pos.y, pos.z + spawnManager.Radius),
                        new Vector3(pos.x + spawnManager.Radius, pos.y, pos.z),
                        new Vector3(pos.x, pos.y, pos.z - spawnManager.Radius)
                    };

                    Handles.color = Color.black;
                    Handles.DrawWireArc(spawnManager.transform.position, Vector3.up, Vector3.forward, 360, spawnManager.Radius);
                    Handles.color = new Color(0.25f, 0.25f, 0.25f, 0.1f);
                    Handles.DrawSolidArc(spawnManager.transform.position, Vector3.up, Vector3.forward, 360, spawnManager.Radius);
                    foreach (Vector3 posCube in cverts)
                    {
                        Handles.color = new Color32(167, 1, 42, 255);
                        Handles.CubeHandleCap(0, posCube, Quaternion.identity, 0.25f, EventType.Repaint);
                    }

                    Handles.color = Color.white;
                    spawnManager.Radius = Handles.ScaleSlider(spawnManager.Radius, new Vector3(pos.x, pos.y, pos.z - spawnManager.Radius), -spawnManager.transform.forward, spawnManager.transform.rotation, 2.0f, 0.5f);
                    spawnManager.Radius = Handles.ScaleSlider(spawnManager.Radius, new Vector3(pos.x + spawnManager.Radius, pos.y, pos.z), spawnManager.transform.right, spawnManager.transform.rotation, 2.0f, 0.5f);
                    spawnManager.Radius = Handles.ScaleSlider(spawnManager.Radius, new Vector3(pos.x, pos.y, pos.z + spawnManager.Radius), spawnManager.transform.forward, spawnManager.transform.rotation, 2.0f, 0.5f);
                    spawnManager.Radius = Handles.ScaleSlider(spawnManager.Radius, new Vector3(pos.x - spawnManager.Radius, pos.y, pos.z), -spawnManager.transform.right, spawnManager.transform.rotation, 2.0f, 0.5f);
                    break;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            BeginBackground();
            Title("Spawn Area");
            BeginBox();
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);
            GUILayout.Label("Spawn Parameters", MiniTitleText);
            GUILayout.Space(3);
            EditorGUILayout.PropertyField(e_Shape, new GUIContent("Shape"));
            EditorGUILayout.PropertyField(e_Player, new GUIContent("Character"));
            EditorGUILayout.PropertyField(e_Rotation, new GUIContent("Rotation"));
            switch (spawnManager.Shape)
            {
                case SpawnShape.Rectangle:
                    EditorGUILayout.PropertyField(e_Lenght, new GUIContent("Lenght"));
                    EditorGUILayout.PropertyField(e_Weight, new GUIContent("Weight"));
                    break;
                case SpawnShape.Circle:
                    EditorGUILayout.PropertyField(e_Radius, new GUIContent("Radius"));
                    break;
            }
            EditorGUILayout.PropertyField(e_Time, new GUIContent("Time"));
            EditorGUILayout.PropertyField(e_AutoSpawn, new GUIContent("Auto Spawn"));
            if(!e_AutoSpawn.boolValue)
                EditorGUILayout.PropertyField(e_SpawnKey, new GUIContent("Spawn Key"));
            EditorGUILayout.PropertyField(e_Sound, new GUIContent("Sound"));
            GUILayout.Space(5);
            GUILayout.EndVertical();

            GUILayout.Space(3);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);
            GUILayout.Label("UI", MiniTitleText);
            GUILayout.Space(3);
            EditorGUILayout.PropertyField(e_Panel, new GUIContent("Panel"));
            EditorGUILayout.PropertyField(e_Message, new GUIContent("Message"));
            GUILayout.Space(5);
            GUILayout.EndVertical();
            EndBox();
            EndBackground();
            serializedObject.ApplyModifiedProperties();
        }
    }
}