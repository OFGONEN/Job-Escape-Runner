/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class ParallaxEffect_ArbitaryAxis : MonoBehaviour
{
	#region Fields
	public SharedReferenceProperty targetReference; // Players rigidbody
	public Vector3 parallaxRatio_X_Axis;
	public Vector3 parallaxRatio_Y_Axis;
	public Vector3 parallaxRatio_Z_Axis;

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

		var final = startPosition;

		final.x += diff.x * parallaxRatio_X_Axis.x + diff.y * parallaxRatio_X_Axis.y + diff.z * parallaxRatio_X_Axis.z;
		final.y += diff.x * parallaxRatio_Y_Axis.x + diff.y * parallaxRatio_Y_Axis.y + diff.z * parallaxRatio_Y_Axis.z;
		final.z += diff.x * parallaxRatio_Z_Axis.x + diff.y * parallaxRatio_Z_Axis.y + diff.z * parallaxRatio_Z_Axis.z;

		transform.position = final;
	}
	#endregion

	#region API
	#endregion

	#region Implementation
	#endregion
}
