/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FFStudio;
using NaughtyAttributes;
using UnityEditor;

public class FallingObstacle : MonoBehaviour
{
	#region Fields
	[Header( "Fired Events" )]
	public IntGameEvent activateEntityRagdoll;

	[HorizontalLine]
    [Tooltip("Wait time after collider triggered")]
	public float triggerWaitTime; // Amount of time to fall after collider is triggered
    [Tooltip("Duration of the fall")]
	public float fallDuration;

    [Label("Use custom animation curve")]
	public bool useAnimationCurve = false;
    [ShowIf("useAnimationCurve"), Tooltip("Falling tween ease curve")]
	public AnimationCurve fallCurve;

	// Private fields
	private Transform parentTransform;
	private Collider obstacleCollider;
	private ColliderListener_EventRaiser colliderListener; // Fence that triggers this obstacle
	#endregion

	#region Unity API
    private void OnEnable()
    {
		colliderListener.triggerEnter += OnTrigger;
	}

    private void OnDisable()
    {
		colliderListener.triggerEnter -= OnTrigger;
	}

    private void Awake()
    {
		parentTransform  = transform.parent;
		colliderListener = transform.parent.GetComponent< ColliderListener_EventRaiser >();
		obstacleCollider = GetComponent< Collider >();

		obstacleCollider.isTrigger = true;
	}
    private void OnTriggerEnter( Collider other )
    {
		activateEntityRagdoll.eventValue = other.gameObject.GetInstanceID();
		activateEntityRagdoll.Raise();
	}
	#endregion

	#region API
	#endregion

	#region Implementation
    void OnTrigger( Collider other )
    {
		gameObject.SetActive( true );

		var fallingTween = transform.DOMoveY( parentTransform.position.y, fallDuration );

        if(useAnimationCurve)
			fallingTween.SetEase( fallCurve );

		fallingTween.SetDelay( triggerWaitTime );
		fallingTween.OnComplete( OnFallingStop );
	}

    void OnFallingStop()
    {
		obstacleCollider.isTrigger = false;
	}
	#endregion

#if UNITY_EDITOR
	#region EditorOnly 
	private void OnDrawGizmos()
    {
        if (transform.parent == null) return;

		var position = transform.position;
		position.y = transform.parent.position.y;

		Handles.color = Color.red;
		Handles.DrawSolidDisc( position, Vector3.up, 0.5f );
	}

    private void OnValidate()
    {
        if(transform.parent == null)
			Debug.Log( "Set a parent for falling obstacle" );
	}
    #endregion
#endif
}
