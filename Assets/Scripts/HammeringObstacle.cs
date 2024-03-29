using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;
using DG.Tweening;

public class HammeringObstacle : MonoBehaviour
{
	#region Fields
	[Header( "Fired Events" )]
	public IntGameEvent activateEntityRagdoll;
	public IntGameEvent resetEntityRagdoll;

    [HorizontalLine]
	public Transform rotatePivot;
	public Collider triggerCollider;
	public ColliderListener_EventRaiser colliderListener;


    [Header ("Angles"), HorizontalLine]
    public Vector3 startAngle;
    public Vector3 endAngle;

    [Header("Durations"), HorizontalLine]
    [Tooltip("Hammer down delay. Amount of time to await after the first frame of the Scene")] public float down_Delay;
    [Tooltip("Hammer down duration")] public float down_Duration;
    [Tooltip("Hammer down curve")] public AnimationCurve down_Curve;
    [Tooltip("Hammer returning duration")] public float up_Duration;
    [Tooltip("Hammer returning curve")] public AnimationCurve up_Curve;

    // Private Fields
    Tween hammerSequence;
    #endregion

    #region UnityAPI

    private void OnEnable()
    {
        colliderListener.triggerEnter += TriggerEnterResponse;
    }

    private void OnDisable()
    {
        colliderListener.triggerEnter -= TriggerEnterResponse;

        hammerSequence.Kill();
    }
    private void Awake()
    {
        rotatePivot.localEulerAngles = startAngle;

		DOVirtual.DelayedCall( down_Delay, () => hammerSequence = StartHammerSequence() );
    }
    #endregion

    #region Implementation

    Tween StartHammerSequence()
    {
        var sequence = DOTween.Sequence();

		sequence.AppendCallback( () => triggerCollider.enabled = true );
		sequence.Append(rotatePivot.DOLocalRotate(endAngle, down_Duration).SetEase(down_Curve));
		sequence.AppendCallback( () => triggerCollider.enabled = false );
        sequence.Append(rotatePivot.DOLocalRotate(startAngle, up_Duration).SetEase(up_Curve));

        sequence.SetLoops( -1 );

        return sequence;
    }

    void TriggerEnterResponse(Collider other)
    {
		activateEntityRagdoll.eventValue = other.gameObject.GetInstanceID();
		activateEntityRagdoll.Raise();

		resetEntityRagdoll.eventValue = other.gameObject.GetInstanceID();
		resetEntityRagdoll.Raise();
	}
    #endregion
}
