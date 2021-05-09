/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class PlayerController : MonoBehaviour
{
#region Fields
	public float force = 10.0f;

	public float angularSpeed = 10.0f;
	[ MinMaxSlider( -90, +90 ) ]
	public Vector2 angularClamping;

	[ SerializeField ] private Rigidbody movingRigidbody;
	[ SerializeField ] private Transform rotatingBody;

	private Vector3 Right	  			=>  movingRigidbody.transform.right;
	private Vector3 Left  	  			=> -movingRigidbody.transform.right;
	private Vector3 Forward 			=>  movingRigidbody.transform.forward;
	private Vector3 ForwardLeft  		=> (  Left + Forward ) / 2;
	private Vector3 ForwardRight 		=> ( Right + Forward ) / 2;
	
	private float totalDeltaAngle = 0.0f;
	private float startEulerYAngle;
	
	private float eulerYAngleBeforeCollision;
#endregion

#region Unity API
	private void Start()
	{
		startEulerYAngle = rotatingBody.rotation.y;
	}

	private void FixedUpdate()
    {
		// TODO: Read input from actual shared Vector2.
		
		if( Input.GetKey( KeyCode.LeftArrow ) )
		{
			movingRigidbody.AddForce( ForwardRight * force * Time.fixedDeltaTime, ForceMode.Force );

			totalDeltaAngle += angularSpeed * Time.fixedDeltaTime;
		}
		
		if( Input.GetKey( KeyCode.RightArrow ) )
		{
			movingRigidbody.AddForce( ForwardLeft * force * Time.fixedDeltaTime, ForceMode.Force );

			totalDeltaAngle -= angularSpeed * Time.fixedDeltaTime;
		}

		ClampAndSetTotalRotationDelta();
	}
#endregion

#region API
#endregion

#region Implementation
	private void ClampAndSetTotalRotationDelta()
	{
		totalDeltaAngle = Mathf.Clamp( totalDeltaAngle, angularClamping.x, angularClamping.y );

		rotatingBody.transform.eulerAngles = rotatingBody.transform.eulerAngles.SetY( startEulerYAngle + totalDeltaAngle );
	}
#endregion
}
