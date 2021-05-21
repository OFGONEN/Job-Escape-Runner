/* Created by and for usage of FF Studios (2021). */

using System.Linq;
using UnityEngine;
using UnityEditor;
using FFStudio;

public class AgentController : EntityController
{
#region Fields
	public SharedReferenceProperty sourceWaypointsSharedReference;

	private Vector3[] waypoints = null;
	private Transform[] sourceWaypoints = null;

	private int currentWaypointIndex = 0;
	private Vector3 GoalWaypoint => waypoints[ currentWaypointIndex ];

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
		
		waypoints = null;
	}

	protected override void Start()
	{
		gameSettings    = GameSettings.Instance;
		aIAgentSettings = gameSettings.aIAgent;

		base.Start(); // Need to initialize gameSettings related stuff first as they will be used in base.Start().

		speedMultiplier = aIAgentSettings.MoveSpeedAndForceRandomMultiplier();

		sourceWaypoints = ( sourceWaypointsSharedReference.sharedValue as Transform ).GetComponentsInChildren< Transform >();
		waypoints = sourceWaypoints.Where( ( sourceWaypointTransform, index ) => index > 0 )
								   .Select( sourceWaypointTransform => sourceWaypointTransform.position )
								   .ToArray();

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
			Debug.LogWarning( "Waypoints of " + name + " are not set!" );
			return;
		}

		Handles.color = Color.green;

		for( var i = 0; i < waypoints.Length - 1; i++ )
		{
			Handles.DrawWireDisc( waypoints[ i     ], Vector3.up, 1.0f );
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

		if( Vector3.Distance( GoalWaypoint, transform.position ) < aIAgentSettings.waypointArrivalThreshold &&
		    ( currentWaypointIndex + 1 ) < waypoints.Length )
			currentWaypointIndex++;

		return currentInputDirection = ( GoalWaypoint - transform.position ).normalized.SetZ ( 1.0f );
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
			AddBurstForce_OneFrame();
			burstForceCounter = 0;
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
