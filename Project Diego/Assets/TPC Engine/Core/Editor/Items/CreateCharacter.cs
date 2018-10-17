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
	public static class CreateCharacter
	{
		private static GameObject character = Resources.Load("Character") as GameObject;
		private static GameObject emptyCharacter = Resources.Load("EmptyCharacter") as GameObject;
		private static GameObject camera = Resources.Load("Camera") as GameObject;

		public static void Create(string type)
		{
			switch (type)
			{
				case "Default":
					if (character == null)
					{
						Debug.Log("Character not found, check the Resources in the Editor folder.");
						break;
					}
					GameObject _character = GameObject.Instantiate(character, Vector3.zero, Quaternion.identity);
					_character.name = "Character";
					break;
				case "Empty":
					if (emptyCharacter == null)
					{
						Debug.Log("Empty Character not found, check the Resources in the Editor folder.");
						break;
					}
					GameObject _emptyCharacter = GameObject.Instantiate(emptyCharacter, Vector3.zero, Quaternion.identity);
					_emptyCharacter.name = "Empty Character";
					break;
				case "Camera":
					if (camera == null)
					{
						Debug.Log("Camera not found, check the Resources in the Editor folder.");
						break;
					}
					GameObject _camera = GameObject.Instantiate(camera, Vector3.zero, Quaternion.identity);
					_camera.name = "Camera";
					break;
			}
		}
	}
}