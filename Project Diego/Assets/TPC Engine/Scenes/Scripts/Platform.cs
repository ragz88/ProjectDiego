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
	public class Platform : MonoBehaviour
	{
		private enum Angle { X, Y, Z }

		[SerializeField] private float delay;
		[SerializeField] private bool randomValues;
		[SerializeField] private Angle angle;
		[SerializeField] private float speed;
		[SerializeField] private float range;

		private float defaultAngle;

		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		protected virtual void Start()
		{
			if (randomValues)
			{
				delay = Random.Range(0.0f, 10.0f);
				range = Random.Range(0.0f, 0.1f);
			}
			switch (angle)
			{
				case Angle.X:
					defaultAngle = transform.position.x;
					break;
				case Angle.Y:
					defaultAngle = transform.position.y;
					break;
				case Angle.Z:
					defaultAngle = transform.position.z;
					break;
			}
			StartCoroutine(Process());
		}

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		protected virtual IEnumerator Process()
		{
			yield return new WaitForSeconds(delay);
			while (true)
			{
				switch (angle)
				{
					case Angle.X:
						transform.position = new Vector3(defaultAngle + Mathf.Sin(Time.time * speed) * range, transform.position.y, transform.position.z);
						break;
					case Angle.Y:
						transform.position = new Vector3(transform.position.x, defaultAngle + Mathf.Sin(Time.time * speed) * range, transform.position.z);
						break;
					case Angle.Z:
						transform.position = new Vector3(transform.position.x, transform.position.y, defaultAngle + Mathf.Sin(Time.time * speed) * range);
						break;
				}
				yield return null;
			}
		}
	}
}