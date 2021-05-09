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

	[ SerializeField ] private Rigidbody playerRigidbody;
	[ SerializeField ] private Rigidbody rotatingBody;
	[ SerializeField ] private Rigidbody[] ragdollRigidbodiesToActivate;

	private Vector3 Right	  			=>  playerRigidbody.transform.right;
	private Vector3 Left  	  			=> -playerRigidbody.transform.right;
	private Vector3 Forward 			=>  playerRigidbody.transform.forward;
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
			playerRigidbody.AddForce( ForwardRight * force * Time.fixedDeltaTime, ForceMode.Force );

			totalDeltaAngle += angularSpeed * Time.fixedDeltaTime;
		}
		
		if( Input.GetKey( KeyCode.RightArrow ) )
		{
			playerRigidbody.AddForce( ForwardLeft * force * Time.fixedDeltaTime, ForceMode.Force );

			totalDeltaAngle -= angularSpeed * Time.fixedDeltaTime;
		}

		ClampAndSetTotalRotationDelta();
	}
#endregion

#region API
	[ Button() ]
	public void ActivateFullRagdoll()
	{
		/* Let all children go! */
		rotatingBody.transform.SetParent( null );
		rotatingBody.transform.GetChild( 0 ).SetParent( null );
		
		/* Make rigidbodies of ragdoll elements dynamic to activate them. */
		rotatingBody.isKinematic = false;
		
		foreach( var rigidbody in ragdollRigidbodiesToActivate )
			rigidbody.isKinematic = false;

		/* Transfer players velocity to chair. */
		rotatingBody.velocity        = playerRigidbody.velocity / 3;
		rotatingBody.angularVelocity = playerRigidbody.angularVelocity / 10;

		/* Completely stop player rigidbody as well. */
		playerRigidbody.velocity = playerRigidbody.angularVelocity = Vector3.zero;
	}
#endregion

#region Implementation
	private void ClampAndSetTotalRotationDelta()
	{
		totalDeltaAngle = Mathf.Clamp( totalDeltaAngle, angularClamping.x, angularClamping.y );

		rotatingBody.transform.eulerAngles = rotatingBody.transform.eulerAngles.SetY( startEulerYAngle + totalDeltaAngle );
	}
#endregion
}
