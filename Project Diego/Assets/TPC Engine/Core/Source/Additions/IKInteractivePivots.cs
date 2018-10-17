/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using UnityEngine;

namespace TPCEngine
{
	public class IKInteractivePivots : MonoBehaviour
	{
		[SerializeField] private Transform leftHandPivot;
		[SerializeField] private Transform rightHandPivot;
		[SerializeField] private float smooth;

		public Transform LeftHandPivot { get { return leftHandPivot; } set { leftHandPivot = value; } }

		public Transform RighthandPivot { get { return rightHandPivot; } set { rightHandPivot = value; } }

		public float Smooth { get { return smooth; } set { smooth = value; } }
	}
}