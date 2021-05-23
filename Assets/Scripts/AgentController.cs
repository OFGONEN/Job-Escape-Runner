/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEditor;
using FFStudio;

public class AgentController : EntityController
{
#region Fields
	private Vector3 currentInputDirection;

	private GameSettings.AIAgentSettings aIAgentSettings;
	private GameSettings gameSettings;
	
	private float speedMultiplier;
	private float burstForceCounter = 0.0f;
#endregion

#region Unity API
	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void Start()
	{
		gameSettings    = GameSettings.Instance;
		aIAgentSettings = gameSettings.aIAgent;

		base.Start(); // Need to initialize gameSettings related stuff first as they will be used in base.Start().

		speedMultiplier = aIAgentSettings.MoveSpeedAndForceRandomMultiplier();

		var currentLevelData = CurrentLevelData.Instance.levelData;
		
		for( var i = 0; i < waypoints.Length; i++ )
			waypoints[ i ] = sourceWaypoints[ i + 1 ].position +
							 sourceWaypoints[ i + 1 ].right * currentLevelData.RandomHorizontalWaypointOffset();
	}

#if UNITY_EDITOR // Required when Handles API is used, as it causes issues on build.
	private void OnDrawGizmosSelected()
	{
		if( waypoints == null )
			return;

		if( waypoints.Length == 0 && Application.isPlaying )
		{
			FFLogger.LogError( "Waypoints of " + name + " are not set!", gameObject );
			return;
		}

		Handles.color = Color.green;

		for( var i = 0; i < waypoints.Length - 1; i++ )
		{
			if( i == currentWaypointIndex )
				Handles.DrawSolidDisc( waypoints[ i ], Vector3.up, 1.0f );
			else
				Handles.DrawWireDisc( waypoints[ i ], Vector3.up, 1.0f );
				
			Handles.DrawWireDisc( waypoints[ i + 1 ], Vector3.up, 1.0f );

			Handles.DrawDottedLine( waypoints[ i ], waypoints[ i + 1 ], 10.0f );
		}
	}
#endif
#endregion

#region API
#endregion

#region EntityController Overrides
	protected override Vector3 InputDirection()
	{
		/* Currently assuming: Levels always progress on +Z: Code below will barf on U-turns etc. */

		if( Mathf.Abs( GoalWaypoint.z - transform.position.z ) < aIAgentSettings.waypointArrivalThreshold &&
		    ( currentWaypointIndex + 1 ) < waypoints.Length )
			currentWaypointIndex++;
		
		currentInputDirection = ( GoalWaypoint - transform.position ).normalized.SetZ ( 1.0f );

		if(Mathf.Abs(currentInputDirection.x) <= GameSettings.Instance.deadZoneThreshold / 100)
			currentInputDirection.x = 0;

		return currentInputDirection;
	}

	protected override float InputCofactor()
	{
		return aIAgentSettings.inputHorizontalCofactor;
	}

	protected override float RigidbodyMass()
	{
		return aIAgentSettings.rigidBody_Mass;
	}

	protected override float RigidbodyDrag()
	{
		return aIAgentSettings.rigidBody_Drag;
	}

	protected override void ClampVelocity()
	{
		var velocity          = topmostRigidbody.velocity;
		var velocityMagnitude = velocity.magnitude;

		velocityMagnitude = Mathf.Min( velocityMagnitude, gameSettings.velocityClamp * speedMultiplier );

		topmostRigidbody.velocity = velocity.normalized * velocityMagnitude;
	}

	protected override void MoveViaPhysics( Vector3 inputDirection )
	{
		/* AI agents apply force as one-frame bursts, on regular intervals. */
		burstForceCounter += Time.fixedDeltaTime;

		if( burstForceCounter > aIAgentSettings.forceBurstCooldown )
		{
			animator.SetBool( "isInputActive", true );
			AddBurstForce_OneFrame();
			burstForceCounter = 0;
		}
		else 
		{
			animator.SetBool( "isInputActive", false );
		}
	}
#endregion

#region Implementation
	private void AddBurstForce_OneFrame()
	{
		topmostRigidbody.AddForce( currentInputDirection * aIAgentSettings.force * InputCofactor() );
	}
#endregion
}
