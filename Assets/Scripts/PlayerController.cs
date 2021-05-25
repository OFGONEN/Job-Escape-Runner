/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class PlayerController : EntityController
{
#region Fields
	[ Header( "Event Listeners" ) ]
	public EventListenerDelegateResponse screenTapListener;

	[ HorizontalLine ]

	[ Header( "Shared Variables" ) ]
	public SharedVector3 inputDirection;
	[ Label( "Input Cofactor" ) ]
	public SharedFloatPropertyTweener input_cofactor;

	private Vector3 currentInputDirection;
#endregion

#region Unity API
	protected override void OnEnable()
	{
		base.OnEnable();

		screenTapListener.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		screenTapListener.OnDisable();

		FFLogger.Log( "PlayerController disabled" );
	}

	protected override void Awake()
	{
		base.Awake();

		screenTapListener.response = ScreenTapResponse;

		// Set entity info for world space UI
		entityInfoUI.entityName.text = "Player";
		entityInfoUI.entityFlag.sprite = null;
		entityInfoUI.entityFlag.enabled = false;
	}

	protected override void Start()
	{
		base.Start();
	}
#endregion

#region API
#endregion

#region Implementation
	private void ScreenTapResponse()
	{
		var changeEvent = screenTapListener.gameEvent as BoolGameEvent;
		animator.SetBool( "isInputActive", changeEvent.eventValue );
	}
#endregion

#region EntityController Overrides
	protected override Vector3 InputDirection()
	{
		/* Currently assuming: Levels always progress on +Z: Code below will barf on U-turns etc. */

		/* Player does not use the waypoints as a guide, as it is controlled by the user directly, via mobile input.
		 * But the waypoints are still updated, because current waypoint is used when resetting the player upon fail. */

		if( Mathf.Abs( GoalWaypoint.z - transform.position.z ) < GameSettings.Instance.player.waypointArrivalThreshold &&
			( currentWaypointIndex + 1 ) < waypoints.Length )
			currentWaypointIndex++;

		return currentInputDirection = inputDirection.sharedValue;
	}

	protected override float InputCofactor()
	{
		return input_cofactor.sharedValue;
	}

	protected override float RigidbodyMass()
	{
		return GameSettings.Instance.player.rigidBody_Mass;
	}

	protected override float RigidbodyDrag()
	{
		return GameSettings.Instance.player.rigidBody_Drag;
	}

	protected override void MoveViaPhysics( Vector3 inputDirection )
	{
		topmostRigidbody.AddForce( inputDirection * GameSettings.Instance.player.force * InputCofactor() * Time.fixedDeltaTime );
	}
#endregion
}
