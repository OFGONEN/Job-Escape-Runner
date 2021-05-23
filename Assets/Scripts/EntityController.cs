/* Created by and for usage of FF Studios (2021). */

using System.Linq;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;
using DG.Tweening;

public abstract class EntityController : MonoBehaviour
{
#region Fields
	[ Header( "Event Listeners" ) ]
	[ BoxGroup( "Base Entity Controller Properties" ) ] public EventListenerDelegateResponse activateRagdollListener;
	[ BoxGroup( "Base Entity Controller Properties" ) ] public EventListenerDelegateResponse resetRagdollListener;

	[ HorizontalLine]
	[ BoxGroup( "Waypoint Properties" ) ] public SharedReferenceProperty sourceWaypointsSharedReference;

	/* Protected Fields. */
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] protected Rigidbody topmostRigidbody;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] protected Animator animator;

	/* Private Fields. */
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Transform ragdollBodyTransform;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Collider rotatingBody_Part;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Rigidbody rotatingBody;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Rigidbody[] ragdollRigidbodiesToActivate;

	/* Waypoint. */
	protected Vector3[] waypoints = null;
	protected Transform[] sourceWaypoints = null;
	protected int currentWaypointIndex = 0;
	protected Vector3 GoalWaypoint => waypoints[ currentWaypointIndex ];

	/* Resetting ragdoll. */
	private Rigidbody[] ragdollRigidbodies;
	private TransformInfo[] transformInfos;

	/* Rotation Clamping. */
	private float totalDeltaAngle = 0.0f;
	private float startEulerYAngle;
#endregion

#region Unity API
	protected virtual void OnEnable()
	{
		activateRagdollListener.OnEnable();
		resetRagdollListener.OnEnable();
	}

	protected virtual void OnDisable()
	{
		activateRagdollListener.OnDisable();
	}

	private void OnDestroy()
	{
		resetRagdollListener.OnDisable();
	}

	protected virtual void Awake()
	{
		activateRagdollListener.response = ActivateFullRagdoll;
		resetRagdollListener.response = ResetEntity;

		GetTransformInfos();
	}

	protected virtual void Start()
	{
		totalDeltaAngle = startEulerYAngle = rotatingBody.transform.eulerAngles.y;

		topmostRigidbody.mass = RigidbodyMass();
		topmostRigidbody.drag = RigidbodyDrag();

		sourceWaypoints = ( sourceWaypointsSharedReference.sharedValue as Transform ).GetComponentsInChildren< Transform >();
		waypoints = sourceWaypoints.Where( ( sourceWaypointTransform, index ) => index > 0 ) // Don't include parent.
								   .Select( sourceWaypointTransform => sourceWaypointTransform.position )
								   .ToArray();
	}

	private void FixedUpdate()
	{
		/* All cases regarding input and the value of inputDirection:
		 * [INPUT]			[VALUE OF inputDirection]
		 * Left  						< +1,  0, +1 >
		 * Right 						< -1,  0, +1 >
		 * Both  						<  0,  0, +1 >
		 * None							<  0,  0,  0 > */

		var inputDirection = InputDirection();

		MoveViaPhysics( inputDirection );
		Rotate( inputDirection );

		ClampVelocity();
		ClampAndSetTotalRotationDelta();
	}
#endregion

#region API
#endregion

#region Implementation
	protected void ActivateFullRagdoll()
	{
		if( enabled == false ||
			( activateRagdollListener.gameEvent as IntGameEvent ).eventValue != gameObject.GetInstanceID() )
			return;

		/* Let all children go! */
		rotatingBody.transform.SetParent( null );

		/* Reassemble rotating body. */
		rotatingBody_Part.transform.SetParent( rotatingBody.transform );
		rotatingBody_Part.enabled = true;

		ragdollBodyTransform.SetParent( null );

		/* Disable animator as it is controlling the lower body limbs. */
		animator.enabled = false;

		/* Make rigidbodies of ragdoll elements dynamic to activate them. */
		rotatingBody.isKinematic = false;

		foreach( var rigidbody in ragdollRigidbodiesToActivate )
			rigidbody.isKinematic = false;

		/* Transfer topmost rigidbody's velocity to chair. */
		rotatingBody.velocity = topmostRigidbody.velocity / 3;
		rotatingBody.angularVelocity = topmostRigidbody.angularVelocity / 10;

		/* Completely stop topmost rigidbody as well. */
		topmostRigidbody.velocity = topmostRigidbody.angularVelocity = Vector3.zero;

		/* Disable the component. We are interested in the enabled flag actually. */
		enabled = false;
	}

	protected void ReassembleRagdoll()
	{
		rotatingBody.transform.SetParent( transform );

		rotatingBody_Part.transform.SetParent( transform );
		rotatingBody_Part.enabled = false;

		ragdollBodyTransform.SetParent( rotatingBody.transform );

		animator.enabled = true;

		rotatingBody.isKinematic = true;

		foreach( var rigidbody in ragdollRigidbodiesToActivate )
			rigidbody.isKinematic = true;

		rotatingBody.velocity = Vector3.zero;
		rotatingBody.angularVelocity = Vector3.zero; ;

		topmostRigidbody.velocity = topmostRigidbody.angularVelocity = Vector3.zero;

		enabled = true;

		/* Setting back the values. */
		rotatingBody.SetTransformInfo( transformInfos[ 0 ] );
		rotatingBody_Part.transform.SetTransformInfo( transformInfos[ 1 ] );
		ragdollBodyTransform.SetTransformInfo( transformInfos[ 2 ] );

		rotatingBody.gameObject.SetActive( true );
		rotatingBody_Part.gameObject.SetActive( true );

		for( int i = 0; i < ragdollRigidbodies.Length; i++ )
		{
			ragdollRigidbodies[ i ].SetTransformInfo( transformInfos[ i + 3 ] );
			ragdollRigidbodies[ i ].gameObject.SetActive( true );
		}

		transform.position = GoalWaypoint.SetY( transform.position.y );
	}

	private void ResetEntity()
	{
		if( ( resetRagdollListener.gameEvent as IntGameEvent ).eventValue != gameObject.GetInstanceID() )
			return;

		FFLogger.Log( "Resetting: " + name, gameObject );

		if( gameObject.CompareTag( "Player" ) )
			DOVirtual.DelayedCall( GameSettings.Instance.player.resetWaitTime, ReassembleRagdoll );
		else if( gameObject.CompareTag( "Agent" ) )
			DOVirtual.DelayedCall( GameSettings.Instance.aIAgent.resetWaitTime, ReassembleRagdoll );
	}

	private void GetTransformInfos()
	{
		ragdollRigidbodies = ragdollBodyTransform.GetComponentsInChildren<Rigidbody>();
		transformInfos     = new TransformInfo[ ragdollRigidbodies.Length + 3 ];
					   
		transformInfos[ 0 ] = new TransformInfo( rotatingBody );
		transformInfos[ 1 ] = new TransformInfo( rotatingBody_Part.transform );
		transformInfos[ 2 ] = new TransformInfo( ragdollBodyTransform );

		for( int i = 0; i < ragdollRigidbodies.Length; i++ )
			transformInfos[ i + 3 ] = new TransformInfo( ragdollRigidbodies[ i ] );
	}

	private void Rotate( Vector3 inputDirection )
	{
		totalDeltaAngle += inputDirection.x * GameSettings.Instance.angularSpeed * Time.fixedDeltaTime;
	}

	private void ClampAndSetTotalRotationDelta()
	{
		totalDeltaAngle = Mathf.Clamp( totalDeltaAngle,
									   startEulerYAngle + GameSettings.Instance.angularClamping.x,
									   startEulerYAngle + GameSettings.Instance.angularClamping.y );

		rotatingBody.transform.eulerAngles = rotatingBody.transform.eulerAngles.SetY( totalDeltaAngle );
	}
#endregion

#region Protected API 
	abstract protected Vector3 InputDirection();
	abstract protected float InputCofactor();
	abstract protected float RigidbodyMass();
	abstract protected float RigidbodyDrag();
	abstract protected void MoveViaPhysics( Vector3 inputDirection );

	protected virtual void ClampVelocity()
	{
		var velocity = topmostRigidbody.velocity;
		var velocityMagnitude = velocity.magnitude;

		velocityMagnitude = Mathf.Min( velocityMagnitude, GameSettings.Instance.velocityClamp );

		topmostRigidbody.velocity = velocity.normalized * velocityMagnitude;
	}
#endregion
}