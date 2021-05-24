/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class Test_TransitionRagdoll : MonoBehaviour
{
#region Fields
	public Rigidbody spine;
	public Rigidbody targetSpine;
#endregion

#region Unity API
#endregion

#region API

    [Button]
    public void TransitionRagdoll()
    {
		spine.isKinematic = true;
		spine.useGravity  = false;

		spine.DOMove( targetSpine.position, 1f );
		spine.DORotate( targetSpine.rotation.eulerAngles, 1f );
	}
#endregion

#region Implementation
#endregion
}
