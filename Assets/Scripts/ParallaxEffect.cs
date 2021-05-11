/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class ParallaxEffect : MonoBehaviour
{
	#region Fields
	public SharedReferenceProperty targetReference; // Players rigidbody
	public Vector3 parallaxAxis;

	// Private Fields
	private Transform targetTransform;
	private Vector3 startPosition;
	private Vector3 target_StartPosition;
	#endregion

	#region Unity API
    private void Start()
    {
		targetTransform = (targetReference.sharedValue as Rigidbody).transform;

		startPosition = transform.position;
		target_StartPosition = targetTransform.position;
	}

    private void Update()
    {
		var diff = targetTransform.position - target_StartPosition;

		diff.x *= parallaxAxis.x;
		diff.y *= parallaxAxis.y;
		diff.z *= parallaxAxis.z;

		transform.position = startPosition + diff;
	}
	#endregion

	#region API
	#endregion

	#region Implementation
	#endregion
}
