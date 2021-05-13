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
	public EventListenerDelegateResponse tapInputListener;

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
		tapInputListener.OnEnable();

		playerRigidbodyReference.SetValue( playerRigidbody );
	}

	private void OnDisable()
	{
		activateRagdollListener.OnDisable();
		screenTapListener      .OnDisable();
		tapInputListener.OnDisable();

		playerRigidbodyReference.SetValue( null );

		FFLogger.Log( "PlayerController disabled" );
	}
	
	private void Awake()
	{
		activateRagdollListener.response = ActivateFullRagdoll;
		screenTapListener.response 		 = ScreenTapResponse;
		tapInputListener.response = TapInputListener;
	}
	private void Start()
	{
		totalDeltaAngle = startEulerYAngle = rotatingBody.transform.eulerAngles.y;
	}

	private void FixedUpdate() 
	{
		ClampVelocity();
		ClampAndSetTotalRotationDelta();
	}
	
	private void TapInputListener()
    {
		/* All cases regarding input and the value of inputDirection:
		 * [INPUT]			[VALUE OF inputDirection]
		 * Left  						< +1,  0, +1 >
		 * Right 						< -1,  0, +1 >
		 * Both  						<  0,  0, +1 >
		 * None							<  0,  0,  0 > */

		var changeEvent = tapInputListener.gameEvent as Vector2GameEvent;
		var input = new Vector3( changeEvent.eventValue.x, 0, changeEvent.eventValue.y );
		playerRigidbody.AddForce( input * GameSettings.Instance.player.force * Time.fixedDeltaTime, ForceMode.Force );

		totalDeltaAngle += input.x * GameSettings.Instance.player.angularSpeed * Time.fixedDeltaTime;
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
		var velocity_magnitude = velocity.magnitude;

		FFLogger.Log( "Velocity:" + velocity_magnitude );

		velocity_magnitude = Mathf.Min( velocity_magnitude, GameSettings.Instance.player.maxVelocity );

		playerRigidbody.velocity = velocity.normalized * velocity_magnitude;
	}

	private void ScreenTapResponse()
	{
		var changeEvent = screenTapListener.gameEvent as StringGameEvent;
		animator.SetTrigger( changeEvent.eventValue );
	}
#endregion
}
