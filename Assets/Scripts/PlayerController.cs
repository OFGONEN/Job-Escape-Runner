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
	public SharedReferenceProperty playerRigidbodyReference;
#endregion

#region Unity API
	protected override void OnEnable()
	{
		base.OnEnable();

		screenTapListener.OnEnable();

		playerRigidbodyReference.SetValue( topmostRigidbody );
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		
		screenTapListener.OnDisable();

		playerRigidbodyReference.SetValue( null );

		FFLogger.Log( "PlayerController disabled" );
	}

	protected override void Awake()
	{
		base.Awake();

		screenTapListener.response = ScreenTapResponse;
	}
#endregion

#region API
#endregion

#region Implementation
	private void ScreenTapResponse()
	{
		var changeEvent = screenTapListener.gameEvent as StringGameEvent;
		animator.SetTrigger( changeEvent.eventValue );
	}
#endregion

#region EntityController Overrides
	protected override Vector3 InputSource()
	{
		return inputDirection.sharedValue;
	}

	protected override float InputCofactor()
	{
		return input_cofactor.sharedValue;
	}
#endregion
}
