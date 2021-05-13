/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class PlayerController : MonoBehaviour
{
	#region Fields
	[Header( "Event Listeners" )]
	public EventListenerDelegateResponse activateRagdollListener;
	public EventListenerDelegateResponse screenTapListener;

	[HorizontalLine]

	[Header("Shared Variables")]
	public SharedVector3 inputDirection;
	public SharedReferenceProperty playerRigidbodyReference;

	[HorizontalLine]

	[ SerializeField ] private Animator animator;
	[ SerializeField ] private Rigidbody playerRigidbody;
	[ SerializeField ] private Rigidbody rotatingBody;
	[ SerializeField ] private Rigidbody[] ragdollRigidbodiesToActivate;

	private float totalDeltaAngle = 0.0f;
	private float startEulerYAngle;
#endregion

#region Unity API
	private void OnEnable()
	{
		activateRagdollListener.OnEnable();
		screenTapListener      .OnEnable();

		playerRigidbodyReference.SetValue( playerRigidbody );
	}

	private void OnDisable()
	{
		activateRagdollListener.OnDisable();
		screenTapListener      .OnDisable();

		playerRigidbodyReference.SetValue( null );

		FFLogger.Log( "PlayerController disabled" );
	}
	
	private void Awake()
	{
		activateRagdollListener.response = ActivateFullRagdoll;
		screenTapListener.response 		 = ScreenTapResponse;
	}
	private void Start()
	{
		totalDeltaAngle = startEulerYAngle = rotatingBody.transform.eulerAngles.y;

		playerRigidbody.mass = GameSettings.Instance.player.rigidBody_Mass;
		playerRigidbody.drag = GameSettings.Instance.player.rigidBody_Drag;
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

		ClampVelocity();
		ClampAndSetTotalRotationDelta();
	}
#endregion

#region API

#endregion

#region Implementation
	[ Button() ]
	private void ActivateFullRagdoll()
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
	private void ClampAndSetTotalRotationDelta()
	{
		totalDeltaAngle = Mathf.Clamp( totalDeltaAngle,
									   startEulerYAngle + GameSettings.Instance.player.angularClamping.x,
									   startEulerYAngle + GameSettings.Instance.player.angularClamping.y );

		rotatingBody.transform.eulerAngles = rotatingBody.transform.eulerAngles.SetY( totalDeltaAngle );
	}

	private void ClampVelocity()
	{
		var velocity = playerRigidbody.velocity;
		var magnitude = velocity.magnitude;

		magnitude = Mathf.Min( magnitude, GameSettings.Instance.player.velocityClamp );

		playerRigidbody.velocity = velocity.normalized * magnitude;
	}

	private void ScreenTapResponse()
	{
		var changeEvent = screenTapListener.gameEvent as StringGameEvent;
		animator.SetTrigger( changeEvent.eventValue );
	}
#endregion
}
