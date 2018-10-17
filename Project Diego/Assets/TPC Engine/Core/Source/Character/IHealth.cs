/* ==================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================== */

using UnityEngine;

namespace TPCEngine
{
	/// <summary>
	/// Interface describing the architecture of the health
	/// </summary>
	public interface IHealth
	{
		/// <summary>
		/// Take Damage
		/// </summary>
		/// <param name="damage"></param>
		void TakeDamage(int damage);

		/// <summary>
		/// Health count
		/// </summary>
		/// <returns>Health</returns>
		int Health { get; set; }

		/// <summary>
		/// Max health count
		/// </summary>
		/// <returns>MaxHealth</returns>
		int MaxHealth { get; set; }

		/// <summary>
		/// Health state
		/// </summary>
		/// <returns>IsAlive</returns>
		bool IsAlive { get; }
	}
}