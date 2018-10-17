/* ==================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using TPCEngine.Utility;

namespace TPCEngine
{
	/// <summary>
	/// TPCInput this is the input handler class.
	/// Through this class you can get the input data of the current controller used.
	/// </summary>
	/// <typeparam name="TPCInput"></typeparam>
	public class TPCInput
	{
		//Initialization Input Controller interface
		private static InputController inputController = InputController.Instance;

		/// <summary>
		/// Returns the value of the virtual axis identified by axisName.
		/// The value will be in the range -1...1 for keyboard and joystick input.
		/// </summary>
		/// <param name="axisName">Axis identified</param>
		/// <returns>float</returns>
        public static float GetAxis(string axisName)
        {
            return inputController.GetAxis(axisName);
        }

		/// <summary>
		/// Returns the value of the virtual axis identified by axisName with no smoothing filtering applied.
		/// The value will be in the range -1...1 for keyboard and joystick input. 
		/// Since input is not smoothed, keyboard input will always be either -1, 0 or 1.
		/// </summary>
		/// <param name="axisName">Axis identified</param>
		/// <returns>float</returns>
        public static float GetAxisRaw(string axisName)
        {
            return inputController.GetAxisRaw(axisName);
        }

		/// <summary>
		/// Returns true while the virtual button identified by buttonName is held down.
		/// </summary>
		/// <param name="buttonName">Button identified</param>
		/// <returns>bool True when an axis has been held down and not released.</returns>
        public static bool GetButtonDown(string butoonName)
        {
            return inputController.GetButtonDown(butoonName);
        }

		/// <summary>
		/// Returns true during the frame the user pressed down the virtual button identified by buttonName.
		/// </summary>
		/// <param name="butoonName">Button identified</param>
		/// <returns>bool True when an axis has been pressed and not released.</returns>
        public static bool GetButton(string buttonName)
        {
            return inputController.GetButton(buttonName);
        }
		

		/// <summary>
		/// Returns true the first frame the user releases the virtual button identified by buttonName.
		/// </summary>
		/// <param name="buttonName">Button identified</param>
		/// <returns>bool True when an axis has been released</returns>
        public static bool GetButtonUp(string buttonName)
        {
            return inputController.GetButtonUp(buttonName);
        }
	}
}