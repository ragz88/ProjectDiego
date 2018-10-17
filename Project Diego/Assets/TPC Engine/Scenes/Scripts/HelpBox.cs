/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using UnityEngine;
using UnityEngine.UI;

namespace TPCEngine
{
	[RequireComponent(typeof(Collider))]
	public class HelpBox : MonoBehaviour
	{
		[TextArea][SerializeField] private string message;
		[SerializeField] private Transform _UIPanel;
		[SerializeField] private Text _UIText;

		[Header("Interactive")]
		[SerializeField] private AudioClip grabSound;
		[SerializeField] private float _MoveSpeed;
		[SerializeField] private float _RotationSpeed;
		[SerializeField] private bool destroyOnGrab;

		private float height;

		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		protected virtual void Start()
		{
			height = transform.position.y;
		}

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		protected virtual void Update()
		{
			transform.position = new Vector3(transform.position.x, height + Mathf.Sin(Time.time * _MoveSpeed) / 1.5f, transform.position.z);
			transform.Rotate(Vector3.up * (_RotationSpeed * Time.deltaTime));
		}

		/// <summary>
		/// OnTriggerEnter is called when the Collider other enters the trigger.
		/// </summary>
		/// <param name="other">The other Collider involved in this collision.</param>
		protected virtual void OnTriggerEnter(Collider collider)
		{
			if (collider.CompareTag("Player") && _UIText != null)
			{
				_UIPanel.gameObject.SetActive(true);
				_UIText.text = message;
				if (grabSound != null && collider.GetComponent<AudioSource>() != null)
					collider.GetComponent<AudioSource>().PlayOneShot(grabSound);
				if (destroyOnGrab)
					Destroy(gameObject);
			}
		}
	}
}