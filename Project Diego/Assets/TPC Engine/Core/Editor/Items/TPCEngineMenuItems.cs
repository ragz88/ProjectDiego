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
	public static class TPCEngineMenuItems
	{
		[MenuItem("TPC Engine/About", false, 0)]
		private static void OpenAboutWindow()
		{
			About.Open();
		}

		[MenuItem("TPC Engine/Documentation", false, 1)]
		private static void OpenDocumentation()
		{
			Documentation.Open();
		}

		[MenuItem("TPC Engine/Create/Character", false, 21)]
		private static void CreateDefaultCharacter()
		{
			CreateCharacter.Create("Default");
		}

		[MenuItem("TPC Engine/Create/Empty Character", false, 22)]
		private static void CreateEmptyCharacter()
		{
			CreateCharacter.Create("Empty");
		}

		[MenuItem("TPC Engine/Create/Camera", false, 23)]
		private static void CreateCamera()
		{
			CreateCharacter.Create("Camera");
		}

		[MenuItem("TPC Engine/Create/Spawn Area", false, 71)]
		private static void CreateSpawnArea()
		{
			GameObject spawnArea = new GameObject();
			spawnArea.name = "Spawn Area";
			spawnArea.AddComponent<SpawnArea>();
		}

		[MenuItem("TPC Engine/Plugin Installer", false, 221)]
		private static void OpenPluginInstaller()
		{
			PluginInstaller.Open();
		}
	}
}