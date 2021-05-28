/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEditor;
using FFStudio;

public class CameraManager : MonoBehaviour
{
#region Fields
	[ Header("Shared Variables") ]
	public SharedReferenceProperty followTarget_Rigidbody;

	/* Private Fields */
	private Transform followTarget_Transform;
	
	private Vector3 originalDirection;
#endregion

#region Unity API
	private void Start()
	{
		transform.position = transform.position.SetX( 0 );
		originalDirection = Vector3.forward;
	}

	private void OnEnable()
	{
		followTarget_Rigidbody.changeEvent += OnTargetRigidbodyChange;
	}

	private void OnDisable()
	{
		followTarget_Rigidbody.changeEvent -= OnTargetRigidbodyChange;
	}

	private void Update()
	{
		transform.position = transform.position.SetZ( followTarget_Transform.position.z - GameSettings.Instance.camera_followingOffSet );
		transform.LookAtOverTimeAxis( followTarget_Transform.position, Vector3.up, GameSettings.Instance.camera_rotationSpeed );
		ClampHorizontally();
	}
	
#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Handles.color = new Color( 0.5f, 0.5f, 0.5f, 0.5f );

		var arcTotalAngle = Mathf.Abs( GameSettings.Instance.camera_horizontalClamping.x ) +
							Mathf.Abs( GameSettings.Instance.camera_horizontalClamping.y );
		var leftLimit  = Quaternion.AngleAxis( -GameSettings.Instance.camera_horizontalClamping.x, Vector3.up ) * originalDirection;
		var rightLimit = Quaternion.AngleAxis( +GameSettings.Instance.camera_horizontalClamping.x, Vector3.up ) * originalDirection;
		
		Handles.DrawSolidArc( transform.position, Vector3.up, rightLimit, arcTotalAngle, 2.0f );
	}
#endif
#endregion

#region API
#endregion

#region Implementation
	void OnTargetRigidbodyChange()
	{
		followTarget_Transform = ( followTarget_Rigidbody.sharedValue as Rigidbody ).transform;
	}

	void ClampHorizontally()
	{
		/* Find delta angle with the original direction. */
		var deltaAngle = Vector3.SignedAngle( originalDirection, transform.forward.SetY( 0 ), Vector3.up );

		/* Clamp & set that as the new Y angle. */
		var newY = Mathf.Clamp( deltaAngle,
								GameSettings.Instance.camera_horizontalClamping.x, GameSettings.Instance.camera_horizontalClamping.y );
		transform.eulerAngles = transform.eulerAngles.SetY( newY );
	}
#endregion
}
