/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using System.Collections;
using UnityEngine;

namespace TPCEngine
{
	public class HelpMessage : MonoBehaviour
	{
		[SerializeField] private float time;
		private bool once;

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		protected virtual void Update()
		{
			if (Input.GetKeyDown(KeyCode.H) && gameObject.activeSelf)
				gameObject.SetActive(false);
			else if (!once && gameObject.activeSelf)
				StartCoroutine(Hide(time));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public virtual IEnumerator Hide(float time)
		{
			once = true;
			yield return new WaitForSeconds(time);
			gameObject.SetActive(false);
			once = false;
			yield break;
		}
	}
}