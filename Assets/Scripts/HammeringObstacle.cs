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
	public GameEvent levelFailEvent;

    [HorizontalLine]
	public Transform rotatePivot;
    public ColliderListener_EventRaiser colliderListener;


    [Header ("Angles"), HorizontalLine]
    public Vector3 startAngle;
    public Vector3 endAngle;

    [Header("Durations"), HorizontalLine]
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
        rotatePivot.eulerAngles = startAngle;

        hammerSequence = StartHammerSequence();
    }
    #endregion

    #region Implementation

    Tween StartHammerSequence()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(rotatePivot.DORotate(endAngle, down_Duration).SetEase(down_Curve));
        sequence.Append(rotatePivot.DORotate(startAngle, up_Duration).SetEase(up_Curve));

        sequence.SetLoops( -1 );

        return sequence;
    }

    void TriggerEnterResponse(Collider other)
    {
        var controller = other.GetComponent<PlayerController>();

        if(controller)
            controller.ActivateFullRagdoll();
    }
    #endregion
}
