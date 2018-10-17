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
	public class IKInteractive : MonoBehaviour
	{
		[SerializeField] private float range;

		private TPCInverseKinematics inverseKinematics;

		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		protected virtual void Start()
		{
			inverseKinematics = GetComponent<TPCharacter>().GetInverseKinematics();
			StartCoroutine("IKInteractiveProcessing", 0.1f);
		}

		/// <summary>
		/// IK hand interactive processing
		/// </summary>
		/// <param name="deltaTime"></param>
		/// <returns></returns>
		public virtual IEnumerator IKInteractiveProcessing(float deltaTime)
		{
			Collider[] colliders;
			IKInteractivePivots interactivePivots = null;

			while (true)
			{
				if (interactivePivots == null)
				{
					yield return new WaitForSeconds(deltaTime);
					Debug.Log("!");
					colliders = Physics.OverlapSphere(transform.position, range);
					for (int i = 0; i < colliders.Length; i++)
					{
						interactivePivots = colliders[i].GetComponent<IKInteractivePivots>();
						if (interactivePivots != null)
							break;
					}
					// if (inverseKinematics.HandIKWeight != 0)
					// 	inverseKinematics.HandIKWeight = Mathf.MoveTowards(inverseKinematics.HandIKWeight, 0, 1);
				}
				else
				{
					if (inverseKinematics.HandIKWeight != 1)
						inverseKinematics.HandIKWeight = Mathf.MoveTowards(inverseKinematics.HandIKWeight, 1, interactivePivots.Smooth);
					inverseKinematics.UpdateHandsTarget(interactivePivots.LeftHandPivot, interactivePivots.RighthandPivot);

					if (Vector3.Distance(transform.position, interactivePivots.transform.position) >= range)
						interactivePivots = null;
				}
				yield return null;
			}
		}
	}
}