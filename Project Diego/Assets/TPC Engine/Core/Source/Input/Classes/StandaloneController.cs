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
	/// Base StandaloneController Controller in TPC Engine, which used default Input manager.
	/// </summary>
	public class StandaloneController : InputController
	{
		/// <summary>
		/// Returns the value of the virtual axis identified by axisName.
		/// The value will be in the range -1...1 for keyboard and joystick input.
		/// </summary>
		/// <param name="axisName">Axis identified</param>
		/// <returns>float</returns>
		public override float GetAxis(string axisName)
		{
			return InputManager.GetAxis(axisName);
		}

		/// <summary>
		/// Returns the value of the virtual axis identified by axisName with no smoothing filtering applied.
		/// The value will be in the range -1...1 for keyboard and joystick input. 
		/// Since input is not smoothed, keyboard input will always be either -1, 0 or 1.
		/// </summary>
		/// <param name="axisName">Axis identified</param>
		/// <returns>float</returns>
		public override float GetAxisRaw(string axisName)
		{
			return InputManager.GetAxisRaw(axisName);
		}

		/// <summary>
		/// Returns true while the virtual button identified by buttonName is held down.
		/// </summary>
		/// <param name="buttonName">Button identified</param>
		/// <returns>bool True when an axis has been held down and not released.</returns>
		public override bool GetButtonDown(string butoonName)
		{
			return InputManager.GetButtonDown(butoonName);
		}

		/// <summary>
		/// Returns true during the frame the user pressed down the virtual button identified by buttonName.
		/// </summary>
		/// <param name="butoonName">Button identified</param>
		/// <returns>bool True when an axis has been pressed and not released.</returns>
		public override bool GetButton(string buttonName)
		{
			return InputManager.GetButton(buttonName);
		}

		/// <summary>
		/// Returns true the first frame the user releases the virtual button identified by buttonName.
		/// </summary>
		/// <param name="buttonName">Button identified</param>
		/// <returns>bool True when an axis has been released</returns>
		public override bool GetButtonUp(string buttonName)
		{
			return InputManager.GetButtonUp(buttonName);
		}
	}
}