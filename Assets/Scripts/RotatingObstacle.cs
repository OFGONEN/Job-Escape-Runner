using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class RotatingObstacle : MonoBehaviour
{
    #region Fields
    public Transform push_Destination;

    // Private Fields
    private Vector3 push_Direction;
    #endregion

    private void Awake()
    {
        push_Direction = (push_Destination.position - transform.position).normalized;
    }

    #region UnityAPI
    private void OnTriggerEnter( Collider other )
    {
        other.GetComponentInChildren< Rigidbody >().AddForce(push_Direction * GameSettings.Instance.obstacle_rotating_forceToApply);

        // FFLogger.Log( "Pushed:" + other.gameObject );
    }
    #endregion

}
