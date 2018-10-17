/*	-----------------------------------------------------------
	|	Author:		Tamerlan Shakirov
	|	Publisher:	Infinite Dawn
	|	Project:	TPC Engine
	|	Copyright © Tamerlan Shakirov 2018 All rights reserved.
	----------------------------------------------------------- */

using UnityEditor;
using UnityEngine;

namespace TPCEngine.Editor
{
	/// <summary>
	/// 
	/// </summary>
	public class TPCEditor : UnityEditor.Editor
	{
		/// <summary>
        /// Deflaut Title
        /// </summary>
        /// <param name="message"></param>
        public virtual void Title(string message)
        {
            GUI.color = Color.white;
            GUILayout.BeginVertical(GUI.skin.button);
            EditorGUILayout.LabelField(message, TitleText);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Title with custom color
        /// </summary>
        /// <param name="message"></param>
        /// <param name="titleColor"></param>
        public virtual void Title(string message, Color32 titleColor)
        {
            GUI.color = titleColor;
            GUILayout.BeginVertical(GUI.skin.button);
            EditorGUILayout.LabelField(message, TitleText);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Deflaut Background
        /// </summary>
        public virtual void BeginBackground()
        {
            GUILayout.Space(7);
            GUI.color = new Color32(97, 95, 93, 255);
            GUILayout.BeginVertical(GUI.skin.window, GUILayout.Height(1));
        }

        /// <summary>
        /// Background with custom color
        /// </summary>
        /// <param name="color"></param>
        public virtual void BeginBackground(Color32 color)
        {
            GUILayout.Space(7);
            GUI.color = color;
            GUILayout.BeginVertical(GUI.skin.window, GUILayout.Height(1));
        }

        /// <summary>
        /// End Background
        /// </summary>
        public virtual void EndBackground()
        {
            GUILayout.EndVertical();
            GUILayout.Space(7);
        }

        public virtual void BeginBox()
        {
            GUI.color = Color.white;
            GUILayout.BeginVertical(GUI.skin.button);
            GUILayout.Space(7);
        }

        public virtual void BeginBox(Color color)
        {
            GUI.color = color;
            GUILayout.BeginVertical(GUI.skin.button);
            GUILayout.Space(7);
        }

        public virtual void EndBox()
        {
            GUILayout.Space(7);
            GUILayout.EndVertical();
        }
        /// <summary>
        /// Progress Bar
        /// </summary>
        /// <param name="value"></param>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <param name="heigth"></param>
        public virtual void ProgressBar(float value, string label, float width = 20, float heigth = 20, float labelX = 1, float labelY = 1)
        {
            Rect rect = GUILayoutUtility.GetRect(width, heigth, EditorStyles.helpBox);
            EditorGUI.ProgressBar(rect, value, "");
            EditorGUI.LabelField(new Rect(GUILayoutUtility.GetRect(0, 0).x + (width * labelX), GUILayoutUtility.GetRect(0, 0).y - (heigth * labelY), 350, 50), label, EditorStyles.boldLabel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual GUIStyle TitleText
        {
            get
            {
                GUIStyle style = new GUIStyle();
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.MiddleCenter;
                return style;
            }
        }

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual GUIStyle MiniTitleText
        {
            get
            {
                GUIStyle style = new GUIStyle();
                style.fontSize = 11;
				style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.MiddleCenter;
                return style;
            }
        }
	}
}