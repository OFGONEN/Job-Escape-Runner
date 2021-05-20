/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public abstract class EntityController : MonoBehaviour
{
#region Fields
	[ BoxGroup( "Base Entity Controller Properties" ), Header( "Event Listeners" ) ]
	public EventListenerDelegateResponse activateRagdollListener;

	[ HorizontalLine ]

    /* Protected Fields. */
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] protected Rigidbody  topmostRigidbody;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] protected Animator   animator;
    
    /* Private Fields. */
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Transform   ragdollBodyTransform;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Collider    rotatingBody_Part;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Rigidbody   rotatingBody;
	[ BoxGroup( "Base Entity Controller Properties" ), SerializeField ] private Rigidbody[] ragdollRigidbodiesToActivate;

	private float totalDeltaAngle = 0.0f;
	private float startEulerYAngle;
#endregion

#region Unity API
	protected virtual void OnEnable()
	{
		activateRagdollListener.OnEnable();
	}

	protected virtual void OnDisable()
	{
		activateRagdollListener.OnDisable();
	}
	
	protected virtual void Awake()
	{
		activateRagdollListener.response = ActivateFullRagdoll;
	}
	
	private void Start()
	{
		totalDeltaAngle = startEulerYAngle = rotatingBody.transform.eulerAngles.y;

		topmostRigidbody.mass = GameSettings.Instance.player.rigidBody_Mass;
		topmostRigidbody.drag = GameSettings.Instance.player.rigidBody_Drag;
	}
	
	private void FixedUpdate()
    {
		/* All cases regarding input and the value of inputDirection:
		 * [INPUT]			[VALUE OF inputDirection]
		 * Left  						< +1,  0, +1 >
		 * Right 						< -1,  0, +1 >
		 * Both  						<  0,  0, +1 >
		 * None							<  0,  0,  0 > */

		var input = InputSource();

		topmostRigidbody.AddForce( input * GameSettings.Instance.player.force * InputCofactor() * Time.fixedDeltaTime );

		totalDeltaAngle += input.x * GameSettings.Instance.player.angularSpeed * Time.fixedDeltaTime;

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
		if( enabled == false || 
            ( activateRagdollListener.gameEvent as IntGameEvent ).eventValue != gameObject.GetInstanceID() )
			return;
			
		/* Let all children go! */
		rotatingBody.transform.SetParent( null );
		
		/* Reassemble rotating body. */
		rotatingBody_Part.transform.SetParent( rotatingBody.transform );
		rotatingBody_Part.enabled = true;

		ragdollBodyTransform.SetParent( null );

		/* Make rigidbodies of ragdoll elements dynamic to activate them. */
		rotatingBody.isKinematic = false;
		foreach( var rigidbody in ragdollRigidbodiesToActivate )
			rigidbody.isKinematic = false;

		/* Transfer players velocity to chair. */
		rotatingBody.velocity        = topmostRigidbody.velocity / 3;
		rotatingBody.angularVelocity = topmostRigidbody.angularVelocity / 10;

		/* Completely stop player rigidbody as well. */
		topmostRigidbody.velocity = topmostRigidbody.angularVelocity = Vector3.zero;

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
		var velocity = topmostRigidbody.velocity;
		var velocityMagnitude = velocity.magnitude;

		velocityMagnitude = Mathf.Min( velocityMagnitude, GameSettings.Instance.player.velocityClamp );

		topmostRigidbody.velocity = velocity.normalized * velocityMagnitude;
	}
#endregion

#region Protected API 
	abstract protected Vector3 InputSource();
	abstract protected float   InputCofactor();
#endregion
}
