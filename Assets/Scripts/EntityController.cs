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
	[ BoxGroup( "Base Entity Controller Properties" ) ] public EventListenerDelegateResponse levelStartedListener;
	[ BoxGroup( "Base Entity Controller Properties" ) ] public EventListenerDelegateResponse activateRagdollListener;
	[ BoxGroup( "Base Entity Controller Properties" ) ] public EventListenerDelegateResponse resetRagdollListener;

	[ HorizontalLine]
	[ Header( "Fired Events" ) ]
	public ReferenceGameEvent participateEvent;

	[ HorizontalLine]
	[ BoxGroup( "Waypoint Properties" ) ] public SharedReferenceProperty sourceWaypointsSharedReference;

	[HorizontalLine]
	public EntityInfoLibrary entityInfoLibrary;

	[HorizontalLine] /* Transition */
	public Rigidbody transitionRigidbody;
	public TransitionBodySet transitionBodies;

	/* Protected Fields. */
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] protected Rigidbody topmostRigidbody;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] protected Animator animator;

	/* Private Fields. */
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Transform ragdollBodyTransform;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Collider rotatingBody_Part;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Rigidbody rotatingBody;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Rigidbody[] ragdollRigidbodiesToActivate;

	// Entity Info
	protected UIWorldSpace entityInfoUI;
	protected UIEntityInfo entityInfo;

	/* Waypoint. */
	protected Vector3[] waypoints = null;
	protected Transform[] sourceWaypoints = null;
	protected int currentWaypointIndex = 0;
	protected Vector3 GoalWaypoint => waypoints[ currentWaypointIndex ];
	protected Vector3 PrevWaypoint => waypoints[ Mathf.Max( 0, currentWaypointIndex ) ];

	/* Resetting ragdoll. */
	private Collider entityCollider;
	private Rigidbody[] ragdollRigidbodies;
	private TransformInfo[] transformInfos;

	/* Rotation Clamping. */
	private float totalDeltaAngle = 0.0f;
	private float startEulerYAngle;

	/* Delegates */
	private UnityMessage fixedUpdate;
	protected TweenCallback podiumTransitionDone;

	/* Podium Transition Tweens */
	private Tween moveTween;
	private Tween rotationTween;
	private Tween transitionCallTween;

	/* Rank in the Race */
	[ ReadOnly ] public float finishLineDistance;
	protected int rank;
	public virtual int Rank
	{
		get 
		{
			return rank;
		}

		set 
		{
			rank = value;
		}
	}

	#endregion

	#region Unity API
	protected virtual void OnEnable()
	{
		activateRagdollListener.OnEnable();
		levelStartedListener   .OnEnable();
		resetRagdollListener   .OnEnable();
	}

	protected virtual void OnDisable()
	{
		activateRagdollListener.OnDisable();
		levelStartedListener   .OnDisable();

		if(moveTween != null)
		{
			moveTween.Kill();
			moveTween = null;
		}

		if(rotationTween != null)
		{
			rotationTween.Kill();
			rotationTween = null;
		}

		if(transitionCallTween != null)
		{
			transitionCallTween.Kill();
			transitionCallTween = null;
		}
	}

	private void OnDestroy()
	{
		resetRagdollListener.OnDisable();
	}

	protected virtual void Awake()
	{
		activateRagdollListener.response = ActivateFullRagdoll;
		resetRagdollListener.response    = ResetEntity;
		levelStartedListener.response    = () => fixedUpdate = PhysicMovement;

		entityInfoUI   = GetComponentInChildren< UIWorldSpace >();
		entityCollider = GetComponent< Collider >();

		fixedUpdate          = ExtensionMethods.EmptyMethod;
		podiumTransitionDone = ExtensionMethods.EmptyMethod;

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

		participateEvent.eventValue = this;
		participateEvent.Raise();
	}

	private void FixedUpdate()
	{
		fixedUpdate();
	}

	protected void Update()
	{
		int parameter = 0;
		var inputHorizontal = InputDirection().x;

		if(!Mathf.Approximately(0, inputHorizontal))
			parameter = ( int )Mathf.Sign( inputHorizontal );

		animator.SetInteger( "leg", parameter );
	}
#endregion

#region API
	public void FinishLineCrossed()
	{
		transitionCallTween = DOVirtual.DelayedCall( GameSettings.Instance.finishLineTransitionWaitTime, TransitionToPodium )
		.OnKill( NullTransitionTween );
	}
#endregion

#region Implementation
	private void PhysicMovement()
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
	protected void ActivateFullRagdoll()
	{
		if( enabled == false ||
			( activateRagdollListener.gameEvent as IntGameEvent ).eventValue != gameObject.GetInstanceID() )
			return;

		entityInfoUI.gameObject.SetActive( false );

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

		/* Disable entity collider so that while waiting for a reset another agents would not collide with it */
		entityCollider.enabled = false;

		/* Disable the component. We are interested in the enabled flag actually. */
		enabled = false;
	}

	protected void ReassembleRagdoll()
	{
		entityInfoUI.gameObject.SetActive( true );

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

		entityCollider.enabled = true;

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

		transform.position = PrevWaypoint.SetY( transform.position.y );
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

	protected void TransitionToPodium()
	{
		if(Rank <= 3)
		{
			Transform target;

			transitionBodies.itemDictionary.TryGetValue( Rank - 1, out target );

			if(target != null)
			{
				transitionRigidbody.isKinematic = true;
				transitionRigidbody.useGravity = false;

				transitionRigidbody.DOMove( target.position, GameSettings.Instance.finishLineDistanceThreshold )
				.OnComplete( podiumTransitionDone )
				.OnKill( NullMoveTween );

				transitionRigidbody.DORotate( target.rotation.eulerAngles, GameSettings.Instance.finishLineDistanceThreshold )
				.OnKill( NullRotationTween );
			}
		}
		else
		{
			DOVirtual.DelayedCall( GameSettings.Instance.finishLineTransitionWaitTime, podiumTransitionDone );
		}
	}

	private void NullRotationTween()
	{
		rotationTween = null;
	}

	private void NullMoveTween()
	{
		moveTween = null;
	}

	private void NullTransitionTween()
	{
		transitionCallTween = null;
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