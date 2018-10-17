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
    [CustomEditor(typeof(CharacterHealth), true)]
    [CanEditMultipleObjects]
    public class CharacterHealthEditor : TPCEditor
    {

        private SerializedProperty e_PlayerHealth;
        private SerializedProperty e_MaxPlayerHealth;
        private SerializedProperty e_StartPlayerHealth;
        private ReorderableList e_fallDamageParams;
        private SerializedProperty e_DamageImage;
        private SerializedProperty e_UseRegeniration;
        private SerializedProperty e_RegenerationParam;
        private bool foldout_Health;
        private bool foldout_OnDeadEvent;
        private bool foldout_FallDamage;
        private int currentID;
        private bool foldout;

        void OnEnable()
        {
            e_PlayerHealth = serializedObject.FindProperty("health");
            e_MaxPlayerHealth = serializedObject.FindProperty("maxHealth");
            e_StartPlayerHealth = serializedObject.FindProperty("startHealth");
            e_DamageImage = serializedObject.FindProperty("damageImage");
            e_UseRegeniration = serializedObject.FindProperty("useRegeniration");
            e_RegenerationParam = serializedObject.FindProperty("regenerationParam");
            e_fallDamageParams = new ReorderableList(serializedObject, serializedObject.FindProperty("fallDamageParams"), true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                    {
                        EditorGUI.LabelField(rect, "Fall Damage");
                    },

                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        SerializedProperty property = serializedObject.FindProperty("fallDamageParams").GetArrayElementAtIndex(index);

						EditorGUI.LabelField(new Rect(rect.x + 5, rect.y + 1, 50, EditorGUIUtility.singleLineHeight), "Damage");
						property.FindPropertyRelative("damage").intValue = EditorGUI.IntField(new Rect(rect.x + 55, rect.y + 1, 45, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("damage").intValue);
                        
						float min = property.FindPropertyRelative("minHeight").floatValue;
						float max = property.FindPropertyRelative("maxHeight").floatValue;
						EditorGUI.LabelField(new Rect(rect.x + 115, rect.y + 1, 35, EditorGUIUtility.singleLineHeight), "Height");
                        min = EditorGUI.FloatField(new Rect(rect.x + 160, rect.y + 1, 50, EditorGUIUtility.singleLineHeight), min);
						EditorGUI.MinMaxSlider(new Rect(rect.x + 220, rect.y + 1, 150, EditorGUIUtility.singleLineHeight), ref min, ref max, 0, 100);
                        max = EditorGUI.FloatField(new Rect(rect.x + 380, rect.y + 1, 50, EditorGUIUtility.singleLineHeight), max);
						property.FindPropertyRelative("minHeight").floatValue = min;
						property.FindPropertyRelative("maxHeight").floatValue = max;
                    }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            BeginBackground();
            Title("Character Health");

            //Foldout Health
            GUILayout.BeginVertical("Button", GUILayout.Height(50));
            EditorGUILayout.Space();
            foldout_Health = EditorGUILayout.Foldout(foldout_Health, new GUIContent("Health"));
            if (foldout_Health)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Health Parameters", EditorStyles.boldLabel);
                EditorGUILayout.IntSlider(e_PlayerHealth, 0, 1000, new GUIContent("Player Health", "Health player at the moment"));
                EditorGUILayout.IntSlider(e_StartPlayerHealth, 0, 1000, new GUIContent("Start Player Health", "Health player in the start game"));
                EditorGUILayout.IntSlider(e_MaxPlayerHealth, 0, 1000, new GUIContent("Max Player Health", "Maximum value health"));
                EditorGUILayout.PropertyField(e_DamageImage, new GUIContent("Damage Image"));
                EditorGUILayout.PropertyField(e_UseRegeniration, new GUIContent("Use Regeneration"));
                if (e_UseRegeniration.boolValue)
                {
                    EditorGUILayout.PropertyField(e_RegenerationParam, new GUIContent("Regeneration Settings"), true);
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Fall Damage", EditorStyles.boldLabel);
                foldout_FallDamage = EditorGUILayout.Foldout(foldout_FallDamage, new GUIContent("Edit Fall Damage"), true);
                if (foldout_FallDamage)
                {
                    e_fallDamageParams.DoLayoutList();
                }
                GUILayout.EndVertical();
            }
            if (!foldout_Health) { EditorGUILayout.LabelField("Edit Health"); }
            EditorGUILayout.Space();
            GUILayout.EndVertical();

            EndBackground();
            serializedObject.ApplyModifiedProperties();
        }
    }
}