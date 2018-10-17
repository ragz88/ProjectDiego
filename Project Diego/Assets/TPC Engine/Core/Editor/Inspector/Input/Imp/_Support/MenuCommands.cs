#region [Copyright (c) 2015 Cristian Alexandru Geambasu]
//	Distributed under the terms of an MIT-style license:
//
//	The MIT License
//
//	Copyright (c) 2015 Cristian Alexandru Geambasu
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
//	and associated documentation files (the "Software"), to deal in the Software without restriction, 
//	including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//	and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all copies or substantial 
//	portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion
using UnityEngine;
using UnityEditor;
using UnityInputConverter;

namespace TPCEngine.Editor
{
	public static class MenuCommands
	{
		[MenuItem("TPC Engine/Input Manager/Create Input Manager", false, 152)]
		private static void CreateInputManager()
		{
			GameObject gameObject = new GameObject("Input Manager");
			gameObject.AddComponent<InputManager>();

			// Register Input Manager for undo, mark scene as dirty.
			Undo.RegisterCreatedObjectUndo(gameObject, "Create Input Manager");

			Selection.activeGameObject = gameObject;
		}

		[MenuItem("TPC Engine/Input Manager/Convert Unity Input", false, 153)]
		private static void ConvertInput()
		{
			string sourcePath = EditorUtility.OpenFilePanel("Select Unity input settings asset", "", "asset");
			if(!string.IsNullOrEmpty(sourcePath))
			{
				string destinationPath = EditorUtility.SaveFilePanel("Save imported input axes", "", "input_manager", "xml");
				if(!string.IsNullOrEmpty(destinationPath))
				{
					try
					{
						InputConverter converter = new InputConverter();
						converter.ConvertUnityInputManager(sourcePath, destinationPath);

						EditorUtility.DisplayDialog("Success", "Unity input converted successfuly!", "OK");
					}
					catch(System.Exception ex)
					{
						Debug.LogException(ex);

						string message = "Failed to convert Unity input! Please make sure 'InputManager.asset' is serialized as a YAML text file.";
						EditorUtility.DisplayDialog("Error", message, "OK");
					}
				}
			}
		}
        [MenuItem("TPC Engine/Input Manager/Documentation", false, 401)]
		public static void OpenDocumentationPage()
		{
			Application.OpenURL("https://github.com/daemon3000/InputManager/wiki");
		}
	}
}
