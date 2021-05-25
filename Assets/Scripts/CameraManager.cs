/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class CameraManager : MonoBehaviour
{
#region Fields
    [ Header("Shared Variables") ]
    public SharedReferenceProperty followTarget_Rigidbody;

    // Private Fields
    private Transform followTarget_Transform;
#endregion

#region Unity API
    private void OnEnable()
    {
		followTarget_Rigidbody.changeEvent += OnTargetRigidbodyChange;
	}

    private void OnDisable()
    {
		followTarget_Rigidbody.changeEvent -= OnTargetRigidbodyChange;
	}

    private void Update()
    {
		transform.position = transform.position.SetZ( followTarget_Transform.position.z - GameSettings.Instance.camera_followingOffSet );
		transform.LookAtOverTimeAxis( followTarget_Transform.position, Vector3.up, GameSettings.Instance.camera_rotationSpeed );

    }
#endregion

#region API
#endregion

#region Implementation
    void OnTargetRigidbodyChange()
    {
		followTarget_Transform = ( followTarget_Rigidbody.sharedValue as Rigidbody ).transform;
	}
#endregion
}
