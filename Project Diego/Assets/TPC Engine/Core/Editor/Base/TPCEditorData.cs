/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using UnityEngine;

namespace TPCEngine.Editor
{
	/// <summary>
	/// Third Person Character Engine editor preferences data.
	/// </summary>
	public class TPCEditorData : ScriptableObject
	{
		//Paths
		private string sourcePath;
		private string pluginsPath;

		//Mode
		private bool debugMode;

		//Editor GUI Colors
		private Color backgroundColor;
		private Color boxColor;

		/// <summary>
		/// Third Person Character Engine source path
		/// </summary>
		/// <value></value>
		public string SourcePath { get { return sourcePath; } set { sourcePath = value; } }

		/// <summary>
		/// Third Person Character Engine plugins path
		/// </summary>
		/// <value></value>
		public string PluginsPath { get { return pluginsPath; } set { pluginsPath = value; } }

		/// <summary>
		/// Third Person Character Engine debug mode (Editor Only)
		/// </summary>
		/// <value></value>
		public bool DebugMode { get { return debugMode; } set { debugMode = value; } }

		/// <summary>
		/// Third Person Character Engine editor components background color
		/// </summary>
		/// <value></value>
		public Color BackgroundColor { get { return backgroundColor; } set { backgroundColor = value; } }

		/// <summary>
		/// Third Person Character Engine editor components box color
		/// </summary>
		/// <value></value>
		public Color BoxColor { get { return boxColor; } set { boxColor = value; } }
	}
} 