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
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class GrabObject : MonoBehaviour
	{
		[SerializeField] private Transform leftHandPivot;
		[SerializeField] private Transform rightHandPivot;
		[SerializeField] private float range;
		[SerializeField] private float heightOffset;

		public Transform LeftHandPivot { get { return leftHandPivot; } set { leftHandPivot = value; } }

		public Transform RighthandPivot { get { return rightHandPivot; } set { rightHandPivot = value; } }

		public float Range { get { return range; } set { range = value; } }

		public float HeightOffset { get { return heightOffset; } set { heightOffset = value; } }
	}
}