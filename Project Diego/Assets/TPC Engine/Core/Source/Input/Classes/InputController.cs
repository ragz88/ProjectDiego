/* ==================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using TPCEngine.Utility;
using UnityEngine;

namespace TPCEngine
{
	/// <summary>
	/// Input Controller interface, base interface for all controllers used in TPCEngine.
	/// Contains main functions for implementation.
	/// </summary>
	public abstract class InputController : Singleton<InputController>
	{
		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		protected virtual void Start()
		{
			// Start Cursor Processing
			StartCoroutine("CursorProcess");
		}

		/// <summary>
		/// Returns the value of the virtual axis identified by axisName.
		/// The value will be in the range -1...1 for keyboard and joystick input.
		/// </summary>
		/// <param name="axisName">Axis identified</param>
		/// <returns>float</returns>
		public abstract float GetAxis(string axisName);

		/// <summary>
		/// Returns the value of the virtual axis identified by axisName with no smoothing filtering applied.
		/// The value will be in the range -1...1 for keyboard and joystick input. 
		/// Since input is not smoothed, keyboard input will always be either -1, 0 or 1.
		/// </summary>
		/// <param name="axisName">Axis identified</param>
		/// <returns>float</returns>
		public abstract float GetAxisRaw(string axisName);

		/// <summary>
		/// Returns true during the frame the user pressed down the virtual button identified by buttonName.
		/// </summary>
		/// <param name="butoonName">Button identified</param>
		/// <returns>bool True when an axis has been pressed and not released.</returns>
		public abstract bool GetButtonDown(string butoonName);

		/// <summary>
		/// Returns true while the virtual button identified by buttonName is held down.
		/// </summary>
		/// <param name="buttonName">Button identified</param>
		/// <returns>bool True when an axis has been held down and not released.</returns>
		public abstract bool GetButton(string buttonName);

		/// <summary>
		/// Returns true the first frame the user releases the virtual button identified by buttonName.
		/// </summary>
		/// <param name="buttonName">Button identified</param>
		/// <returns>bool True when an axis has been released</returns>
		public abstract bool GetButtonUp(string buttonName);

		/// <summary>
		/// Cursor processing coroutine
		/// </summary>
		/// <returns>yield</returns>
		protected System.Collections.IEnumerator CursorProcess()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			while (true)
			{
				if (TPCInput.GetButtonDown("Menu") && !Cursor.visible)
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
				else if (TPCInput.GetButtonDown("Menu") && Cursor.visible)
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
				}
				yield return null;
			}
		}
	}
}