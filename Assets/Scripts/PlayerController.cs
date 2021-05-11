/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class PlayerController : MonoBehaviour
{
#region Fields
	public SharedVector3 inputDirection;

	[ SerializeField ] private Rigidbody playerRigidbody;
	[ SerializeField ] private Rigidbody rotatingBody;
	[ SerializeField ] private Rigidbody[] ragdollRigidbodiesToActivate;

	private float totalDeltaAngle = 0.0f;
	private float startEulerYAngle;
#endregion

#region Unity API
	private void Start()
	{
		startEulerYAngle = rotatingBody.rotation.y;
	}
	
	private void FixedUpdate()
    {
		/* All cases regarding input and the value of inputDirection:
		 * [INPUT]			[VALUE OF inputDirection]
		 * Left  						< +1,  0, +1 >
		 * Right 						< -1,  0, +1 >
		 * Both  						<  0,  0, +1 >
		 * None							<  0,  0,  0 > */
		
		playerRigidbody.AddForce( inputDirection.sharedValue * GameSettings.Instance.player.force * Time.fixedDeltaTime, ForceMode.Force );

		totalDeltaAngle += inputDirection.sharedValue.x * GameSettings.Instance.player.angularSpeed * Time.fixedDeltaTime;
		
		ClampAndSetTotalRotationDelta();
	}
#endregion

#region API
	[ Button() ]
	public void ActivateFullRagdoll()
	{
		if( enabled == false )
			return;

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

		/* Disable the component. We are interested in the enabled flag actually. */
		enabled = false;
	}
#endregion

#region Implementation
	private void ClampAndSetTotalRotationDelta()
	{
		totalDeltaAngle = Mathf.Clamp( totalDeltaAngle,
									   GameSettings.Instance.player.angularClamping.x,
									   GameSettings.Instance.player.angularClamping.y );

		rotatingBody.transform.eulerAngles = rotatingBody.transform.eulerAngles.SetY( startEulerYAngle + totalDeltaAngle );
	}
#endregion
}
